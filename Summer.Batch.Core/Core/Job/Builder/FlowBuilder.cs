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
using Summer.Batch.Common.TaskExecution;
using Summer.Batch.Core.Job.Flow;
using Summer.Batch.Core.Job.Flow.Support;
using Summer.Batch.Core.Job.Flow.Support.State;

namespace Summer.Batch.Core.Job.Builder
{
    /// <summary>
    /// A builder for a flow of steps that can be executed as a job or as part of a job. Steps can be linked together with
    /// conditional transitions that depend on the exit status of the previous step.
    /// </summary>
    /// <typeparam name="T">&nbsp;The type of object returned by the builder (by default a Flow).</typeparam>
    public class FlowBuilder<T>
    {
        private readonly string _name;
        private readonly string _prefix;
        private readonly List<StateTransition> _transitions = new List<StateTransition>();
        private readonly IDictionary<string, IState> _tos = new Dictionary<string, IState>();
        private IState _currentState;
        private readonly EndState _failedState;
        private readonly EndState _completedState;
        private readonly EndState _stoppedState;
        private int _splitCounter ;//def. to 0
        private int _endCounter;//def. to 0
        private readonly IDictionary<object, IState> _states = new Dictionary<object, IState>();
        private SimpleFlow _flow;
        private bool _dirty = true;

        /// <summary>
        /// Custom constructor using a name.
        /// </summary>
        /// <param name="name"></param>
        public FlowBuilder(string name)
        {
            _name = name;
            _prefix = name + ".";
            _failedState = new EndState(FlowExecutionStatus.Failed, _prefix + "FAILED");
            _completedState = new EndState(FlowExecutionStatus.Completed, _prefix + "COMPLETED");
            _stoppedState = new EndState(FlowExecutionStatus.Stopped, _prefix + "STOPPED");
        }

        /// <summary>
        /// Validates the current state of the builder and build a flow. 
        /// Subclasses may override this to build an object of a
        /// different type that itself depends on the flow.
        /// </summary>
        /// <returns></returns>
        public virtual T Build()
        {
            T result = (T) Flow();
            return result;
        }

        /// <summary>
        /// Transition to the next step on completion of the current step, except if the current step has failed.
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        public FlowBuilder<T> Next(IStep step)
        {
            DoNext(step);
            return this;
        }

        /// <summary>
        /// Start a flow. If some steps are already registered, just a synonym for From(IStep).
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        public FlowBuilder<T> Start(IStep step)
        {
            DoStart(step);
            return this;
        }

        /// <summary>
        /// Go back to a previously registered step and start a new path. 
        /// If no steps are registered yet just a synonym for Start(IStep).
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        public FlowBuilder<T> From(IStep step)
        {
            DoFrom(step);
            return this;
        }

        /// <summary>
        /// Go next on successful completion to a subflow.
        /// </summary>
        /// <param name="flow"></param>
        /// <returns></returns>
        public FlowBuilder<T> Next(IFlow flow)
        {
            DoNext(flow);
            return this;
        }

        /// <summary>
        /// If a flow should start with a subflow use this as the first state.
        /// </summary>
        /// <param name="flow"></param>
        /// <returns></returns>
        public FlowBuilder<T> Start(IFlow flow)
        {
            DoStart(flow);
            return this;
        }

        /// <summary>
        /// Starts again from a subflow that was already registered.
        /// </summary>
        /// <param name="flow"></param>
        /// <returns></returns>
        public FlowBuilder<T> From(IFlow flow)
        {
            DoFrom(flow);
            return this;
        }

        /// <summary>
        /// Splits the flow.
        /// </summary>
        /// <param name="executor">a task executor to execute the split flows</param>
        /// <returns></returns>
        public SplitBuilder Split(ITaskExecutor executor)
        {
            return new SplitBuilder(this, executor);
        }

        /// <summary>
        /// Starts a transition to a new state if the exit status from the previous state matches the pattern given.
        /// Successful completion normally results in an exit status equal to (or starting with by convention) "COMPLETED".
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public TransitionBuilder On(string pattern)
        {
            return new TransitionBuilder(this, pattern);
        }

        /// <summary>
        /// A synonym for Build which callers might find useful. Subclasses can override build to create an object
        /// of the desired type (e.g. a parent builder or an actual flow).
        /// </summary>
        /// <returns></returns>
        public T End()
        {
            return Build();
        }

        /// <summary>
        /// Builds a new flow that will be returned by <see cref="Build"/>.
        /// </summary>
        /// <returns>A new <see cref="SimpleFlow"/>.</returns>
        protected IFlow Flow()
        {
            if (!_dirty)
            {
                // optimization in case this method is called consecutively
                return _flow;
            }
            _flow = new SimpleFlow(_name);
            // optimization for flows that only have one state that itself is a flow:
            var state = _currentState as FlowState;
            if (state != null && _states.Count() == 1)
            {
                return state.GetFlows().First();
            }
            AddDanglingEndStates();
            _flow.StateTransitions = _transitions;
            _dirty = false;
            return _flow;
        }

