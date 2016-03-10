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
using System.Linq;

namespace Summer.Batch.Infrastructure.Item.Support
{
    /// <summary>
    /// Simple item reader that pulls its data from a supplied list.
    /// </summary>
    /// <typeparam name="T">&nbsp;</typeparam>
    public class ListItemReader<T> : IItemReader<T> where T : class
    {
        private readonly IList<T> _list;

        /// <summary>
        /// Constructor with the list supplying the read elements.
        /// </summary>
        /// <param name="list"></param>
        public ListItemReader(IEnumerable<T> list)
        {
            _list = new List<T>(list);
        }

        /// <summary>
        /// Reads at each call the next item in the list.
        /// </summary>
        /// <returns>the next item in the list</returns>
        public T Read()
        {
            if (_list.Any())
            {
                T element = _list[0];
                _list.RemoveAt(0);
                return element;
            }
            return null;
        }
    }
}