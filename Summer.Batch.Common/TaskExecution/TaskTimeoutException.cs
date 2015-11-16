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
//  distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

// This file has been modified.
// Original copyright notice :

/*
 * Copyright 2002-2012 the original author or authors.
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

namespace Summer.Batch.Common.TaskExecution
{
    /// <summary>
    /// Exception thrown when a IAsyncTaskExecutor rejects to 
    /// accept a given task for execution because of the specified timeout.
    /// </summary>
    [Serializable]
    public class TaskTimeoutException : TaskRejectedException
    {
        /// <summary>
        /// Custom constructor using a message.
        /// </summary>
        /// <param name="message"></param>
        public TaskTimeoutException(string message) : base(message) { }

        /// <summary>
        /// Custom constructor using a message and an inner exception.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cause"></param>
        public TaskTimeoutException(string message, Exception cause) : base(message, cause) { }
    }
}
