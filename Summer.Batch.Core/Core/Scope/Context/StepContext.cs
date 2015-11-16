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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Summer.Batch.Common.Util;
using Summer.Batch.Infrastructure.Repeat.Context;

namespace Summer.Batch.Core.Scope.Context
{
    /// <summary>
    /// A context object that can be used to interrogate the current
    /// StepExecution and some of its associated properties using expressions
    /// based on bean paths. Has public getters for the step execution and
    /// convenience methods for accessing commonly used properties like the
    /// ExecutionContext associated with the step or its enclosing job
    /// execution.
    /// </summary>
    public class StepContext : SynchronizedAttributeAccessor
    {
        private readonly StepExecution _stepExecution;

        /// <summary>
        /// Step Execution property.
        /// </summary>
        public StepExecution StepExecution { get { return _stepExecution; } }

        private readonly IDictionary<string, HashSet<Task>> _callbacks = new Dictionary<string, HashSet<Task>>();

        /// <summary>
        /// Create a new instance of StepContext for this StepExecution.
        /// </summary>
        /// <param name="stepExecution"></param>
        public StepContext(StepExecution stepExecution)
        {
            Assert.NotNull(stepExecution, "A StepContext must have a non-null StepExecution");
            _stepExecution = stepExecution;
        }

        /// <summary>
        /// Convenient accessor for current step name identifier. Usually this is the
        /// same as the bean name of the step that is executing (but might not be
        /// e.g. in a partition).
        /// </summary>
        /// <returns></returns>
        public string GetStepName()
        {
            return _stepExecution.StepName;
        }

        /// <summary>
        /// Convenient accessor for current job name identifier.
        /// </summary>
        /// <returns></returns>
        public string GetJobName()
        {
            Assert.NotNull(_stepExecution.JobExecution, "StepExecution does not have a JobExecution");
            Assert.NotNull(_stepExecution.JobExecution.JobInstance, "StepExecution does not have a JobInstance");
            return _stepExecution.JobExecution.JobInstance.JobName;
        }

        /// <summary>
        /// Returns the step execution context.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyDictionary<string, object> GetStepExecutionContext()
        {
            IDictionary<string, object> result = _stepExecution.ExecutionContext.EntrySet.ToDictionary(entry => entry.Key, entry => entry.Value);
            return new ReadOnlyDictionary<string, object>(result);

        }

        /// <summary>
        /// Returns the job execution context.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyDictionary<string, object> GetJobExecutionContext()
        {
            IDictionary<string, object> result = _stepExecution.JobExecution.ExecutionContext.EntrySet.ToDictionary(entry => entry.Key, entry => entry.Value);
            return new ReadOnlyDictionary<string, object>(result);

        }

        /// <summary>
        /// Returns the job parameters.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyDictionary<string, JobParameter> GetJobParameters()
        {
            IDictionary<string, JobParameter> result =
            _stepExecution.GetJobParameters().GetParameters().ToDictionary(entry => entry.Key, entry => entry.Value);
            return new ReadOnlyDictionary<string, JobParameter>(result);
        }

        /// <summary>
        /// </summary>
        /// <returns>an unique identifier for this context based on the step execution</returns>
        public string GetId()
        {
            Assert.State(_stepExecution.Id != null, "StepExecution has no id.  "
                    + "It must be saved before it can be used in step scope.");
            return string.Format("execution#{0}", StepExecution.Id);
        }

        /// <summary>
        /// Equals override.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(object other)
        {
            if (!(other is StepContext))
            {
                return false;
            }
            if (other == this)
            {
                return true;
            }
            StepContext context = (StepContext)other;
            if (context.StepExecution == StepExecution)
            {
                return true;
            }
            return _stepExecution.Equals(context.StepExecution);
        }

        /// <summary>
        /// Overrides the default behaviour to provide a hash code based only on the
        /// step execution.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return _stepExecution.GetHashCode();
        }

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Concat(base.ToString(),
                string.Format(", stepExecutionContext={0}, jobExecutionContext={1}, jobParameters={2}",
                GetStepExecutionContext(), GetJobExecutionContext(), GetJobParameters()));
        }


        /// <summary>
        /// Allow clients to register callbacks for clean up on close.
        /// </summary>
        /// <param name="name">the callback id (unique attribute key in this context)</param>
        /// <param name="callback">a callback to execute on close</param>
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
        /// Unregisters destruction callback.
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

        ///<summary>
        /// Clean up the context at the end of a step execution. Must be called once
        /// at the end of a step execution to honour the destruction callback
        /// contract from the Step Scope.
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

            throw new UnexpectedJobExecutionException(
                string.Format("Could not close step context, rethrowing first of {0} exceptions", errors.Count),
                errors[0]);
        }


    }
}
