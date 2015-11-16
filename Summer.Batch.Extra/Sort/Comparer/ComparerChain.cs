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

namespace Summer.Batch.Extra.Sort.Comparer
{
    /// <summary>
    /// Implementation of <see cref="IComparer{T}"/> that chains a list of <see cref="IComparer{T}"/>s.
    /// The result of the comparison is the result of the first sub-comparers that has a result different than zero,
    /// or zero if all comparers return zero.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ComparerChain<T> : IComparer<T>
    {
        /// <summary>
        /// The sub-comparers.
        /// </summary>
        public IList<IComparer<T>> Comparers { get; set; }

        /// <summary>
        /// Compares two objects using sub-comparers.
        /// </summary>
        /// <returns>the result of the first sub-comparers that has a result different than zero, or zero if all comparers return zero</returns>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        public int Compare(T x, T y)
        {
            foreach (var comparer in Comparers)
            {
                var comparison = comparer.Compare(x, y);
                if (comparison != 0)
                {
                    return comparison;
                }
            }
            return 0;
        }
    }
}