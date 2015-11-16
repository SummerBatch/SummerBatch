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
using Summer.Batch.Infrastructure.Repeat.Context;

namespace Summer.Batch.Infrastructure.Repeat.Policy
{
    /// <summary>
    /// Policy for terminating a batch after a fixed number of operations. Internal
    /// state is maintained and a counter incremented, so successful use of this
    /// policy requires that IsComplete() is only called once per batch item. Using
    /// the standard RepeatTemplate should ensure this contract is kept, but it needs
    /// to be carefully monitored.
    /// </summary>
    public class SimpleCompletionPolicy : DefaultResultCompletionPolicy
    {
        /// <summary>
        /// Default chunk size.
        /// </summary>
        public static readonly int DefaultChunkSize = 5;

        #region Attributes
        private int _chunkSize; // 0 is already default value
        
        /// <summary>
        /// Chunk size.
        /// </summary>
        public int ChunkSize { get { return _chunkSize; } set { _chunkSize = value; } }
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public SimpleCompletionPolicy()
            : this(DefaultChunkSize)
        {
        }

        /// <summary>
        /// Custom constructor
        /// </summary>
        /// <param name="chunkSize"></param>
        public SimpleCompletionPolicy(int chunkSize)
        {
            _chunkSize = chunkSize;
        }
        #endregion

        /// <summary>
        ///  Resets the counter.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override IRepeatContext Start(IRepeatContext context)
        {
            return new SimpleTerminationContext(context);
        }

        /// <summary>
        ///  Terminates if the chunk size has been reached, or the result is null.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool IsComplete(IRepeatContext context, RepeatStatus result)
        {
            return base.IsComplete(context, result) ||
                ((SimpleTerminationContext)context).IsComplete(this);
        }

        /// <summary>
        ///  Terminates if the chunk size has been reached.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool IsComplete(IRepeatContext context)
        {
            return ((SimpleTerminationContext)context).IsComplete(this);
        }

        #region SimpleTerminationContext protected class
        /// <summary>
        /// SimpleTerminationContext
        /// </summary>
        protected class SimpleTerminationContext : RepeatContextSupport
        {
            
            /// <summary>
            ///  Custom Constructor.
            /// </summary>
            /// <param name="context"></param>
            public SimpleTerminationContext(IRepeatContext context)
                : base(context)
            {
            }

            /// <summary>
            /// Delegates to RepeatContextSupport#Increment.
            /// </summary>
            public void Update()
            {
                Increment();
            }

            /// <summary>
            /// Tests completion using the owner's chunk size.
            /// </summary>
            /// <param name="owner"></param>
            /// <returns></returns>
            public bool IsComplete(SimpleCompletionPolicy owner)
            {
                return GetStartedCount() >= owner._chunkSize;
            }
        }
        #endregion

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("SimpleCompletionPolicy : chunkSize = {0}" , _chunkSize);
        }
    }
}