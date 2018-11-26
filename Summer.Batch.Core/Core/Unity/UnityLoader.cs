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

using System.Configuration;
using Microsoft.Practices.Unity;
using Summer.Batch.Common.TaskExecution;
using Summer.Batch.Core.Configuration;
using Summer.Batch.Core.Configuration.Support;
using Summer.Batch.Core.Explore;
using Summer.Batch.Core.Explore.Support;
using Summer.Batch.Core.Launch;
using Summer.Batch.Core.Launch.Support;
using Summer.Batch.Core.Repository;
using Summer.Batch.Core.Repository.Dao;
using Summer.Batch.Core.Repository.Support;
using Summer.Batch.Core.Unity.Xml;

namespace Summer.Batch.Core.Unity
{
    /// <summary>
    /// Class that registers job artifacts in a unity container.
    /// It will load a default configuration for the main job architecture, then 
    /// parse a job xml file if supplied.
    /// 
    /// Users should extend this class and override:
    /// <list type="bullet">
    ///     <item>
    ///         <description>
    ///             <see cref="LoadConfiguration"/> to override the default job architecture
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             <see cref="LoadArtifacts"/> to supply batch artifacts (readers, writers, processors, tasklets...), 
    ///         </description>
    ///     </item>
    /// </list>
    /// </summary>
    public class UnityLoader
    {
        /// <summary>
        /// The XML job specification to load the artifacts for.
        /// </summary>
        public XmlJob Job { get; set; }

        /// <summary>
        /// Whether to support job restartability or not
        /// </summary>
        protected virtual bool PersistenceSupport { get { return false; } }
 
        /// <summary>
        /// Loads the XML job specification (if available) and registers all the required
        /// artifacts to the Unity container.
        /// </summary>
        /// <param name="unityContainer">The container to register to.</param>
        public void Load(IUnityContainer unityContainer)
        {
            LoadConfiguration(unityContainer);
            LoadArtifacts(unityContainer);
            if (Job != null)
            {
                new JobRegistrationHandler().LoadJob(unityContainer, Job);
            }

            RegisterJobsInRegistry(unityContainer);
        }

        /// <summary>
        /// Loads the main job configuration.
        /// </summary>
        /// <param name="unityContainer">The container to register to.</param>
        protected virtual void LoadConfiguration(IUnityContainer unityContainer)
        {
            if (PersistenceSupport)
            {
                var tablePrefix = ConfigurationManager.AppSettings[AbstractDbBatchMetadataDao.TablePrefixSetting];

                var datasourceName = ConfigurationManager.AppSettings["datasourceName"];

                if (string.IsNullOrEmpty(datasourceName))
                {
                    datasourceName = "Default";
                }

                var injectionMembers = new InjectionMember[tablePrefix == null ? 1 : 2];
                injectionMembers[0] = new InjectionProperty("ConnectionStringSettings",
                    ConfigurationManager.ConnectionStrings[datasourceName]);
                if (tablePrefix != null)
                {
                    injectionMembers[1] = new InjectionProperty("TablePrefix", tablePrefix);
                }

                unityContainer.RegisterSingletonWithFactory<IJobRepository, DbJobRepositoryFactory>(injectionMembers);
                unityContainer.RegisterSingletonWithFactory<IJobExplorer, DbJobExplorerFactory>(injectionMembers);
            }
            else
            {
                unityContainer.RegisterSingletonWithFactory<IJobRepository, MapJobRepositoryFactory>();
                unityContainer.RegisterSingletonWithFactory<IJobExplorer, MapJobExplorerFactory>();

            }
            unityContainer.RegisterSingleton<IJobOperator, SimpleJobOperator>(new InjectionProperty("JobLauncher"),
                                                                        new InjectionProperty("JobRepository"),
                                                                        new InjectionProperty("JobExplorer"),
                                                                        new InjectionProperty("JobRegistry"));
            unityContainer.RegisterSingleton<IJobLauncher, SimpleJobLauncher>(new InjectionProperty("JobRepository"));
            unityContainer.RegisterSingleton<IListableJobLocator, MapJobRegistry>();

            unityContainer.RegisterSingleton<IJobParametersIncrementer, RunIdIncrementer>();

            unityContainer.RegisterSingleton<ITaskExecutor, SimpleAsyncTaskExecutor>();
        }

        /// <summary>
        /// Loads the batch various artifacts (readers, writrs, processors, tasklets...)
        /// </summary>
        /// <param name="unityContainer">The container to register to.</param>
        public virtual void LoadArtifacts(IUnityContainer unityContainer)
        {
            // By default, do nothing
        }

        /// <summary>
        /// Registers the declared jobs in the registry.
        /// </summary>
        /// <param name="unityContainer">The container to register to.</param>
        private void RegisterJobsInRegistry(IUnityContainer unityContainer)
        {
            var locator = unityContainer.Resolve<IListableJobLocator>();
            var registry = locator as IJobRegistry;
            if (registry != null)
            {
                foreach (var job in unityContainer.ResolveAll<IJob>())
                {
                    registry.Register(new ReferenceJobFactory(job));
                }
            }
        }
    }
}