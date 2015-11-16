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
using System.IO;
using System.Text;

namespace Summer.Batch.Extra.Sort.Legacy
{
    /// <summary>
    /// Implementation of <see cref="IRecordAccessorFactory{T}"/> for records separated by a specific separator.
    /// </summary>
    public class SeparatorAccessorFactory : IRecordAccessorFactory<byte[]>
    {
        /// <summary>
        /// The bytes corresponding to the separator between records. It will not be returned as part of the record.
        /// Default is <see cref="Environment.NewLine"/> encoded using <see cref="Encoding.Default"/>.
        /// </summary>
        public byte[] Separator { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SeparatorAccessorFactory()
        {
            Separator = Encoding.Default.GetBytes(Environment.NewLine);
        }

        /// <summary>
        /// Creates a new <see cref="SeparatorRecordReader"/>.
        /// </summary>
        /// <param name="stream">the stream to read from</param>
        /// <returns>a new <see cref="SeparatorRecordReader"/></returns>
        public IRecordReader<byte[]> CreateReader(Stream stream)
        {
            return new SeparatorRecordReader(stream) { Separator = Separator };
        }

        /// <summary>
        /// Creates a new <see cref="SeparatorRecordWriter"/>.
        /// </summary>
        /// <param name="stream">the stream to write to</param>
        /// <returns>a new <see cref="SeparatorRecordWriter"/></returns>
        public IRecordWriter<byte[]> CreateWriter(Stream stream)
        {
            return new SeparatorRecordWriter(stream) { Separator = Separator };
        }
    }
}