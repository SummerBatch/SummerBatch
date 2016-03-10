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
using System.Collections.Generic;

namespace Summer.Batch.Core.Explore.Support
{
    /// <summary>
    /// Implementation of <see cref="IJobExplorer"/> using the injected DAOs.
    /// </summary>
    public class SimpleJobExplorer : IJobExplorer
    {
        #region Attributes
        private readonly IJobInstanceDao _jobInstanceDao;
        private readonly IJobExecutionDao _jobExecutionDao;
        private readonly IStepExecutionDao _stepExecutionDao;
        private readonly IExecutionContextDao _executionContextDao;
        #endregion

        /// <summary>
        /// Constructs a new <see cref="SimpleJobExplorer"/> with the specified DAOs.
        /// </summary>
        /// <param name="jobInstanceDao">The job instance DAO.</param>
        /// <param name="jobExecutionDao">The job execution DAO.</param>
        /// <param name="stepExecutionDao">The step execution DAO.</param>
        /// <param name="executionContextDao">The execution context DAO.</param>
        public SimpleJobExplorer(IJobInstanceDao jobInstanceDao, IJobExecutionDao jobExecutionDao,
            IStepExecutionDao stepExecutionDao, IExecutionContextDao executionContextDao)
        {
            _jobInstanceDao = jobInstanceDao;
            _jobExecutionDao = jobExecutionDao;
            _stepExecutionDao = stepExecutionDao;
            _executionContextDao = executionContextDao;
        }

        #region IJobExplorer methods implementation

        /// <summary>
        /// Retrieves job executions by their job instance. The corresponding step
        /// executions may not be fully hydrated (e.g. their execution context may be
        /// missing), depending on the implementation. Use
        /// <see cref="GetStepExecution"/> to hydrate them in that case.
        /// </summary>
        /// <param name="jobInstance">the JobInstance to query</param>
        /// <returns>the set of all executions for the specified JobInstance</returns>
        public IList<JobExecution> GetJobExecutions(JobInstance jobInstance)
        {
            IList<JobExecution> executions = _jobExecutionDao.FindJobExecutions(jobInstance);
            foreach (JobExecution jobExecution in executions)
            {
                GetJobExecutionDependencies(jobExecution);
                foreach (StepExecution stepExecution in jobExecution.StepExecutions)
                {
                    GetStepExecutionDependencies(stepExecution);
                }
            }
            return executions;
        }

        /// <summary>
        /// Retrieves running job executions. The corresponding step executions may
        /// not be fully hydrated (e.g. their execution context may be missing),
        /// depending on the implementation. Use
        /// GetStepExecution(Long, Long) to hydrate them in that case.
        /// </summary>
        /// <param name="jobName"> the name of the job</param>
        /// <returns>the set of running executions for jobs with the specified name</returns>
        public ISet<JobExecution> FindRunningJobExecutions(string jobName)
        {
            ISet<JobExecution> executions = _jobExecutionDao.FindRunningJobExecutions(jobName);
            foreach (JobExecution jobExecution in executions)
            {
                GetJobExecutionDependencies(jobExecution);
                foreach (StepExecution stepExecution in jobExecution.StepExecutions)
                {
                    GetStepExecutionDependencies(stepExecution);
                }
            }
            return executions;
        }

        /// <summary>
        /// Retrieves a JobExecution by its id. The complete object graph for
        /// this execution should be returned (unless otherwise indicated) including
        /// the parent JobInstance and associated ExecutionContext
        /// and StepExecution instances (also including their execution
        /// contexts).
        /// </summary>
        /// <param name="executionId">the job execution id</param>
        /// <returns>the JobExecution with this id, or <c>null</c> if not found</returns>
        public JobExecution GetJobExecution(long executionId)
        {
            JobExecution jobExecution = _jobExecutionDao.GetJobExecution(executionId);
            if (jobExecution == null)
            {
                return null;
            }
            GetJobExecutionDependencies(jobExecution);
            foreach (StepExecution stepExecution in jobExecution.StepExecutions)
            {
                GetStepExecutionDependencies(stepExecution);
            }
            return jobExecution;
        }

