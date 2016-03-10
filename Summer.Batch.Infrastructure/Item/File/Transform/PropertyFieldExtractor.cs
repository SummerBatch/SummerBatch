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
using System.Linq;
using Summer.Batch.Common.Property;
using Summer.Batch.Common.Util;

namespace Summer.Batch.Infrastructure.Item.File.Transform
{
    /// <summary>
    /// Implementation of <see cref="T:IFieldExtractor"/> that retrieve values from property names.
    /// </summary>
    /// <typeparam name="T">&nbsp;</typeparam>
    public class PropertyFieldExtractor<T> : IFieldExtractor<T>
    {
        /// <summary>
        /// The names of the properties to retrieve.
        /// </summary>
        public IEnumerable<string> Names { get; set; }

        /// <summary>
        /// Extracts an item to an array.
        /// </summary>
        /// <param name="item">the item to extract</param>
        /// <returns>an array representing the given item</returns>
        public object[] Extract(T item)
        {
            Assert.NotNull(Names, "Names must be set.");
            var accessor = new PropertyAccessor(item);
            return Names.Select(propertyName => accessor.GetProperty(propertyName)).ToArray();
        }
    }
}