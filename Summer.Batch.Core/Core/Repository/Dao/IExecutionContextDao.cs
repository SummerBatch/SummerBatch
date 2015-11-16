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

using Summer.Batch.Infrastructure.Item;
using System.Collections.Generic;

namespace Summer.Batch.Core.Repository.Dao
{
    /// <summary>
    /// Data Access Object for execution contexts.
    /// </summary>
    public interface IExecutionContextDao
    {
        /// <param name="jobExecution">a job execution</param>
        /// <returns>the execution context associated with the given job execution</returns>
        ExecutionContext GetExecutionContext(JobExecution jobExecution);

        /// <param name="stepExecution">a step execution</param>
        /// <returns>the execution context associated with the given step execution</returns>
        ExecutionContext GetExecutionContext(StepExecution stepExecution);

        /// <summary>
        /// Persists the execution context associated with the given job execution.
        /// A persistent entry for the context should not exist yet.
        /// </summary>
        /// <param name="jobExecution">a job execution</param>
        void SaveExecutionContext(JobExecution jobExecution);

        /// <summary>
        /// Persists the execution context associated with the given step execution.
        /// A persistent entry for the context should not exist yet.
        /// </summary>
        /// <param name="stepExecution">a step execution</param>
        void SaveExecutionContext(StepExecution stepExecution);

        /// <summary>
        /// Persists the execution contexts associated with each step execution in a collection.
        /// Persistent entries for these contexts should not exist.
        /// </summary>
        /// <param name="stepExecutions">a collection of step executions</param>
        void SaveExecutionContexts(ICollection<StepExecution> stepExecutions);

        /// <summary>
        /// Persists the updates of the execution context associated with the given job execution.
        /// A persistent entry should already exist for this context.
        /// </summary>
        /// <param name="jobExecution">a job execution</param>
        void UpdateExecutionContext(JobExecution jobExecution);

        /// <summary>
        /// Persists the updates of the execution context associated with the given step execution.
        /// A persistent entry should already exist for this context.
        /// </summary>
        /// <param name="stepExecution">a step execution</param>
        void UpdateExecutionContext(StepExecution stepExecution);
    }
}
