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
using System.Collections.Generic;
using System.Diagnostics;

namespace Summer.Batch.Common.Collections
{
    /// <summary>
    /// <p>A queue where elements are ordered by a <see cref="IComparer{T}"/> or according to their natural order
    /// if they implement <see cref="IComparable{T}"/>. The comparer always takes precedence.</p>
    /// 
    /// <p>The head of the queue is the minimum element with respect to the order. Ties are allowed and are broken arbitrarily.</p>
    /// 
    /// <p>Time complexity for <see cref="Add"/>, <see cref="Contains"/>, and <see cref="Remove"/> is O(log(n)).
    /// Time complexity for <see cref="Poll"/> and <see cref="AbstractPriorityQueue{T}.Peek"/> is O(1).</p>
    /// 
    /// <p>This implementation is NOT thread-safe.</p>
    /// </summary>
    /// <typeparam name="T">&nbsp;The type of the elements in the queue.</typeparam>
    [DebuggerDisplay("InternalCount={Count}")]
    public class PriorityQueue<T> : AbstractPriorityQueue<T>
    {
        /// <summary>
        /// Constructs a new priority queue with a default capacity and no elements.
        /// </summary>
        /// <param name="comparer">the comparer to use for ordering elements</param>
        /// <exception cref="InvalidOperationException">&nbsp;
        /// if <typeparamref name="T"/> does not implement <see cref="IComparable{T}"/> and comparer is null.
        /// </exception>
        public PriorityQueue(IComparer<T> comparer = null) : base(comparer) { }

        /// <summary>
        /// Constructs a new priority queue with the specified capacity and no elements.
        /// </summary>
        /// <param name="capacity">the initial capacity</param>
        /// <param name="comparer">the comparer to use for ordering elements</param>
        /// <exception cref="InvalidOperationException">&nbsp;
        /// if <typeparamref name="T"/> does not implement <see cref="IComparable{T}"/> and comparer is null.
        /// </exception>
        public PriorityQueue(int capacity, IComparer<T> comparer = null) : base(capacity, comparer) { }

        /// <summary>
        /// Constructs a new priority queue and initializes it with the given elements.
        /// </summary>
        /// <param name="enumerable">an enumerable containing the initial elements</param>
        /// <param name="comparer">the comparer to use for ordering elements</param>
        /// <exception cref="InvalidOperationException">&nbsp;
        /// if <typeparamref name="T"/> does not implement <see cref="IComparable{T}"/> and comparer is null.
        /// </exception>
        public PriorityQueue(IEnumerable<T> enumerable, IComparer<T> comparer = null) : base(enumerable, comparer) { }

        /// <summary>
        /// Removes the head of the queue and returns it.
        /// </summary>
        /// <returns>the head of the queue</returns>
        public override T Poll()
        {
            return DoPoll();
        }

        #region ICollection{T} methods

        /// <summary>
        /// Adds an item to the queue. The item is added at the correct position according to the current order.
        /// </summary>
        /// <param name="item">the item to add</param>
        public override void Add(T item)
        {
            DoAdd(item);
        }

        /// <summary>
        /// Removes all elements from the queue.
        /// </summary>
        public override void Clear()
        {
            DoClear();
        }

        /// <summary>
        /// Checks if an element is inside the queue. The comparison is done using the current
        /// order rather than <see cref="object.Equals(object)"/>.
        /// </summary>
        /// <param name="item">the item to check</param>
        /// <returns>true if there is an item with the same order as <paramref name="item"/>; false otherwise</returns>
        public override bool Contains(T item)
        {
            return DoContains(item);
        }

        /// <summary>
        /// Removes an item from the queue.
        /// </summary>
        /// <param name="item">the item to remove</param>
        /// <returns>true if the item was removed from the queue; false otherwise</returns>
        public override bool Remove(T item)
        {
            return DoRemove(item);
        }

        #endregion
    }
}