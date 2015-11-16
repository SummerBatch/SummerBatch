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
using System.Configuration;
using System.Reflection;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Summer.Batch.Core.Configuration;
using Summer.Batch.Core.Configuration.Support;
using Summer.Batch.Core.Explore;
using Summer.Batch.Core.Explore.Support;
using Summer.Batch.Core.Launch;
using Summer.Batch.Core.Launch.Support;
using Summer.Batch.Core.Repository;
using Summer.Batch.Core.Repository.Dao;
using Summer.Batch.Core.Repository.Support;
using Summer.Batch.Core.Unity;
using Summer.Batch.Common.Util;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Summer.Batch.CoreTests.Core.Launch.Support
{
    [TestClass()]
    public class UnityTests
    {
        [TestMethod()]
        public void RunTestWithStandardConfig()
        {
            IJobOperator jobOperator = BatchRuntime.GetJobOperator();
            Assert.IsInstanceOfType(jobOperator, typeof(SimpleJobOperator));
            Type t = jobOperator.GetType();
            PropertyInfo f = t.GetProperty("JobLauncher", BindingFlags.Instance | BindingFlags.Public);
            IJobLauncher jobLauncher = (IJobLauncher)f.GetValue(jobOperator);
            Assert.IsNotNull(jobLauncher);
            Assert.IsInstanceOfType(jobLauncher, typeof(SimpleJobLauncher));
            PropertyInfo f2 = t.GetProperty("JobRepository", BindingFlags.Instance | BindingFlags.Public);
            IJobRepository jobRepository = (IJobRepository)f2.GetValue(jobOperator);
            Assert.IsNotNull(jobRepository);
            Assert.IsInstanceOfType(jobRepository, typeof(SimpleJobRepository));
            PropertyInfo f3 = t.GetProperty("JobRegistry", BindingFlags.Instance | BindingFlags.Public);
            IListableJobLocator jobRegistry = (IListableJobLocator)f3.GetValue(jobOperator);
            Assert.IsNotNull(jobRegistry);
            Assert.IsInstanceOfType(jobRegistry, typeof(MapJobRegistry));
            PropertyInfo f4 = t.GetProperty("JobExplorer", BindingFlags.Instance | BindingFlags.Public);
            IJobExplorer jobExplorer = (IJobExplorer)f4.GetValue(jobOperator);
            Assert.IsNotNull(jobExplorer);
            Assert.IsInstanceOfType(jobExplorer, typeof(SimpleJobExplorer));
            FieldInfo f5 = jobRepository.GetType().GetField("_jobInstanceDao", BindingFlags.Instance | BindingFlags.NonPublic);
            IJobInstanceDao dao = (IJobInstanceDao) f5.GetValue(jobRepository);
            Assert.IsNotNull(dao);
            Assert.IsInstanceOfType(dao, typeof(MapJobInstanceDao));
        }

        [TestMethod()]
        public void RunTestWithSuppliedDbConfig()
        {
            IJobOperator jobOperator = BatchRuntime.GetJobOperator(new MyDbUnityLoader());
            Assert.IsInstanceOfType(jobOperator, typeof(SimpleJobOperator));
            Type t = jobOperator.GetType();
            PropertyInfo f = t.GetProperty("JobLauncher", BindingFlags.Instance | BindingFlags.Public);
            IJobLauncher jobLauncher = (IJobLauncher)f.GetValue(jobOperator);
            Assert.IsNotNull(jobLauncher);
            Assert.IsInstanceOfType(jobLauncher, typeof(SimpleJobLauncher));
            PropertyInfo f2 = t.GetProperty("JobRepository", BindingFlags.Instance | BindingFlags.Public);
            IJobRepository jobRepository = (IJobRepository)f2.GetValue(jobOperator);
            Assert.IsNotNull(jobRepository);
            Assert.IsInstanceOfType(jobRepository, typeof(SimpleJobRepository));
            PropertyInfo f3 = t.GetProperty("JobRegistry", BindingFlags.Instance | BindingFlags.Public);
            IListableJobLocator jobRegistry = (IListableJobLocator)f3.GetValue(jobOperator);
            Assert.IsNotNull(jobRegistry);
            Assert.IsInstanceOfType(jobRegistry, typeof(MapJobRegistry));
            PropertyInfo f4 = t.GetProperty("JobExplorer", BindingFlags.Instance | BindingFlags.Public);
            IJobExplorer jobExplorer = (IJobExplorer)f4.GetValue(jobOperator);
            Assert.IsNotNull(jobExplorer);
            Assert.IsInstanceOfType(jobExplorer, typeof(SimpleJobExplorer));
            FieldInfo f5 = jobRepository.GetType().GetField("_jobInstanceDao", BindingFlags.Instance | BindingFlags.NonPublic);
            IJobInstanceDao dao = (IJobInstanceDao)f5.GetValue(jobRepository);
            Assert.IsNotNull(dao);
            Assert.IsInstanceOfType(dao, typeof(DbJobInstanceDao));
        }

        private class MyDbUnityLoader : UnityLoader
        {
            protected override void LoadConfiguration(IUnityContainer unityContainer)
            {
                unityContainer.RegisterInstance(ConfigurationManager.ConnectionStrings["connection"]);
                unityContainer.RegisterSingletonWithFactory<IJobRepository, DbJobRepositoryFactory>(new InjectionProperty("ConnectionStringSettings"));
                unityContainer.RegisterSingletonWithFactory<IJobExplorer, DbJobExplorerFactory>(new InjectionProperty("ConnectionStringSettings"));

                unityContainer.RegisterSingleton<IJobOperator, SimpleJobOperator>(new InjectionProperty("JobLauncher"),
                                                                            new InjectionProperty("JobRepository"),
                                                                            new InjectionProperty("JobExplorer"),
                                                                            new InjectionProperty("JobRegistry"));
                unityContainer.RegisterSingleton<IJobLauncher, SimpleJobLauncher>(new InjectionProperty("JobRepository"));
                unityContainer.RegisterSingleton<IListableJobLocator, MapJobRegistry>();
            }
        }
    }
}
