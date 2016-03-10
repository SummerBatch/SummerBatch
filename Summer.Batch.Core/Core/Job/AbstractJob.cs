//
//   Copyright 2015 Blu Age Corporation - Plano, Texas
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

//   This file has been modified.
//   Original copyright notice :

/*
 * Copyright 2006-2013 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using NLog;
using Summer.Batch.Core.Launch;
using Summer.Batch.Core.Launch.Support;
using Summer.Batch.Core.Listener;
using Summer.Batch.Core.Repository;
using Summer.Batch.Core.Scope.Context;
using Summer.Batch.Core.Step;
using Summer.Batch.Infrastructure.Repeat;
using System;
using System.Collections.Generic;
using System.Linq;
using Summer.Batch.Common.Factory;
using Summer.Batch.Common.Util;

namespace Summer.Batch.Core.Job
{
    /// <summary>
    /// Abstract implementation of the <see cref="IJob"/> interface. Common dependencies
    /// such as a <see cref="IJobRepository"/>, <see cref="IJobExecutionListener"/>'s, and various
    /// configuration parameters are set here. Therefore, common error handling and
    /// listener calling activities are abstracted away from implementations.
    /// </summary>
    public abstract class AbstractJob : IJob, IStepLocator, IInitializationPostOperations
    {
        /// <summary>
        /// Logger.
        /// </summary>
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Name property.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Restartable flag property.
        /// </summary>
        public bool Restartable { get; set; }

        /// <summary>
        /// job parameters incrementer property.
        /// </summary>
        public IJobParametersIncrementer JobParametersIncrementer { get; set; }
        private IJobParametersValidator _jobParametersValidator = new DefaultJobParametersValidator();

        /// <summary>
        /// Job parameters validator property.
        /// </summary>
        public IJobParametersValidator JobParametersValidator
        {
            get { return _jobParametersValidator; }
            set { _jobParametersValidator = value; }
        }


        private IJobRepository _jobRepository;

        /// <summary>
        /// Job repository property.
        /// </summary>
        public IJobRepository JobRepository
        {
            get { return _jobRepository; }
            set
            {
                _jobRepository = value;
                _stepHandler = new SimpleStepHandler(_jobRepository);
            }
        }

        private readonly CompositeJobExecutionListener _listener = new CompositeJobExecutionListener();

        private IStepHandler _stepHandler;

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected AbstractJob()
        {
            Restartable = true;
        }

        /// <summary>
        /// Custom constructor with a name.
        /// </summary>
        /// <param name="name"></param>
        protected AbstractJob(string name)
        {
            Name = name;
            Restartable = true;
        }

        private void UpdateStatus(JobExecution jobExecution, BatchStatus status)
        {
            jobExecution.Status = status;
            JobRepository.Update(jobExecution);
        }

        /// <summary>
        /// Actual job execution.
        /// To be implemented by sub-classes.
        /// </summary>
        /// <param name="execution"></param>
        /// <exception cref="JobExecutionException">&nbsp;</exception>
        protected abstract void DoExecute(JobExecution execution);


        /// <summary>
        /// Convenience method for subclasses to delegate the handling of a specific
        /// step in the context of the current <see cref="JobExecution"/> . Clients of this
        /// method do not need access to the <see cref="JobRepository"/>, nor do they need
        /// to worry about populating the execution context on a restart, nor
        /// detecting the interrupted state (in job or step execution).
        /// </summary>
        /// <param name="step">the step to execute</param>
        /// <param name="execution">the current job execution</param>
        /// <returns></returns>
        /// <exception cref="JobInterruptedException">&nbsp;</exception>
        /// <exception cref="JobRestartException">&nbsp;</exception>
        /// <exception cref="StartLimitExceededException">&nbsp;</exception>
        protected StepExecution HandleStep(IStep step, JobExecution execution)
        {
            return _stepHandler.HandleStep(step, execution);
        }

        /// <summary>
        /// Executes job.
        /// </summary>
        /// <param name="execution"></param>
        public void Execute(JobExecution execution)
        {
            Logger.Debug("Job execution starting: {0}", execution.JobInstance.JobName);
            JobSynchronizationManager.Register(execution);
            try
            {
                JobParametersValidator.Validate(execution.JobParameters);
                if (execution.Status != BatchStatus.Stopping)
                {
                    HandleExecution(execution);
                }
                else
                {
                    execution.Status = BatchStatus.Stopped;
                    execution.ExitStatus = ExitStatus.Completed;
                    Logger.Debug("Job execution was stopped: {0}", execution.JobInstance.JobName);
                }
            }
            catch (JobInterruptedException e)
            {
                HandleJobInterruptedException(execution, e);
            }
            catch (Exception t)
            {
                HandleException(execution, t);
            }
            finally
            {
                HandlePostExecution(execution);
            }
        }

        /// <summary>
        /// Call to listeners that might add some post execution behaviour.
        /// </summary>
        /// <param name="execution"></param>
        private void HandlePostExecution(JobExecution execution)
        {
            try
            {
                if (execution.Status.IsLessThanOrEqualTo(BatchStatus.Stopped)
                    && !execution.StepExecutions.Any())
                {
                    ExitStatus exitStatus = execution.ExitStatus;
                    ExitStatus newExitStatus =
                        ExitStatus.Noop.AddExitDescription("All steps already completed or no steps configured for this job.");
                    execution.ExitStatus = exitStatus.And(newExitStatus);
                }
                execution.EndTime = DateTime.Now;

                try
                {
                    _listener.AfterJob(execution);
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Exception encountered in afterStep callback");
                }

                JobRepository.Update(execution);
                Logger.Debug("Current job execution: {0}", execution);
            }
            finally
            {
                JobSynchronizationManager.Release();
            }
        }

        /// <summary>
        /// Computes exit status depending on exception.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="execution"></param>
        /// <returns></returns>
        protected ExitStatus GetDefaultExitStatusForFailure(Exception ex, JobExecution execution)
        {
            ExitStatus exitStatus;
            if (ex is JobInterruptedException
                    || ex.InnerException is JobInterruptedException)
            {
                exitStatus = ExitStatus.Stopped.AddExitDescription("JobInterruptedException");
            }
            else if (ex is NoSuchJobException
                  || ex.InnerException is NoSuchJobException)
            {
                exitStatus = new ExitStatus(ExitCodeMapperConstants.NoSuchJob, ex.GetType().Name);
            }
            else
            {
                exitStatus = ExitStatus.Failed.AddExitDescription(ex);
            }

            return exitStatus;
        }

        /// <summary>
        /// Exception handling.
        /// </summary>
        /// <param name="execution"></param>
        /// <param name="e"></param>
        private void HandleException(JobExecution execution, Exception e)
        {
            Logger.Error(e, "Encountered fatal error executing job");
            execution.ExitStatus = GetDefaultExitStatusForFailure(e, execution);
            execution.Status = BatchStatus.Failed;
            execution.AddFailureException(e);
        }

        /// <summary>
        /// Job interruption handling.
        /// </summary>
        /// <param name="execution"></param>
        /// <param name="e"></param>
        private void HandleJobInterruptedException(JobExecution execution, JobInterruptedException e)
        {
            Logger.Info("Encountered interruption executing job: " + e.Message);
            Logger.Debug(e, "Full exception");
            execution.ExitStatus = GetDefaultExitStatusForFailure(e, execution);
            execution.Status = BatchStatus.Max(BatchStatus.Stopped, e.Status);
            execution.AddFailureException(e);
        }

        /// <summary>
        /// Actual job execution. Delegates to DoExecute.
        /// </summary>
        /// <param name="execution"></param>
        private void HandleExecution(JobExecution execution)
        {
            execution.StartTime = DateTime.Now;
            UpdateStatus(execution, BatchStatus.Started);
            _listener.BeforeJob(execution);
            try
            {
                Logger.Debug("Current job execution:  {0}", execution);
                DoExecute(execution);
                Logger.Debug("Job execution complete: {0}", execution.JobInstance.JobName);
            }
            catch (RepeatException e)
            {
                throw e.InnerException;
            }
        }

        /// <summary>
        /// To be overriden by subclasses.
        /// </summary>
        /// <returns></returns>
        public abstract ICollection<string> GetStepNames();

        /// <summary>
        /// To be overriden by subclasses.
        /// </summary>
        /// <param name="stepName"></param>
        /// <returns></returns>
        public abstract IStep GetStep(string stepName);

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}:[ name={1}]", GetType().Name, Name);
        }

        /// <summary>
        /// Post-init. checks.
        /// @see IInitializationPostOperations#AfterPropertiesSet.
        /// </summary>
        public void AfterPropertiesSet()
        {
            Assert.NotNull(_jobRepository, "JobRepository must be set");
        }

        /// <summary>
        /// Public setter for injecting <see cref="IJobExecutionListener"/>s.
        /// They will all  be given the listener callbacks at the appropriate point in the job.
        /// </summary>
        /// <param name="listeners"></param>
        public void SetJobExecutionListeners(IJobExecutionListener[] listeners)
        {
            foreach (IJobExecutionListener jobExecutionListener in listeners)
            {
                _listener.Register(jobExecutionListener);
            }
        }


        /// <summary>
        /// Register a single listener for the <see cref="IJobExecutionListener"/> callbacks.
        /// </summary>
        /// <param name="listener"></param>
        public void RegisterJobExecutionListener(IJobExecutionListener listener)
        {
            _listener.Register(listener);
        }
    }
}
