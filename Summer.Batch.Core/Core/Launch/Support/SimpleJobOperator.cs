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

#region usings
using NLog;
using Summer.Batch.Common.Collections;
using Summer.Batch.Core.Configuration;
using Summer.Batch.Core.Converter;
using Summer.Batch.Core.Explore;
using Summer.Batch.Core.Repository;
using Summer.Batch.Core.Scope.Context;
using Summer.Batch.Core.Step;
using Summer.Batch.Core.Step.Tasklet;
using Summer.Batch.Infrastructure.Support;
using Summer.Batch.Common.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using Summer.Batch.Common.Factory;

#endregion

namespace Summer.Batch.Core.Launch.Support
{

    /// <summary>
    /// Simple implementation of the <see cref="IJobOperator"/> interface.  Due to the amount of
    /// functionality the implementation is combining, the following dependencies
    /// are required:
    /// <ul>
    ///    <li> <see cref="IJobLauncher"/> </li>
    ///    <li> <see cref="IJobExplorer"/> </li>
    ///    <li> <see cref="IJobRepository"/> </li>
    ///    <li> <see cref="IJobRegistry"/> </li>
    /// </ul>
    /// </summary>
    public class SimpleJobOperator : IJobOperator, IInitializationPostOperations
    {
        private const string IllegalStateMsg = "Illegal state (only happens on a race condition): {0} with name={1} and parameters={2}";

        #region Attributes
        /// <summary>
        /// Job registry property.
        /// </summary>
        public IListableJobLocator JobRegistry { get; set; }

        /// <summary>
        /// Job explorer property.
        /// </summary>
        public IJobExplorer JobExplorer { get; set; }

        /// <summary>
        /// Job launcher property.
        /// </summary>
        public IJobLauncher JobLauncher { get; set; }

        /// <summary>
        /// Job repository property.
        /// </summary>
        public IJobRepository JobRepository { get; set; }

        private IJobParametersConverter _jobParametersConverter = new DefaultJobParametersConverter();

        /// <summary>
        /// Job parameters converter property.
        /// </summary>
        public IJobParametersConverter JobParametersConverter { set { _jobParametersConverter = value; } }

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        #endregion

        /// <summary>
        /// post-init checks.
        /// IInitializationPostOperations#AfterPropertiesSet
        /// </summary>
        /// <exception cref="Exception">&nbsp;</exception>
        public void AfterPropertiesSet()
        {
            Assert.NotNull(JobLauncher, "JobLauncher must be provided");
            Assert.NotNull(JobRegistry, "JobLocator must be provided");
            Assert.NotNull(JobExplorer, "JobExplorer must be provided");
            Assert.NotNull(JobRepository, "JobRepository must be provided");
        }

        #region IJobOperator methods implementation
        /// <summary>
        /// @see IJobOperator#GetExecutions .
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        /// <exception cref="NoSuchJobInstanceException">&nbsp;</exception>
        public IList<long?> GetExecutions(long instanceId)
        {
            JobInstance jobInstance = JobExplorer.GetJobInstance(instanceId);
            if (jobInstance == null)
            {
                throw new NoSuchJobInstanceException(string.Format("No job instance with id={0}", instanceId));
            }
            return JobExplorer.GetJobExecutions(jobInstance).Select(jobExecution => jobExecution.Id).ToList();
        }

        /// <summary>
        /// @see IJobOperator#GetJobNames .
        /// </summary>
        /// <returns></returns>
        public ISet<string> GetJobNames()
        {
            return new SortedSet<string>(JobRegistry.GetJobNames());
        }

