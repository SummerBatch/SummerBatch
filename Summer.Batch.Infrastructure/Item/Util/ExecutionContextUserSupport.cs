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

// This file has been modified.
// Original copyright notice :

/*
 * Copyright 2006-2013 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using Summer.Batch.Common.Util;

namespace Summer.Batch.Infrastructure.Item.Util
{
    /// <summary>
    /// Facilitates data persistence in the execution context, with keys based on a name.
    /// </summary>
    public class ExecutionContextUserSupport
    {
        /// <summary>
        /// Name used as a prefix in keys to identify this instance entries in the execution context.
        /// </summary>
        public string Name { protected get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ExecutionContextUserSupport()
        {
        }

        /// <summary>
        /// Custom constructor using a name.
        /// </summary>
        /// <param name="name"></param>
        public ExecutionContextUserSupport(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Transform the given key to use the name of this instance as prefix.
        /// </summary>
        /// <param name="key">a key to transform</param>
        /// <returns>the given key with an identifying prefix</returns>
        public string GetKey(string key)
        {
            Assert.HasText(Name, "Name must be assigned to the sake of defining the execution context keys prefix.");
            return Name + "." + key;
        }

    }
}