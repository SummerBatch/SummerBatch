using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Summer.Batch.Common.Collections
{
    /// <summary>
    /// Priority Blocking Queue; inherits from AbstractPriorityQueue
    /// </summary>
    /// <typeparam name="T">&nbsp;The type of the elements in the queue.</typeparam>
    public class PriorityBlockingQueue<T> : AbstractPriorityQueue<T>, ICollection
    {
        private readonly object _lock = new object();

        /// <summary>
        /// The number of elements in the queue.
        /// </summary>
        public override int Count
        {
            get
            {
                lock (_lock)
                {
                    return base.Count;
                }
            }
        }

        /// <summary>
        /// Constructs a new priority queue with a default capacity and no elements.
        /// </summary>
        /// <param name="comparer">the comparer to use for ordering elements</param>
        /// <exception cref="InvalidOperationException">&nbsp;
        /// if <typeparamref name="T"/> does not implement <see cref="IComparable{T}"/> and comparer is null.
        /// </exception>
        public PriorityBlockingQueue(IComparer<T> comparer = null) : base(comparer) { }

        /// <summary>
        /// Constructs a new priority queue with the specified capacity and no elements.
        /// </summary>
        /// <param name="capacity">the initial capacity</param>
        /// <param name="comparer">the comparer to use for ordering elements</param>
        /// <exception cref="InvalidOperationException">&nbsp;
        /// if <typeparamref name="T"/> does not implement <see cref="IComparable{T}"/> and comparer is null.
        /// </exception>
        public PriorityBlockingQueue(int capacity, IComparer<T> comparer = null) : base(capacity, comparer) { }

        /// <summary>
        /// Constructs a new priority queue and initializes it with the given elements.
        /// </summary>
        /// <param name="enumerable">an enumerable containing the initial elements</param>
        /// <param name="comparer">the comparer to use for ordering elements</param>
        /// <exception cref="InvalidOperationException">&nbsp;
        /// if <typeparamref name="T"/> does not implement <see cref="IComparable{T}"/> and comparer is null.
        /// </exception>
        public PriorityBlockingQueue(IEnumerable<T> enumerable, IComparer<T> comparer = null) : base(enumerable, comparer) { }

        /// <summary>
        /// Removes the head of the queue and returns it.
        /// </summary>
        /// <returns>the head of the queue</returns>
        public override T Poll()
        {
            lock (_lock)
            {
                return DoPoll();
            }
        }

        /// <summary>
        /// Removes the head of the queue and returns it.
        /// If the queue is empty, waits until an element is added.
        /// </summary>
        /// <returns></returns>
        public T Take()
        {
            lock (_lock)
            {
                while (Count == 0)
                {
                    Monitor.Wait(_lock);
                }
                return DoPoll();
            }
        }

        /// <summary>
        /// Returns the head of the queue without removing it.
        /// </summary>
        /// <returns>the head of the queue</returns>
        public override T Peek()
        {
            lock (_lock)
            {
                return base.Peek();
            }
        }

        #region ICollection{T} methods

        /// <summary>
        /// Adds an item to the queue. The item is added at the correct position according to the current order.
        /// </summary>
        /// <param name="item">the item to add</param>
        public override void Add(T item)
        {
            lock (_lock)
            {
                DoAdd(item);
                Monitor.Pulse(_lock);
            }
        }

        /// <summary>
        /// Removes all elements from the queue.
        /// </summary>
        public override void Clear()
        {
            lock (_lock)
            {
                DoClear();
            }
        }

        /// <summary>
        /// Checks if an element is inside the queue. The comparison is done using the current
        /// order rather than <see cref="object.Equals(object)"/>.
        /// </summary>
        /// <param name="item">the item to check</param>
        /// <returns>true if there is an item with the same order as <paramref name="item"/>; false otherwise</returns>
        public override bool Contains(T item)
        {
            lock (_lock)
            {
                return DoContains(item);
            }
        }

        /// <summary>
        /// Copies the elements of the queue to an array, preserving the order.
        /// </summary>
        /// <param name="array">the array to copy the elements to</param>
        /// <param name="arrayIndex">the index where to start copying</param>
        /// <exception cref="ArgumentOutOfRangeException">&nbsp;
        /// if the size of <paramref name="array"/> is lower than
        /// <paramref name="arrayIndex"/> + <see cref="AbstractPriorityQueue{T}.Count"/>.
        /// </exception>
        public override void CopyTo(T[] array, int arrayIndex)
        {
            lock (_lock)
            {
                base.CopyTo(array, arrayIndex);
            }
        }

        /// <summary>
        /// Removes an item from the queue.
        /// </summary>
        /// <param name="item">the item to remove</param>
        /// <returns>true if the item was removed from the queue; false otherwise</returns>
        public override bool Remove(T item)
        {
            lock (_lock)
            {
                return DoRemove(item);
            }
        }

        #endregion

        #region ICollection methods

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        /// <returns>
        /// An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </returns>
        object ICollection.SyncRoot { get { return _lock; } }

        /// <summary>
        /// Always true, as <see cref="PriorityBlockingQueue{T}"/> is synchronized.
        /// </summary>
        /// <returns>true</returns>
        bool ICollection.IsSynchronized { get { return true; } }

        #endregion
    }
}