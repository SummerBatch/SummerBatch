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
using System.Reflection.Emit;
using Summer.Batch.Common.Util;

namespace Summer.Batch.Common.Proxy
{
    /// <summary>
    /// Factory for dynamic proxy.
    /// This factory can create a proxy for an interface. The created type will implement all the required interfaces
    /// as well as the <see cref="IProxyObject"/> interface. The method implementation redirect to the underlying
    /// instance. This instance can be changed and can be momentarilly be null, but it must be set when the proxy is
    /// manipulated.
    /// The created proxy supports, properties (including indexed properties), methods, and events.
    /// </summary>
    public static class ProxyFactory
    {
        #region constants

        private const string DynamicAssemblyName = "SummerBatchProxyAssembly";
        private const string DynamicModuleName = "MainModule";
        private const string DynamicTypePrefix = "Proxy_";

        private const string GetInstanceMethodName = "GetInstance";

        private const MethodAttributes InterfaceMethodAttributes = MethodAttributes.Public
                                                                   | MethodAttributes.HideBySig
                                                                   | MethodAttributes.Virtual
                                                                   | MethodAttributes.NewSlot;

        private const MethodAttributes PropertyMethodAttributes = MethodAttributes.Public
                                                                  | MethodAttributes.SpecialName
                                                                  | MethodAttributes.HideBySig
                                                                  | MethodAttributes.Virtual
                                                                  | MethodAttributes.NewSlot;

        #endregion

        private static readonly MethodInfo InstanceGetter = typeof(IProxyObject).GetMethod(GetInstanceMethodName, new Type[0]);

        private static readonly AssemblyName AssemblyName = new AssemblyName(DynamicAssemblyName);

        private static readonly AssemblyBuilder AssemblyBuilder
            = AppDomain.CurrentDomain.DefineDynamicAssembly(AssemblyName, AssemblyBuilderAccess.Run);

        private static readonly ModuleBuilder ModuleBuilder = AssemblyBuilder.DefineDynamicModule(DynamicModuleName);

        // cache for the created types
        private static readonly IDictionary<ProxyCacheKey, Type> ProxyTypeCache = new Dictionary<ProxyCacheKey, Type>();

        /// <summary>
        /// Creates a new proxy for the specified interface and optionally sets its underlying instance.
        /// </summary>
        /// <typeparam name="T">the interface to proxy</typeparam>
        /// <param name="superType">the super-type of the proxy object</param>
        /// <param name="instance">the underlying instance</param>
        /// <returns>a new proxy for the specified interface</returns>
        public static T Create<T>(Type superType = null, object instance = null)
        {
            return (T)Create(new List<Type> { typeof(T) }, superType, instance);
        }

        /// <summary>
        /// Creates a new proxy for the specified interface and optionally sets its underlying instance.
        /// </summary>
        /// <param name="interface">the interface to proxy</param>
        /// <param name="superType">the super-type of the proxy object</param>
        /// <param name="instance">the underlying instance</param>
        /// <returns>a new proxy for the specified interface</returns>
        public static object Create(Type @interface, Type superType = null, object instance = null)
        {
            return Create(new List<Type> { @interface }, superType, instance);
        }

        /// <summary>
        /// Creates a new proxy for the specified interface and optionally sets its underlying instance.
        /// </summary>
        /// <param name="interfaces">the interfaces to proxy</param>
        /// <param name="superType">the super-type of the proxy object</param>
        /// <param name="instance">the underlying instance</param>
        /// <returns>a new proxy for the specified interface</returns>
        public static object Create(IList<Type> interfaces, Type superType = null, object instance = null)
        {
            if (interfaces.Any(i => !i.IsInterface))
            {
                throw new ProxyException(string.Format("Cannot create a proxy for types [{0}]: only interfaces can be proxied.",
                                                       string.Join(", ", interfaces.Select(i => i.FullName))),
                                         interfaces);
            }

            var key = new ProxyCacheKey(interfaces);
            Type proxyType;
            if (!ProxyTypeCache.TryGetValue(key, out proxyType))
            {
                proxyType = BuildType(interfaces, superType ?? typeof(ProxyObject));
                ProxyTypeCache[key] = proxyType;
            }
            var proxy = Activator.CreateInstance(proxyType);
            if (proxy is ProxyObject)
            {
                ((IProxyObject)proxy).SetInstance(instance);
            }
            return proxy;
        }

        /// <summary>
        /// Builds a dynamic type for the specified interface.
        /// </summary>
        /// <param name="interfaces">the interfaces to proxy</param>
        /// <param name="superType">the super-type of the proxy object</param>
        /// <returns>a new type</returns>
        private static Type BuildType(IList<Type> interfaces, Type superType)
        {
            Assert.NotEmpty(interfaces, "At least one interface must be specified.");
            var name = DynamicTypePrefix + Guid.NewGuid().ToString("N");
            var typeBuilder = ModuleBuilder.DefineType(name, TypeAttributes.Public, superType, interfaces.ToArray());

            foreach (var currentInterface in GetAllInterfaces(interfaces))
            {
                foreach (var property in currentInterface.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    CreateProperty(typeBuilder, property);
                }
                foreach (var method in currentInterface.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                            .Where(m => !m.IsSpecialName))
                {
                    CreateMethod(typeBuilder, method, InterfaceMethodAttributes);
                }
                foreach (var @event in currentInterface.GetEvents(BindingFlags.Public | BindingFlags.Instance)
                            .Where(e => !e.IsSpecialName))
                {
                    CreateEvent(typeBuilder, @event);
                }
            }

            return typeBuilder.CreateType();
        }

