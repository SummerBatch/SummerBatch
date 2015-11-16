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

using System.Collections.Specialized;

namespace Summer.Batch.Core.Converter
{
    /// <summary>
    ///  A factory for JobParameters instances. A job can be executed with
    /// many possible runtime parameters, which identify the instance of the job.
    /// This converter allows job parameters to be converted to and from Properties.
    /// </summary>
    public interface IJobParametersConverter
    {
        /// <summary>
        ///  Get a new JobParameters instance. If given null, or an empty
        /// properties, an empty JobParameters will be returned.
        /// </summary>
        /// <param name="properties">the runtime parameters in the form of String literals.</param>
        /// <returns>a JobParameters properties converted to the correct types.</returns>
        JobParameters GetJobParameters(NameValueCollection properties);

        /// <summary>
        /// The inverse operation: get a Properties instance. If given null
        /// or empty JobParameters, an empty Properties should be returned.
        /// </summary>
        /// <param name="parms"></param>
        /// <returns>a representation of the parameters as properties</returns>
        NameValueCollection GetProperties(JobParameters parms);
    }
}
