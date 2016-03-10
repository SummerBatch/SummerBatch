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

using System.Threading.Tasks;

namespace Summer.Batch.Common.TaskExecution
{

    /// <summary>
    /// Extended interface for asynchronous ITaskExecutor implementations,
    /// offering an overloaded #Execute(Task, long) variant with a start
    /// timeout parameter.
    /// </summary>
    public interface IAsyncTaskExecutor : ITaskExecutor
    {
        /// <summary>
        /// Executes the given task.
        /// </summary>
        /// <param name="task">The task to execute.</param>
        /// <param name="startTimeout">The time duration ( inmilliseconds) within which the task is
        /// supposed to start. This is intended as a hint to the executor, allowing for
        /// preferred handling of immediate tasks.</param>
        /// <exception cref="TaskRejectedException">&nbsp;If the given task was not accepted.</exception>
        /// <exception cref="TaskTimeoutException">&nbsp;If the task being rejected because
        /// of the timeout (i.e., it cannot be started in time).</exception>
        /// <seealso cref="AsyncTaskExecutorConstants.TimeoutImmediate"/>
        /// <seealso cref="AsyncTaskExecutorConstants.TimeoutIndefinite"/>
        void Execute(Task task, long startTimeout);
        
        /// <summary>
        /// Submits a task for execution.
        /// </summary>
        /// <param name="task">The task to execute.</param>
        /// <exception cref="TaskRejectedException">&nbsp;If the given task was not accepted.</exception>
        void Submit(Task task);
    }
}
