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
using System.IO;

namespace Summer.Batch.Extra.Sort.Legacy
{
    /// <summary>
    /// Implementation of <see cref="IRecordAccessorFactory{T}"/> for block readers and writers.
    /// </summary>
    public class BlockAccessorFactory : IRecordAccessorFactory<byte[]>
    {
        /// <summary>
        /// The record length. If not specified, the blocks are assumed to be of variable length.
        /// </summary>
        public int RecordLength { get; set; }

        /// <summary>
        /// Creates a new <see cref="BlockRecordReader"/>.
        /// </summary>
        /// <param name="stream">the stream to read from</param>
        /// <returns>a new <see cref="BlockRecordReader"/></returns>
        public IRecordReader<byte[]> CreateReader(Stream stream)
        {
            return new BlockRecordReader(stream, RecordLength);
        }

        /// <summary>
        /// Creates a new <see cref="BlockRecordWriter"/>.
        /// </summary>
        /// <param name="stream">the stream to write to</param>
        /// <returns>a new <see cref="BlockRecordWriter"/></returns>
        public IRecordWriter<byte[]> CreateWriter(Stream stream)
        {
            return new BlockRecordWriter(stream);
        }
    }
}