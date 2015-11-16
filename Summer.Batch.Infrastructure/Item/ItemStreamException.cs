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
    /// Exception representing any errors encountered while processing a stream.
    /// </summary>
    [Serializable]
    public class ItemStreamException : Exception
    {
        /// <summary>
        /// Constructs a new <see cref="ItemStreamException"/> with the specified message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public ItemStreamException(string message) : base(message) { }

        /// <summary>
        /// Constructs a new <see cref="ItemStreamException"/> with the specified message and inner exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="cause">The cause of the error.</param>
        public ItemStreamException(string message, Exception cause) : base(message, cause) { }

        /// <summary>
        /// Constructs a new <see cref="ItemStreamException"/> with the specified inner exception.
        /// </summary>
        /// <param name="cause">The cause of the error.</param>
        public ItemStreamException(Exception cause) : base(string.Empty, cause) { }

        /// <summary>
        /// Constructor for deserialization.
        /// </summary>
        /// <param name="info">the info holding the serialization data</param>
        /// <param name="context">the serialization context</param>
        protected ItemStreamException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}