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
    /// Data Access Object for job executions.
    /// </summary>
    public interface IJobExecutionDao
    {
        /// <summary>
        /// Persists a new job execution.
        /// The corresponding job instance must have been persisted.
        /// </summary>
        /// <param name="jobExecution">a job execution</param>
        void SaveJobExecution(JobExecution jobExecution);

        /// <summary>
        /// Updates the updates of a job execution.
        /// The job execution must have already been persisted.
        /// </summary>
        /// <param name="jobExecution">a job execution</param>
        void UpdateJobExecution(JobExecution jobExecution);

        /// <summary>
        /// Finds all the job executions for a job instance,
        /// sorted by descending creation order (the first element is the most recent).
        /// </summary>
        /// <param name="jobInstance">a job instance</param>
        /// <returns>a list of job executions</returns>
        IList<JobExecution> FindJobExecutions(JobInstance jobInstance);

        /// <param name="jobInstance">a job instance</param>
        /// <returns>the last created job execution for the job instance</returns>
        JobExecution GetLastJobExecution(JobInstance jobInstance);

        /// <param name="jobName">a job name</param>
        /// <returns>a set containing the job executions that are still runinig for the specified job name</returns>
        ISet<JobExecution> FindRunningJobExecutions(string jobName);

        /// <param name="executionId">an id for an existing job execution</param>
        /// <returns>the job execution with the given id</returns>
        JobExecution GetJobExecution(long executionId);

        /// <summary>
        /// Persists the status and version fields of a job execution. 
        /// The job execution must have already been persisted.
        /// </summary>
        /// <param name="jobExecution"></param>
        void SynchronizeStatus(JobExecution jobExecution);
    }
}
