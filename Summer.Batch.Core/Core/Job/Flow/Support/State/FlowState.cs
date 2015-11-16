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
using System.Collections.ObjectModel;

namespace Summer.Batch.Core.Job.Flow.Support.State
{
    /// <summary>
    /// State that delegates to an <see cref="IFlow"/>.
    /// </summary>
    public class FlowState : AbstractState, IFlowHolder
    {
        private readonly IFlow _flow;

        #region Constructors
        /// <summary>
        /// Custom constructor using a name.
        /// </summary>
        /// <param name="name"></param>
        public FlowState(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Custom constructor using a flow and a name.
        /// </summary>
        /// <param name="flow"></param>
        /// <param name="name"></param>
        public FlowState(IFlow flow, string name)
            : base(name)
        {
            _flow = flow;
        }
        #endregion

        #region IState methods implementation
        /// <summary>
        /// @see IState#Handle .
        /// </summary>
        /// <param name="executor"></param>
        /// <returns></returns>
        public override FlowExecutionStatus Handle(IFlowExecutor executor)
        {
            return _flow.Start(executor).Status;
        }

        /// <summary>
        /// @see IState#IsEndState .
        /// </summary>
        /// <returns></returns>
        public override bool IsEndState()
        {
            return false;
        }
        #endregion

        #region IFlowHolder methods implementation
        /// <summary>
        /// @see IFlowHolder#GetFlows .
        /// </summary>
        /// <returns></returns>
        public ICollection<IFlow> GetFlows()
        {
            return new ReadOnlyCollection<IFlow>(new[] { _flow });
        }
        #endregion
    }
}