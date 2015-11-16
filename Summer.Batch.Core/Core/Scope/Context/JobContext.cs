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

using Summer.Batch.Infrastructure.Repeat.Context;
using Summer.Batch.Common.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Summer.Batch.Core.Scope.Context
{
    /// <summary>
    /// A context object that can be used to interrogate the current JobExecution and some of its associated
    /// properties using expressions
    /// based on bean paths. Has public getters for the job execution and
    /// convenience methods for accessing commonly used properties like the  ExecutionContext associated with the job
    /// execution.
    /// </summary>
    public class JobContext : SynchronizedAttributeAccessor
    {

        private readonly JobExecution _jobExecution;

        /// <summary>
        /// Job execution property.
        /// </summary>
        public JobExecution JobExecution { get { return _jobExecution; } }


        private readonly IDictionary<string, HashSet<Task>> _callbacks = new Dictionary<string, HashSet<Task>>();

        /// <summary>
        /// Custom constructor using a JobExecution.
        /// </summary>
        /// <param name="jobExecution"></param>
        public JobContext(JobExecution jobExecution)
        {
            Assert.NotNull(jobExecution, "A JobContext must have a non-null JobExecution");
            _jobExecution = jobExecution;
        }

        /// <summary>
        /// Convenient accessor for current job name identifier.
        /// </summary>
        /// <returns></returns>
        public string GetJobName()
        {
            Assert.State(_jobExecution.JobInstance != null, "JobExecution does not have a JobInstance");
            return _jobExecution.JobInstance.JobName;
        }

        /// <summary> 
        /// </summary>
        /// <returns>a dictionary containing the items from the job ExecutionContext</returns>
        public IReadOnlyDictionary<string, object> GetJobExecutionContext()
        {
            IDictionary<string, object> result =
                _jobExecution.ExecutionContext.EntrySet.ToDictionary(entry => entry.Key, entry => entry.Value);
            return new ReadOnlyDictionary<string, object>(result);
        }

        /// <summary> 
        /// </summary>
        /// <returns>a dictionary containing the items from the job ExecutionContext</returns>
        public IReadOnlyDictionary<string, JobParameter> GetJobParameters()
        {
            Dictionary<string, JobParameter> result =
                _jobExecution.JobParameters.GetParameters().ToDictionary(entry => entry.Key, entry => entry.Value);
            return new ReadOnlyDictionary<string, JobParameter>(result);
        }

        /// <summary>
        /// Registers a destruction callback. 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        public void RegisterDestructionCallback(string name, Task callback)
        {
            lock (_callbacks)
            {
                HashSet<Task> set;
                _callbacks.TryGetValue(name, out set);
                if (set == null)
                {
                    set = new HashSet<Task>();
                    _callbacks.Add(name, set);
                }
                set.Add(callback);
            }
        }

        /// <summary>
        /// Unregisters a destruction callback, given its name.
        /// </summary>
        /// <param name="name"></param>
        private void UnregisterDestructionCallbacks(string name)
        {
            lock (_callbacks)
            {
                _callbacks.Remove(name);
            }
        }

        /// <summary>
        /// Override base class behaviour to ensure destruction callbacks are
        /// unregistered as well as the default behaviour.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override object RemoveAttribute(string name)
        {
            UnregisterDestructionCallbacks(name);
            return base.RemoveAttribute(name);
        }

        /// <summary>
        /// Cleans up the context at the end of a step execution. Must be called once
        ///  at the end of a step execution to honour the destruction callback
        ///  contract from the StepScope.
        /// </summary>
        public void Close()
        {
            List<Exception> errors = new List<Exception>();

            IReadOnlyDictionary<string, HashSet<Task>> copy =
                new ReadOnlyDictionary<string, HashSet<Task>>(_callbacks);

            foreach (KeyValuePair<string, HashSet<Task>> entry in copy)
            {
                HashSet<Task> set = entry.Value;
                foreach (Task callback in set)
                {
                    if (callback != null)
                    {
                        try
                        {
                            callback.RunSynchronously();
                        }
                        catch (Exception t)
                        {
                            errors.Add(t);
                        }
                    }
                }
            }

            if (!errors.Any())
            {
                return;
            }

            Exception error = errors[0];
            
            throw new UnexpectedJobExecutionException(
                string.Format("Could not close step context, rethrowing first of {0} exceptions.", errors.Count), error);
        }

        /// <summary>
        /// </summary>
        /// <returns>an unique identifier for this context based on the job execution</returns>
        public string GetId()
        {
            Assert.State(_jobExecution.Id != null, "StepExecution has no id.  "
                    + "It must be saved before it can be used in step scope.");
            return string.Format("execution#{0}", _jobExecution.Id);
        }

        /// <summary>
        /// Equals override.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(object other)
        {
            if (!(other is JobContext))
            {
                return false;
            }
            if (other == this)
            {
                return true;
            }
            JobContext context = (JobContext)other;
            if (context._jobExecution == _jobExecution)
            {
                return true;
            }
            return _jobExecution.Equals(context._jobExecution);
        }


        /// <summary>
        /// GetHashCode override.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return _jobExecution.GetHashCode();
        }

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0},  , jobExecutionContext={1},  jobParameters={2}",
                base.ToString(),
                GetJobExecutionContext(), GetJobParameters());            
        }


    }
}
