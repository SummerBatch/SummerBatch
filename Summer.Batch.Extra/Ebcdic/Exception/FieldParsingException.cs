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
using Summer.Batch.Extra.Copybook;

namespace Summer.Batch.Extra.Ebcdic.Exception
{
    /// <summary>
    /// Exception is thrown by EbcdicDecoder to signal an error while trying to parse an Ebcdic field.
    /// </summary>
    [Serializable]
    public class FieldParsingException : EbcdicException
    {

        private readonly byte[] _readData;
        private readonly FieldFormat _fieldFormat;

        /// <summary>
        /// Custom constructor with a message
        /// </summary>
        /// <param name="message"></param>
        public FieldParsingException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Custom constructor with a message and an inner exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cause"></param>
        public FieldParsingException(string message, System.Exception cause)
            : base(message, cause)
        {
        }

        /// <summary>
        /// Custom constructor using FieldFormat and read data
        /// </summary>
        /// <param name="fieldFormat"></param>
        /// <param name="readData"></param>
        public FieldParsingException(FieldFormat fieldFormat, byte[] readData) :
            base(string.Format("Error while reading field {0} - read data: {1}",
            fieldFormat.Name,
            readData))
        {
            _fieldFormat = fieldFormat;
            _readData = new byte[readData.Length];
            readData.CopyTo(_readData, 0);
        }

        /// <summary>
        /// Constructor for deserialization.
        /// </summary>
        /// <param name="info">the info holding the serialization data</param>
        /// <param name="context">the serialization context</param>
        public FieldParsingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _fieldFormat = (FieldFormat) info.GetValue("FieldFormat", typeof(FieldFormat));
            _readData = (byte[]) info.GetValue("ReadData", typeof (byte[]));
        }

        /// <summary>
        /// A message that describes the current exception.
        /// </summary>
        public override string Message
        {
            get
            {
                return string.Format("Error while reading field {0} - read data: {1}",
                    _fieldFormat.Name,
                    _readData);
            }
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
            info.AddValue("FieldFormat",_fieldFormat);
            info.AddValue("ReadData",_readData);
        }
    }
}