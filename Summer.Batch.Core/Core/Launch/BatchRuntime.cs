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

//   This file has been modified.
//   Original copyright notice :

/*
 * Copyright 2006-2013 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Microsoft.Practices.Unity;
using NLog;
using Summer.Batch.Core.Unity;
using Summer.Batch.Core.Unity.Singleton;
using Summer.Batch.Core.Unity.StepScope;
using Summer.Batch.Core.Unity.Xml;

namespace Summer.Batch.Core.Launch
{
    /// <summary>
    /// Factory that builds instances of <see cref="IJobOperator"/>.
    /// </summary>
    public static class BatchRuntime
    {
        private readonly static Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Creates an <see cref="IJobOperator"/> with the default <see cref="UnityLoader"/>.
        /// </summary>
        /// <returns>An instance of <see cref="IJobOperator"/>.</returns>
        public static IJobOperator GetJobOperator()
        {
            return GetJobOperator(new UnityLoader());
        }

        /// <summary>
        /// Creates an <see cref="IJobOperator"/> with a <see cref="UnityLoader"/> build with the given job specification.
        /// </summary>
        /// <param name="job">The job specification used to build the <see cref="UnityLoader"/>.</param>
        /// <returns>An instance of <see cref="IJobOperator"/>.</returns>
        public static IJobOperator GetJobOperator(XmlJob job)
        {
            return GetJobOperator(new UnityLoader { Job = job });
        }

        /// <summary>
        /// Creates an <see cref="IJobOperator"/> with the specified <see cref="UnityLoader"/> and the given job specification.
        /// </summary>
        /// <param name="loader"></param>
        /// <param name="job">The job specification used to build the <see cref="UnityLoader"/>.</param>
        /// <returns>An instance of <see cref="IJobOperator"/>.</returns>
        public static IJobOperator GetJobOperator(UnityLoader loader, XmlJob job)
        {
            loader.Job = job;
            return GetJobOperator(loader);
        }

        /// <summary>
        /// Creates an <see cref="IJobOperator"/> with the specified <see cref="UnityLoader"/>.
        /// </summary>
        /// <param name="loader">The <see cref="UnityLoader"/> to use.</param>
        /// <returns>An instance of <see cref="IJobOperator"/>.</returns>
        public static IJobOperator GetJobOperator(UnityLoader loader)
        {
            IUnityContainer unityContainer = new UnityContainer();
            unityContainer.AddNewExtension<PostprocessingUnityExtension>();
            unityContainer.AddNewExtension<StepScopeExtension>();
            unityContainer.AddNewExtension<SingletonExtension>();
            loader.Load(unityContainer);

            IJobOperator jobOperator = unityContainer.Resolve<IJobOperator>();
            if (jobOperator != null)
            {
                Logger.Debug("Loaded BatchContainerServiceProvider with className = {0}", jobOperator.GetType());
            }
            return jobOperator;
        }

    }
}
