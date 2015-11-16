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

using Summer.Batch.Common.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Summer.Batch.Core.Repository.Dao
{
    /// <summary>
    /// In-memory implementation of <see cref="IStepExecutionDao"/>.
    /// </summary>
    public class MapStepExecutionDao : IStepExecutionDao
    {
        private readonly IDictionary<long?, IDictionary<long?, StepExecution>> _executionsByJobExecutionId = new ConcurrentDictionary<long?, IDictionary<long?, StepExecution>>();
        private readonly IDictionary<long?, StepExecution> _executionsByStepExecutionId = new ConcurrentDictionary<long?, StepExecution>();
        private long _currentId;

        /// <summary>
        /// Clears Job executions dictionary.
        /// </summary>
        public void Clear()
        {
            _executionsByJobExecutionId.Clear();
            _executionsByStepExecutionId.Clear();
        }

        /// <summary>
        /// Copies using serialization/deserialization.
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        private static StepExecution Copy(StepExecution original)
        {
            return original.Serialize().Deserialize<StepExecution>();
        }

        #region IStepExecutionDao methods implementation
        /// <summary>
        /// @see IStepExecutionDao#SaveStepExecution.
        /// </summary>
        /// <param name="stepExecution"></param>
        public void SaveStepExecution(StepExecution stepExecution)
        {
            Assert.IsTrue(stepExecution.Id == null);
            Assert.IsTrue(stepExecution.Version == null);
            var jobExecutionId = stepExecution.GetJobExecutionId();
            if (jobExecutionId == null)
            {
                throw new ArgumentException("The corresponding job execution must have already been saved.");
            }

            IDictionary<long?, StepExecution> executions;
            if (!_executionsByJobExecutionId.TryGetValue(jobExecutionId, out executions))
            {
                executions = new ConcurrentDictionary<long?, StepExecution>();
                _executionsByJobExecutionId[jobExecutionId] = executions;
            }

            stepExecution.Id = Interlocked.Increment(ref _currentId);
            stepExecution.IncrementVersion();
            StepExecution copy = Copy(stepExecution);
            executions[stepExecution.Id] = copy;
            _executionsByStepExecutionId[stepExecution.Id] = copy;
        }

        /// <summary>
        /// @see IStepExecutionDao#SaveStepExecutions.
        /// </summary>
        /// <param name="stepExecutions"></param>
        public void SaveStepExecutions(ICollection<StepExecution> stepExecutions)
        {
            Assert.NotNull(stepExecutions, "Attempt to save a null collection of step executions");
            foreach (StepExecution stepExecution in stepExecutions)
            {
                SaveStepExecution(stepExecution);
            }
        }

        /// <summary>
        /// @see IStepExecutionDao#UpdateStepExecution.
        /// </summary>
        /// <param name="stepExecution"></param>
        public void UpdateStepExecution(StepExecution stepExecution)
        {
            IDictionary<long?, StepExecution> executions;
            StepExecution persisted;
            var jobExecutionId = stepExecution.GetJobExecutionId();

            if (jobExecutionId == null || !_executionsByJobExecutionId.TryGetValue(jobExecutionId, out executions) ||
                stepExecution.Id == null || !_executionsByStepExecutionId.TryGetValue(stepExecution.Id, out persisted))
            {
                throw new ArgumentException("The step execution must have already been saved.");
            }

            lock (stepExecution)
            {
                if (persisted.Version != stepExecution.Version)
                {
                    throw new ArgumentException(string.Format("Attempt to update step execution (id={0}) with version {1}, but current version is {2}.",
                        stepExecution.Id, stepExecution.Version, persisted.Version));
                }
                stepExecution.IncrementVersion();
                StepExecution copy = Copy(stepExecution);
                executions[stepExecution.Id] = copy;
                _executionsByStepExecutionId[stepExecution.Id] = copy;
            }
        }

        /// <summary>
        /// @see IStepExecutionDao#GetStepExecution.
        /// </summary>
        /// <param name="jobExecution"></param>
        /// <param name="stepExecutionId"></param>
        /// <returns></returns>
        public StepExecution GetStepExecution(JobExecution jobExecution, long stepExecutionId)
        {
            StepExecution result;
            _executionsByStepExecutionId.TryGetValue(stepExecutionId, out result);
            return result;
        }

        /// <summary>
        /// @see IStepExecutionDao#AddStepExecutions.
        /// </summary>
        /// <param name="jobExecution"></param>
        public void AddStepExecutions(JobExecution jobExecution)
        {
            IDictionary<long?, StepExecution> executions;
            if (jobExecution.Id == null || (!_executionsByJobExecutionId.TryGetValue(jobExecution.Id, out executions) || executions.Count == 0))
            {
                return;
            }
            IList<StepExecution> result = new List<StepExecution>(executions.Values).OrderByDescending(s => s.Id).ToList();

            IList<StepExecution> copy = result.Select(Copy).ToList();
            jobExecution.AddStepExecutions(copy);
        } 
        #endregion
    }
}
