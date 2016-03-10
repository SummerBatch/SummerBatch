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
namespace Summer.Batch.Extra.Sort.Filter
{
    /// <summary>
    /// Interface for selecting records. Correspond to omit or include in legacy.
    /// </summary>
    /// <typeparam name="T">&nbsp;the type of the records</typeparam>
    public interface IFilter<in T>
    {
        /// <summary>
        /// Determines if a record should be selected.
        /// </summary>
        /// <param name="record">a record in a file being sorted</param>
        /// <returns><c>true</c> if the record is selected, <c>false</c> otherwise</returns>
        bool Select(T record);
    }
}