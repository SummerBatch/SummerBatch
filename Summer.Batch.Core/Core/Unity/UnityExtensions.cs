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
using Microsoft.Practices.Unity;
using Summer.Batch.Core.Unity.StepScope;
using Summer.Batch.Common.Util;

namespace Summer.Batch.Core.Unity
{
    /// <summary>
    /// Extensions methods for Unity.
    /// </summary>
    public static class UnityExtensions
    {
        #region Singleton extensions

        /// <summary>
        /// Extension method automating the new ContainerControlledLifetimeManager() argument;
        /// Register a singleton, no name.
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTo"></typeparam>
        /// <param name="unityContainer"></param>
        /// <param name="injectionMembers"></param>
        /// <returns></returns>
        public static IUnityContainer RegisterSingleton<TFrom, TTo>(this IUnityContainer unityContainer,
            params InjectionMember[] injectionMembers) where TTo : TFrom
        {
            return unityContainer.RegisterType<TFrom, TTo>(new ContainerControlledLifetimeManager(), injectionMembers);
        }

        /// <summary>
        /// Extension method automating the new ContainerControlledLifetimeManager() argument.
        /// Register a named singleton.
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTo"></typeparam>
        /// <param name="unityContainer"></param>
        /// <param name="name"></param>
        /// <param name="injectionMembers"></param>
        /// <returns></returns>
        public static IUnityContainer RegisterSingleton<TFrom, TTo>(this IUnityContainer unityContainer, string name,
            params InjectionMember[] injectionMembers) where TTo : TFrom
        {
            return unityContainer.RegisterType<TFrom, TTo>(name, new ContainerControlledLifetimeManager(),
                injectionMembers);
        }

        /// <summary>
        /// Extension method automating the new ContainerControlledLifetimeManager() argument.
        /// Register a singleton, no name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="unityContainer"></param>
        /// <param name="injectionMembers"></param>
        /// <returns></returns>
        public static IUnityContainer RegisterSingleton<T>(this IUnityContainer unityContainer,
            params InjectionMember[] injectionMembers)
        {
            return unityContainer.RegisterType<T>(new ContainerControlledLifetimeManager(), injectionMembers);
        }

        /// <summary>
        /// Extension method automating the new ContainerControlledLifetimeManager() argument.
        /// Register a named singleton.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="unityContainer"></param>
        /// <param name="name"></param>
        /// <param name="injectionMembers"></param>
        /// <returns></returns>
        public static IUnityContainer RegisterSingleton<T>(this IUnityContainer unityContainer, string name,
            params InjectionMember[] injectionMembers)
        {
            return unityContainer.RegisterType<T>(name, new ContainerControlledLifetimeManager(), injectionMembers);
        }

        #endregion

        #region Factory extensions

        /// <summary>
        /// Extension method automating the registering of a factory and the use of this factory as
        /// an InjectionFactory for retrieving instances. Final object is not singleton: each time it is resolved, 
        /// Factory.GetObject() will be called.
        /// </summary>
        public static IUnityContainer RegisterWithFactory<TFrom, TTo>(this IUnityContainer unityContainer,
            params InjectionMember[] injectionMembers) where TTo : IFactory<TFrom>
        {
            return unityContainer.RegisterType<TTo>(new ContainerControlledLifetimeManager(), injectionMembers)
                .RegisterType<TFrom>(new InjectionFactory(c => c.Resolve<TTo>().GetObject()));
        }

        /// <summary>
        /// Extension method automating the registering of a factory and the use of this factory as
        /// an InjectionFactory for retrieving instances. Final object is singleton: Factory.GetObject() will 
        /// be called only once.
        /// </summary>
        public static IUnityContainer RegisterSingletonWithFactory<TFrom, TTo>(this IUnityContainer unityContainer,
            params InjectionMember[] injectionMembers) where TTo : IFactory<TFrom>
        {
            return unityContainer.RegisterType<TTo>(new ContainerControlledLifetimeManager(), injectionMembers)
                .RegisterType<TFrom>(new ContainerControlledLifetimeManager(),
                    new InjectionFactory(c => c.Resolve<TTo>().GetObject()));
        }

        #endregion

        #region Step scope extensions

        /// <summary>
        /// Registers a new depency in the step scope.
        /// </summary>
        /// <typeparam name="TFrom">the type that will be requested</typeparam>
        /// <typeparam name="TTo">the actual type that will be returned</typeparam>
        /// <param name="unityContainer">the container to configure</param>
        /// <param name="injectionMembers">the injection configuration objects</param>
        /// <returns>the configured container (<paramref name="unityContainer"/>)</returns>
        public static IUnityContainer RegisterStepScope<TFrom, TTo>(this IUnityContainer unityContainer,
            params InjectionMember[] injectionMembers) where TTo : TFrom
        {
            return unityContainer.RegisterType<TFrom, TTo>(injectionMembers);
        }

        /// <summary>
        /// Registers a new depency in the step scope.
        /// </summary>
        /// <typeparam name="TFrom">the type that will be requested</typeparam>
        /// <typeparam name="TTo">the actual type that will be returned</typeparam>
        /// <param name="unityContainer">the container to configure</param>
        /// <param name="name">the name to use for registration</param>
        /// <param name="injectionMembers">the injection configuration objects</param>
        /// <returns>the configured container (<paramref name="unityContainer"/>)</returns>
        public static IUnityContainer RegisterStepScope<TFrom, TTo>(this IUnityContainer unityContainer, string name,
            params InjectionMember[] injectionMembers) where TTo : TFrom
        {
            return unityContainer.RegisterType<TFrom, TTo>(name, new StepScopeLifetimeManager(), injectionMembers);
        }

        /// <summary>
        /// Registers a new depency in the step scope.
        /// </summary>
        /// <typeparam name="T">the actual type of the dependency</typeparam>
        /// <param name="unityContainer">the container to configure</param>
        /// <param name="injectionMembers">the injection configuration objects</param>
        /// <returns>the configured container (<paramref name="unityContainer"/>)</returns>
        public static IUnityContainer RegisterStepScope<T>(this IUnityContainer unityContainer,
            params InjectionMember[] injectionMembers)
        {
            return unityContainer.RegisterType<T>(new StepScopeLifetimeManager(), injectionMembers);
        }

        /// <summary>
        /// Registers a new depency in the step scope.
        /// </summary>
        /// <typeparam name="T">the actual type of the dependency</typeparam>
        /// <param name="unityContainer">the container to configure</param>
        /// <param name="name">the name to use for registration</param>
        /// <param name="injectionMembers">the injection configuration objects</param>
        /// <returns>the configured container (<paramref name="unityContainer"/>)</returns>
        public static IUnityContainer RegisterStepScope<T>(this IUnityContainer unityContainer, string name,
            params InjectionMember[] injectionMembers)
        {
            return unityContainer.RegisterType<T>(name, new StepScopeLifetimeManager(), injectionMembers);
        }

        #endregion
    }
}