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
using System.Collections.Generic;

namespace Summer.Batch.Core.Repository
{
    /// <summary>
    /// Repository responsible for persistence of batch meta-data entities.
    /// </summary>
    public interface IJobRepository
    {

        /// <summary>
        /// Checks if an instance of this job already exists with the parameters
        /// provided.
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="jobParameters"></param>
        /// <returns></returns>
        bool IsJobInstanceExists(string jobName, JobParameters jobParameters);

        /// <summary>
        /// Creates a new JobInstance with the name and job parameters provided.
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="jobParameters"></param>
        /// <returns></returns>
        JobInstance CreateJobInstance(string jobName, JobParameters jobParameters);

        /// <summary>
        /// Creates a new JobExecution based upon the JobInstance it's associated
        /// with, the JobParameters used to execute it with and the location of the configuration
        /// file that defines the job.
        /// </summary>
        /// <param name="jobInstance"></param>
        /// <param name="jobParameters"></param>
        /// <param name="jobConfigurationLocation"></param>
        /// <returns></returns>
        JobExecution CreateJobExecution(JobInstance jobInstance, JobParameters jobParameters, string jobConfigurationLocation);

        /// <summary>
        /// <p>
        /// Creates a JobExecution for a given Job and JobParameters. If matching JobInstance already exists,
        /// the job must be restartable and it's last JobExecution must *not* be
        /// completed. If matching JobInstance does not exist yet it will be created.
        /// </p>
        ///
        /// <p>
        /// If this method is run in a transaction (as it normally would be) with
        /// isolation level at Isolation#REPEATABLE_READ or better, then this
        /// method should block if another transaction is already executing it (for
        /// the same JobParameters and job name). The first transaction to
        /// complete in this scenario obtains a valid  JobExecution, and
        /// others throw JobExecutionAlreadyRunningException (or timeout).
        /// There are no such guarantees if the JobInstanceDao and
        /// JobExecutionDao do not respect the transaction isolation levels
        /// (e.g. if using a non-relational data-store, or if the platform does not
        /// support the higher isolation levels).
        /// </p>
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="jobParameters"></param>
        /// <returns></returns>
        /// <exception cref="JobExecutionAlreadyRunningException"></exception>
        /// <exception cref="JobRestartException"></exception>
        /// <exception cref="JobInstanceAlreadyCompleteException"></exception>
        JobExecution CreateJobExecution(String jobName, JobParameters jobParameters);

        /// <summary>
        /// Updates the JobExecution (but not its ExecutionContext).
        ///
        /// Preconditions: JobExecution must contain a valid
        /// JobInstance and be saved (have an id assigned).
        /// </summary>
        /// <param name="jobExecution"></param>
        void Update(JobExecution jobExecution);

        /// <summary>
        ///	 Saves the StepExecution and its ExecutionContext. ID will
        /// be assigned - it is not permitted that an ID be assigned before calling
        /// this method. Instead, it should be left blank, to be assigned by a
        /// JobRepository.
        ///
        /// Preconditions: StepExecution must have a valid Step.
        /// </summary>
        /// <param name="stepExecution"></param>
        void Add(StepExecution stepExecution);

        /// <summary>
        /// Saves a collection of  StepExecutions and each  ExecutionContext. The
        /// StepExecution ID will be assigned - it is not permitted that an ID be assigned before calling
        ///this method. Instead, it should be left blank, to be assigned by  JobRepository.
        ///
        /// Preconditions: StepExecution must have a valid Step.
        /// </summary>
        /// <param name="stepExecutions"></param>
        void AddAll(ICollection<StepExecution> stepExecutions);

        /// <summary>
        /// Updates the  StepExecution (but not its ExecutionContext).
        /// Preconditions: StepExecution must be saved (have an id assigned).
        /// </summary>
        /// <param name="stepExecution"></param>
        void Update(StepExecution stepExecution);

        /// <summary>
        /// 
        /// Persists the updated ExecutionContexts of the given
        /// StepExecution.
        /// </summary>
        /// <param name="stepExecution"></param>
        void UpdateExecutionContext(StepExecution stepExecution);

        /// <summary>
        /// 
        /// Persists the updated  ExecutionContext of the given
        ///JobExecution
        /// </summary>
        /// <param name="jobExecution"></param>
        void UpdateExecutionContext(JobExecution jobExecution);

        /// <summary>
        /// Returns the last step execution.
        /// </summary>
        /// <param name="jobInstance"></param>
        /// <param name="stepName">the name of the step execution that might have run.</param>
        /// <returns>the last execution of step for the given job instance.</returns>
        StepExecution GetLastStepExecution(JobInstance jobInstance, string stepName);

        /// <summary>
        /// Returns the step executions count.
        /// </summary>
        /// <param name="jobInstance"></param>
        /// <param name="stepName">the name of the step execution that might have run.</param>
        /// <returns>the execution count of the step within the given job instance.</returns>
        int GetStepExecutionCount(JobInstance jobInstance, string stepName);

        /// <summary>
        /// Returns the last JobExecution.
        /// </summary>
        /// <param name="jobName">the name of the job that might have run</param>
        /// <param name="jobParameters">parameters identifying the JobInstance</param>
        /// <returns>the last execution of job if exists, null otherwise</returns>
        JobExecution GetLastJobExecution(string jobName, JobParameters jobParameters);
    }
}
