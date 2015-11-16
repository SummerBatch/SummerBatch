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

namespace Summer.Batch.Extra.Sort
{
    /// <summary>
    /// Factory interface for creating instances of <see cref="IRecordReader{T}"/> and <see cref="IRecordWriter{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRecordAccessorFactory<T>
    {
        /// <summary>
        /// Creates a new <see cref="IRecordReader{T}"/>.
        /// </summary>
        /// <param name="stream">the stream to read from</param>
        /// <returns>a new <see cref="IRecordReader{T}"/></returns>
        IRecordReader<T> CreateReader(Stream stream);

        /// <summary>
        /// Creates a new <see cref="IRecordWriter{T}"/>.
        /// </summary>
        /// <param name="stream">the stream to write to</param>
        /// <returns>a new <see cref="IRecordWriter{T}"/></returns>
        IRecordWriter<T> CreateWriter(Stream stream);
    }
}