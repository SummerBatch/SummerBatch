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
using Microsoft.Practices.Unity;
using Summer.Batch.Core.Unity.Injection;
using Summer.Batch.Core.Unity.StepScope;

namespace Summer.Batch.Core.Unity
{
    /// <summary>
    /// Extension for easier use of <see cref="Registration{TFrom,TTo}"/>.
    /// </summary>
    public static class RegistrationExtension
    {
        /// <summary>
        /// Creates a new <see cref="Registration{TFrom,TTo}"/> for a singleton.
        /// </summary>
        /// <typeparam name="T">&nbsp;the type to register</typeparam>
        /// <param name="container">the container where to register</param>
        /// <param name="name">the name of the registration</param>
        /// <returns>an instance of <see cref="Registration{TFrom,TTo}"/></returns>
        public static Registration<T, T> SingletonRegistration<T>(this IUnityContainer container,
            string name = null)
        {
            return new Registration<T, T>(name, new ContainerControlledLifetimeManager(), container);
        }

        /// <summary>
        /// Creates a new <see cref="Registration{TFrom,TTo}"/> for a singleton.
        /// </summary>
        /// <typeparam name="TFrom">&nbsp;the requested type of the registration</typeparam>
        /// <typeparam name="TTo">&nbsp;the actual type of the registration</typeparam>
        /// <param name="container">the container where to register</param>
        /// <param name="name">the name of the registration</param>
        /// <returns>an instance of <see cref="Registration{TFrom,TTo}"/></returns>
        public static Registration<TFrom, TTo> SingletonRegistration<TFrom, TTo>(this IUnityContainer container,
            string name = null)
        {
            return new Registration<TFrom, TTo>(name, new ContainerControlledLifetimeManager(), container);
        }

        /// <summary>
        /// Creates a new <see cref="Registration{TFrom,TTo}"/> wit the step scope.
        /// </summary>
        /// <typeparam name="T">&nbsp;the type to register</typeparam>
        /// <param name="container">the container where to register</param>
        /// <param name="name">the name of the registration</param>
        /// <returns>an instance of <see cref="Registration{TFrom,TTo}"/></returns>
        public static Registration<T, T> StepScopeRegistration<T>(this IUnityContainer container,
            string name = null)
        {
            return new Registration<T, T>(name, new StepScopeLifetimeManager(), container);
        }

        /// <summary>
        /// Creates a new <see cref="Registration{TFrom,TTo}"/> wit the step scope.
        /// </summary>
        /// <typeparam name="TFrom">&nbsp;the requested type of the registration</typeparam>
        /// <typeparam name="TTo">&nbsp;the actual type of the registration</typeparam>
        /// <param name="container">the container where to register</param>
        /// <param name="name">the name of the registration</param>
        /// <returns>an instance of <see cref="Registration{TFrom,TTo}"/></returns>
        public static Registration<TFrom, TTo> StepScopeRegistration<TFrom, TTo>(this IUnityContainer container,
            string name = null)
        {
            return new Registration<TFrom, TTo>(name, new StepScopeLifetimeManager(), container);
        }
    }

    /// <summary>
    /// Registration
    /// </summary>
    /// <typeparam name="TFrom">&nbsp;</typeparam>
    /// <typeparam name="TTo">&nbsp;</typeparam>
    public class Registration<TFrom, TTo>
    {
        private readonly string _name;
        private readonly LifetimeManager _lifetimeManager;
        private readonly IUnityContainer _container;

        private readonly IList<InjectionMember> _injectionMembers = new List<InjectionMember>();

        private string _memberName;
        private MemberType _memberType = MemberType.None;
        private IList<object> _values;

        /// <summary>
        /// Creates a new <see cref="Registration{TFrom,TTo}"/> with the specified lifetime manager and container.
        /// </summary>
        /// <param name="lifetimeManager">the lifetime manager of the registration</param>
        /// <param name="container">the container where to register</param>
        public Registration(LifetimeManager lifetimeManager, IUnityContainer container)
        {
            _lifetimeManager = lifetimeManager;
            _container = container;
        }

        /// <summary>
        /// Creates a new <see cref="Registration{TFrom,TTo}"/> with the specified name, lifetime manager, and container.
        /// </summary>
        /// <param name="name">the name of the registration</param>
        /// <param name="lifetimeManager">the lifetime manager of the registration</param>
        /// <param name="container">the container where to register</param>
        public Registration(string name, LifetimeManager lifetimeManager, IUnityContainer container)
        {
            _name = name;
            _lifetimeManager = lifetimeManager;
            _container = container;
        }

        /// <summary>
        /// Performs the actual registration.
        /// </summary>
        /// <param name="addititionalInjectionMembers">additional injection members that will be added to those previously specified</param>
        public void Register(params InjectionMember[] addititionalInjectionMembers)
        {
            BuildInjectionMember();
            var injectionMembers = addititionalInjectionMembers == null
                ? _injectionMembers.ToArray()
                : _injectionMembers.Concat(addititionalInjectionMembers).ToArray();
            _container.RegisterType(typeof(TFrom), typeof(TTo), _name, _lifetimeManager, injectionMembers);
        }

        /// <summary>
        /// Specifies a new constructor injection.
        /// </summary>
        /// <returns>the current instance</returns>
        public Registration<TFrom, TTo> Constructor()
        {
            BuildInjectionMember();
            _memberType = MemberType.Constructor;
            _values = new List<object>();
            return this;
        }

        /// <summary>
        /// Specifies a new method injection.
        /// </summary>
        /// <param name="name">the name of the injected method</param>
        /// <returns>the current instance</returns>
        public Registration<TFrom, TTo> Method(string name)
        {
            BuildInjectionMember();
            _memberName = name;
            _memberType = MemberType.Method;
            _values = new List<object>();
            return this;
        }

