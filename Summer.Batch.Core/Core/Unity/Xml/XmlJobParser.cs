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

namespace Summer.Batch.Core.Unity.Xml
{
    /// <summary>
    /// Parser for the  batch xml configuration.
    /// </summary>
    public static class XmlJobParser
    {
        /// <summary>
        /// Load configuration from file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static XmlJob LoadJob(string path)
        {
            XmlJob job;

            using (StreamReader reader = new StreamReader(path))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(XmlJob));
                job = (XmlJob)serializer.Deserialize(reader);
            }

            return job;
        }
    }
}