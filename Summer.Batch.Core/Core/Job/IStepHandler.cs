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

namespace Summer.Batch.Core.Job
{
    /// <summary>
    /// Strategy interface for handling an <see cref="IStep"/> on behalf of a <see cref="IJob"/>.
    /// </summary>
    public interface IStepHandler
    {
        /// <summary>
        /// Handles a step and returns the execution for it. Does not save the
        /// JobExecution, but should manage the persistence of the
        /// StepExecution if required (e.g. at least it needs to be added to
        /// a repository before the step can be executed).
        /// </summary>
        /// <param name="step">a step</param>
        /// <param name="jobExecution">a job execution</param>
        /// <returns></returns>
        /// <exception cref="JobInterruptedException">&nbsp;if there is an interruption</exception>
        /// <exception cref="JobRestartException">&nbsp;if there is a problem restarting a failed step</exception>
        /// <exception cref="StartLimitExceededException">&nbsp;if the step exceeds its start limit</exception>
        StepExecution HandleStep(IStep step, JobExecution jobExecution);

    }
}
