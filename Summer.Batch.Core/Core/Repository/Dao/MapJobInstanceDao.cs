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

using Summer.Batch.Core.Launch;
using Summer.Batch.Common.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Summer.Batch.Core.Repository.Dao
{
    /// <summary>
    ///  In-memory implementation of <see cref="IJobInstanceDao"/>.
    /// </summary>
    public class MapJobInstanceDao : IJobInstanceDao
    {
        private readonly IDictionary<string, JobInstance> _jobInstances = new ConcurrentDictionary<string, JobInstance>();
        private readonly IJobKeyGenerator<JobParameters> _jobKeyGenerator = new DefaultJobKeyGenerator();
        private long _currentId;

        /// <summary>
        /// Clears job instances dictionary.
        /// </summary>
        public void Clear()
        {
            _jobInstances.Clear();
        }

        #region IJobInstanceDao methods implementation
        /// <summary>
        /// @see IJobInstanceDao#CreateJobInstance.
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="jobParameters"></param>
        /// <returns></returns>
        public JobInstance CreateJobInstance(string jobName, JobParameters jobParameters)
        {
            Assert.State(GetJobInstance(jobName, jobParameters) == null, "A job instance with this name and parameters should not already exist");

            JobInstance jobInstance = new JobInstance(Interlocked.Increment(ref _currentId), jobName);
            jobInstance.IncrementVersion();
            _jobInstances[GetKey(jobName, jobParameters)] = jobInstance;

            return jobInstance;
        }

        /// <summary>
        /// @see IJobInstanceDao#GetJobInstance.
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="jobParameters"></param>
        /// <returns></returns>
        public JobInstance GetJobInstance(string jobName, JobParameters jobParameters)
        {
            JobInstance result;
            _jobInstances.TryGetValue(GetKey(jobName, jobParameters), out result);
            return result;
        }

        /// <summary>
        /// @see IJobInstanceDao#GetJobInstance.
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        public JobInstance GetJobInstance(long instanceId)
        {
            try
            {
                return _jobInstances.Values.First(j => j.Id == instanceId);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        /// <summary>
        /// @see IJobInstanceDao#GetJobInstance.
        /// </summary>
        /// <param name="jobExecution"></param>
        /// <returns></returns>
        public JobInstance GetJobInstance(JobExecution jobExecution)
        {
            return jobExecution.JobInstance;
        }

        /// <summary>
        /// @see IJobInstanceDao#GetJobInstances.
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IList<JobInstance> GetJobInstances(string jobName, int start, int count)
        {
            return _jobInstances.Values.Where(j => j.JobName == jobName).OrderByDescending(j => j.Id).Skip(start).Take(count).ToList();
        }

        /// <summary>
        /// @see IJobInstanceDao#GetJobNames.
        /// </summary>
        /// <returns></returns>
        public IList<string> GetJobNames()
        {
            return _jobInstances.Values.Select(j => j.JobName).Distinct().OrderBy(n => n).ToList();
        }

        /// <summary>
        /// @see IJobInstanceDao#GetJobInstanceCount.
        /// </summary>
        /// <param name="jobName"></param>
        /// <returns></returns>
        public int GetJobInstanceCount(string jobName)
        {
            int count = _jobInstances.Values.Count(j => j.JobName == jobName);

            if (count == 0)
            {
                throw new NoSuchJobException(string.Format("No job instances for job name {0} were found", jobName));
            }
            return count;
        } 
        #endregion

        #region private methods
        /// <summary>
        /// Returns key using job name and parameters.
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="jobParameters"></param>
        /// <returns></returns>
        private string GetKey(string jobName, JobParameters jobParameters)
        {
            return jobName + '|' + _jobKeyGenerator.GenerateKey(jobParameters);
        } 
        #endregion
    }
}
