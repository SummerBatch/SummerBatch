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
using NLog;
using Summer.Batch.Extra.Sort.Format;
using Summer.Batch.Extra.Sort.Sum;
using Summer.Batch.Common.Util;

namespace Summer.Batch.Extra.Sort
{
    /// <summary>
    /// Implementation of a <see cref="IRecordWriter{T}"/> that can sum similare records using a <see cref="ISum{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SumWriter<T> : IRecordWriter<T>
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IRecordWriter<T> _writer;
        private readonly ISum<T> _sum;
        private readonly IComparer<T> _comparer;
        private readonly IFormatter<T> _outputFormatter;
        private readonly IList<T> _buffer = new List<T>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="writer">the underlying writer</param>
        /// <param name="sum">the sum to use for similar items (or <code>null</code>)</param>
        /// <param name="comparer">the comparer to use for records</param>
        /// <param name="outputFormatter">the formatter to use when writing the records (or <code>null</code>)</param>
        public SumWriter(IRecordWriter<T> writer, ISum<T> sum, IComparer<T> comparer, IFormatter<T> outputFormatter)
        {
            _writer = writer;
            _sum = sum;
            _comparer = comparer;
            _outputFormatter = outputFormatter;
        }

        /// <summary>
        /// Writes a record.
        /// </summary>
        /// <param name="record">the record to write</param>
        public void Write(T record)
        {
            if (_sum == null)
            {
                WriteRecord(record);
            }
            else
            {
                if (_buffer.Count == 0 || _comparer.Compare(record, _buffer[0]) == 0)
                {
                    _logger.Trace("Record added to sum buffer");
                    _buffer.Add(record);
                }
                else
                {
                    _logger.Trace("Writing sum buffer");
                    WriteRecord(_sum.Sum(_buffer));
                    _buffer.Clear();
                    _buffer.Add(record);
                }
            }
        }

        /// <summary>
        /// Writes the header
        /// </summary>
        /// <param name="header">the header, as a list of records</param>
        public void WriteHeader(IEnumerable<T> header)
        {
            _writer.WriteHeader(header);
        }

        /// <summary>
        /// Writes a record using the output formatter if it has been defined.
        /// </summary>
        /// <param name="record">the record to write</param>
        private void WriteRecord(T record)
        {
            if (_logger.IsTraceEnabled) { _logger.Trace("Writing record: {0}", ObjectUtils.Dump(record)); }
            _writer.Write(_outputFormatter == null ? record : _outputFormatter.Format(record));
        }

        #region Dispose pattern members

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
            if (disposing && _writer != null)
            {
                if (_buffer.Count > 0)
                {
                    WriteRecord(_sum.Sum(_buffer));
                }
                _writer.Dispose();
            }
        }

        #endregion

    }
}