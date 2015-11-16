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
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Summer.Batch.Extra.Copybook
{
    /// <summary>
    /// Xml representation of RecordFormat.
    /// </summary>
    public class RecordFormat : IFieldsList
    {
        /// <summary>
        /// List of copybook elements.
        /// </summary>
        [XmlElement("FieldFormat", typeof(FieldFormat))]
        [XmlElement("FieldsGroup", typeof(FieldsGroup))]
        public List<CopybookElement> Elements { get; set; }

        /// <summary>
        /// distinguishFieldValue attribute.
        /// </summary>
        [XmlAttribute("distinguishFieldValue")]
        public string DiscriminatorPattern { get; set; }

        /// <summary>
        /// cobolRecordName attribute.
        /// </summary>
        [XmlAttribute("cobolRecordName")]
        public string CobolRecordName { get; set; }
    }
}
