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
using System.Numerics;

namespace Summer.Batch.Extra.Sort.Legacy.Accessor
{
    /// <summary>
    /// Implementation of <see cref="IAccessor{T}"/> for binary encoded numbers.
    /// </summary>
    public class BinaryAccessor : AbstractAccessor<decimal>
    {
        /// <summary>
        /// Whether the number is signed.
        /// </summary>
        public bool Signed { get; set; }

        /// <summary>
        /// Gets a value from a record.
        /// </summary>
        /// <param name="record">the record to get the value from</param>
        /// <returns>the read value</returns>
        public override decimal Get(byte[] record)
        {
            var bytes = record.SubArray(Start, Length);
            Array.Reverse(bytes);
            if (!Signed)
            {
                // Add an empty byte at the end so that it is interpreted as a positive integer
                Array.Resize(ref bytes, bytes.Length + 1);
            }
            return (decimal)new BigInteger(bytes);
        }

        /// <summary>
        /// Sets a value on a record.
        /// </summary>
        /// <param name="record">the record to set the value on</param>
        /// <param name="value">the value to set</param>
        public override void Set(byte[] record, decimal value)
        {
            var bytes = ((BigInteger) value).ToByteArray();
            Array.Reverse(bytes);
            SetBytes(record, bytes, 0);
        }
    }
}