        /// <summary>
        /// Creates a property and its get and set methods if required.
        /// </summary>
        /// <param name="typeBuilder">the builder for the proxy type</param>
        /// <param name="property">the original property</param>
        private static void CreateProperty(TypeBuilder typeBuilder, PropertyInfo property)
        {
            var indexParameterTypes = property.GetIndexParameters().Select(p => p.ParameterType).ToArray();
            var propertyBuilder = typeBuilder.DefineProperty(property.Name, PropertyAttributes.None,
                property.PropertyType, indexParameterTypes);
            if (property.GetMethod != null)
            {
                propertyBuilder.SetGetMethod(CreateMethod(typeBuilder, property.GetMethod, PropertyMethodAttributes));
            }
            if (property.SetMethod != null)
            {
                propertyBuilder.SetSetMethod(CreateMethod(typeBuilder, property.SetMethod, PropertyMethodAttributes));
            }
        }

        /// <summary>
        /// Creates an event and its methods if required.
        /// </summary>
        /// <param name="typeBuilder">the builder for the proxy type</param>
        /// <param name="event">the original event</param>
        private static void CreateEvent(TypeBuilder typeBuilder, EventInfo @event)
        {
            var eventBuilder = typeBuilder.DefineEvent(@event.Name, EventAttributes.None, @event.EventHandlerType);
            if (@event.AddMethod != null)
            {
                eventBuilder.SetAddOnMethod(CreateMethod(typeBuilder, @event.AddMethod, PropertyMethodAttributes));
            }
            if (@event.RemoveMethod != null)
            {
                eventBuilder.SetRemoveOnMethod(CreateMethod(typeBuilder, @event.RemoveMethod,
                    PropertyMethodAttributes));
            }
            if (@event.RaiseMethod != null)
            {
                eventBuilder.SetRaiseMethod(CreateMethod(typeBuilder, @event.RaiseMethod, PropertyMethodAttributes));
            }
        }

        /// <summary>
        /// Creates a method.
        /// </summary>
        /// <param name="typeBuilder">the builder for the proxy type</param>
        /// <param name="method">the original method</param>
        /// <param name="attributes">the attributes of the method</param>
        /// <returns>the corresponding method builder</returns>
        private static MethodBuilder CreateMethod(TypeBuilder typeBuilder, MethodInfo method, MethodAttributes attributes)
        {
            var name = method.DeclaringType.FullName + "." + method.Name;
            var parameters = method.GetParameters().Select(p => p.ParameterType).ToArray();
            var methodBuilder = typeBuilder.DefineMethod(name, attributes, method.ReturnType, parameters);

            foreach (var parameter in method.GetParameters())
            {
                methodBuilder.DefineParameter(parameter.Position + 1, parameter.Attributes, parameter.Name);
            }

            var il = methodBuilder.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0); // Load this
            il.Emit(OpCodes.Callvirt, InstanceGetter); // Get instance

            // Load parameters
            for (var i = 0; i < method.GetParameters().Length; i++)
            {
                il.Emit(OpCodes.Ldarg, i + 1);
            }

            il.Emit(OpCodes.Callvirt, method); // Call method
            il.Emit(OpCodes.Ret); // Return

            // We define it as an explicit implementation, this way we avoid any name collision
            typeBuilder.DefineMethodOverride(methodBuilder, method);

            return methodBuilder;
        }

        /// <summary>
        /// Computes all the interfaces extended by the specified interface.
        /// </summary>
        /// <param name="interfaces">the interfaces to proxy</param>
        /// <returns>an <see cref="IEnumerable{T}"/> of the extended interfaces</returns>
        private static IEnumerable<Type> GetAllInterfaces(IEnumerable<Type> interfaces)
        {
            var yeld = new HashSet<Type>();
            var stack = new Stack<Type>(interfaces);
            while (stack.Count != 0)
            {
                var currentInterface = stack.Pop();
                if (yeld.Contains(currentInterface))
                {
                    continue;
                }
                yeld.Add(currentInterface);
                yield return currentInterface;
                foreach (var superInterface in currentInterface.GetInterfaces())
                {
                    stack.Push(superInterface);
                }
            }
        }

        private struct ProxyCacheKey
        {
            private readonly Type[] _interfaces;

            public ProxyCacheKey(IEnumerable<Type> interfaces)
            {
                _interfaces = interfaces.OrderBy(i => i.FullName).ToArray();
            }

            public override bool Equals(object obj)
            {
                if (obj.GetType() != typeof(ProxyCacheKey))
                {
                    return false;
                }
                var key = (ProxyCacheKey)obj;
                if (key._interfaces.Length != _interfaces.Length)
                {
                    return false;
                }
                return !_interfaces.Where((t, i) => t != key._interfaces[i]).Any();
            }

            public override int GetHashCode()
            {
                return _interfaces.Aggregate(17, (current, @interface) => current * 486187739 + @interface.GetHashCode());
            }

            public override string ToString()
            {
                return "[" + string.Join(", ", _interfaces.Select(i => i.FullName)) + "]";
            }
        }
    }
}