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

using Summer.Batch.Core.Repository;

namespace Summer.Batch.Core.Job.Flow
{
    /// <summary>
    /// Context and execution strategy for <see cref="FlowJob"/> to allow it to delegate
    /// its execution step by step.
    /// </summary>
    public interface IFlowExecutor
    {

        /// <summary>
        /// Executes the given step.
        /// </summary>
        /// <param name="step">an IStep to execute</param>
        /// <returns>the exit status that drives the surrounding Flow</returns>
        /// <exception cref="JobInterruptedException">&nbsp;</exception>
        /// <exception cref="JobRestartException">&nbsp;</exception>
        /// <exception cref="StartLimitExceededException">&nbsp;</exception>
        string ExecuteStep(IStep step);

        /// <summary>
        /// </summary>
        /// <returns>the current JobExecution</returns>
        JobExecution GetJobExecution();

        /// <summary>
        /// </summary>
        /// <returns>the latest StepExecution or null if there is none</returns>
        StepExecution GetStepExecution();

        /// <summary>
        /// Chance to clean up resources at the end of a flow (whether it completed
        /// successfully or not).
        /// </summary>
        /// <param name="result">the final FlowExecution</param>
        void Close(FlowExecution result);

        /// <summary>
        /// Handle any status changes that might be needed at the start of a state.
        /// </summary>
        void AbandonStepExecution();

        /// <summary>
        /// Handle any status changes that might be needed in the JobExecution.
        /// </summary>
        /// <param name="status"></param>
        void UpdateJobExecutionStatus(FlowExecutionStatus status);

        /// <summary>
        /// </summary>
        /// <returns>true if the flow is at the beginning of a restart</returns>
        bool IsRestart();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code">code the label for the exit status when a flow or sub-flow ends</param>
        void AddExitStatus(string code);
    }
}