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

using System.Configuration;
using Summer.Batch.Core.Repository.Dao;
using Summer.Batch.Data;
using Summer.Batch.Common.Factory;
using Summer.Batch.Common.Util;
using Summer.Batch.Data.Incrementer;

namespace Summer.Batch.Core.Repository.Support
{
    /// <summary>
    ///  A Factory that automates the creation of a
    /// <see cref="SimpleJobRepository"/> using ODBC DAO implementations which persist
    /// batch metadata in database. Requires the user to describe what kind of
    /// database they are using.
    /// </summary>
    public class DbJobRepositoryFactory : AbstractJobRepositoryFactory, IInitializationPostOperations
    {
        /// <summary>
        /// Connection strings property.
        /// </summary>
        public ConnectionStringSettings ConnectionStringSettings { private get; set; }

        /// <summary>
        /// DbOperator property (in charge of db related operations)
        /// </summary>
        public DbOperator DbOperator { private get; set; }

        private string _tablePrefix = AbstractDbBatchMetadataDao.DefaultTablePrefix;

        /// <summary>
        /// Table prefix property.
        /// </summary>
        public string TablePrefix
        {
            private get { return _tablePrefix; }
            set { _tablePrefix = value; }
        }

        private int _maxVarCharLength = AbstractDbBatchMetadataDao.DefaultExitMessageLength;
        
        /// <summary>
        /// Maximum usable db varchar length.
        /// </summary>
        public int MaxVarCharLength
        {
            private get { return _maxVarCharLength; }
            set { _maxVarCharLength = value; }
        }

        /// <summary>
        /// Post initialization operation.
        /// </summary>
        public void AfterPropertiesSet()
        {
            Assert.NotNull(ConnectionStringSettings, "Connection String Settings must be supplied");
          
            if (DbOperator == null)
            {
                DbOperator = new DbOperator
                {
                    ConnectionProvider = new ConnectionProvider { ConnectionStringSettings = ConnectionStringSettings }
                };
            }
        }

        /// <summary>
        /// Creates an IJobInstanceDao.
        /// </summary>
        /// <returns>IJobInstanceDao</returns>
        protected override IJobInstanceDao CreateJobInstanceDao()
        {
            DbJobInstanceDao dao = new DbJobInstanceDao
            {
                DbOperator = DbOperator,
                JobIncrementer = GetIncrementer(TablePrefix + "JOB_SEQ"),
                TablePrefix = TablePrefix,
            };
            dao.AfterPropertiesSet();
            return dao;
        }

        /// <summary>
        /// Creates an IJobExecutionDao. 
        /// </summary>
        /// <returns>IJobExecutionDao</returns>
        protected override IJobExecutionDao CreateJobExecutionDao()
        {
            DbJobExecutionDao dao = new DbJobExecutionDao
            {
                DbOperator = DbOperator,
                JobIncrementer = GetIncrementer(TablePrefix + "JOB_EXECUTION_SEQ"),
                TablePrefix = TablePrefix,
                ExitMessageLength = MaxVarCharLength
            };
            dao.AfterPropertiesSet();
            return dao;
        }

        /// <summary>
        /// Creates an IStepExecutionDao.
        /// </summary>
        /// <returns>IStepExecutionDao</returns>
        protected override IStepExecutionDao CreateStepExecutionDao()
        {
            DbStepExecutionDao dao = new DbStepExecutionDao
            {
                DbOperator = DbOperator,
                StepIncrementer = GetIncrementer(TablePrefix + "STEP_EXECUTION_SEQ"),
                TablePrefix = TablePrefix,
                ExitMessageLength = MaxVarCharLength
            };
            dao.AfterPropertiesSet();
            return dao;
        }

        /// <summary>
        /// Creates an IExecutionContextDao.
        /// </summary>
        /// <returns>IExecutionContextDao</returns>
        protected override IExecutionContextDao CreateExecutionContextDao()
        {
            DbExecutionContextDao dao = new DbExecutionContextDao
            {
                DbOperator = DbOperator,
                TablePrefix = TablePrefix
            };
            dao.AfterPropertiesSet();
            return dao;
        }

        private IDataFieldMaxValueIncrementer GetIncrementer(string incrementerName)
        {
            var incrementer = DatabaseExtensionManager.GetIncrementer(ConnectionStringSettings.ProviderName);
            incrementer.ConnectionStringSettings = ConnectionStringSettings;
            incrementer.IncrementerName = incrementerName;
            return incrementer;
        }
    }
}