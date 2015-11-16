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


using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Summer.Batch.Core
{
    /// <summary>
    /// Default implementation of the <see cref="IJobKeyGenerator{JobParameters}"/> interface.
    /// </summary>
    public class DefaultJobKeyGenerator : IJobKeyGenerator<JobParameters>
    {

        /// <summary>
        /// Generates the job key to be used based on the JobParameters instance
        /// provided.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public string GenerateKey(JobParameters source)
        {

            IDictionary<string, JobParameter> props = source.GetParameters();
            StringBuilder stringBuffer = new StringBuilder();
            List<string> keys = new List<string>(props.Keys);
            keys.Sort();
            foreach (string key in keys)
            {
                JobParameter jobParameter;
                var got = props.TryGetValue(key, out jobParameter);
                if (got && jobParameter.Identifying)
                {
                    string value = jobParameter.Value == null ? "" : jobParameter.ToString();
                    stringBuffer.Append(key + "=" + value + ";");
                }
            }

            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(stringBuffer.ToString()));

                StringBuilder oSb = new StringBuilder();
                foreach (var bytev in bytes)
                {
                    oSb.Append(bytev.ToString("x2"));
                }
                return oSb.ToString();
            }
        }
    }
}
