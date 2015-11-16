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
    /// Exception indicating that an error has been encountered doing I/O from a
    /// reader, and the exception should be considered fatal.
    /// </summary>
    [Serializable]
    public class NonTransientResourceException : ItemReaderException
    {
        /// <summary>
        /// Create a new NonTransientResourceException based on a message and another exception.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public NonTransientResourceException(string message, Exception exception)
            : base(message, exception)
        {
        }

        /// <summary>
        ///  Create a new NonTransientResourceException based on a message.
        /// </summary>
        /// <param name="message"></param>
        public NonTransientResourceException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor for deserialization.
        /// </summary>
        /// <param name="info">the info holding the serialization data</param>
        /// <param name="context">the serialization context</param>
        public NonTransientResourceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}