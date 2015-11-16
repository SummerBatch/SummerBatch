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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Summer.Batch.Core.Step.Item
{
    /// <summary>
    /// Encapsulation of a list of items to be processed and possibly a list of
    /// failed items to be skipped. To mark an item as skipped clients should iterate
    /// over the chunk , and if there is a failure call Remove() on the items list.
    /// The skipped items are then available through the chunk.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Chunk<T>
    {
        #region Attributes
        private readonly List<T> _items = new List<T>();

        /// <summary>
        /// List of items property.
        /// </summary>
        public IList<T> Items { get { return _items; } }

        private readonly List<Exception> _errors = new List<Exception>();

        /// <summary>
        /// Collection of errors property.
        /// </summary>
        public ICollection<Exception> Errors { get { return new ReadOnlyCollection<Exception>(_errors); } }

        /// <summary>
        /// User data property.
        /// </summary>
        public object UserData { get; set; }

        /// <summary>
        /// End (of the chunk) flag property.
        /// </summary>
        public bool End { get; set; }

        /// <summary>
        /// Busy chunk flag property.
        /// </summary>
        public bool Busy { get; set; }
        #endregion

        #region Constructors
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Chunk() : this(null) { }

        /// <summary>
        /// Custom constructor with a list of items.
        /// </summary>
        /// <param name="items"></param>
        public Chunk(ICollection<T> items)
        {
            if (items != null)
            {
                _items = new List<T>(items);
            }
        }
        #endregion

        /// <summary>
        /// Adds the item to the chunk.
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            _items.Add(item);
        }

        /// <summary>
        /// Clears the items down to signal that we are done.
        /// </summary>
        public void Clear()
        {
            _items.Clear();
            UserData = null;
        }

        /// <summary>
        /// Registers an anonymous skip. To skip an individual item, use
        /// ChunkIterator#Remove().
        /// </summary>
        /// <param name="e"></param>
        public void Skip(Exception e)
        {
            _errors.Add(e);
        }

        /// <summary>
        /// </summary>
        /// <returns>true if there are no items in the chunk</returns>
        public bool IsEmpty()
        {
            return !_items.Any();
        }

        /// <summary>
        /// </summary>
        /// <returns>the number of items (excluding skips)</returns>
        public int Size()
        {
            return _items.Count;
        }

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[items={0}]", _items);
        }

    }
}