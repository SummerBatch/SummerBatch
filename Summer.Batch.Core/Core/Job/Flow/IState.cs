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

namespace Summer.Batch.Core.Job.Flow
{
    /// <summary>
    /// Interface to be implemented by flow state.
    /// </summary>
    public interface IState
    {

        /// <summary>
        /// The name of the state. Should be unique within a flow.
        /// </summary>
        /// <returns>The name of the state</returns>
        string GetName();

        /// <summary>
        /// Handle some business or processing logic and return a status that can be
        /// used to drive a flow to the next <see cref="IState"/>. The status can be any
        /// string, but special meaning is assigned to the static constants in
        /// FlowExecution. The context can be used by implementations to do
        /// whatever they need to do. The same context will be passed to all
        /// <see cref="IState"/> instances, so implementations should be careful that the
        /// context is thread-safe, or used in a thread-safe manner.
        /// </summary>
        /// <param name="executor"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        FlowExecutionStatus Handle(IFlowExecutor executor);

        /// <summary>
        /// Inquire as to whether a <see cref="IState"/> is an end state. Implementations
        /// should return false if processing can continue, even if that would
        /// require a restart.
        /// </summary>
        /// <returns> true if this IState is the end of processing</returns>
        bool IsEndState();
    }
}