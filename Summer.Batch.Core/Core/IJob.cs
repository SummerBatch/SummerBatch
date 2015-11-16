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
    /// A Job's contract.
    /// </summary>
    public interface IJob
    {
        /// <summary>
        /// Name.
        /// </summary>
        /// <returns></returns>
        string Name { get; set; }

        /// <summary>
        /// Flag to indicate if this job can be restarted, at least in principle
        /// (Actual restartability is bound to the use of a persisted job repository).
        /// </summary>
        /// <returns></returns>
        bool Restartable { get; set; }
    
        /// <summary>
        /// Runs the <see cref="JobExecution"/>and update the meta information like status
        /// and statistics as necessary. This method should not throw any exceptions
        /// for failed execution. Clients should be careful to inspect the
        /// JobExecution status to determine success or failure.
        /// </summary>
        /// <param name="execution"></param>
        void Execute(JobExecution execution);

        /// <summary>
        /// If clients need to generate new parameters for the next execution in a
        /// sequence they can use this incrementer. The return value may be null, in
        /// the case that this job does not have a natural sequence.
        /// </summary>
        /// <returns>an incrementer to be used for creating new parameters</returns>
        IJobParametersIncrementer JobParametersIncrementer { get; set; }

        /// <summary>
        /// A validator for the job parameters of a <see cref="JobExecution"/>. Clients of
        /// a Job may need to validate the parameters for a launch, before or during
        /// the execution.
        /// </summary>
        /// <returns>	 a validator that can be used to check parameter values (never null)</returns>
        IJobParametersValidator JobParametersValidator { get; set; }
    }
}
