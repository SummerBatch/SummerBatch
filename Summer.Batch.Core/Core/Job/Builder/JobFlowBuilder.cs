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
using Summer.Batch.Common.Factory;

namespace Summer.Batch.Core.Job.Builder
{
    /// <summary>
    /// Job Flow builder.
    /// </summary>
    public class JobFlowBuilder : FlowBuilder<FlowJobBuilder>
    {
        private readonly FlowJobBuilder _parent;

        #region Constructors
        /// <summary>
        /// Custom constructor using a parent JobFlowBuilder.
        /// </summary>
        /// <param name="parent"></param>
        public JobFlowBuilder(FlowJobBuilder parent)
            : base(parent.GetName())
        {
            _parent = parent;
        }

        /// <summary>
        /// Custom constructor using a parent JobFlowBuilder and a step.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="step"></param>
        public JobFlowBuilder(FlowJobBuilder parent, IStep step)
            : base(parent.GetName())
        {
            _parent = parent;
            Start(step);
        }

        /// <summary>
        /// Custom constructor using a parent JobFlowBuilder and a flow.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="flow"></param>
        public JobFlowBuilder(FlowJobBuilder parent, IFlow flow)
            : base(parent.GetName())
        {
            _parent = parent;
            Start(flow);
        }
        #endregion

        /// <summary>
        /// Builds a flow and inject it into the parent builder. The parent builder is then returned so it can be enhanced
        /// before building an actual job.  Normally called explicitly via End().
        /// </summary>
        /// <returns></returns>
        public override FlowJobBuilder Build()
        {
            IFlow flow = Flow();

            var operations = flow as IInitializationPostOperations;
            if (operations != null)
            {
                try
                {
                    operations.AfterPropertiesSet();
                }
                catch (Exception e)
                {
                    throw new FlowBuilderException(e);
                }
            }

            _parent.Flow(flow);
            return _parent;
        }
    }
}