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

using System.Collections.Generic;

namespace Summer.Batch.Core.Launch
{
    /// <summary>
    /// Low level interface for inspecting and controlling jobs with access only to
    /// primitive and collection types. Suitable for a command-line client (e.g. that
    /// launches a new process for each operation), or a remote launcher.
    /// </summary>
    public interface IJobOperator
    {
        /// <summary>
        /// Lists the JobExecutions associated with a particular
        /// <see cref="JobInstance"/>, in reverse order of creation (and therefore usually
        /// of execution). 
        /// </summary>
        /// <param name="instanceId">the id of a JobInstance</param>
        /// <returns>the id values of all the  JobExecutions associated with this instance</returns>
        /// <exception cref="NoSuchJobInstanceException"></exception>
        IList<long?> GetExecutions(long instanceId);


        /// <summary>
        /// Lists the JobInstances for a given job name, in
        /// reverse order of creation (and therefore usually of first execution).
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        /// <exception cref="Summer.Batch.Core.Launch.NoSuchJobException"></exception>
        IList<long?> GetJobInstances(string jobName, int start, int count);


        /// <summary>
        /// Gets the id values of all the running JobExecutions
        /// with the given job name. 
        /// </summary>
        /// <param name="jobName"></param>
        /// <returns></returns>
        /// <exception cref="Summer.Batch.Core.Launch.NoSuchJobException"></exception>
        ICollection<long?> GetRunningExecutions(string jobName);

        /// <summary>
        /// Gets the <see cref="JobParameters"/> as an easily readable String.
        /// </summary>
        /// <param name="executionId"></param>
        /// <returns></returns>
        /// <exception cref="Summer.Batch.Core.Launch.NoSuchJobExecutionException"></exception>
        string GetParameters(long executionId);

        /// <summary>
        ///  Starts a new instance of a job with the parameters specified.
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <exception cref="Summer.Batch.Core.Launch.NoSuchJobException">if there is no Job with the specified name</exception>
        /// <exception cref="Summer.Batch.Core.Launch.JobInstanceAlreadyExistsException">if a job instance with this name and parameters already exists</exception>
        /// <exception cref="Summer.Batch.Core.JobParametersInvalidException"></exception>
        long? Start(string jobName, string parameters);

        /// <summary>
        /// Restarts a failed or stopped <see cref="JobExecution"/>; Fails with an exception
        /// if the id provided does not exist or corresponds to a <see cref="JobInstance"/>
        /// that in normal circumstances already completed successfully.
        /// </summary>
        /// <param name="executionId"></param>
        /// <returns></returns>
        /// <exception cref="Summer.Batch.Core.Launch.NoSuchJobException"></exception>
        /// <exception cref="Summer.Batch.Core.Launch.NoSuchJobExecutionException"></exception>
        /// <exception cref="Summer.Batch.Core.Repository.JobRestartException"></exception>
        /// <exception cref="Summer.Batch.Core.Repository.JobInstanceAlreadyCompleteException"></exception>
        /// <exception cref="Summer.Batch.Core.JobParametersInvalidException"></exception>
        long? Restart(long executionId);


        /// <summary>
        /// Launches the next in a sequence of <see cref="JobInstance"/> determined by the
        /// <see cref="IJobParametersIncrementer"/> attached to the specified job. If the
        /// previous instance is still in a failed state, this method should still
        /// create a new instance and run it with different parameters (as long as
        /// the IJobParametersIncrementer is working)
        /// </summary>
        /// <param name="jobName"></param>
        /// <returns></returns>
        /// <exception cref="Summer.Batch.Core.Launch.NoSuchJobException"></exception>
        /// <exception cref="Summer.Batch.Core.Launch.NoSuchJobExecutionException"></exception>
        /// <exception cref="Summer.Batch.Core.Launch.JobParametersNotFoundException"></exception>
        /// <exception cref="Summer.Batch.Core.Repository.JobRestartException"></exception>
        /// <exception cref="Summer.Batch.Core.Repository.JobInstanceAlreadyCompleteException"></exception>
        /// <exception cref="Summer.Batch.Core.JobParametersInvalidException"></exception> 
        /// <exception cref="Summer.Batch.Core.UnexpectedJobExecutionException"></exception> 
        long? StartNextInstance(string jobName);


        /// <summary>
        /// Sends a stop signal to the <see cref="JobExecution"/> with the supplied id. The
        /// signal is successfully sent if this method returns true, but that doesn't
        /// mean that the job has stopped. The only way to be sure of that is to poll
        /// the job execution status.
        /// </summary>
        /// <param name="executionId"></param>
        /// <returns></returns>
        ///  <exception cref="Summer.Batch.Core.Launch.NoSuchJobExecutionException"></exception>
        ///  <exception cref="Summer.Batch.Core.Launch.JobExecutionNotRunningException"></exception>
        bool Stop(long executionId);

        /// <summary>
        ///  Summarises the <see cref="JobExecution"/> with the supplied id, giving details
        /// of status, start and end times etc.
        /// </summary>
        /// <param name="executionId"></param>
        /// <returns></returns>
        /// <exception cref="Summer.Batch.Core.Launch.NoSuchJobExecutionException"></exception>
        string GetSummary(long executionId);

        /// <summary>
        /// Summarises the <see cref="StepExecution"/> instances belonging to the
        /// JobExecution with the supplied id, giving details of status,
        /// start and end times etc.
        /// </summary>
        /// <param name="executionId"></param>
        /// <returns></returns>
        /// <exception cref="Summer.Batch.Core.Launch.NoSuchJobExecutionException"></exception>
        IDictionary<long?, string> GetStepExecutionSummaries(long executionId);

        /// <summary>
        /// List the available job names that can be launched with #start(String, String).
        /// </summary>
        /// <returns></returns>
        ISet<string> GetJobNames();

        /// <summary>
        /// Mark the <see cref="JobExecution"/> as ABANDONED. If a stop signal is ignored
        /// because the process died this is the best way to mark a job as finished
        /// with (as opposed to STOPPED). An abandoned job execution can be
        /// restarted, but a stopping one cannot.
        /// </summary>
        /// <param name="jobExecutionId"></param>
        /// <returns></returns>
        /// <exception cref="Summer.Batch.Core.Launch.NoSuchJobExecutionException"></exception>
        /// <exception cref="Summer.Batch.Core.Repository.JobExecutionAlreadyRunningException"></exception>
        JobExecution Abandon(long jobExecutionId);

    }
}
