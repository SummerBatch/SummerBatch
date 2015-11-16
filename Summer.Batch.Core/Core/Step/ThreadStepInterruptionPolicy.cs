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

using NLog;
using Summer.Batch.Common.Util;

namespace Summer.Batch.Core.Step
{
    /// <summary>
    /// Policy that checks the current thread to see if it has been interrupted.
    /// </summary>
    public class ThreadStepInterruptionPolicy : IStepInterruptionPolicy
    {

        /// <summary>
        /// Logger.
        /// </summary>
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Checks if step execution has been interrupted. Throws a JobInterrupdeException in that case.
        /// </summary>
        /// <param name="stepExecution"></param>
        /// <exception cref="JobInterruptedException"></exception>
        public void CheckInterrupted(StepExecution stepExecution)
        {
            if (IsInterrupted(stepExecution))
            {
                throw new JobInterruptedException("Job interrupted status detected.");
            }
        }

        /// <summary>
        /// Test for api or applicative interruption.
        /// </summary>
        /// <param name="stepExecution"></param>
        /// <returns></returns>
        private bool IsInterrupted(StepExecution stepExecution)
        {
            bool interrupted = ThreadUtils.IsCurrentThreadInterrupted();
            if (interrupted)
            {
                Logger.Info("Step interrupted through Thread API");
            }
            else
            {
                interrupted = stepExecution.TerminateOnly;
                if (interrupted)
                {
                    Logger.Info("Step interrupted through StepExecution");
                }
            }
            return interrupted;
        }

    }
}