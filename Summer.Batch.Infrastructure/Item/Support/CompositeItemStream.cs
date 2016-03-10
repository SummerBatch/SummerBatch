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

// This file has been modified.
// Original copyright notice :

/*
 * Copyright 2006-2013 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace Summer.Batch.Infrastructure.Item.Support
{
    /// <summary>
    /// Simple IITemStream that delegates to a list of other streams.
    /// </summary>
    public class CompositeItemStream : IItemStream
    {
        private List<IItemStream> _streams = new List<IItemStream>();

        /// <summary>
        /// The underlying streams.
        /// </summary>
        public IItemStream[] Streams
        {
            set
            {
                _streams = value.ToList();
            }
        }

        /// <summary>
        ///  Register a IITemStream as one of the interesting providers under
        /// the provided key.
        /// </summary>
        /// <param name="stream"></param>
        public void Register(IItemStream stream)
        {
            lock (_streams)
            {
                if (!_streams.Contains(stream))
                {
                    _streams.Add(stream);
                }
            }
        }

        /// <summary>
        /// Broadcast the call to open.
        /// </summary>
        /// <param name="executionContext"></param>
        /// <exception cref="ItemStreamException">&nbsp;</exception>
        public void Open(ExecutionContext executionContext)
        {
            foreach (var itemStream in _streams)
            {
                itemStream.Open(executionContext);
            }
        }

        /// <summary>
        /// Simple aggregate ExecutionContext provider for the contributions
        /// registered under the given key.
        /// </summary>
        /// <param name="executionContext"></param>
        /// <exception cref="ItemStreamException">&nbsp;</exception>
        public void Update(ExecutionContext executionContext)
        {
            foreach (var itemStream in _streams)
            {
                itemStream.Update(executionContext);
            }
        }

        /// <summary>
        /// Flushes all streams.
        /// </summary>
        public void Flush()
        {
            foreach (var itemStream in _streams)
            {
                itemStream.Flush();
            }
        }

        /// <summary>
        /// Brodcast the call to close
        /// </summary>
        /// <exception cref="ItemStreamException">&nbsp;</exception>
        public void Close()
        {
            foreach (var itemStream in _streams)
            {
                itemStream.Close();
            }
        }

        #region IDisposable members

        /// <summary>
        /// @see IDisposable#Dispose .
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Does actual dispose. Simply delegates to Close();
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }
        }

        #endregion
    }
}