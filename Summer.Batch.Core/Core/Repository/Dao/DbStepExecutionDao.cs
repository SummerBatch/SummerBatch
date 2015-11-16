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
using Summer.Batch.Data;
using Summer.Batch.Data.Incrementer;
using Summer.Batch.Common.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace Summer.Batch.Core.Repository.Dao
{
    /// <summary>
    /// Database implementation of <see cref="IStepExecutionDao"/>.
    /// Allows customization of the tables names used for step meta data via a prefix property.
    /// Uses sequences or tables (via <see cref="IDataFieldMaxValueIncrementer"/>
    /// abstraction) to create all primary keys before inserting a new row. All
    /// objects are checked to ensure all fields to be stored are not null. If any
    /// are found to be null, an ArgumentException will be thrown. 
    /// </summary>
    public class DbStepExecutionDao : AbstractDbBatchMetadataDao, IStepExecutionDao
    {
        #region Queries

        private const string SaveStepExecutionQuery = "INSERT into {0}STEP_EXECUTION(STEP_EXECUTION_ID, VERSION, STEP_NAME, JOB_EXECUTION_ID, START_TIME, END_TIME, STATUS, COMMIT_COUNT, READ_COUNT, FILTER_COUNT, WRITE_COUNT, EXIT_CODE, EXIT_MESSAGE, READ_SKIP_COUNT, WRITE_SKIP_COUNT, PROCESS_SKIP_COUNT, ROLLBACK_COUNT, LAST_UPDATED) values(@id, @version, @stepName, @jobId, @startTime, @endTime, @status, @commitCount, @readCount, @filterCount, @writeCount, @exitCode, @exitMessage, @readSkipCount, @writeSkipCount, @processSkipCount, @rollbackCount, @lastUpdated)";

        private const string UpdateStepExecutionQuery = "UPDATE {0}STEP_EXECUTION set START_TIME = @startTime, END_TIME = @endTime, STATUS = @status, COMMIT_COUNT = @commitCount, READ_COUNT = @readCount, FILTER_COUNT = @filterCount, WRITE_COUNT = @writeCount, EXIT_CODE = @exitCode, EXIT_MESSAGE = @exitMessage, VERSION = @newVersion, READ_SKIP_COUNT = @readSkipCount, PROCESS_SKIP_COUNT = @processSkipCount, WRITE_SKIP_COUNT = @writeSkipCount, ROLLBACK_COUNT = @rollbackCount, LAST_UPDATED = @lastUpdated where STEP_EXECUTION_ID = @id and VERSION = @version";

        private const string GetRawStepExecutionsQuery = "SELECT STEP_EXECUTION_ID, STEP_NAME, START_TIME, END_TIME, STATUS, COMMIT_COUNT, READ_COUNT, FILTER_COUNT, WRITE_COUNT, EXIT_CODE, EXIT_MESSAGE, READ_SKIP_COUNT, WRITE_SKIP_COUNT, PROCESS_SKIP_COUNT, ROLLBACK_COUNT, LAST_UPDATED, VERSION from {0}STEP_EXECUTION where JOB_EXECUTION_ID = @jobId";

        private const string GetStepExecutionsQuery = GetRawStepExecutionsQuery + " order by STEP_EXECUTION_ID";

        private const string GetStepExecutionQuery = GetRawStepExecutionsQuery + " and STEP_EXECUTION_ID = @stepId";

        private const string CurrentVersionStepExecutionQuery = "SELECT VERSION FROM {0}STEP_EXECUTION WHERE STEP_EXECUTION_ID=@id";

        #endregion

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private int _exitMessageLength = DefaultExitMessageLength;

        private IDataFieldMaxValueIncrementer _stepExecutionIncrementer;

        /// <summary>
        /// The exit message maximum length.
        /// </summary>
        public int ExitMessageLength { set { _exitMessageLength = value; } }

        /// <summary>
        /// The data field incrementer for job execution ids.
        /// </summary>
        public IDataFieldMaxValueIncrementer StepIncrementer { set { _stepExecutionIncrementer = value; } }

        #region IStepExecutionDao methods implementation

        /// <summary>
        /// Persists the given step execution. It must not have been persisted yet.
        /// </summary>
        /// <param name="stepExecution">the stepExecution to persist</param>
        public void SaveStepExecution(StepExecution stepExecution)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionOptions))
            {
                DbOperator.Update(InsertTablePrefix(SaveStepExecutionQuery), BuildStepExecutionParameters(stepExecution));
                scope.Complete();
            }
        }

        /// <summary>
        /// Persists the step executions in a collection. The step executions must not have been persisted yet.
        /// </summary>
        /// <param name="stepExecutions">a collection of step executions to persists</param>
        public void SaveStepExecutions(ICollection<StepExecution> stepExecutions)
        {
            Assert.NotNull(stepExecutions, "Attempt to save a null collection of step executions");

            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionOptions))
            {
                if (stepExecutions.Count > 0)
                {
                    var parameters = stepExecutions.Select(BuildStepExecutionParameters).ToList();
                    DbOperator.BatchUpdate(InsertTablePrefix(SaveStepExecutionQuery), parameters);
                }
                scope.Complete();
            }
        }

        /// <summary>
        /// Persits the updates of a step execution. It must have already been persisted.
        /// </summary>
        /// <param name="stepExecution">a persisted step execution</param>
        public void UpdateStepExecution(StepExecution stepExecution)
        {
            Assert.NotNull(stepExecution, "the step execution must not be null");
            Assert.NotNull(stepExecution.StepName, "the step execution name must not be null");
            Assert.NotNull(stepExecution.Id, "the step execution must have already been saved and have an id");

            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionOptions))
            {
                var exitDescription = TruncateExitDescription(stepExecution.ExitStatus.ExitDescription);

                lock (stepExecution)
                {
                    var version = stepExecution.Version + 1;
                    var parameters = new Dictionary<string, object>
                    {
                        {"startTime", stepExecution.StartTime}, {"endTime", (object) stepExecution.EndTime ?? DBNull.Value},
                        {"status", stepExecution.BatchStatus.ToString()}, {"commitCount", stepExecution.CommitCount},
                        {"readCount", stepExecution.ReadCount}, {"filterCount", stepExecution.FilterCount}, {"writeCount", stepExecution.WriteCount},
                        {"exitCode", stepExecution.ExitStatus.ExitCode}, {"exitMessage", exitDescription}, {"newVersion", version},
                        {"readSkipCount", stepExecution.ReadSkipCount}, {"processSkipCount", stepExecution.ProcessSkipCount},
                        {"writeSkipCount", stepExecution.WriteSkipCount}, {"rollbackCount", stepExecution.RollbackCount},
                        {"lastUpdated", (object) stepExecution.LastUpdated ?? DBNull.Value}, {"id", stepExecution.Id}, {"version", stepExecution.Version}
                    };
                    var count = DbOperator.Update(InsertTablePrefix(UpdateStepExecutionQuery), parameters);

                    if (count == 0)
                    {
                        var currentVersion = DbOperator.Query<int>(InsertTablePrefix(CurrentVersionStepExecutionQuery),
                            new Dictionary<string, object> { { "id", stepExecution.Id } });
                        throw new ArgumentException(string.Format("Attempt to update step id={0} with version {1} but current version is {2}",
                            stepExecution.Id, stepExecution.Version, currentVersion));
                    }

                    stepExecution.IncrementVersion();
                }

                scope.Complete();
            }
        }

        /// <param name="jobExecution">a job execution</param>
        /// <param name="stepExecutionId">a step execution id</param>
        /// <returns>the step execution with the given id in the given job execution</returns>
        public StepExecution GetStepExecution(JobExecution jobExecution, long stepExecutionId)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionOptions))
            {
                var executions = DbOperator.Select(InsertTablePrefix(GetStepExecutionQuery),
                    GetStepExecutionRowMapper(jobExecution),
                    new Dictionary<string, object> { { "jobId", jobExecution.Id }, { "stepId", stepExecutionId } });

                var result = executions.Count == 0 ? null : executions[0];

                scope.Complete();
                return result;
            }
        }

        /// <summary>
        /// Adds persisted step executions to a job execution.
        /// </summary>
        /// <param name="jobExecution">a job execution</param>
        public void AddStepExecutions(JobExecution jobExecution)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionOptions))
            {
                DbOperator.Select(InsertTablePrefix(GetStepExecutionsQuery),
                    GetStepExecutionRowMapper(jobExecution),
                    new Dictionary<string, object> { { "jobId", jobExecution.Id } });
                scope.Complete();
            }
        }


        #endregion

        #region private utility methods

        /// <summary>
        /// Creates the parameters for inserting a new step execution in the database.
        /// </summary>
        /// <param name="stepExecution">the step execution to insert</param>
        /// <returns>an array containing the parameters</returns>
        private IDictionary<string, object> BuildStepExecutionParameters(StepExecution stepExecution)
        {
            Assert.NotNull(stepExecution, "the step execution must not be null");
            Assert.NotNull(stepExecution.StepName, "the step execution name must not be null");
            Assert.IsNull(stepExecution.Id, "the step execution must not already have an id");
            Assert.IsNull(stepExecution.Version, "the step execution must not already have a version");

            stepExecution.Id = _stepExecutionIncrementer.NextLong();
            stepExecution.IncrementVersion();

            var exitDescription = TruncateExitDescription(stepExecution.ExitStatus.ExitDescription);
            var parameters = new Dictionary<string, object>
            {
                {"id", stepExecution.Id}, {"version", stepExecution.Version}, {"stepName", stepExecution.StepName},
                {"jobId",stepExecution.GetJobExecutionId()}, {"startTime", stepExecution.StartTime},
                {"endTime", (object) stepExecution.EndTime ?? DBNull.Value}, {"status", stepExecution.BatchStatus.ToString()},
                {"commitCount", stepExecution.CommitCount}, {"readCount", stepExecution.ReadCount}, {"filterCount",stepExecution.FilterCount},
                {"writeCount", stepExecution.FilterCount}, {"exitCode", stepExecution.ExitStatus.ExitCode}, {"exitMessage", exitDescription},
                {"readSkipCount", stepExecution.ReadSkipCount}, {"writeSkipCount", stepExecution.WriteSkipCount},
                {"processSkipCount", stepExecution.ProcessSkipCount}, {"rollbackCount", stepExecution.RollbackCount},
                {"lastUpdated", (object) stepExecution.LastUpdated ?? DBNull.Value}
            };
            return parameters;
        }

        /// <summary>
        /// Checks the exit description message length.
        /// </summary>
        /// <param name="exitDescription">the exist description</param>
        /// <returns>the exit description, truncated if its length is greater than the maximum</returns>
        /// <seealso cref="ExitMessageLength"/>
        private string TruncateExitDescription(string exitDescription)
        {
            if (exitDescription != null && exitDescription.Length > _exitMessageLength)
            {
                _logger.Debug("Truncating long message before updating step execution. Original message: {0}", exitDescription);
                return exitDescription.Substring(0, _exitMessageLength);
            }
            return exitDescription;
        }

        /// <summary>
        /// Creates a row mapper that for step executions.
        /// </summary>
        /// <param name="jobExecution">the job execution to use when creating the step executions</param>
        /// <returns>a row mapper</returns>
        private static RowMapper<StepExecution> GetStepExecutionRowMapper(JobExecution jobExecution)
        {
            return (dataRecord, i) =>
            {
                var wrapper = new DataRecordWrapper(dataRecord);
                var stepExecution = new StepExecution(wrapper.Get<string>((1)), jobExecution, wrapper.Get<long>((0)))
                {
                    StartTime = wrapper.Get<DateTime>((2)),
                    EndTime = wrapper.Get<DateTime>((3)),
                    BatchStatus = BatchStatus.ValueOf(wrapper.Get<string>((4))),
                    CommitCount = wrapper.Get<int>((5)),
                    ReadCount = wrapper.Get<int>((6)),
                    FilterCount = wrapper.Get<int>((7)),
                    WriteCount = wrapper.Get<int>((8)),
                    ExitStatus = new ExitStatus(wrapper.Get<string>((9)), wrapper.Get<string>(10)),
                    ReadSkipCount = wrapper.Get<int>((11)),
                    WriteSkipCount = wrapper.Get<int>((12)),
                    ProcessSkipCount = wrapper.Get<int>((13)),
                    RollbackCount = wrapper.Get<int>((14)),
                    LastUpdated = wrapper.Get<DateTime?>(15),
                    Version = wrapper.Get<int?>((16))
                };
                return stepExecution;
            };
        }

        #endregion
    }
}
