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

namespace Summer.Batch.Infrastructure.Item
{
    /// <summary>
    /// Exception indicating that an IItemReader needs to be opened before read.
    /// </summary>
    [Serializable]
    public class ReaderNotOpenException : ItemReaderException
    {
        /// <summary>
        /// Creates a new <see cref="ReaderNotOpenException"/> based on a message and another exception.
        /// </summary>
        /// <param name="message">the error message</param>
        /// <param name="exception">the inner exception</param>
        public ReaderNotOpenException(string message, Exception exception) : base(message, exception)
        {
        }

        /// <summary>
        /// Creates a new <see cref="ReaderNotOpenException"/> based on a message.
        /// </summary>
        /// <param name="message">the error message</param>
        public ReaderNotOpenException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructor for deserialization.
        /// </summary>
        /// <param name="info">the info holding the serialization data</param>
        /// <param name="context">the serialization context</param>
        public ReaderNotOpenException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}