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

using System.Collections.Generic;
using Summer.Batch.Data.Incrementer;
using Summer.Batch.Data.Parameter;

namespace Summer.Batch.Data
{
    /// <summary>
    /// Interface to add support for a database.
    /// Implementations should have a parameter-less constructor so they
    /// can be automatically registered by <see cref="DatabaseExtensionManager"/>.
    /// </summary>
    public interface IDatabaseExtension
    {
        /// <summary>
        /// The provider names supported by this extension.
        /// </summary>
        IEnumerable<string> ProviderNames { get; }

        /// <summary>
        /// The instance of <see cref="IPlaceholderGetter"/> that should be used
        /// by for queries using one of the supported provider names.
        /// </summary>
        IPlaceholderGetter PlaceholderGetter { get; }

        /// <summary>
        /// An instance of <see cref="IDataFieldMaxValueIncrementer"/> that should be used
        /// by the persisted job repository to compute unique ids. Each call should return
        /// a new instance.
        /// </summary>
        IDataFieldMaxValueIncrementer Incrementer { get; }
    }
}