        /// <summary>
        /// @see IJobOperator#GetJobInstances .
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        /// <exception cref="NoSuchJobExecutionException">&nbsp;</exception>
        public IList<long?> GetJobInstances(string jobName, int start, int count)
        {
            IList<JobInstance> jobInstances = JobExplorer.GetJobInstances(jobName, start, count);
            IList<long?> list = jobInstances.Select(jobInstance => jobInstance.Id).ToList();
            if (!list.Any() && !JobRegistry.GetJobNames().Contains(jobName))
            {
                throw new NoSuchJobException(string.Format("No such job (either in registry or in historical data): {0}" , jobName));
            }
            return list;
        }

        /// <summary>
        /// @see IJobOperator#GetParameters .
        /// </summary>
        /// <param name="executionId"></param>
        /// <returns></returns>
        /// <exception cref="NoSuchJobExecutionException">&nbsp;</exception>
        public string GetParameters(long executionId)
        {
            JobExecution jobExecution = FindExecutionById(executionId);
            return PropertiesConverter.PropertiesToString(_jobParametersConverter.GetProperties(jobExecution.JobParameters));
        }

        /// <summary>
        /// @see IJobOperator#GetRunningExecutions .
        /// </summary>
        /// <param name="jobName"></param>
        /// <returns></returns>
        /// <exception cref="NoSuchJobException">&nbsp;</exception>
        public ICollection<long?> GetRunningExecutions(string jobName)
        {
            // SLE: OrderedSet is not a ISet: must return a Collection (Interface changed for that purpose)
            ICollection<long?> set = new OrderedSet<long?>();
            foreach (JobExecution jobExecution in JobExplorer.FindRunningJobExecutions(jobName))
            {
                set.Add(jobExecution.Id);
            }
            if (!set.Any() && !JobRegistry.GetJobNames().Contains(jobName))
            {
                throw new NoSuchJobException(string.Format("No such job (either in registry or in historical data): {0}", jobName));
            }
            return set;
        }

        /// <summary>
        /// @see IJobOperator#GetStepExecutionSummaries .
        /// </summary>
        /// <param name="executionId"></param>
        /// <returns></returns>
        /// <exception cref="NoSuchJobExecutionException">&nbsp;</exception>
        public IDictionary<long?, string> GetStepExecutionSummaries(long executionId)
        {
            JobExecution jobExecution = FindExecutionById(executionId);

            ICollection<StepExecution> stepExecutions = jobExecution.StepExecutions;
            IDictionary<long?, string> map = new OrderedDictionary<long?, string>(stepExecutions.Count);
            foreach (StepExecution stepExecution in stepExecutions)
            {
                map.Add(stepExecution.Id, stepExecution.ToString());
            }
            return map;
        }

        /// <summary>
        /// @see IJobOperator#GetSummary .
        /// </summary>
        /// <param name="executionId"></param>
        /// <returns></returns>
        /// <exception cref="NoSuchJobExecutionException">&nbsp;</exception>
        public string GetSummary(long executionId)
        {
            JobExecution jobExecution = FindExecutionById(executionId);
            return jobExecution.ToString();
        }

        /// <summary>
        /// @see IJobOperator#Restart .
        /// </summary>
        /// <param name="executionId"></param>
        /// <returns></returns>
        /// <exception cref="JobInstanceAlreadyCompleteException">&nbsp;</exception>
        /// <exception cref="NoSuchJobExecutionException">&nbsp;</exception>
        /// <exception cref="NoSuchJobException">&nbsp;</exception>
        /// <exception cref="JobRestartException">&nbsp;</exception>
        /// <exception cref="JobParametersInvalidException">&nbsp;</exception>
        public long? Restart(long executionId)
        {
            _logger.Info("Checking status of job execution with id= {0}",executionId);

            JobExecution jobExecution = FindExecutionById(executionId);

            string jobName = jobExecution.JobInstance.JobName;
            IJob job = JobRegistry.GetJob(jobName);
            JobParameters parameters = jobExecution.JobParameters;

            _logger.Info("Attempting to resume job with name={0} and parameters={1}", jobName, parameters);
            try
            {
                return JobLauncher.Run(job, parameters).Id;
            }
            catch (JobExecutionAlreadyRunningException e)
            {
                throw new UnexpectedJobExecutionException(string.Format(IllegalStateMsg, "job execution already running", jobName, parameters), e);
            }
        }

