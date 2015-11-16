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
using System.Linq;
using System.Threading.Tasks;
using Summer.Batch.Common.TaskExecution;

namespace Summer.Batch.Core.Job.Flow.Support.State
{
    /// <summary>
    /// A <see cref="IState"/> implementation that splits an <see cref="IFlow"/> into multiple parallel subflows.
    /// </summary>
    public class SplitState : AbstractState, IFlowHolder
    {
        private readonly ICollection<IFlow> _flows;
        private ITaskExecutor _taskExecutor = new SyncTaskExecutor();
        
        /// <summary>
        /// Task executor property.
        /// </summary>
        public ITaskExecutor TaskExecutor { set { _taskExecutor = value; } }
        private readonly IFlowExecutionAggregator _aggregator = new MaxValueFlowExecutionAggregator();

        #region Constructors
        /// <summary>
        /// Custom constructor using a name.
        /// </summary>
        /// <param name="name"></param>
        public SplitState(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Custom constructor using flows collection and a name.
        /// </summary>
        /// <param name="flows"></param>
        /// <param name="name"></param>
        public SplitState(ICollection<IFlow> flows, string name)
            : this(name)
        {
            _flows = flows;
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
            ICollection<Task<FlowExecution>> tasks = new List<Task<FlowExecution>>();

            foreach (IFlow flow in _flows)
            {
                IFlow aFlow = flow;
                Task<FlowExecution> task = new Task<FlowExecution>(() => aFlow.Start(executor));
                tasks.Add(task);
                try
                {
                    _taskExecutor.Execute(task);
                }
                catch (TaskRejectedException)
                {
                    throw new FlowExecutionException("TaskExecutor rejected task for flow=" + flow.GetName());
                }
            }
            ICollection<FlowExecution> results = tasks.Select(task => task.Result).ToList();

            return DoAggregation(results, executor);
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
            return _flows;
        }
        #endregion

        /// <summary>
        /// Do Aggregation.
        /// </summary>
        /// <param name="results"></param>
        /// <param name="executor"></param>
        /// <returns></returns>
        protected FlowExecutionStatus DoAggregation(ICollection<FlowExecution> results, IFlowExecutor executor)
        {
            return _aggregator.Aggregate(results);
        }
    }
}