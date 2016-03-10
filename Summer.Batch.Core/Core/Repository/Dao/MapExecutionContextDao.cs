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

using Summer.Batch.Infrastructure.Item;
using Summer.Batch.Common.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Summer.Batch.Core.Repository.Dao
{
    /// <summary>
    /// In-memory implementation of IExecutionContextDao backed by dictionaries.
    /// </summary>
    public class MapExecutionContextDao : IExecutionContextDao
    {
        private readonly IDictionary<ContextKey, ExecutionContext> _contexts = new ConcurrentDictionary<ContextKey, ExecutionContext>();

        /// <summary>
        /// Clears the contexts dictionary.
        /// </summary>
        public void Clear()
        {
            _contexts.Clear();
        }

        /// <summary>
        /// Copies through serialization/deserialization process.
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        private static ExecutionContext Copy(ExecutionContext original)
        {
            return original.Serialize().Deserialize<ExecutionContext>();
        }

        #region IExecutionContextDao methods implementation
        /// <summary>
        /// @see IExecutionContextDao#GetExecutionContext .
        /// </summary>
        /// <param name="jobExecution"></param>
        /// <returns></returns>
        public ExecutionContext GetExecutionContext(JobExecution jobExecution)
        {
            return Copy(_contexts[jobExecution.GetContextKey()]);
        }

        /// <summary>
        /// @see IExecutionContextDao#GetExecutionContext .
        /// </summary>
        /// <param name="stepExecution"></param>
        /// <returns></returns>
        public ExecutionContext GetExecutionContext(StepExecution stepExecution)
        {
            return Copy(_contexts[stepExecution.GetContextKey()]);
        }

        /// <summary>
        /// @see IExecutionContextDao#SaveExecutionContext .
        /// </summary>
        /// <param name="jobExecution"></param>
        public void SaveExecutionContext(JobExecution jobExecution)
        {
            UpdateExecutionContext(jobExecution);
        }

        /// <summary>
        /// @see IExecutionContextDao#SaveExecutionContext .
        /// </summary>
        /// <param name="stepExecution"></param>
        public void SaveExecutionContext(StepExecution stepExecution)
        {
            UpdateExecutionContext(stepExecution);
        }

        /// <summary>
        /// @see IExecutionContextDao#SaveExecutionContexts .
        /// </summary>
        /// <param name="stepExecutions"></param>
        public void SaveExecutionContexts(ICollection<StepExecution> stepExecutions)
        {
            Assert.NotNull(stepExecutions, "Attempt to save a null collection of step executions");
            foreach (var stepExecution in stepExecutions)
            {
                SaveExecutionContext(stepExecution);
                SaveExecutionContext(stepExecution.JobExecution);
            }
        }

        /// <summary>
        /// @see IExecutionContextDao#UpdateExecutionContext .
        /// </summary>
        /// <param name="jobExecution"></param>
        public void UpdateExecutionContext(JobExecution jobExecution)
        {
            var executionContext = jobExecution.ExecutionContext;
            if (executionContext != null)
            {
                _contexts[jobExecution.GetContextKey()] = Copy(executionContext);
            }
        }

        /// <summary>
        /// @see IExecutionContextDao#UpdateExecutionContext .
        /// </summary>
        /// <param name="stepExecution"></param>
        public void UpdateExecutionContext(StepExecution stepExecution)
        {
            var executionContext = stepExecution.ExecutionContext;
            if (executionContext != null)
            {
                _contexts[stepExecution.GetContextKey()] = Copy(executionContext);
            }
        } 
        #endregion
    }

    /// <summary>
    /// ContextKey Inner class. 
    /// </summary>
    [Serializable]
    class ContextKey : IComparable<ContextKey>
    {
        /// <summary>
        /// Possible contexts enumeration
        /// </summary>
        public enum Type { Step, Job }

        private readonly Type _type;
        private readonly long? _id;

        /// <summary>
        /// Custom constructor with type and id.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        public ContextKey(Type type, long? id)
        {
            _type = type;
            _id = id;
        }

        /// <summary>
        /// @see IComparable#CompareTo.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(ContextKey other)
        {
            if (other == null)
            {
                return 1;
            }
            var idCompare = Nullable.Compare(_id, other._id);
            return idCompare == 0 ? _type.CompareTo(other._type) : idCompare;

        }

        /// <summary>
        /// Equals override.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(Object other)
        {
            var key = other as ContextKey;
            if (key != null)
            {
                return _id == key._id && _type == key._type;
            }
            return false;
        }

        /// <summary>
        /// GetHashCode override.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var value = _id == null ? 0 : (int)(_id ^ ((uint)_id) >> 32);
            switch (_type)
            {
                case Type.Step: return value;
                case Type.Job: return -value;
                default: throw new InvalidOperationException("Unknown type encountered");
            }
        }
    }

    /// <summary>
    /// ContextKey class extension.
    /// </summary>
    static class ContextKeyExtension
    {
        /// <summary>
        /// Returns the context key for the given job execution.
        /// </summary>
        /// <param name="jobExecution"></param>
        /// <returns></returns>
        public static ContextKey GetContextKey(this JobExecution jobExecution)
        {
            return new ContextKey(ContextKey.Type.Job, jobExecution.Id);
        }

        /// <summary>
        /// Returns the context key for the given step execution.
        /// </summary>
        /// <param name="stepExecution"></param>
        /// <returns></returns>
        public static ContextKey GetContextKey(this StepExecution stepExecution)
        {
            return new ContextKey(ContextKey.Type.Step, stepExecution.Id);
        }
    }
}
