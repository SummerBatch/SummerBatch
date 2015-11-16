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
using Summer.Batch.Data;
using Summer.Batch.Data.Incrementer;
using Summer.Batch.Common.Util;
using System.Collections.Generic;
using System.Transactions;

namespace Summer.Batch.Core.Repository.Dao
{
    /// <summary>
    /// Database implementation of <see cref="IJobInstanceDao"/>. Uses sequences (via <see cref="IDataFieldMaxValueIncrementer"/> 
    /// abstraction) to create all primary keys before inserting a new row. Objects are checked to ensure all mandatory
    /// fields to be stored are not null. If any are found to be null, an ArgumentException will be thrown. 
    /// </summary>
    public class DbJobInstanceDao : AbstractDbBatchMetadataDao, IJobInstanceDao
    {
        #region Queries

        private const string CreateJobInstanceQuery = "INSERT into {0}JOB_INSTANCE(JOB_INSTANCE_ID, JOB_NAME, JOB_KEY, VERSION) values (@id, @jobName, @key, @version)";

        private const string FindJobsWithNameQuery = "SELECT JOB_INSTANCE_ID, JOB_NAME from {0}JOB_INSTANCE where JOB_NAME = @jobName";

        private const string CountJobsWithNameQuery = "SELECT COUNT(*) from {0}JOB_INSTANCE where JOB_NAME = @jobName";

        private const string FindJobsWithKeyQuery = FindJobsWithNameQuery + " and JOB_KEY = @key";

        private const string FindJobsWithEmptyKey = FindJobsWithNameQuery + " and (JOB_KEY = @key OR JOB_KEY is NULL)";

        private const string GetJobFromIdQuery = "SELECT JOB_INSTANCE_ID, JOB_NAME, JOB_KEY, VERSION from {0}JOB_INSTANCE where JOB_INSTANCE_ID = @id";

        private const string GetJobFromExecutionIdQuery = "SELECT ji.JOB_INSTANCE_ID, JOB_NAME, JOB_KEY, ji.VERSION from {0}JOB_INSTANCE ji, {0}JOB_EXECUTION je where JOB_EXECUTION_ID = @id and ji.JOB_INSTANCE_ID = je.JOB_INSTANCE_ID";

        private const string FindJobNamesQuery = "SELECT distinct JOB_NAME from {0}JOB_INSTANCE order by JOB_NAME";

        private const string FindLastJobsByNameQuery = "SELECT JOB_INSTANCE_ID, JOB_NAME from {0}JOB_INSTANCE where JOB_NAME = @jobName order by JOB_INSTANCE_ID desc";

        #endregion

        /// <summary>
        /// Row mapper for job instances.
        /// </summary>
        private readonly RowMapper<JobInstance> _rowMapper = (dataRecord, i) =>
        {
            var wrapper = new DataRecordWrapper(dataRecord);
            var jobInstance = new JobInstance(wrapper.Get<long>(0), wrapper.Get<string>(1));
            jobInstance.IncrementVersion();
            return jobInstance;
        };

        private IDataFieldMaxValueIncrementer _jobIncrementer;

        private readonly IJobKeyGenerator<JobParameters> _jobKeyGenerator = new DefaultJobKeyGenerator();

        /// <summary>
        /// The data field incrementer for job instance ids.
        /// </summary>
        public IDataFieldMaxValueIncrementer JobIncrementer { set { _jobIncrementer = value; } }

        #region IJobInstanceDao methods implementation

