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
using System;
using Summer.Batch.Extra.Sort.Legacy.Accessor;

namespace Summer.Batch.Extra.Sort.Legacy.Comparer
{
    /// <summary>
    /// Default implementation of <see cref="AbstractBytesComparer"/> that relies on an <see cref="IAccessor{T}"/>
    /// to retrieve a value to compare in a byte array.
    /// </summary>
    /// <typeparam name="T">the type of the extracted value</typeparam>
    public class DefaultComparer<T> : AbstractBytesComparer where T : IComparable<T>
    {
        /// <summary>
        /// Accessor to the value to compare.
        /// </summary>
        public IAccessor<T> Accessor { get; set; }

        /// <summary>
        /// Compares two byte arrays by comparing a value retrieved on each one.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>The comparison of <paramref name="x"/> and <paramref name="y"/></returns>
        protected override int DoCompare(byte[] x, byte[] y)
        {
            var v1 = Accessor.Get(x);
            var v2 = Accessor.Get(y);
            return v1.CompareTo(v2);
        }
    }
}