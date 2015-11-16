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
using Summer.Batch.Core;

namespace Summer.Batch.CoreTests.Batch.Listeners
{
    /// <summary>
    /// Custom code listener
    /// </summary>
    public class CustomCodeListener : IStepExecutionListener
    {
        /// <summary>
        /// see IStepExecutionListener#BeforeStep
        /// Does nothing on purpose.
        /// </summary>
        /// <param name="stepExecution"></param>
        public void BeforeStep(StepExecution stepExecution)
        {
            // Do Nothing
        }

        /// <summary>
        /// see IStepExecutionListener#AfterStep
        /// </summary>
        /// <param name="stepExecution"></param>
        /// <returns></returns>
        public ExitStatus AfterStep(StepExecution stepExecution)
        {
            return new ExitStatus("OK");
        }
    }
}