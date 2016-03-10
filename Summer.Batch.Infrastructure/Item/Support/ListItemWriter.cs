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
using System.Collections.Generic;

namespace Summer.Batch.Infrastructure.Item.Support
{
    /// <summary>
    /// Simple item writer that populates a List with the elements to be written.
    /// </summary>
    /// <typeparam name="T">&nbsp;</typeparam>
    public class ListItemWriter<T> : IItemWriter<T> where T:class
    {
        private readonly IList<T> _writtenItems = new List<T>();

        /// <summary>
        /// Ability to retrieve the list containing the written items.
        /// </summary>
        public IList<T> WrittemItems 
        {
            get { return _writtenItems; }
        }

        /// <summary>
        /// Writes items in the inner list.
        /// </summary>
        /// <param name="items">the items to write</param>
        public void Write(IList<T> items)
        {
            foreach (var item in items)
            {
                _writtenItems.Add(item);
            }
        }
    }
}