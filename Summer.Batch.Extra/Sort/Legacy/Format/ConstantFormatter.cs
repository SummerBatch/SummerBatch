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

namespace Summer.Batch.Extra.Sort.Legacy.Format
{
    /// <summary>
    /// Implementation of <see cref="ISubFormatter"/> that writes a constant.
    /// </summary>
    public class ConstantFormatter : ISubFormatter
    {
        /// <summary>
        /// The constant to write.
        /// </summary>
        public byte[] Constant { get; set; }

        /// <summary>
        /// The length of the constant.
        /// </summary>
        public int Length { get { return Constant.Length; } }

        /// <summary>
        /// The zero-based index of the first byte to write in the output record.
        /// </summary>
        public int OutputIndex { get; set; }

        /// <summary>
        /// Writes the constant in the output record.
        /// </summary>
        /// <param name="input">the input record</param>
        /// <param name="output">the output record</param>
        public void Format(byte[] input, byte[] output)
        {
            Buffer.BlockCopy(Constant, 0, output, OutputIndex, Length);
        }
    }
}