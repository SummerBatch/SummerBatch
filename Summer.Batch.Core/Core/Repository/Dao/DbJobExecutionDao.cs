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

using NLog;
using Summer.Batch.Core.Launch;
using Summer.Batch.Data;
using Summer.Batch.Data.Incrementer;
using Summer.Batch.Common.Util;
using System;
using System.Collections.Generic;
using System.Transactions;
using Summer.Batch.Common.Collections;
using ParameterType = Summer.Batch.Core.JobParameter.ParameterType;

namespace Summer.Batch.Core.Repository.Dao
{
    /// <summary>
    /// Database implementation of <see cref="IJobExecutionDao"/>. Uses sequences (via 
    /// <see cref="IDataFieldMaxValueIncrementer"/> abstraction) to create all primary keys
    /// before inserting a new row. Objects are checked to ensure all mandatory
    /// fields to be stored are not null. If any are found to be null, an
    /// <see cref="ArgumentException"/> will be thrown.
    /// </summary>
    public class DbJobExecutionDao : AbstractDbBatchMetadataDao, IJobExecutionDao
    {
        #region Queries

        private const string SaveJobExecutionQuery = "INSERT into {0}JOB_EXECUTION(JOB_EXECUTION_ID, JOB_INSTANCE_ID, START_TIME, END_TIME, STATUS, EXIT_CODE, EXIT_MESSAGE, VERSION, CREATE_TIME, LAST_UPDATED, JOB_CONFIGURATION_LOCATION) values (@executionId, @jobId, @startTime, @endTime, @status, @exitCode, @exitMessage, @version, @createTime, @lastUpdated, @jobConfigurationLocation)";

        private const string CheckJobExecutionExistsQuery = "SELECT COUNT(1) FROM {0}JOB_EXECUTION WHERE JOB_EXECUTION_ID = @id";

        private const string GetStatusQuery = "SELECT STATUS from {0}JOB_EXECUTION where JOB_EXECUTION_ID = @id";

        private const string UpdateJobExecutionQuery = "UPDATE {0}JOB_EXECUTION set START_TIME = @startTime, END_TIME = @endTime,  STATUS = @status, EXIT_CODE = @exitCode, EXIT_MESSAGE = @exitMessage, VERSION = @newVersion, CREATE_TIME = @createTime, LAST_UPDATED = @lastUpdated where JOB_EXECUTION_ID = @id and VERSION = @version";

        private const string FindJobExecutionsQuery = "SELECT JOB_EXECUTION_ID, START_TIME, END_TIME, STATUS, EXIT_CODE, EXIT_MESSAGE, CREATE_TIME, LAST_UPDATED, VERSION, JOB_CONFIGURATION_LOCATION from {0}JOB_EXECUTION where JOB_INSTANCE_ID = @id order by JOB_EXECUTION_ID desc";

        private const string GetLastExecutionQuery = "SELECT JOB_EXECUTION_ID, START_TIME, END_TIME, STATUS, EXIT_CODE, EXIT_MESSAGE, CREATE_TIME, LAST_UPDATED, VERSION, JOB_CONFIGURATION_LOCATION from {0}JOB_EXECUTION E where JOB_INSTANCE_ID = @id and JOB_EXECUTION_ID in (SELECT max(JOB_EXECUTION_ID) from {0}JOB_EXECUTION E2 where E2.JOB_INSTANCE_ID = @id)";

        private const string GetExecutionByIdQuery = "SELECT JOB_EXECUTION_ID, START_TIME, END_TIME, STATUS, EXIT_CODE, EXIT_MESSAGE, CREATE_TIME, LAST_UPDATED, VERSION, JOB_CONFIGURATION_LOCATION from {0}JOB_EXECUTION where JOB_EXECUTION_ID = @id";

        private const string GetRunningExecutionsQuery = "SELECT E.JOB_EXECUTION_ID, E.START_TIME, E.END_TIME, E.STATUS, E.EXIT_CODE, E.EXIT_MESSAGE, E.CREATE_TIME, E.LAST_UPDATED, E.VERSION, E.JOB_CONFIGURATION_LOCATION from {0}JOB_EXECUTION E, {0}JOB_INSTANCE I where E.JOB_INSTANCE_ID=I.JOB_INSTANCE_ID and I.JOB_NAME=@jobName and E.END_TIME is NULL order by E.JOB_EXECUTION_ID desc";

        private const string CurrentVersionJobExecutionQuery = "SELECT VERSION FROM {0}JOB_EXECUTION WHERE JOB_EXECUTION_ID=@id";

