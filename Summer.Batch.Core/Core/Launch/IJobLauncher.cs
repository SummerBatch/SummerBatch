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

using Summer.Batch.Core.Repository;

namespace Summer.Batch.Core.Launch
{
    /// <summary>
    /// Simple interface for controlling jobs, including possible ad-hoc executions,
    /// based on different runtime identifiers. It is extremely important to note
    /// that this interface makes absolutely no guarantees about whether or not calls
    /// to it are executed synchronously or asynchronously. The docs for specific
    /// implementations should be checked to ensure callers fully understand how the
    /// job will be run.
    /// </summary>
    public interface IJobLauncher
    {
        /// <summary>
        /// Starts a job execution for the given <see cref="IJob"/> and <see cref="JobParameters"/>.
        /// If a <see cref="JobExecution"/> was able to be created successfully, it will
        /// always be returned by this method, regardless of whether or not the
        /// execution was successful. If there is a past JobExecution which
        /// has paused, the same JobExecution is returned instead of a new
        /// one created. A exception will only be thrown if there is a failure to
        /// start the job. If the job encounters some error while processing, the
        /// JobExecution will be returned, and the status will need to be inspected.
        /// 
        ///</summary>
        /// <param name="job"></param>
        /// <param name="jobParameters"></param>
        /// <returns>the JobExecution if it returns synchronously. If the
        /// implementation is asynchronous, the status might well be unknown</returns>
        /// <exception cref="JobExecutionAlreadyRunningException">&nbsp;if the JobInstance identified by the properties already has an execution running</exception>
        /// <exception cref="JobRestartException">&nbsp;if the job has been run before and circumstances that preclude a re-start</exception>
        /// <exception cref="JobInstanceAlreadyCompleteException">&nbsp; if the job has been run before with the same parameters and completed successfully</exception>
        /// <exception cref="JobParametersInvalidException">&nbsp;if the parameters are not valid for this job</exception>
        JobExecution Run(IJob job, JobParameters jobParameters);

    }
}
