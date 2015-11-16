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

using NLog;
using System;
using System.Collections.Generic;

namespace Summer.Batch.Core.Launch.Support
{
    /// <summary>
    ///  An implementation of  <see cref="IExitCodeMapper"/> that can be configured through a
    /// dictioanry from batch exit codes (string) to integer results. Some default entries
    /// are set up to recognise common cases.  Any that are injected are added to these.
    /// </summary>
    public class SimpleVmExitCodeMapper : IExitCodeMapper
    {

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Mapping property.
        /// </summary>
        public Dictionary<string, int> Mapping { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SimpleVmExitCodeMapper()
        {
            //using object initializer
            Mapping = new Dictionary<string, int>
            {
                {ExitStatus.Completed.ExitCode, ExitCodeMapperConstants.VmExitcodeCompleted},
                {ExitStatus.Failed.ExitCode, ExitCodeMapperConstants.VmExitcodeGenericError},
                {ExitCodeMapperConstants.JobNotProvided, ExitCodeMapperConstants.VmExitcodeJobError},
                {ExitCodeMapperConstants.NoSuchJob, ExitCodeMapperConstants.VmExitcodeJobError}
            };
        }

        /// <summary>
        /// Gets the operating system exit status that matches a certain Batch
        /// Framework exit code.
        /// </summary>
        /// <param name="exitCode">The exit code of the Batch Job as known by the Batch Framework</param>
        /// <returns>The exitCode of the Batch Job as known by the VM</returns>
        public int? IntValue(string exitCode)
        {

            int? statusCode = null;

            try
            {
                statusCode = Mapping[exitCode];
            }
            catch (Exception e)
            {
                // We still need to return an exit code, even if there is an issue
                // with the mapper.
                _logger.Fatal(e, "Error mapping exit code, generic exit status returned.");
            }

            return statusCode ?? ExitCodeMapperConstants.VmExitcodeGenericError;
        }
    }
}
