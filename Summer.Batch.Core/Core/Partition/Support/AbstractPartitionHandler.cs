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

namespace Summer.Batch.Core.Partition.Support
{
    /// <summary>
    /// Base <see cref="IPartitionHandler"/> implementation providing common base
    /// features. Subclasses are expected to implement only the <see cref="DoHandle"/>
    /// method which returns with the result of the execution(s) or an exception if
    /// the step failed to process.
    /// </summary>
    public abstract class AbstractPartitionHandler : IPartitionHandler
    {

        private int _gridSize = 1;
        
        /// <summary>
        /// Grid size property. Defaults to 1.
        /// </summary>
        public int GridSize { get { return _gridSize; } set { _gridSize = value; } }

        /// <summary>
        ///Executes the specified <see cref="StepExecution"/> instances and returns an updated
        /// view of them. Throws an Exception if anything goes wrong.
        /// </summary>
        /// <param name="masterStepExecution">the whole partition execution</param>
        /// <param name="partitionStepExecutions"> the StepExecution instances to execute</param>
        /// <returns>an updated view of these completed StepExecution instances</returns>
        /// <exception cref="Exception">&nbsp;if anything goes wrong. This allows implementations to
        /// be liberal and rely on the caller to translate an exception into a step
        /// failure as necessary.</exception>
        protected abstract ICollection<StepExecution> DoHandle(StepExecution masterStepExecution,
            ICollection<StepExecution> partitionStepExecutions);

        /// <summary>
        /// see IPartitionHandler#Handle .
        /// </summary>
        /// <param name="stepSplitter"></param>
        /// <param name="masterStepExecution"></param>
        /// <returns></returns>
        /// <exception cref="Exception">&nbsp;</exception>
        public ICollection<StepExecution> Handle(IStepExecutionSplitter stepSplitter, StepExecution masterStepExecution)
        {
            HashSet<StepExecution> stepExecutions = stepSplitter.Split(masterStepExecution, _gridSize);
            if (masterStepExecution.ExecutionContext.ContainsKey("batch.restart"))
            {
                foreach(StepExecution stepExecution in stepExecutions)
                {
                    stepExecution.ExecutionContext.Put("batch.restart", true);
                }
            }
            return DoHandle(masterStepExecution, stepExecutions);
        }
    }
}