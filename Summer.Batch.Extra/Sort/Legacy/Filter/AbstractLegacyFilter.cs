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
    /// Abstract base class for legacy filters based on byte array records.
    /// The filter compares two values using <see cref="IAccessor{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbstractLegacyFilter<T> : IFilter<byte[]>
    {
        /// <summary>
        /// The <see cref="IAccessor{T}"/> for the left value
        /// </summary>
        public IAccessor<T> Left { get; set; }

        /// <summary>
        /// The <see cref="IAccessor{T}"/> for the right value
        /// </summary>
        public IAccessor<T> Right { get; set; }

        /// <summary>
        /// The operator used to compare the values
        /// </summary>
        public ComparisonOperator Operator { get; set; }

        /// <summary>
        /// Determines if a record should be selected.
        /// </summary>
        /// <param name="record">a record in a file being sorted</param>
        /// <returns><code>true</code> if the record is selected, <code>false</code> otherwise</returns>
        public bool Select(byte[] record)
        {
            bool result;
            var comparison = DoComparison(Left.Get(record), Right.Get(record));

            switch (Operator)
            {
                case ComparisonOperator.Eq:
                    result = comparison == 0;
                    break;
                case ComparisonOperator.Ne:
                    result = comparison != 0;
                    break;
                case ComparisonOperator.Gt:
                    result = comparison > 0;
                    break;
                case ComparisonOperator.Ge:
                    result = comparison >= 0;
                    break;
                case ComparisonOperator.Lt:
                    result = comparison < 0;
                    break;
                case ComparisonOperator.Le:
                    result = comparison <= 0;
                    break;
                default:
                    result = false;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Does the actual comparison between the values
        /// </summary>
        /// <returns>the result of the comparison, as an integer</returns>
        /// <param name="leftValue">the left value of the comparison</param>
        /// <param name="rightValue">the right value of the comparison</param>
        protected abstract int DoComparison(T leftValue, T rightValue);
    }
}