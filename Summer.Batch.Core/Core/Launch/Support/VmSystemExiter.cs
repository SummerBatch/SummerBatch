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

namespace Summer.Batch.Core.Launch.Support
{
    /// <summary>
    /// Implementation of the <see cref="ISystemExiter"/> interface that calls the standards
    /// Environment.Exit method. It should be noted that there will be no unit tests for
    /// this class, since there is only one line of actual code, that would only be
    /// testable by mocking System or Runtime.
    /// </summary>
    public class VmSystemExiter : ISystemExiter
    {
        /// <summary>
        /// Terminates the current running Virtual Machine.
        /// Delegates to Environment.Exit .
        /// </summary>
        /// <param name="status">exit status</param>
        public void Exit(int status)
        {
            Environment.Exit(status);
        }
    }
}
