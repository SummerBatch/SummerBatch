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

namespace Summer.Batch.Core.Explore
{
    /// <summary>
    /// Entry point for browsing executions of running or historical jobs and steps.
    /// Since the data may be re-hydrated from persistent storage, it may not contain
    /// volatile fields that would have been present when the execution was active.
    /// </summary>
    public interface IJobExplorer
    {
        /// <summary>
        /// Fetches JobInstance values in descending order of creation (and therefore usually of first execution).
        /// </summary>
        /// <param name="jobName">the name of the job to query</param>
        /// <param name="start">the start index of the instances to return</param>
        /// <param name="count">the maximum number of instances to return</param>
        /// <returns>the JobInstance values up to a maximum of count values</returns>
        IList<JobInstance> GetJobInstances(string jobName, int start, int count);

        /// <summary>
        /// Retrieves a JobExecution by its id. The complete object graph for
        /// this execution should be returned (unless otherwise indicated) including
        /// the parent JobInstance and associated ExecutionContext
        /// and StepExecution instances (also including their execution
        /// contexts).
        /// </summary>
        /// <param name="executionId">the job execution id</param>
        /// <returns>the JobExecution with this id, or null if not found</returns>
        JobExecution GetJobExecution(long executionId);

        /// <summary>
        /// Retrieves a StepExecution by its id and parent
        /// JobExecution id. The execution context for the step should be
        /// available in the result, and the parent job execution should have its
        /// primitive properties, but may not contain the job instance information.
        /// </summary>
        /// <param name="jobExecutionId">the parent job execution id</param>
        /// <param name="stepExecutionId">the step execution id</param>
        /// <returns>the StepExecution with this id, or null if not found</returns>
        StepExecution GetStepExecution(long jobExecutionId, long stepExecutionId);

        ///<summary>
        /// Gets the Job Instance given its instance id
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns>the JobInstance with this id, or null</returns>
        JobInstance GetJobInstance(long instanceId);

        /// <summary>
        /// Retrieves job executions by their job instance. The corresponding step
        /// executions may not be fully hydrated (e.g. their execution context may be
        /// missing), depending on the implementation. Use
        /// GetStepExecution(Long, Long) to hydrate them in that case.
        /// </summary>
        /// <param name="jobInstance">the JobInstance to query</param>
        /// <returns>the set of all executions for the specified JobInstance</returns>
        IList<JobExecution> GetJobExecutions(JobInstance jobInstance);

        /// <summary>
        /// Retrieves running job executions. The corresponding step executions may
        /// not be fully hydrated (e.g. their execution context may be missing),
        /// depending on the implementation. Use
        /// GetStepExecution(Long, Long) to hydrate them in that case.
        /// </summary>
        /// <param name="jobName"> the name of the job</param>
        /// <returns>the set of running executions for jobs with the specified name</returns>
        ISet<JobExecution> FindRunningJobExecutions(string jobName);

        /// <summary>
        /// Queries the repository for all unique JobInstance names (sorted alphabetically).
        /// </summary>
        /// <returns>the set of job names that have been executed</returns>
        IList<string> GetJobNames();

        /// <summary>
        /// Queries the repository for the number of unique JobInstances
        /// associated with the supplied job name.
        /// </summary>
        /// <param name="jobName">the name of the job to query for</param>
        /// <returns>the number of JobInstances that exist within the associated job repository</returns>
        /// <exception cref="T:Summer.Batch.Core.Launch.NoSuchJobException"/>
        int GetJobInstanceCount(string jobName);
    }
}
