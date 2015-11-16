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
using System.Runtime.Serialization;

namespace Summer.Batch.Common.Property
{
    /// <summary>
    /// Exception thrown when trying to retrieve an invalid property.
    /// </summary>
    [Serializable]
    public class InvalidPropertyException : Exception
    {
        /// <summary>
        /// The type of the accessed object.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// The name of the accessed property
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        /// Constructs a new <see cref="InvalidPropertyException"/>
        /// </summary>
        /// <param name="message">the detail message</param>
        /// <param name="type">the type of the object</param>
        /// <param name="propertyName">the name of the property</param>
        /// <param name="innerException">the inner exception</param>
        public InvalidPropertyException(string message, Type type, string propertyName, Exception innerException = null)
            : base(message, innerException)
        {
            Type = type;
            PropertyName = propertyName;
        }

        /// <summary>
        /// Constructor for deserialization.
        /// </summary>
        /// <param name="info">the info holding the serialization data</param>
        /// <param name="context">the serialization context</param>
        protected InvalidPropertyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Type = (Type) info.GetValue("Type", typeof(Type));
            PropertyName = info.GetString("PropertyName");
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
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            info.AddValue("Type",Type);
            info.AddValue("PropertyName",PropertyName);
            base.GetObjectData(info, context);
        }
    }
}