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
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;

namespace Summer.Batch.Core.Unity.Injection
{
    /// <summary>
    /// Implementation of <see cref="InjectionParameterValue"/> that reads a property from the step context.
    /// </summary>
    /// <typeparam name="T">&nbsp;the type of the resource to get</typeparam>
    public class StepContextValue<T> : TypedInjectionValue
    {
        private readonly string _propertyName;
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="propertyName">the name of the property to read</param>
        public StepContextValue(string propertyName) : base(typeof(T))
        {
            _propertyName = propertyName;
        }

        /// <summary>
        /// Returns the resolver policy for the given type to build.
        /// </summary>
        /// <param name="typeToBuild"></param>
        /// <returns></returns>
        public override IDependencyResolverPolicy GetResolverPolicy(Type typeToBuild)
        {
            return new StepContextDependencyResolverPolicy<T>(_propertyName);
        }
    }
}