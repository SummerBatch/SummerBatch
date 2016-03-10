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


using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Summer.Batch.Infrastructure.Item
{
    /// <summary>
    /// Object representing a context for an IItemStream. It is a thin wrapper
    /// for a dictionary that allows optionally for type safety on reads. It also allows for
    /// dirty checking by setting a 'dirty' flag whenever any put is called.
    ///
    /// Note that putting <c>null</c> value is equivalent to removing the entry
    /// for the given key.
    /// </summary>
    [Serializable]
    public class ExecutionContext : IEnumerable<KeyValuePair<string, object>>
    {
        private volatile bool _dirty;
        /// <summary>
        /// Dirty property.
        /// </summary>
        public bool Dirty { get { return _dirty; } }

        /// <summary>
        /// Inner storage.
        /// </summary>
        private readonly IDictionary<string, Object> _map;

        /// <summary>
        /// Inner storage as array.
        /// </summary>
        public KeyValuePair<string, Object>[] EntrySet
        {
            get { return _map.ToArray(); }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ExecutionContext()
        {
            _map = new ConcurrentDictionary<string, Object>();
        }


        /// <summary>
        /// Empty the context. Used by ContextManager.
        /// </summary>
        public void Empty()
        {
            _map.Clear();
        }

        /// <summary>
        /// Custom constructor
        /// </summary>
        /// <param name="map"></param>
        public ExecutionContext(IDictionary<string, Object> map)
        {
            _map = new ConcurrentDictionary<string, Object>(map);
        }

        /// <summary>
        /// Custom constructor with given map
        /// </summary>
        /// <param name="executionContext"></param>
        public ExecutionContext(ExecutionContext executionContext)
            : this()
        {

            if (executionContext == null)
            {
                return;
            }

            foreach (KeyValuePair<string, Object> entry in executionContext._map)
            {
                _map.Add(entry);
            }
        }

        /// <summary>
        /// Store a string value in context.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void PutString(string key, string value)
        {
            Put(key, value);
        }

        /// <summary>
        /// Store a long value in context.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void PutLong(string key, long value)
        {
            Put(key, value);
        }

        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void PutInt(string key, int value)
        {
            Put(key, value);
        }

        /// <summary>
        /// Store a double value in context.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void PutDouble(string key, double value)
        {

            Put(key, value);
        }

        /// <summary>
        ///  Add an Object value to the context. Putting null
        /// value for a given key removes the key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Put(string key, Object value)
        {
            if (value != null)
            {
                _map[key] = value;
                Object result;
                _map.TryGetValue(key, out result);
                _dirty = result == null || !result.Equals(value);
            }
            else
            {
                bool removed = _map.Remove(key);
                _dirty = removed;
            }
        }

        /// <summary>
        /// Typesafe getter
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetString(string key)
        {

            return (string)ReadAndValidate(key, typeof(string));
        }

        /// <summary>
        /// Typesafe getter with default value if key is not represented
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultString"></param>
        /// <returns></returns>
        public string GetString(string key, string defaultString)
        {
            if (!_map.ContainsKey(key))
            {
                return defaultString;
            }

            return (string)ReadAndValidate(key, typeof(string));
        }

        /// <summary>
        /// Typesafe getter
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long GetLong(string key)
        {
            return (long)ReadAndValidate(key, typeof(long));
        }


        /// <summary>
        /// Typesafe getter with default value if key is not represented
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultLong"></param>
        /// <returns></returns>
        public long GetLong(string key, long defaultLong)
        {
            if (!_map.ContainsKey(key))
            {
                return defaultLong;
            }

            return (long)ReadAndValidate(key, typeof(long));
        }

        /// <summary>
        /// Typesafe getter
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetInt(string key)
        {

            return (int)ReadAndValidate(key, typeof(int));
        }

        /// <summary>
        /// Typesafe getter with default value if key is not represented
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultInt"></param>
        /// <returns></returns>
        public int GetInt(string key, int defaultInt)
        {
            if (!_map.ContainsKey(key))
            {
                return defaultInt;
            }

            return (int)ReadAndValidate(key, typeof(int));
        }

        /// <summary>
        /// Typesafe getter
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public double GetDouble(string key)
        {
            return (double)ReadAndValidate(key, typeof(double));
        }

        /// <summary>
        /// Typesafe getter with default value if key is not represented
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultDouble"></param>
        /// <returns></returns>
        public double GetDouble(string key, double defaultDouble)
        {
            if (!_map.ContainsKey(key))
            {
                return defaultDouble;
            }

            return (double)ReadAndValidate(key, typeof(double));
        }

        /// <summary>
        /// Getter for the value represented by the provided key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Object Get(string key)
        {
            Object returned;
            _map.TryGetValue(key, out returned);
            return returned;
        }

        /// <summary>
        /// Read the value for the given key and checks that it is of the given type.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private Object ReadAndValidate(string key, Type type)
        {

            Object val;
            _map.TryGetValue(key, out val);

            if (!type.IsInstanceOfType(val))
            {
                throw new InvalidCastException("Value for key=[" + key + "] is not of type: [" +
                        type + "], it is ["
                        + (val == null ? null : "(" + val.GetType() + ")" + val) + "]");
            }

            return val;
        }

        /// <summary>
        /// Checks if inner storage is empty.
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return _map.Count == 0;
        }


        /// <summary>
        /// Clears the dirty flag. 
        /// </summary>
        public void ClearDirtyFlag()
        {
            _dirty = false;
        }

        /// <summary>
        /// Checks if inner storage contains the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            return _map.ContainsKey(key);
        }

        /// <summary>
        /// Remove item referenced by given key from inner storage.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Object Remove(string key)
        {
            Object removed = _map.TryGetValue(key, out removed);
            bool got = _map.Remove(key);
            if (got) { return removed; } else { return null; }
        }

        /// <summary>
        /// Checks if inner storage contains the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ContainsValue(Object value)
        {
            return _map.Values.Contains(value);
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(Object obj)
        {
            if (!(obj is ExecutionContext))
            {
                return false;
            }
            if (this == obj)
            {
                return true;
            }
            ExecutionContext rhs = (ExecutionContext)obj;
            return _map.Count == rhs._map.Count && !_map.Except(rhs._map).Any();
        }

        /// <summary>
        /// GetHashCode override
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return _map.Sum(pair => pair.GetHashCode());
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _map.ToString();
        }

        /// <summary>
        /// Returns the size of the inner storage.
        /// </summary>
        /// <returns></returns>
        public int Size()
        {
            return _map.Count;
        }

        #region IEnumerable members

        /// <summary>
        /// @see IEnumerator#GetEnumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _map.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _map.GetEnumerator();
        }

        #endregion
    }
}
