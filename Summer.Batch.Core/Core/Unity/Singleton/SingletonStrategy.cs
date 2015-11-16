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
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;

namespace Summer.Batch.Core.Unity.Singleton
{
    /// <summary>
    /// Unity builder strategy that changes the default lifetime manager to <see cref="ContainerControlledLifetimeManager"/>.
    /// It needs to be executed before <see cref="LifetimeStrategy"/>, and thus should be registered with the
    /// <see cref="Microsoft.Practices.Unity.ObjectBuilder.UnityBuildStage.TypeMapping"/> stage.
    /// </summary>
    public class SingletonStrategy : BuilderStrategy
    {
        /// <summary>
        /// Checks if the instance being resolved has a lifetime manager. If it does not, an instance of
        /// <see cref="ContainerControlledLifetimeManager"/> is registered, which will then be used by
        /// <see cref="LifetimeStrategy"/>.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        public override void PreBuildUp(IBuilderContext context)
        {
            if (context.Existing == null && !HasLifetimeManager(context))
            {
                ILifetimePolicy lifetimeManager = new ContainerControlledLifetimeManager();
                context.PersistentPolicies.Set(lifetimeManager, context.BuildKey);
            }
        }

        // Checks if the instance being resolved has a lifetime manager
        private static bool HasLifetimeManager(IBuilderContext context)
        {
            var policy = context.Policies.GetNoDefault<ILifetimePolicy>(context.BuildKey, false);
            return policy != null ||
                   (context.BuildKey.Type.GetTypeInfo().IsGenericType && HasLifetimeFactoryPolicy(context));
        }

        // Checks if the instance being resolved has a lifetime manager factory
        private static bool HasLifetimeFactoryPolicy(IBuilderContext context)
        {
            var openGenericBuildKey = new NamedTypeBuildKey(context.BuildKey.Type.GetGenericTypeDefinition(),
                context.BuildKey.Name);

            IPolicyList factorySource;
            var factoryPolicy = context.Policies.Get<ILifetimeFactoryPolicy>(openGenericBuildKey, out factorySource);

            return factoryPolicy != null;
        }
    }
}