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
namespace Summer.Batch.Data.Parameter
{
    /// <summary>
    /// Implementation of <see cref="T:IQueryParameterSourceProvider"/> that creates a new <see cref="PropertyParameterSource"/>
    /// for each item.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyParameterSourceProvider<T> : IQueryParameterSourceProvider<T>
    {
        /// <summary>
        /// Returns a new <see cref="PropertyParameterSource"/> for the given item.
        /// </summary>
        /// <param name="item">the item for which to create a parameter source</param>
        /// <returns>a new parameter source for the given item</returns>
        public IQueryParameterSource CreateParameterSource(T item)
        {
            return new PropertyParameterSource(item);
        }
    }
}