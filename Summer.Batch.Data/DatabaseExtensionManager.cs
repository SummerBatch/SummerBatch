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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NLog;
using Summer.Batch.Data.Incrementer;
using Summer.Batch.Data.Parameter;

namespace Summer.Batch.Data
{
    /// <summary>
    /// Static class to manage support to different databases.
    /// </summary>
    public static class DatabaseExtensionManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly IDictionary<string, IDatabaseExtension> Extensions =
            new ConcurrentDictionary<string, IDatabaseExtension>();

        /// <summary>
        /// Static constructor that uses reflection to find all implementation of
        /// <see cref="IDatabaseExtension"/> and register them.
        /// </summary>
        static DatabaseExtensionManager()
        {
            var extensionType = typeof(IDatabaseExtension);
            var extensions = new List<Type>();
            // Retrieve all types implementing IDatabaseExtension
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    extensions.AddRange(
                        assembly.GetTypes()
                            .Where(type => type.IsClass && !type.IsAbstract && type.GetInterfaces().Contains(extensionType)));
                }
                catch (ReflectionTypeLoadException e)
                {
                    Logger.Debug("An exception was thrown while trying to load assembly '{0}': {1}", assembly.FullName, e.LoaderExceptions[0].Message);
                }
            }
            // Instantiate each extension and register it
            foreach (var extension in extensions)
            {
                try
                {
                    var instance = Activator.CreateInstance(extension) as IDatabaseExtension;
                    if (instance != null)
                    {
                        Register(instance);
                    }
                }
                catch (Exception e)
                {
                    Logger.Warn(e,
                        "An exception was thrown while trying to create an instance of {0}. Please make sure that it has a parameter-less constructor.",
                        extension.FullName);
                }
            }
        }

        /// <summary>
        /// Retrieves the instance of <see cref="IPlaceholderGetter"/> for a provider.
        /// </summary>
        /// <param name="providerName">A provider name.</param>
        /// <returns>
        /// The placeholder getter for the provider or null if no extension has
        /// been registered for this provider name.
        /// </returns>
        public static IPlaceholderGetter GetPlaceholderGetter(string providerName)
        {
            IDatabaseExtension extension;
            return Extensions.TryGetValue(providerName, out extension)
                ? extension.PlaceholderGetter
                : null;
        }

        /// <summary>
        /// Retrieves the instance of <see cref="IDataFieldMaxValueIncrementer"/> for a provider.
        /// </summary>
        /// <param name="providerName">A provider name.</param>
        /// <returns>
        /// The incrementer for the provider or null if no extension has been registered for this provider name
        /// </returns>
        public static IDataFieldMaxValueIncrementer GetIncrementer(string providerName)
        {
            IDatabaseExtension extension;
            return Extensions.TryGetValue(providerName, out extension)
                ? extension.Incrementer
                : null;
        }

        /// <summary>
        /// Registers a new extension.
        /// </summary>
        /// <param name="extension">The extension to register.</param>
        private static void Register(IDatabaseExtension extension)
        {
            foreach (var providerName in extension.ProviderNames)
            {
                Extensions[providerName] = extension;
            }
        }
    }
}