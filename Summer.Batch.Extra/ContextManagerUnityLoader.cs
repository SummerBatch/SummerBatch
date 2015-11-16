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
using Microsoft.Practices.Unity;
using Summer.Batch.Common.IO;
using Summer.Batch.Core.Unity;
using Summer.Batch.Common.Settings;
using Summer.Batch.Common.TaskExecution;
using Summer.Batch.Extra.IO;

namespace Summer.Batch.Extra
{
    /// <summary>
    /// Unity loader registering context managers.
    /// </summary>
    public class ContextManagerUnityLoader : UnityLoader
    {
        /// <summary>
        /// Add specific configuration for context managers.
        /// </summary>
        /// <param name="container">the container for the registrations</param>
        protected override void LoadConfiguration(IUnityContainer container)
        {
            base.LoadConfiguration(container);

            container.RegisterSingleton<IContextManager, ContextManager>(BatchConstants.JobContextManagerName);
            
            container.RegisterStepScope<IContextManager, ContextManager>(BatchConstants.StepContextManagerName);

            container.RegisterSingleton<ResourceLoader, GdgResourceLoader>();
        }
    }
}
