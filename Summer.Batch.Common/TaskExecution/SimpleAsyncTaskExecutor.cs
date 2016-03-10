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

// This file has been modified.
// Original copyright notice :

/*
 * Copyright 2002-2012 the original author or authors.
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
using System.Threading.Tasks;
using Summer.Batch.Common.Util;

namespace Summer.Batch.Common.TaskExecution
{
    /// <summary>
    /// ITaskExecutor asynchronous implementation.
    /// Supports limiting concurrent threads through the "concurrencyLimit"
    /// bean property. By default, the number of concurrent threads is unlimited.
    /// NOTE: This implementation does not reuse threads! Consider a
    /// thread-pooling TaskExecutor implementation instead, in particular for
    /// executing a large number of short-lived tasks.
    /// 
    /// </summary>
    [Serializable]
    public class SimpleAsyncTaskExecutor : IAsyncTaskExecutor
    {
        /// <summary>
        /// Internal concurrency throttle used by this executor.
        /// </summary>
        private static readonly ConcurrencyThrottleAdapter ConcurrencyThrottle
            = new ConcurrencyThrottleAdapter();

        /// <summary>
        /// Return whether owned throttle is currently active.
        /// </summary>
        /// <returns></returns>
        public bool IsThrottleActive()
        {
            return ConcurrencyThrottle.IsThrottleActive();
        }

        /// <summary>
        /// Template method for the actual execution of a task.
        /// The default implementation starts the task.
        /// </summary>
        /// <param name="task">The task to execute.</param>
        protected void DoExecute(Task task)
        {
            task.Start();
        }

        /// <summary>
        /// Executes the given task.
        /// </summary>
        /// <param name="task">The task to execute.</param>
        /// <param name="startTimeout">The time duration ( inmilliseconds) within which the task is
        /// supposed to start. This is intended as a hint to the executor, allowing for
        /// preferred handling of immediate tasks.</param>
        /// <exception cref="TaskRejectedException">&nbsp;</exception>
        /// <exception cref="TaskTimeoutException">&nbsp;in case of the task being rejected because
        /// of the timeout (i.e. it cannot be started in time)</exception>
        /// <seealso cref="AsyncTaskExecutorConstants.TimeoutImmediate"/>
        /// <seealso cref="AsyncTaskExecutorConstants.TimeoutIndefinite"/>
        public void Execute(Task task, long startTimeout)
        {
            Assert.NotNull(task, "Runnable must not be null");
            if (IsThrottleActive() && startTimeout > AsyncTaskExecutorConstants.TimeoutImmediate)
            {
                ConcurrencyThrottle.BeforeAccess();
                DoExecute(GetConcurrencyThrottlingTask(task));
            }
            else
            {
                DoExecute(task);
            }
        }

        /// <summary>
        /// Submits a task for execution.
        /// </summary>
        /// <param name="task">The task to execute.</param>
        /// <exception cref="TaskRejectedException">&nbsp;If the given task was not accepted.</exception>
        public void Submit(Task task)
        {
            Execute(task, AsyncTaskExecutorConstants.TimeoutIndefinite);
        }

        /// <summary>
        /// Executes the given task.
        /// The call might return immediately if the implementation uses an asynchronous execution strategy, 
        /// or might block in the case of synchronous execution. 
        /// </summary>
        /// <param name="task">the Task to execute (never null).</param>
        /// <exception cref="TaskRejectedException">&nbsp;If the given task was not accepted.</exception>
        public void Execute(Task task)
        {
            Execute(task, AsyncTaskExecutorConstants.TimeoutIndefinite);
        }

        // Returns a task that will execute the specified task
        // and call AfterAccess once it finished.
        private static Task GetConcurrencyThrottlingTask(Task target)
        {
            return new Task(() =>
            {
                try
                {
                    target.RunSynchronously();
                }
                finally
                {
                    ConcurrencyThrottle.AfterAccess();
                }
            });
        }

        #region ConcurrencyThrottleAdapter private class
        /// <summary>
        ///Subclass of the general ConcurrencyThrottleSupport class,
        /// making BeforeAccess() and AfterAccess() visible to the surrounding class. 
        /// </summary>
        private class ConcurrencyThrottleAdapter : ConcurrencyThrottleSupport
        {
            public new void BeforeAccess()
            {
                base.BeforeAccess();
            }
            public new void AfterAccess()
            {
                base.AfterAccess();
            }
        }
        #endregion
    }
}
