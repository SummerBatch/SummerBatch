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

namespace Summer.Batch.Core.Configuration
{
    /// <summary>
    /// A runtime service registry interface for registering job configurations by name.
    /// </summary>
    public interface IJobRegistry : IListableJobLocator
    {
        /// <summary>
        /// Registers a job at runtime.
        /// </summary>
        /// <param name="jobFactory">the job to be registered</param>
        /// <exception cref="DuplicateJobException">&nbsp;if a factory with the same job name has already been registered</exception>
        void Register(IJobFactory jobFactory);

        /// <summary>
        /// Unregisters a previously registered job. If it was not previously registered there is no error.
        /// </summary>
        /// <param name="jobName">the job to unregister</param>
        void Unregister(string jobName);
    }
}
