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
    /// <see cref="IState"/> implementation for ending a job if it is in progress and
    /// continuing if just starting.
    /// </summary>
    public class EndState : AbstractState
    {
        #region Attributes
        /// <summary>
        /// Status.
        /// </summary>
        protected FlowExecutionStatus Status { get; private set; }

        /// <summary>
        /// Abandon flag.
        /// </summary>
        protected bool Abandon { get; private set; }

        /// <summary>
        /// Code.
        /// </summary>
        protected string Code { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Custom constructor using a name.
        /// </summary>
        /// <param name="name"></param>
        public EndState(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Custom constructor using a status and a name.
        /// </summary>
        /// <param name="status">The FlowExecutionStatus to end with</param>
        /// <param name="name">The name of the state</param>
        public EndState(FlowExecutionStatus status, string name)
            : this(status, status.Name, name) { }

        /// <summary>
        /// Custom constructor using a status, a code and a name.
        /// </summary>
        /// <param name="status">The FlowExecutionStatus to end with</param>
        /// <param name="code"></param>
        /// <param name="name">The name of the state</param>
        public EndState(FlowExecutionStatus status, string code, string name) : this(status, code, name, false) { }

        /// <summary>
        /// Custom constructor using a status, a code, a name and an abandon flag.
        /// </summary>
        /// <param name="status">The FlowExecutionStatus to end with</param>
        /// <param name="code"></param>
        /// <param name="name">The name of the state</param>
        /// <param name="abandon">flag to indicate that previous step execution can be marked as abandoned (if there is one)</param>
        public EndState(FlowExecutionStatus status, string code, string name, bool abandon)
            : this(name)
        {
            Status = status;
            Code = code;
            Abandon = abandon;
        }
        #endregion

        #region IState methods implementation
        /// <summary>
        /// Returns the FlowExecutionStatus stored.
        /// @see IState#Handle
        /// </summary>
        /// <param name="executor"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public override FlowExecutionStatus Handle(IFlowExecutor executor)
        {
            lock (executor)
            {
                // Special case. If the last step execution could not complete we
                // are in an unknown state (possibly unrecoverable).
                StepExecution stepExecution = executor.GetStepExecution();
                if (stepExecution != null && executor.GetStepExecution().BatchStatus == BatchStatus.Unknown)
                {
                    return FlowExecutionStatus.Unkown;
                }
                if (Status.IsStop())
                {
                    if (!executor.IsRestart())
                    {
                         //If there are step executions, then we are not at the
                         // beginning of a restart.                     
                        if (Abandon)
                        {
                             // Only if instructed to do so, upgrade the status of
                             // last step execution so it is not replayed on a
                             // restart...
                            executor.AbandonStepExecution();
                        }
                    }
                    else
                    {
                         // If we are a stop state and we got this far then it must
                         // be a restart, so return COMPLETED.
                        return FlowExecutionStatus.Completed;
                    }
                }
                SetExitStatus(executor, Code);
                return Status;

            }
        }

        /// <summary>
        /// @see IState#IsEndState .
        /// </summary>
        /// <returns></returns>
        public override bool IsEndState()
        {
            return !Status.IsStop();
        }
        #endregion


        /// <summary>
        /// Performs any logic to update the exit status for the current flow.
        /// </summary>
        /// <param name="executor">FlowExecutor for the current flow</param>
        /// <param name="code">The exit status to save</param>
        protected void SetExitStatus(IFlowExecutor executor, string code)
        {
            executor.AddExitStatus(code);
        }

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} status=[{1}]", base.ToString(), Status);
        }
    }
}