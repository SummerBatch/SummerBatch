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
using System.Collections.Generic;
using System.Linq;
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
using Summer.Batch.Core.Scope.Context;
using Summer.Batch.Infrastructure.Repeat;

namespace Summer.Batch.CoreTests.Batch.Tasklets
{
    [TestClass]
    public class FileUtilsTaskletFileCompareTests
    {
        protected const string TestDataDirectoryIn = @".\TestData\FileUtils";
        protected const string TestDataDirectoryLogs = @".";

        [TestMethod]
        public void FileCompareDefault()
        {
            XmlJob job = XmlJobParser.LoadJob("Job1FileCompareDefault.xml");
            IJobOperator jobOperator = BatchRuntime.GetJobOperator(new MyUnityLoaderJob1FileCompareDefault(), job);
            Assert.IsNotNull(jobOperator);
            long? executionId = jobOperator.StartNextInstance(job.Id);
            Assert.IsNotNull(executionId);

            JobExecution jobExecution = ((SimpleJobOperator)jobOperator).JobExplorer.GetJobExecution((long)executionId);
            Assert.IsFalse(jobExecution.Status.IsUnsuccessful());
            Assert.IsFalse(jobExecution.Status.IsRunning());
        }

        [TestMethod]
        public void FileCompareIEBCOMPRLike()
        {
            XmlJob job = XmlJobParser.LoadJob("Job1FileCompareIEBCOMPRLike.xml");
            IJobOperator jobOperator = BatchRuntime.GetJobOperator(new MyUnityLoaderJob1FileCompareIEBCOMPRLike(), job);
            Assert.IsNotNull(jobOperator);
            long? executionId = jobOperator.StartNextInstance(job.Id);
            Assert.IsNotNull(executionId);

            JobExecution jobExecution = ((SimpleJobOperator)jobOperator).JobExplorer.GetJobExecution((long)executionId);
            Assert.IsFalse(jobExecution.Status.IsUnsuccessful());
            Assert.IsFalse(jobExecution.Status.IsRunning());
        }