        /// <summary>
        /// @see IJobOperator#Start .
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <exception cref="NoSuchJobException">&nbsp;</exception>
        /// <exception cref="JobInstanceAlreadyExistsException">&nbsp;</exception>
        /// <exception cref="JobParametersInvalidException">&nbsp;</exception>
        public long? Start(string jobName, string parameters)
        {
            _logger.Info("Checking status of job with name= {0}", jobName);
            JobParameters jobParameters = _jobParametersConverter.GetJobParameters(PropertiesConverter.StringToProperties(parameters));

            if (JobRepository.IsJobInstanceExists(jobName, jobParameters))
            {
                throw new JobInstanceAlreadyExistsException(string.Format("Cannot start a job instance that already exists with name={0} and parameters={1}", jobName, parameters));
            }

            IJob job = JobRegistry.GetJob(jobName);
            _logger.Info("Attempting to launch job with name={0} and parameters={1}", jobName, parameters);
            
            try
            {
                return JobLauncher.Run(job, jobParameters).Id;
            }
            catch (JobExecutionAlreadyRunningException e)
            {
                throw new UnexpectedJobExecutionException(string.Format(IllegalStateMsg, "job execution already running", jobName, parameters), e);
            }
            catch (JobRestartException e)
            {
                throw new UnexpectedJobExecutionException(string.Format(IllegalStateMsg, "job not restartable", jobName, parameters), e);
            }
            catch (JobInstanceAlreadyCompleteException e)
            {
                throw new UnexpectedJobExecutionException(string.Format(IllegalStateMsg, "job already complete", jobName, parameters), e);
            }
        }

        /// <summary>
        /// @see IJobOperator#StartNextInstance .
        /// </summary>
        /// <param name="jobName"></param>
        /// <returns></returns>
        /// <exception cref="NoSuchJobException">&nbsp;</exception>
        /// <exception cref="JobParametersNotFoundException">&nbsp;</exception>
        /// <exception cref="UnexpectedJobExecutionException">&nbsp;</exception>
        /// <exception cref="JobParametersInvalidException">&nbsp;</exception>
        public long? StartNextInstance(string jobName)
        {
            _logger.Info("Locating parameters for next instance of job with name={0}", jobName);

            IJob job = JobRegistry.GetJob(jobName);
            IList<JobInstance> lastInstances = JobExplorer.GetJobInstances(jobName, 0, 1);

            IJobParametersIncrementer incrementer = job.JobParametersIncrementer;
            if (incrementer == null)
            {
                throw new JobParametersNotFoundException(
                    string.Format("No job parameters incrementer found for job={0}", jobName));
            }

            JobParameters parameters;
            if (!lastInstances.Any())
            {
                parameters = incrementer.GetNext(new JobParameters());
                if (parameters == null)
                {
                    throw new JobParametersNotFoundException(
                        string.Format("No bootstrap parameters found for job={0}", jobName));
                }
            }
            else
            {
                IList<JobExecution> lastExecutions = JobExplorer.GetJobExecutions(lastInstances.First());
                parameters = incrementer.GetNext(lastExecutions.First().JobParameters);
            }

            _logger.Info("Attempting to launch job with name={0} and parameters={1}", jobName, parameters);
            try
            {
                return JobLauncher.Run(job, parameters).Id;
            }
            catch (JobExecutionAlreadyRunningException e)
            {
                throw new UnexpectedJobExecutionException(string.Format(IllegalStateMsg, "job already running", jobName, parameters), e);
            }
            catch (JobRestartException e)
            {
                throw new UnexpectedJobExecutionException(string.Format(IllegalStateMsg, "job not restartable", jobName, parameters), e);
            }
            catch (JobInstanceAlreadyCompleteException e)
            {
                throw new UnexpectedJobExecutionException(string.Format(IllegalStateMsg, "job instance already complete", jobName, parameters), e);
            }

        }

