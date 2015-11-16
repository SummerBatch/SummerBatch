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
using Summer.Batch.Infrastructure.Item;
using Summer.Batch.Infrastructure.Repeat;
using Summer.Batch.Common.Util;
using System;
using Summer.Batch.Common.Factory;

namespace Summer.Batch.Core.Step
{
    /// <summary>
    /// An <see cref="IStep"/>implementation that provides common behavior to subclasses, 
    /// including registering and calling listeners.
    /// </summary>
    public abstract class AbstractStep : IStep, IInitializationPostOperations
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #region Attributes

        /// <summary>
        /// Name property.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Allow start if complete flag property.
        /// </summary>
        public bool? AllowStartIfComplete { get; set; }
        private int _startLimit = Int32.MaxValue;
        
        /// <summary>
        /// Start limit for this step.
        /// </summary>
        public int StartLimit
        {
            get
            { return _startLimit; }
            set { _startLimit = value == 0 ? Int32.MaxValue : value; }
        }
        
        /// <summary>
        /// Job repository property.
        /// </summary>
        public IJobRepository JobRepository { get; set; }
        private readonly CompositeStepExecutionListener _stepExecutionListener = new CompositeStepExecutionListener(); 
        #endregion

        /// <summary>
        /// Custom constructor with name.
        /// </summary>
        /// <param name="name"></param>
        protected AbstractStep(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected AbstractStep() { }

        /// <summary>
        /// used programmatically by JobStepBuilder ...
        /// IInitializationPostOperations#AfterPropertiesSet.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public virtual void AfterPropertiesSet()  {
            Assert.State(Name != null, "A Step must have a name");
            Assert.State(JobRepository != null, "JobRepository is mandatory");
        }

        /// <summary>
        /// Extension point for subclasses to execute business logic. Subclasses should set the ExitStatus on the
        /// StepExecution before returning.
        /// </summary>
        /// <param name="stepExecution"></param>
        /// <exception cref="Exception"></exception>
        protected abstract void DoExecute(StepExecution stepExecution);

        /// <summary>
        /// Extension point for subclasses to provide callbacks to their collaborators 
        /// at the beginning of a step, to open or acquire resources. Does nothing by default.
        /// </summary>
        /// <param name="ctx"></param>
        /// <exception cref="Exception"></exception>
        protected virtual void Open(ExecutionContext ctx) { }

        /// <summary>
        /// Extension point for subclasses to provide callbacks to their collaborators 
        /// at the end of a step, to close or release resources. Does nothing by default.
        /// </summary>
        /// <param name="ctx"></param>
        /// <exception cref="Exception"></exception>
        protected virtual void Close(ExecutionContext ctx) { }

