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
using Summer.Batch.Core.Launch;
using Summer.Batch.Core.Step;

namespace Summer.Batch.Core.Configuration
{
    /// <summary>
    /// Registry keeping track of all the IStep defined in an IJob.
    /// </summary>
    public interface IStepRegistry
    {
        /// <summary>
        /// Registers all the step of the given job. If the job is already registered,
        /// the method #UnregisterStepsFromJob(string)} is called before registering
        /// the given steps.
        /// </summary>
        /// <param name="jobName"> the given job name</param>
        /// <param name="steps"> the job steps</param>
        /// <exception cref="DuplicateJobException"> if a job with the same job name has already been registered.</exception>
        void Register(string jobName, ICollection<IStep> steps);

        /// <summary>
        ///  Unregisters all the steps of the given job. If the job is not registered, nothing happens.
        /// </summary>
        /// <param name="jobName"> the given job name</param>
        void UnregisterStepFromJob(string jobName);

        /// <summary>
        /// Returns the IStep of the specified job based on its name.
        /// </summary>
        /// <param name="jobName"> the name of the job</param>
        /// <param name="stepName"> the name of the step to retrieve</param>
        /// <returns>the step with the given name belonging to the mentioned job</returns>
        /// <exception cref="NoSuchJobException"> no such job with that name exists</exception>
        /// <exception cref="NoSuchStepException"> no such step with that name for that job exists</exception>
        IStep GetStep(string jobName, string stepName);
    }
}
