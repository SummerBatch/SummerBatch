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
using Summer.Batch.Infrastructure.Item;
using Summer.Batch.Common.Util;
using System;
using System.Collections.Generic;
using Summer.Batch.Common.Factory;

namespace Summer.Batch.Core.Partition.Support
{
    /// <summary>
    /// Generic implementation of <see cref="IStepExecutionSplitter"/> that delegates to a
    /// <see cref="IPartitioner"/> to generate <see cref="ExecutionContext"/> instances. Takes
    /// care of restartability and identifying the step executions from previous runs
    /// of the same job. The generated <see cref="StepExecution"/> instances have names
    /// that identify them uniquely in the partition. The name is constructed from a
    /// base (name of the target step) plus a suffix taken from the
    /// IPartitioner identifiers, separated by a colon, e.g.
    /// <code>{step1:partition0, step1:partition1, ...}</code>.
    /// </summary>
    public class SimpleStepExecutionSplitter : IStepExecutionSplitter, IInitializationPostOperations
    {
        private const string StepNameSeparator = ":";

        #region Attributes
        /// <summary>
        /// Step name property.
        /// </summary>
        public string StepName { get; set; }

        /// <summary>
        /// Partitioner property.
        /// </summary>
        public IPartitioner Partitioner { private get; set; }

        /// <summary>
        /// Job repository property.
        /// </summary>
        public IJobRepository JobRepository { private get; set; }

        /// <summary>
        /// Flag to indicate whether start if complete is allowed or not.
        /// </summary>
        public bool AllowStartIfComplete { private get; set; }        
        #endregion

        /// <summary>
        /// Default constructor for convenience in configuration.
        /// </summary>
        public SimpleStepExecutionSplitter()
        {
        }

        /// <summary>
        /// Constructs a <see cref="SimpleStepExecutionSplitter"/> from its mandatory properties.
        /// </summary>
        /// <param name="jobRepository"></param>
        /// <param name="allowStartIfComplete">flag specifying preferences on restart</param>
        /// <param name="stepName"></param>
        /// <param name="partitioner">a IPartitioner to use for generating input parameters</param>
        public SimpleStepExecutionSplitter(IJobRepository jobRepository, bool allowStartIfComplete,
            string stepName, IPartitioner partitioner)
        {
            JobRepository = jobRepository;
            AllowStartIfComplete = allowStartIfComplete;
            Partitioner = partitioner;
            StepName = stepName;
        }

        /// <summary>
        /// Post-init checks.
        /// IInitializationPostOperations#AfterPropertiesSet .
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void AfterPropertiesSet()
        {
            Assert.State(JobRepository != null, "A JobRepository is required");
            Assert.State(StepName != null, "A step name is required");
            Assert.State(Partitioner != null, "A Partitioner is required");
        }


