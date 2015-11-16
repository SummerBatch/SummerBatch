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
 * Copyright 2002-2012 the original author or authors.
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

using System;

namespace Summer.Batch.Common.Util
{
    /// <summary>
    /// Order metadata attribute.
    /// see <see cref="T:Summer.Batch.CoreTests.Core.Listener.OrderedCompositeTest"/> for an example of Order usage :
    /// [Order(order)] order being an int
    /// Lowest orders come first.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Class
      | AttributeTargets.Interface)]
    public class Order:Attribute
    {
        /// <summary>
        /// LowestPrecedence constant.
        /// </summary>
        public const int LowestPrecedence = Int32.MaxValue;

        /// <summary>
        /// HighestPrecedence constant.
        /// </summary>
        public const int HighestPrecedence = Int32.MinValue;

        private readonly int _value;
        
        /// <summary>
        /// Returns the value.
        /// </summary>
        public int Value { get { return _value; } }

        /// <summary>
        /// Custom constructor with given order.
        /// </summary>
        /// <param name="order"></param>
        public Order(int order)
        {
            _value = order;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Order()
        {
            _value = LowestPrecedence;
        }

    }
}