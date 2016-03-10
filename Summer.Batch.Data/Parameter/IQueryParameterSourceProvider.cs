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
    /// Interface in charge of providing <see cref="IQueryParameterSource"/> for items.
    /// </summary>
    /// <typeparam name="T">&nbsp;the type of the items that this provider supports</typeparam>
    public interface IQueryParameterSourceProvider<in T>
    {
        /// <summary>
        /// Creates a new <see cref="IQueryParameterSource"/> for the given item.
        /// </summary>
        /// <param name="item">the item for which to create a parameter source</param>
        /// <returns>a new parameter source for the given item</returns>
        IQueryParameterSource CreateParameterSource(T item);
    }
}