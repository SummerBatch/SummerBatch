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
//  distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
using System;
using Microsoft.Practices.Unity;
using NLog;
using Summer.Batch.Common.Transaction;
using Summer.Batch.Core;


namespace Summer.Batch.Extra
{
    /// <summary>
    /// Common implementation of the pre processor, processor and post processor.
    /// Manages step and job contexts.
    /// </summary>
    public class AbstractExecutionListener : IStepExecutionListener
    {
        private const string Restart = "batch.restart";
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The context manager for the job context.
        /// </summary>
        [Dependency(BatchConstants.JobContextManagerName)]
        public IContextManager JobContextManager { private get; set; }
        
        /// <summary>
        /// The context manager for the step context.
        /// </summary>
        [Dependency(BatchConstants.StepContextManagerName)]
        public IContextManager StepContextManager { private get; set; }

        /// <summary>
        /// Default implementation of preprocessing will do nothing.
        /// If a pre processor exists, subclass will override this method.
        /// </summary>
        protected virtual void Preprocess()
        {
            //does nothing on purpose
        }

        /// <summary>
        /// Default implementation of postprocessing will only return COMPLETED code.
        /// If a post processor exists, subclass will override this method.
        /// </summary>
        /// <returns></returns>
        protected virtual ExitStatus Postprocess()
        {
            return ExitStatus.Completed;
        }

        /// <summary>
        /// Logic launched before the step. Will register the contexts and launch the preprocessor.
        /// @see IStepExecutionListener#BeforeStep
        /// </summary>
        /// <param name="stepExecution"></param>
        public virtual void BeforeStep(StepExecution stepExecution)
        {
            RegisterContexts(stepExecution);
            if (!stepExecution.ExecutionContext.ContainsKey(Restart))
                Preprocess();
        }

        /// <summary>
        /// Logic launched after the step. Will launch the postprocessor.
        /// @see IStepExecutionListener#AfterStep
        /// </summary>
        /// <param name="stepExecution"></param>
        /// <returns></returns>
        public virtual ExitStatus AfterStep(StepExecution stepExecution)
        {
            ExitStatus returnStatus = stepExecution.ExitStatus;
            if (!"FAILED".Equals(returnStatus.ExitCode))
            {
                using (var scope = TransactionScopeManager.CreateScope())
                {
                    try
                    {
                        returnStatus = Postprocess();
                    }
                    catch (Exception e)
                    {
                        // Need to catch exception to log and set status to FAILED, while
                        // Spring batch would only log and keep the status COMPLETED
                        Logger.Error(e, "Exception during postprocessor");
                        stepExecution.UpgradeStatus(BatchStatus.Failed);
                        throw;
                    }
                    scope.Complete();
                }
            }
            return returnStatus;
        }

        /// <summary>
        /// registers both job and step contexts.
        /// </summary>
        /// <param name="stepExecution"></param>
        private void RegisterContexts(StepExecution stepExecution)
        {
            JobExecution jobExecution = stepExecution.JobExecution;
            JobContextManager.Context = jobExecution.ExecutionContext;
            StepContextManager.Context = stepExecution.ExecutionContext;
        }


        /// <summary>
        /// Checks if the record is the last one.
        /// </summary>
        /// <param name="arg">the record to check</param>
        /// <returns>whether the record is the last one</returns>
        protected bool IsLast(object arg)
        {
            bool isLast = false;
            if (StepContextManager.ContainsKey(BatchConstants.LastRecordKey))
            {
                isLast = StepContextManager.GetFromContext(BatchConstants.LastRecordKey).Equals(arg);
            }
            return isLast;
        }

    }
}