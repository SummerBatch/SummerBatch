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

using Summer.Batch.Common.Util;
using System;

namespace Summer.Batch.Core
{
    /// <summary>
    /// Batch domain object representing a uniquely identifiable job run.
    /// JobInstance can be restarted multiple times in case of execution failure and
    /// it's lifecycle ends with first successful execution.
    ///
    /// Trying to execute an existing JobInstance that has already completed
    /// successfully will result in error. Error will be raised also for an attempt
    /// to restart a failed JobInstance if the Job is not restartable.
    /// </summary>
    [Serializable]
    public class JobInstance : Entity
    {
        private readonly string _jobName;

        /// <summary>
        /// Job name.
        /// </summary>
        public string JobName { get { return _jobName; } }

        /// <summary>
        /// Custom constructor using id and name.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="jobName"></param>
        public JobInstance(long id, string jobName)
            : base(id)
        {
            Assert.HasLength(jobName);
            _jobName = jobName;
        }

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0}, Job=[{1}]",base.ToString(),_jobName);            
        }
    }
}
