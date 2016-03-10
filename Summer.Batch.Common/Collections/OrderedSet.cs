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
using System.Collections;
using System.Collections.Generic;

namespace Summer.Batch.Common.Collections
{
    /// <summary>
    /// A set that preserves the insertion order.
    /// </summary>
    /// <typeparam name="T">&nbsp;The type of the elements in the set.</typeparam>
    public class OrderedSet<T> : ICollection<T>
    {
        private readonly IDictionary<T, LinkedListNode<T>> _dictionary;
        private readonly LinkedList<T> _linkedList;

        /// <summary>
        /// Constructs a new <see cref="OrderedDictionary{TKey,TValue}"/> with the default comparer.
        /// </summary>
        public OrderedSet() : this(EqualityComparer<T>.Default) { }

        /// <summary>
        /// Constructs a new <see cref="OrderedDictionary{TKey,TValue}"/> with the specified comparer.
        /// </summary>
        /// <param name="comparer">The comparer to use for ordering elements.</param>
        public OrderedSet(IEqualityComparer<T> comparer)
        {
            _dictionary = new Dictionary<T, LinkedListNode<T>>(comparer);
            _linkedList = new LinkedList<T>();
        }

        /// <summary>
        /// The number of elements in the collection.
        /// </summary>
        public int Count
        {
            get { return _dictionary.Count; }
        }

        /// <summary>
        /// Whether the set is readonly.
        /// </summary>
        public virtual bool IsReadOnly
        {
            get { return _dictionary.IsReadOnly; }
        }

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        /// <summary>
        /// Clears the set.
        /// </summary>
        public void Clear()
        {
            _linkedList.Clear();
            _dictionary.Clear();
        }

        /// <summary>
        /// Removes an item from the set.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            LinkedListNode<T> node;
            var found = _dictionary.TryGetValue(item, out node);
            if (!found) return false;
            _dictionary.Remove(item);
            _linkedList.Remove(node);
            return true;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the set.
        /// </summary>
        /// <returns>
        /// A <see cref="IEnumerator{T}"/> that can be used to iterate through the set.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _linkedList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Determines whether the set contains a specific value.
        /// </summary>
        /// <returns><c>true</c> if <paramref name="item"/> is found in the set; <c>false</c> otherwise.</returns>
        /// <param name="item">The object to locate in the set.</param>
        public bool Contains(T item)
        {
            return _dictionary.ContainsKey(item);
        }

        /// <summary>
        /// Copies the elements of the set to an array, preserving the order.
        /// </summary>
        /// <param name="array">The array to copy the elements to.</param>
        /// <param name="arrayIndex">The index where to start copying.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _linkedList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Adds an item to the set.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns><c>tru</c> if the item was actually added; <c>false</c> if it was alread present in the set.</returns>
        public bool Add(T item)
        {
            if (_dictionary.ContainsKey(item)) return false;
            var node = _linkedList.AddLast(item);
            _dictionary.Add(item, node);
            return true;
        }
    }
}