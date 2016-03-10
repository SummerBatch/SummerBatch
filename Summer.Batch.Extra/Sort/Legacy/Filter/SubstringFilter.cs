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
using Summer.Batch.Extra.Sort.Filter;
using Summer.Batch.Extra.Sort.Legacy.Accessor;

namespace Summer.Batch.Extra.Sort.Legacy.Filter
{
    /// <summary>
    /// Implementation of <see cref="AbstractLegacyFilter{T}"/> for substring comparison.
    /// </summary>
    public class SubstringFilter : IFilter<byte[]>
    {
        /// <summary>
        /// The <see cref="IAccessor{T}"/> for the left string
        /// </summary>
        public IAccessor<string> Left { get; set; }

        /// <summary>
        /// The <see cref="IAccessor{T}"/> for the right string
        /// </summary>
        public IAccessor<string> Right { get; set; }

        /// <summary>
        /// Determines if a record should be selected.
        /// </summary>
        /// <param name="record">a record in a file being sorted</param>
        /// <returns><c>true</c> if the record is selected, <c>false</c> otherwise</returns>
        public bool Select(byte[] record)
        {
            var leftString = Left.Get(record);
            var rightString = Right.Get(record);
            return leftString.Contains(rightString) || rightString.Contains(leftString);
        }
    }
}