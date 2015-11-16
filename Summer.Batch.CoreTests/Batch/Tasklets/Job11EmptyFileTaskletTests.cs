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
using System.IO;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Summer.Batch.Core;
using Summer.Batch.Core.Launch;
using Summer.Batch.Core.Launch.Support;
using Summer.Batch.Core.Step.Tasklet;
using Summer.Batch.Core.Unity;
using Summer.Batch.Core.Unity.Xml;
using Summer.Batch.Extra.EmptyCheckSupport;
using Summer.Batch.Common.IO;

namespace Summer.Batch.CoreTests.Batch.Tasklets
{
    [TestClass()]
    public class Job11EmptyFileTaskletTests
    {
        protected const string TestDataDirectoryIn = @".\TestData\Batch\";
        private static readonly string FileToCheck1 = Path.Combine(TestDataDirectoryIn, "Job11NotEmptyFile.txt");
        private static readonly string FileToCheck2 = Path.Combine(TestDataDirectoryIn, "Job11EmptyFile.txt");
        private static readonly string FileToCheck3 = Path.Combine(TestDataDirectoryIn, "Job11Absent.txt");

        [TestMethod()]
        public void RunJobWithEmptyFileTasklet()
        {
            XmlJob job = XmlJobParser.LoadJob("Job11.xml");
            IJobOperator jobOperator = BatchRuntime.GetJobOperator(new MyUnityLoaderJob11(), job);
            Assert.IsNotNull(jobOperator);
            long? executionId = jobOperator.StartNextInstance(job.Id);
            Assert.IsNotNull(executionId);

            JobExecution jobExecution = ((SimpleJobOperator)jobOperator).JobExplorer.GetJobExecution((long)executionId);
            Assert.IsFalse(jobExecution.Status.IsUnsuccessful());
            Assert.IsFalse(jobExecution.Status.IsRunning());
        }

        /// <summary>
        /// Extends UnityLoader and redefines LoadArtifacts() to supply the batch artifacts.
        /// </summary>
        private class MyUnityLoaderJob11 : UnityLoader
        {
            public override void LoadArtifacts(IUnityContainer unityContainer)
            {
                unityContainer.RegisterType<ITasklet, EmptyFileCheckTasklet>("check1", new InjectionProperty("FileToCheck", new FileSystemResource(FileToCheck1)));
                unityContainer.RegisterType<ITasklet, EmptyFileCheckTasklet>("check2", new InjectionProperty("FileToCheck", new FileSystemResource(FileToCheck2)));
                unityContainer.RegisterType<ITasklet, EmptyFileCheckTasklet>("check3", new InjectionProperty("FileToCheck", new FileSystemResource(FileToCheck3)));
            }
        }
    }
}
