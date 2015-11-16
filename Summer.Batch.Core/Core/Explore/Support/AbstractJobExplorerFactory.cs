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

namespace Summer.Batch.Core.Explore.Support
{
    /// <summary>
    /// An <see cref="Summer.Batch.Common.Util.IFactory{T}"/> that automates the creation of a
    /// <see cref="SimpleJobExplorer"/>. Declares abstract methods for providing DAO
    /// object implementations.
    /// </summary>
    public abstract class AbstractJobExplorerFactory : IFactory<IJobExplorer>
    {
        /// <summary>
        /// Creates the job instance DAO.
        /// </summary>
        /// <returns>An instance if <see cref="IJobInstanceDao"/>.</returns>
        protected abstract IJobInstanceDao CreateJobInstanceDao();

        /// <summary>
        /// Creates the job execution DAO.
        /// </summary>
        /// <returns>An instance of <see cref="IJobExecutionDao"/>.</returns>
        protected abstract IJobExecutionDao CreateJobExecutionDao();

        /// <summary>
        /// Creates the step execution DAO.
        /// </summary>
        /// <returns>An instance of <see cref="IStepExecutionDao"/>.</returns>
        protected abstract IStepExecutionDao CreateStepExecutionDao();

        /// <summary>
        /// Creates the execution context DAO.
        /// </summary>
        /// <returns>An instance of <see cref="IExecutionContextDao"/>.</returns>
        protected abstract IExecutionContextDao CreateExecutionContextDao();

        /// <summary>
        /// Provides the IJobExplorer
        /// </summary>
        /// <returns></returns>
        public abstract IJobExplorer GetObject();
    }
}