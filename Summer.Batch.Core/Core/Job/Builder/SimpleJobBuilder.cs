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
using System.Linq;
using Summer.Batch.Common.TaskExecution;
using Summer.Batch.Common.Util;

namespace Summer.Batch.Core.Job.Builder
{
    /// <summary>
    /// Simple job builder.
    /// </summary>
    public class SimpleJobBuilder : JobBuilderHelper
    {
        private readonly List<IStep> _steps = new List<IStep>();
        private JobFlowBuilder _builder;

        /// <summary>
        /// Creates a new builder initialized with any properties in the parent. 
        /// The parent is copied, so it can be re-used.
        /// </summary>
        /// <param name="parent"></param>
        public SimpleJobBuilder(JobBuilderHelper parent)
            : base(parent)
        {
        }

        /// <summary>
        /// Builds the job.
        /// </summary>
        /// <returns></returns>
        public IJob Build()
        {
            if (_builder != null)
            {
                return _builder.End().Build();
            }
            SimpleJob job = new SimpleJob(GetName());
            Enhance(job);
            job.Steps = _steps;
            try
            {
                job.AfterPropertiesSet();
            }
            catch (Exception e)
            {
                throw new JobBuilderException(e);
            }
            return job;
        }

        /// <summary>
        /// Starts the job with this step.
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        public SimpleJobBuilder Start(IStep step)
        {
            if (_steps.Any())
            {
                _steps[0] = step;
            }
            else
            {
                _steps.Add(step);
            }
            return this;
        }

        /// <summary>
        /// Branches into a flow conditional on the outcome of the current step.
        /// </summary>
        /// <param name="pattern">a pattern for the exit status of the current step</param>
        /// <returns></returns>
        public FlowBuilder<FlowJobBuilder>.TransitionBuilder On(string pattern)
        {
            Assert.State(_steps.Count >= 0, "You have to start a job with a step");
            foreach (var step in _steps)
            {
                if (_builder == null)
                {
                    _builder = new JobFlowBuilder(new FlowJobBuilder(this), step);
                }
                else
                {
                    _builder.Next(step);
                }
            }
            return _builder.On(pattern);
        }

        /// <summary>
        /// Continues or ends a job with this step if the previous step was successful.
        /// </summary>
        /// <param name="step">a step to execute next</param>
        /// <returns></returns>
        public SimpleJobBuilder Next(IStep step)
        {
            _steps.Add(step);
            return this;
        }

        /// <summary>
        /// Splits, using the given task executor.
        /// </summary>
        /// <param name="executor"></param>
        /// <returns></returns>
        public JobFlowBuilder.SplitBuilder Split(ITaskExecutor executor)
        {
            foreach (IStep step in _steps)
            {
                if (_builder == null)
                {
                    _builder = new JobFlowBuilder(new FlowJobBuilder(this), step);
                }
                else
                {
                    _builder.Next(step);
                }
            }
            if (_builder == null)
            {
                _builder = new JobFlowBuilder(new FlowJobBuilder(this));
            }
            return _builder.Split(executor);
        }
    }
}