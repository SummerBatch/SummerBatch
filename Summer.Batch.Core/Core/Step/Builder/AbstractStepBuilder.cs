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
using Summer.Batch.Core.Repository;

namespace Summer.Batch.Core.Step.Builder
{
    /// <summary>
    /// Base class for all step builders. Holds the common attributes to all steps.
    /// </summary>
    public abstract class AbstractStepBuilder
    {
        /// <summary>
        /// The container that will be used to resolve the step.
        /// </summary>
        protected readonly IUnityContainer Container;

        /// <summary>
        /// The listeners to register for the step being built, identified by their type and name in the container.
        /// </summary>
        private readonly List<Tuple<Type, string>> _stepExecutionListeners = new List<Tuple<Type, string>>();   

        /// <summary>
        /// The name of the step.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Start limit property.
        /// </summary>
        public int StartLimit { get; set; }

        /// <summary>
        /// Allow start if complete flag property.
        /// </summary>
        public bool AllowStartIfComplete { get; set; }

        /// <summary>
        /// The job repository of the step.
        /// </summary>
        public IJobRepository JobRepository { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="container">the container used to resolve the step</param>
        /// <param name="name">the name of the step</param>
        protected AbstractStepBuilder(IUnityContainer container, string name)
        {
            Container = container;
            Name = name;
            StartLimit = int.MaxValue;
        }

        /// <summary>
        /// Sets the job repository.
        /// </summary>
        /// <param name="jobRepository">the new job repository</param>
        /// <returns>the current step builder</returns>
        public AbstractStepBuilder Repository(IJobRepository jobRepository)
        {
            JobRepository = jobRepository;
            return this;
        }

        /// <summary>
        /// Adds a new listener.
        /// </summary>
        /// <param name="type">the type to use when resolving the listener</param>
        /// <param name="listener">the name to use when resolving the listener</param>
        /// <returns>the current step builder</returns>
        public AbstractStepBuilder Listener(Type type, string listener)
        {
            _stepExecutionListeners.Add(new Tuple<Type, string>(type, listener));
            return this;
        }

        /// <summary>
        /// Builds the step.
        /// </summary>
        /// <returns>the built step</returns>
        public IStep Build()
        {
            var injectionMembers = InjectionMembers.Concat(GetAdditionalInjectionMembers()).ToArray();
            Container.RegisterType(typeof (IStep), Type, Name, new ContainerControlledLifetimeManager(), injectionMembers);
            return Container.Resolve<IStep>(Name);
        }

        /// <summary>
        /// The concrete type of the step to build.
        /// </summary>
        protected abstract Type Type { get; }

        /// <summary>
        /// The injection members to register for the step.
        /// </summary>
        protected abstract IEnumerable<InjectionMember> InjectionMembers { get; }

        /// <returns>the injection memebers defined by <see cref="AbstractStepBuilder"/></returns>
        private IEnumerable<InjectionMember> GetAdditionalInjectionMembers()
        {
            var injectionMembers = new List<InjectionMember>
            {
                new InjectionProperty("JobRepository", JobRepository),
                new InjectionProperty("AllowStartIfComplete", AllowStartIfComplete),
                new InjectionProperty("StartLimit", StartLimit)
            };
            injectionMembers.AddRange(_stepExecutionListeners.Select(listener =>
                new InjectionMethod("RegisterStepExecutionListener", new ResolvedParameter(listener.Item1, listener.Item2))));

            return injectionMembers;
        }
    }
}