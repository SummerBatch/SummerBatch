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
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Summer.Batch.Common.Util
{
    /// <summary>
    /// Serialization helper.
    /// </summary>
    public static class SerializationUtils
    {
        /// <summary>
        /// Serializes an object to a byte array.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>A byte array representing the <paramref name="obj"/>.</returns>
        public static byte[] Serialize(this object obj)
        {
            using (var stream = new MemoryStream())
            {
                var serializer = new BinaryFormatter();
                serializer.Serialize(stream, obj);
                return stream.GetBuffer();
            }
        }

        /// <summary>
        /// Deserializes a byte array to an object.
        /// </summary>
        /// <typeparam name="T">&nbsp;The type of the object to deserialize to.</typeparam>
        /// <param name="bytes">The byte array to deserialize.</param>
        /// <returns>The deserialized object.</returns>
        public static T Deserialize<T>(this byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                var serializer = new BinaryFormatter();
                return (T) serializer.Deserialize(stream);
            }
        }
    }
}
