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
using Summer.Batch.Common.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using Summer.Batch.Common.Factory;

namespace Summer.Batch.Core.Job.Flow.Support
{
    /// <summary>
    /// An <see cref="IFlow"/> that branches conditionally depending on the exit status of
    /// the last <see cref="IState"/>. The input parameters are the state transitions (in no
    /// particular order). The start state name can be specified explicitly (and must
    /// exist in the set of transitions), or computed from the existing transitions,
    /// if unambiguous.
    /// </summary>
    public class SimpleFlow : IFlow, IInitializationPostOperations
    {

        #region Attributes
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Start state property.
        /// </summary>
        public IState StartState { get; private set; }

        private readonly IDictionary<string, ICollection<StateTransition>> _transitionMap = new Dictionary<string, ICollection<StateTransition>>();
        
        /// <summary>
        /// Transitions dictionary
        /// </summary>
        protected IDictionary<string, ICollection<StateTransition>> TransitionMap
        {
            get { return _transitionMap; }
        }
        private readonly IDictionary<string, IState> _stateMap = new Dictionary<string, IState>();
        
        /// <summary>
        /// States dictionary. 
        /// </summary>
        protected IDictionary<string, IState> StateMap { get { return _stateMap; } }
        private List<StateTransition> _stateTransitions = new List<StateTransition>();

        /// <summary>
        /// List of state transitions property.
        /// </summary>
        public List<StateTransition> StateTransitions { set { _stateTransitions = value; } }

        /// <summary>
        /// Name property.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// State transition comparer.
        /// </summary>
        public IComparer<StateTransition> StateTransitionComparer { private get; set; }

        #endregion

        /// <summary>
        /// Performs post-initialization checks.
        /// see IInitializationPostOperations#AfterPropertiesSet .
        /// </summary>
        /// <exception cref="Exception">&nbsp;</exception>
        public void AfterPropertiesSet()
        {
            if (StartState == null)
            {
                InitializeTransitions();
            }
        }


        /// <summary>
        /// Constructor using a name.
        /// </summary>
        /// <param name="name"></param>
        public SimpleFlow(string name)
        {
            Name = name;
        }

        #region IFlow methods

        /// <summary>
        /// @see IFlow#GetName .
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return Name;
        }

        /// <summary>
        /// @see IFlow#GetState .
        /// </summary>
        /// <param name="stateName"></param>
        /// <returns></returns>
        public IState GetState(string stateName)
        {
            IState result;
            _stateMap.TryGetValue(stateName, out result);
            return result;
        }

        /// <summary>
        /// @see IFlow#Start .
        /// </summary>
        /// <param name="executor"></param>
        /// <returns></returns>
        /// <exception cref="FlowExecutionException">&nbsp;</exception>
        public FlowExecution Start(IFlowExecutor executor)
        {
            if (StartState == null)
            {
                InitializeTransitions();
            }
            IState state = StartState;
            string stateName = state.GetName();
            return Resume(stateName, executor);
        }

        /// <summary>
        ///  @see IFlow#Resume .
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="executor"></param>
        /// <returns></returns>
        /// <exception cref="FlowExecutionException">&nbsp;</exception>
        public FlowExecution Resume(string stateName, IFlowExecutor executor)
        {
            FlowExecutionStatus status = FlowExecutionStatus.Unkown;
            IState state;
            _stateMap.TryGetValue(stateName, out state);

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Resuming state={0} with status={1}", stateName, status);
            }
            StepExecution stepExecution = null;

