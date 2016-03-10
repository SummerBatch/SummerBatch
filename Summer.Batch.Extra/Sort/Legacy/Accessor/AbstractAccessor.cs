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
using System.Text;

namespace Summer.Batch.Extra.Sort.Legacy.Accessor
{
    /// <summary>
    /// Base class for <see cref="IAccessor{T}"/>
    /// </summary>
    /// <typeparam name="T">&nbsp;</typeparam>
    public abstract class AbstractAccessor<T> : IAccessor<T>
    {
        /// <summary>
        /// The zero-based index of the first character of the accessed column.
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// The length of the column.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// The encoding to use to encode or decode the column.
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected AbstractAccessor()
        {
            Encoding = Encoding.Default;
        }

        /// <summary>
        /// Gets a value from a record.
        /// </summary>
        /// <param name="record">the record to get the value from</param>
        /// <returns>the read value</returns>
        public abstract T Get(byte[] record);

        /// <summary>
        /// Sets a value on a record.
        /// </summary>
        /// <param name="record">the record to set the value on</param>
        /// <param name="value">the value to set</param>
        public abstract void Set(byte[] record, T value);

        /// <summary>
        /// Replaces the bytes from <see cref="Start"/> to <see cref="Start"/> + <see cref="Length"/>
        /// with the specified bytes. If the size of <paramref name="bytes"/> is not equal to <see cref="Length"/>,
        /// it is truncated or padded on the left to adapt its size.
        /// </summary>
        /// <param name="record">the record to modify</param>
        /// <param name="bytes">the new bytes</param>
        /// <param name="paddingValue">the value to use when padding</param>
        public void SetBytes(byte[] record, byte[] bytes, byte paddingValue)
        {
            for (var i = 0; i < Length - bytes.Length; i++)
            {
                record[Start + i] = paddingValue;
            }
            var srcStart = Math.Max(0, bytes.Length - Length);
            var dstStart = Start + Math.Max(0, Length - bytes.Length);
            var size = Math.Min(bytes.Length, Length);
            Buffer.BlockCopy(bytes, srcStart, record, dstStart, size);
        }
    }
}