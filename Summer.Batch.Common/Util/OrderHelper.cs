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
using System;
using System.Linq;

namespace Summer.Batch.Common.Util
{
    /// <summary>
    /// Order Helper; needed to check for Attribute being set.
    /// </summary>
    public static class OrderHelper
    {
        /// <summary>
        /// Gets the order from an object.
        /// </summary>
        /// <param name="obj">The object to get the order from.</param>
        /// <returns>The <see cref="Order"/> of <paramref name="obj"/>, or <c>null</c> if it has no order.</returns>
        public static Order GetOrderFromAttribute(object obj)
        {
            var attrs = Attribute.GetCustomAttributes(obj.GetType());  // Reflection. 
            return attrs.OfType<Order>().FirstOrDefault(); // linq
        }

        /// <summary>
        /// Checks if <see cref="Order"/> has been set on the type of an object.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <returns>
        /// <c>true</c> if the type of <paramref name="obj"/> has an <see cref="Order"/> attribute,
        /// <c>false</c>otherwise.
        /// </returns>
        public static bool IsOrdered(object obj)
        {
            var attrs = Attribute.GetCustomAttributes(obj.GetType());  // Reflection. 
            return attrs.OfType<Order>().Any(); // linq
        }
    }
}