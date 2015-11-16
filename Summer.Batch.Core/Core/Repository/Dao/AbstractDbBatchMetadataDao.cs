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

using System.Transactions;
using Summer.Batch.Common.Factory;
using Summer.Batch.Common.Util;
using Summer.Batch.Data;
using Summer.Batch.Data.Parameter;

namespace Summer.Batch.Core.Repository.Dao
{
    /// <summary>
    /// Abstract super class for database implementations of the DAOs.
    /// </summary>
    public abstract class AbstractDbBatchMetadataDao : IInitializationPostOperations
    {
        /// <summary>
        /// The name of the setting for the table prefix.
        /// </summary>
        public const string TablePrefixSetting = "TablePrefix";

        /// <summary>
        /// Default table prefix constant.
        /// </summary>
        public const string DefaultTablePrefix = "BATCH_";

        /// <summary>
        /// Default exit message length constant.
        /// </summary>
        public const int DefaultExitMessageLength = 2500;

        /// <summary>
        /// The default transaction options for transaction scopes in the DAOs.
        /// </summary>
        protected static readonly TransactionOptions TransactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted
        };

        private string _tablePrefix = DefaultTablePrefix;

        /// <summary>
        /// The prefix for the batch persistence tables in the database. Default value is "BATCH_".
        /// </summary>
        public string TablePrefix
        {
            get { return _tablePrefix; }
            set { _tablePrefix = value; }
        }

        /// <summary>
        /// The database operator used for persistence.
        /// </summary>
        public DbOperator DbOperator { get; set; }

        /// <summary>
        /// The parameter helper used to create parameter placeholders in queries.
        /// </summary>
        public IPlaceholderGetter PlaceholderGetter { get; set; }

        /// <summary>
        /// Inserts the table prefix in the specified query. It uses the <see cref="string.Format(string,object)"/> syntax,
        /// and expect the prefix to be the parameter 0 (<code>{0}</code>).
        /// </summary>
        /// <param name="query">the query in which to insert the table prefix</param>
        /// <returns></returns>
        protected string InsertTablePrefix(string query)
        {
            return string.Format(query, _tablePrefix);
        }

        /// <summary>
        /// Checks that <see cref="DbOperator"/> has been correctly set.
        /// </summary>
        public virtual void AfterPropertiesSet()
        {
            Assert.NotNull(DbOperator);
        }
    }
}
