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
using System.Linq;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;

namespace Summer.Batch.Core.Unity.Injection
{
    /// <summary>
    /// Implementation of <see cref="InjectionParameterValue"/> for late binding expressions.
    /// </summary>
    /// <typeparam name="T">&nbsp;the expected type</typeparam>
    public class LateBindingInjectionValue<T> : TypedInjectionValue
    {
        private readonly LateBindingNode _node;

        /// <summary>
        /// Constructs a new <see cref="LateBindingInjectionValue{T}"/>
        /// </summary>
        /// <param name="configuration">the late binding configuration</param>
        public LateBindingInjectionValue(string configuration) : base(typeof(T))
        {
            _node = new LateBindingConfigurationParser(configuration).Parse();
        }

        /// <summary>
        /// Returns the resolver policy for the given type to build.
        /// </summary>
        /// <param name="typeToBuild"></param>
        /// <returns></returns>
        public override IDependencyResolverPolicy GetResolverPolicy(Type typeToBuild)
        {
            return DoGetResolverPolicy((dynamic)_node);
        }

        #region DoGetResolverPolicy overloads

        private IDependencyResolverPolicy DoGetResolverPolicy(LateBindingNode node)
        {
            throw new InvalidOperationException(string.Format("Invalid expression at {0}.", node.Position.Start));
        }

        private IDependencyResolverPolicy DoGetResolverPolicy(IndexAccessorNode node)
        {
            var identifier = node.Children[0] as IdentifierNode;
            var index = node.Children[1] as StringNode;
            if (identifier == null || index == null)
            {
                throw new InvalidOperationException("Index accessor node should have an identifier and an index.");
            }
            if (identifier.Identifier == "stepExecutionContext")
            {
                return new StepContextDependencyResolverPolicy<T>(index.Literal);
            }
            if (identifier.Identifier == "jobExecutionContext")
            {
                return new JobContextDependencyResolverPolicy<T>(index.Literal);
            }
            if (identifier.Identifier == "settings")
            {
                return new SettingsDependencyResolverPolicy<T>(index.Literal);
            }
            throw new InvalidOperationException(string.Format("Unsupported identifier at {0}: {1}.", node.Position.Start,
                identifier.Identifier));
        }

        private IDependencyResolverPolicy DoGetResolverPolicy(StringNode node)
        {
            return new LiteralValueDependencyResolverPolicy<T>(node.Literal);
        }

        private IDependencyResolverPolicy DoGetResolverPolicy(ConcatenationNode node)
        {
            var children = node.Children.Select<LateBindingNode, dynamic>(child =>
            {
                var stringNode = child as StringNode;
                if (stringNode != null)
                {
                    return stringNode.Literal;
                }
                return DoGetResolverPolicy((IndexAccessorNode) child);
            }).ToArray();
            return new StringConcatenationResolverPolicy(children);
        }

        #endregion
    }
}