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

namespace Summer.Batch.Extra.Ebcdic.Exception
{
    /// <summary>
    /// Exception thrown by EbcdicReader to signal that the end of file has been prematurely encountered.
    /// </summary>
    [Serializable]
    public class EndOfFileException : EbcdicException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public EndOfFileException()
            : base(String.Empty)
        {
        }

        /// <summary>
        /// Custom constructor using a message
        /// </summary>
        /// <param name="message"></param>
        public EndOfFileException(string message) : base(message)
        {
        }

        /// <summary>
        /// Custom constructor using a message and an inner exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cause"></param>
        public EndOfFileException(string message, System.Exception cause) : base(message, cause)
        {
        }
    }
}