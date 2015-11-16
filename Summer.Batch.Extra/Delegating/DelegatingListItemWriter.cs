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
using Summer.Batch.Infrastructure.Item;

namespace Summer.Batch.Extra.Delegating
{
    /// <summary>
    /// Writter that flattens collections into a list before delegating to an inner writer. Used when a
    /// processor returns a collection.
    /// </summary>
    /// <typeparam name="TCollection">The type of objects written by the writer. 
    /// Must be a collection of TItem (enforced by constraint on type parameter, see below)</typeparam>
    /// <typeparam name="TItem">The type of objects written by the delegated writers</typeparam>
    public class DelegatingListItemWriter<TCollection, TItem> : 
        IItemStreamWriter<TCollection>
        where TCollection : class,ICollection<TItem> 
        where TItem:class       
    {
        /// <summary>
        /// Delegate property.
        /// </summary>
        public IItemWriter<TItem> Delegate { private get; set; } 

        /// <summary>
        ///  Simply delegating to the inner writer.
        /// </summary>
        /// <param name="executionContext"></param>
        public void Open(ExecutionContext executionContext)
        {
            var stream = Delegate as IItemStream;
            if (stream != null)
            {
                stream.Open(executionContext);
            }
        }

        /// <summary>
        /// Simply delegating to the inner writer.
        /// </summary>
        /// <param name="executionContext"></param>
        public void Update(ExecutionContext executionContext)
        {
            var stream = Delegate as IItemStream;
            if (stream != null)
            {
                stream.Update(executionContext);
            }
        }

        /// <summary>
        /// Simply delegating to the inner writer.
        /// </summary>
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
        /// Writes through the inner writer, writing each element in the input inner list.
        /// </summary>
        /// <param name="items"></param>
        public void Write(IList<TCollection> items ) 
        {
            var flattenedList = new List<TItem>();
            foreach (var item in items)
            {
                flattenedList.AddRange(item);
            }
            Delegate.Write(flattenedList);
        }

        #region Disposable pattern

        /// <summary>
        /// Releases the resources used by the writer.
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