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

using Summer.Batch.Core.Step;
using System.Collections.Generic;

namespace Summer.Batch.Core.Job
{
    /// <summary>
    /// Simple implementation of <see cref="IJob"/> interface providing the ability to run a
    /// <see cref="JobExecution"/>. Sequentially executes a job by iterating through its
    /// list of steps.  Any Step that fails will fail the job.  The job is
    /// considered complete when all steps have been executed.
    /// </summary>
    public class SimpleJob : AbstractJob
    {
        /// <summary>
        /// Custom constructor with a name.
        /// </summary>
        /// <param name="name"></param>
        public SimpleJob(string name) : base(name) { }

        /// <summary>
        /// Default empty constructor.
        /// </summary>
        public SimpleJob() { }

        private readonly List<IStep> _steps = new List<IStep>();

        /// <summary>
        /// Public setter for the steps in this job. Overrides any calls to
        /// #AddStep(IStep)}.
        /// </summary>
        public List<IStep> Steps
        {
            set
            {
                _steps.Clear();
                foreach (var entry in value)
                {
                    _steps.Add(entry);
                }
            }
        }

        /// <summary>
        /// Add given step to the steps collection.
        /// </summary>
        /// <param name="step"></param>
        public void AddStep(IStep step) { _steps.Add(step); }

        /// <summary>
        /// Returns the step given its name, or null if step could not be found.
        /// </summary>
        /// <param name="stepName"></param>
        /// <returns></returns>
        public override IStep GetStep(string stepName)
        {
            foreach (var step in _steps)
            {
                if (step.Name.Equals(stepName))
                {
                    return step;
                }
                if (step is IStepLocator)
                {
                    IStep result = ((IStepLocator)step).GetStep(stepName);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            //not found
            return null;
        }



        /// <summary>
        ///  Handler of steps sequentially as provided, checking each one for success
        /// before moving to the next. Returns the last StepExecution
        /// successfully processed if it exists, and null if none were processed.
        /// </summary>
        /// <param name="execution"></param>
        protected override void DoExecute(JobExecution execution)
        {
            StepExecution stepExecution = null;
            foreach (IStep step in _steps)
            {
                stepExecution = HandleStep(step, execution);
                if (stepExecution.BatchStatus != BatchStatus.Completed)
                {
                    break;
                }
            }

            //
            // Update the job status to be the same as the last step
            //
            if (stepExecution != null)
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Upgrading JobExecution status: {0}", stepExecution);
                }
                execution.UpgradeStatus(stepExecution.BatchStatus);
                execution.ExitStatus = stepExecution.ExitStatus;
            }
        }

        /// <summary>
        /// Convenience method for clients to inspect the steps for this job.
        /// </summary>
        /// <returns></returns>
        public override ICollection<string> GetStepNames()
        {
            List<string> names = new List<string>();
            foreach (IStep step in _steps)
            {
                names.Add(step.Name);
                if (step is IStepLocator)
                {
                    names.AddRange(((IStepLocator)step).GetStepNames());
                }
            }
            return names;
        }

    }
}
