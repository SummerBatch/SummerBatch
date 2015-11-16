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
//  distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System.Collections.Generic;
using Summer.Batch.Core;
using Summer.Batch.Extra.Process;

namespace Summer.Batch.Extra
{
    /// <summary>
    /// Prepares the step by initalizing the process writers called inside
    /// </summary>
    public class WriterResourceManager : IStepExecutionListener
    {
        /// <summary>
        /// Step context manager property.
        /// </summary>
        public IContextManager StepContextManager { get; set; }

        /// <summary>
        /// Collection of writers
        /// </summary>
        public IList<IProcessAdapter> Writers { get; set; }

        /// <summary>
        /// @see IStepExecutionListener#BeforeStep
        /// Launched before the step. Initializes the writers associated streams, if any.
        /// </summary>
        /// <param name="stepExecution"></param>
        public void BeforeStep(StepExecution stepExecution)
        {
            StepContextManager.Context = stepExecution.ExecutionContext;
            foreach (var writer in Writers)
            {
                writer.InitStream();
            }
        }

        /// <summary>
        /// @see IStepExecutionListener#AfterStep
        ///  Launched after the step. Not used, thus does nothing.
        /// </summary>
        /// <param name="stepExecution"></param>
        /// <returns></returns>
        public ExitStatus AfterStep(StepExecution stepExecution)
        {
            return ExitStatus.Completed;
        }
    }
}