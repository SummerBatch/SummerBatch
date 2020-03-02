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
using Summer.Batch.Infrastructure.Item.Support;

namespace Summer.Batch.Extra.Process
{
    /// <summary>
    /// Common logic for managing streams in readers and writers used in a process.
    /// If the underlying reader/writer is not an ItemStream, this base class does nothing
    /// Not to be used directly, it contains only the base common logic of ProcessReaderAdapter 
    /// and ProcessWriterAdapter.
    /// </summary>
    public class AbstractProcessAdapter<T> : IProcessAdapter where T : class
    {
        private const string ReadCount = "read.count";
        private const string WriteInProcess = "batch.writeInProcess";
        /// <summary>
        /// Step context manager property.
        /// </summary>
        public IContextManager StepContextManager { private get; set; }

        /// <summary>
        /// The underlying stream
        /// </summary>
        private IItemStream _stream;

        /// <summary>
        /// stream opening must be done once only
        /// </summary>
        private bool _initDone; //Defaults to false


        /// <summary>
        /// @see IDisposable#Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Effective dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_stream != null)
                {
                    _stream.Close();
                }
                _stream = null;
            }
        }

        /// <summary>
        /// @see IProcessAdapter#RegisterStream
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterStream(object obj)
        {
            var stream = obj as IItemStream;
            if (stream != null)
            {
                _stream = stream;
            }
        }

        /// <summary>
        /// @see IProcessAdapter#InitStream
        /// </summary>
        public void InitStream()
        {
            
            if (!_initDone)
            {
                if (_stream != null)
                {
                    _stream.Open(StepContextManager.Context);
                    _initDone = true;
                }
            }
            else
            {
                StepContextManager.Context.Put(WriteInProcess, true);
            }
        }

        public void UpdateStream()
        {
            _stream.Update(StepContextManager.Context);
        }
        /// <summary>
        /// flush underlying stream.
        /// </summary>
        public void Flush()
        {
            _stream.Flush();
        }

        /// <summary>
        /// @see IProcessAdapter#ResetStream
        /// </summary>
        public void ResetStream()
        {
            ExecutionContext executionContext = StepContextManager.Context;
            _stream.Close();
            var reader = _stream as AbstractItemCountingItemStreamItemReader<T>;
            if (reader != null)
            {
                executionContext.Remove( reader.GetExecutionContextKey(ReadCount));
            }
            _stream.Open(executionContext);
        }
    }
}