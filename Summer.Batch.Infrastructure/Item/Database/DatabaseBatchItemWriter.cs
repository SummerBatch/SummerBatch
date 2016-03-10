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
//  distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using NLog;
using Summer.Batch.Common.Factory;
using Summer.Batch.Common.Util;
using Summer.Batch.Data;
using Summer.Batch.Data.Parameter;

namespace Summer.Batch.Infrastructure.Item.Database
{
    /// <summary>
    /// Implementation of <see cref="T:IItemWriter"/> that writes items in a database using a
    /// <see cref="T:IQueryParameterSourceProvider"/> to retrieve the parameter values for each update/insert.
    /// </summary>
    /// <typeparam name="T">&nbsp;</typeparam>
    public class DatabaseBatchItemWriter<T> : IItemWriter<T>, IInitializationPostOperations where T:class
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private DbOperator _dbOperator;

        /// <summary>
        /// The <see cref="ConnectionStringSettings"/> used to  connect to the database on which
        /// the query will executed.
        /// </summary>
        public ConnectionStringSettings ConnectionString
        {
            set
            {
                _dbOperator = new DbOperator
                {
                    ConnectionProvider = new ConnectionProvider { ConnectionStringSettings = value }
                };
            }
        }

        /// <summary>
        /// The sql query. It is expected to be a insert or update query.
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// The <see cref="T:IQueryParameterSourceProvider"/> that will provide a <see cref="IQueryParameterSource"/>
        /// for each item to write.
        /// </summary>
        public IQueryParameterSourceProvider<T> DbParameterSourceProvider { get; set; }

        /// <summary>
        /// Whether to check if rows have actually been updated.
        /// Defaults to true (see default constructor).
        /// </summary>
        public bool AssertUpdates { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DatabaseBatchItemWriter()
        {
            AssertUpdates = true;
        }

        /// <summary>
        /// Checks that the required properties are correctly set.
        /// </summary>
        public void AfterPropertiesSet()
        {
            Assert.NotNull(_dbOperator, "DbOperator must be provided");
            Assert.NotNull(Query, "The sql query must be provided");
            Assert.NotNull(DbParameterSourceProvider, "DbParameterSourceProvider must be provided");
        }

        /// <summary>
        /// Writes the items in the database
        /// </summary>
        /// <param name="items">the items to write</param>
        public void Write(IList<T> items)
        {
            _logger.Debug("Executing batch database writer with {0} items.", items.Count);

            if (items.Count == 0)
            {
                _logger.Warn("Executing batch database writer : empty list of items has been given.");
                return;
            }


            var parameterSources = items.Select(i => DbParameterSourceProvider.CreateParameterSource(i)).ToList();
            var updateCounts = _dbOperator.BatchUpdate(Query, parameterSources);

            if (AssertUpdates)
            {
                for (var i = 0; i < updateCounts.Length; i++)
                {
                    if (updateCounts[i] == 0)
                    {
                        throw new EmptyUpdateException(string.Format("Item {0} of {1} did not update any rows: [{2}]",
                            i, updateCounts.Length, items[i]));
                    }
                }
            }
        }
    }
}