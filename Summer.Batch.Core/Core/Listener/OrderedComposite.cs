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

//   This file has been modified.
//   Original copyright notice :

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

using Summer.Batch.Common.Util;
using System.Collections.Generic;

namespace Summer.Batch.Core.Listener
{
    /// <summary>
    /// Ordered Composite : Composite pattern used on items that need to hold an order metadata information.
    /// Relies on System.Attribute Order being put on classes / interfaces
    /// (see Order / OrderHelper / OrderComparer in Util NS)
    /// </summary>
    /// <typeparam name="TS"></typeparam>
    public class OrderedComposite<TS>
    {

        private readonly List<TS> _unordered = new List<TS>();
        private readonly List<TS> _ordered = new List<TS>();
        private readonly IComparer<TS> _comparer = new OrderComparer<TS>();
        private readonly List<TS> _list = new List<TS>();

        /// <summary>
        /// Public setter for items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        public void SetItems<T>(List<T> items) where T : TS
        {
            _unordered.Clear();
            _ordered.Clear();
            foreach (T s in items)
            {
                Add(s);
            }
        }

        /// <summary>
        /// Registers an additional item.
        /// </summary>
        /// <param name="item"></param>
        public void Add(TS item)
        {
            if (OrderHelper.IsOrdered(item))
            {
                if (!_ordered.Contains(item))
                {
                    _ordered.Add(item);
                }
            }

            else if (!_unordered.Contains(item))
            {
                _unordered.Add(item);
            }

            _ordered.Sort(_comparer);

            _list.Clear();
            _list.AddRange(_ordered);
            _list.AddRange(_unordered);
        }

        
        /// <summary>
        /// Public getter for the list of items. The Ordered items come
        /// first, followed by any unordered ones.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TS> Enumerator()
        {
            return new List<TS>(_list).GetEnumerator();
        }
        
        /// <summary>
        /// Public getter for the list of items in reverse order. The Ordered items
        /// come last, after any unordered ones.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TS> Reverse()
        {
            List<TS> result = new List<TS>(_list);
            result.Reverse();
            return result.GetEnumerator();
        }
    }
}