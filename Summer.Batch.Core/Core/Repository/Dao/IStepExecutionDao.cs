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

using System.Collections.Generic;

namespace Summer.Batch.Core.Repository.Dao
{
    /// <summary>
    /// Data Access Object for step executions.
    /// </summary>
    public interface IStepExecutionDao
    {
        /// <summary>
        /// Persists the given step execution. It must not have been persisted yet.
        /// </summary>
        /// <param name="stepExecution">the stepExecution to persist</param>
        void SaveStepExecution(StepExecution stepExecution);

        /// <summary>
        /// Persists the step executions in a collection. The step executions must not have been persisted yet.
        /// </summary>
        /// <param name="stepExecutions">a collection of step executions to persists</param>
        void SaveStepExecutions(ICollection<StepExecution> stepExecutions);

        /// <summary>
        /// Persits the updates of a step execution. It must have already been persisted.
        /// </summary>
        /// <param name="stepExecution">a persisted step execution</param>
        void UpdateStepExecution(StepExecution stepExecution);

        /// <param name="jobExecution">a job execution</param>
        /// <param name="stepExecutionId">a step execution id</param>
        /// <returns>the step execution with the given id in the given job execution</returns>
        StepExecution GetStepExecution(JobExecution jobExecution, long stepExecutionId);

        /// <summary>
        /// Adds persisted step executions to a job execution.
        /// </summary>
        /// <param name="jobExecution">a job execution</param>
        void AddStepExecutions(JobExecution jobExecution);
    }
}
