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
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Summer.Batch.Common.Proxy
{
    /// <summary>
    /// Exception thrown while creating or manipulating a proxy.
    /// </summary>
    [Serializable]
    public class ProxyException : Exception
    {
        private readonly IEnumerable<Type> _proxiedTypes;

        /// <summary>
        /// The type that is being proxied.
        /// </summary>
        public IEnumerable<Type> ProxiedTypes { get { return _proxiedTypes; } }

        /// <summary>
        /// Constructs a new <see cref="ProxyException"/> with the specified message.
        /// </summary>
        /// <param name="message">the exception message</param>
        public ProxyException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructs a new <see cref="ProxyException"/> with the specified message and proxied type.
        /// </summary>
        /// <param name="message">the exception message</param>
        /// <param name="proxiedTypes">the type bieng proxied</param>
        public ProxyException(string message, IEnumerable<Type> proxiedTypes)
            : base(message)
        {
            _proxiedTypes = proxiedTypes;
        }

        /// <summary>
        /// Constructor for deserialization.
        /// </summary>
        /// <param name="info">the info holding the serialization data</param>
        /// <param name="context">the serialization context</param>
        protected ProxyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _proxiedTypes = (IEnumerable<Type>) info.GetValue("_proxiedType", typeof (IEnumerable<Type>));
        }

        /// <summary>
        /// Sets the <see cref="SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("_proxiedType", _proxiedTypes);
        }
    }
}