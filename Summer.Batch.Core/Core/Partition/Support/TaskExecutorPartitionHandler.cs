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

using Summer.Batch.Core.Step;
using Summer.Batch.Common.TaskExecution;
using Summer.Batch.Common.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Summer.Batch.Core.Partition.Support
{
    /// <summary>
    /// An <see cref="IPartitionHandler"/> that uses an <see cref="ITaskExecutor"/> to execute the
    /// partitioned <see cref="IStep"/> locally in multiple threads. This can be an
    /// effective approach for scaling batch steps that are IO intensive, like
    /// directory and filesystem scanning and copying.
    /// By default, the thread pool is synchronous.
    /// </summary>
    public class TaskExecutorPartitionHandler : AbstractPartitionHandler, IStepHolder
    {
        private ITaskExecutor _taskExecutor = new SyncTaskExecutor();
        
        /// <summary>
        /// Task executor property.
        /// </summary>
        public ITaskExecutor TaskExecutor { set { _taskExecutor = value; } }

        /// <summary>
        /// Step property.
        /// </summary>
        public IStep Step { get; set; }

        /// <summary>
        /// @see AbstractPartitionHandler#DoHandle .
        /// </summary>
        /// <param name="masterStepExecution"></param>
        /// <param name="partitionStepExecutions"></param>
        /// <returns></returns>
        protected override ICollection<StepExecution> DoHandle(StepExecution masterStepExecution, ICollection<StepExecution> partitionStepExecutions)
        {
            Assert.NotNull(Step, "A Step must be provided.");
            HashSet<Task<StepExecution>> tasks = new HashSet<Task<StepExecution>>();
            HashSet<StepExecution> result = new HashSet<StepExecution>();

            foreach (StepExecution stepExecution in partitionStepExecutions)
            {
                Task<StepExecution> task = CreateTask(Step, stepExecution);
                try
                {
                    _taskExecutor.Execute(task);
                    tasks.Add(task);
                }
                catch (TaskRejectedException)
                {
                    // couldn't execute one of the tasks
                    ExitStatus exitStatus = ExitStatus.Failed.AddExitDescription("TaskExecutor rejected the task for this step.");
                    
                    // Set the status in case the caller is tracking it through the
                    // JobExecution.                    
                    stepExecution.BatchStatus = BatchStatus.Failed;
                    stepExecution.ExitStatus = exitStatus;
                    result.Add(stepExecution);
                }
            }

            foreach (Task<StepExecution> task in tasks)
            {
                // Accessing Result is blocking (waits for asynchronous execution to complete)
                result.Add(task.Result);
            }

            return result;
        }

        /// <summary>
        /// Creates a task to be executed later on for the given stepExecution.
        /// </summary>
        /// <param name="step"></param>
        /// <param name="stepExecution"></param>
        /// <returns></returns>
        protected Task<StepExecution> CreateTask(IStep step, StepExecution stepExecution)
        {
            return new Task<StepExecution>(() =>
            {
                step.Execute(stepExecution);
                return stepExecution;
            });
        }

    }
}