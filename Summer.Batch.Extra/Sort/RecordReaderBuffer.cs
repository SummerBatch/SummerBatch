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
using System.Diagnostics;

namespace Summer.Batch.Extra.Sort
{
    /// <summary>
    /// Comparable buffer for <see cref="IRecordReader{T}"/>. It is used by <see cref="Sorter{T}"/>
    /// when merging different files. The buffers are sorted using their next record.
    /// </summary>
    /// <typeparam name="T">&nbsp;the type of the records</typeparam>
    [DebuggerDisplay("Cache={CacheDebuggerDisplay}")]
    public class RecordReaderBuffer<T> : IComparable<RecordReaderBuffer<T>>, IDisposable where T : class
    {
        private readonly IRecordReader<T> _reader;
        private readonly IComparer<T> _comparer;
        private Boolean _stableExternalSortTempFile;

        //last resort order if records are the same, use the order of the buffer to determine the comparison result
        public int Order;

        private T _cache;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="reader">the reader to read from</param>
        /// <param name="comparer">the comparer for the records</param>
        /// <param name="stableExternalSortTempFile">used to trigger a comparison of the file names in last resort
        /// If comparison of the next cached items are the same, compare the input file name ( in external sort input files are temporary pre-sorted files
        /// This is used in stable sort to ensure that for equals records (equality determined based upon the sort card) , the first
        /// record in is the first record out when reading using the external sort algorithm.</param>
        public RecordReaderBuffer(IRecordReader<T> reader, IComparer<T> comparer, Boolean stableExternalSortTempFile = false, int order = 0)
        {
            _reader = reader;
            _comparer = comparer;
            _cache = _reader.Read();
            _stableExternalSortTempFile = stableExternalSortTempFile;
            Order = order;
        }

        /// <summary>
        /// Reads a record.
        /// </summary>
        /// <returns>the next record</returns>
        public T Read()
        {
            var result = _cache;
            if (_cache != null)
            {
                _cache = _reader.Read();
            }
            return result;
        }

        /// <returns><c>true</c> if there are more items to read, <c>false</c> otherwise</returns>
        public bool HasNext()
        {
            return _cache != null;
        }

        /// <summary>
        /// Compares with another buffer by comparing the next item of each buffer.
        /// </summary>
        /// <param name="other">another buffer</param>
        /// <returns>the comparison of the two buffers</returns>
        public int CompareTo(RecordReaderBuffer<T> other)
        {
            int comparison = _comparer.Compare(_cache, other._cache);
            if (comparison == 0 && _stableExternalSortTempFile)
            {
                //records are the same, as a last resort, compare the file metadata
                return Order - other.Order;
            }
            // return the comparer comparison  

            return comparison;
            
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
            if (disposing && _reader != null)
            {
                _reader.Dispose();
            }
        }

        #endregion

        #region Debug

#if DEBUG
        private string CacheDebuggerDisplay
        {
            get
            {
                var bytes = _cache as byte[];
                return bytes != null ? System.Text.Encoding.Default.GetString(bytes) : _cache.ToString();
            }
        }
#endif

        #endregion

    }
}