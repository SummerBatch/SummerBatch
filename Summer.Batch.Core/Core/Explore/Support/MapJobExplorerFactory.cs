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

using Summer.Batch.Core.Repository.Dao;
using Summer.Batch.Core.Repository.Support;
using Summer.Batch.Common.Factory;
using Summer.Batch.Common.Util;

namespace Summer.Batch.Core.Explore.Support
{
    /// <summary>
    ///  An <see cref="Summer.Batch.Common.Util.IFactory{T}"/> that automates the creation of a
    /// <see cref="SimpleJobExplorer"/> using in-memory DAO implementations.
    /// </summary>
    public class MapJobExplorerFactory : AbstractJobExplorerFactory, IInitializationPostOperations
    {
        /// <summary>
        /// Map job repository factory property.
        /// </summary>
        public MapJobRepositoryFactory RepositoryFactory { private get; set; }

        /// <summary>
        /// Custom constructor with a repository factory.
        /// </summary>
        /// <param name="repositoryFactory"></param>
        public MapJobExplorerFactory(MapJobRepositoryFactory repositoryFactory)
        {
            RepositoryFactory = repositoryFactory;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MapJobExplorerFactory()
        {
            
        }

        /// <summary>
        /// Retrieves the job instance DAO.
        /// </summary>
        /// <returns>The <see cref="IJobInstanceDao"/> of <see cref="RepositoryFactory"/>.</returns>
        protected override IJobInstanceDao CreateJobInstanceDao()
        {
            return RepositoryFactory.JobInstanceDao;
        }

        /// <summary>
        /// Retrieves the job execution DAO.
        /// </summary>
        /// <returns>The <see cref="IJobExecutionDao"/> of <see cref="RepositoryFactory"/>.</returns>
        protected override IJobExecutionDao CreateJobExecutionDao()
        {
            return RepositoryFactory.JobExecutionDao;
        }

        /// <summary>
        /// Retrieves the step execution DAO.
        /// </summary>
        /// <returns>The <see cref="IStepExecutionDao"/> of <see cref="RepositoryFactory"/>.</returns>
        protected override IStepExecutionDao CreateStepExecutionDao()
        {
            return RepositoryFactory.StepExecutionDao;
        }

        /// <summary>
        /// Retrieves the execution context DAO.
        /// </summary>
        /// <returns>The <see cref="IExecutionContextDao"/> of <see cref="RepositoryFactory"/>.</returns>
        protected override IExecutionContextDao CreateExecutionContextDao()
        {
            return RepositoryFactory.ExecutionContextDao;
        }

        /// <summary>
        /// Provides the Job Explorer.
        /// </summary>
        /// <returns></returns>
        public override IJobExplorer GetObject()
        {
            return new SimpleJobExplorer(CreateJobInstanceDao(), CreateJobExecutionDao(), CreateStepExecutionDao(), CreateExecutionContextDao());
        }

        /// <summary>
        /// Post initialization operation.
        /// </summary>
        public void AfterPropertiesSet()
        {
            Assert.NotNull(RepositoryFactory, "A MapJobRepositoryFactoryBean must be provided");
        }
    }
}