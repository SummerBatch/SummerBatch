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
using System.Linq;
using Summer.Batch.Infrastructure.Item;

namespace Summer.Batch.Extra.Delegating
{
    /// <summary>
    /// This class delegates writing to the inner writer, checking a condition
    /// through the supplied ItemCondition for each element to decide if
    /// it must be written.
    /// </summary>
    /// <typeparam name="TT"> The type of objects written by the writer</typeparam>
    public class DelegatingConditionalItemWriter<TT> : IItemStreamWriter<TT> where TT:class
    {
        /// <summary>
        /// DElegate property.
        /// </summary>
        public IItemWriter<TT> Delegate { private get; set; }

        /// <summary>
        /// Condition property.
        /// </summary>
        public IItemCondition<TT> Condition { private get; set; }

        /// <summary>
        /// Simply delegating to the inner conditioned writer
        /// </summary>
        /// <param name="executionContext">the execution context</param>
        public void Open(ExecutionContext executionContext)
        {
            var stream = Delegate as IItemStream;
            if (stream != null)
            {
                stream.Open(executionContext);
            }
        }

        /// <summary>
        /// Simply delegating to the inner conditioned writer
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
        /// Simply delegating to the inner conditioned writer
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
        /// Writes through the inner writer, effectively writing only if condition is satisfied.
        /// </summary>
        /// <param name="items">the chunk to write</param>
        /// <exception cref="Exception"></exception>
        public void Write(IList<TT> items)
        {
            List<TT> toWrite = items.Where(element => Condition.Check(element)).ToList();
            Delegate.Write(toWrite);
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