        /// <summary>
        /// @see IJobOperator#Stop .
        /// </summary>
        /// <param name="executionId"></param>
        /// <returns></returns>
        /// <exception cref="NoSuchJobException">&nbsp;</exception>
        /// <exception cref="JobExecutionNotRunningException">&nbsp;</exception>
        public bool Stop(long executionId)
        {
            JobExecution jobExecution = FindExecutionById(executionId);
            // Indicate the execution should be stopped by setting it's status to
            // 'STOPPING'. It is assumed that
            // the step implementation will check this status at chunk boundaries.
            BatchStatus status = jobExecution.Status;
            if (!(status == BatchStatus.Started || status == BatchStatus.Starting))
            {
                throw new JobExecutionNotRunningException(
                    string.Format("JobExecution must be running so that it can be stopped: {0}", jobExecution));
            }
            jobExecution.Status = BatchStatus.Stopping;
            JobRepository.Update(jobExecution);

            try
            {
                IJob job = JobRegistry.GetJob(jobExecution.JobInstance.JobName);
                var locator = job as IStepLocator;
                if (locator != null)
                {
                    //can only process as StepLocator is the only way to get the step object
                    //get the current stepExecution
                    foreach (StepExecution stepExecution in jobExecution.StepExecutions)
                    {
                        if (stepExecution.BatchStatus.IsRunning())
                        {
                            try
                            {
                                //have the step execution that's running -> need to 'stop' it
                                IStep step = locator.GetStep(stepExecution.StepName);
                                var taskletStep = step as TaskletStep;
                                if (taskletStep != null)
                                {
                                    ITasklet tasklet = taskletStep.Tasklet;
                                    var stoppableTasklet = tasklet as IStoppableTasklet;
                                    if (stoppableTasklet != null)
                                    {
                                        StepSynchronizationManager.Register(stepExecution);
                                        stoppableTasklet.Stop();
                                        StepSynchronizationManager.Release();
                                    }
                                }
                            }
                            catch (NoSuchStepException e)
                            {
                                _logger.Warn("Step not found {0}", e.Message);
                            }
                        }
                    }
                }
            }
            catch (NoSuchJobException e)
            {
                _logger.Warn("Cannot find Job object {0}", e.Message);
            }

            return true;
        }

        /// <summary>
        /// @see IJobOperator#Abandon .
        /// </summary>
        /// <param name="jobExecutionId"></param>
        /// <returns></returns>
        public JobExecution Abandon(long jobExecutionId)
        {
            JobExecution jobExecution = FindExecutionById(jobExecutionId);

            if (jobExecution.Status.IsLessThan(BatchStatus.Stopping))
            {
                throw new JobExecutionAlreadyRunningException("JobExecution is running or complete and therefore cannot be aborted");
            }

            _logger.Info("Aborting job execution: {0} ", jobExecution);
            jobExecution.UpgradeStatus(BatchStatus.Abandoned);
            jobExecution.EndTime = new DateTime?();
            JobRepository.Update(jobExecution);

            return jobExecution;
        }
        #endregion

        #region private methods
        /// <summary>
        /// find JobExecution given its id
        /// </summary>
        /// <param name="executionId"></param>
        /// <returns></returns>
        /// <exception cref="NoSuchJobExecutionException">&nbsp;if no jobexecution can be found for the given id</exception>
        private JobExecution FindExecutionById(long executionId)
        {
            JobExecution jobExecution = JobExplorer.GetJobExecution(executionId);
            if (jobExecution == null)
            {
                throw new NoSuchJobExecutionException(
                    string.Format("No JobExecution found for id: [{0}]", executionId));
            }
            return jobExecution;
        }
        #endregion
    }
}
