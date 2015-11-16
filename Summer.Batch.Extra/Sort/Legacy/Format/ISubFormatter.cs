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
namespace Summer.Batch.Extra.Sort.Legacy.Format
{
    /// <summary>
    /// Interface used by <see cref="LegacyFormatter"/> to format a sub-part of the output record.
    /// </summary>
    public interface ISubFormatter
    {
        /// <summary>
        /// The length of the formatted sub-part
        /// </summary>
        int Length { get; }

        /// <summary>
        /// The zero-based index of the first byte to write in the output record.
        /// </summary>
        int OutputIndex { get; }

        /// <summary>
        /// Formats a sub-part of the output record.
        /// </summary>
        /// <param name="input">the input record</param>
        /// <param name="output">the output record</param>
        void Format(byte[] input, byte[] output);
    }
}