        private void DoNext(object input)
        {
            if (_currentState == null)
            {
                DoStart(input);
            }
            IState next = CreateState(input);
            AddTransition("FAILED", _failedState);
            AddTransition("*", next);
            _currentState = next;
        }

        private void DoStart(object input)
        {
            if (_currentState != null)
            {
                DoFrom(input);
            }
            _currentState = CreateState(input);
        }

        private void DoFrom(object input)
        {
            if (_currentState == null)
            {
                DoStart(input);
            }
            IState state = CreateState(input);
            _tos[_currentState.GetName()] = _currentState;
            _currentState = state;
        }

        private IState CreateState(object input)
        {
            IState result;
            var step1 = input as IStep;
            if (step1 != null)
            {
                if (!_states.ContainsKey(step1))
                {
                    IStep step = step1;
                    _states[input] = new StepState(step, _prefix + step.Name);
                }
                result = _states[input];
            }
            else
            {
                var flow1 = input as IFlow;
                if (flow1 != null)
                {
                    if (!_states.ContainsKey(flow1))
                    {
                        IFlow flow = flow1;
                        _states[input] = new FlowState(flow, _prefix + flow.GetName());
                    }
                    result = _states[input];
                }
                else
                {
                    throw new FlowBuilderException("No state can be created for: " + input);
                }
            }
            _dirty = true;
            return result;
        }

        private SplitState CreateState(ICollection<IFlow> flows, ITaskExecutor executor)
        {
            if (!_states.ContainsKey(flows))
            {
                _states[flows] = new SplitState(flows, _prefix + "split" + _splitCounter++);
            }
            SplitState result = (SplitState)_states[flows];
            if (executor != null)
            {
                result.TaskExecutor = executor;
            }
            _dirty = true;
            return result;
        }

        private void AddDanglingEndStates()
        {
            ISet<string> froms = new HashSet<string>();
            foreach (StateTransition transition in _transitions)
            {
                froms.Add(transition.State.GetName());
            }
            if (!_tos.Any() && _currentState != null)
            {
                _tos[_currentState.GetName()] = _currentState;
            }
            IDictionary<string, IState> copy = new Dictionary<string, IState>(_tos);
            // Find all the states that are really end states but not explicitly declared as such
            FindEndStates(copy, froms);
            
            copy = new Dictionary<string, IState>(_tos);
            // Then find the states that do not have a default transition
            FindEndStatesWithoutDefaultTransition(copy);
        }

        private void FindEndStatesWithoutDefaultTransition(IDictionary<string, IState> copy)
        {
            foreach (string from in copy.Keys)
            {
                _currentState = copy[@from];
                if (!_currentState.IsEndState())
                {
                    if (!HasFail(@from))
                    {
                        AddTransition("*", _failedState);
                    }
                    if (!HasCompleted(@from))
                    {
                        AddTransition("*", _completedState);
                    }
                }
            }
        }

        private void FindEndStates(IDictionary<string, IState> copy, ISet<string> froms)
        {
            foreach (string to in copy.Keys)
            {
                if (!froms.Contains(to))
                {
                    _currentState = copy[to];
                    if (!_currentState.IsEndState())
                    {
                        AddTransition("FAILED", _failedState);
                        AddTransition("*", _completedState);
                    }
                }
            }
        }

        private bool HasFail(string from)
        {
            return Matches(from, "FAILED");
        }

        private bool HasCompleted(string from)
        {
            return Matches(from, "COMPLETED");
        }

        private bool Matches(string from, string status)
        {
            return _transitions.Any(transition => @from.Equals(transition.State.GetName()) && transition.Matches(status));
        }

        private void AddTransition(string pattern, IState next)
        {
            _tos[next.GetName()] = next;
            _transitions.Add(StateTransition.CreateStateTransition(_currentState, pattern, next.GetName()));
            if (_transitions.Count == 1)
            {
                _transitions.Add(StateTransition.CreateEndStateTransition(_failedState));
                _transitions.Add(StateTransition.CreateEndStateTransition(_completedState));
                _transitions.Add(StateTransition.CreateEndStateTransition(_stoppedState));
            }
            if (next.IsEndState())
            {
                _transitions.Add(StateTransition.CreateEndStateTransition(next));
            }
            _dirty = true;
        }

        private void Stop(string pattern)
        {
            AddTransition(pattern, _stoppedState);
        }

        private void Stop(string pattern, IState restart)
        {
            EndState next = new EndState(FlowExecutionStatus.Stopped, "STOPPED", _prefix + "stop" + _endCounter++, true);
            AddTransition(pattern, next);
            _currentState = next;
            AddTransition("*", restart);
        }

        private void End(string pattern)
        {
            AddTransition(pattern, _completedState);
        }

