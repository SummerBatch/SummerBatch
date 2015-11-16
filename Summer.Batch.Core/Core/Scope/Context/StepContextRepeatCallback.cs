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
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

//   This file has been modified.
//   Original copyright notice :

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

using NLog;
using Summer.Batch.Infrastructure.Repeat;
using Summer.Batch.Common.Util;
using System;
using System.Collections.Concurrent;

namespace Summer.Batch.Core.Scope.Context
{
    /// <summary>
    /// Do the work required for this chunk of the step. The ChunkContext
    /// provided is managed by the base class, so that if there is still work to
    /// do for the task in hand state can be stored here. In a multi-threaded
    /// client, the base class ensures that only one thread at a time can be
    /// working on each instance of ChunkContext} Workers should signal
    /// that they are finished with a context by removing all the attributes they
    /// have added. If a worker does not remove them another thread might see
    /// stale state.
    /// NOTE : moved from abstract method to delegate 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="chunkContext"></param>
    /// <exception cref="Exception"></exception>
    /// <returns></returns>
    public delegate RepeatStatus DoInChunkContext(IRepeatContext context, ChunkContext chunkContext);

    /// <summary>
    /// Convenient base class for clients who need to do something in a repeat callback inside a IStep.
    /// </summary>
    public static class StepContextRepeatCallback
    {
        /// <summary>
        /// Manage the StepContext lifecycle. Business processing should be
        /// delegated to #DoInChunkContext(RepeatContext, ChunkContext). This
        /// is to ensure that the current thread has a reference to the context, even
        /// if the callback is executed in a pooled thread. Handles the registration
        /// and unregistration of the step context, so clients should not duplicate
        /// those calls.
        /// </summary>
        /// <param name="stepExecution"></param>
        /// <param name="doInChunkContext"></param>
        /// <returns></returns>
        public static RepeatCallback GetRepeatCallback(StepExecution stepExecution, DoInChunkContext doInChunkContext)
        {
            BlockingCollection<ChunkContext> attributeQueue = new BlockingCollection<ChunkContext>();
            return context =>
            {
                // The StepContext has to be the same for all chunks,
                // otherwise step-scoped beans will be re-initialised for each chunk.
                StepContext stepContext = StepSynchronizationManager.Register(stepExecution);
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Preparing chunk execution for StepContext: {0}",
                                 ObjectUtils.IdentityToString(stepContext));
                }

                ChunkContext chunkContext;
                attributeQueue.TryTake(out chunkContext);
                if (chunkContext == null)
                {
                    chunkContext = new ChunkContext(stepContext);
                }
                try
                {
                    Logger.Debug("Chunk execution starting: queue size= {0}", attributeQueue.Count);
                    return doInChunkContext(context, chunkContext); //Delegation
                }
                finally
                {
                    // Still some stuff to do with the data in this chunk,
                    // pass it back.
                    if (!chunkContext.Complete)
                    {
                        attributeQueue.Add(chunkContext);
                    }
                    StepSynchronizationManager.Close();
                }
            };
        }

        /// <summary>
        /// Logger
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    }
}
