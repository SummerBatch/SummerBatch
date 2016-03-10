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

using System.Collections.Generic;

namespace Summer.Batch.Core.Job.Flow
{
    /// <summary>
    /// Interface to be implemented by flows.
    /// </summary>
    public interface IFlow
    {
        /// <summary>
        /// </summary>
        /// <returns>the name of the flow</returns>
        string GetName();

        /// <summary>
        /// Retrieve the <see cref="IState"/> with the given name. If there is no State with the
        /// given name, then return null.
        /// </summary>
        /// <param name="stateName"></param>
        /// <returns></returns>
        IState GetState(string stateName);

        /// <summary>
        /// Starts the flow, using given executor.
        /// </summary>
        /// <param name="executor"></param>
        /// <returns></returns>
        /// <exception cref="FlowExecutionException">&nbsp;</exception>
        FlowExecution Start(IFlowExecutor executor);

        /// <summary>
        /// Resumes the flow.
        /// </summary>
        /// <param name="currentStateName"> the name of the state to resume on</param>
        /// <param name="executor">the context to be passed into each state executed</param>
        /// <returns>a FlowExecution containing the exit status of the flow</returns>
        /// <exception cref="FlowExecutionException">&nbsp;</exception>
        FlowExecution Resume(string currentStateName, IFlowExecutor executor);

        /// <summary>
        ///  Convenient accessor for clients needing to explore the states of this flow.
        /// </summary>
        /// <returns></returns>
        ICollection<IState> GetStates();
    }
}