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

//   This file has been modified.
//   Original copyright notice :

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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Summer.Batch.Common.Collections;

namespace Summer.Batch.Core
{
    /// <summary>
    /// Value object representing runtime parameters to a batch job. Because the
    /// parameters have no individual meaning outside of the JobParameters they are
    /// contained within, it is a value object rather than an entity. It is also
    /// extremely important that a parameters object can be reliably compared to
    /// another for equality, in order to determine if one JobParameters object
    /// equals another. Furthermore, because these parameters will need to be
    /// persisted, it is vital that the types added are restricted. 
    /// This class is immutable and therefore thread-safe.
    /// </summary>
    [Serializable]
    public sealed class JobParameters : IEnumerable<KeyValuePair<string, JobParameter>>
    {
        /// <summary>
        /// Parameters.
        /// </summary>
        private readonly IDictionary<string, JobParameter> _parameters;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public JobParameters()
        {
            _parameters = new OrderedDictionary<string, JobParameter>(16);
        }

        /// <summary>
        /// Alternative constructor using provided parameters.
        /// </summary>
        /// <param name="parameters"></param>
        public JobParameters(IDictionary<string, JobParameter> parameters)
        {
            _parameters = new OrderedDictionary<string, JobParameter>(parameters.Count);
            foreach (KeyValuePair<string,JobParameter> entry in parameters)
            {
                _parameters.Add(entry.Key,entry.Value);
            }
        }
        
        /// <summary>
        /// Typesafe Getter for the Long represented by the provided key.
        /// </summary>
        /// <param name="key">The key to get a value for</param>
        /// <returns>the long value</returns>
        public long GetLong(string key)
        {
            if (!_parameters.ContainsKey(key))
            {
                return 0L;
            }
            JobParameter val;
            var got = _parameters.TryGetValue(key, out val);
            return got && val != null ? (long)val.Value : 0L;
        }

        /// <summary>
        /// Typesafe Getter for the Long represented by the provided key.  If the
        /// key does not exist, the default value will be returned.
        /// </summary>
        /// <param name="key">key to return the value for</param>
        /// <param name="defaultValue">defaultValue to return if the value doesn't exist</param>
        /// <returns>the parameter represented by the provided key, defaultValue otherwise</returns>
        public long GetLong(string key, long defaultValue)
        {
            if (_parameters.ContainsKey(key))
            {
                return GetLong(key);
            }
            return defaultValue;

        }
       
        /// <summary>
        /// Typesafe Getter for the String represented by the provided key.
        /// </summary>
        /// <param name="key">The key to get a value for</param>
        /// <returns>the string value</returns>
        public string GetString(string key)
        {
            JobParameter val;
            var got = _parameters.TryGetValue(key, out val);
            return got && val != null ? (string) val.Value : null;
        }

        /// <summary>
        /// Typesafe Getter for the String represented by the provided key.  If the
        /// key does not exist, the default value will be returned.
        /// </summary>
        /// <param name="key"> key to return the value for</param>
        /// <param name="defaultValue">defaultValue to return if the value doesn't exist</param>
        /// <returns>the parameter represented by the provided key, defaultValue otherwise</returns>
        public string GetString(string key, string defaultValue)
        {
            if (_parameters.ContainsKey(key))
            {
                return GetString(key);
            }
            return defaultValue;

        }

        /// <summary>
        /// Typesafe Getter for the Long represented by the provided key.
        /// </summary>
        /// <param name="key">The key to get a value for</param>
        /// <returns>the double value</returns>
        public double GetDouble(string key)
        {
            if (!_parameters.ContainsKey(key))
            {
                return 0.0;
            }
            JobParameter val;
            var got = _parameters.TryGetValue(key, out val);
            return got && val != null ? (double)val.Value : 0.0;
        }

        /// <summary>
        /// Typesafe Getter for the Double represented by the provided key.  If the
        /// key does not exist, the default value will be returned.
        /// </summary>
        /// <param name="key">key to return the value for</param>
        /// <param name="defaultValue">defaultValue to return if the value doesn't exist</param>
        /// <returns>the parameter represented by the provided key, defaultValue otherwise</returns>
        public double GetDouble(string key, double defaultValue)
        {
            if (_parameters.ContainsKey(key))
            {
                return GetDouble(key);
            }
            return defaultValue;

        }

        /// <summary>
        /// Typesafe Getter for the Date represented by the provided key.
        /// </summary>
        /// <param name="key">key The key to get a value for</param>
        /// <returns>the datetime value</returns>
        public DateTime? GetDate(string key)
        {
            return GetDate(key, null);
        }

        /// <summary>
        /// Typesafe Getter for the Date represented by the provided key.  If the
        /// key does not exist, the default value will be returned.
        /// </summary>
        /// <param name="key">key to return the value for</param>
        /// <param name="defaultValue">defaultValue to return if the value doesn't exist</param>
        /// <returns>the parameter represented by the provided key, defaultValue otherwise.</returns>
        public DateTime? GetDate(string key, DateTime? defaultValue)
        {
            if (_parameters.ContainsKey(key))
            {
                JobParameter val;
                var got = _parameters.TryGetValue(key, out val);
                if (got)
                {
                    return (DateTime)val.Value;
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// Get a dictionary of all parameters, including string, long, and date.
        /// </summary>
        /// <returns>an unmodifiable dictionary containing all parameters.</returns>
        public IDictionary<string, JobParameter> GetParameters()
        {
            IDictionary<string, JobParameter> copy = new OrderedDictionary<string, JobParameter>(_parameters.Count);
            foreach (var entry in _parameters)
            {
                copy.Add(entry.Key,entry.Value);
            }
            return copy;
        }

        /// <summary>
        /// Test on parameters emptiness.
        /// </summary>
        /// <returns>true if the parameters is empty, false otherwise</returns>
        public bool IsEmpty()
        {
            return !_parameters.Any();
        }

        /// <summary>
        /// Equals override.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is JobParameters))
            {
                return false;
            }
            if (obj == this)
            {
                return true;
            }
            JobParameters rhs = (JobParameters)obj;
            return DictionaryUtils<string,JobParameter>.AreEqual(_parameters, rhs._parameters);
        }

        /// <summary>
        /// GetHashCode override.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return 17 + 23*_parameters.GetHashCode();
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Join(",", _parameters);
        }

        /// <summary>
        /// Convert to properties.
        /// </summary>
        /// <returns></returns>
        public NameValueCollection ToProperties()
        {
            NameValueCollection props = new NameValueCollection();
            foreach (KeyValuePair<string, JobParameter> param in _parameters)
            {
                if (param.Value != null)
                {
                    props.Add(param.Key, param.Value.ToString());
                }
            }
            return props;
        }

        #region IEnumerate members

        /// <summary>
        /// Return Enumerator over parameters.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, JobParameter>> GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }

        /// <summary>
        /// Return Enumerator over parameters.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }
        #endregion
   }
}
