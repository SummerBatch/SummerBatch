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
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Summer.Batch.Core;
using Summer.Batch.Core.Launch;
using Summer.Batch.Core.Launch.Support;
using Summer.Batch.Core.Unity;
using Summer.Batch.Core.Unity.Xml;

namespace Summer.Batch.CoreTests.Batch.Listeners
{
    public abstract class AbstractListenersLaunchTests
    {
        protected const string TestDataDirectoryIn = @".\TestData\Batch\";
        protected const string TestDataDirectoryOut = @"C:\temp\out\";

        protected abstract string[] GetFileNamesIn();
        protected abstract string[] GetFileNamesOut();

        public void RunJob(string xmlFile, string jobName, UnityLoader loader, bool shouldFail)
        {
            // Flush output file 
            GetFileNamesOut().ForEach(s => { if (File.Exists(s)) { File.Delete(s); } });

            // Prerequisites
            GetFileNamesIn().ForEach(s => Assert.IsTrue(new FileInfo(s).Exists, "Job input file " + s + " does not exist, job can't be run"));
            GetFileNamesOut().ForEach(s => Assert.IsFalse(new FileInfo(s).Exists, "Job output file " + s + " should have been deleted before test"));

            XmlJob job = XmlJobParser.LoadJob(xmlFile);
            IJobOperator jobOperator = BatchRuntime.GetJobOperator(loader, job);
            Assert.IsNotNull(jobOperator);
            long? executionId = jobOperator.StartNextInstance(jobName);
            Assert.IsNotNull(executionId);

            JobExecution jobExecution = ((SimpleJobOperator)jobOperator).JobExplorer.GetJobExecution((long)executionId);
            //job SHOULD BE FAILED because of rollback having occured
            if (shouldFail)
            {
                Assert.IsTrue(jobExecution.Status.IsUnsuccessful());
            }
            else
            {
                Assert.IsFalse(jobExecution.Status.IsUnsuccessful());
            }
            Assert.IsFalse(jobExecution.Status.IsRunning());
        }


        public void RunJob(string xmlFile, string jobName, UnityLoader loader)
        {
            RunJob(xmlFile,jobName,loader, false);
        }
    }
}