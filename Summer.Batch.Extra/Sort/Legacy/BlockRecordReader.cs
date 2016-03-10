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
    /// Implementation of <see cref="IRecordReader{T}"/> for block records.
    /// If record length is more than zero the blocks are fixed length record, otherwise
    /// they are variable length record and a RDW (record descriptor word) should be present.
    /// </summary>
    public class BlockRecordReader : IRecordReader<byte[]>
    {
        private readonly int _recordLength;
        private readonly Stream _stream;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="stream">the stream to read from</param>
        /// <param name="recordLength">the record length (or zero if variable length)</param>
        public BlockRecordReader(Stream stream, int recordLength)
        {
            _stream = stream;
            _recordLength = recordLength;
        }

        /// <summary>
        /// Reads a record.
        /// </summary>
        /// <returns>the read record, or <c>null</c> if the end has been reached</returns>
        public byte[] Read()
        {
            var size = GetSize();            

            if (size == -1)
            {
                return null;
            }
            else if (size == 0)
            {
                throw new IOException("Error while reading record descriptor word: length 0 returned.");
            }


            var record = new byte[size];
            var read = _stream.Read(record);

            if (read == 0)
            {
                return null;
            }
            if (read != size)
            {
                throw new IOException("End of file reached before end of record.");
            }

            return record;
        }

        /// <returns>the size of the record to read</returns>
        private int GetSize()
        {
            if (_recordLength > 0)
            {
                return _recordLength;
            }
            var position = _stream.Position;
            var rdw = new byte[2];
            var read = _stream.Read(rdw);
            _stream.Position = position;
            if (read == 0)
            {
                return -1;
            }
            return (rdw[0] << 8) + rdw[1];
        }

        /// <summary>
        /// Headers are not supported on block records.
        /// </summary>
        /// <param name="headerSize">the size of the header</param>
        /// <returns><c>null</c></returns>
        public IEnumerable<byte[]> ReadHeader(int headerSize)
        {
            return null;
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