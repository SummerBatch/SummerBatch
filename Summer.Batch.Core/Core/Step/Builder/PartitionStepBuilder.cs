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
using Microsoft.Practices.Unity;
using Summer.Batch.Core.Partition;
using Summer.Batch.Core.Partition.Support;
using Summer.Batch.Common.TaskExecution;

namespace Summer.Batch.Core.Step.Builder
{
    /// <summary>
    /// Step builder for partition steps.
    /// </summary>
    public class PartitionStepBuilder : AbstractStepBuilder
    {
        private const int DefaultGridSize = 6;

        private IStep _step;
        private ITaskExecutor _taskExecutor;
        private IPartitioner _partitioner;
        private IPartitionHandler _partitionHandler;
        private int _gridSize = DefaultGridSize;
        private IStepExecutionSplitter _splitter;
        private string _stepName;
        private IStepExecutionAggregator _aggregator;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="container">the container used to resolve the step</param>
        /// <param name="name">the name of the step</param>
        public PartitionStepBuilder(IUnityContainer container, string name)
            : base(container, name)
        {
        }

        /// <summary>
        /// Sets the step to partition.
        /// </summary>
        /// <param name="step">the step to partition</param>
        /// <returns>the current step builder</returns>
        public PartitionStepBuilder Step(IStep step)
        {
            _step = step;
            return this;
        }

        /// <summary>
        /// Sets the task executor.
        /// </summary>
        /// <param name="taskExecutor">the task executor</param>
        /// <returns>the current step builder</returns>
        public PartitionStepBuilder TaskExecutor(ITaskExecutor taskExecutor)
        {
            _taskExecutor = taskExecutor;
            return this;
        }

        /// <summary>
        /// Sets the partitioner.
        /// </summary>
        /// <param name="slaveStepName">the name of the partitioned step</param>
        /// <param name="name">the partitioner</param>
        /// <returns>the current step builder</returns>
        public PartitionStepBuilder Partitioner(string slaveStepName, IPartitioner name)
        {
            _stepName = slaveStepName;
            _partitioner = name;
            return this;
        }

        /// <summary>
        /// Sets the partition handler.
        /// </summary>
        /// <param name="partitionHandler">the partition handler</param>
        /// <returns>the current step builder</returns>
        public PartitionStepBuilder PartitionHander(IPartitionHandler partitionHandler)
        {
            _partitionHandler = partitionHandler;
            return this;
        }

        /// <summary>
        /// Sets the grid size.
        /// </summary>
        /// <param name="gridSize">the grid size</param>
        /// <returns>the current step builder</returns>
        public PartitionStepBuilder GridSize(int gridSize)
        {
            _gridSize = gridSize > 0 ? gridSize : DefaultGridSize;
            return this;
        }

        /// <summary>
        /// Sets the splitter.
        /// </summary>
        /// <param name="splitter">the splitter</param>
        /// <returns>the current step builder</returns>
        public PartitionStepBuilder Splitter(IStepExecutionSplitter splitter)
        {
            _splitter = splitter;
            return this;
        }

        /// <summary>
        /// Sets the execution aggregator.
        /// </summary>
        /// <param name="aggregator">the execution aggregator</param>
        /// <returns>the current step builder</returns>
        public PartitionStepBuilder Aggregator(IStepExecutionAggregator aggregator)
        {
            _aggregator = aggregator;
            return this;
        }

        /// <summary>
        /// Override Type.
        /// </summary>
        protected override Type Type
        {
            get { return typeof(PartitionStep); }
        }

        /// <summary>
        /// Override InjectionMembers.
        /// </summary>
        protected override IEnumerable<InjectionMember> InjectionMembers
        {
            get
            {
                var injectionMembers = new List<InjectionMember>
                {
                    new InjectionConstructor(Name),
                    new InjectionProperty("PartitionHandler", GetPartitionHandler()),
                    new InjectionProperty("StepExecutionSplitter", GetSpliter())
                };
                if (_aggregator != null)
                {
                    injectionMembers.Add(new InjectionProperty("StepExecutionAggregator", _aggregator));
                }

                return injectionMembers;
            }
        }

        private IPartitionHandler GetPartitionHandler()
        {
            if (_partitionHandler != null)
            {
                return _partitionHandler;
            }
            return new TaskExecutorPartitionHandler
            {
                Step = _step,
                TaskExecutor = _taskExecutor ?? new SyncTaskExecutor(),
                GridSize = _gridSize
            };
        }

        private IStepExecutionSplitter GetSpliter()
        {
            if (_splitter != null)
            {
                return _splitter;
            }
            var name = _stepName;
            var allowStartIfComplete = AllowStartIfComplete;
            if (_step != null)
            {
                name = _step.Name;
                allowStartIfComplete = _step.AllowStartIfComplete ?? false;
            }
            return new SimpleStepExecutionSplitter
            {
                Partitioner = _partitioner,
                JobRepository = JobRepository,
                AllowStartIfComplete = allowStartIfComplete,
                StepName = name
            };
        }
    }
}