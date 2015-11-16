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
using System;
using System.Collections.Generic;
using System.Linq;
using Summer.Batch.Common.Collections;

namespace Summer.Batch.Common.Util
{
    /// <summary>
    /// Support class for AttributeAccessors, providing a base implementation of all methods. 
    /// To be extended by subclasses.
    /// </summary>
    [Serializable]
    public abstract class AttributeAccessorSupport : IAttributeAccessor,IDisposable
    {

        private IDictionary<string, object> _attributes = new OrderedDictionary<string, object>();

        /// <summary>
        /// Sets the attribute.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        public void SetAttribute(string name, object val)
        {
            Assert.NotNull(name, "Name must not be null");
            if (val != null)
            {
                _attributes.Add(name, val);
            }
            else
            {
                RemoveAttribute(name);
            }
        }

        /// <summary>
        /// Returns the attribute.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetAttribute(string name)
        {
            Assert.NotNull(name, "Name must not be null");
            Object ret;
            _attributes.TryGetValue(name, out ret);
            return ret;
        }

        /// <summary>
        /// Removes the attribute from the dictionary.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object RemoveAttribute(string name)
        {
            Assert.NotNull(name, "Name must not be null");
            Object ret = null;
            if (_attributes.ContainsKey(name))
            {
                _attributes.TryGetValue(name, out ret);
                _attributes.Remove(name);
            }
            return ret;
        }

        /// <summary>
        /// Tests if this key is present in the attributes dictionary.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasAttribute(string name)
        {
            Assert.NotNull(name, "Name must not be null");
            return _attributes.ContainsKey(name);
        }

        /// <summary>
        /// Dumps keys to an array of string.
        /// </summary>
        /// <returns></returns>
        public string[] AttributeNames()
        {
            return _attributes.Keys.ToArray();
        }

        #region IDisposable
        /// <summary>
        /// see https://msdn.microsoft.com/fr-fr/library/ms244737.aspx
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// see https://msdn.microsoft.com/fr-fr/library/ms244737.aspx
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources                
                _attributes = null;
            }
        } 
        #endregion
    }
}
