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
using System.Linq;
using Summer.Batch.Common.Util;

namespace Summer.Batch.Infrastructure.Item.File.Transform
{
    /// <summary>
    /// Abstract implementation of <see cref="T:ILineAggregator"/> that uses a <see cref="T:IFieldExtractor"/>
    /// to convert the incoming object to an array of its parts.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ExtractorLineAggregator<T> : ILineAggregator<T> where T : class
    {
        /// <summary>
        /// The field extractor for the elements that are aggregated
        /// </summary>
        public IFieldExtractor<T> FieldExtractor { get; set; }

        /// <summary>
        /// Protected default constructor that defines <see cref="PassThroughFieldExtractor"/> as the default field extractor.
        /// </summary>
        protected ExtractorLineAggregator()
        {
            FieldExtractor = new PassThroughFieldExtractor();
        }

        /// <summary>
        /// Transforms an item into a line.
        /// </summary>
        /// <param name="item">the item to transform</param>
        /// <returns>the line corresponding to the given item</returns>
        public string Aggregate(T item)
        {
            Assert.NotNull(item);
            var fields = FieldExtractor.Extract(item).Select(o => o ?? string.Empty).ToArray();

            return DoAggregate(fields);
        }

        /// <summary>
        /// Aggregates the extracted fields to a string.
        /// </summary>
        /// <param name="fields">the extracted fields</param>
        /// <returns>the aggregated line</returns>
        protected abstract string DoAggregate(object[] fields);
    }
}