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

using Summer.Batch.Core.Repository;
using System;
using System.Linq;
using System.Threading;

namespace Summer.Batch.Core.Job.Flow
{
    /// <summary>
    /// Implementation of IFlowExecutor for use in components that need to
    /// execute a flow related to a JobExecution.
    /// </summary>
    public class JobFlowExecutor : IFlowExecutor, IDisposable
    {

        private ThreadLocal<StepExecution> _stepExecutionHolder = new ThreadLocal<StepExecution>();
        private readonly JobExecution _execution;
        /// <summary>
        /// Exit status.
        /// </summary>
        protected ExitStatus ExitStatus = ExitStatus.Executing;
        private readonly IStepHandler _stepHandler;
        private readonly IJobRepository _jobRepository;

        /// <summary>
        /// Custom constructor using a job repository, a step hander and a job execution.
        /// </summary>
        /// <param name="jobRepository"></param>
        /// <param name="stepHandler"></param>
        /// <param name="execution"></param>
        public JobFlowExecutor(IJobRepository jobRepository, IStepHandler stepHandler, JobExecution execution)
        {
            _jobRepository = jobRepository;
            _stepHandler = stepHandler;
            _execution = execution;
            _stepExecutionHolder.Value = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        private bool IsStepRestart(IStep step)
        {
            int count = _jobRepository.GetStepExecutionCount(_execution.JobInstance, step.Name);
            return count > 0;
        }

        /// <summary>
        /// @see IflowExecutor#ExecuteStep .
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        /// <exception cref="JobInterruptedException">&nbsp;</exception>
        /// <exception cref="JobRestartException">&nbsp;</exception>
        /// <exception cref="StartLimitExceededException">&nbsp;</exception>
        public string ExecuteStep(IStep step)
        {
            bool isRerun = IsStepRestart(step);
            StepExecution stepExecution = _stepHandler.HandleStep(step, _execution);
            _stepExecutionHolder.Value = stepExecution;

            if (stepExecution == null)
            {
                return ExitStatus.Completed.ExitCode;
            }
            if (stepExecution.TerminateOnly)
            {
                throw new JobInterruptedException(string.Format("Step requested termination: {0}",stepExecution), stepExecution.BatchStatus);
            }

            if (isRerun)
            {
                stepExecution.ExecutionContext.Put("batch.restart", true);
            }

            return stepExecution.ExitStatus.ExitCode;
        }

        /// <summary>
        /// @see IFlowExecutor#GetJobExecution .
        /// </summary>
        /// <returns></returns>
        public JobExecution GetJobExecution()
        {
            return _execution;
        }

        /// <summary>
        /// @see IFlowExectutor#GetStepExecution .
        /// </summary>
        /// <returns></returns>
        public StepExecution GetStepExecution()
        {
            return _stepExecutionHolder.Value;
        }

        /// <summary>
        /// @see IFlowExecutor#Close .
        /// </summary>
        /// <param name="result"></param>
        public void Close(FlowExecution result)
        {
            _stepExecutionHolder.Value = null;
        }

        /// <summary>
        /// @see IFlowExecutor#AbandonStepExecution .
        /// </summary>
        public void AbandonStepExecution()
        {
            StepExecution lastStepExecution = _stepExecutionHolder.Value;
            if (lastStepExecution != null && lastStepExecution.BatchStatus.IsGreaterThan(BatchStatus.Stopping))
            {
                lastStepExecution.UpgradeStatus(BatchStatus.Abandoned);
                _jobRepository.Update(lastStepExecution);
            }
        }

        /// <summary>
        /// Retrieves batch status.
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        protected BatchStatus FindBatchStatus(FlowExecutionStatus status)
        {
            foreach (BatchStatus batchStatus in BatchStatus.Values)
            {
                if (status.Name.StartsWith(batchStatus.ToString()))
                {
                    return batchStatus;
                }
            }
            return BatchStatus.Unknown;
        }

        /// <summary>
        /// @see IFlowExecutor#UpdateJobExecutionStatus .
        /// </summary>
        /// <param name="status"></param>
        public void UpdateJobExecutionStatus(FlowExecutionStatus status)
        {
            _execution.Status = FindBatchStatus(status);
            ExitStatus = ExitStatus.And(new ExitStatus(status.Name));
            _execution.ExitStatus = ExitStatus;
        }

        /// <summary>
        /// @see IFlowExecutor#IsRestart .
        /// </summary>
        /// <returns></returns>
        public bool IsRestart()
        {
            if (GetStepExecution() != null && GetStepExecution().BatchStatus == BatchStatus.Abandoned)
            {
                 //This is assumed to be the last step execution and it was marked
                 //abandoned, so we are in a restart of a stopped step.             
                return true;
            }
            return !_execution.StepExecutions.Any(); 
        }

        /// <summary>
        /// @see IFlowExecutor#AddExitStatus .
        /// </summary>
        /// <param name="code"></param>
        public void AddExitStatus(string code)
        {
            ExitStatus = ExitStatus.And(new ExitStatus(code));
        }

        #region IDisposable
        /// <summary>
        /// see https://msdn.microsoft.com/fr-fr/library/ms244737.aspx
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// see https://msdn.microsoft.com/fr-fr/library/ms244737.aspx
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (disposing && _stepExecutionHolder != null)
            {
                _stepExecutionHolder.Dispose();
                _stepExecutionHolder = null;

            }
        }
        #endregion
    }
}