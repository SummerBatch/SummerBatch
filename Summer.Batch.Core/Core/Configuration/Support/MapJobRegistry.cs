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

using Summer.Batch.Core.Launch;
using Summer.Batch.Common.Util;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Summer.Batch.Core.Configuration.Support
{
    /// <summary>
    /// Simple, thread-safe, dictionary-based implementation of IJobRegistry.
    /// </summary>
    public class MapJobRegistry : IJobRegistry
    {
        private readonly ConcurrentDictionary<string, IJobFactory> _map = new ConcurrentDictionary<string, IJobFactory>();

        #region IJobRegistry methods implementation

        /// <summary>
        /// Registers a job at runtime.
        /// </summary>
        /// <param name="jobFactory">the job to be registered</param>
        /// <exception cref="DuplicateJobException">if a factory with the same job name has already been registered</exception>
        public void Register(IJobFactory jobFactory)
        {
            Assert.NotNull(jobFactory);
            string name = jobFactory.JobName;
            Assert.NotNull(name, "Job configuration must have a name.");
            IJobFactory value = _map.GetOrAdd(name, jobFactory);
            if (value != jobFactory)
            {
                throw new DuplicateJobException("A job configuration with this name [" + name + "] was already registered");
            }
        }

        /// <summary>
        /// Unregisters a previously registered job. If it was not previously registered there is no error.
        /// </summary>
        /// <param name="name">the job to unregister</param>
        public void Unregister(string name)
        {
            Assert.NotNull(name, "Job configuration must have a name.");
            IJobFactory dummy;
            _map.TryRemove(name, out dummy);
        }

        #endregion

        #region IJobLocator methods implementation

        /// <summary>
        /// Locates a Job at runtime.
        /// </summary>
        /// <param name="name">the name of the Job which should be unique</param>
        /// <returns>a Job identified by the given name</returns>
        /// <exception cref="T:Summer.Batch.Core.Launch.NoSuchJobException"/>
        public IJob GetJob(string name)
        {
            IJobFactory factory;
            if (!_map.TryGetValue(name, out factory))
            {
                throw new NoSuchJobException("No job configuration with the name [" + name + "] was registered");
            }
            return factory.CreateJob();
        }

        #endregion

        #region IListableJobLocator methods implementation

        /// <summary>
        /// Provides the currently registered job names. The return value is
        /// unmodifiable and disconnected from the underlying registry storage.
        /// </summary>
        /// <returns>a collection of String. Empty if none are registered.</returns>
        public ICollection<string> GetJobNames()
        {
            return new ReadOnlyCollection<string>(_map.Keys.ToList());
        }

        #endregion

    }
}