        [TestMethod]
        public void FileCompare()
        {
            XmlJob job = XmlJobParser.LoadJob("Job1FileCompare.xml");
            IJobOperator jobOperator = BatchRuntime.GetJobOperator(new MyUnityLoaderJob1FileCompare(), job);
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
        private class MyUnityLoaderJob1FileCompareDefault : UnityLoader
        {
            public override void LoadArtifacts(IUnityContainer unityContainer)
            {
                //Register Tasklet
                unityContainer.RegisterType<ITasklet, FileUtilsTasklet>("JobFileCompare/CompareDifferentFiles",
                    new InjectionProperty("Strict", true),
                    new InjectionProperty("Mode", FileUtilsTasklet.FileUtilsMode.Compare),
                    new InjectionProperty("FileCompareMode", FileUtilsTasklet.FileType.Text),
                    new InjectionProperty("SequenceEqualityComparerType", FileUtilsTasklet.EqualityComparerType.Default),
                    new InjectionProperty("Sources",
                        new List<IResource> 
                        { 
                            new FileSystemResource(Path.Combine(TestDataDirectoryIn, "report0.txt")),
                            new FileSystemResource(Path.Combine(TestDataDirectoryIn, "report0_modified.txt")) 
 
                        })
                    );

                // 
                unityContainer.RegisterStepScope<ITasklet, BatchErrorTasklet>("JobFileCompare/BatchError");
                unityContainer.RegisterStepScope<ITasklet, EndBatchTasklet>("JobFileCompare/EndBatch");
            }
        }

        /// <summary>
        /// Extends UnityLoader and redefines LoadArtifacts() to supply the batch artifacts.
        /// </summary>
        private class MyUnityLoaderJob1FileCompareIEBCOMPRLike : UnityLoader
        {
            public override void LoadArtifacts(IUnityContainer unityContainer)
            {
                //Register Tasklet
                unityContainer.RegisterType<ITasklet, FileUtilsTasklet>("JobFileCompare/CompareDifferentFiles",
                    new InjectionProperty("Strict", true),
                    new InjectionProperty("Mode", FileUtilsTasklet.FileUtilsMode.Compare),
                    new InjectionProperty("FileCompareMode", FileUtilsTasklet.FileType.Text),
                    new InjectionProperty("SequenceEqualityComparerType", FileUtilsTasklet.EqualityComparerType.IebcomprLike),
                    new InjectionProperty("Sources",
                        new List<IResource> 
                        { 
                            new FileSystemResource(Path.Combine(TestDataDirectoryIn, "report0.txt")),
                            new FileSystemResource(Path.Combine(TestDataDirectoryIn, "report0_modified.txt"))                             
                        })
                    );

                // 
                unityContainer.RegisterStepScope<ITasklet, BatchErrorTasklet>("JobFileCompare/BatchError");
                unityContainer.RegisterStepScope<ITasklet, EndBatchTasklet>("JobFileCompare/EndBatch");
            }
        }

        /// <summary>
        /// Extends UnityLoader and redefines LoadArtifacts() to supply the batch artifacts.
        /// </summary>
        private class MyUnityLoaderJob1FileCompare : UnityLoader
        {
            public override void LoadArtifacts(IUnityContainer unityContainer)
            {

                //Register JobFileCompare/CompareSameFile step
                unityContainer.RegisterType<ITasklet, FileUtilsTasklet>("JobFileCompare/CompareSameFile",
                    new InjectionProperty("Strict", true),
                    new InjectionProperty("Mode", FileUtilsTasklet.FileUtilsMode.Compare),
                    new InjectionProperty("FileCompareMode", FileUtilsTasklet.FileType.Binary),
                    new InjectionProperty("Sources",
                        new List<IResource> 
                        { 
                            new FileSystemResource(Path.Combine(TestDataDirectoryIn, "report2.zip")),
                            new FileSystemResource(Path.Combine(TestDataDirectoryIn, "report2.zip")) 
                        })
                    );

                //Register JobFileCompare/CompareSameFiles step
                unityContainer.RegisterType<ITasklet, FileUtilsTasklet>("JobFileCompare/CompareSameFiles",
                    new InjectionProperty("Strict", true),
                    new InjectionProperty("Mode", FileUtilsTasklet.FileUtilsMode.Compare),
                    new InjectionProperty("FileCompareMode", FileUtilsTasklet.FileType.Text),
                    new InjectionProperty("Sources",
                        new List<IResource> 
                        { 
                            new FileSystemResource(Path.Combine(TestDataDirectoryIn, "report0.txt")),
                            new FileSystemResource(Path.Combine(TestDataDirectoryIn, "report0_copy.txt")) 
                        })
                    );

                //Register Tasklet
                unityContainer.RegisterType<ITasklet, FileUtilsTasklet>("JobFileCompare/CompareDifferentFiles",
                    new InjectionProperty("Strict", true),
                    new InjectionProperty("Mode", FileUtilsTasklet.FileUtilsMode.Compare),
                    new InjectionProperty("FileCompareMode", FileUtilsTasklet.FileType.Text),
                    new InjectionProperty("Sources",
                        new List<IResource> 
                        { 
                            //new FileSystemResource(Path.Combine(TestDataDirectoryIn, "except0.txt")),
                            //new FileSystemResource(Path.Combine(TestDataDirectoryIn, "except1.txt")) 
                            new FileSystemResource(Path.Combine(TestDataDirectoryIn, "report0.txt")),
                            new FileSystemResource(Path.Combine(TestDataDirectoryIn, "report0_modified.txt")) 
                        })
                    );

                // 
                unityContainer.RegisterStepScope<ITasklet, BatchErrorTasklet>("JobFileCompare/BatchError");
                unityContainer.RegisterStepScope<ITasklet, EndBatchTasklet>("JobFileCompare/EndBatch");
            }
        }

        public class EndBatchTasklet : ITasklet, IStepExecutionListener
        {
            /// <summary>
            /// Do nothing before step
            /// </summary>
            /// <param name="stepExecution"></param>
            public void BeforeStep(StepExecution stepExecution)
            {
                // Do nothing
            }

            /// <summary>
            /// Do nothing execution, since all the logic is in after step
            /// </summary>
            /// <param name="contribution"></param>
            /// <param name="chunkContext"></param>
            /// <returns></returns>
            public RepeatStatus Execute(StepContribution contribution, ChunkContext chunkContext)
            {
                return RepeatStatus.Finished;
            }

            public ExitStatus AfterStep(StepExecution stepExecution)
            {
                JobExecution job = stepExecution.JobExecution;
                var jobName = job.JobInstance.JobName;

                var filePathName = Path.Combine(TestDataDirectoryLogs, "1_" + jobName + "_" + job.StartTime.Value.Ticks+ ".log");
                FileInfo fInfo = new FileInfo(filePathName);

                using (StreamWriter logSW = new StreamWriter(fInfo.FullName, true))
                {
                    logSW.WriteLine("Job Name: " + jobName + ", Id: " + job.Id + " ended with EndBatch");
                    for (int i = job.StepExecutions.Count - 1; i > 0; i--)
                    {
                        var stepName = job.StepExecutions.ElementAt(i).StepName;
                        var exitCode = job.StepExecutions.ElementAt(i).ExitStatus.ExitCode;
                        var batchStatus = job.StepExecutions.ElementAt(i).BatchStatus;
                        var summary = job.StepExecutions.ElementAt(i).GetSummary();
                        ICollection<Exception> exceptions = job.StepExecutions.ElementAt(i).GetFailureExceptions();

                        //=> summary message...
                        logSW.WriteLine(summary);

                        //=> if there are exceptions do tell us...
                        if (exceptions.Count > 0)
                            logSW.WriteLine("Exceptions for Step: " + stepName + ", Batch Status: " + batchStatus + ", Exit Status: " + exitCode);

                        for (int j = 0; j < exceptions.Count; j++)
                        {
                            logSW.WriteLine("Exception @ Step[" + stepName + "]: " + exceptions.ElementAt(j).InnerException);
                        }
                    }
                }

                return ExitStatus.Completed;
            }

        }

        public class BatchErrorTasklet : ITasklet, IStepExecutionListener
        {
            /// <summary>
            /// Do nothing before step
            /// </summary>
            /// <param name="stepExecution"></param>
            public void BeforeStep(StepExecution stepExecution)
            {
                // Do nothing
            }

            /// <summary>
            /// Do nothing execution, since all the logic is in after step
            /// </summary>
            /// <param name="contribution"></param>
            /// <param name="chunkContext"></param>
            /// <returns></returns>
            public RepeatStatus Execute(StepContribution contribution, ChunkContext chunkContext)
            {
                return RepeatStatus.Finished;
            }

            public ExitStatus AfterStep(StepExecution stepExecution)
            {
                JobExecution job = stepExecution.JobExecution;
                var jobName = job.JobInstance.JobName;

                var filePathName = Path.Combine(TestDataDirectoryLogs, "1_" + jobName + "_" + job.StartTime.Value.Ticks+ ".log");
                FileInfo fInfo = new FileInfo(filePathName);

                using (StreamWriter logSW = new StreamWriter(fInfo.FullName, true))
                {
                    logSW.WriteLine("Job Name: " + jobName + ", Id: " + job.Id + " ended with BatchError.");
                    for (int i = job.StepExecutions.Count - 1; i > 0; i--)
                    {
                        var stepName = job.StepExecutions.ElementAt(i).StepName;
                        var exitCode = job.StepExecutions.ElementAt(i).ExitStatus.ExitCode;
                        var batchStatus = job.StepExecutions.ElementAt(i).BatchStatus;
                        var summary = job.StepExecutions.ElementAt(i).GetSummary();
                        ICollection<Exception> exceptions = job.StepExecutions.ElementAt(i).GetFailureExceptions();

                        logSW.WriteLine(summary);
                        logSW.WriteLine("Step: " + stepName + ", Batch Status: " + batchStatus + ", Exit Status: " + exitCode);
                        for (int j = 0; j < exceptions.Count; j++)
                        {
                            logSW.WriteLine("Exception @ Step[" + stepName + "]: " + exceptions.ElementAt(j).InnerException);
                        }
                    }
                }

                return ExitStatus.Completed;
            }
        }
    }
}
