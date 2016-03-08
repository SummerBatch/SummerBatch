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

namespace Summer.Batch.Extra.Sort.Legacy
{
    /// <summary>
    /// Implementation of <see cref="IOutputFile{T}"/> for byte array records.
    /// </summary>
    public class LegacyOutputFile : IOutputFile<byte[]>
    {
        private IRecordWriter<byte[]> _writer;

        /// <summary>
        /// The output file.
        /// </summary>
        public FileInfo Output { get; set; }

        /// <summary>
        /// The filter used for selecting the records that are written in this output file.
        /// </summary>
        public IFilter<byte[]> Filter { get; set; }

        /// <summary>
        /// Formatter used while writing records.
        /// </summary>
        public IFormatter<byte[]> Formatter { get; set; }

        /// <summary>
        /// Opens the writer for this output file.
        /// </summary>
        /// <param name="recordAccessorFactory">
        /// The record accessor factory to use to open the record writer.
        /// </param>
        public void OpenWriter(IRecordAccessorFactory<byte[]> recordAccessorFactory)
        {
            _writer = recordAccessorFactory.CreateWriter(Output.Create());
        }

        /// <summary>
        /// Writes a record.
        /// </summary>
        /// <param name="record">the record to write</param>
        public void Write(byte[] record)
        {
            if (Filter == null || Filter.Select(record))
            {
                _writer.Write(Formatter == null ? record : Formatter.Format(record));
            }
        }

        /// <summary>
        /// Writes the header
        /// </summary>
        /// <param name="header">The header, as a list of records.</param>
        public void WriteHeader(IEnumerable<byte[]> header)
        {
            _writer.WriteHeader(header);
        }

        #region Disposable pattern

        /// <summary>
        /// Releases the used resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Actually disposes the current object.
        /// </summary>
        /// <param name="disposing">
        /// Indicates whether the method was invoked from the <see cref="IDisposable.Dispose"/>
        /// implementation or from the finalizer
        /// </param>
        private void Dispose(bool disposing)
        {
            if (disposing && _writer != null)
            {
                _writer.Dispose();
            }
        }

        #endregion
    }
}