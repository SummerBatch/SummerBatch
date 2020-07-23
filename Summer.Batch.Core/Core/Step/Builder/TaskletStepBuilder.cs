﻿//
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

using Microsoft.Practices.Unity;

namespace Summer.Batch.Core.Step.Builder
{
    /// <summary>
    /// Step builder for steps with custom tasklet.
    /// </summary>
    public class TaskletStepBuilder : AbstractTaskletStepBuilder
    {
        private string _tasklet;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="container">the container used to resolve the step</param>
        /// <param name="name">the name of the step</param>
        public TaskletStepBuilder(IUnityContainer container, string name) : base(container, name)
        {
        }

        public TaskletStepBuilder(IUnityContainer container, string name, int delayConfig) : base(container, name, delayConfig)
        {
        }
        /// <summary>
        /// Sets the tasklet.
        /// </summary>
        /// <param name="tasklet">the name of the tasklet</param>
        /// <returns>the current step builder</returns>
        public TaskletStepBuilder Tasklet(string tasklet)
        {
            _tasklet = tasklet;
            return this;
        }

        /// <summary>
        /// Override RegisterTasklet.
        /// </summary>
        /// <returns></returns>
        protected override string RegisterTasklet()
        {
            return _tasklet;
        }
    }
}