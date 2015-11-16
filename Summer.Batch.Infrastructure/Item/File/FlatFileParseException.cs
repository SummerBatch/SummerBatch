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

namespace Summer.Batch.Infrastructure.Item.File
{
    /// <summary>
    /// Exception thrown when errors are encountered while parsing flat files.
    /// </summary>
    [Serializable]
    public class FlatFileParseException : ParseException
    {
        /// <summary>
        /// The original input.
        /// </summary>
        public string Input { get; private set; }

        /// <summary>
        /// The number of the line read when the error occured.
        /// </summary>
        public int LineNumber { get; private set; }

        /// <summary>
        /// Constructs a new <see cref="T:FlatFileParseException"/> with the specified message and input.
        /// </summary>
        /// <param name="message">the detail message</param>
        /// <param name="input">the read input</param>
        public FlatFileParseException(string message, string input) : base(message)
        {
            Input = input;
        }

        /// <summary>
        /// Constructs a new <see cref="T:FlatFileParseException"/> with the specified message, input, and line number.
        /// </summary>
        /// <param name="message">the detail message</param>
        /// <param name="input">the read input</param>
        /// <param name="lineNumber">the number of the read line</param>
        public FlatFileParseException(string message, string input, int lineNumber) : base(message)
        {
            Input = input;
            LineNumber = lineNumber;
        }

        /// <summary>
        /// Constructs a new <see cref="T:FlatFileParseException"/> with the specified message, input, line number, and parent exception.
        /// </summary>
        /// <param name="message">the detail message</param>
        /// <param name="exception">the parent exception</param>
        /// <param name="input">the read input</param>
        /// <param name="lineNumber">the number of the read line</param>
        public FlatFileParseException(string message, Exception exception, string input, int lineNumber) : base(message, exception)
        {
            Input = input;
            LineNumber = lineNumber;
        }

        /// <summary>
        /// Constructor for deserialization.
        /// </summary>
        /// <param name="info">the info holding the serialization data</param>
        /// <param name="context">the serialization context</param>
        public FlatFileParseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Input = info.GetString("Input");
            LineNumber = info.GetInt32("LineNumber");
        }

        #region ISerializable member

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
            info.AddValue("Input", Input);
            info.AddValue("LineNumber", LineNumber);
        }

        #endregion

    }
}