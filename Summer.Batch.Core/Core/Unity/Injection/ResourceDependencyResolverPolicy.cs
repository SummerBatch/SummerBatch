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
using Summer.Batch.Common.IO;

namespace Summer.Batch.Core.Unity.Injection
{
    /// <summary>
    /// Implementation of <see cref="IDependencyResolverPolicy"/> that resolves a URI to an <see cref="IResource"/>.
    /// </summary>
    public class ResourceDependencyResolverPolicy : IDependencyResolverPolicy
    {
        private readonly IDependencyResolverPolicy _uriResolver;
        private readonly bool _many;

        /// <summary>
        /// Constructs a new <see cref="ResourceDependencyResolverPolicy"/>.
        /// </summary>
        /// <param name="uriResolver">a resolver that returns a URI</param>
        /// <param name="many">true if there can be several resources; false if only one is expected</param>
        public ResourceDependencyResolverPolicy(IDependencyResolverPolicy uriResolver, bool many)
        {
            _uriResolver = uriResolver;
            _many = many;
        }

        /// <summary>
        /// Resolve object from given context.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public object Resolve(IBuilderContext context)
        {
            var uri = (string) _uriResolver.Resolve(context);
            var resourceLoader = context.NewBuildUp<ResourceLoader>();
            if (_many)
            {
                return resourceLoader.GetResources(uri);
            }
            return resourceLoader.GetResource(uri);
        }
    }
}