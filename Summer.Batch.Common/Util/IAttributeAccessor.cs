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

// This file has been modified.
// Original copyright notice :

/*
 * Copyright 2002-2012 the original author or authors.
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
namespace Summer.Batch.Common.Util
{
    /// <summary>
    /// Interface defining a generic contract for attaching and 
    /// accessing metadata to/from arbitrary objects. 
    /// </summary>
    public interface IAttributeAccessor
    {
        /// <summary>
        /// Sets the attribute defined by name to the supplied value. If value is <c>null</c>, the attribute is removed.
        /// In general, users should take care to prevent overlaps with other metadata
        /// attributes by using fully-qualified names, perhaps using class or package names as prefix. 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        void SetAttribute(string name, object val);

        /// <summary>
        /// Gets the value of the attribute identified by name. Returns null if the attribute doesn't exist. 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        object GetAttribute(string name);

        /// <summary>
        /// Removes the attribute identified by name and return its value. Return null if no attribute under name is found. 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        object RemoveAttribute(string name);

        /// <summary>
        /// Returns true if the attribute identified by name exists. Otherwise return false. 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool HasAttribute(string name);

        /// <summary>
        /// Returns the names of all attributes. 
        /// </summary>
        /// <returns></returns>
        string[] AttributeNames();

    }
}
