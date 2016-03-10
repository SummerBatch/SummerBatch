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
    /// An interface for getting placeholder for SQL queries.
    /// </summary>
    public interface IPlaceholderGetter
    {
        /// <summary>
        /// Whether the placeholders provided by this <see cref="IPlaceholderGetter"/>
        /// are named. If <c>false</c>, they are position-based.
        /// </summary>
        bool Named { get; }

        /// <summary>
        /// Gets the correct placeholder for a query parameter.
        /// </summary>
        /// <param name="name">the name of the parameter</param>
        /// <returns>the corresponding parameter placeholder</returns>
        string GetPlaceholder(string name);
    }
}