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
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.ObjectBuilder;
using NLog;
using Summer.Batch.Core.Scope;

namespace Summer.Batch.Core.Unity.StepScope
{
    /// <summary>
    /// Builder strategy for step scope. It should be added at the <see cref="UnityBuildStage.PreCreation"/> stage.
    /// Before the instance is created, it checks resolvers for the constructor parameters, properties, and method
    /// parameters. If any dependency is in the step scope but the current instance being built is not, the resolver
    /// is overriden using <see cref="StepScopeOverride"/>.
    /// </summary>
    public class StepScopeStrategy : BuilderStrategy
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Checks if resolvers need to be overriden.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        public override void PreBuildUp(IBuilderContext context)
        {
            var typeToBuild = context.BuildKey.Type;
            //
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("StepScopeStrategy > PreBuildUp > typeToBuild :[{0}]", typeToBuild.FullName);
            }
            //
            if (typeToBuild.IsArray)
            {
                return;
            }

            var lifetimePolicy = context.Policies.GetNoDefault<ILifetimePolicy>(context.BuildKey, false);

            // Constructor parameters resolvers
            var constructorParameters = new Dictionary<string, StepScopeDependency>();
            IPolicyList resolverPolicyDestination;
            var constructorSelector = context.Policies.Get<IConstructorSelectorPolicy>(context.BuildKey, out resolverPolicyDestination);
            var constructor = constructorSelector.SelectConstructor(context, resolverPolicyDestination);
            if (constructor != null)
            {
                ResolveConstructorParameters(constructor, constructorParameters);
            }

            // Properties resolvers
            var properties = new Dictionary<string, StepScopeDependency>();
            var propertySelector = context.Policies.Get<IPropertySelectorPolicy>(context.BuildKey, out resolverPolicyDestination);
            ResolveProperties(context, propertySelector, resolverPolicyDestination, properties);

            // Method parameters resolvers
            var methodParameters = new Dictionary<Tuple<string, string>, IList<StepScopeDependency>>();
            var methodSelector = context.Policies.Get<IMethodSelectorPolicy>(context.BuildKey, out resolverPolicyDestination);
            ResolveMethodParameters(context, methodSelector, resolverPolicyDestination, methodParameters);

            if (constructorParameters.Any() || properties.Any() || methodParameters.Any())
            {
                context.AddResolverOverrides(new StepScopeOverride(constructorParameters, properties, methodParameters));
            }
        }

        private static void ResolveMethodParameters(IBuilderContext context, IMethodSelectorPolicy methodSelector,
            IPolicyList resolverPolicyDestination, Dictionary<Tuple<string, string>, IList<StepScopeDependency>> methodParameters)
        {
            foreach (var method in methodSelector.SelectMethods(context, resolverPolicyDestination))
            {
                var parameterResolvers = method.GetParameterResolvers();
                for (var i = 0; i < parameterResolvers.Length; i++)
                {
                    var namedTypeResolver = parameterResolvers[i] as NamedTypeDependencyResolverPolicy;
                    if (namedTypeResolver != null)
                    {
                        var type = namedTypeResolver.Type;
                        var name = namedTypeResolver.Name;
                        if (StepScopeSynchronization.IsStepScope(type, name))
                        {
                            var parameter = method.Method.GetParameters()[i];
                            var key = new Tuple<string, string>(GetSignature(method.Method), parameter.Name);
                            AddMethodParameterDependency(methodParameters, key,
                                new StepScopeDependency(type, name));
                        }
                    }
                }
            }
        }

        private static void ResolveProperties(IBuilderContext context, IPropertySelectorPolicy propertySelector,
            IPolicyList resolverPolicyDestination, Dictionary<string, StepScopeDependency> properties)
        {
            foreach (var property in propertySelector.SelectProperties(context, resolverPolicyDestination))
            {
                var namedTypeResolver = property.Resolver as NamedTypeDependencyResolverPolicy;
                if (namedTypeResolver != null)
                {
                    var type = namedTypeResolver.Type;
                    var name = namedTypeResolver.Name;
                    if (StepScopeSynchronization.IsStepScope(type, name))
                    {
                        properties[property.Property.Name] = new StepScopeDependency(type, name);
                    }
                }
            }
        }

        private static void ResolveConstructorParameters(SelectedConstructor constructor, Dictionary<string, StepScopeDependency> constructorParameters)
        {
            var parameterResolvers = constructor.GetParameterResolvers();
            for (var i = 0; i < parameterResolvers.Length; i++)
            {
                var namedTypeResolver = parameterResolvers[i] as NamedTypeDependencyResolverPolicy;
                if (namedTypeResolver != null)
                {
                    var type = namedTypeResolver.Type;
                    var name = namedTypeResolver.Name;
                    if (StepScopeSynchronization.IsStepScope(type, name))
                    {
                        var parameter = constructor.Constructor.GetParameters()[i];
                        constructorParameters[parameter.Name] = new StepScopeDependency(type, name);
                    }
                }
            }
        }

        private static void AddMethodParameterDependency(
            Dictionary<Tuple<string, string>, IList<StepScopeDependency>> methodParameters,
            Tuple<string, string> key, StepScopeDependency dependency)
        {
            IList<StepScopeDependency> dependencies;
            if (!methodParameters.TryGetValue(key, out dependencies))
            {
                dependencies = new List<StepScopeDependency>();
                methodParameters[key] = dependencies;
            }
            dependencies.Add(dependency);
        }

        private static string GetSignature(MethodBase method)
        {
            var builder = new StringBuilder();
            builder.Append(method.Name).Append('(');
            var parameters = method.GetParameters();
            for (var i = 0; i < parameters.Length; i++)
            {
                if (i > 0)
                {
                    builder.Append(", ");
                }
                builder.Append(parameters[i].ParameterType.FullName).Append(' ').Append(parameters[i].Name);
            }
            builder.Append(')');
            return builder.ToString();
        }
    }
}