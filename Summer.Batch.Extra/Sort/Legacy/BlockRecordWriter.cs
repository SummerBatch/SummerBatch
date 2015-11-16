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
using System.IO;
using Summer.Batch.Common.Extensions;

namespace Summer.Batch.Extra.Sort.Legacy
{
    /// <summary>
    /// Implementation of <see cref="IRecordWriter{T}"/> for block records.
    /// </summary>
    public class BlockRecordWriter : IRecordWriter<byte[]>
    {
        private readonly Stream _stream;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="stream">the stream to write to</param>
        public BlockRecordWriter(Stream stream)
        {
            _stream = stream;
        }

        /// <summary>
        /// Headers are not supported on block records.
        /// </summary>
        /// <param name="header">ignored</param>
        public void WriteHeader(IEnumerable<byte[]> header)
        {
            // Nothing to do
        }

        /// <summary>
        /// Writes a record.
        /// </summary>
        /// <param name="record">the record to write</param>
        public void Write(byte[] record)
        {
            _stream.Write(record);
        }

        #region Dispose pattern

        /// <summary>
        /// @see IDisposable#Dispose
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
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _stream != null)
            {
                _stream.Dispose();             
            }
        }

        #endregion
    }
}