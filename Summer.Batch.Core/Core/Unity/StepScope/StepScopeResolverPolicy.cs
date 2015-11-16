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
//  distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
using Microsoft.Practices.ObjectBuilder2;
using Summer.Batch.Core.Scope;
using Summer.Batch.Common.Proxy;

namespace Summer.Batch.Core.Unity.StepScope
{
    /// <summary>
    /// A dependency resolver used when resolving dependencies in the step scope for an instance that is not
    /// in the step scope itself. It creates a proxy object, the underlying instance of which will be updated
    /// at the start of each step.
    /// </summary>
    public class StepScopeResolverPolicy : IDependencyResolverPolicy
    {
        private readonly StepScopeDependency _dependency;

        /// <summary>
        /// Creates a new <see cref="StepScopeResolverPolicy"/>.
        /// </summary>
        /// <param name="dependency">the dependency to resolve</param>
        public StepScopeResolverPolicy(StepScopeDependency dependency)
        {
            _dependency = dependency;
        }

        /// <summary>
        /// Resolves the dependency by crating a proxy and registers it with <see cref="StepScopeSynchronization"/>.
        /// </summary>
        /// <param name="context">Current build context</param>
        /// <returns>The created proxy</returns>
        public object Resolve(IBuilderContext context)
        {
            var mappedType = StepScopeSynchronization.GetMappedType(_dependency.RegisteredType, _dependency.Name);
            var proxy = ProxyFactory.Create(mappedType.GetInterfaces(), typeof(StepScopeProxyObject));
            StepScopeSynchronization.AddProxy(_dependency.RegisteredType, _dependency.Name, (IProxyObject)proxy);
            return proxy;
        }
    }
}