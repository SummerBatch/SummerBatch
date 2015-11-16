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
    /// Enumeration of the different comparison possibles for filter.
    /// </summary>
    public enum ComparisonOperator
    {
        /// <summary>
        /// The two parts must be equal
        /// </summary>
        Eq,
        /// <summary>
        /// The two parts must be different
        /// </summary>
        Ne,
        /// <summary>
        /// the first part must be greater than the second part
        /// </summary>
        Gt,
        /// <summary>
        /// the first part must be greater than or equal to the second part
        /// </summary>
        Ge,
        /// <summary>
        /// the first part must be lower than the second part
        /// </summary>
        Lt,
        /// <summary>
        /// the first part must be lower than or equal to the second part
        /// </summary>
        Le
    }
}