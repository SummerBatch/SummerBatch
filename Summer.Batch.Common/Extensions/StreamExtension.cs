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

namespace Summer.Batch.Common.Extensions
{
    /// <summary>
    /// Extension class for <see cref="System.IO.Stream"/>.
    /// </summary>
    public static class StreamExtension
    {
        /// <summary>
        /// Reads a stream into a byte array with default values (0, byte array length).
        /// </summary>
        /// <param name="stream">The stream to read the bytes from.</param>
        /// <param name="data">The array into which the bytes are copied.</param>
        /// <returns></returns>
        public static int Read(this Stream stream , byte[] data)
        {
            return stream.Read(data, 0, data.Length);
        }

        /// <summary>
        /// Writes a byte array into a stream using default values (0, byte array length).
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="data">The array containing the bytes to write.</param>
        public static void Write(this Stream stream, byte[] data)
        {
            stream.Write(data, 0, data.Length);
        } 
    }
}