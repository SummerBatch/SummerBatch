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

namespace Summer.Batch.Infrastructure.Item.Util
{
    /// <summary>
    /// Basic implementation of IItemStream.
    /// </summary>
    public abstract class ItemStreamSupport : IItemStream
    {
        private readonly ExecutionContextUserSupport _executionContextUserSupport = new ExecutionContextUserSupport();

        /// <summary>
        /// The name of the component that will be used as prefix for keys in the execution context.
        /// </summary>
        public string Name { set { _executionContextUserSupport.Name = value; } }

        /// <summary>
        /// Close the stream
        /// </summary>
        public virtual void Close()
        {
        }

        /// <summary>
        /// Opens the stream.
        /// </summary>
        /// <param name="executionContext"></param>
        public virtual void Open(ExecutionContext executionContext)
        {
        }

        /// <summary>
        /// Updates the stream.
        /// </summary>
        /// <param name="executionContext"></param>
        public virtual void Update(ExecutionContext executionContext)
        {
        }

        /// <summary>
        /// Flushes the stream.
        /// </summary>
        public virtual void Flush()
        {
        }

        /// <summary>
        /// Transform the given key to use the name of this instance as prefix.
        /// </summary>
        /// <param name="key">a key to transform</param>
        /// <returns>the given key with an identifying prefix</returns>
        public string GetExecutionContextKey(string key)
        {
            return _executionContextUserSupport.GetKey(key);
        }

        #region IDisposable members


        /// <summary>
        /// @see IDisposable#Dispose .
        /// </summary>
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Does the actual dipose, delegating to Close();
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