        /// <summary>
        /// Specifies a new property injection.
        /// </summary>
        /// <param name="name">the name of the property</param>
        /// <returns>the current instance</returns>
        public Registration<TFrom, TTo> Property(string name)
        {
            BuildInjectionMember();
            _memberName = name;
            _memberType = MemberType.Property;
            _values = new List<object>();
            return this;
        }

        /// <summary>
        /// Specifies an instance as value for the previous injection.
        /// </summary>
        /// <param name="value">the instance to inject</param>
        /// <returns>the current instance</returns>
        public Registration<TFrom, TTo> Instance(object value)
        {
            _values.Add(value);
            return this;
        }

        /// <summary>
        /// Specifies a value for the previous injection.
        /// </summary>
        /// <param name="value">the instance to inject</param>
        /// <returns>the current instance</returns>
        public Registration<TFrom, TTo> Value(object value)
        {
            _values.Add(value);
            return this;
        }

        /// <summary>
        /// Specifies a reference for the previous injection.
        /// </summary>
        /// <typeparam name="T">&nbsp;the requested type for the reference</typeparam>
        /// <param name="referenceName">the name of the reference</param>
        /// <returns>the current instance</returns>
        public Registration<TFrom, TTo> Reference<T>(string referenceName = null)
        {
            _values.Add(new ResolvedParameter<T>(referenceName));
            return this;
        }

        /// <summary>
        /// Specifies a list of types to resolve and inject as an array.
        /// </summary>
        /// <typeparam name="T">&nbsp;the type of the array to inject</typeparam>
        /// <param name="types">the types to resolve to create the array</param>
        /// <returns>the current instance</returns>
        public Registration<TFrom, TTo> References<T>(params Type[] types)
        {
            _values.Add(new ResolvedArrayParameter<T>(types.Select(t => new ResolvedParameter(t)).Cast<object>().ToArray()));
            return this;
        }

        /// <summary>
        /// Specifies a list of names to resolve and inject as an array.
        /// </summary>
        /// <typeparam name="T">&nbsp;the type of the array to inject</typeparam>
        /// <param name="names">the names to resolve to create the array</param>
        /// <returns>the current instance</returns>
        public Registration<TFrom, TTo> References<T>(params string[] names)
        {
            _values.Add(new ResolvedArrayParameter<T>(names.Select(n => new ResolvedParameter<T>(n)).Cast<object>().ToArray()));
            return this;
        }

        /// <summary>
        /// Specifies a list of types and names to resolve and inject as an array.
        /// </summary>
        /// <typeparam name="T">&nbsp;the type of the array to inject</typeparam>
        /// <param name="references">the types and their corresponding name to resolve to create the array</param>
        /// <returns>the current instance</returns>
        public Registration<TFrom, TTo> References<T>(params Reference[] references)
        {
            _values.Add(
                new ResolvedArrayParameter<T>(
                    references.Select(r => new ResolvedParameter(r.Type, r.Name)).Cast<object>().ToArray()));
            return this;
        }

        /// <summary>
        /// Specifies a late binding expression to be resolved at injection.
        /// </summary>
        /// <typeparam name="T">&nbsp;the type returned by the late binding expression</typeparam>
        /// <param name="lateBinding">the late binding expression</param>
        /// <returns>the current instance</returns>
        public Registration<TFrom, TTo> LateBinding<T>(string lateBinding)
        {
            _values.Add(new LateBindingInjectionValue<T>(lateBinding));
            return this;
        }

        /// <summary>
        /// Specifies a single resource for the previous injection.
        /// </summary>
        /// <param name="path">the path of the resource (can use the <see cref="LateBindingConfigurationParser"/> syntax)</param>
        /// <returns>the current instance</returns>
        public Registration<TFrom, TTo> Resource(string path)
        {
            _values.Add(new ResourceInjectionValue(new LateBindingInjectionValue<string>(path)));
            return this;
        }

        /// <summary>
        /// Specifies a list of resources for the previous injection.
        /// </summary>
        /// <param name="path">the paths of the resources (can use the <see cref="LateBindingConfigurationParser"/> syntax)</param>
        /// <returns>the current instance</returns>
        public Registration<TFrom, TTo> Resources(string path)
        {
            _values.Add(new ResourceInjectionValue(new LateBindingInjectionValue<string>(path), true));
            return this;
        }

        // type of injected members
        private enum MemberType
        {
            None, Constructor, Method, Property
        }

        // builds the current injection member
        private void BuildInjectionMember()
        {
            switch (_memberType)
            {
                case MemberType.Constructor:
                    _injectionMembers.Add(new InjectionConstructor(_values.ToArray()));
                    break;
                case MemberType.Method:
                    _injectionMembers.Add(new InjectionMethod(_memberName, _values.ToArray()));
                    break;
                case MemberType.Property:
                    _injectionMembers.Add(new InjectionProperty(_memberName, _values[0]));
                    break;              
            }
            _memberName = null;
            _memberType = MemberType.None;
        }
    }

    /// <summary>
    /// Structure that represent a reference.
    /// </summary>
    public struct Reference
    {
        private readonly Type _type;
        private readonly string _name;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="type">the type of the reference</param>
        /// <param name="name">the name of the reference (optional)</param>
        public Reference(Type type, string name = null)
        {
            _type = type;
            _name = name;
        }

        /// <summary>
        /// The type of the reference.
        /// </summary>
        public Type Type
        {
            get { return _type; }
        }

        /// <summary>
        /// The name of the reference.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }
    }
}