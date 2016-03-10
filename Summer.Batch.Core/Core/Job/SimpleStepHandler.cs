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
using Summer.Batch.Core.Repository;
using Summer.Batch.Common.Factory;
using Summer.Batch.Infrastructure.Item;
using Summer.Batch.Common.Util;

namespace Summer.Batch.Core.Job
{
    /// <summary>
    ///  Implementation of <see cref="IStepHandler"/> that manages repository and restart concerns.
    /// </summary>
    public class SimpleStepHandler : IStepHandler, IInitializationPostOperations
    {

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Job repository property.
        /// </summary>
        public IJobRepository JobRepository { get; set; }

        private ExecutionContext _executionContext;

        /// <summary>
        /// Execution context property.
        /// </summary>
        public ExecutionContext Context
        {
            set { _executionContext = value; }
        }

        /// <summary>
        /// Convenient default constructor for configuration usage.
        /// </summary>
        public SimpleStepHandler() : this(null) { }

        /// <summary>
        /// Custom constructor using a job repository and an execution context.
        /// </summary>
        /// <param name="jobRepository"></param>
        /// <param name="executionContext"></param>
        public SimpleStepHandler(IJobRepository jobRepository, ExecutionContext executionContext)
        {
            JobRepository = jobRepository;
            _executionContext = executionContext;
        }

        /// <summary>
        /// Custom constructor using a job repository.
        /// </summary>
        /// <param name="jobRepository"></param>
        public SimpleStepHandler(IJobRepository jobRepository)
            : this(jobRepository, new ExecutionContext())
        { }


        /// <summary>
        /// Check mandatory properties (jobRepository).
        /// @see IInitializationPostOperations#AfterPropertiesSet .
        /// </summary>
        public void AfterPropertiesSet() {
            Assert.State(JobRepository != null, "A JobRepository must be provided");
        }

        /// <summary>
        /// @see IStepHandler#HandleStep .
        /// </summary>
        /// <param name="step"></param>
        /// <param name="execution"></param>
        /// <returns></returns>
        /// <exception cref="JobInterruptedException">&nbsp;</exception>
        /// <exception cref="JobRestartException">&nbsp;</exception>
        /// <exception cref="StartLimitExceededException">&nbsp;</exception>
        public StepExecution HandleStep(IStep step, JobExecution execution)
        {
            if (execution.IsStopping())
            {
                throw new JobInterruptedException("JobExecution interrupted.");
            }

            JobInstance jobInstance = execution.JobInstance;
            StepExecution lastStepExecution = JobRepository.GetLastStepExecution(jobInstance, step.Name);
            if (StepExecutionPartOfExistingJobExecution(execution, lastStepExecution))
            {
                // If the last execution of this step was in the same job, it's
                // probably intentional so we want to run it again...
                Logger.Info("Duplicate step [{0}] detected in execution of job=[{1}]. "
                            + "If either step fails, both will be executed again on restart.",
                            step.Name,
                            jobInstance.JobName
                        );
                lastStepExecution = null;
            }
            StepExecution currentStepExecution = lastStepExecution;
            if (ShouldStart(lastStepExecution, execution, step))
            {
                currentStepExecution = execution.CreateStepExecution(step.Name);

                //Handle restart if needed
                HandleRestart(lastStepExecution, currentStepExecution);

                //Handle normal step execution
                HandleStepExecution(step, execution, currentStepExecution);

                if (currentStepExecution.BatchStatus == BatchStatus.Stopping
                        || currentStepExecution.BatchStatus == BatchStatus.Stopped)
                {
                    // Ensure that the job gets the message that it is stopping
                    execution.Status = BatchStatus.Stopping;
                    throw new JobInterruptedException("Job interrupted by step execution");
                }
            }

            return currentStepExecution;
        }

        /// <summary>
        /// Handle normal step execution.
        /// </summary>
        /// <param name="step"></param>
        /// <param name="execution"></param>
        /// <param name="currentStepExecution"></param>
        private void HandleStepExecution(IStep step, JobExecution execution, StepExecution currentStepExecution)
        {
            JobRepository.Add(currentStepExecution);
            Logger.Info("Executing step: [ {0} ]", step.Name);
            try
            {
                step.Execute(currentStepExecution);
                currentStepExecution.ExecutionContext.Put("batch.executed", true);
            }
            catch (JobInterruptedException)
            {
                // Ensure that the job gets the message that it is stopping
                // and can pass it on to other steps that are executing
                // concurrently.
                execution.Status = BatchStatus.Stopping;
                throw;
            }

            JobRepository.UpdateExecutionContext(execution);
        }

        /// <summary>
        /// Handle restartability if needed.
        /// </summary>
        /// <param name="lastStepExecution"></param>
        /// <param name="currentStepExecution"></param>
        private void HandleRestart(StepExecution lastStepExecution, StepExecution currentStepExecution)
        {
            bool isRestart = (lastStepExecution != null && !lastStepExecution.BatchStatus.Equals(BatchStatus.Completed));
            if (isRestart)
            {
                currentStepExecution.ExecutionContext = lastStepExecution.ExecutionContext;
                if (lastStepExecution.ExecutionContext.ContainsKey("batch.executed"))
                {
                    currentStepExecution.ExecutionContext.Remove("batch.executed");
                }
            }
            else
            {
                currentStepExecution.ExecutionContext = new ExecutionContext(_executionContext);
            }
        }


        private bool StepExecutionPartOfExistingJobExecution(JobExecution jobExecution, StepExecution stepExecution)
        {
            return stepExecution != null && stepExecution.GetJobExecutionId() != null
                    && stepExecution.GetJobExecutionId().Equals(jobExecution.Id);
        }

        /// <summary>
        /// Given a step and configuration, return true if the step should start,
        /// false if it should not, and throw an exception if the job should finish.	 
        /// </summary>
        /// <param name="lastStepExecution"></param>
        /// <param name="jobExecution"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        /// <exception cref="JobRestartException">&nbsp;</exception>
        /// <exception cref="StartLimitExceededException">&nbsp;</exception>
        protected bool ShouldStart(StepExecution lastStepExecution, JobExecution jobExecution, IStep step)
        {
            var stepStatus = lastStepExecution == null ? BatchStatus.Starting : lastStepExecution.BatchStatus;

            if (stepStatus == BatchStatus.Unknown)
            {
                throw new JobRestartException("Cannot restart step from UNKNOWN status. "
                        + "The last execution ended with a failure that could not be rolled back, "
                        + "so it may be dangerous to proceed. Manual intervention is probably necessary.");
            }

            if (stepStatus == BatchStatus.Completed && 
                ( step.AllowStartIfComplete !=null && !step.AllowStartIfComplete.Value)
                    || stepStatus == BatchStatus.Abandoned)
            {
                // step is complete, false should be returned, indicating that the
                // step should not be started
                Logger.Info("Step already complete or not restartable, so no action to execute: {0}",lastStepExecution);
                return false;
            }

            if (JobRepository.GetStepExecutionCount(jobExecution.JobInstance, step.Name) < step.StartLimit)
            {
                // step start count is less than start max, return true
                return true;
            }
            else
            {
                // start max has been exceeded, throw an exception.
                
                throw new StartLimitExceededException(
                    string.Format("Maximum start limit exceeded for step: {0} StartMax: {1}", 
                        step.Name, step.StartLimit));
            }
        }

    }
}