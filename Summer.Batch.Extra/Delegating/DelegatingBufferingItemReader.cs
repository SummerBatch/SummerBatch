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
using Summer.Batch.Infrastructure.Item;

namespace Summer.Batch.Extra.Delegating
{
    /// <summary>
    /// This class delegates reading to the inner reader and keeps track of
    /// the next record to read, in order to know if the current one is the last one
    /// The next record to read is then buffered to be returned next time.
    /// The "last one" information is saved in the step context.
    /// </summary>
    /// <typeparam name="T">&nbsp;The type of objects read by the reader</typeparam>
    public class DelegatingBufferingItemReader<T> : IItemStreamReader<T> where T : class
    {

        private const string BufferReader = "batch.bufferReader";
        private const string Restart = "batch.restart";
        /// <summary>
        /// The delegate, i.e. the real reader that is buffered.
        /// </summary>
        public IItemReader<T> Delegate { private get; set; }

        /// <summary>
        /// The buffer, containing the record to be processed next time.
        /// </summary>
        private T _buffer;

        /// <summary>
        /// The context, used to transmit "last one" information to the process.
        /// </summary>
        private ExecutionContext _executionContext;

        /// <summary>
        /// Boolean to know if this is the first read.
        /// </summary>
        private bool _isFirst = true;

        /// <summary>
        /// Boolean to know if this is the second read.
        /// </summary>
        private bool _isSecond = false;

        /// <summary>
        /// Simply delegating to the inner buffered reader.
        /// </summary>
        /// <param name="executionContext">the execution context</param>
        /// <exception cref="ItemStreamException">&nbsp;</exception>
        public void Open(ExecutionContext executionContext)
        {
            _executionContext = executionContext;
            var stream = Delegate as IItemStream;
            if (stream != null)
            {
                stream.Open(executionContext);
            }
        }

        /// <summary>
        /// Simply delegating to the inner buffered reader.
        /// </summary>
        /// <param name="executionContext"></param>
        /// <exception cref="ItemStreamException">&nbsp;</exception>
        public void Update(ExecutionContext executionContext)
        {
            var stream = Delegate as IItemStream;
            if (_isSecond)
            {
                executionContext.Put(BufferReader, true);
                _isSecond = false;
            }
            if (stream != null)
            {
                stream.Update(executionContext);
            }
        }

        /// <summary>
        /// Simply delegating to the inner buffered reader.
        /// </summary>
        /// <exception cref="ItemStreamException">&nbsp;</exception>
        public void Close()
        {
            var stream = Delegate as IDisposable;
            if (stream != null)
            {
                stream.Dispose();
            }
        }

        /// <summary>
        /// Flushes the delegate writer if it is an instance of <see cref="IItemStream"/>.
        /// </summary>
        public void Flush()
        {
            var stream = Delegate as IItemStream;
            if (stream != null)
            {
                stream.Flush();
            }
        }

        /// <summary>
        /// Read through delegate, buffering records to always have on more and be able
        /// to indicate if current one is the last one.
        /// </summary>
        /// <returns>the read record</returns>
        public T Read()
        {
            T toReturn;
            if (_isFirst)
            {
               //read first record through delegate
                toReturn = Delegate.Read();
                _isFirst = false;
                _isSecond = true;
            }
            else
            {
                // read current record in buffer
                toReturn = _buffer;
            }

            // If current record is null, we have reached the end of the step
            // We must not read next, which would overflow
            if (toReturn != null)
            {
                // Read next record through delegate and store it in buffer
                _buffer = Delegate.Read();
                // Null next record means that the current one is the last one
                if (_buffer == null)
                {
                    _executionContext.Put(BatchConstants.LastRecordKey, toReturn);
                }
            }
            return toReturn;
        }

        #region Disposable pattern

        /// <summary>
        /// Releases the resources used by the reader.
        /// </summary>
        public virtual void Dispose()
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
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }
        }

        #endregion
    }
}