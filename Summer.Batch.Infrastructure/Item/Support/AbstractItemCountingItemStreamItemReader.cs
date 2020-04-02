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
using Summer.Batch.Common.Util;
using System;

namespace Summer.Batch.Infrastructure.Item.Support
{
    /// <summary>
    /// Abstract superclass for IItemReaders that supports restart by storing item count in the execution context
    /// and therefore requires item ordering to be preserved between runs.
    /// </summary>
    /// <typeparam name="T">&nbsp;The type of the read items</typeparam>
    public abstract class AbstractItemCountingItemStreamItemReader<T> : AbstractItemStreamItemReader<T> where T : class
    {
        private const string ReadCount = "read.count";
        private const string ReadCountMax = "read.count.max";
        private const string BufferReader = "batch.bufferReader";
        private int _maxItemCount = int.MaxValue;
        private bool _saveState = true;

        /// <summary>
        /// The index of the item to start reading from.
        /// If the execution context contains a key <c>[name].read.count</c>
        /// (where [name] is the name of this component), the value from the execution
        /// context will be used instead.
        /// </summary>
        public int CurrentItemCount { protected get; set; }

        /// <summary>
        /// The maximum index of the items to be read.
        /// If the execution context contains a key <c>[name].read.count.max</c>
        /// (where [name] is the name of this component), the value from the execution
        /// context will be used instead.
        /// </summary>
        public int MaxItemCount
        {
            set { _maxItemCount = value; }
        }

        /// <summary>
        /// Flat that determines whether to save internal data in the execution context.
        /// Default value is true. Switch to false if you don't want to save any state
        /// from this stream or you don't want it to be restartable.
        /// </summary>
        public bool SaveState
        {
            get { return _saveState; }
            set { _saveState = value; }
        }

        /// <summary>
        /// Reads a piece of input data and advance to the next one. Implementations
        /// <strong>must</strong> return <c>null</c> at the end of the input
        /// data set. In a transactional setting, caller might get the same item
        /// twice from successive calls (or otherwise), if the first call was in a
        /// transaction that rolled back.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">&nbsp;</exception>
        /// <exception cref="UnexpectedInputException">&nbsp;</exception>
        /// <exception cref="NonTransientResourceException">&nbsp;</exception>
        /// <exception cref="ParseException">&nbsp;</exception>
        public override T Read()
        {
            if (CurrentItemCount >= _maxItemCount)
            {
                return null;
            }
            CurrentItemCount++;
            return DoRead();
        }

        /// <summary>
        /// Open the stream for the provided ExecutionContext.
        /// </summary>
        /// <param name="executionContext">current step's ExecutionContext.  Will be the
        /// executionContext from the last run of the step on a restart.</param>
        /// <exception cref="ItemStreamException">&nbsp;</exception>
        /// <exception cref="ArgumentException">&nbsp;if execution context is null</exception>
        public override void Open(ExecutionContext executionContext)
        {
            base.Open(executionContext);
            DoOpen();
            if (!SaveState)
            {
                return;
            }

            var maxKey = GetExecutionContextKey(ReadCountMax);
            if (executionContext.ContainsKey(maxKey))
            {
                MaxItemCount = executionContext.GetInt(maxKey);
            }

            var countKey = GetExecutionContextKey(ReadCount);
            var itemCount = executionContext.ContainsKey(countKey) ? executionContext.GetInt(countKey) : CurrentItemCount;

            if (itemCount > 0 && itemCount < _maxItemCount)
            {
                JumpToItem(itemCount);
            }

            CurrentItemCount = itemCount;
        }

        /// <summary>
        ///  If any resources are needed for the stream to operate they need to be destroyed here. Once this method has been
        /// called all other methods (except open) may throw an exception.
        /// </summary>
        /// <exception cref="ItemStreamException">&nbsp;</exception>
        public override void Close()
        {
            base.Close();
            CurrentItemCount = 0;
            DoClose();
        }

        /// <summary>
        /// Indicates that the execution context provided during open is about to be saved. If any state is remaining, but
        /// has not been put in the context, it should be added here.
        /// </summary>
        /// <param name="executionContext">to be updated</param>
        /// <exception cref="ItemStreamException">&nbsp;</exception>
        /// <exception cref="ArgumentException">&nbsp;if execution context is null</exception>
        public override void Update(ExecutionContext executionContext)
        {
            base.Update(executionContext);
            if (SaveState)
            {
                Assert.NotNull(executionContext, "executionContext must not be null");
                UpdateForBufferedReader(executionContext);
                executionContext.PutInt(GetExecutionContextKey(ReadCount), CurrentItemCount);
                if (_maxItemCount < int.MaxValue)
                {
                    executionContext.PutInt(GetExecutionContextKey(ReadCountMax), _maxItemCount);
                }
            }
        }
        /// <summary>
        /// Updates the execution for BufferReader
        /// </summary>
        /// <param name="executionContext"></param>
        private void UpdateForBufferedReader(ExecutionContext executionContext)
        {
            if (executionContext.ContainsKey(BufferReader) && (bool)executionContext.Get(BufferReader))
            {
                CurrentItemCount--;
                executionContext.Put(BufferReader, false);
            }
        }

        /// <summary>
        /// Reads the next item from input.
        /// </summary>
        /// <returns>the read item or null if the end of the stream has been reached</returns>
        protected abstract T DoRead();

        /// <summary>
        /// Opens the stream.
        /// </summary>
        protected abstract void DoOpen();

        /// <summary>
        /// Closes the stream.
        /// </summary>
        protected abstract void DoClose();

        /// <summary>
        /// Move to the given item index.
        /// </summary>
        /// <param name="itemIndex">zero-based index of the item to jump to</param>
        protected virtual void JumpToItem(int itemIndex)
        {
            for (var i = 0; i < itemIndex; i++)
            {
                Read();
            }
        }

    }
}