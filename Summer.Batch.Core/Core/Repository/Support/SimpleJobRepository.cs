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

using NLog;
using Summer.Batch.Core.Repository.Dao;
using Summer.Batch.Infrastructure.Item;
using Summer.Batch.Common.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Summer.Batch.Core.Repository.Support
{
    /// <summary>
    /// Implementation of <see cref="IJobRepository"/> that stores <see cref="JobInstance"/>s,
    /// <see cref="JobExecution"/>s, and <see cref="StepExecution"/>s using the injected DAOs.
    /// </summary>
    public class SimpleJobRepository : IJobRepository
    {

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IJobInstanceDao _jobInstanceDao;
        private readonly IJobExecutionDao _jobExecutionDao;
        private readonly IStepExecutionDao _stepExecutionDao;
        private readonly IExecutionContextDao _executionContextDao;

        /// <summary>
        /// Code reused.
        /// </summary>
        /// <param name="stepExecution"></param>
        private static void ValidateStepExecution(StepExecution stepExecution)
        {
            Assert.NotNull(stepExecution, "StepExecution cannot be null.");
            Assert.NotNull(stepExecution.StepName, "StepExecution's step name cannot be null.");
            Assert.NotNull(stepExecution.GetJobExecutionId(), "StepExecution must belong to persisted JobExecution");
        }

        /// <summary>
        /// Check to determine whether or not the JobExecution that is the parent of
        /// the provided StepExecution has been interrupted. If, after synchronizing
        /// the status with the database, the status has been updated to STOPPING,
        /// then the job has been interrupted.
        /// </summary>
        /// <param name="stepExecution"></param>
        private void CheckForInterruption(StepExecution stepExecution)
        {
            JobExecution jobExecution = stepExecution.JobExecution;
            _jobExecutionDao.SynchronizeStatus(jobExecution);
            if (jobExecution.IsStopping())
            {
                _logger.Info("Parent JobExecution is stopped, so passing message on to StepExecution");
                stepExecution.SetTerminateOnly();
            }
        }

        /// <summary>
        /// Custom constructor, providing all dao's as parameters.
        /// </summary>
        /// <param name="jobInstanceDao"></param>
        /// <param name="jobExecutionDao"></param>
        /// <param name="stepExecutionDao"></param>
        /// <param name="executionContextDao"></param>
        public SimpleJobRepository(IJobInstanceDao jobInstanceDao, IJobExecutionDao jobExecutionDao, IStepExecutionDao stepExecutionDao, IExecutionContextDao executionContextDao)
        {
            _jobInstanceDao = jobInstanceDao;
            _jobExecutionDao = jobExecutionDao;
            _stepExecutionDao = stepExecutionDao;
            _executionContextDao = executionContextDao;
        }

        /// <summary>
        /// IsJobInstanceExists.
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="jobParameters"></param>
        /// <returns></returns>
        public bool IsJobInstanceExists(string jobName, JobParameters jobParameters)
        {
            return _jobInstanceDao.GetJobInstance(jobName, jobParameters) != null;
        }

        /// <summary>
        /// Creates JobInstance.
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="jobParameters"></param>
        /// <returns></returns>
        public JobInstance CreateJobInstance(string jobName, JobParameters jobParameters)
        {
            Assert.NotNull(jobName, "A job name is required to create a JobInstance");
            Assert.NotNull(jobParameters, "Job parameters are required to create a JobInstance");
            var jobInstance = _jobInstanceDao.CreateJobInstance(jobName, jobParameters);
            return jobInstance;
        }

        /// <summary>
        /// Creates JobExecution.
        /// </summary>
        /// <param name="jobInstance"></param>
        /// <param name="jobParameters"></param>
        /// <param name="jobConfigurationLocation"></param>
        /// <returns></returns>
        public JobExecution CreateJobExecution(JobInstance jobInstance, JobParameters jobParameters, string jobConfigurationLocation)
        {
            Assert.NotNull(jobInstance, "A JobInstance is required to associate the JobExecution with");
            Assert.NotNull(jobParameters, "A JobParameters object is required to create a JobExecution");
            ExecutionContext executionContext = new ExecutionContext();

            var jobExecution = new JobExecution(jobInstance, jobParameters, jobConfigurationLocation)
            {
                ExecutionContext = executionContext,
                LastUpdated = DateTime.Now
            };

            // Save the JobExecution so that it picks up an ID (useful for clients
            // monitoring asynchronous executions):
            _jobExecutionDao.SaveJobExecution(jobExecution);
            _executionContextDao.SaveExecutionContext(jobExecution);

            return jobExecution;
        }

        /// <summary>
        /// Creates JobExecution.
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="jobParameters"></param>
        /// <returns></returns>
        public JobExecution CreateJobExecution(string jobName, JobParameters jobParameters)
        {
            Assert.NotNull(jobName, "Job name must not be null.");
            Assert.NotNull(jobParameters, "JobParameters must not be null.");
            var jobInstance = _jobInstanceDao.GetJobInstance(jobName, jobParameters);
            ExecutionContext executionContext;

            // existing job instance found
            if (jobInstance != null)
            {

                var executions = _jobExecutionDao.FindJobExecutions(jobInstance);

                // check for running executions and find the last started
                foreach (var execution in executions)
                {
                    if (execution.IsRunning())
                    {
                        throw new JobExecutionAlreadyRunningException(string.Format("A job execution for this job is already running: {0}", 
                            jobInstance));
                    }

                    var status = execution.Status;
                    if (execution.JobParameters.GetParameters().Any()
                        && (status == BatchStatus.Completed || status == BatchStatus.Abandoned))
                    {
                        throw new JobInstanceAlreadyCompleteException(string.Format(
                                "A job instance already exists and is complete for parameters={0}"
                                + ".  If you want to run this job again, change the parameters.", jobParameters));
                    }
                }
                executionContext = _executionContextDao.GetExecutionContext(_jobExecutionDao.GetLastJobExecution(jobInstance));
            }
            else
            {
                // no job found, create one
                jobInstance = _jobInstanceDao.CreateJobInstance(jobName, jobParameters);
                executionContext = new ExecutionContext();
            }

            var jobExecution = new JobExecution(jobInstance, jobParameters, null)
            {
                ExecutionContext = executionContext,
                LastUpdated = DateTime.Now
            };

            // Save the JobExecution so that it picks up an ID (useful for clients
            // monitoring asynchronous executions):
            _jobExecutionDao.SaveJobExecution(jobExecution);
            _executionContextDao.SaveExecutionContext(jobExecution);

            return jobExecution;
        }

        /// <summary>
        /// Updates JobExecution.
        /// </summary>
        /// <param name="jobExecution"></param>
        public void Update(JobExecution jobExecution)
        {
            Assert.NotNull(jobExecution, "JobExecution cannot be null.");
            Assert.NotNull(jobExecution.GetJobId(), "JobExecution must have a Job ID set.");
            Assert.NotNull(jobExecution.Id, "JobExecution must be already saved (have an id assigned).");
            jobExecution.LastUpdated = DateTime.Now;
            _jobExecutionDao.SynchronizeStatus(jobExecution);
            _jobExecutionDao.UpdateJobExecution(jobExecution);
        }

        /// <summary>
        /// Adds StepExecution.
        /// </summary>
        /// <param name="stepExecution"></param>
        public void Add(StepExecution stepExecution)
        {
            ValidateStepExecution(stepExecution);
            stepExecution.LastUpdated = DateTime.Now;
            _stepExecutionDao.SaveStepExecution(stepExecution);
            _executionContextDao.SaveExecutionContext(stepExecution);
        }

        /// <summary>
        /// Adds all StepExecutions.
        /// </summary>
        /// <param name="stepExecutions"></param>
        public void AddAll(ICollection<StepExecution> stepExecutions)
        {
            Assert.NotNull(stepExecutions, "Attempt to save a null collection of step executions");
            foreach (var stepExecution in stepExecutions)
            {
                ValidateStepExecution(stepExecution);
                stepExecution.LastUpdated = DateTime.Now;
            }
            _stepExecutionDao.SaveStepExecutions(stepExecutions);
            _executionContextDao.SaveExecutionContexts(stepExecutions);
        }

        /// <summary>
        /// Uodate StepExecution.
        /// </summary>
        /// <param name="stepExecution"></param>
        public void Update(StepExecution stepExecution)
        {
            ValidateStepExecution(stepExecution);
            Assert.NotNull(stepExecution.Id, "StepExecution must already be saved (have an id assigned)");
            stepExecution.LastUpdated = DateTime.Now;
            _stepExecutionDao.UpdateStepExecution(stepExecution);
            CheckForInterruption(stepExecution);
        }

        /// <summary>
        /// Updates ExecutionContext with StepExecution.
        /// </summary>
        /// <param name="stepExecution"></param>
        public void UpdateExecutionContext(StepExecution stepExecution)
        {
            ValidateStepExecution(stepExecution);
            Assert.NotNull(stepExecution.Id, "StepExecution must already be saved (have an id assigned)");
            _executionContextDao.UpdateExecutionContext(stepExecution);
        }

        /// <summary>
        /// Updates ExecutionContext with JobExecution.
        /// </summary>
        /// <param name="jobExecution"></param>
        public void UpdateExecutionContext(JobExecution jobExecution)
        {
            _executionContextDao.UpdateExecutionContext(jobExecution);
        }

        /// <summary>
        /// Returns last StepExecution.
        /// </summary>
        /// <param name="jobInstance"></param>
        /// <param name="stepName"></param>
        /// <returns></returns>
        public StepExecution GetLastStepExecution(JobInstance jobInstance, string stepName)
        {
            var jobExecutions = _jobExecutionDao.FindJobExecutions(jobInstance);
            var stepExecutions = new List<StepExecution>(jobExecutions.Count);

            foreach (var jobExecution in jobExecutions)
            {
                _stepExecutionDao.AddStepExecutions(jobExecution);
                stepExecutions.AddRange(jobExecution.StepExecutions.Where(stepExecution => stepName.Equals(stepExecution.StepName)));
            }

            StepExecution latest = null;
            foreach (var stepExecution in stepExecutions)
            {
                if (latest == null)
                {
                    latest = stepExecution;
                }
                if (latest.StartTime.Ticks < stepExecution.StartTime.Ticks)
                {
                    latest = stepExecution;
                }
            }

            if (latest == null)
            {
                return null;
            }
            var stepExecutionContext = _executionContextDao.GetExecutionContext(latest);
            latest.ExecutionContext = stepExecutionContext;
            var jobExecutionContext = _executionContextDao.GetExecutionContext(latest.JobExecution);
            latest.JobExecution.ExecutionContext = jobExecutionContext;

            return latest;
        }

        /// <summary>
        /// Returns StepExecution count.
        /// </summary>
        /// <param name="jobInstance"></param>
        /// <param name="stepName"></param>
        /// <returns></returns>
        public int GetStepExecutionCount(JobInstance jobInstance, string stepName)
        {
            var count = 0;
            var jobExecutions = _jobExecutionDao.FindJobExecutions(jobInstance);
            foreach (var jobExecution in jobExecutions)
            {
                _stepExecutionDao.AddStepExecutions(jobExecution);
                count += jobExecution.StepExecutions.Count(stepExecution => stepName.Equals(stepExecution.StepName));
            }
            return count;
        }

        /// <summary>
        /// Returns the last JobExecution.
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="jobParameters"></param>
        /// <returns></returns>
        public JobExecution GetLastJobExecution(string jobName, JobParameters jobParameters)
        {
            var jobInstance = _jobInstanceDao.GetJobInstance(jobName, jobParameters);
            if (jobInstance == null)
            {
                return null;
            }
            var jobExecution = _jobExecutionDao.GetLastJobExecution(jobInstance);
            if (jobExecution == null)
            {
                return null;
            }
            jobExecution.ExecutionContext = _executionContextDao.GetExecutionContext(jobExecution);
            _stepExecutionDao.AddStepExecutions(jobExecution);
            return jobExecution;
        }
    }
}
