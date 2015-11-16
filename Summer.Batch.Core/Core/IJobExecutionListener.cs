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

namespace Summer.Batch.Core
{
    /// <summary>
    /// Provide callbacks at specific points in the lifecycle of an <see cref="IJob"/>.
    /// Implementations can be stateful if they are careful to either ensure thread
    /// safety, or to use one instance of a listener per job, assuming that job
    /// instances themselves are not used by more than one thread.
    /// </summary>
    public interface IJobExecutionListener
    {
        /// <summary>
        /// Callback before a job executes.
        /// </summary>
        /// <param name="jobExecution"></param>
        void BeforeJob(JobExecution jobExecution);

        /// <summary>
        ///Callback after completion of a job. Called after both both successful and
        /// failed executions. To perform logic on a particular status, use
        /// "if (jobExecution.Status== BatchStatus.X)".
        /// </summary>
        /// <param name="jobExecution"></param>
        void AfterJob(JobExecution jobExecution);
    }
}
