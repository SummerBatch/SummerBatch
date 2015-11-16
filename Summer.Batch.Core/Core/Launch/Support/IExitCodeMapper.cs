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
    /// This interface should be implemented when an environment calling the batch
    /// framework has specific requirements regarding the operating system process
    /// return status.
    /// </summary>
    public interface IExitCodeMapper
    {
        /// <summary>
        /// Convert the exit code from String into an integer that the calling
        /// environment as an operating system can interpret as an exit status.
        /// </summary>
        /// <param name="exitCode">The exit code which is used internally.</param>
        /// <returns>The corresponding exit status as known by the calling
        /// environment</returns>
        int? IntValue(string exitCode);
    }
}
