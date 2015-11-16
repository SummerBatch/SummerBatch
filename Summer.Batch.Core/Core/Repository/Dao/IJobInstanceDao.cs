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
    /// Data Access Object for job instances.
    /// </summary>
    public interface IJobInstanceDao
    {
        /// <summary>
        /// Creates a job instance with given name and parameters.
        /// A job instance with the same name and parameters should not already exist.
        /// </summary>
        /// <param name="jobName">the job name</param>
        /// <param name="jobParameters">job parameters</param>
        /// <returns>a new persisted job instance</returns>
        JobInstance CreateJobInstance(string jobName, JobParameters jobParameters);

        /// <param name="jobName">a job name</param>
        /// <param name="jobParameters">job parameters</param>
        /// <returns>the job instance with the given name and parameters or <c>null</c> if it does not exist</returns>
        JobInstance GetJobInstance(string jobName, JobParameters jobParameters);

        /// <param name="instanceId">an id</param>
        /// <returns>the job instance with the given id or <c>null</c> if it does not exist</returns>
        JobInstance GetJobInstance(long instanceId);

        /// <param name="jobExecution">a job execution</param>
        /// <returns>the job instance for the given job execution or <c>null</c> if it does not exist</returns>
        JobInstance GetJobInstance(JobExecution jobExecution);

        /// <summary>
        /// Fetches a list of of job instances ordered by descending primary key.
        /// </summary>
        /// <param name="jobName">the name of a job</param>
        /// <param name="start">the index of the first instance to return</param>
        /// <param name="count">the number of instances to return</param>
        /// <returns>a list containing the requested job instances</returns>
        IList<JobInstance> GetJobInstances(string jobName, int start, int count);

        /// <returns>the list of all the job names, sorted ascendingly</returns>
        IList<string> GetJobNames();

        /// <param name="jobName">a job name</param>
        /// <returns>the number of job instances for the given job name</returns>
        /// <exception cref="Summer.Batch.Core.Launch.NoSuchJobException">if there are no job instances for this job name.</exception>
        int GetJobInstanceCount(string jobName);
    }
}
