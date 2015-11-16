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
namespace Summer.Batch.Infrastructure.Item.File.Transform
{
    /// <summary>
    /// Implementation of <see cref="T:ILineAggregator"/> that simply calls <see cref="object.ToString"/>.
    /// </summary>
    /// <typeparam name="T">the type of the aggregated instances</typeparam>
    public class PassThroughLineAggregator<T> : ILineAggregator<T>
    {
        /// <summary>
        /// Transforms an item into a line using <see cref="object.ToString"/>.
        /// </summary>
        /// <param name="item">the item to transform</param>
        /// <returns>the line corresponding to the given item</returns>
        public string Aggregate(T item)
        {
            return item.ToString();
        }
    }
}