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
using System.Collections;
using System.Collections.Generic;

namespace Summer.Batch.Common.Collections
{
    /// <summary>
    /// Abstract Priority Queue.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the queue.</typeparam>
    public abstract class AbstractPriorityQueue<T> : IQueue<T>, ICollection
    {
        private const int DefaultCapacity = 4;

        private static readonly T[] EmptyArray = new T[0];

        // The array containing the elements
        private T[] _items;

        // The number of elements in the array (can be lower than _items.Length)
        private int _internalCount;

        // Optimistic locking for the enumarator
        private int _version;

        /// <summary>
        /// The comparer used to order the elements in the queue.
        /// Can be null if <typeparamref name="T"/> implements <see cref="IComparable{T}"/>.
        /// </summary>
        public IComparer<T> Comparer { get; protected set; }

        /// <summary>
        /// The number of elements in the queue.
        /// </summary>
        public virtual int Count { get { return _internalCount; } }

        /// <summary>
        /// Always false
        /// </summary>
        public bool IsReadOnly { get { return false; } }

        /// <summary>
        /// The internal capacity of the internal array. It is automatically expended when needed.
        /// Can be set if it can be estimated, thus avoiding multiple resize, or to reduce its size
        /// if the number of elements has been considerably reduced.
        /// </summary>
        public int Capacity
        {
            get { return _items.Length; }
            set
            {
                if (value < _internalCount)
                {
                    throw new ArgumentOutOfRangeException("value", value,
                        string.Format("Cannot set Capacity to {0} as it is lower than current size ({1})", value, _internalCount));
                }
                if (value != _items.Length)
                {
                    if (value > 0)
                    {
                        Array.Resize(ref _items, value);
                    }
                    else
                    {
                        _items = EmptyArray;
                    }
                }
            }
        }

        /// <summary>
        /// Constructs a new priority queue with a default capacity and no elements.
        /// </summary>
        /// <param name="comparer">the comparer to use for ordering elements</param>
        /// <exception cref="InvalidOperationException">
        /// if <typeparamref name="T"/> does not implement <see cref="IComparable{T}"/> and comparer is null.
        /// </exception>
        protected AbstractPriorityQueue(IComparer<T> comparer = null)
        {
            CheckComparer(comparer);
            _items = EmptyArray;
            Comparer = comparer;
        }

        /// <summary>
        /// Constructs a new priority queue with the specified capacity and no elements.
        /// </summary>
        /// <param name="capacity">the initial capacity</param>
        /// <param name="comparer">the comparer to use for ordering elements</param>
        /// <exception cref="InvalidOperationException">
        /// if <typeparamref name="T"/> does not implement <see cref="IComparable{T}"/> and comparer is null.
        /// </exception>
        protected AbstractPriorityQueue(int capacity, IComparer<T> comparer = null)
        {
            CheckComparer(comparer);
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException("capacity", capacity, "capacity must be positive");
            }
            _items = capacity == 0 ? EmptyArray : new T[capacity];
            Comparer = comparer;
        }

        /// <summary>
        /// Constructs a new priority queue and initializes it with the given elements.
        /// </summary>
        /// <param name="enumerable">an enumerable containing the initial elements</param>
        /// <param name="comparer">the comparer to use for ordering elements</param>
        /// <exception cref="InvalidOperationException">
        /// if <typeparamref name="T"/> does not implement <see cref="IComparable{T}"/> and comparer is null.
        /// </exception>
        protected AbstractPriorityQueue(IEnumerable<T> enumerable, IComparer<T> comparer = null)
        {
            CheckComparer(comparer);
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }
            Comparer = comparer;
            var collection = enumerable as ICollection<T>;
            _items = collection == null ? EmptyArray : new T[collection.Count];
            foreach (var item in enumerable)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Removes the head of the queue and returns it.
        /// </summary>
        /// <returns>the head of the queue</returns>
        public abstract T Poll();

        /// <summary>
        /// Returns the head of the queue without removing it.
        /// </summary>
        /// <returns>the head of the queue</returns>
        public virtual T Peek()
        {
            return _internalCount == 0 ? default(T) : _items[0];
        }

        #region ICollection{T} methods

        /// <summary>
        /// Adds an item to the queue. The item is added at the correct position according to the current order.
        /// </summary>
        /// <param name="item">the item to add</param>
        public abstract void Add(T item);

        /// <summary>
        /// Clears the queue.
        /// </summary>
        public abstract void Clear();

        /// <summary>
        /// Checks if an element is inside the queue. The comparison is done using the current
        /// order rather than <see cref="object.Equals(object)"/>.
        /// </summary>
        /// <param name="item">the item to check</param>
        /// <returns><c>true</c> if there is an item with the same order as <paramref name="item"/>; <c>false</c> otherwise</returns>
        public abstract bool Contains(T item);

