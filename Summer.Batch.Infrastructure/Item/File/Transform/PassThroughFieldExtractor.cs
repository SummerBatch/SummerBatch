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
using System.Collections.Generic;
using System.Linq;

namespace Summer.Batch.Infrastructure.Item.File.Transform
{
    /// <summary>
    /// An implementation of <see cref="T:IFieldExtractor"/> that returns the original item.
    /// If it is an array it is returned as is. Collections, dictionaries, and field sets are
    /// converted to array. In any other cases the object is wrapped in a single element array.
    /// 
    /// This implementation relies on the contravariance of the type parameter of <see cref="T:IFieldExtractor"/>,
    /// thus it can be used at any place where an <see cref="T:IFieldExtractor"/> is expected as long
    /// as the type parameter is a reference type (i.e., if <c>T</c> is the type parameter you should have
    /// the following constraint : "<c>where T : class</c>").
    /// </summary>
    public class PassThroughFieldExtractor : IFieldExtractor<object>
    {
        /// <summary>
        /// Extracts an item to an array.
        /// The parameter is dynamic because the extraction depends on the runtime type.
        /// </summary>
        /// <param name="item">the item to extract</param>
        /// <returns>an array representing the given item</returns>
        public object[] Extract(dynamic item)
        {
            return DoExtract(item);
        }

        /// <summary>
        /// Extracts a dictionary to an array of objects.
        /// </summary>
        /// <typeparam name="TKey">&nbsp;the type of the dictionary keys</typeparam>
        /// <typeparam name="TValue">&nbsp;the type of the dictionary values</typeparam>
        /// <param name="dictionary">the dictionary to extract</param>
        /// <returns>an array of objects containing the values of the dictionary</returns>
        private static object[] DoExtract<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
        {
            return dictionary.Values.Cast<object>().ToArray();
        }

        /// <summary>
        /// Extracts an enumerable to an array of objects.
        /// </summary>
        /// <typeparam name="T">&nbsp;The type of the elements of the enumerable</typeparam>
        /// <param name="enumerable">an enumerable</param>
        /// <returns>an array of objects containing the elements of the enumerable</returns>
        private static object[] DoExtract<T>(IEnumerable<T> enumerable)
        {
            return enumerable.Cast<object>().ToArray();
        }

        /// <summary>
        /// Extracts a field set to an array of objects.
        /// </summary>
        /// <param name="fieldSet">a field set</param>
        /// <returns>an array of objects containing the values of the field set</returns>
        private static object[] DoExtract(IFieldSet fieldSet)
        {
            return fieldSet.Values.Cast<object>().ToArray();
        }

        /// <summary>
        /// Extracts an object to an array contaning it.
        /// </summary>
        /// <param name="obj">an object</param>
        /// <returns>an array containing the specified object</returns>
        private static object[] DoExtract(object obj)
        {
            return new[] { obj };
        }
    }
}