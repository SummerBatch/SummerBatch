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
using NLog;
using Summer.Batch.Extra.Sort.Sum;
using Summer.Batch.Common.Util;

namespace Summer.Batch.Extra.Sort
{
    /// <summary>
    /// Implementation of a <see cref="IRecordWriter{T}"/> that can sum similar records using a <see cref="ISum{T}"/>.
    /// </summary>
    /// <typeparam name="T">&nbsp;type of the records</typeparam>
    public class SumWriter<T> : IDisposable where T : class
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IOutputFile<T> _outputFile;
        private readonly ISum<T> _sum;
        private readonly IComparer<T> _comparer;
        private readonly IList<T> _buffer = new List<T>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="outputFile">The description of the current output file.</param>
        /// <param name="sum">The sum to use for similar items (or <c>null</c>).</param>
        /// <param name="comparer">The comparer to use for sorting records.</param>
        public SumWriter(IOutputFile<T> outputFile, ISum<T> sum, IComparer<T> comparer)
        {
            _outputFile = outputFile;
            _sum = sum;
            _comparer = comparer;
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
            _outputFile.WriteHeader(header);
        }

        /// <summary>
        /// Writes a record using the output formatter if there is one.
        /// </summary>
        /// <param name="record">the record to write</param>
        private void WriteRecord(T record)
        {
            if (_logger.IsTraceEnabled) { _logger.Trace("Writing record: {0}", ObjectUtils.Dump(record)); }
            _outputFile.Write(record);
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
            if (disposing && _outputFile != null)
            {
                if (_buffer.Count > 0)
                {
                    WriteRecord(_sum.Sum(_buffer));
                }
                _outputFile.Dispose();
            }
        }

        #endregion
    }
}