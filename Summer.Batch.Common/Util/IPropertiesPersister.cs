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
using System.Collections.Specialized;
using System.IO;

namespace Summer.Batch.Common.Util
{
    /// <summary>
    /// Strategy interface for persisting a NameValueCollection,
    /// allowing for pluggable parsing strategies.
    /// 
    /// The default implementation is DefaultPropertiesPersister,
    /// providing the standard parsing of NameValueCollection,
    /// but allowing for reading from any Reader and writing to any Writer
    /// (which allows to specify an encoding for a properties file).
    /// </summary>
    public interface IPropertiesPersister
    {
        /// <summary>
        /// Load properties from a text reader.
        /// </summary>
        /// <param name="properties">The <see cref="NameValueCollection"/> where to store the properties.</param>
        /// <param name="reader">The <see cref="TextReader"/> to read the properties from.</param>
        /// <exception cref="IOException">in case of I/O errors</exception>
        void Load(NameValueCollection properties, TextReader reader);

        /// <summary>
        /// Writes properties to a text writer.
        /// </summary>
        /// <param name="properties">The properties to write.</param>
        /// <param name="writer">The <see cref="TextWriter"/> to write the properties to.</param>
        /// <exception cref="IOException">in case of I/O errors</exception>
        void Store(NameValueCollection properties, TextWriter writer);
    }
}
