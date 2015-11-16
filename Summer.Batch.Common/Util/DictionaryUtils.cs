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

namespace Summer.Batch.Common.Util
{
    /// <summary>
    /// Dictionary Helper.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public static class DictionaryUtils<TKey,TValue>
    {
        /// <summary>
        /// Test for equality between two dictionaries, by comparing their inner contents.
        /// </summary>
        /// <param name="dictionary1"></param>
        /// <param name="dictionary2"></param>
        /// <returns></returns>
        public static bool AreEqual(IDictionary<TKey, TValue> dictionary1, IDictionary<TKey, TValue> dictionary2)
        {
            if (dictionary1.Count != dictionary2.Count)
            {
                return false;
            }
            foreach (KeyValuePair<TKey, TValue> pair in dictionary1)
            {
                if (!dictionary2.ContainsKey(pair.Key))
                {
                    return false;
                }
                if (!pair.Value.Equals(dictionary2[pair.Key]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}