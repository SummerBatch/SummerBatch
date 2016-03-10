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
using System.Collections.Generic;
using System.Linq;

namespace Summer.Batch.Extra.Sort.Filter
{
    /// <summary>
    /// A filter that does the logical disjunction of several filters.
    /// </summary>
    /// <typeparam name="T">&nbsp;the type of the filtered records</typeparam>
    public class DisjunctionFilter<T> : IFilter<T>
    {
        /// <summary>
        /// The filters to use in the disjunction.
        /// </summary>
        public ICollection<IFilter<T>> Filters { get; set; }

        /// <summary>
        /// Determines if a record should be selected.
        /// </summary>
        /// <param name="record">a record in a file being sorted</param>
        /// <returns><c>true</c> if the record is selected, <c>false</c> otherwise</returns>
        public bool Select(T record)
        {
            return Filters.Any(f => f.Select(record));
        }
    }
}