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
namespace Summer.Batch.Extra.Sort.Legacy.Filter
{
    /// <summary>
    /// Implementation of <see cref="AbstractLegacyFilter{T}"/> for comparing decimals.
    /// </summary>
    public class DecimalFilter : AbstractLegacyFilter<decimal>
    {
        /// <summary>
        /// Does the actual comparison between the values
        /// </summary>
        /// <returns>the result of the comparison, as an integer</returns>
        /// <param name="leftValue">the left value of the comparison</param>
        /// <param name="rightValue">the right value of the comparison</param>
        protected override int DoComparison(decimal leftValue, decimal rightValue)
        {
            return leftValue.CompareTo(rightValue);
        }
    }
}