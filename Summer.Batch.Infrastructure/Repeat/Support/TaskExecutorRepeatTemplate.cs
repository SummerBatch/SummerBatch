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
using Summer.Batch.Common.TaskExecution;
using Summer.Batch.Common.Util;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Summer.Batch.Infrastructure.Repeat.Support
{
    /// <summary>
    /// Support for RepeatOperations, including interceptors (used to modify or monitor 
    /// the behavior at run time).
    /// Thread-safe class, provided that its collaborators are thread-safe as well.
    /// </summary>
    public class TaskExecutorRepeatTemplate : RepeatTemplate
    {
        /// <summary>
        /// Default limit for maximum number of concurrent unfinished results allowed
        /// by the template.
        /// </summary>
        public const int DefaultThrottleLimit = 4;

        private int _throttleLimit = DefaultThrottleLimit;

        /// <summary>
        /// Public setter for the throttle limit. The throttle limit is the largest
        /// number of concurrent tasks that can be executing at one time - if a new
        /// task arrives and the throttle limit is breached we wait for one of the
        /// executing tasks to finish before submitting the new one to the
        /// IExecutor. Default value is DefaultThrottleLimit.
        /// N.B. when used with a thread pooled IExecutor the thread pool
        /// might prevent the throttle limit actually being reached (so make the core
        /// pool size larger than the throttle limit if possible).
        /// </summary>
        public int ThrottleLimit { set { _throttleLimit = value; } }

        private ITaskExecutor _taskExecutor = new SyncTaskExecutor();

        /// <summary>
        ///  Setter for task executor to be used to run the individual item callbacks.
        /// </summary>
        public ITaskExecutor TaskExecutor
        {
            set
            {
                Assert.NotNull(value);
                _taskExecutor = value;
            }
        }

        /// <summary>
        /// see RepeatTemplate#CreateInternalState .
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override IRepeatInternalState CreateInternalState(IRepeatContext context)
        {
            // Queue of pending results:
            return new ResultQueueInternalState(_throttleLimit);
        }

        #region GetNextResult method
        /// <summary>
        /// Use the TaskExecutor to generate a result. The
        /// internal state in this case is a queue of unfinished result holders of
        /// IResultHolder. The holder with the return value should not be
        /// on the queue when this method exits. The queue is scoped in the calling
        /// method so there is no need to synchronize access.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="callback"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        protected override RepeatStatus GetNextResult(IRepeatContext context, RepeatCallback callback,
            IRepeatInternalState state)
        {
            ExecutingRunnable runnable;
            IResultQueue<IResultHolder> queue = ((ResultQueueInternalState)state).ResultQueue;
            do
            {
                // Wrap the callback in a runnable that will add its result to the
                // queue when it is ready.                
                runnable = new ExecutingRunnable(callback, context, queue);

                // Tell the runnable that it can expect a result. This could have
                // been in-lined with the constructor, but it might block, so it's
                // better to do it here, since we have the option (it's a private
                // class).
                runnable.Expect();

                //Start the task possibly concurrently / in the future.
                _taskExecutor.Execute(new Task(delegate { runnable.Run(); }));

                // Allow termination policy to update its state. This must happen
                // immediately before or after the call to the task executor.
                Update(context);

                // Keep going until we get a result that is finished, or early
                // termination...                 
            }
            while (queue.IsEmpty() && !IsComplete(context));

            IResultHolder result = queue.Take();
            if (result.Error != null)
            {
                throw result.Error;
            }
            return result.Result;
        }
        #endregion

        #region WaitForResults method
        /// <summary>
        /// see RepeatTemplate#WaitForResults
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected override bool WaitForResults(IRepeatInternalState state)
        {
            IResultQueue<IResultHolder> queue = ((ResultQueueInternalState)state).ResultQueue;
            bool result = true;
            while (queue.IsExpecting())
            {
                // Careful that no runnables that are not going to finish ever get
                // onto the queue, else this may block forever.                 
                IResultHolder future;
                try
                {
                    future = queue.Take();
                }
                catch (ThreadInterruptedException)
                {
                    Thread.CurrentThread.Interrupt();
                    throw new RepeatException("InterruptedException while waiting for result.");
                }

                if (future.Error != null)
                {
                    state.GetExceptions().Add(future.Error);
                    result = false;
                }
                else
                {
                    RepeatStatus status = future.Result;
                    result = result && CanContinue(status);
                    ExecuteAfterInterceptors(future.Context, status);
                }
            }
            Assert.State(queue.IsEmpty(), "Future results queue should be empty at end of batch.");
            return result;
        }
        #endregion

        #region ExecutingRunnable private class
        private class ExecutingRunnable : IResultHolder
        {
            private readonly RepeatCallback _callback;
            private readonly IResultQueue<IResultHolder> _queue;

            public RepeatStatus Result { get; private set; }
            public System.Exception Error { get; private set; }
            public IRepeatContext Context { get; private set; }

            public ExecutingRunnable(RepeatCallback callback, IRepeatContext context, IResultQueue<IResultHolder> queue)
            {
                _callback = callback;
                Context = context;
                _queue = queue;
            }

            /// <summary>
            /// Tell the queue to expect a result
            /// </summary>
            public void Expect()
            {
                try
                {
                    _queue.Expect();
                }
                catch (ThreadInterruptedException)
                {
                    Thread.CurrentThread.Interrupt();
                    throw new RepeatException("InterruptedException waiting for to acquire lock on input.");
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public void Run()
            {
                bool clearContext = false;
                try
                {
                    if (RepeatSynchronizationManager.GetContext() == null)
                    {
                        clearContext = true;
                        RepeatSynchronizationManager.Register(Context);
                    }
                    if (Logger.IsDebugEnabled)
                    {
                        Logger.Debug("Repeat operation about to start at count= {0}", Context.GetStartedCount());
                    }
                    Result = _callback(Context); //"DoInIteration"
                }
                catch (System.Exception e)
                {
                    Error = e;
                }
                finally
                {
                    if (clearContext)
                    {
                        RepeatSynchronizationManager.Clear();
                    }
                    _queue.Put(this);
                }
            }
        }
        #endregion

        #region ResultQueueInternalState private class
        private class ResultQueueInternalState : RepeatInternalStateSupport, IDisposable
        {
            private IResultQueue<IResultHolder> _results;
            public IResultQueue<IResultHolder> ResultQueue { get { return _results; } }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="throttleLimit"></param>
            public ResultQueueInternalState(int throttleLimit)
            {
                _results = new ResultHolderResultQueue(throttleLimit);
            }

            #region IDisposable
            /// <summary>
            /// see https://msdn.microsoft.com/fr-fr/library/ms244737.aspx
            /// </summary>
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// see https://msdn.microsoft.com/fr-fr/library/ms244737.aspx
            /// </summary>
            /// <param name="disposing"></param>
            private void Dispose(bool disposing)
            {
                if (disposing && _results != null)
                {
                    // free managed resources
                    ((ResultHolderResultQueue)_results).Dispose();
                    _results = null;

                }
            }
        }
            #endregion

        #endregion
    }
}