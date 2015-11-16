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
using System;
using System.Runtime.Serialization;

namespace Summer.Batch.Extra.Service.Stop
{
    /// <summary>
    /// Exception thrown when a job is stopped with <see cref="ServiceStop"/>.
    /// </summary>
    [Serializable]
    public class FailedJobException : Exception
    {
        /// <summary>
        /// Initializes a new instance with a specified error message.
        /// </summary>
        /// <param name="message">the message that describes the error</param>
        public FailedJobException(string message) : base(message) { }

        /// <summary>
        /// Constructor for deserialization.
        /// </summary>
        /// <param name="info">the info holding the serialization data</param>
        /// <param name="context">the serialization context</param>
        protected FailedJobException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}