        private void End(string pattern, string code)
        {
            AddTransition(pattern, new EndState(FlowExecutionStatus.Completed, code, _prefix + "end" + _endCounter++, true));
        }

        private void Fail(string pattern)
        {
            AddTransition(pattern, _failedState);
        }

        /// <summary>
        /// A builder for transitions within a flow.
        /// </summary>
        public class TransitionBuilder
        {
            private readonly FlowBuilder<T> _parent;
            private readonly string _pattern;

            /// <summary>
            /// Custom constructor using parent and pattern.
            /// </summary>
            /// <param name="parent"></param>
            /// <param name="pattern"></param>
            public TransitionBuilder(FlowBuilder<T> parent, string pattern)
            {
                _parent = parent;
                _pattern = pattern;
            }

            /// <summary>
            /// Specify the next step and return a FlowBuilder for chaining.
            /// </summary>
            /// <param name="step"></param>
            /// <returns></returns>
            public FlowBuilder<T> To(IStep step)
            {
                IState next = _parent.CreateState(step);
                _parent.AddTransition(_pattern, next);
                _parent._currentState = next;
                return _parent;
            }

            /// <summary>
            /// Specify the next state as a complete flow and return a FlowBuilder for chaining.
            /// </summary>
            /// <param name="flow"></param>
            /// <returns></returns>
            public FlowBuilder<T> To(IFlow flow)
            {
                IState next = _parent.CreateState(flow);
                _parent.AddTransition(_pattern, next);
                _parent._currentState = next;
                return _parent;
            }

            /// <summary>
            /// Signal the successful end of the flow and return a FlowBuilder for chaining.
            /// </summary>
            /// <returns></returns>
            public FlowBuilder<T> Stop()
            {
                _parent.Stop(_pattern);
                return _parent;
            }

            /// <summary>
            /// Stop and Restart flow and return a FlowBuilder for chaining.
            /// </summary>
            /// <param name="flow"></param>
            /// <returns></returns>
            public FlowBuilder<T> StopAndRestart(IFlow flow)
            {
                IState next = _parent.CreateState(flow);
                _parent.Stop(_pattern, next);
                return _parent;
            }

            /// <summary>
            /// Stop the flow and provide a decider to start with if the flow is restarted.
            /// Return a FlowBuilder for chaining.
            /// </summary>
            /// <param name="step"></param>
            /// <returns></returns>
            public FlowBuilder<T> StopAndRestart(IStep step)
            {
                IState next = _parent.CreateState(step);
                _parent.Stop(_pattern, next);
                return _parent;
            }

            /// <summary>
            /// End and return a FlowBuilder for chaining.
            /// </summary>
            /// <returns></returns>
            public FlowBuilder<T> End()
            {
                _parent.End(_pattern);
                return _parent;
            }

            /// <summary>
            /// Signal the successful end of the flow and return a FlowBuilder for chaining.
            /// </summary>
            /// <param name="status"></param>
            /// <returns></returns>
            public FlowBuilder<T> End(string status)
            {
                _parent.End(_pattern, status);
                return _parent;
            }

            /// <summary>
            /// Signal the end of the flow with an error condition return a FlowBuilder for chaining.
            /// </summary>
            /// <returns></returns>
            public FlowBuilder<T> Fail()
            {
                _parent.Fail(_pattern);
                return _parent;
            }
        }

        /// <summary>
        /// A builder for building a split state.
        /// </summary>
        public class SplitBuilder
        {
            private readonly FlowBuilder<T> _parent;
            private readonly ITaskExecutor _executor;

            /// <summary>
            /// Custom constructor with parent and executor
            /// </summary>
            /// <param name="parent"></param>
            /// <param name="executor"></param>
            public SplitBuilder(FlowBuilder<T> parent, ITaskExecutor executor)
            {
                _parent = parent;
                _executor = executor;
            }

            /// <summary>
            /// Add flows to the split, in addition to the current state already present in the parent builder.
            /// </summary>
            /// <param name="flows"> more flows to add to the split</param>
            /// <returns>the parent builder</returns>
            public FlowBuilder<T> Add(IFlow[] flows)
            {
                ICollection<IFlow> list = new List<IFlow>(flows);
                var name = "split" + _parent._splitCounter++;
                IState one = _parent._currentState;
                IFlow flow = null;
                if (!(one is FlowState))
                {
                    FlowBuilder<IFlow> stateBuilder = new FlowBuilder<IFlow>(name + "_0") {_currentState = one};
                    flow = stateBuilder.Build();
                }
                else if (_parent._states.Count == 1)
                {
                    list.Add(((FlowState) one).GetFlows().First());
                }

                if (flow != null)
                {
                    list.Add(flow);
                }
                IState next = _parent.CreateState(list, _executor);
                _parent._currentState = next;
                return _parent;
            }

        }
    }
}