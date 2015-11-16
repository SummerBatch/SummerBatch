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

namespace Summer.Batch.Extra.Sort.Legacy.Comparer
{
    /// <summary>
    /// Base class for implementations of <see cref="IComparer{T}"/> on byte arrays.
    /// </summary>
    public abstract class AbstractBytesComparer : IComparer<byte[]>
    {
        private int _ascendingCoef = 1;

        /// <summary>
        /// Whether the byte arrays are sorted in ascending order.
        /// </summary>
        public bool Ascending
        {
            get { return _ascendingCoef == 1; }
            set { _ascendingCoef = value ? 1 : -1; }
        }

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>The comparison of <paramref name="x"/> and <paramref name="y"/></returns>
        public int Compare(byte[] x, byte[] y)
        {
            return _ascendingCoef * DoCompare(x, y);
        }

        /// <summary>
        /// Method that does the actual comparison. Implementations should return a value assuming an ascending order,
        /// the value will be negated if the actual sorting should be done in descending order.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>The comparison of <paramref name="x"/> and <paramref name="y"/></returns>
        protected abstract int DoCompare(byte[] x, byte[] y);
    }
}