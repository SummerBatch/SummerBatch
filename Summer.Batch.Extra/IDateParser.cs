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

namespace Summer.Batch.Extra
{
    /// <summary>
    /// Interface for date parsers.
    /// </summary>
    public interface IDateParser
    {

        /// <summary>
        /// Date parsers can decode a decimal date.
        /// </summary>
        /// <param name="bdDate">decimal to decode</param>
        /// <returns>the decoded date</returns>
        DateTime? Decode(decimal bdDate);

        /// <summary>
        /// Date parsers can decode a String date.
        /// </summary>
        /// <param name="sDate">the string to decode</param>
        /// <returns>the decoded date</returns>
        DateTime? Decode(string sDate);

        /// <summary>
        /// Date parsers can encode a Date to String.
        /// </summary>
        /// <param name="date">the date to encode</param>
        /// <returns>the encoded string</returns>
        string EncodeString(DateTime? date);

        /// <summary>
        /// Date parsers can encode a Date to Decimal.
        /// </summary>
        /// <param name="date">the date to encode</param>
        /// <returns>the encodedDecimal</returns>
        decimal EncodeDecimal(DateTime? date);
    }
}