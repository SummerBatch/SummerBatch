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

namespace Summer.Batch.Core.Unity.Xml
{
    /// <summary>
    /// Xml representation of job.
    /// </summary>
    [XmlRoot(ElementName = "job", Namespace = "http://www.summerbatch.com/xmlns")]
    public class XmlJob : IXmlStepContainer
    {
        /// <summary>
        /// id attribute.
        /// </summary>
        [XmlAttribute("id")]
        public string Id { get; set; }
       
        /// <summary>
        /// restartable attribute
        /// </summary>
        [XmlAttribute("restartable")]
        public string Restartable { get; set; }
        
        /// <summary>
        /// listeners
        /// </summary>
        [XmlElement("listeners")]
        public XmlListeners Listeners { get; set; }
        
        /// <summary>
        /// Job elements (flow, split,step)
        /// </summary>
        [XmlElement("flow", typeof(XmlFlow))]
        [XmlElement("split", typeof(XmlSplit))]
        [XmlElement("step", typeof(XmlStep))]
        public List<XmlJobElement> JobElements { get; set; }
    }
}