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
using Summer.Batch.Extra.Service.Stop;

namespace Summer.Batch.Extra.Service
{
    /// <summary>
    /// Super class for services that need access to job and step context via context managers.
    /// </summary>
    public class AbstractService
    {
        /// <summary>
        /// Injected job context manager property.
        /// </summary>
        [Dependency(BatchConstants.JobContextManagerName)]
        public IContextManager JobContextManager { protected get; set; }

        /// <summary>
        /// Injected step context manager property.
        /// </summary>
        [Dependency(BatchConstants.StepContextManagerName)]
        public IContextManager StepContextManager { protected get; set; }

        /// <summary>
        /// Injectd ServiceStop.
        /// </summary>
        [Dependency]
        public ServiceStop ServiceStop { protected get; set; }
    }
}