        /// <summary>
        /// see IStepExecutionSplitter#Split .
        /// </summary>
        /// <param name="stepExecution"></param>
        /// <param name="gridSize"></param>
        /// <returns></returns>
        public HashSet<StepExecution> Split(StepExecution stepExecution, int gridSize)
        {
            JobExecution jobExecution = stepExecution.JobExecution;

            IDictionary<string, ExecutionContext> contexts = GetContexts(stepExecution, gridSize);
            HashSet<StepExecution> set = new HashSet<StepExecution>();

            foreach (KeyValuePair<string, ExecutionContext> context in contexts)
            {
                // Make the step execution name unique and repeatable
                string stepName = StepName + StepNameSeparator + context.Key;
                StepExecution currentStepExecution = jobExecution.CreateStepExecution(stepName);
                bool startable = GetStartable(currentStepExecution, context.Value);

                if (startable)
                {
                    set.Add(currentStepExecution);
                }
            }
            JobRepository.AddAll(set);
            return set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stepExecution"></param>
        /// <param name="gridSize"></param>
        /// <returns></returns>
        private IDictionary<string, ExecutionContext> GetContexts(StepExecution stepExecution, int gridSize)
        {

            ExecutionContext context = stepExecution.ExecutionContext;
            string key = "SimpleStepExecutionSplitter.GRID_SIZE";

            // If this is a restart we must retain the same grid size, ignoring the
            // one passed in...
            int splitSize = (int)context.GetLong(key, gridSize);
            context.PutLong(key, splitSize);

            IDictionary<string, ExecutionContext> result;
            if (context.Dirty)
            {
                // The context changed so we didn't already know the partitions
                JobRepository.UpdateExecutionContext(stepExecution);
                result = Partitioner.Partition(splitSize);
            }
            else
            {
                if (Partitioner is IPartitionNameProvider)
                {
                    result = new Dictionary<string, ExecutionContext>();
                    ICollection<string> names = ((IPartitionNameProvider)Partitioner).GetPartitionNames(splitSize);
                    foreach (string name in names)
                    {
                         // We need to return the same keys as the original (failed)
                         // execution, but the execution contexts will be discarded
                         // so they can be empty.                         
                        result.Add(name, new ExecutionContext());
                    }
                }
                else
                {
                    // If no names are provided, grab the partition again.
                    result = Partitioner.Partition(splitSize);
                }
            }

            return result;
        }

        /// <summary>
        /// Should it start ?
        /// </summary>
        /// <param name="stepExecution"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="JobExecutionException"></exception>
        protected bool GetStartable(StepExecution stepExecution, ExecutionContext context)
        {
            JobInstance jobInstance = stepExecution.JobExecution.JobInstance;
            string stepName = stepExecution.StepName;
            StepExecution lastStepExecution = JobRepository.GetLastStepExecution(jobInstance, stepName);
            bool isRestart = (lastStepExecution != null && lastStepExecution.BatchStatus != BatchStatus.Completed);

            stepExecution.ExecutionContext = isRestart ? lastStepExecution.ExecutionContext : context;
            return ShouldStart(AllowStartIfComplete, stepExecution, lastStepExecution) || isRestart;
        }

        /// <summary>
        /// Tests if step execution should start.
        /// </summary>
        /// <param name="allowStartIfComplete"></param>
        /// <param name="stepExecution"></param>
        /// <param name="lastStepExecution"></param>
        /// <returns></returns>
        /// <exception cref="JobExecutionException"></exception>
        private bool ShouldStart(bool allowStartIfComplete, StepExecution stepExecution, StepExecution lastStepExecution)
        {
            if (lastStepExecution == null)
            {
                return true;
            }
            BatchStatus stepStatus = lastStepExecution.BatchStatus;
            
            //unknown status handling
            HandleUnknownStatus(stepStatus);

            //Completed status handling
            if (stepStatus == BatchStatus.Completed)
            {
                return allowStartIfComplete || IsSameJobExecution(stepExecution, lastStepExecution);
            }

            //Stopped or failed permits restart
            if (HandleStoppedOrFailedStatus(stepStatus)) return true;

            //other statuses should not allow restart.
            PreventFromRestarting(stepStatus);
            
            throw new JobExecutionException(
                string.Format("Cannot restart step from {0} status.  "
                    + "We believe the old execution was abandoned and therefore has been marked as un-restartable.",
                    stepStatus));

        }

        private static void PreventFromRestarting(BatchStatus stepStatus)
        {
            if (stepStatus == BatchStatus.Started || stepStatus == BatchStatus.Starting
                || stepStatus == BatchStatus.Stopping)
            {
                throw new JobExecutionException(string.Format(
                    "Cannot restart step from {0} status.  "
                    + "The old execution may still be executing, so you may need to verify manually that this is the case.",stepStatus));
            }
        }

        private static bool HandleStoppedOrFailedStatus(BatchStatus stepStatus)
        {
            if (stepStatus == BatchStatus.Stopped || stepStatus == BatchStatus.Failed)
            {
                return true;
            }
            return false;
        }

        private static void HandleUnknownStatus(BatchStatus stepStatus)
        {
            if (stepStatus == BatchStatus.Unknown)
            {
                throw new JobExecutionException("Cannot restart step from UNKNOWN status.  "
                                                + "The last execution ended with a failure that could not be rolled back, "
                                                + "so it may be dangerous to proceed.  " +
                                                "Manual intervention is probably necessary.");
            }
        }

        /// <summary>
        /// Tests if same job execution.
        /// </summary>
        /// <param name="stepExecution"></param>
        /// <param name="lastStepExecution"></param>
        /// <returns></returns>
        private bool IsSameJobExecution(StepExecution stepExecution, StepExecution lastStepExecution)
        {
            if (stepExecution.GetJobExecutionId() == null)
            {
                return lastStepExecution.GetJobExecutionId() == null;
            }
            return stepExecution.GetJobExecutionId().Equals(lastStepExecution.GetJobExecutionId());
        }
    }
}