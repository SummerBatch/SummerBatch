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

namespace Summer.Batch.Core.Scope.Context
{
    /// <summary>
    /// Central convenience class for framework use in managing the job scope
    /// context. Generally only to be used by implementations of Job. N.B.
    /// it is the responsibility of every Job implementation to ensure that
    /// a JobContext is available on every thread that might be involved in
    /// a job execution, including worker threads from a pool.
    /// </summary>
    public static class JobSynchronizationManager
    {
        class LocalSynchronizationManagerSupport : SynchronizationManagerSupport<JobExecution, JobContext>
        {
            protected override void Close(JobContext context)
            {
                context.Close();
            }

            protected override JobContext CreateNewContext(JobExecution execution)
            {
                return new JobContext(execution);
            }
        }
        private static readonly SynchronizationManagerSupport<JobExecution, JobContext> Manager = new LocalSynchronizationManagerSupport();

        /// <summary>
        /// Getter for the current context if there is one, otherwise returns null.
        /// </summary>
        /// <returns>the current  JobContext or null if there is none (if one has not been registered for this thread).</returns>
        public static JobContext GetContext()
        {
            return Manager.GetContext();
        }

        /// <summary>
        ///  Register a context with the current thread - always put a matching
        /// <see cref="Close"/> call in a finally block to ensure that the correct
        /// context is available in the enclosing block.
        /// </summary>
        /// <param name="jobExecution">the step context to register</param>
        /// <returns>a new JobContext or the current one if it has the same JobExecution</returns>
        public static JobContext Register(JobExecution jobExecution)
        {
            return Manager.Register(jobExecution);
        }


        /// <summary>
        /// Method for unregistering the current context - should always and only be
        /// used by in conjunction with a matching <see cref="Register"/>
        /// to ensure that <see cref="GetContext"/> always returns the correct value.
        /// Does not call <see cref="JobContext.Close"/> - that is left up to the caller
        /// because he has a reference to the context (having registered it) and only
        /// he has knowledge of when the step actually ended.
        /// </summary>
        public static void Close()
        {
            Manager.Close();
        }

        /// <summary>
        /// Release (delegates to Manager#Release).
        /// </summary>
        public static void Release()
        {
            Manager.Release();
        }
    }

}
