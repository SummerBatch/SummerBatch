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

namespace Summer.Batch.Extra.Sort.Legacy.Parser
{
    /// <summary>
    /// Exception thrown when an error occurs while parsing a configuration card.
    /// </summary>
    [Serializable]
    public class ParsingException : SortException
    {
        /// <summary>
        /// The parsed string.
        /// </summary>
        public string ParsedString { get; private set; }

        /// <summary>
        /// The index in the parsed string where the error occured.
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Constructs a new <see cref="ParsingException"/> with a message but no parsed string or index.
        /// </summary>
        /// <param name="message">the error message</param>
        public ParsingException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructor for deserialization.
        /// </summary>
        /// <param name="info">the info holding the serialization data</param>
        /// <param name="context">the serialization context</param>
        protected ParsingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ParsedString = info.GetString("ParsedString");
            Index = info.GetInt32("Index");
        }

        /// <summary>
        /// Constructs a new <see cref="ParsingException"/> with a message, a parsed string, and an index.
        /// </summary>
        /// <param name="message">the error message</param>
        /// <param name="parsedString">the parsed string</param>
        /// <param name="index">an index in the parsed string</param>
        public ParsingException(string message, string parsedString, int index) : base(message)
        {
            ParsedString = parsedString;
            Index = index;
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
            info.AddValue("ParsedString", ParsedString);
            info.AddValue("Index", Index);
        }
    }
}