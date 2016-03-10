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
using System;
using System.Collections.Generic;

namespace Summer.Batch.Core.Partition.Support
{
    /// <summary>
    /// Implementation of <see cref="IStep"/> which partitions the execution and spreads the
    ///load using an <see cref="IPartitionHandler"/>.
    /// </summary>
    public class PartitionStep : AbstractStep
    {
        private IStepExecutionAggregator _stepExecutionAggregator = new DefaultStepExecutionAggregator();

        /// <summary>
        /// Step Execution splitter property.
        /// </summary>
        public IStepExecutionSplitter StepExecutionSplitter { protected get; set; }

        /// <summary>
        /// Partition handler property.
        /// </summary>
        public IPartitionHandler PartitionHandler { protected get; set; }

        /// <summary>
        /// Step execution aggregator property.
        /// </summary>
        public IStepExecutionAggregator StepExecutionAggregator
        {
            private get { return _stepExecutionAggregator; }
            set { _stepExecutionAggregator = value; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PartitionStep() { }

        /// <summary>
        /// Custom constructor using a name.
        /// </summary>
        /// <param name="name"></param>
        public PartitionStep(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Asserts that mandatory properties are set (stepExecutionSplitter,
        /// partitionHandler) and delegate to superclass.
        /// @see IInitializationPostOperations#AfterPropertiesSet
        /// </summary>
        /// <exception cref="Exception">&nbsp;</exception>
        public override void AfterPropertiesSet()
        {
            Assert.NotNull(StepExecutionSplitter, "StepExecutionSplitter must be provided");
            Assert.NotNull(PartitionHandler, "PartitionHandler must be provided");
            base.AfterPropertiesSet();
        }


        /// <summary>
        /// Delegates execution to the provided <see cref="IPartitionHandler"/>. The
        /// <see cref="StepExecution"/> passed in here becomes the parent or master
        /// execution for the partition, summarising the status on exit of the
        /// logical grouping of work carried out by the PartitionHandler. The
        /// individual step executions and their input parameters (through
        /// ExecutionContext) for the partition elements are provided by the
        /// StepExecutionSplitter.
        /// </summary>
        /// <param name="stepExecution">the master step execution for the partition</param>
        /// <exception cref="Exception">&nbsp;</exception>
        protected override void DoExecute(StepExecution stepExecution)
        {
            stepExecution.ExecutionContext.Put(StepConstants.StepTypeKey, GetType().Name);

            // Wait for task completion and then aggregate the results
            ICollection<StepExecution> executions = PartitionHandler.Handle(StepExecutionSplitter, stepExecution);
            stepExecution.UpgradeStatus(BatchStatus.Completed);
            StepExecutionAggregator.Aggregate(stepExecution, executions);

            // If anything failed or had a problem we need to crap out
            if (stepExecution.BatchStatus.IsUnsuccessful())
            {
                throw new JobExecutionException("Partition handler returned an unsuccessful step");
            }
        }
    }
}