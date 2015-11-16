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

using Summer.Batch.Core.Job.Flow;

namespace Summer.Batch.Core.Job.Builder
{
    /// <summary>
    /// Convenient entry point for building jobs of various kinds.
    /// </summary>
    public class JobBuilder : JobBuilderHelper
    {
        #region Constructors
        /// <summary>
        /// Custom constructor using a name.
        /// </summary>
        /// <param name="name"></param>
        public JobBuilder(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Custom constructor using parent.
        /// </summary>
        /// <param name="parent"></param>
        public JobBuilder(JobBuilderHelper parent)
            : base(parent)
        {
        }
        #endregion

        /// <summary>
        /// Creates a new job builder that will execute a step or sequence of steps.
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        public SimpleJobBuilder Start(IStep step)
        {
            return new SimpleJobBuilder(this).Start(step);
        }

        /// <summary>
        /// Creates a new job builder that will execute a flow.
        /// </summary>
        /// <param name="flow"></param>
        /// <returns></returns>
        public JobFlowBuilder Start(IFlow flow)
        {
            return new FlowJobBuilder(this).Start(flow);
        }

        /// <summary>
        /// Creates a new job builder that will execute a step or sequence of steps.
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        public JobFlowBuilder Flow(IStep step)
        {
            return new FlowJobBuilder(this).Start(step);
        }
    }
}