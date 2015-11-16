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

namespace Summer.Batch.Extra.Sort.Legacy
{
    /// <summary>
    /// Implementation of <see cref="IRecordReader{T}"/> for records separated by a specific separator.
    /// </summary>
    public class SeparatorRecordReader : IRecordReader<byte[]>
    {
        // Default size for the buffer (8MB)
        private const int DefaultBufferSize = 8 * 1024;

        // a buffer for the read bytes
        private byte[] _buffer;

        // the number of read bytes stored in the buffer
        private int _count;

        // the current position in the buffer
        private int _position;

        private readonly Stream _stream;

        /// <summary>
        /// The bytes corresponding to a record separator.
        /// </summary>
        public byte[] Separator { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="stream">the stream to read from</param>
        public SeparatorRecordReader(Stream stream)
        {
            _stream = stream;
            _buffer = new byte[DefaultBufferSize];
        }

        /// <summary>
        /// Reads a record.
        /// </summary>
        /// <returns>the read record, or <code>null</code> if the end has been reached</returns>
        public byte[] Read()
        {
            var separatorIndex = 0;
            var size = 0;
            var end = false;

            while (!end)
            {
                if (_position >= _count)
                {
                    // We have read all the data in the buffer but the end of the
                    // record has not been reached. Put more data in the buffer.
                    Fill();
                }
                if (_position >= _count)
                {
                    // Still no available data after a call to Fill, we reached end of file
                    size = _position;
                    end = true;
                }
                while (_position < _count && !end)
                {
                    if (_buffer[_position] == Separator[separatorIndex])
                    {
                        if (separatorIndex == Separator.Length - 1)
                        {
                            // The whole separator has been read, end of the record
                            size = _position - Separator.Length + 1;
                            end = true;
                        }
                        else
                        {
                            // We have started to read the seprator but it is not fully read
                            separatorIndex++;
                        }
                    }
                    else
                    {
                        // The current byte does not correspond to the expected byte in the separator,
                        // reset the separator index
                        separatorIndex = 0;
                    }
                    _position++;
                }
            }

            if (size <= 0)
            {
                return null;
            }
            // copy the record from the buffer to a new byte array
            var result = new byte[size];
            Buffer.BlockCopy(_buffer, 0, result, 0, size);
            // then reset the buffer
            ResetBuffer();
            return result;
        }

        /// <summary>
        /// Reads the header.
        /// </summary>
        /// <param name="headerSize">the size of the header</param>
        /// <returns>the header as a list of records</returns>
        public IEnumerable<byte[]> ReadHeader(int headerSize)
        {
            var header = new List<byte[]>();
            for (var i = 0; i < headerSize; i++)
            {
                header.Add(Read());
            }
            return header;
        }

        /// <summary>
        /// Fills the available bytes of the buffer by reading from the stream.
        /// If the buffer is full, its size is increased.
        /// </summary>
        private void Fill()
        {
            if (_position >= _buffer.Length)
            {
                // Buffer is full, increase size
                var newBuffer = new byte[_position * 2];
                Buffer.BlockCopy(_buffer, 0, newBuffer, 0, _buffer.Length);
                _buffer = newBuffer;
            }
            // Read into the buffer to fill all the available bytes
            var read = _stream.Read(_buffer, _position, _buffer.Length - _position);
            _count = read > 0 ? _position + read : _position;
        }

        /// <summary>
        /// Resets the buffer by copying all the available bytes that have not been read
        /// at the beginning of the buffer.
        /// </summary>
        private void ResetBuffer()
        {
            var available = _count - _position;
            if (available > 0)
            {
                // copy the available data to the beginning of the buffer
                Buffer.BlockCopy(_buffer, _position, _buffer, 0, available);
            }
            _position = 0;
            _count = available;
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