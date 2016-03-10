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

namespace Summer.Batch.Core.Partition
{
    /// <summary>
    /// Interface defining the responsibilities of controlling the execution of a
    /// partitioned  <see cref="StepExecution"/>. Implementations will need to create a
    /// partition with the <see cref="IStepExecutionSplitter"/>, and then use an execution
    /// fabric (grid, etc.), to execute the partitioned step. The results of the
    /// executions can be returned raw from remote workers to be aggregated by the
    /// caller.
    /// </summary>
    public interface IPartitionHandler
    {
        /// <summary>
        /// Main entry point for IPartitionHandler interface. The splitter
        /// creates all the executions that need to be farmed out, along with their
        /// input parameters (in the form of their <see cref="Summer.Batch.Infrastructure.Item.ExecutionContext"/>). The
        /// master step execution is used to identify the partition and group
        /// together the results logically.
        /// </summary>
        /// <param name="stepSplitter">a strategy for generating a collection of <see cref="StepExecution"/> instances</param>
        /// <param name="stepExecution">the master step execution for the whole partition</param>
        /// <returns>a collection of completed StepExecution instances</returns>
        /// <exception cref="Exception">&nbsp;if anything goes wrong. This allows implementations to
        /// be liberal and rely on the caller to translate an exception into a step
        /// failure as necessary.</exception>
        ICollection<StepExecution> Handle(IStepExecutionSplitter stepSplitter, StepExecution stepExecution);
    }
}