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
//
//   Initial code found at
//   https://bitbucket.org/brimock/ordereddictionary/src/f9ae04458a9177d6c7a45d8b52844111f318d7eb/OrderedDictionary.cs
//   Included here with substantial improvements.
//
//   Original copyright notice:
//
//   The MIT License (MIT)
//
//   Copyright (c) 2013 Clinton Brennan
//
//   Permission is hereby granted, free of charge, to any person obtaining a copy
//   of this software and associated documentation files (the "Software"), to deal
//   in the Software without restriction, including without limitation the rights
//   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//   copies of the Software, and to permit persons to whom the Software is
//   furnished to do so, subject to the following conditions:
//
//   The above copyright notice and this permission notice shall be included in
//   all copies or substantial portions of the Software.
//
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//   THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Summer.Batch.Common.Collections
{
    /// <summary>
    /// Dictionary that preserves the insertion order during enumeration.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    [Serializable]
    public class OrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ISerializable
    {
        // Constants for serialization
        private const string PairsName = "Pairs";
        private const string ComparerName = "Comparer";

        private readonly Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>> _dictionary;
        private readonly LinkedList<KeyValuePair<TKey, TValue>> _list;

        private readonly KeyCollection _keyCollection;
        private readonly ValueCollection _valueCollection;

        /// <summary>
        /// Constructs a new <see cref="OrderedDictionary{TKey,TValue}"/> with the specified comparer and the default capacity.
        /// </summary>
        /// <param name="comparer">Comparer to use when comparing keys</param>
        public OrderedDictionary(IEqualityComparer<TKey> comparer) : this(0, comparer) { }

        /// <summary>
        /// Constructs a new <see cref="OrderedDictionary{TKey,TValue}"/> with the specified capacity and comparer.
        /// </summary>
        /// <param name="capacity">Initial number of elements that the dictionary can contain.</param>
        /// <param name="comparer">Comparer to use when comparing keys</param>
        /// <exception cref="ArgumentOutOfRangeException">capacity is less than 0</exception>
        public OrderedDictionary(int capacity = 0, IEqualityComparer<TKey> comparer = null)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException("capacity");
            }

            _dictionary = new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>(capacity, comparer);
            _list = new LinkedList<KeyValuePair<TKey, TValue>>();
            _valueCollection = new ValueCollection(this);
            _keyCollection = new KeyCollection(this);
        }

        /// <summary>
        /// Constructor for deserialization.
        /// </summary>
        /// <param name="info">the info holding the serialization data</param>
        /// <param name="context">the serialization context</param>
        protected OrderedDictionary(SerializationInfo info, StreamingContext context)
        {
            var comparer = (IEqualityComparer<TKey>)info.GetValue(ComparerName, typeof(IEqualityComparer<TKey>));
            var pairs = (KeyValuePair<TKey, TValue>[])info.GetValue(PairsName, typeof(KeyValuePair<TKey, TValue>[]));
            _dictionary = new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>(pairs.Length, comparer);
            _list = new LinkedList<KeyValuePair<TKey, TValue>>();
            _valueCollection = new ValueCollection(this);
            _keyCollection = new KeyCollection(this);
            foreach (var pair in pairs)
            {
                Add(pair.Key, pair.Value);
            }
        }

        /// <summary>
        /// Gets or sets a value for the specified key.
        /// </summary>
        /// <param name="key">The key to get or set.</param>
        /// <returns>The value for the specified key or <c>null</c> if it does not exist.</returns>
        public TValue this[TKey key]
        {
            get
            {
                return _dictionary[key].Value.Value;
            }
            set
            {
                if (_dictionary.ContainsKey(key))
                {
                    var lln = _dictionary[key];
                    lln.Value = new KeyValuePair<TKey, TValue>(key, value);
                }
                else
                {
                    Add(key, value);
                }
            }
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">Contains the value associated with the specified key, if the key is found.</param>
        /// <returns>Whether the key is found.</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            LinkedListNode<KeyValuePair<TKey, TValue>> lln;
            bool found = _dictionary.TryGetValue(key, out lln);
            if (!found)
            {
                value = default(TValue);
                return false;
            }
            value = lln.Value.Value;
            return true;
        }

        /// <summary>
        /// Returns whether the dictionary contains the specified key.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>whether the key exists in the dictionary.</returns>
        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        /// <summary>
        /// Adds a new entry with the specified key and value into the dictionary.
        /// </summary>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The value to add.</param>
        public void Add(TKey key, TValue value)
        {
            var lln = new LinkedListNode<KeyValuePair<TKey, TValue>>(new KeyValuePair<TKey, TValue>(key, value));
            _dictionary.Add(key, lln);
            _list.AddLast(lln);
        }

        /// <summary>
        /// Removes the entry with the specified key from the dictionary.
        /// </summary>
        /// <param name="key">The key to remove</param>
        /// <returns>whether the key was found in the dictionary and an actual removal happened</returns>
        /// <exception cref="ArgumentNullException">if key is null</exception>
        public bool Remove(TKey key)
        {
            LinkedListNode<KeyValuePair<TKey, TValue>> lln;
            // if key is null, an ArgumentNullException will be thrown
            bool found = _dictionary.TryGetValue(key, out lln);
            if (found)
            {
                _dictionary.Remove(key);
                _list.Remove(lln);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Clears the dictionary.
        /// </summary>
        public void Clear()
        {
            _dictionary.Clear();
            _list.Clear();
        }

        /// <summary>
        /// Gets the number pairs in the dictionary.
        /// </summary>
        /// <value>The number pairs in the dictionary.</value>
        public int Count
        {
            get { return _list.Count; }
        }

        /// <summary>
        /// Always returns false, as this implementation is not read only.
        /// </summary>
        /// <value>false.</value>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Returns an enumerator over pairs in this dictionary
        /// </summary>
        /// <returns>The enumerator</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets the collection of keys in the correct order.
        /// </summary>
        /// <value>A collection of keys in the correct order.</value>
        public ICollection<TKey> Keys
        {
            get { return _keyCollection; }
        }

        /// <summary>
        /// Gets the collection of values in the correct order.
        /// </summary>
        /// <value>A collection of values in the correct order.</value>
        public ICollection<TValue> Values
        {
            get { return _valueCollection; }
        }

        /// <summary>
        /// Adds the specified pair to the dictionary.
        /// </summary>
        /// <param name="item">The pair to add.</param>
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            var lln = new LinkedListNode<KeyValuePair<TKey, TValue>>(item);
            _dictionary.Add(item.Key, lln);
            _list.AddLast(lln);
        }

        /// <summary>
        /// Determines whether the dictionary contains the spcified pair.
        /// </summary>
        /// <param name="item">The pair to check.</param>
        /// <returns>Whether the pair exists in the dictionary.</returns>
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            TValue val;
            return TryGetValue(item.Key, out val) && EqualityComparer<TValue>.Default.Equals(val, item.Value);
        }

        /// <summary>
        /// Copies the elements of the dictionary to an array of pairs, starting at the specified index.
        /// </summary>
        /// <param name="array">The array of pairs to fill.</param>
        /// <param name="arrayIndex">The index in the array at which to start copy (zero-based).</param>
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes a key and value from the dictionary.
        /// </summary>
        /// <param name="item">The pair to remove.</param>
        /// <returns>Whether actual removal took place.</returns>
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            TValue val;
            if (TryGetValue(item.Key, out val) && EqualityComparer<TValue>.Default.Equals(val, item.Value))
            {
                Remove(item.Key);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds the proper information for serialization.
        /// </summary>
        /// <param name="info">the info holding the serialization data</param>
        /// <param name="context">the serialization context</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(ComparerName, _dictionary.Comparer);
            info.AddValue(PairsName, _list.ToArray());
        }

        private sealed class KeyCollection : ICollection<TKey>
        {
            private readonly OrderedDictionary<TKey, TValue> _dictionary;

            internal KeyCollection(OrderedDictionary<TKey, TValue> dictionary)
            {
                _dictionary = dictionary;
            }

            public IEnumerator<TKey> GetEnumerator()
            {
                return new KeyEnumerator(_dictionary._list);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void Add(TKey item)
            {
                throw new NotSupportedException();
            }

            public void Clear()
            {
                throw new NotSupportedException();
            }

            public bool Contains(TKey key)
            {
                return _dictionary.ContainsKey(key);
            }

            public void CopyTo(TKey[] array, int arrayIndex)
            {
                if (array == null)
                {
                    throw new ArgumentNullException("array");
                }

                if (arrayIndex < 0 || arrayIndex > array.Length || array.Rank > 1 || array.Length - arrayIndex < _dictionary._list.Count)
                {
                    throw new ArgumentOutOfRangeException("arrayIndex");
                }

                var i = arrayIndex;
                foreach (var pair in _dictionary._list)
                {
                    array[i++] = pair.Key;
                }
            }

            public bool Remove(TKey item)
            {
                throw new NotSupportedException();
            }

            public int Count
            {
                get { return _dictionary._list.Count; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            private class KeyEnumerator : IEnumerator<TKey>
            {
                private readonly IEnumerator<KeyValuePair<TKey, TValue>> _enumerator;

                internal KeyEnumerator(LinkedList<KeyValuePair<TKey, TValue>> list)
                {
                    _enumerator = list.GetEnumerator();
                }

                public void Dispose()
                {
                    _enumerator.Dispose();
                }

                public bool MoveNext()
                {
                    return _enumerator.MoveNext();
                }

                public void Reset()
                {
                    _enumerator.Reset();
                }

                public TKey Current
                {
                    get { return _enumerator.Current.Key; }
                }

                object IEnumerator.Current
                {
                    get { return Current; }
                }
            }
        }

        private sealed class ValueCollection : ICollection<TValue>
        {
            private readonly OrderedDictionary<TKey, TValue> _dictionary;

            public ValueCollection(OrderedDictionary<TKey, TValue> dictionary)
            {
                _dictionary = dictionary;
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                return new ValueEnumerator(_dictionary._list);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void Add(TValue item)
            {
                throw new NotSupportedException();
            }

            public void Clear()
            {
                throw new NotSupportedException();
            }

            public bool Contains(TValue value)
            {
                return _dictionary._list.Any(entry => EqualityComparer<TValue>.Default.Equals(value, entry.Value));
            }

            public void CopyTo(TValue[] array, int arrayIndex)
            {
                if (array == null)
                {
                    throw new ArgumentNullException("array");
                }

                if (arrayIndex < 0 || arrayIndex > array.Length || array.Rank > 1 || array.Length - arrayIndex < _dictionary._list.Count)
                {
                    throw new ArgumentOutOfRangeException("arrayIndex");
                }

                var i = arrayIndex;
                foreach (var pair in _dictionary._list)
                {
                    array[i++] = pair.Value;
                }
            }

            public bool Remove(TValue item)
            {
                throw new NotSupportedException();
            }

            public int Count
            {
                get { return _dictionary._list.Count; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            private class ValueEnumerator : IEnumerator<TValue>
            {
                private readonly IEnumerator<KeyValuePair<TKey, TValue>> _enumerator;

                internal ValueEnumerator(LinkedList<KeyValuePair<TKey, TValue>> list)
                {
                    _enumerator = list.GetEnumerator();
                }

                public void Dispose()
                {
                    _enumerator.Dispose();
                }

                public bool MoveNext()
                {
                    return _enumerator.MoveNext();
                }

                public void Reset()
                {
                    _enumerator.Reset();
                }

                public TValue Current
                {
                    get { return _enumerator.Current.Value; }
                }

                object IEnumerator.Current
                {
                    get { return Current; }
                }
            }
        }
    }
}