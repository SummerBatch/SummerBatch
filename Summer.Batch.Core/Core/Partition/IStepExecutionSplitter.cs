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

namespace Summer.Batch.Core.Partition
{
    /// <summary>
    /// Strategy interface for generating input contexts for a partitioned step
    /// execution independent from the fabric they are going to run on.
    /// </summary>
    public interface IStepExecutionSplitter
    {
        /// <summary>
        /// The name of the step configuration that will be executed remotely. Remote
        /// workers are going to execute a the same step for each execution context
        /// in the partition.
        /// </summary>
        /// <returns></returns>
        string StepName { get; set; }

        /// <summary>
        /// Partition the provided <see cref="StepExecution"/> into a set of parallel
        /// executable instances with the same parent <see cref="JobExecution"/>. The grid
        /// size will be treated as a hint for the size of the collection to be
        /// returned. It may or may not correspond to the physical size of an
        /// execution grid.	 
        /// On a restart clients of the <see cref="IStepExecutionSplitter"/> should expect
        /// it to reconstitute the state of the last failed execution and only return
        /// those executions that need to be restarted. Thus the grid size hint will
        /// be ignored on a restart.
        /// </summary>
        /// <param name="stepExecution">the StepExecution to be partitioned.</param>
        /// <param name="gridSize"> a hint for the splitter if the size of the grid is known</param>
        /// <returns>a set of StepExecution instances for remote processing</returns>
        /// <exception cref="JobExecutionException"></exception>
        HashSet<StepExecution> Split(StepExecution stepExecution, int gridSize);
    }
}