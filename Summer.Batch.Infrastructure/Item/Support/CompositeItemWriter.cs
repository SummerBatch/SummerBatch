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
using Summer.Batch.Common.Util;

namespace Summer.Batch.Infrastructure.Item.Support
{
    /// <summary>
    /// Calls a collection of IItemWriter. 
    /// Delegate writers are kept in an ordered collection.
    /// Thread-safe, provided delegates are thread-safe as well.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CompositeItemWriter<T> : IItemStreamWriter<T> where T:class
    {
        /// <summary>
        /// Collection of delegate writers.
        /// </summary>
        public IList<IItemWriter<T>> Delegates { get; set; }

        /// <summary>
        /// Open list of delegate writers.
        /// </summary>
        /// <param name="executionContext"></param>
        public void Open(ExecutionContext executionContext)
        {
            Assert.NotEmpty(Delegates, "Delegates must not be empty");
            foreach (var writer in Delegates.OfType<IItemStream>())
            {
                writer.Open(executionContext);
            }
        }

        /// <summary>
        /// Call update on list of delegate writers.
        /// </summary>
        /// <param name="executionContext"></param>
        public void Update(ExecutionContext executionContext)
        {
            foreach (var writer in Delegates.OfType<IItemStream>())
            {
                writer.Update(executionContext);
            }
        }

        /// <summary>
        /// Call close on list of delegate writers.
        /// </summary>
        public void Close()
        {
            foreach (var writer in Delegates.OfType<IItemStream>())
            {
                writer.Close();
            }
        }

        /// <summary>
        /// Call flush on list of delegate writers.
        /// </summary>
        public void Flush()
        {
            foreach (var writer in Delegates.OfType<IItemStream>())
            {
                writer.Flush();
            }
        }

        /// <summary>
        /// Call write on list of delegate writers.
        /// </summary>
        /// <param name="items"></param>
        public void Write(IList<T> items)
        {
            foreach (var writer in Delegates)
            {
                writer.Write(items);
            }
        }

        #region Disposable pattern

        /// <summary>
        /// Releases the resources used by the writers.
        /// </summary>
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Does the actual dispose, simply delegating to Close();
        /// </summary>
        /// <param name="disposing"></param>
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