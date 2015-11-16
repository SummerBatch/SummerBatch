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

using Summer.Batch.Data;
using Summer.Batch.Infrastructure.Item;
using Summer.Batch.Common.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Transactions;
using Summer.Batch.Data.Parameter;

namespace Summer.Batch.Core.Repository.Dao
{
    /// <summary>
    /// Database DAO for <see cref="ExecutionContext"/>
    /// Stores execution context data related to both Step and Job using
    /// a different table for each.
    /// </summary>
    public class DbExecutionContextDao : AbstractDbBatchMetadataDao, IExecutionContextDao
    {
        #region Queries
        // Parameter names constants
        private const string Context = "context";
        private const string Id = "id";

        private const string FindJobExecutionContextQuery = "SELECT SERIALIZED_CONTEXT FROM {0}JOB_EXECUTION_CONTEXT WHERE JOB_EXECUTION_ID = @id";

        private const string InsertJobExecutionContextQuery = "INSERT INTO {0}JOB_EXECUTION_CONTEXT (SERIALIZED_CONTEXT, JOB_EXECUTION_ID) VALUES(@context, @id)";

        private const string UpdateJobExecutionContextQuery = "UPDATE {0}JOB_EXECUTION_CONTEXT SET SERIALIZED_CONTEXT = @context WHERE JOB_EXECUTION_ID = @id";

        private const string FindStepExecutionContextQuery = "SELECT SERIALIZED_CONTEXT FROM {0}STEP_EXECUTION_CONTEXT WHERE STEP_EXECUTION_ID = @id";

        private const string InsertStepExecutionContextQuery = "INSERT INTO {0}STEP_EXECUTION_CONTEXT (SERIALIZED_CONTEXT, STEP_EXECUTION_ID) VALUES(@context, @id)";

        private const string UpdateStepExecutionContextQuery = "UPDATE {0}STEP_EXECUTION_CONTEXT SET SERIALIZED_CONTEXT = @context WHERE STEP_EXECUTION_ID = @id";

        #endregion

        #region IExecutionContextDao methods implementation

        /// <summary>
        /// Returns the execution context associated with the given job execution.
        /// <param name="jobExecution">a job execution</param>
        /// <returns>the execution context associated with the given job execution</returns>
        /// </summary>
        public ExecutionContext GetExecutionContext(JobExecution jobExecution)
        {
            return GetExecutionContext(jobExecution.Id, FindJobExecutionContextQuery);
        }

        /// <summary>
        /// Returns the execution associated with the given step execution.
        /// <param name="stepExecution">a step execution</param>
        /// <returns>the execution context associated with the given step execution</returns>
        /// </summary>
        public ExecutionContext GetExecutionContext(StepExecution stepExecution)
        {
            return GetExecutionContext(stepExecution.Id, FindStepExecutionContextQuery);
        }

        /// <summary>
        /// Persist the execution context associated with the given job execution.
        /// A persistent entry for the context should not exist yet.
        /// </summary>
        /// <param name="jobExecution">a job execution</param>
        public void SaveExecutionContext(JobExecution jobExecution)
        {
            PersistSerializedContext(jobExecution.Id, jobExecution.ExecutionContext, InsertJobExecutionContextQuery);
        }

        /// <summary>
        /// Persists the execution context associated with the given step execution.
        /// A persistent entry for the context should not exist yet.
        /// </summary>
        /// <param name="stepExecution">a step execution</param>
        public void SaveExecutionContext(StepExecution stepExecution)
        {
            PersistSerializedContext(stepExecution.Id, stepExecution.ExecutionContext, InsertStepExecutionContextQuery);
        }

