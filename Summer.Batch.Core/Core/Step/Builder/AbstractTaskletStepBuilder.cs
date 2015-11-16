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
using Microsoft.Practices.Unity;
using Summer.Batch.Core.Step.Tasklet;
using Summer.Batch.Infrastructure.Repeat;
using Summer.Batch.Infrastructure.Repeat.Exception;
using Summer.Batch.Infrastructure.Repeat.Support;
using Summer.Batch.Common.TaskExecution;

namespace Summer.Batch.Core.Step.Builder
{
    /// <summary>
    /// Base class for tasklet based step builders.
    /// </summary>
    public abstract class AbstractTaskletStepBuilder : AbstractStepBuilder
    {
        /// <summary>
        /// TaskletSuffix constant.
        /// </summary>
        protected const string TaskletSuffix = "__tasklet";

        private ITaskExecutor _taskExecutor;
        private IRepeatOperations _stepOperations;
        private IExceptionHandler _exceptionHandler = new DefaultExceptionHandler();
        private int _throttleLimit = TaskExecutorRepeatTemplate.DefaultThrottleLimit;
        private readonly IList<Tuple<Type, string>> _streams = new List<Tuple<Type, string>>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="container">the container used to resolve the step</param>
        /// <param name="name">the name of the step</param>
        protected AbstractTaskletStepBuilder(IUnityContainer container, string name)
            : base(container, name)
        {
        }

        /// <summary>
        /// Sets the task executor.
        /// </summary>
        /// <param name="taskExecutor">the task executor</param>
        /// <returns>the current step builder</returns>
        public AbstractTaskletStepBuilder TaskExecutor(ITaskExecutor taskExecutor)
        {
            _taskExecutor = taskExecutor;
            return this;
        }

        /// <summary>
        /// Sets step operations.
        /// </summary>
        /// <param name="stepOperations">the step operations</param>
        /// <returns>the current step builder</returns>
        public AbstractTaskletStepBuilder StepOperations(IRepeatOperations stepOperations)
        {
            _stepOperations = stepOperations;
            return this;
        }

        /// <summary>
        /// Sets the exception handler.
        /// </summary>
        /// <param name="exceptionHandler">the exception handler</param>
        /// <returns>the current step builder</returns>
        public AbstractTaskletStepBuilder ExceptionHander(IExceptionHandler exceptionHandler)
        {
            _exceptionHandler = exceptionHandler;
            return this;
        }

        /// <summary>
        /// Sets the throttle limit.
        /// </summary>
        /// <param name="throttleLimit">the throttle limit</param>
        /// <returns>the current step builder</returns>
        public AbstractTaskletStepBuilder ThrottleLimit(int throttleLimit)
        {
            _throttleLimit = throttleLimit;
            return this;
        }

        /// <summary>
        /// Adds a new stream to the step.
        /// </summary>
        /// <param name="type">the type to use when resolving the stream</param>
        /// <param name="stream">the name to use when resolving the stream</param>
        /// <returns>the current step builder</returns>
        public AbstractTaskletStepBuilder Stream(Type type, string stream)
        {
            _streams.Add(new Tuple<Type, string>(type, stream));
            return this;
        }

        /// <summary>
        /// Override Type.
        /// </summary>
        protected override Type Type
        {
            get { return typeof(TaskletStep); }
        }

        /// <summary>
        /// Override InjectionMembers.
        /// </summary>
        protected sealed override IEnumerable<InjectionMember> InjectionMembers
        {
            get
            {
                var taskletName = RegisterTasklet();

                var injectionMembers = new List<InjectionMember>
                {
                    new InjectionConstructor(Name),
                    new InjectionProperty("Tasklet", new ResolvedParameter<ITasklet>(taskletName)),
                    new InjectionProperty("StepOperations", GetStepOperations())
                };
                injectionMembers.AddRange(_streams.Select(stream =>
                    new InjectionMethod("RegisterStream", new ResolvedParameter(stream.Item1, stream.Item2))));
                return injectionMembers;
            }
        }

        /// <summary>
        /// Registers the tasklet with the given name.
        /// </summary>
        /// <returns>the name of the tasklet</returns>
        protected abstract string RegisterTasklet();

        private IRepeatOperations GetStepOperations()
        {
            if (_stepOperations != null)
            {
                return _stepOperations;
            }
            if (_taskExecutor != null)
            {
                return new TaskExecutorRepeatTemplate
                {
                    TaskExecutor = _taskExecutor,
                    ThrottleLimit = _throttleLimit,
                    ExceptionHandler = _exceptionHandler
                };
            }
            return new RepeatTemplate { ExceptionHandler = _exceptionHandler };
        }
    }
}