        /// <summary>
        /// Creates a job instance with given name and parameters.
        /// A job instance with the same name and parameters should not already exist.
        /// </summary>
        /// <param name="jobName">the job name</param>
        /// <param name="jobParameters">job parameters</param>
        /// <returns>a new persisted job instance</returns>
        public JobInstance CreateJobInstance(string jobName, JobParameters jobParameters)
        {
            Assert.NotNull(jobName, "Job name must not be null");
            Assert.NotNull(jobParameters, "Job parameters must not be null");
            Assert.State(GetJobInstance(jobName, jobParameters) == null,
                "A job instance with this combination of name and parameters must not already exist");

            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionOptions))
            {
                var jobId = _jobIncrementer.NextLong();

                var jobInstance = new JobInstance(jobId, jobName);
                jobInstance.IncrementVersion();

                var parameters = new Dictionary<string, object>
                {
                    { "id", jobId }, { "jobName", jobName }, { "key", _jobKeyGenerator.GenerateKey(jobParameters) }, { "version", jobInstance.Version }
                };

                DbOperator.Update(InsertTablePrefix(CreateJobInstanceQuery), parameters);

                scope.Complete();
                return jobInstance;
            }
        }

        ///<summary>
        /// Returns the job instance for the given job name and parameters.
        /// </summary>       
        /// <param name="jobName">a job name</param>
        /// <param name="jobParameters">job parameters</param>
        /// <returns>the job instance with the given name and parameters or <c>null</c> if it does not exist</returns>
        public JobInstance GetJobInstance(string jobName, JobParameters jobParameters)
        {
            Assert.NotNull(jobName, "Job name must not be null");
            Assert.NotNull(jobParameters, "Job parameters must not be null");

            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionOptions))
            {
                var jobKey = _jobKeyGenerator.GenerateKey(jobParameters);

                var parameters = new Dictionary<string, object> { { "jobName", jobName }, { "key", jobKey } };
                var instances = DbOperator.Select(InsertTablePrefix(string.IsNullOrWhiteSpace(jobKey) ? FindJobsWithEmptyKey : FindJobsWithKeyQuery),
                    _rowMapper, parameters);

                if (instances.Count == 0)
                {
                    return null;
                }
                Assert.State(instances.Count == 1);

                scope.Complete();
                return instances[0];
            }
        }

        /// <param name="instanceId">an id</param>
        /// <returns>the job instance with the given id or <c>null</c> if it does not exist</returns>
        public JobInstance GetJobInstance(long instanceId)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionOptions))
            {
                var instances = DbOperator.Select(InsertTablePrefix(GetJobFromIdQuery), _rowMapper, new Dictionary<string, object> { { "id", instanceId } });
                var result = instances.Count == 0 ? null : instances[0];

                scope.Complete();
                return result;
            }
        }

        /// <param name="jobExecution">a job execution</param>
        /// <returns>the job instance for the given job execution or <c>null</c> if it does not exist</returns>
        public JobInstance GetJobInstance(JobExecution jobExecution)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionOptions))
            {
                var instances = DbOperator.Select(InsertTablePrefix(GetJobFromExecutionIdQuery), _rowMapper,
                    new Dictionary<string, object> { { "id", jobExecution.Id } });
                var result = instances.Count == 0 ? null : instances[0];

                scope.Complete();
                return result;
            }
        }

        /// <summary>
        /// Fetches a list of of job instances ordered by descending primary key.
        /// </summary>
        /// <param name="jobName">the name of a job</param>
        /// <param name="start">the index of the first instance to return</param>
        /// <param name="count">the number of instances to return</param>
        /// <returns>a list containing the requested job instances</returns>
        public IList<JobInstance> GetJobInstances(string jobName, int start, int count)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionOptions))
            {
                var result = DbOperator.Select(InsertTablePrefix(FindLastJobsByNameQuery), GetJobInstanceExtractor(start, count),
                    new Dictionary<string, object>() { { "jobName", jobName } });

                scope.Complete();
                return result;
            }
        }

        /// <returns>the list of all the job names, sorted ascendingly</returns>
        public IList<string> GetJobNames()
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionOptions))
            {
                var result = DbOperator.Select(InsertTablePrefix(FindJobNamesQuery), (dr, i) => dr.GetString(0), new Dictionary<string, object>());

                scope.Complete();
                return result;
            }
        }

        /// <param name="jobName">a job name</param>
        /// <returns>the number of job instances for the given job name</returns>
        /// <exception cref="Summer.Batch.Core.Launch.NoSuchJobException">if there are no job instances for this job name.</exception>
        public int GetJobInstanceCount(string jobName)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionOptions))
            {
                var count = DbOperator.Query<int>(InsertTablePrefix(CountJobsWithNameQuery), new Dictionary<string, object> { { "jobName", jobName } });
                if (count == 0)
                {
                    throw new NoSuchJobException(string.Format("No job instances for job name {0} were found", jobName));
                }

                scope.Complete();
                return count;
            }
        }

        #endregion

        #region private utility methods

        /// <summary>
        /// Creates a data extractor returning a list of job instances from the specified rows.
        /// </summary>
        /// <param name="start">the zero-based index of the first row to extract</param>
        /// <param name="count">the number of rows to extract</param>
        /// <returns>a data reader extractor for the specified rows</returns>
        private DataReaderExtractor<IList<JobInstance>> GetJobInstanceExtractor(int start, int count)
        {
            return dataReader =>
            {
                IList<JobInstance> result = new List<JobInstance>();
                var rowNum = 0;
                while (rowNum < start && dataReader.Read())
                {
                    rowNum++;
                }
                while (rowNum < start + count && dataReader.Read())
                {
                    result.Add(_rowMapper(dataReader, rowNum));
                    rowNum++;
                }
                return result;
            };
        }

        #endregion
    }
}
