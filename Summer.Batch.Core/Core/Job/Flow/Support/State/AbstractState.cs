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

using System;

namespace Summer.Batch.Core.Job.Flow.Support.State
{
    /// <summary>
    /// Abstract State.
    /// Base class to implement the <see cref="IState"/> interface.
    /// </summary>
    public abstract class AbstractState : IState
    {
        private readonly string _name;

        /// <summary>
        /// @see IState#GetName .
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return _name;
        }

        /// <summary>
        /// Custom constructor using a name.
        /// </summary>
        /// <param name="name"></param>
        protected AbstractState(string name)
        {
            _name = name;
        }

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} name=[{1}]", GetType().Name, _name);
        }

        /// <summary>
        /// @see IState#Handle .
        /// </summary>
        /// <param name="executor"></param>
        /// <returns></returns>
        /// <exception cref="Exception">&nbsp;</exception>
        public abstract FlowExecutionStatus Handle(IFlowExecutor executor);

        /// <summary>
        /// @see IState#IsEndState .
        /// </summary>
        /// <returns></returns>
        public abstract bool IsEndState();

    }
}