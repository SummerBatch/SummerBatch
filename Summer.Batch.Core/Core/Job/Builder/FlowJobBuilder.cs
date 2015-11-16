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
using Summer.Batch.Core.Job.Flow;

namespace Summer.Batch.Core.Job.Builder
{
    /// <summary>
    /// A job builder for FlowJob instances. A flow job delegates processing to a nested flow composed of steps and
    /// conditional transitions between steps.
    /// </summary>
    public class FlowJobBuilder : JobBuilderHelper
    {
        private IFlow _flow;

        /// <summary>
        /// Creates a new builder initialized with any properties in the parent. The parent is copied, so it can be re-used.
        /// </summary>
        /// <param name="parent"></param>
        public FlowJobBuilder(JobBuilderHelper parent) : base(parent)
        {
            
        }

        /// <summary>
        /// Starts a job with this flow, but expect to transition from there to other flows or steps.
        /// </summary>
        /// <param name="flow"></param>
        /// <returns></returns>
        public JobFlowBuilder Start(IFlow flow)
        {
            return new JobFlowBuilder(this, flow);
        }

        /// <summary>
        /// Starts a job with this step, but expect to transition from there to other flows or steps.
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        public JobFlowBuilder Start(IStep step)
        {
            return new JobFlowBuilder(this, step);
        }

        /// <summary>
        /// Provides a single flow to execute as the job.
        /// </summary>
        /// <param name="flow"></param>
        /// <returns></returns>
        public FlowJobBuilder Flow(IFlow flow)
        {
            _flow = flow;
            return this;
        }

        /// <summary>
        /// Builds a job that executes the flow provided, normally composed of other steps.
        /// </summary>
        /// <returns></returns>
        public IJob Build()
        {
            FlowJob job = new FlowJob(GetName()) {Flow = _flow};
            Enhance(job);
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
    }
}