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

namespace Summer.Batch.Extra.Utils
{
    /// <summary>
    /// Dictionary helper.
    /// </summary>
    public static class MapUtils
    {
        /// <summary>
        /// Test if dictionary contains key
        /// </summary>
        /// <typeparam name="TK">the key type</typeparam>
        /// <typeparam name="TV">the value type</typeparam>
        /// <param name="dictionary">the dictionary</param>
        /// <param name="key"></param>
        /// <returns>true if this dictionary contains a value for the specified key.</returns>
        public static bool ContainsKey<TK, TV>(IDictionary<TK, TV> dictionary, TK key) 
            where TK:class 
        {
            TV value;
            return key == null ? false : dictionary.TryGetValue(key, out value);
        }

        /// <summary>
        ///  Test if dictionary contains value.
        ///  </summary>
        /// <typeparam name="TK">the key type</typeparam> 
        /// <typeparam name="TV">the value type</typeparam> 
        /// <param name="dictionary">the dictionary</param>
        /// <param name="value">the value</param>
        /// <returns>true if this dictionary maps one or more keys to the specified value.</returns> 
        public static bool ContainsValue<TK, TV>(IDictionary<TK, TV> dictionary, TV value)
            where TK : class
        {
            return dictionary.Values.Contains(value);
        }

        ///<summary>
        /// Remove entry for given key.
        /// </summary>
        /// <typeparam name="TK">the key type</typeparam> 
        /// <typeparam name="TV">the value type</typeparam> 
        ///<param name="dictionary">the dictionary</param> 
        ///<param name="key">the key</param>  
        ///<returns>the previous value associated with key, or the default value if there was no dictionaryping for key.</returns> 
        public static TV Remove<TK, TV>(IDictionary<TK, TV> dictionary, TK key)
            where TK : class
        {
            if (key == null)
            {
                return default(TV);
            }
            else
            {
                TV value;
                dictionary.TryGetValue(key, out value);
                dictionary.Remove(key);
                return value;
            }
        }
    
        ///<summary>
        ///Associates the specified value with the specified key in the specified dictionary.
        ///</summary>
        /// <typeparam name="TK">the key type</typeparam> 
        /// <typeparam name="TV">the value type</typeparam> 
        ///<param name="dictionary">the dictionary where to store the value</param>
        ///<param name="key">the key to associate the value to</param>
        ///<param name="value">the value to put in the dictionary</param>
        ///<returns>the previous value associated to the key, or the default value if there was none</returns> 
        public static TV Put<TK, TV>(IDictionary<TK, TV> dictionary, TK key, TV value)
            where TK : class
        {
            TV result;
            if (!dictionary.TryGetValue(key, out result))
            {
                dictionary.Add(key,value);
            }
            else
            {
                dictionary[key] = value;
            }
            return result;
        }
    
        ///<summary>
        ///Copies all of the mappings from dictionary1 to dictionary2.
        ///</summary>
        /// <typeparam name="TK">the key type</typeparam> 
        /// <typeparam name="TV">the value type</typeparam>
        ///<param name="dictionary1">the dictionary1</param>
        ///<param name="dictionary2">the dictionary2</param>
        public static void PutAll<TK,TV>(IDictionary<TK, TV> dictionary1, IDictionary<TK, TV> dictionary2)
            where TK : class
        {
            foreach (var item in dictionary2){
                Put(dictionary1, item.Key, item.Value);
            }
        }
    
        ///<summary>
        ///Returns a Set view of the keys contained in this dictionary.
        ///</summary>
        /// <typeparam name="TK">the key type</typeparam> 
        /// <typeparam name="TV">the value type</typeparam> 
        ///<param name="dictionary">the dictionary</param>  
        ///<returns>a set view of the keys contained in this dictionary.</returns>
        public static ISet<TK> KeySet<TK, TV>(IDictionary<TK, TV> dictionary)
            where TK : class
        {
            return new HashSet<TK>(dictionary.Keys);
        }
    
        ///<summary>
        ///Returns the value to which the specified key is dictionaryped, 
        ///or null if this dictionary contains no dictionaryping for the key.
        ///</summary>
        /// <typeparam name="TK">the key type</typeparam> 
        /// <typeparam name="TV">the value type</typeparam>  
        ///<param name="key">the key to get the value from</param>
        ///<param name="dictionary">the dictionary</param>  
        ///<returns>the value dictionaryped with the key.</returns> 
        public static TV Get<TK, TV>(IDictionary<TK,TV> dictionary, TK key)
            where TK : class
        {
            if (key == null)
            {
                return default(TV);
            }
            else
            {
                TV result;
                dictionary.TryGetValue(key, out result);
                return result;
            }
        }

        /// <summary>
        /// Returns a Collection view of the values contained in this dictionary.
        /// </summary>
        ///  <typeparam name="TK">the key type</typeparam> 
        ///  <typeparam name="TV">the value type</typeparam>
        /// <param name="dictionary">the dictionary</param>
        /// <returns>the value dictionaryped with the key</returns> 
        public static ICollection<TV> Values<TK,TV>(IDictionary<TK, TV> dictionary)
            where TK : class
        {
            return dictionary.Values;
        }
    }
}
