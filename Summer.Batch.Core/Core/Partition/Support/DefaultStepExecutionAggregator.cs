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
using Summer.Batch.Common.Util;

namespace Summer.Batch.Core.Partition.Support
{
    /// <summary>
    /// Default implementation of <see cref="IStepExecutionAggregator"/>.
    /// </summary>
    public class DefaultStepExecutionAggregator : IStepExecutionAggregator
    {
        /// <summary>
        /// Aggregates the input executions into the result <see cref="StepExecution"/>.
        /// The aggregated fields are:
        /// <list type="table">
        ///     <item>
        ///         <term>BatchStatus</term>
        ///         <description>using the highest value using <see cref="BatchStatus.Max"/></description>
        ///     </item>
        ///     <item>
        ///         <term>ExitStatus</term>
        ///         <description>using <see cref="ExitStatus.And"/></description>
        ///     </item>
        ///     <item>
        ///         <term>counters (e.g., CommitCount, RollbackCount)</term>
        ///         <description>by arithmetic sum</description>
        ///     </item>
        /// </list>
        /// </summary>
        /// <param name="result">the result to overwrite</param>
        /// <param name="executions">the inputs</param>
        public void Aggregate(StepExecution result, ICollection<StepExecution> executions)
        {
            Assert.NotNull(result, "To aggregate into a result it must be non-null.");
            if (executions == null)
            {
                return;
            }
            foreach (var stepExecution in executions)
            {
                result.BatchStatus = BatchStatus.Max(result.BatchStatus, stepExecution.BatchStatus);
                result.ExitStatus = result.ExitStatus.And(stepExecution.ExitStatus);
                result.FilterCount = result.FilterCount + stepExecution.FilterCount;
                result.ProcessSkipCount = result.ProcessSkipCount + stepExecution.ProcessSkipCount;
                result.CommitCount = result.CommitCount + stepExecution.CommitCount;
                result.RollbackCount = result.RollbackCount + stepExecution.RollbackCount;
                result.ReadCount = result.ReadCount + stepExecution.ReadCount;
                result.ReadSkipCount = result.ReadSkipCount + stepExecution.ReadSkipCount;
                result.WriteCount = result.WriteCount + stepExecution.WriteCount;
                result.WriteSkipCount = result.WriteSkipCount + stepExecution.WriteSkipCount;
            }
        }
    }
}