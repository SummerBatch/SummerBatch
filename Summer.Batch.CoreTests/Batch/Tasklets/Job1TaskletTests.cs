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
using System.IO;
using System.Threading;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Summer.Batch.Core;
using Summer.Batch.Core.Launch;
using Summer.Batch.Core.Scope.Context;
using Summer.Batch.Core.Step.Tasklet;
using Summer.Batch.Core.Unity;
using Summer.Batch.Core.Unity.Xml;
using Summer.Batch.Infrastructure.Repeat;

namespace Summer.Batch.CoreTests.Batch.Tasklets
{
    [TestClass()]
    public class Job1TaskletTests
    {
        [TestMethod()]
        public void RunJobWithTasklet()
        {
            XmlJob job = XmlJobParser.LoadJob("Job1.xml");
            IJobOperator jobOperator = BatchRuntime.GetJobOperator(new MyUnityLoaderJob1(), job);
            Assert.IsNotNull(jobOperator);
            Assert.AreEqual(1,jobOperator.StartNextInstance(job.Id));
        }

        /// <summary>
        /// Extends UnityLoader and redefines LoadArtifacts() to supply the batch artifacts.
        /// </summary>
        private class MyUnityLoaderJob1 : UnityLoader
        {
            public override void LoadArtifacts(IUnityContainer unityContainer)
            {
                unityContainer.RegisterType<ITasklet, MyDummyTasklet>("tasklet1");
            }
        }

        private class MyDummyTasklet : ITasklet
        {
            public RepeatStatus Execute(StepContribution contribution, ChunkContext chunkContext)
            {
                // Write counter to a file - Should be running for 10 seconds roughly
                int counter = 1000;
                string[] lines = new string[counter];
                for (int i = 0; i < counter; i++)
                {
                    lines[i] = DateTime.Now.Ticks.ToString();
                    Thread.Sleep(10);
                }
                File.WriteAllLines(@"C:\temp\MyDummyTasklet_out_" + DateTime.Now.Ticks + ".txt", lines);
                return RepeatStatus.Finished;
            }
        }
    }
}
