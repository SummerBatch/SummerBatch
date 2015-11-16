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

namespace Summer.Batch.Core.Launch.Support
{
    /// <summary>
    /// Helper class for id incrementation on job parameters.
    /// </summary>
    public class RunIdIncrementer : IJobParametersIncrementer
    {
        private const string RunIdKey = "run.id";

        private string _key = RunIdKey;

        /// <summary>
        /// Key property.
        /// </summary>
        public string Key { set { _key = value; } }


        /// <summary>
        /// Increment the run.id parameter (starting with 1).
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public JobParameters GetNext(JobParameters parameters)
        {
            JobParameters parms = parameters ?? new JobParameters();
            long id = parms.GetLong(_key, 0L) + 1;
            return new JobParametersBuilder(parms).AddLong(_key, id).ToJobParameters();
        }
    }
}
