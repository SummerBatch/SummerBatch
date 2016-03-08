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

using System;
using System.Collections.Generic;
using System.IO;
using Summer.Batch.Extra.Sort.Filter;
using Summer.Batch.Extra.Sort.Format;

namespace Summer.Batch.Extra.Sort
{
    /// <summary>
    /// Class that represents an output file and its associated options (e.g., filter, formatter).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IOutputFile<T> : IDisposable where T : class
    {
        /// <summary>
        /// The output file.
        /// </summary>
        FileInfo Output { get; set; }

        /// <summary>
        /// The filter used for selecting the records that are written in this output file.
        /// </summary>
        IFilter<T> Filter { get; set; }

        /// <summary>
        /// Formatter used while writing records.
        /// </summary>
        IFormatter<T> Formatter { get; set; }

        /// <summary>
        /// Opens the writer for this output file.
        /// </summary>
        /// <param name="recordAccessorFactory">
        /// The record accessor factory to use to open the record writer.
        /// </param>
        void OpenWriter(IRecordAccessorFactory<T> recordAccessorFactory);

        /// <summary>
        /// Writes a record.
        /// </summary>
        /// <param name="record">the record to write</param>
        void Write(T record);

        /// <summary>
        /// Writes the header
        /// </summary>
        /// <param name="header">The header, as a list of records.</param>
        void WriteHeader(IEnumerable<T> header);
    }
}