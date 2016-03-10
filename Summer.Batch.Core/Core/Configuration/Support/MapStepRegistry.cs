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

using System.Collections.Concurrent;
using System.Collections.Generic;
using Summer.Batch.Core.Launch;
using Summer.Batch.Core.Step;
using Summer.Batch.Common.Util;

namespace Summer.Batch.Core.Configuration.Support
{
    /// <summary>
    /// Simple dictionary-based implementation of IStepRegistry. Access to the dictionary is
    /// synchronized, guarded by an internal lock.
    /// </summary>
    public class MapStepRegistry : IStepRegistry
    {

        private readonly ConcurrentDictionary<string, IDictionary<string, IStep>> _map =
            new ConcurrentDictionary<string, IDictionary<string, IStep>>();

        #region IStepRegistry methods implementation

        /// <summary>
        /// Registers all the step of the given job. If the job is already registered,
        /// the method <see cref="UnregisterStepFromJob"/> is called before registering
        /// the given steps.
        /// </summary>
        /// <param name="jobName"> the given job name</param>
        /// <param name="steps"> the job steps</param>
        /// <exception cref="DuplicateJobException">&nbsp; if a job with the same job name has already been registered.</exception>
        public void Register(string jobName, ICollection<IStep> steps)
        {
            Assert.NotNull(jobName, "The job name cannot be null.");
            Assert.NotNull(steps, "The job steps cannot be null.");


            IDictionary<string, IStep> jobSteps = new Dictionary<string, IStep>();
            foreach (IStep step in steps)
            {
                jobSteps.Add(step.Name, step);
            }

            if (_map.ContainsKey(jobName))
            {
                throw new DuplicateJobException(string.Format("A job configuration with this name [{0}] was already registered", jobName));
            }
            else
            {
                _map.TryAdd(jobName, jobSteps);
            }

        }

        /// <summary>
        ///  Unregisters all the steps of the given job. If the job is not registered, nothing happens.
        /// </summary>
        /// <param name="jobName"> the given job name</param>
        public void UnregisterStepFromJob(string jobName)
        {
            Assert.NotNull(jobName, "Job configuration must have a name.");
            IDictionary<string, IStep> toBeRemoved;
            _map.TryRemove(jobName, out toBeRemoved);
        }

        /// <summary>
        /// Returns the IStep of the specified job based on its name.
        /// </summary>
        /// <param name="jobName"> the name of the job</param>
        /// <param name="stepName"> the name of the step to retrieve</param>
        /// <returns>the step with the given name belonging to the mentioned job</returns>
        /// <exception cref="NoSuchJobException">&nbsp; no such job with that name exists</exception>
        /// <exception cref="NoSuchStepException">&nbsp; no such step with that name for that job exists</exception>
        public IStep GetStep(string jobName, string stepName)
        {
            Assert.NotNull(jobName, "The job name cannot be null.");
            Assert.NotNull(stepName, "The step name cannot be null.");
            if (!_map.ContainsKey(jobName))
            {
                throw new NoSuchJobException(
                    string.Format("No job configuration with the name [{0}] was registered", jobName));
            }
            var jobSteps = _map[jobName]; // we are sure the key is present
            if (jobSteps.ContainsKey(stepName))
            {
                return jobSteps[stepName];
            }
            throw new NoSuchStepException(
                string.Format("The step called [{0}] does not exist in the job [{1}]", stepName, jobName));
        }

        #endregion
    }
}
