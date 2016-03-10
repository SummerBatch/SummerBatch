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

using Summer.Batch.Core.Step;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Summer.Batch.Core.Job.Flow
{
    /// <summary>
    /// Implementation of the <see cref="IJob"/> interface that allows for complex flows of
    /// steps, rather than requiring sequential execution. In general, this job
    /// implementation was designed to be used behind a parser, allowing for a
    /// namespace to abstract away details.
    /// </summary>
    public class FlowJob : AbstractJob
    {
        /// <summary>
        /// Flow property.
        /// </summary>
        public IFlow Flow { protected get; set; }
        private readonly IDictionary<string, IStep> _stepMap = new ConcurrentDictionary<string, IStep>();
        private volatile bool _initialized = false;

        /// <summary>
        /// Custom constructor with name.
        /// </summary>
        /// <param name="name"></param>
        public FlowJob(string name) : base(name) { }

        /// <summary>
        /// @see AbstractJob#DoExecute .
        /// </summary>
        /// <param name="execution"></param>
        /// <exception cref="JobExecutionException">&nbsp;</exception>
        protected override void DoExecute(JobExecution execution)
        {
            try
            {
                JobFlowExecutor executor =
                    new JobFlowExecutor(JobRepository,
                    new SimpleStepHandler(JobRepository),
                    execution);
                executor.UpdateJobExecutionStatus(Flow.Start(executor).Status);
            }
            catch (FlowExecutionException e)
            {
                var exception = e.InnerException as JobExecutionException;
                if (exception != null)
                {
                    throw exception;
                }
                throw new JobExecutionException("Flow execution ended unexpectedly", e);
            }
        }

        /// <summary>
        /// @see AbstractJob#GetStepNames .
        /// </summary>
        /// <returns></returns>
        public override ICollection<string> GetStepNames()
        {
            if (!_initialized)
            {
                Init();
            }
            return _stepMap.Keys;
        }

        /// <summary>
        /// @see AbstractJob#GetStep .
        /// </summary>
        /// <param name="stepName"></param>
        /// <returns></returns>
        public override IStep GetStep(string stepName)
        {
            if (!_initialized)
            {
                Init();
            }
            IStep result;
            _stepMap.TryGetValue(stepName, out result);
            return result;
        }

        /// <summary>
        /// Initialize the step names.
        /// </summary>
        private void Init()
        {
            FindSteps(Flow, _stepMap);
            //don't do init twice
            _initialized = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flow"></param>
        /// <param name="map"></param>
        private void FindSteps(IFlow flow, IDictionary<string, IStep> map)
        {
            foreach (IState state in flow.GetStates())
            {
                var stepLocator = state as IStepLocator;
                if (stepLocator != null)
                {
                    IStepLocator locator = stepLocator;
                    foreach (string name in locator.GetStepNames())
                    {
                        map.Add(name, locator.GetStep(name));
                    }
                }
                else
                {
                    var holder = state as IStepHolder;
                    if (holder != null)
                    {
                        IStep step = holder.Step;
                        string name = step.Name;
                        _stepMap.Add(name, step);
                    }
                    else
                    {
                        var flowHolder = state as IFlowHolder;
                        if (flowHolder != null)
                        {
                            foreach (IFlow subflow in flowHolder.GetFlows())
                            {
                                FindSteps(subflow, map);
                            }
                        }
                    }
                }
            }
        }

    }
}