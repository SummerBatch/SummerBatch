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
using Summer.Batch.Common.Util;

namespace Summer.Batch.Core.Repository.Support
{
    /// <summary>
    /// A Factory that automates the creation of a
    /// <see cref="SimpleJobRepository"/>. Declares abstract methods for providing DAO
    /// object implementations.
    /// </summary>
    public abstract class AbstractJobRepositoryFactory : IFactory<IJobRepository>
    {
        /// <summary>
        /// Provides a SimpleJobRepository, with DAO implementations.
        /// </summary>
        /// <returns></returns>
        public IJobRepository GetObject()
        {
            return new SimpleJobRepository(CreateJobInstanceDao(), CreateJobExecutionDao(), CreateStepExecutionDao(), CreateExecutionContextDao());
        }

        /// <summary>
        /// Creates an IJobInstanceDao.
        /// </summary>
        /// <returns>IJobInstanceDao</returns>
        protected abstract IJobInstanceDao CreateJobInstanceDao();
        
        /// <summary>
        /// Creates an IJobExecutionDao
        /// </summary>
        /// <returns>IJobExecutionDao</returns>
        protected abstract IJobExecutionDao CreateJobExecutionDao();

        /// <summary>
        /// Creates an IStepExecutionDao.
        /// </summary>
        /// <returns>IStepExecutionDao</returns>
        protected abstract IStepExecutionDao CreateStepExecutionDao();

        /// <summary>
        /// Creates an IExecutionContextDao
        /// </summary>
        /// <returns>IExecutionContextDao</returns>
        protected abstract IExecutionContextDao CreateExecutionContextDao();

    }
}
