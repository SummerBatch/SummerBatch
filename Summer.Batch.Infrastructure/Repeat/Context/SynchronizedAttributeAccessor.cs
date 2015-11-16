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
using System;
using System.Text;

namespace Summer.Batch.Infrastructure.Repeat.Context
{
    /// <summary>
    /// An AttributeAccessor that synchronizes on a mutex (not this) before 
    /// modifying or accessing the underlying attributes. 
    /// </summary>
    public class SynchronizedAttributeAccessor : IAttributeAccessor, IDisposable
    {
        private class MyAnonymousAttributeAccessorSupport : AttributeAccessorSupport { }

        readonly AttributeAccessorSupport _support = new MyAnonymousAttributeAccessorSupport();

        /// <summary>
        /// @see IAttributeAccessor#SetAttribute .
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        public void SetAttribute(string name, object val)
        {
            lock (_support)
            {
                _support.SetAttribute(name,val);
            }
        }

        /// <summary>
        /// @see IAttributeAccessor#GetAttribute .
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetAttribute(string name)
        {
            lock (_support)
            {
                return _support.GetAttribute(name);
            }
        }

        /// <summary>
        /// @see IAttributeAccessor#RemoveAttribute .
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual object RemoveAttribute(string name)
        {
            lock (_support)
            {
                return _support.RemoveAttribute(name);
            }
        }

        /// <summary>
        /// @see IAttributeAccessor#HasAttribute .
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasAttribute(string name)
        {
            lock (_support)
            {
                return _support.HasAttribute(name);
            }
        }

        /// <summary>
        /// @see IAttributeAccessor#AttributeNames .
        /// </summary>
        /// <returns></returns>
        public string[] AttributeNames()
        {
            lock (_support)
            {
                return _support.AttributeNames();
            }
        }

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder buffer = new StringBuilder("SynchronizedAttributeAccessor: [");
            lock (_support)
            {
                string[] names = _support.AttributeNames();
                int i = 0;
                foreach (var name in names)
                {
                    buffer.Append(name).Append("=").Append(_support.GetAttribute(name));
                    if (i < names.Length - 1)
                    {
                        buffer.Append(", ");
                    }
                    i++;
                }
                buffer.Append("]");
                return buffer.ToString();
            }
        }

        /// <summary>
        /// Additional support for atomic put if absent. 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns>null if the attribute was not already set, the existing value otherwise.</returns>
        public object SetAttributeIfAbsent(string name, object val)
        {
            lock (_support)
            {
                Object old = _support.GetAttribute(name);
                if (old != null)
                {
                    return old;
                }
                _support.SetAttribute(name, val);
            }
            return null;
        }

        /// <summary>
        /// GetHashCode override.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return _support.GetHashCode();
        }

        /// <summary>
        /// Equals override.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(object other)
        {
            if (this == other)
            {
                return true;
            }
            AttributeAccessorSupport that;
            var accessor = other as SynchronizedAttributeAccessor;
            if (accessor != null)
            {
                that = accessor._support;
            }
            else
            {
                var support = other as AttributeAccessorSupport;
                if (support != null)
                {
                    that = support;
                }
                else
                {
                    return false;
                }
            }
            lock (_support)
            {
                return _support.Equals(that);
            }
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
            if (disposing && _support != null)
            {
                // free managed resources               
                _support.Dispose();
               
            }
        } 
        #endregion
    }
}
