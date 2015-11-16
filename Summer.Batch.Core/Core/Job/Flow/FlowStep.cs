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

using Summer.Batch.Core.Step;
using Summer.Batch.Common.Util;

namespace Summer.Batch.Core.Job.Flow
{
    /// <summary>
    /// An <see cref="IStep"/> implementation that delegates to an <see cref="IFlow"/>. 
    /// Useful for logical grouping of steps, and especially for partitioning with multiple
    /// steps per execution. If the flow has steps then when the FlowStep
    /// executes, all steps including the parent FlowStep will have
    /// executions in the <see cref="Summer.Batch.Core.Repository.IJobRepository"/> (one for the parent and one each for
    /// the flow steps).
    /// </summary>
    public class FlowStep : AbstractStep
    {
        /// <summary>
        /// Flow property.
        /// </summary>
        public IFlow Flow { private get; set; }

        /// <summary>
        ///  Default constructor convenient for configuration purposes.
        /// </summary>
        public FlowStep() : base(null) { }

        /// <summary>
        ///  Constructor for a FlowStep that sets the flow and of the step explicitly.
        /// </summary>
        /// <param name="flow"></param>
        public FlowStep(IFlow flow) : base(flow.GetName()) { }

        /// <summary>
        /// Custom constructor with a name.
        /// </summary>
        /// <param name="name"></param>
        public FlowStep(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Post-init. checks.
        /// @see IInitializationPostOperations#AfterPropertiesSet.
        /// </summary>
         public override void AfterPropertiesSet() {
            Assert.State(Flow != null, "A Flow must be provided");
            if (Name == null)
            {
                Name = Flow.GetName();
            }
            base.AfterPropertiesSet();
        }

        /// <summary>
        /// Delegates to the flow provided for the execution of the step.
        /// </summary>
        /// <param name="stepExecution"></param>
        /// <exception cref="System.Exception"></exception>
        protected override void DoExecute(StepExecution stepExecution)
        {
            try
            {
                stepExecution.ExecutionContext.Put(StepConstants.StepTypeKey, GetType().Name);
                IStepHandler stepHandler = new SimpleStepHandler(JobRepository, stepExecution.ExecutionContext);
                IFlowExecutor executor = new JobFlowExecutor(JobRepository, stepHandler, stepExecution.JobExecution);
                executor.UpdateJobExecutionStatus(Flow.Start(executor).Status);
                stepExecution.UpgradeStatus(executor.GetJobExecution().Status);
                stepExecution.ExitStatus = executor.GetJobExecution().ExitStatus;
            }
            catch (FlowExecutionException e)
            {
                if (e.InnerException is JobExecutionException)
                {
                    throw e.InnerException;
                }
                throw new JobExecutionException("Flow execution ended unexpectedly", e);
            }
        }
    }
}