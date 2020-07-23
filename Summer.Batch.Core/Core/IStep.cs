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
    /// Batch domain interface representing the configuration of a step. As with the Job, an IStep is meant to
    /// explicitly represent the configuration of a step by a developer, but also the ability to execute the step.
    /// </summary>
    public interface IStep
    {
        /// <summary>
        /// Name.
        /// </summary>
        /// <returns></returns>
        string Name { get; }

        /// <summary>
        /// Allow start if complete flag.
        /// </summary>
        /// <returns></returns>
        bool? AllowStartIfComplete { get; set; }

        /// <summary>
        /// Start limit.
        /// </summary>
        /// <returns></returns>
        int StartLimit { get; set; }

        int DelayConfig { get; set; }

        /// <summary>
        /// Executes the step.
        /// </summary>
        /// <param name="stepExecution"></param>
        /// <exception cref="JobExecutionException">&nbsp;</exception>
        void Execute(StepExecution stepExecution);
    }
}