        /// <summary>
        /// Save execution contexts.
        /// </summary>
        /// <param name="stepExecutions"></param>
        public void SaveExecutionContexts(ICollection<StepExecution> stepExecutions)
        {
            Assert.NotNull(stepExecutions, "Attempt to save a null collection of step executions");
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionOptions))
            {
                var serializedContexts = new Dictionary<long?, byte[]>();
                foreach (var stepExecution in stepExecutions)
                {
                    Assert.NotNull(stepExecution.Id, "The execution id must not be null");
                    Assert.NotNull(stepExecution.ExecutionContext, "The execution context must not be null");
                    serializedContexts[stepExecution.Id] = SerializeContext(stepExecution.ExecutionContext);
                }
                PersistSerializedContexts(serializedContexts, InsertStepExecutionContextQuery);
                scope.Complete();
            }
        }

        /// <summary>
        /// Persists the updates of the execution context associated with the given job execution.
        /// A persistent entry should already exist for this context.
        /// </summary>
        /// <param name="jobExecution">a job execution</param>
        public void UpdateExecutionContext(JobExecution jobExecution)
        {
            PersistSerializedContext(jobExecution.Id, jobExecution.ExecutionContext, UpdateJobExecutionContextQuery);
        }

        /// <summary>
        /// Persists the updates of the execution context associated with the given step execution.
        /// A persistent entry should already exist for this context.
        /// </summary>
        /// <param name="stepExecution">a step execution</param>
        public void UpdateExecutionContext(StepExecution stepExecution)
        {
            PersistSerializedContext(stepExecution.Id, stepExecution.ExecutionContext, UpdateStepExecutionContextQuery);
        }

        #endregion

        #region private members

        /// <summary>
        /// Retrieves an execution context using the given query
        /// </summary>
        /// <param name="executionId">the id of the execution context to get</param>
        /// <param name="query">the query to use</param>
        /// <returns>the persisted execution context if it exists, or a new one otherwise</returns>
        private ExecutionContext GetExecutionContext(long? executionId, string query)
        {
            Assert.NotNull(executionId, "The execution id must not be null");
            ExecutionContext executionContext;
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionOptions))
            {
                var results = DbOperator.Select(InsertTablePrefix(query), _executionContextMapper,
                    new Dictionary<string, object> { { Id, executionId } });
                executionContext = results.Count > 0 ? results[0] : new ExecutionContext();
                scope.Complete();
            }
            return executionContext;
        }

        /// <summary>
        /// Serialize an execution context using a binary serializer.
        /// </summary>
        /// <param name="context">the context to serialize</param>
        /// <returns>the byte array representing the context</returns>
        private static byte[] SerializeContext(ExecutionContext context)
        {
            try
            {
                return context.Serialize();
            }
            catch (SerializationException e)
            {
                throw new ArgumentException("Unable to serialize the execution context", e);
            }
        }

        /// <summary>
        /// Persists an execution context.
        /// </summary>
        /// <param name="executionId">the id of the corresponding job or step</param>
        /// <param name="executionContext">the execution context to persist</param>
        /// <param name="query">the query to use for persisting the execution context in the database</param>
        private void PersistSerializedContext(long? executionId, ExecutionContext executionContext, string query)
        {
            Assert.NotNull(executionId, "The execution id must not be null");
            Assert.NotNull(executionContext, "The execution context must not be null");

            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionOptions))
            {
                var serializedContext = SerializeContext(executionContext);

                var parameters = new Dictionary<string, object>
                {
                    { Context, serializedContext }, { Id, executionId }
                };

                DbOperator.Update(InsertTablePrefix(query), parameters);
                scope.Complete();
            }
        }

        /// <summary>
        /// Persists several execution contexts.
        /// </summary>
        /// <param name="serializedContexts">a dictionary associating the id of a job or step with its context execution</param>
        /// <param name="query">the query to use for persisting the execution contexts in the database</param>
        private void PersistSerializedContexts(IDictionary<long?, byte[]> serializedContexts, string query)
        {
            var parameterSourceProvider = new ExecutionContextParameterSourceProvider(serializedContexts);
            DbOperator.BatchUpdate(InsertTablePrefix(query),
                                   serializedContexts.Select(p => parameterSourceProvider.CreateParameterSource(p.Key)).ToList());
        }

        /// <summary>
        /// The row mapper that deserializes a persisted context execution
        /// </summary>
        private readonly RowMapper<ExecutionContext> _executionContextMapper = (dataRecord, i) =>
        {
            var serializedContext = (byte[])dataRecord[0];

            try
            {
                return serializedContext.Deserialize<ExecutionContext>();
            }
            catch (SerializationException e)
            {
                throw new ArgumentException("Unable to deserialize the execution context", e);
            }
        };

        #endregion

        #region internal class ExecutionContextParameterSourceProvider

        /// <summary>
        /// Implementation of <see cref="T:IQueryParameterSourceProvider"/> that takes a long and return
        /// a <see cref="DictionaryParameterSource"/> that has the long and short contexts properly set.
        /// </summary>
        internal class ExecutionContextParameterSourceProvider : IQueryParameterSourceProvider<long?>
        {
            private readonly IDictionary<long?, byte[]> _serializedContexts;

            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="serializedContexts">a dictionary associating an id to its serialized context</param>
            public ExecutionContextParameterSourceProvider(IDictionary<long?, byte[]> serializedContexts)
            {
                _serializedContexts = serializedContexts;
            }

            /// <summary>
            /// Creates a new <see cref="IQueryParameterSource"/> for the given id.
            /// </summary>
            /// <param name="item">an execution context id</param>
            /// <returns>a new parameter source for the given id</returns>
            public IQueryParameterSource CreateParameterSource(long? item)
            {
                byte[] context;
                if (item == null || !_serializedContexts.TryGetValue(item, out context))
                {
                    throw new InvalidOperationException(string.Format("Could not find serialized contexts for item {0}", item));
                }

                return new DictionaryParameterSource
                {
                    Parameters = new Dictionary<string, object> { { Id, item }, { Context, context } }
                };
            }
        }

        #endregion
    }
}
