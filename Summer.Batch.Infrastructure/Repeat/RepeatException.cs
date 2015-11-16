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
using System.Runtime.Serialization;
using System.Text;

namespace Summer.Batch.Infrastructure.Repeat
{
    /// <summary>
    /// Exception thrown when repeat issues occur.
    /// </summary>
    [Serializable]
    public class RepeatException : System.Exception
    {
        /// <summary>
        /// Custom Message showing nested exception if any
        /// </summary>
        public override string Message
        {
            get
            {
                if (InnerException != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(base.Message).Append("; nested exception is ").Append(InnerException.Message);
                    return sb.ToString();
                }
                else
                {
                    return base.Message;
                }
            }
        }

        /// <summary>
        /// Custom constructor using a name
        /// </summary>
        /// <param name="msg"></param>
        public RepeatException(string msg) : base(msg) { }

        /// <summary>
        /// Custom constructor using a name and an inner exception
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="cause"></param>
        public RepeatException(string msg, System.Exception cause) : base(msg, cause) { }

        /// <summary>
        /// Constructor for Serialization support
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected RepeatException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
