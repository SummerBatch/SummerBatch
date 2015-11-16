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

using Summer.Batch.Infrastructure.Support;
using Summer.Batch.Common.Util;
using System;

namespace Summer.Batch.Core.Job.Flow.Support
{
    /// <summary>
    /// Value object representing a potential transition from one <see cref="IState"/> to
    /// another. The originating IState name and the next IState to execute are
    /// linked by a pattern for the ExitStatus#ExitCode exit code of an
    /// execution of the originating IState.
    /// </summary>
    public class StateTransition
    {
        /// <summary>
        /// State property.
        /// </summary>
        public IState State { get; private set; }

        /// <summary>
        /// Pattern property.
        /// </summary>
        public string Pattern { get; private set; }

        /// <summary>
        /// Next state name property.
        /// </summary>
        public string Next { get; private set; }

        #region Create Method variants
        /// <summary>
        /// Create a new end state <see cref="StateTransition"/> specification. This
        /// transition explicitly goes unconditionally to an end state (i.e. no more
        /// executions).
        /// </summary>
        /// <param name="state">the IState used to generate the outcome for this transition</param>
        /// <returns></returns>
        public static StateTransition CreateEndStateTransition(IState state)
        {
            return CreateStateTransition(state, null, null);
        }

        /// <summary>
        /// Create a new end state <see cref="StateTransition"/> specification. This
        /// transition explicitly goes to an end state (i.e. no more processing) if
        /// the outcome matches the pattern.
        /// </summary>
        /// <param name="state">the IState used to generate the outcome for this transition</param>
        /// <param name="pattern">the pattern to match in the exit status of the IState</param>
        /// <returns></returns>
        public static StateTransition CreateEndStateTransition(IState state, string pattern)
        {
            return CreateStateTransition(state, pattern, null);
        }

        /// <summary>
        /// Convenience method to switch the origin and destination of a transition,
        /// creating a new instance.
        /// </summary>
        /// <param name="stateTransition">an existing state transition</param>
        /// <param name="state">the new state for the origin</param>
        /// <param name="next">the new name for the destination</param>
        /// <returns></returns>
        public static StateTransition SwitchOriginAndDestination(StateTransition stateTransition, IState state, string next)
        {
            return CreateStateTransition(state, stateTransition.Pattern, next);
        }

        /// <summary>
        /// Create a new state IStateTransition specification with a wildcard pattern that matches all outcomes.
        /// </summary>
        /// <param name="state">the IState used to generate the outcome for this transition</param>
        /// <param name="next">the name of the next IState to execute</param>
        /// <returns></returns>
        public static StateTransition CreateStateTransition(IState state, string next)
        {
            return CreateStateTransition(state, null, next);
        }

        /// <summary>
        /// Create a new StateTransition specification from one IState to another (by name).
        /// </summary>
        /// <param name="state">the IState used to generate the outcome for this transition</param>
        /// <param name="pattern">the pattern to match in the exit status of the IState</param>
        /// <param name="next">the name of the next IState to execute</param>
        /// <returns></returns>
        public static StateTransition CreateStateTransition(IState state, string pattern, string next)
        {
            return new StateTransition(state, pattern, next);
        }
        #endregion

        #region Constructor
        /// <summary>
        /// private all purpose constructor.
        /// All create methods delegate to this.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="pattern"></param>
        /// <param name="next"></param>
        private StateTransition(IState state, string pattern, string next)
        {
            Pattern = string.IsNullOrWhiteSpace(pattern) ? "*" : pattern;

            Assert.NotNull(state, "A state is required for a StateTransition");
            if (state.IsEndState() && !string.IsNullOrWhiteSpace(next))
            {
                throw new InvalidOperationException(string.Format("End state cannot have next: {0}", state));
            }

            Next = next;
            State = state;
        }
        #endregion

        /// <summary>
        /// Check if the provided status matches the pattern, signalling that the
        /// next State should be executed.
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool Matches(string status)
        {
            return PatternMatcher.Match(Pattern, status);
        }

        /// <summary>
        /// Is this state the last ?
        /// </summary>
        /// <returns></returns>
        public bool IsEnd()
        {
            return Next == null;
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("StateTransition: [state={0}, pattern={1}, next={2}]",
            State == null ? null : State.GetName(), Pattern, Next);
        }
    }
}