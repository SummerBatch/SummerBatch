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
using Summer.Batch.Core.Repository;
using Summer.Batch.Common.TaskExecution;
using Summer.Batch.Common.Util;
using System;
using System.Threading.Tasks;
using Summer.Batch.Common.Factory;

namespace Summer.Batch.Core.Launch.Support
{
    /// <summary>
    /// Simple implementation of the <see cref="IJobLauncher"/> interface. The 
    /// <see cref="ITaskExecutor"/> interface is used to launch a Job. This means
    /// that the type of executor set is very important. If a
    /// <see cref="SyncTaskExecutor"/> is used, then the job will be processed
    /// <strong>within the same thread that called the launcher.</strong> Care should
    /// be taken to ensure any users of this class understand fully whether or not
    /// the implementation of TaskExecutor used will start tasks synchronously or
    /// asynchronously. The default setting uses a synchronous task executor.
    ///
    /// There is only one required dependency of this Launcher, an
    /// <see cref="IJobRepository"/>. The IJobRepository is used to obtain a valid
    /// <see cref="JobExecution"/>. The Repository must be used because the provided Job
    /// could be a restart of an existing  <see cref="JobInstance"/>, and only the
    /// Repository can reliably recreate it.
    /// </summary>
    public class SimpleJobLauncher : IJobLauncher, IInitializationPostOperations
    {
        /// <summary>
        /// Logger.
        /// </summary>
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private IJobRepository _jobRepository;
        
        /// <summary>
        /// Job repository property.
        /// </summary>
        public IJobRepository JobRepository { set { _jobRepository = value; } }

        private ITaskExecutor _taskExecutor = new SyncTaskExecutor();

        /// <summary>
        /// Task executor property.
        /// </summary>
        public ITaskExecutor TaskExecutor { set { _taskExecutor = value; } }

        /// <summary>
        /// Runs the provided job with the given <see cref="JobParameters"/>. The
        /// JobParameters will be used to determine if this is an execution
        /// of an existing job instance, or if a new one should be created.
        /// </summary>
        /// <param name="job"></param>
        /// <param name="jobParameters"></param>
        /// <returns></returns>
        /// <exception cref="JobRestartException"> if the execution would be a re-start, but a re-start is either not allowed or not needed.</exception>
        /// <exception cref="JobExecutionAlreadyRunningException">if the JobInstance already exists and has an execution already running</exception>
        /// <exception cref="JobInstanceAlreadyCompleteException"> if this instance has already completed successfully</exception>
        /// <exception cref="JobParametersInvalidException">if given parameters do not pass validation process</exception>
        public JobExecution Run(IJob job, JobParameters jobParameters)
        {
            Assert.NotNull(job, "The job must be not null");
            Assert.NotNull(jobParameters, "The job parameters must be not null");
            JobExecution lastExecution = _jobRepository.GetLastJobExecution(job.Name, jobParameters);

            //manage last execution if needed
            HandleLastExecution(job, lastExecution);

            //validate Parameters            
            job.JobParametersValidator.Validate(jobParameters);

            //create new job execution
            var jobExecution = _jobRepository.CreateJobExecution(job.Name, jobParameters);

            //make an creation to be able to create a task, that will be executed by given TaskExecutor as expected
            var jobAction = CreateJobAction(job, jobParameters, jobExecution);

            using (var jobTask = new Task(jobAction))
            {
                try
                {
                    _taskExecutor.Execute(jobTask);
                    //in case of asynchronous executor ...
                    jobTask.Wait();
                }
                catch (InvalidOperationException exception)
                {
                    jobExecution.UpgradeStatus(BatchStatus.Failed);
                    if (jobExecution.ExitStatus.Equals(ExitStatus.Unknown))
                    {
                        jobExecution.ExitStatus = ExitStatus.Failed.AddExitDescription(exception);
                    }
                    _jobRepository.Update(jobExecution);
                }
            }
            return jobExecution;
        }

        /// <summary>
        /// Action creation helper.
        /// Given a job, job parameters and a job execution, 
        /// will wrap the execution of the job into a <see cref="System.Action"/>.
        /// </summary>
        /// <param name="job">the job to execute</param>
        /// <param name="jobParameters">the job parameters</param>
        /// <param name="jobExecution">the job execution</param>
        /// <returns></returns>
        private static Action CreateJobAction(IJob job, JobParameters jobParameters, JobExecution jobExecution)
        {
            Action jobAction = (() =>
            {
                try
                {
                    Logger.Info("Job: [{0} ] launched with the following parameters:[{1}]",job,jobParameters);
                    job.Execute(jobExecution);
                    Logger.Info("Job: [{0}] completed with the following parameters:[{1}] and the following status: [{2}]",
                                 job,jobParameters,jobExecution.Status);

                }
                catch (Exception exception)
                {
                    Logger.Info("Job: [{0}] failed unexpectedly and fatally with the following parameters: [{1}]",job,exception);
                     throw;
                }
            });
            return jobAction;
        }

        /// <summary>
        /// Manage last job execution if required.
        /// </summary>
        /// <param name="job"></param>
        /// <param name="lastExecution"></param>
        /// <exception cref="JobRestartException"></exception>
        private static void HandleLastExecution(IJob job, JobExecution lastExecution)
        {
            //Last Execution handling
            if (lastExecution != null)
            {
                if (!job.Restartable)
                {
                    throw new JobRestartException("JobInstance already exists and is not restartable !");
                }
                foreach (StepExecution execution in lastExecution.StepExecutions)
                {
                    if (execution.BatchStatus == BatchStatus.Unknown)
                    {                      
                        throw new JobRestartException(string.Format("Step [{0}] is of status UNKNOWN", execution.StepName));
                    } 
                }
            }
        }

        /// <summary>
        /// Ensure the required dependencies of an <see cref="IJobRepository"/> have been
        /// set.
        /// Used programmatically by JobStepBuilder
        /// see IInitializationPostOperations#AfterPropertiesSet
        /// </summary>
        /// <exception cref="System.Exception"></exception>
        public void AfterPropertiesSet()
        {
            Assert.State(_jobRepository != null, "A JobRepository has not been set.");
        }
    }
}
