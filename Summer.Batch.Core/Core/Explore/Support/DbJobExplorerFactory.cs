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

using System;
using System.Configuration;
using Summer.Batch.Common.Factory;
using Summer.Batch.Common.Util;
using Summer.Batch.Core.Repository.Dao;
using Summer.Batch.Data;
using Summer.Batch.Data.Incrementer;

namespace Summer.Batch.Core.Explore.Support
{
    /// <summary>
    /// An <see cref="Summer.Batch.Common.Util.IFactory{T}"/> that automates the creation of a
    /// <see cref="SimpleJobExplorer"/> using Database DAO implementations. Requires the user
    /// to describe what kind of database they are using.
    /// </summary>
    public class DbJobExplorerFactory : AbstractJobExplorerFactory, IInitializationPostOperations
    {
        /// <summary>
        /// Connection string settings property.
        /// </summary>
        public ConnectionStringSettings ConnectionStringSettings { private get; set; }

        /// <summary>
        /// DbOperator property.
        /// </summary>
        public DbOperator DbOperator { private get; set; }

        private string _tablePrefix = AbstractDbBatchMetadataDao.DefaultTablePrefix;

        /// <summary>
        /// TablePrefix property.
        /// </summary>
        public string TablePrefix
        {
            private get { return _tablePrefix; }
            set { _tablePrefix = value; }
        }

        private readonly IDataFieldMaxValueIncrementer _incrementer = new LocalDataFieldMaxValueIncrementer();

        /// <summary>
        /// Creates the job instance DAO.
        /// </summary>
        /// <returns>An instance if <see cref="DbJobInstanceDao"/>.</returns>
        protected override IJobInstanceDao CreateJobInstanceDao()
        {
            var dao = new DbJobInstanceDao
            {
                DbOperator = DbOperator,
                JobIncrementer = _incrementer,
                TablePrefix = TablePrefix,
            };
            dao.AfterPropertiesSet();
            return dao;
        }

        /// <summary>
        /// Creates the job execution DAO.
        /// </summary>
        /// <returns>An instance of <see cref="DbJobExecutionDao"/>.</returns>
        protected override IJobExecutionDao CreateJobExecutionDao()
        {
            var dao = new DbJobExecutionDao
            {
                DbOperator = DbOperator,
                JobIncrementer = _incrementer,
                TablePrefix = TablePrefix,
            };
            dao.AfterPropertiesSet();
            return dao;
        }

        /// <summary>
        /// Creates the step execution DAO.
        /// </summary>
        /// <returns>An instance of <see cref="DbStepExecutionDao"/>.</returns>
        protected override IStepExecutionDao CreateStepExecutionDao()
        {
            var dao = new DbStepExecutionDao
            {
                DbOperator = DbOperator,
                StepIncrementer = _incrementer,
                TablePrefix = TablePrefix,
            };
            dao.AfterPropertiesSet();
            return dao;
        }

        /// <summary>
        /// Creates the execution context DAO.
        /// </summary>
        /// <returns>An instance of <see cref="DbExecutionContextDao"/>.</returns>
        protected override IExecutionContextDao CreateExecutionContextDao()
        {
            var dao = new DbExecutionContextDao
            {
                DbOperator = DbOperator,
                TablePrefix = TablePrefix,
            };
            dao.AfterPropertiesSet();
            return dao;
        }

        /// <summary>
        /// Provides the IJobExplorer.
        /// </summary>
        /// <returns></returns>
        public override IJobExplorer GetObject()
        {
            return new SimpleJobExplorer(CreateJobInstanceDao(), CreateJobExecutionDao(), CreateStepExecutionDao(), CreateExecutionContextDao());
        }

        /// <summary>
        /// Post-initialization operation.
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
    }

    /// <summary>
    /// Local <see cref="IDataFieldMaxValueIncrementer"/> implementation.
    /// Since it is not supported here, a <see cref="NotSupportedException"/> will be thrown
    /// by the <see cref="NextLong"/> method.
    /// </summary>
    class LocalDataFieldMaxValueIncrementer : AbstractDataFieldMaxValueIncrementer
    {
        /// <summary>
        /// Returns the next value for the current incrementer.
        /// Not supported here, wo will throw a NotSupportedException if invoked.
        /// </summary>
        /// <returns></returns>
        public override long NextLong()
        {
            throw new NotSupportedException("JobExplorer is read only.");
        }
    }
}