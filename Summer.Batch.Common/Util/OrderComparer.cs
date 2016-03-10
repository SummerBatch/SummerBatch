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
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
using System.Collections.Generic;

namespace Summer.Batch.Common.Util
{
    /// <summary>
    /// Comparer based on <see cref="Order"/>.
    /// </summary>
    /// <typeparam name="T">&nbsp;The type of objects to compare.</typeparam>
    public class OrderComparer<T> : IComparer<T>
    {
        /// <summary>
        /// Compares two instances of <typeparamref name="T"/> and returns a value indicating whether
        /// one is less than, equal to, or greater than the other.
        /// </summary>
        /// <returns>
        /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>.
        /// If less that zero <paramref name="x"/> is less than <paramref name="y"/>.
        /// If zero <paramref name="x"/> equals <paramref name="y"/>.
        /// If greater than zero <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        public int Compare(T x, T y)
        {
            var i1 = GetOrder(x);
            var i2 = GetOrder(y);
            return (i1 < i2) ? -1 : (i1 > i2) ? 1 : 0;
        }

        // Gets the order from an object.
        private static int GetOrder(object obj)
        {
            var order = OrderHelper.GetOrderFromAttribute(obj);
            return order != null ? order.Value : Order.LowestPrecedence;
        }
    }
}