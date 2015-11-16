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

namespace Summer.Batch.Core.Job.Flow
{
    /// <summary>
    /// Exception used when an issue with the flow execution occurs.
    /// see <see cref="Summer.Batch.Core.Job.Flow.Support.SimpleFlow"/> for typical usages.
    /// </summary>
    [Serializable]
    public class FlowExecutionException : Exception
    {
       /// <summary>
       /// Custom constructor with a message.
       /// </summary>
       /// <param name="message"></param>
        public FlowExecutionException(string message) : base(message)
        {
        }

        /// <summary>
        /// Custom constructor with a message and an inner cause.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cause"></param>
        public FlowExecutionException(string message, Exception cause) : base(message,cause)
        {
        }
            
    }
}