        /// <summary>
        /// Retrieves a StepExecution by its id and parent
        /// JobExecution id. The execution context for the step should be
        /// available in the result, and the parent job execution should have its
        /// primitive properties, but may not contain the job instance information.
        /// </summary>
        /// <param name="jobExecutionId">the parent job execution id</param>
        /// <param name="executionId">the step execution id</param>
        /// <returns>the StepExecution with this id, or <c>null</c> if not found</returns>
        public StepExecution GetStepExecution(long jobExecutionId, long executionId)
        {
            JobExecution jobExecution = _jobExecutionDao.GetJobExecution(jobExecutionId);
            if (jobExecution == null)
            {
                return null;
            }
            GetJobExecutionDependencies(jobExecution);
            StepExecution stepExecution = _stepExecutionDao.GetStepExecution(jobExecution, executionId);
            GetStepExecutionDependencies(stepExecution);
            return stepExecution;
        }

        /// <summary>
        /// Gets the JobInstance for the given instance id.
        /// </summary>
        /// <param name="instanceId">the instance id</param>
        /// <returns>the JobInstance with this id, or <c>null</c></returns>
        public JobInstance GetJobInstance(long instanceId)
        {
            return _jobInstanceDao.GetJobInstance(instanceId);
        }

        /// <summary>
        /// Fetches JobInstance values in descending order of creation (and therefore usually of first execution).
        /// </summary>
        /// <param name="jobName">the name of the job to query</param>
        /// <param name="start">the start index of the instances to return</param>
        /// <param name="count">the maximum number of instances to return</param>
        /// <returns>the JobInstance values up to a maximum of count values</returns>
        public IList<JobInstance> GetJobInstances(string jobName, int start, int count)
        {
            return _jobInstanceDao.GetJobInstances(jobName, start, count);
        }

        /// <summary>
        /// Queries the repository for all unique JobInstance names (sorted alphabetically).
        /// </summary>
        /// <returns>the set of job names that have been executed</returns>
        public IList<string> GetJobNames()
        {
            return _jobInstanceDao.GetJobNames();
        }

        /// <summary>
        /// Queries the repository for the number of unique JobInstances
        /// associated with the supplied job name.
        /// </summary>
        /// <param name="jobName">the name of the job to query for</param>
        /// <returns>the number of JobInstances that exist within the associated job repository</returns>
        /// <exception cref="T:Summer.Batch.Core.Launch.NoSuchJobException">&nbsp;if no jobinstance could be found for the given name</exception>
        public int GetJobInstanceCount(string jobName)
        {
            return _jobInstanceDao.GetJobInstanceCount(jobName);
        }

        #endregion

        #region private utility methods
        /// <summary>
        /// Finds all dependencies for a JobExecution, including JobInstance (which
        /// requires JobParameters) plus StepExecutions.
        /// </summary>
        /// <param name="jobExecution">the given job execution</param>
        private void GetJobExecutionDependencies(JobExecution jobExecution)
        {
            JobInstance jobInstance = _jobInstanceDao.GetJobInstance(jobExecution);
            _stepExecutionDao.AddStepExecutions(jobExecution);
            jobExecution.JobInstance = jobInstance;
            jobExecution.ExecutionContext = _executionContextDao.GetExecutionContext(jobExecution);
        }

        /// <summary>
        /// Gets execution dependencies for a given StepExecution.
        /// </summary>
        /// <param name="stepExecution">the given step execution</param>
        private void GetStepExecutionDependencies(StepExecution stepExecution)
        {
            if (stepExecution != null)
            {
                stepExecution.ExecutionContext = _executionContextDao.GetExecutionContext(stepExecution);
            }
        }
        #endregion
    }
}
