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
using System;
using System.Collections.Generic;

namespace Summer.Batch.Extra.Sort
{
    /// <summary>
    /// Interface that abstracts a record writer.
    /// </summary>
    /// <typeparam name="T">the type of the records</typeparam>
    public interface IRecordWriter<in T> : IDisposable
    {
        /// <summary>
        /// Writes a record.
        /// </summary>
        /// <param name="record">the record to write</param>
        void Write(T record);

        /// <summary>
        /// Writes the header
        /// </summary>
        /// <param name="header">the header, as a list of records</param>
        void WriteHeader(IEnumerable<T> header);
    }
}