        private const string FindParamsFromIdQuery = "SELECT JOB_EXECUTION_ID, KEY_NAME, TYPE_CD, STRING_VAL, DATE_VAL, LONG_VAL, DOUBLE_VAL, IDENTIFYING from {0}JOB_EXECUTION_PARAMS where JOB_EXECUTION_ID = @id";

        private const string CreateJobParametersQuery = "INSERT into {0}JOB_EXECUTION_PARAMS(JOB_EXECUTION_ID, KEY_NAME, TYPE_CD, STRING_VAL, DATE_VAL, LONG_VAL, DOUBLE_VAL, IDENTIFYING) values (@id, @key, @type, @stringValue, @dateValue, @longValue, @doubleValue, @identifying)";

        #endregion

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private int _exitMessageLength = DefaultExitMessageLength;
        private IDataFieldMaxValueIncrementer _jobExecutionIncrementer;

        /// <summary>
        /// The exit message maximum length.
        /// </summary>
        public int ExitMessageLength { set { _exitMessageLength = value; } }

        /// <summary>
        /// The data field incrementer for job execution ids.
        /// </summary>
        public IDataFieldMaxValueIncrementer JobIncrementer { set { _jobExecutionIncrementer = value; } }

        #region IJobExecutionDao methods implementation

        /// <summary>
        /// Persists a new job execution.
        /// The corresponding job instance must have been persisted.
        /// </summary>
        /// <param name="jobExecution">a job execution</param>
        public void SaveJobExecution(JobExecution jobExecution)
        {
            Assert.NotNull(jobExecution, "The job execution must not be null.");
            Assert.NotNull(jobExecution.GetJobId(), "The corresponding job id must not be null.");

            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionOptions))
            {
                jobExecution.IncrementVersion();
                jobExecution.Id = _jobExecutionIncrementer.NextLong();

                var parameters = new Dictionary<string, object>
                {
                    { "executionId", jobExecution.Id },
                    { "jobId", jobExecution.GetJobId() },
                    { "startTime",(object) jobExecution.StartTime ?? DBNull.Value },
                    { "endTime", (object) jobExecution.EndTime ?? DBNull.Value },
                    { "status", jobExecution.Status.ToString() },
                    { "exitCode", jobExecution.ExitStatus.ExitCode },
                    { "exitMessage", jobExecution.ExitStatus.ExitDescription },
                    { "version", jobExecution.Version },
                    { "createTime", jobExecution.CreateTime },
                    { "lastUpdated", (object) jobExecution.LastUpdated ?? DBNull.Value },
                    { "jobConfigurationLocation", (object) jobExecution.JobConfigurationName ?? DBNull.Value }
                };

                DbOperator.Update(InsertTablePrefix(SaveJobExecutionQuery), parameters);

                InsertJobParameters(jobExecution.Id, jobExecution.JobParameters);
                scope.Complete();
            }
        }

        /// <summary>
        /// Updates the updates of a job execution.
        /// The job execution must have already been persisted.
        /// </summary>
        /// <param name="jobExecution">a job execution</param>
        public void UpdateJobExecution(JobExecution jobExecution)
        {
            Assert.NotNull(jobExecution, "The job execution must not be null.");
            Assert.NotNull(jobExecution.GetJobId(), "The corresponding job id must not be null.");
            Assert.NotNull(jobExecution.Id, "The job execution id must not be null. The execution must have already been saved.");
            Assert.NotNull(jobExecution.Version, "The job execution version must not be null. The execution must have already been saved.");

            lock (jobExecution)
                using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionOptions))
                {
                    var version = (jobExecution.Version ?? 0) + 1;
                    var exitDescription = jobExecution.ExitStatus.ExitDescription;
                    if (exitDescription != null && exitDescription.Length > _exitMessageLength)
                    {
                        exitDescription = exitDescription.Substring(0, _exitMessageLength);
                        Logger.Debug("Truncating long message before updating job execution: {0}", jobExecution);
                    }

                    if (DbOperator.Query<int>(InsertTablePrefix(CheckJobExecutionExistsQuery), new Dictionary<string, object> { { "id", jobExecution.Id } }) != 1)
                    {
                        throw new NoSuchJobException(string.Format("Invalid job execution: no job execution with id {0} found.", jobExecution.Id));
                    }

                    var parameters = new Dictionary<string, object>
                {
                    { "startTime", (object) jobExecution.StartTime ?? DBNull.Value },
                    { "endTime", (object) jobExecution.EndTime ?? DBNull.Value },
                    { "status", jobExecution.Status.ToString() },
                    { "exitCode", jobExecution.ExitStatus.ExitCode },
                    { "exitMessage", exitDescription },
                    { "newVersion", version },
                    { "createTime", jobExecution.CreateTime },
                    { "lastUpdated", (object) jobExecution.LastUpdated ?? DBNull.Value },
                    { "id", jobExecution.Id },
                    { "version", jobExecution.Version }
                };

                    var count = DbOperator.Update(InsertTablePrefix(UpdateJobExecutionQuery), parameters);

                    if (count == 0)
                    {
                        var currentVersion = DbOperator.Query<long>(InsertTablePrefix(CurrentVersionJobExecutionQuery),
                                                                    new Dictionary<string, object> { { "id", jobExecution.Id } });
                        throw new ArgumentException(string.Format("Attempt to update job id={0} with version {1} but current version is {2}", jobExecution.Id,
                            jobExecution.Version, currentVersion));
                    }

                    jobExecution.IncrementVersion();

                    scope.Complete();
                }
        }

        /// <summary>
        /// Finds all the job executions for a job instance,
        /// sorted by descending creation order (the first element is the most recent).
        /// </summary>
        /// <param name="jobInstance">a job instance</param>
        /// <returns>a list of job executions</returns>
        public IList<JobExecution> FindJobExecutions(JobInstance jobInstance)
        {
            Assert.NotNull(jobInstance, "The job must not be null.");
            Assert.NotNull(jobInstance.Id, "the job id must not be null.");

            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionOptions))
            {
                var result = DbOperator.Select(InsertTablePrefix(FindJobExecutionsQuery),
                    GetJobExecutionRowMapper(jobInstance),
                    new Dictionary<string, object> { { "id", jobInstance.Id } });
                scope.Complete();
                return result;
            }
        }

        /// <summary>
        /// Returns the last job execution for a given job instance.
        /// </summary>
        /// <param name="jobInstance">a job instance</param>
        /// <returns>the last created job execution for the job instance</returns>
        public JobExecution GetLastJobExecution(JobInstance jobInstance)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionOptions))
            {
                var executions = DbOperator.Select(InsertTablePrefix(GetLastExecutionQuery),
                    GetJobExecutionRowMapper(jobInstance),
                    new Dictionary<string, object> { { "id", jobInstance.Id } });

                var result = executions.Count > 0 ? executions[0] : null;

                scope.Complete();
                return result;
            }
        }

        /// <summary>
        /// Returns all running job executions for the given job name.
        /// </summary>
        /// <param name="jobName">a job name</param>
        /// <returns>a set containing the job executions that are still running for the specified job name</returns>
        public ISet<JobExecution> FindRunningJobExecutions(string jobName)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionOptions))
            {
                IEnumerable<JobExecution> executions = DbOperator.Select(InsertTablePrefix(GetRunningExecutionsQuery),
                    GetJobExecutionRowMapper(),
                    new Dictionary<string, object> { { "jobName", jobName } });

                var result = new HashSet<JobExecution>(executions);

                scope.Complete();
                return result;
            }
        }

        /// <summary>
        /// Returns the job execution for the given execution id.
        /// </summary>
        /// <param name="executionId">an id for an existing job execution</param>
        /// <returns>the job execution with the given id</returns>
        public JobExecution GetJobExecution(long executionId)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionOptions))
            {
                var executions = DbOperator.Select(InsertTablePrefix(GetExecutionByIdQuery),
                    GetJobExecutionRowMapper(),
                    new Dictionary<string, object> { { "id", executionId } });

                var result = executions.Count == 0 ? null : executions[0];

                scope.Complete();
                return result;
            }
        }

        /// <summary>
        /// Persists the status and version fields of a job execution. 
        /// The job execution must have already been persisted.
        /// </summary>
        /// <param name="jobExecution"></param>
        public void SynchronizeStatus(JobExecution jobExecution)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionOptions))
            {
                var currentVersion = DbOperator.Query<long>(InsertTablePrefix(CurrentVersionJobExecutionQuery),
                    new Dictionary<string, object> { { "id", jobExecution.Id } });

                if (currentVersion != jobExecution.Version)
                {
                    var status = DbOperator.Query<string>(InsertTablePrefix(GetStatusQuery), new Dictionary<string, object> { { "id", jobExecution.Id } });
                    jobExecution.UpgradeStatus(BatchStatus.ValueOf(status));
                    jobExecution.Version = (int?)currentVersion;
                }

                scope.Complete();
            }
        }

        #endregion

        #region private utility methods

        /// <summary>
        /// Inserts job parameters into the job parameters table.
        /// </summary>
        /// <param name="executionId">the id of the job execution</param>
        /// <param name="jobParameters">job parameters to insert</param>
        private void InsertJobParameters(long? executionId, JobParameters jobParameters)
        {
            foreach (var pair in jobParameters)
            {
                var jobParameter = pair.Value;
                InsertParameter(executionId, jobParameter.Type, pair.Key, jobParameter.Value, jobParameter.Identifying);
            }
        }

        /// <summary>
        /// Inserts a parameter into the job parameters table.
        /// </summary>
        /// <param name="executionId">the id of the job execution</param>
        /// <param name="type">the type of the parameter</param>
        /// <param name="key">the key of the parameter</param>
        /// <param name="value">the value of the parameter</param>
        /// <param name="identifying">whether the parameter is part of the job instance identification</param>
        private void InsertParameter(long? executionId, ParameterType type, string key, object value, bool identifying)
        {
            var stringValue = type == ParameterType.String ? value : "";
            var dateValue = type == ParameterType.Date ? value : DBNull.Value;
            var longValue = type == ParameterType.Long ? value : 0L;
            var doubleValue = type == ParameterType.Double ? value : 0D;

            var parameters = new Dictionary<string, object>
            {
                {"id", executionId}, {"key", key}, {"type", type.ToString()}, {"stringValue", stringValue},
                {"dateValue", dateValue}, { "longValue", longValue}, {"doubleValue", doubleValue},
                {"identifying", identifying ? "Y" : "N"}
            };

            DbOperator.Update(InsertTablePrefix(CreateJobParametersQuery), parameters);
        }

        /// <summary>
        /// Retrieves job parameters for an execution.
        /// </summary>
        /// <param name="executionId">the job execution id</param>
        /// <returns>the job parameters of that execution</returns>
        private JobParameters GetJobParameters(long executionId)
        {
            var parameters = new OrderedDictionary<string, JobParameter>(16);

            RowHandler handler = dataRecord =>
            {
                var wrapper = new DataRecordWrapper(dataRecord);
                var type = (ParameterType)Enum.Parse(typeof(ParameterType), wrapper.Get<string>(2));
                JobParameter parameter;

                switch (type)
                {
                    case ParameterType.String:
                        parameter = new JobParameter(wrapper.Get<string>(3), string.Equals(wrapper.Get<string>(7), "Y",
                            StringComparison.OrdinalIgnoreCase));
                        break;
                    case ParameterType.Long:
                        parameter = new JobParameter(wrapper.Get<long>(5), string.Equals(wrapper.Get<string>(7), "Y",
                            StringComparison.OrdinalIgnoreCase));
                        break;
                    case ParameterType.Double:
                        parameter = new JobParameter(wrapper.Get<double>(6), string.Equals(wrapper.Get<string>(7), "Y",
                            StringComparison.OrdinalIgnoreCase));
                        break;
                    case ParameterType.Date:
                        parameter = new JobParameter(wrapper.Get<DateTime>(4), string.Equals(wrapper.Get<string>(7), "Y",
                            StringComparison.OrdinalIgnoreCase));
                        break;
                    default:
                        throw new InvalidOperationException("Unsupported type :[" + type + "]");//should never happen
                }

                parameters.Add(wrapper.Get<string>(1), parameter);
            };

            DbOperator.Select(InsertTablePrefix(FindParamsFromIdQuery), handler, new Dictionary<string, object> { { "id", executionId } });

            return new JobParameters(parameters);
        }

        /// <summary>
        /// Creates a row mapper for job executions.
        /// </summary>
        /// <param name="jobInstance">the job instance of the executions to create (optional)</param>
        /// <returns>a row mapper for job executions</returns>
        private RowMapper<JobExecution> GetJobExecutionRowMapper(JobInstance jobInstance = null)
        {
            JobParameters jobParameters = null;
            return (dataRecord, i) =>
            {
                var wrapper = new DataRecordWrapper(dataRecord);
                var id = wrapper.Get<long>(0);
                var jobConfigurationLocation = wrapper.Get<string>(9);
                if (jobParameters == null)
                {
                    jobParameters = GetJobParameters(id);
                }

                var jobExecution = jobInstance == null ? new JobExecution(id, jobParameters, jobConfigurationLocation)
                                                       : new JobExecution(jobInstance, id, jobParameters, jobConfigurationLocation);

                jobExecution.StartTime = wrapper.Get<DateTime?>(1);
                jobExecution.EndTime = wrapper.Get<DateTime?>(2);
                jobExecution.Status = dataRecord.IsDBNull(3) ? null : BatchStatus.ValueOf(dataRecord.GetString(3));
                jobExecution.ExitStatus = new ExitStatus(wrapper.Get<string>(4), wrapper.Get<string>(5));
                jobExecution.CreateTime = wrapper.Get<DateTime>(6);
                jobExecution.LastUpdated = wrapper.Get<DateTime?>(7);
                jobExecution.Version = wrapper.Get<int?>(8);
                return jobExecution;
            };
        }

        #endregion
    }
}
