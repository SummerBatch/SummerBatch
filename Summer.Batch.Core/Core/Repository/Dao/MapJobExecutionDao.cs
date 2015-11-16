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

using Summer.Batch.Common.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Summer.Batch.Core.Repository.Dao
{
    /// <summary>
    /// In-memory implementation of <see cref="IJobExecutionDao"/>.
    /// </summary>
    public class MapJobExecutionDao : IJobExecutionDao
    {
        private readonly IDictionary<long?, JobExecution> _executionsById = new ConcurrentDictionary<long?, JobExecution>();
        private long _currentId;

        /// <summary>
        /// Clear sthe executions dictionary.
        /// </summary>
        public void Clear()
        {
            _executionsById.Clear();
        }

        /// <summary>
        /// Copies through serialization/deserialization.
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        private static JobExecution Copy(JobExecution original)
        {
            return original.Serialize().Deserialize<JobExecution>();
        }

        #region IJobExecutionDao methods implementation
        /// <summary>
        /// @see IJobExecutionDao#SaveJobExecution.
        /// </summary>
        /// <param name="jobExecution"></param>
        public void SaveJobExecution(JobExecution jobExecution)
        {
            Assert.IsTrue(jobExecution.Id == null);
            jobExecution.Id = Interlocked.Increment(ref _currentId);
            jobExecution.IncrementVersion();
            _executionsById[jobExecution.Id] = Copy(jobExecution);
        }

        /// <summary>
        /// @see IJobExecutionDao#UpdateJobExecution
        /// </summary>
        /// <param name="jobExecution"></param>
        public void UpdateJobExecution(JobExecution jobExecution)
        {
            JobExecution persistedExecution;
            if (jobExecution.Id == null || !_executionsById.TryGetValue(jobExecution.Id, out persistedExecution))
            {
                throw new ArgumentException("jobExecution should have already been saved");
            }
            lock (jobExecution)
            {
                if (jobExecution.Version != persistedExecution.Version)
                {
                    throw new ArgumentException(string.Format("Attempt to update job execution (id={0}) with version {1}, but current version is {2}.",
                        jobExecution.Id, jobExecution.Version, persistedExecution.Version));
                }
                jobExecution.IncrementVersion();
                _executionsById[jobExecution.Id] = Copy(jobExecution);
            }
        }

        /// <summary>
        /// IJobExecutionDao#FindJobExecutions.
        /// </summary>
        /// <param name="jobInstance"></param>
        /// <returns></returns>
        public IList<JobExecution> FindJobExecutions(JobInstance jobInstance)
        {
            return _executionsById.Values.Where(j => j.JobInstance.Equals(jobInstance))
                                        .Select(Copy)
                                        .OrderByDescending(j => j.Id)
                                        .ToList();
        }

        /// <summary>
        /// @see IJobExecutionDao#GetLastJobExecution.
        /// </summary>
        /// <param name="jobInstance"></param>
        /// <returns></returns>
        public JobExecution GetLastJobExecution(JobInstance jobInstance)
        {
            return _executionsById.Values.Where(e => e.JobInstance.Equals(jobInstance))
                                        .OrderByDescending(e => e.CreateTime)
                                        .First();
        }

        /// <summary>
        /// @see IJobExecutionDao#FindRunningJobExecutions.
        /// </summary>
        /// <param name="jobName"></param>
        /// <returns></returns>
        public ISet<JobExecution> FindRunningJobExecutions(string jobName)
        {
            return new HashSet<JobExecution>(_executionsById.Values.Where(j => j.JobInstance.JobName == jobName && j.IsRunning()));
        }

        /// <summary>
        /// @see IJobExecutionDao#GetJobExecution.
        /// </summary>
        /// <param name="executionId"></param>
        /// <returns></returns>
        public JobExecution GetJobExecution(long executionId)
        {
            JobExecution execution;
            return _executionsById.TryGetValue(executionId, out execution) ? Copy(execution) : null;
        }

        /// <summary>
        /// @see IJobExecutionDao#SynchronizeStatus.
        /// </summary>
        /// <param name="jobExecution"></param>
        public void SynchronizeStatus(JobExecution jobExecution)
        {
            JobExecution persistedExecution;
            if (jobExecution.Id != null && _executionsById.TryGetValue(jobExecution.Id, out persistedExecution)
                                        && persistedExecution.Version != jobExecution.Version)
            {
                jobExecution.UpgradeStatus(persistedExecution.Status);
                jobExecution.Version = persistedExecution.Version;
            }
        } 
        #endregion
    }
}