        /// <summary>
        /// Copies the elements of the queue to an array, preserving the order.
        /// </summary>
        /// <param name="array">the array to copy the elements to</param>
        /// <param name="arrayIndex">the index where to start copying</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// if the size of <paramref name="array"/> is lower than <paramref name="arrayIndex"/> + <see cref="Count"/>.
        /// </exception>
        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(_items, 0, array, arrayIndex, _internalCount);
        }

        /// <summary>
        /// Removes an item from the queue.
        /// </summary>
        /// <param name="item">the item to remove</param>
        /// <returns>true if the item was removed from the queue; false otherwise</returns>
        public abstract bool Remove(T item);

        #endregion

        #region ICollection methods

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        /// <returns>
        /// An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </returns>
        object ICollection.SyncRoot { get { return _items; } }

        /// <summary>
        /// Always false as the default implementation is not syncrhonized.
        /// </summary>
        /// <returns>false</returns>
        bool ICollection.IsSynchronized { get { return false; } }

        /// <summary>
        /// Copies the elements of the queue to an array, starting at a particular index.
        /// </summary>
        /// <param name="array">the destination array</param>
        /// <param name="index">the zero-based index in <paramref name="array"/> at which copying begins</param>
        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((T[]) array, index);
        }

        #endregion

        #region IEnumerable methods

        /// <summary>
        /// @see IEnumerable#GetEnumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Internal queue manipulation methods

        /// <summary>
        /// Removes the head and returns it.
        /// </summary>
        /// <returns>The element that was previously the head of the queue, or <c>default(T)</c> if the queue was empty.</returns>
        protected T DoPoll()
        {
            if (_internalCount == 0)
            {
                return default(T);
            }
            var result = _items[0];
            RemoveAt(0);
            return result;
        }

        /// <summary>
        /// Adds an item to the queue.
        /// </summary>
        /// <param name="item">The item to add.</param>
        protected void DoAdd(T item)
        {
            var index = Search(item, true);
            if (index == _items.Length)
            {
                EnsureCapacity(_internalCount + 1);
            }
            else
            {
                Array.Copy(_items, index, _items, index + 1, _internalCount - index);
            }
            _items[index] = item;
            _internalCount++;
            _version++;
        }

        /// <summary>
        /// Clears the queue.
        /// </summary>
        protected void DoClear()
        {
            if (_internalCount > 0)
            {
                Array.Clear(_items, 0, _internalCount);
                _internalCount = 0;
                _version++;
            }
        }

        /// <summary>
        /// Removes an item from the queue.
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <returns><c>true</c> if the item was actually removed; <c>false</c> otherwise.</returns>
        protected bool DoRemove(T item)
        {
            var index = Search(item, false);
            if (index < 0)
            {
                return false;
            }
            RemoveAt(index);
            return true;
        }

        /// <summary>
        /// Checks if an item is present in the queue.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <returns><c>true</c> if <paramref name="item"/> is present in the queue, <c>false</c> otherwise.</returns>
        protected bool DoContains(T item)
        {
            return Search(item, false) >= 0;
        }

        // checks that T is comparable or that a comparer has been provided
        private static void CheckComparer(IComparer<T> comparer)
        {
            if (comparer == null && typeof(T).IsAssignableFrom(typeof(IComparable<T>)))
            {
                throw new InvalidOperationException(
                    string.Format("Type {0} does not implement IComparable and no comparer has been provided.",
                        typeof(T).Name));
            }
        }

        // removes an item at the specified index
        private void RemoveAt(int index)
        {
            _internalCount--;
            if (index < _internalCount)
            {
                Array.Copy(_items, index + 1, _items, index, _internalCount - index);
            }
            _items[_internalCount] = default(T);
            _version++;
        }

        // increases the capacity if required
        private void EnsureCapacity(int capacity)
        {
            if (capacity > _internalCount)
            {
                var newCapacity = _items.Length == 0 ? DefaultCapacity : Math.Min(_items.Length * 2, int.MaxValue);
                Capacity = newCapacity;
            }
        }

        // binary search for an item
        private int Search(T item, bool insert)
        {
            var start = 0;
            var end = _internalCount - 1;
            while (start <= end)
            {
                var index = (start + end) / 2;
                var comparison = Compare(item, _items[index]);
                if (comparison > 0)
                {
                    start = index + 1;
                }
                else if (comparison < 0)
                {
                    end = index - 1;
                }
                else
                {
                    return index;
                }
            }
            // if inserting an item we return the current index
            // otherwise we return -1 to indicate the item was not found
            return insert ? start : -1;
        }

        // does the actual comparison between elements
        private int Compare(T x, T y)
        {
            if (Comparer != null)
            {
                return Comparer.Compare(x, y);
            }
            var comparable = x as IComparable<T>;
            return comparable != null ? comparable.CompareTo(y) : 0;
        }

        #endregion

        private class Enumerator : IEnumerator<T>
        {
            private readonly AbstractPriorityQueue<T> _priorityQueue;
            private readonly int _version;
            private int _index;

            public Enumerator(AbstractPriorityQueue<T> priorityQueue)
            {
                _priorityQueue = priorityQueue;
                _version = priorityQueue._version;
            }

            public T Current { get; private set; }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                if (_version != _priorityQueue._version)
                {
                    throw new InvalidOperationException("Concurrent modification of priority queue during enumeration.");
                }
                if (_index < _priorityQueue.Count)
                {
                    Current = _priorityQueue._items[_index];
                    _index++;
                    return true;
                }
                Current = default(T);
                _index = _priorityQueue.Count;
                return false;
            }

            public void Reset()
            {
                if (_version != _priorityQueue._version)
                {
                    throw new InvalidOperationException("Concurrent modification of priority queue during enumeration.");
                }
                _index = 0;
            }

            public void Dispose()
            {
                // nothing to do
            }
        }
    }
}