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
using Summer.Batch.Common.IO;

namespace Summer.Batch.Core.Unity.Injection
{
    /// <summary>
    /// Implementation of <see cref="TypedInjectionValue"/> that resolves a URI to an <see cref="IResource"/>.
    /// </summary>
    public class ResourceInjectionValue : TypedInjectionValue
    {
        private readonly InjectionParameterValue _injectionParameterValue;
        private readonly bool _many;

        /// <summary>
        /// Constructs a new <see cref="ResourceInjectionValue"/> with a string URI.
        /// </summary>
        /// <param name="uri">a string representing a resource</param>
        /// <param name="many">true if there can be several resources; false if only one is expected</param>
        public ResourceInjectionValue(string uri, bool many = false)
            : base(many ? typeof(IList<IResource>) : typeof(IResource))
        {
            _injectionParameterValue = new LateBindingInjectionValue<string>(uri);
            _many = many;
        }

        /// <summary>
        /// Constructs a new <see cref="ResourceInjectionValue"/> with an <see cref="InjectionParameterValue"/>.
        /// </summary>
        /// <param name="injectionParameterValue"><see cref="InjectionParameterValue"/> that resolves to a resource URI</param>
        /// <param name="many">true if there can be several resources; false if only one is expected</param>
        public ResourceInjectionValue(InjectionParameterValue injectionParameterValue, bool many = false)
            : base(many ? typeof(IList<IResource>) : typeof(IResource))
        {
            _injectionParameterValue = injectionParameterValue;
            _many = many;
        }

        /// <summary>
        /// Returns the resolver policy for the given type to build.
        /// </summary>
        /// <param name="typeToBuild"></param>
        /// <returns></returns>
        public override IDependencyResolverPolicy GetResolverPolicy(Type typeToBuild)
        {
            var uriResolver = _injectionParameterValue.GetResolverPolicy(typeToBuild);
            return new ResourceDependencyResolverPolicy(uriResolver, _many);
        }
    }
}