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

namespace Summer.Batch.Core.Repository
{
    /// <summary>
    /// JobExecutionAlreadyRunningException
    /// </summary>
    [Serializable]
    public class JobExecutionAlreadyRunningException : JobExecutionException
    {
        /// <summary>
        /// Custom constructor with a message.
        /// </summary>
        /// <param name="msg"></param>
        public JobExecutionAlreadyRunningException(string msg) : base(msg) { }

        /// <summary>
        /// Custom constructor with a message and an inner exception.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="cause"></param>
        public JobExecutionAlreadyRunningException(string msg, Exception cause) : base(msg, cause) { }
    }
}
