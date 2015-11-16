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
using System.Configuration;
using System.IO;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Summer.Batch.Core;
using Summer.Batch.Core.Launch;
using Summer.Batch.Core.Launch.Support;
using Summer.Batch.Core.Step.Tasklet;
using Summer.Batch.Core.Unity;
using Summer.Batch.Core.Unity.Xml;
using Summer.Batch.Extra.SqlScriptSupport;
using Summer.Batch.Common.IO;


namespace Summer.Batch.CoreTests.Batch.Tasklets
{
    [TestClass()]
    public class SqlScriptTaskletTests
    {
        protected const string TestDataDirectoryIn = @"TestData\SqlScript\";
        private static readonly string TestPathIn = Path.Combine(TestDataDirectoryIn, "myscript.sql");

        private const string ConnectionString 
            = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=|DataDirectory|\NewTestDB.mdf;Integrated Security=True;Connect Timeout=30";
        private const string ProviderName = "System.Data.SqlClient";

        private static readonly ConnectionStringSettings ConnectionStringSettings = new ConnectionStringSettings
        {
            ConnectionString = ConnectionString,
            ProviderName = ProviderName
        };

        [TestMethod()]
        public void RunJobWithTasklet()
        {
            XmlJob job = XmlJobParser.LoadJob("JobSqlScript.xml");
            IJobOperator jobOperator = BatchRuntime.GetJobOperator(new MyUnityLoaderJob1(), job);
            Assert.IsNotNull(jobOperator);
            Assert.AreEqual(1, jobOperator.StartNextInstance(job.Id));
            JobExecution jobExecution = ((SimpleJobOperator)jobOperator).JobExplorer.GetJobExecution(1);
            Assert.IsFalse(jobExecution.Status.IsUnsuccessful());
        }

        /// <summary>
        /// Extends UnityLoader and redefines LoadArtifacts() to supply the batch artifacts.
        /// </summary>
        private class MyUnityLoaderJob1 : UnityLoader
        {
            public override void LoadArtifacts(IUnityContainer unityContainer)
            {
                //inject database connection and sql script location
                unityContainer.RegisterType<ITasklet, SqlScriptRunnerTasklet>("tasklet1",
                    new InjectionProperty("ConnectionStringSettings",ConnectionStringSettings),
                    new InjectionProperty("Resource", new FileSystemResource(TestPathIn)));
            }
        }

        
    }
}
