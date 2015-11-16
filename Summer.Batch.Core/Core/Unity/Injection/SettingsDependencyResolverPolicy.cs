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
using Summer.Batch.Common.Settings;
using Summer.Batch.Common.Util;

namespace Summer.Batch.Core.Unity.Injection
{
    /// <summary>
    /// Dependency resolver that reads a property from the settings.
    /// </summary>
    public class SettingsDependencyResolverPolicy<T> : IDependencyResolverPolicy
    {
        private readonly string _propertyName;

        /// <summary>
        /// Constructs a new <see cref="SettingsDependencyResolverPolicy{T}"/>.
        /// </summary>
        /// <param name="propertyName">the name of the property to read</param>
        public SettingsDependencyResolverPolicy(string propertyName)
        {
            _propertyName = propertyName;
        }

        /// <summary>
        /// Resolve object from context.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public object Resolve(IBuilderContext context)
        {
            var settingsManager = context.NewBuildUp<SettingsManager>();
            return StringConverter.Convert<T>(settingsManager[_propertyName]);
        }
    }
}