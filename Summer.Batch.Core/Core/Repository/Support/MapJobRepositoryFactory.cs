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

namespace Summer.Batch.Core.Repository.Support
{
    /// <summary>
    ///  A FactoryBean that automates the creation of a
    /// SimpleJobRepository using non-persistent in-memory DAO
    /// implementations. This repository is only really intended for use in testing
    /// and rapid prototyping. 
    /// Not suited for use in multi-threaded jobs with splits, although it should be safe to use in a
    /// multi-threaded step.
    /// 
    /// NO JOB RESTARTABILITY USING THIS FACTORY (since contexts won't be persisted at the end of run time)
    /// </summary>
    public class MapJobRepositoryFactory : AbstractJobRepositoryFactory
    {
        /// <summary>
        /// JobInstance dao property.
        /// </summary>
        public MapJobInstanceDao JobInstanceDao { get; private set; }

        /// <summary>
        /// JOBExecution dao property.
        /// </summary>
        public MapJobExecutionDao JobExecutionDao { get; private set; }

        /// <summary>
        /// StepExecution dao property.
        /// </summary>
        public MapStepExecutionDao StepExecutionDao { get; private set; }

        /// <summary>
        /// ExecutionContexte dao property.
        /// </summary>
        public MapExecutionContextDao ExecutionContextDao { get; private set; }

        /// <summary>
        /// Clears all dao's.
        /// </summary>
        public void Clear()
        {
            JobInstanceDao.Clear();
            JobExecutionDao.Clear();
            StepExecutionDao.Clear();
            ExecutionContextDao.Clear();
        }

        /// <summary>
        /// Creates an IJobInstanceDao.
        /// </summary>
        /// <returns>IJobInstanceDao</returns>
        protected override IJobInstanceDao CreateJobInstanceDao()
        {
            JobInstanceDao = new MapJobInstanceDao();
            return JobInstanceDao;
        }

        /// <summary>
        /// Creates an IJobExecutionDao.
        /// </summary>
        /// <returns>IJobExecutionDao</returns>
        protected override IJobExecutionDao CreateJobExecutionDao()
        {
            JobExecutionDao = new MapJobExecutionDao();
            return JobExecutionDao;
        }

        /// <summary>
        /// Creates an IStepExecutionDao.
        /// </summary>
        /// <returns>IStepExecutionDao</returns>
        protected override IStepExecutionDao CreateStepExecutionDao()
        {
            StepExecutionDao = new MapStepExecutionDao();
            return StepExecutionDao;
        }

        /// <summary>
        /// Creates an IExecutionContextDao.
        /// </summary>
        /// <returns>IExecutionContextDao</returns>
        protected override IExecutionContextDao CreateExecutionContextDao()
        {
            ExecutionContextDao = new MapExecutionContextDao();
            return ExecutionContextDao;
        }
    }
}
