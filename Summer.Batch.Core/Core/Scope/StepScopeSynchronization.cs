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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Summer.Batch.Common.Proxy;
using Summer.Batch.Core.Scope.Context;

namespace Summer.Batch.Core.Scope
{
    /// <summary>
    /// Static class that keeps track of the dependencies in the step scope and synchronizes the proxies.
    /// </summary>
    public static class StepScopeSynchronization
    {
        private static readonly IDictionary<Tuple<Type, string>, IUnityContainer> Containers
            = new Dictionary<Tuple<Type, string>, IUnityContainer>();

        private static readonly IDictionary<Tuple<Type, string>, Type> MappedTypes
            = new Dictionary<Tuple<Type, string>, Type>();

        private static readonly IDictionary<IProxyObject, Tuple<Type, string>> Proxies
            = new Dictionary<IProxyObject, Tuple<Type, string>>();

        private static readonly ThreadLocal<IDictionary<IProxyObject, object>> Instances
            = new ThreadLocal<IDictionary<IProxyObject, object>>(() => new Dictionary<IProxyObject, object>());

        /// <summary>
        /// Checks if a dependency is in the step scope.
        /// </summary>
        /// <param name="type">the type of the dependency</param>
        /// <param name="name">the name of the dependency, or null if it has no name</param>
        /// <returns>true if the dependency is in the step scope; false otherwise</returns>
        public static bool IsStepScope(Type type, string name)
        {
            return Containers.ContainsKey(new Tuple<Type, string>(type, name));
        }

        /// <summary>
        /// Adds a new dependency in the step scope.
        /// </summary>
        /// <param name="registeredType">the type of the dependency</param>
        /// <param name="name">the name of the dependency, or null if it has no name</param>
        /// <param name="container">the container to use for resolving the dependency</param>
        /// <param name="mappedType">the actual type that will be resolved</param>
        public static void AddStepScopeDependency(Type registeredType, string name, IUnityContainer container, Type mappedType)
        {
            var key = new Tuple<Type, string>(registeredType, name);
            Containers[key] = container;
            MappedTypes[key] = mappedType;
        }

        /// <summary>
        /// Removes a dependency from the step scope.
        /// </summary>
        /// <param name="type">the type of the dependency</param>
        /// <param name="name">the name of the dependency, or null if it has no name</param>
        public static void RemoveScopeDependency(Type type, string name)
        {
            var key = new Tuple<Type, string>(type, name);
            Containers.Remove(key);
            MappedTypes.Remove(key);
        }

        /// <summary>
        /// Adds a new proxy for a dependency in the step scope.
        /// </summary>
        /// <param name="type">the type of the dependency</param>
        /// <param name="name">the name of the dependency, or null if it has no name</param>
        /// <param name="proxy">the new proxy</param>
        public static void AddProxy(Type type, string name, IProxyObject proxy)
        {
            Proxies[proxy] = new Tuple<Type, string>(type, name);
        }

        /// <summary>
        /// Resets the instances cache to force resolving new instances when a step changes.
        /// </summary>
        public static void ResetInstances()
        {
            Instances.Value.Clear();
        }

        /// <summary>
        /// Retrieves the instance of a proxy. If an instance has been cached it is returned,
        /// otherwise a new one is resolved using the Unity container.
        /// </summary>
        /// <param name="proxy">the proxy to get the instance for</param>
        /// <returns>the current instance of the proxy</returns>
        public static object GetInstance(IProxyObject proxy)
        {
            var instances = Instances.Value;
            object instance;
            if (!instances.TryGetValue(proxy, out instance))
            {
                Tuple<Type, string> key;
                if (Proxies.TryGetValue(proxy, out key))
                {
                    instance = Containers[key].Resolve(key.Item1, key.Item2);
                    instances[proxy] = instance;
                    // Registering the destruction callback for the instance
                    RegisterDestructionCallback(key, instance);
                }
            }
            return instance;
        }

        /// <summary>
        /// Get mapped type for registered type and given name.
        /// </summary>
        /// <param name="registeredType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Type GetMappedType(Type registeredType, string name)
        {
            var key = new Tuple<Type, string>(registeredType, name);
            Type mappedType;
            MappedTypes.TryGetValue(key, out mappedType);
            return mappedType;
        }

        private static void RegisterDestructionCallback(Tuple<Type, string> key, object instance)
        {
            var disposable = instance as IDisposable;
            if (disposable != null)
            {
                var name = key.Item1.FullName + ':' + key.Item2;
                StepSynchronizationManager.GetContext().RegisterDestructionCallback(name, new Task(() =>
                {
                    disposable.Dispose();
                }));
            }
        }
    }
}