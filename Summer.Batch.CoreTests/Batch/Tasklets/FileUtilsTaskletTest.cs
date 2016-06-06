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
using System.Collections.Generic;
using System.IO;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Summer.Batch.Common.IO;
using Summer.Batch.Core;
using Summer.Batch.Core.Launch;
using Summer.Batch.Core.Launch.Support;
using Summer.Batch.Core.Step.Tasklet;
using Summer.Batch.Core.Unity;
using Summer.Batch.Core.Unity.Xml;
using Summer.Batch.Extra.IO;
using Summer.Batch.Infrastructure.Item.Util;

namespace Summer.Batch.CoreTests.Batch.Tasklets
{
    [TestClass]
    public class FileUtilsTaskletTest
    {

        protected const string TestDataDirectoryIn = @".\TestData\FileUtils";
        protected const string TestDataDirectoryOut = @"C:\temp\fileutils";
        protected const string TestDataDirectoryToDelete = @"C:\temp\toDelete";
        protected const string TestDataDirectoryToReset = @"C:\temp\toReset";
        protected const string TestDataDirectoryToMerge = @"C:\temp\toMerge";

        [TestMethod()]
        public void RunJobWithTaskletCopy()
        {
            XmlJob job = XmlJobParser.LoadJob("Job1.xml");
            IJobOperator jobOperator = BatchRuntime.GetJobOperator(new MyUnityLoaderJob1Copy(), job);
            Assert.IsNotNull(jobOperator);
            long? executionId = jobOperator.StartNextInstance(job.Id);
            Assert.IsNotNull(executionId);

            JobExecution jobExecution = ((SimpleJobOperator)jobOperator).JobExplorer.GetJobExecution((long)executionId);
            Assert.IsFalse(jobExecution.Status.IsUnsuccessful());
            Assert.IsFalse(jobExecution.Status.IsRunning());

        }

        [TestMethod()]
        public void RunJobWithTaskletCopy2()
        {
            XmlJob job = XmlJobParser.LoadJob("Job1.xml");
            IJobOperator jobOperator = BatchRuntime.GetJobOperator(new MyUnityLoaderJob1Copy2(), job);
            Assert.IsNotNull(jobOperator);
            long? executionId = jobOperator.StartNextInstance(job.Id);
            Assert.IsNotNull(executionId);

            JobExecution jobExecution = ((SimpleJobOperator)jobOperator).JobExplorer.GetJobExecution((long)executionId);
            Assert.IsFalse(jobExecution.Status.IsUnsuccessful());
            Assert.IsFalse(jobExecution.Status.IsRunning());

        }

        [TestMethod()]
        public void RunJobWithTaskletDelete()
        {
            FileUtils.CopyDir(TestDataDirectoryIn,TestDataDirectoryToDelete);

            XmlJob job = XmlJobParser.LoadJob("Job1.xml");
            IJobOperator jobOperator = BatchRuntime.GetJobOperator(new MyUnityLoaderJob1Delete(), job);
            Assert.IsNotNull(jobOperator);
            long? executionId = jobOperator.StartNextInstance(job.Id);
            Assert.IsNotNull(executionId);

            JobExecution jobExecution = ((SimpleJobOperator)jobOperator).JobExplorer.GetJobExecution((long)executionId);
            Assert.IsFalse(jobExecution.Status.IsUnsuccessful());
            Assert.IsFalse(jobExecution.Status.IsRunning());

        }

        [TestMethod()]
        public void RunJobWithTaskletReset()
        {
            FileUtils.CopyDir(TestDataDirectoryIn, TestDataDirectoryToReset);

            XmlJob job = XmlJobParser.LoadJob("Job1.xml");
            IJobOperator jobOperator = BatchRuntime.GetJobOperator(new MyUnityLoaderJob1Reset(), job);
            Assert.IsNotNull(jobOperator);
            long? executionId = jobOperator.StartNextInstance(job.Id);
            Assert.IsNotNull(executionId);

            JobExecution jobExecution = ((SimpleJobOperator)jobOperator).JobExplorer.GetJobExecution((long)executionId);
            Assert.IsFalse(jobExecution.Status.IsUnsuccessful());
            Assert.IsFalse(jobExecution.Status.IsRunning());

        }



        [TestMethod()]
        public void RunJobWithTaskletMerge()
        {
            if (!Directory.Exists(TestDataDirectoryToMerge))
            {
                Directory.CreateDirectory(TestDataDirectoryToMerge);
            }

            XmlJob job = XmlJobParser.LoadJob("Job1.xml");
            IJobOperator jobOperator = BatchRuntime.GetJobOperator(new MyUnityLoaderJob1Merge(), job);
            Assert.IsNotNull(jobOperator);
            long? executionId = jobOperator.StartNextInstance(job.Id);
            Assert.IsNotNull(executionId);

            JobExecution jobExecution = ((SimpleJobOperator)jobOperator).JobExplorer.GetJobExecution((long)executionId);
            Assert.IsFalse(jobExecution.Status.IsUnsuccessful());
            Assert.IsFalse(jobExecution.Status.IsRunning());

        }


