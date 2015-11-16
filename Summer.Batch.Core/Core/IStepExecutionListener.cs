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

namespace Summer.Batch.Core
{
    /// <summary>
    /// Listener interface for the lifecycle of an <see cref="IStep"/>.
    /// </summary>
    public interface IStepExecutionListener : IStepListener
    {
        /// <summary>
        ///  Initialize the state of the listener with the <see cref="StepExecution"/> from
        /// the current scope.
        /// </summary>
        /// <param name="stepExecution"></param>
        void BeforeStep(StepExecution stepExecution);

        /// <summary>
        /// Give a listener a chance to modify the exit status from a step. The value
        /// returned will be combined with the normal exit status using
        ///  ExitStatus#And(ExitStatus).
        ///
        /// Called after execution of step's processing logic (both successful or
        /// failed). Throwing exception in this method has no effect, it will only be
        /// logged.
        /// </summary>
        /// <param name="stepExecution"></param>
        /// <returns></returns>
        ExitStatus AfterStep(StepExecution stepExecution);
    }
}
