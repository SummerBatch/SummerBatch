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
using System;
using System.Collections.Generic;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;

namespace Summer.Batch.Core.Unity.StepScope
{
    /// <summary>
    /// Implementation of <see cref="ResolverOverride"/> that overrides dependencies in the step scope.
    /// </summary>
    public class StepScopeOverride : ResolverOverride
    {
        private readonly IDictionary<string, StepScopeDependency> _constructorParameters;
        private readonly IDictionary<string, StepScopeDependency> _properties;
        private readonly IDictionary<Tuple<string, string>, IList<StepScopeDependency>> _methodParameters;
        private readonly IDictionary<Tuple<string, string>, int> _methodCalls = new Dictionary<Tuple<string, string>, int>();

        /// <summary>
        /// Creates a new <see cref="StepScopeOverride"/>.
        /// </summary>
        /// <param name="constructorParameters">the constructor parameter injections to override</param>
        /// <param name="properties">the properties injection to override</param>
        /// <param name="methodParameters">the method parameter injections to override</param>
        public StepScopeOverride(IDictionary<string, StepScopeDependency> constructorParameters,
                                 IDictionary<string, StepScopeDependency> properties,
                                 IDictionary<Tuple<string, string>, IList<StepScopeDependency>> methodParameters)
        {
            _constructorParameters = constructorParameters;
            _properties = properties;
            _methodParameters = methodParameters;
        }

        /// <summary>
        /// Returns a <see cref="StepScopeResolverPolicy"/> for dependencies in the step scope.
        /// </summary>
        /// <param name="context">the current build context</param>
        /// <param name="dependencyType">the type of dependency</param>
        /// <returns>a <see cref="StepScopeResolverPolicy"/> if the dependency being resolved in is step scope; null otherwise</returns>
        public override IDependencyResolverPolicy GetResolver(IBuilderContext context, Type dependencyType)
        {
            var constructorParameterOperation = context.CurrentOperation as ConstructorArgumentResolveOperation;
            StepScopeDependency dependency;
            if (constructorParameterOperation != null &&
                _constructorParameters.TryGetValue(constructorParameterOperation.ParameterName, out dependency))
            {
                return new StepScopeResolverPolicy(dependency);
            }

            var propertyValueOperation = context.CurrentOperation as ResolvingPropertyValueOperation;
            if (propertyValueOperation != null && _properties.TryGetValue(propertyValueOperation.PropertyName, out dependency))
            {
                return new StepScopeResolverPolicy(dependency);
            }

            var methodParameterOperation = context.CurrentOperation as MethodArgumentResolveOperation;
            if (methodParameterOperation != null && 
                TryGetMethodParameterDependency(
                    new Tuple<string, string>(methodParameterOperation.MethodSignature, methodParameterOperation.ParameterName),
                    out dependency))
            {
                return new StepScopeResolverPolicy(dependency);
            }

            return null;
        }

        private bool TryGetMethodParameterDependency(Tuple<string, string> key, out StepScopeDependency dependency)
        {
            int callNb;
            if (!_methodCalls.TryGetValue(key, out callNb))
            {
                callNb = 0;
            }
            var found = false;
            IList<StepScopeDependency> dependencies;
            if (_methodParameters.TryGetValue(key, out dependencies) && dependencies.Count > callNb)
            {
                found = true;
                dependency = dependencies[callNb];
            }
            else
            {
                dependency = default(StepScopeDependency);
            }
            _methodCalls[key] = callNb + 1;
            return found;
        }
    }
}