        [TestMethod()]
        public void RunJobWithTaskletMergeCopy()
        {
            //prepare stuff
            if (!Directory.Exists(TestDataDirectoryToMerge))
            {
                Directory.CreateDirectory(TestDataDirectoryToMerge);
            }

            FileInfo ofi = new FileInfo(Path.Combine(TestDataDirectoryToMerge, "report1_merged_copy.txt"));
            if (!ofi.Exists)
            {
                using (FileStream fs = File.OpenRead(Path.Combine(TestDataDirectoryIn, "report0.txt")))
                using (FileStream ofs = File.OpenWrite(Path.Combine(TestDataDirectoryToMerge, "report1_merged_copy.txt")))
                {
                    fs.CopyTo(ofs);
                }
            }

            XmlJob job = XmlJobParser.LoadJob("Job1.xml");
            IJobOperator jobOperator = BatchRuntime.GetJobOperator(new MyUnityLoaderJob1MergeCopy(), job);
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
        private class MyUnityLoaderJob1Copy : UnityLoader
        {
            public override void LoadArtifacts(IUnityContainer unityContainer)
            {

                //Register Tasklet
                unityContainer.RegisterType<ITasklet, FileUtilsTasklet>("tasklet1",
                    new InjectionProperty("Mode",FileUtilsTasklet.FileUtilsMode.Copy),
                    new InjectionProperty("Sources",new List<IResource>{new FileSystemResource(TestDataDirectoryIn)}),
                    new InjectionProperty("Targets",new List<IResource>{new FileSystemResource(TestDataDirectoryOut)})
                    );
            }
        }
        private class MyUnityLoaderJob1Copy2 : UnityLoader
        {
            public override void LoadArtifacts(IUnityContainer unityContainer)
            {

                //Register Tasklet
                unityContainer.RegisterType<ITasklet, FileUtilsTasklet>("tasklet1",
                    new InjectionProperty("Mode", FileUtilsTasklet.FileUtilsMode.Copy),
                    new InjectionProperty("Sources", new List<IResource>
                    {
                        new FileSystemResource(Path.Combine(TestDataDirectoryIn, "report0.txt"))
                    }),
                    new InjectionProperty("Targets", new List<IResource>
                    {
                        new FileSystemResource(Path.Combine(TestDataDirectoryOut, "report0-copy.txt"))
                    })
                );
            }
        }

        /// <summary>
        /// Extends UnityLoader and redefines LoadArtifacts() to supply the batch artifacts.
        /// </summary>
        private class MyUnityLoaderJob1Delete : UnityLoader
        {
            public override void LoadArtifacts(IUnityContainer unityContainer)
            {

                //Register Tasklet
                unityContainer.RegisterType<ITasklet, FileUtilsTasklet>("tasklet1",
                    new InjectionProperty("Mode", FileUtilsTasklet.FileUtilsMode.Delete),
                    new InjectionProperty("Targets", new List<IResource> { new FileSystemResource(TestDataDirectoryToDelete) })
                    );
            }
        }

        /// <summary>
        /// Extends UnityLoader and redefines LoadArtifacts() to supply the batch artifacts.
        /// </summary>
        private class MyUnityLoaderJob1Reset : UnityLoader
        {
            public override void LoadArtifacts(IUnityContainer unityContainer)
            {

                //Register Tasklet
                unityContainer.RegisterType<ITasklet, FileUtilsTasklet>("tasklet1",
                    new InjectionProperty("Mode", FileUtilsTasklet.FileUtilsMode.Reset),
                    new InjectionProperty("Targets", 
                        new List<IResource>
                        {
                            new FileSystemResource(Path.Combine(TestDataDirectoryToReset,"report0.txt")),
                            new FileSystemResource(Path.Combine(TestDataDirectoryToReset,"report2.txt")),
                            //new FileSystemResource(Path.Combine(TestDataDirectoryToReset,"report4.txt")),
                            //new FileSystemResource(Path.Combine(TestDataDirectoryToReset,"report4_1.txt"))
                        })
                    );
            }
        }

        /// <summary>
        /// Extends UnityLoader and redefines LoadArtifacts() to supply the batch artifacts.
        /// </summary>
        private class MyUnityLoaderJob1Merge : UnityLoader
        {
            public override void LoadArtifacts(IUnityContainer unityContainer)
            {

                //Register Tasklet
                unityContainer.RegisterType<ITasklet, FileUtilsTasklet>("tasklet1",
                    new InjectionProperty("Mode", FileUtilsTasklet.FileUtilsMode.Merge),
                    new InjectionProperty("Sources",
                        new List<IResource>
                        {
                            new FileSystemResource(Path.Combine(TestDataDirectoryIn,"report0.txt")),                            
                        }),
                    new InjectionProperty("Targets",
                        new List<IResource>
                        {
                            new FileSystemResource(Path.Combine(TestDataDirectoryToMerge,"report0_merged.txt")),                            
                        })
                    );
            }
        }

        /// <summary>
        /// Extends UnityLoader and redefines LoadArtifacts() to supply the batch artifacts.
        /// </summary>
        private class MyUnityLoaderJob1MergeCopy : UnityLoader
        {
            public override void LoadArtifacts(IUnityContainer unityContainer)
            {

                //Register Tasklet
                unityContainer.RegisterType<ITasklet, FileUtilsTasklet>("tasklet1",
                    new InjectionProperty("Mode", FileUtilsTasklet.FileUtilsMode.MergeCopy),
                    new InjectionProperty("Sources",
                        new List<IResource>
                        {
                            new FileSystemResource(Path.Combine(TestDataDirectoryToMerge,"report1_merged_copy.txt")),                            
                        }),
                    new InjectionProperty("Targets",
                        new List<IResource>
                        {
                            new FileSystemResource(Path.Combine(TestDataDirectoryToMerge,"report1_merged_copy.txt")),                            
                        })
                    );
            }
        } 
    }
}