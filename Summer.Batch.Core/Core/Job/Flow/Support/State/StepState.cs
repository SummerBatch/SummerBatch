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
using System.Collections.Generic;
using Summer.Batch.Core.Step;

namespace Summer.Batch.Core.Job.Flow.Support.State
{
    /// <summary>
    /// <see cref="IState"/> implementation that delegates to an <see cref="IFlowExecutor"/> 
    /// to execute the specified Step.
    /// </summary>
    public class StepState : AbstractState, IStepHolder, IStepLocator
    {
        /// <summary>
        /// Step property.
        /// </summary>
        public IStep Step { get; set; }

        #region Constructors
        /// <summary>
        /// Custom constructor using a name.
        /// </summary>
        /// <param name="name"></param>
        public StepState(string name)
            : base(name)
        {
        }


        /// <summary>
        /// Custom constructor using an IStep.
        /// </summary>
        /// <param name="step"></param>
        public StepState(IStep step)
            : base(step.Name)
        {
            Step = step;
        }

        /// <summary>
        /// Custom constructor using an IStep and a name.
        /// </summary>
        /// <param name="step"></param>
        /// <param name="name"></param>
        public StepState(IStep step, string name)
            : base(name)
        {
            Step = step;
        }
        #endregion

        #region IState methods implementation
        /// <summary>
        /// @see IState#Handle .
        /// </summary>
        /// <param name="executor"></param>
        /// <returns></returns>
        /// <exception cref="Exception">&nbsp;</exception>
        public override FlowExecutionStatus Handle(IFlowExecutor executor)
        {
            // On starting a new step, possibly upgrade the last execution to make
            // sure it is abandoned on restart if it failed.
            executor.AbandonStepExecution();
            return new FlowExecutionStatus(executor.ExecuteStep(Step));
        }

        /// <summary>
        /// @see IState#IsEndState
        /// </summary>
        /// <returns></returns>
        public override bool IsEndState()
        {
            return false;
        }
        #endregion

        #region IStepLocator methods implementation
        /// <summary>
        /// @see IStepLocator#GetStepNames .
        /// </summary>
        /// <returns></returns>
        public ICollection<string> GetStepNames()
        {
            List<string> names = new List<string> { Step.Name };
            if (Step is IStepLocator)
            {
                names.AddRange(((IStepLocator)Step).GetStepNames());
            }
            return names;
        }

        /// <summary>
        /// @see IStepLocator#GetStep .
        /// </summary>
        /// <param name="stepName"></param>
        /// <returns></returns>
        /// <exception cref="NoSuchStepException">&nbsp;</exception>
        public IStep GetStep(string stepName)
        {
            IStep result = null;
            if (Step.Name.Equals(stepName))
            {
                result = Step;
            }
            else if (Step is IStepLocator)
            {
                result = ((IStepLocator)Step).GetStep(stepName);
            }
            return result;
        }
        #endregion
    }
}