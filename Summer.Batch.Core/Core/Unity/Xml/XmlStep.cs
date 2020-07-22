﻿//
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
using System.Xml.Serialization;

namespace Summer.Batch.Core.Unity.Xml
{
    /// <summary>
    /// Xml representation of step.
    /// </summary>
    public class XmlStep : XmlJobElement
    {
        /// <summary>
        /// listeners.
        /// </summary>
        [XmlElement("listeners")]
        public XmlListeners Listeners { get; set; }
        
        /// <summary>
        /// batchlet.
        /// </summary>
        [XmlElement("batchlet")]
        public XmlBatchlet Batchlet { get; set; }
        
        /// <summary>
        /// chunk.
        /// </summary>
        [XmlElement("chunk")]
        public XmlChunk Chunk { get; set; }
        
        /// <summary>
        /// partition.
        /// </summary>
        [XmlElement("partition")]
        public XmlPartition Partition { get; set; }

        /// <summary>
        /// configuration for adding delay at the end of each chunk
        /// </summary>
        [XmlAttribute("delay-config")]
        public string DelayConfig { get; set; } = "10";
    }
}