            String currentStateName = stateName;
            // Terminate if there are no more states
            while (IsFlowContinued(state, status, stepExecution))
            {
                currentStateName = state.GetName();
                try
                {
                    if (Logger.IsDebugEnabled)
                    {
                        Logger.Debug("Handling state={0}", currentStateName);
                    }
                    status = state.Handle(executor);
                    stepExecution = executor.GetStepExecution();
                }
                catch (FlowExecutionException)
                {
                    executor.Close(new FlowExecution(currentStateName, status));
                    throw;
                }
                catch (Exception e)
                {
                    executor.Close(new FlowExecution(currentStateName, status));
                    throw new FlowExecutionException(
                        string.Format("Ended flow={0} at state={1} with exception", Name, currentStateName), e);
                }
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Completed state={0} with status={1}", currentStateName, status);
                }
                state = NextState(currentStateName, status, stepExecution);
            }
            FlowExecution result = new FlowExecution(currentStateName, status);
            executor.Close(result);
            return result;
        }

        /// <summary>
        /// @see IFlow#GetStates .
        /// </summary>
        /// <returns></returns>
        public ICollection<IState> GetStates()
        {
            return new HashSet<IState>(_stateMap.Values);
        }
        #endregion

        #region Utility methods


        /// <summary>
        /// Returns the next <see cref="IStep"/> (or null if this is the end).
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="status"></param>
        /// <param name="stepExecution"></param>
        /// <returns></returns>
        /// <exception cref="FlowExecutionException">&nbsp;</exception>
        protected IState NextState(string stateName, FlowExecutionStatus status, StepExecution stepExecution)
        {
            ICollection<StateTransition> set;
            _transitionMap.TryGetValue(stateName, out set);

            if (set == null)
            {
                throw new FlowExecutionException(
                    string.Format("No transitions found in flow={0} for state={1}", GetName(), stateName));
            }

            string next = null;
            string exitCode = status.Name;

            foreach (StateTransition stateTransition in set)
            {
                if (stateTransition.Matches(exitCode) || 
                    (exitCode.Equals("PENDING") && stateTransition.Matches("STOPPED")))
                {
                    if (stateTransition.IsEnd())
                    {
                        // End of job
                        return null;
                    }
                    next = stateTransition.Next;
                    break;
                }
            }

            if (next == null)
            {
                throw new FlowExecutionException(
                    string.Format("Next state not found in flow={0} for state={1} with exit status={2}",
                    GetName(), stateName, status.Name));
            }

            if (!_stateMap.ContainsKey(next))
            {
                throw new FlowExecutionException(
                    string.Format("Next state not specified in flow={0} for next={1}", GetName(), next));
            }

            //Getting here ensures the _stateMap contains the next Key (protected by the prior throw)
            return _stateMap[next];

        }

        /// <summary>
        /// Tests if flow is continued.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="status"></param>
        /// <param name="stepExecution"></param>
        /// <returns></returns>
        protected bool IsFlowContinued(IState state, FlowExecutionStatus status, StepExecution stepExecution)
        {
            var continued = state != null && !status.Equals(FlowExecutionStatus.Stopped);

            if (stepExecution != null)
            {
                bool? reRun = (bool?)stepExecution.ExecutionContext.Get("batch.restart");
                bool? executed = (bool?)stepExecution.ExecutionContext.Get("batch.executed");

                if ((executed == null || !executed.Value) && reRun != null && reRun.Value
                    && status.Equals(FlowExecutionStatus.Stopped) && !state.GetName().EndsWith(stepExecution.StepName))
                {
                    continued = true;
                }
            }
            return continued;
        }

        /// <summary>
        /// Analyse the transitions provided and generate all the information needed to execute the flow.
        /// </summary>
        /// <exception cref="ArgumentException">&nbsp;</exception>
        private void InitializeTransitions()
        {
            StartState = null;
            _transitionMap.Clear();
            _stateMap.Clear();
            bool hasEndStep = false;

            if (!_stateTransitions.Any())
            {
                throw new ArgumentException("No start state was found. You must specify at least one step in a job.");
            }

            foreach (StateTransition stateTransition in _stateTransitions)
            {
                IState state = stateTransition.State;
                string stateName = state.GetName();
                _stateMap[stateName] = state;
            }

            foreach (StateTransition stateTransition in _stateTransitions)
            {
                IState state = stateTransition.State;
                if (!stateTransition.IsEnd())
                {
                    string next = stateTransition.Next;
                    if (!_stateMap.ContainsKey(next))
                    {
                        throw new ArgumentException(
                            string.Format("Missing state for [{0}@]", stateTransition));
                    }
                }
                else
                {
                    hasEndStep = true;
                }
                string name = state.GetName();
                ICollection<StateTransition> set;
                _transitionMap.TryGetValue(name, out set);

                if (set == null)
                {
                    // If no comparer is provided, we will maintain the order of insertion
                    if (StateTransitionComparer == null)
                    {
                        set = new OrderedSet<StateTransition>();
                    }
                    else
                    {
                        set = new SortedSet<StateTransition>(StateTransitionComparer);
                    }

                    _transitionMap.Add(name, set);
                }
                set.Add(stateTransition);
            }

            if (!hasEndStep)
            {
                throw new ArgumentException("No end state was found.  You must specify at least one transition with no next state.");
            }
            StartState = _stateTransitions[0].State;
        }
        #endregion
    }
}