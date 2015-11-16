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

namespace Summer.Batch.Extra.Ebcdic.Exception
{
    /// <summary>
    ///  Exception thrown by EbcdicReader and EbcdicWriter when the format of a field is missing or unknown.
    /// </summary>
    [Serializable]
    public class UnexpectedFieldTypeException : EbcdicException
    {
        private readonly char _fieldType;

        /// <summary>
        /// Custom constructor with a message
        /// </summary>
        /// <param name="message"></param>
        public UnexpectedFieldTypeException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Custom constructor with a message and an inner exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cause"></param>
        public UnexpectedFieldTypeException(string message, System.Exception cause)
            : base(message, cause)
        {
        }

        /// <summary>
        /// Custom constructor with a filedType
        /// </summary>
        /// <param name="fieldType"></param>
        public UnexpectedFieldTypeException(char fieldType)
            : base("Unexpected field encountered: " + fieldType)
        {
            _fieldType = fieldType;
        }

        /// <summary>
        /// Constructor for deserialization.
        /// </summary>
        /// <param name="info">the info holding the serialization data</param>
        /// <param name="context">the serialization context</param>
        public UnexpectedFieldTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _fieldType = info.GetChar("FieldType");
        }

        /// <summary>
        /// override on getMessage
        /// </summary>
        public override string Message
        {
            get { return "Unexpected field encountered: " + _fieldType; }
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
            base.GetObjectData(info,context);
            info.AddValue("FieldType",_fieldType);
        }
    }
}