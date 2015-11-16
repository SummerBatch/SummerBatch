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

namespace Summer.Batch.Core.Configuration.Support
{
    /// <summary>
    /// Implementation of  <see cref="IJobFactory"/> that just keeps a reference to an <see cref="IJob"/> without modifying it.
    /// </summary>
    public class ReferenceJobFactory : IJobFactory
    {
        private readonly IJob _job;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="job">The job that will be returned by <see cref="CreateJob"/>.</param>
        public ReferenceJobFactory(IJob job)
        {
            _job = job;
        }

        /// <summary>
        /// The name of the job returned by <see cref="CreateJob"/>.
        /// </summary>
        public string JobName
        {
            get { return _job.Name; }
        }

        /// <summary>
        /// Simply returns the instance that was passed to the constructor.
        /// </summary>
        /// <returns>The job instance passed to the constructor.</returns>
        public IJob CreateJob()
        {
            return _job;
        }


    }
}