        /// <summary>
        /// Template method for step execution logic - calls abstract methods for resource initialization (
        /// Open), execution Logic (DoExecute) and resource closing (Close).
        /// </summary>
        /// <param name="stepExecution"></param>
        /// <exception cref="JobInterruptedException"></exception>
        /// <exception cref="UnexpectedJobExecutionException"></exception>
        public void Execute(StepExecution stepExecution)
        {
            Logger.Debug("Executing: id={0}", stepExecution.Id);
            stepExecution.StartTime = DateTime.Now;
            stepExecution.BatchStatus = BatchStatus.Started;
            JobRepository.Update(stepExecution);

            // Start with a default value that will be trumped by anything
            ExitStatus exitStatus = ExitStatus.Executing;
            DoExecutionRegistration(stepExecution);

            try
            {
                exitStatus = HandleExecution(stepExecution);
            }
            catch (Exception e)
            {
                exitStatus = HandleExecutionException(stepExecution, e, exitStatus);
            }
            finally
            {
                exitStatus = HandleListener(stepExecution, exitStatus);
                exitStatus = HandleUpdateExecutionContext(stepExecution, exitStatus);
                HandleUpdateStepExecution(stepExecution, exitStatus);
                HandleCloseAndRelease(stepExecution);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Step execution complete:{0}", stepExecution.GetSummary());
                }
            }
        }

        #region private utility methods

        /// <summary>
        /// Default mapping from Exception to ExitStatus. Clients can modify the exit code using a
        /// StepExecutionListener.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private ExitStatus GetDefaultExitStatusForFailure(Exception ex)
        {
            ExitStatus exitStatus;
            if (ex is JobInterruptedException || ex.InnerException is JobInterruptedException)
            {
                exitStatus = ExitStatus.Stopped.AddExitDescription("JobInterruptedException");
            }
            else if (ex is NoSuchJobException || ex.InnerException is NoSuchJobException)
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
        /// 
        /// </summary>
        /// <param name="stepExecution"></param>
        private void HandleCloseAndRelease(StepExecution stepExecution)
        {
            try
            {
                Close(stepExecution.ExecutionContext);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Exception while closing step execution resources in step {0} in job {1}", Name,
                    stepExecution.JobExecution.JobInstance.JobName);
                stepExecution.AddFailureException(e);
            }

            DoExecutionRelease();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stepExecution"></param>
        /// <param name="exitStatus"></param>
        private void HandleUpdateStepExecution(StepExecution stepExecution, ExitStatus exitStatus)
        {
            try
            {
                JobRepository.Update(stepExecution);
            }
            catch (Exception e)
            {
                stepExecution.BatchStatus = BatchStatus.Unknown;
                stepExecution.ExitStatus = exitStatus.And(ExitStatus.Unknown);
                stepExecution.AddFailureException(e);
                Logger.Error(e,
                    "Encountered an error saving batch meta data for step {0} in job {1}. This job is now in an unknown state and should not be restarted.",
                    Name, stepExecution.JobExecution.JobInstance.JobName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stepExecution"></param>
        /// <param name="exitStatus"></param>
        /// <returns></returns>
        private ExitStatus HandleUpdateExecutionContext(StepExecution stepExecution, ExitStatus exitStatus)
        {
            ExitStatus returnedExitStatus = exitStatus;
            try
            {
                JobRepository.UpdateExecutionContext(stepExecution);
            }
            catch (Exception e)
            {
                stepExecution.BatchStatus = BatchStatus.Unknown;
                returnedExitStatus = exitStatus.And(ExitStatus.Unknown);
                stepExecution.AddFailureException(e);
                Logger.Error(e,
                    "Encountered an error saving batch meta data for step {0} in job {1}. This job is now in an unknown state and should not be restarted.",
                    Name, stepExecution.JobExecution.JobInstance.JobName);
            }
            stepExecution.EndTime = DateTime.Now;
            stepExecution.ExitStatus = returnedExitStatus;
            return returnedExitStatus;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stepExecution"></param>
        /// <param name="exitStatus"></param>
        /// <returns></returns>
        private ExitStatus HandleListener(StepExecution stepExecution, ExitStatus exitStatus)
        {
            ExitStatus returnedExitStatus = exitStatus;
            try
            {
                // Update the step execution to the latest known value so the
                // listeners can act on it
                returnedExitStatus = returnedExitStatus.And(stepExecution.ExitStatus);
                stepExecution.ExitStatus = returnedExitStatus;
                Logger.Trace("_stepExecutionListener.AfterStep CALL");
                returnedExitStatus = returnedExitStatus.And(_stepExecutionListener.AfterStep(stepExecution));
            }
            catch (Exception e)
            {
                Logger.Error(e, "Exception in afterStep callback in step {0} in job {1}", Name,
                    stepExecution.JobExecution.JobInstance.JobName);
            }
            return returnedExitStatus;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stepExecution"></param>
        /// <param name="e"></param>
        /// <param name="exitStatus"></param>
        /// <returns></returns>
        private ExitStatus HandleExecutionException(StepExecution stepExecution, Exception e, ExitStatus exitStatus)
        {
            stepExecution.UpgradeStatus(DetermineBatchStatus(e));
            ExitStatus returnedExitStatus = exitStatus.And(GetDefaultExitStatusForFailure(e));
            stepExecution.AddFailureException(e);
            if (stepExecution.BatchStatus == BatchStatus.Stopped)
            {
                Logger.Info("Encountered interruption executing step {0} in job {1} : {2}",
                    Name, stepExecution.JobExecution.JobInstance.JobName, e.Message);
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug(e, "Full exception");
                }
            }
            else
            {
                Logger.Error(e, "Encountered an error executing step {0} in job {1} :", Name,
                    stepExecution.JobExecution.JobInstance.JobName);
            }
            return returnedExitStatus;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stepExecution"></param>
        /// <returns></returns>
        private ExitStatus HandleExecution(StepExecution stepExecution)
        {
            _stepExecutionListener.BeforeStep(stepExecution);
            Open(stepExecution.ExecutionContext);
            try
            {
                DoExecute(stepExecution);
            }
            catch (RepeatException e)
            {
                throw e.InnerException;
            }
            var exitStatus = ExitStatus.Completed.And(stepExecution.ExitStatus);

            // Check if someone is trying to stop us
            if (stepExecution.TerminateOnly)
            {
                throw new JobInterruptedException("JobExecution interrupted.");
            }

            // Need to upgrade here not set, in case the execution was stopped
            stepExecution.UpgradeStatus(BatchStatus.Completed);
            Logger.Debug("Step execution success: id= {0}", stepExecution.Id);
            return exitStatus;
        } 
        #endregion


        /// <summary>
        /// Releases the most recent StepExecution
        /// </summary>
        protected void DoExecutionRelease()
        {
            StepSynchronizationManager.Release();
        }

        /// <summary>
        /// Registers the StepExecution for property resolution via StepScope
        /// </summary>
        /// <param name="stepExecution"></param>
        protected void DoExecutionRegistration(StepExecution stepExecution)
        {
            StepSynchronizationManager.Register(stepExecution);
        }

        /// <summary>
        /// Determines the step status based on the exception.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static BatchStatus DetermineBatchStatus(Exception e)
        {
            if (e is JobInterruptedException || e.InnerException is JobInterruptedException)
            {
                return BatchStatus.Stopped;
            }
            else
            {
                return BatchStatus.Failed;
            }
        }


        /// <summary>
        /// Registers step execution listener.
        /// </summary>
        /// <param name="listener"></param>
        public void RegisterStepExecutionListener(IStepExecutionListener listener)
        {            
            _stepExecutionListener.Register(listener);
        }

        /// <summary>
        /// Registers an array of step execution listeners.
        /// </summary>
        /// <param name="listeners"></param>
        public void SetStepExecutionListeners(IStepExecutionListener[] listeners)
        {
            foreach (IStepExecutionListener listener in listeners)
            {
                RegisterStepExecutionListener(listener);
            }
        }


        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} :[name={1}]",GetType().Name,Name);
        }
    }
}