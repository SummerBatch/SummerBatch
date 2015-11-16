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
using System.Xml.Serialization;
using Summer.Batch.Common.Util;

namespace Summer.Batch.Extra.Copybook
{
    /// <summary>
    /// Static utility class to load copybooks
    /// </summary>
    public static class CopybookLoader
    {
        /// <summary>
        /// Loads a copybook from its path.
        /// </summary>
        /// <param name="path">the path to the copybook</param>
        /// <returns>the file format read from the copybook</returns>
        public static FileFormat LoadCopybook(string path)
        {
            FileFormat fileFormat;

            using (var reader = File.OpenRead(path))
            {
                fileFormat = LoadCopybook(reader);
            }

            return fileFormat;
        }

        /// <summary>
        /// Loads a copybook from a stream.
        /// </summary>
        /// <param name="stream">the stream to read from</param>
        /// <returns>the file format read from the copybook</returns>
        public static FileFormat LoadCopybook(Stream stream)
        {
            Assert.IsTrue(stream.CanRead, "Cannot read the stream.");
            var serializer = new XmlSerializer(typeof(FileFormat));
            return (FileFormat) serializer.Deserialize(stream);
        }
    }
}
