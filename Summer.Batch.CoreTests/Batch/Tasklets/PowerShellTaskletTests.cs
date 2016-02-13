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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Summer.Batch.Core;
using Summer.Batch.Core.Unity;
using Summer.Batch.Core.Unity.Xml;
using Summer.Batch.Core.Converter;
using Summer.Batch.Core.Job;
using Summer.Batch.Core.Launch;
using Summer.Batch.Core.Launch.Support;
using Summer.Batch.Core.Repository;
using Summer.Batch.Core.Repository.Dao;
using Summer.Batch.Core.Repository.Support;
using Summer.Batch.Core.Step.Tasklet;
using Summer.Batch.Common.TaskExecution;
using Summer.Batch.Common.IO;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Summer.Batch.Core.Scope.Context;
using Summer.Batch.Infrastructure.Repeat;
using Microsoft.Practices.Unity;

namespace Summer.Batch.CoreTests.Batch.Tasklets
{
    [TestClass()]
    public class SimpleJobLauncherTests
    {
        class DummyValidator : IJobParametersValidator
        {
            public void Validate(JobParameters parameters)
            {
                //Do nothing on purpose
            }
        }

        //[TestMethod()]
        //public void RunLauncherPowerShellTasklet()
        //{
        //    SimpleJobLauncher launcher = new SimpleJobLauncher();
        //    IJobInstanceDao jobInstanceDao = new MapJobInstanceDao();
        //    IJobExecutionDao jobExecutionDao = new MapJobExecutionDao();
        //    IStepExecutionDao stepExecutionDao = new MapStepExecutionDao();
        //    IExecutionContextDao executionContextDao = new MapExecutionContextDao();

        //    IJobRepository jobRepository =
        //        new SimpleJobRepository(jobInstanceDao, jobExecutionDao, stepExecutionDao, executionContextDao);
        //    launcher.JobRepository = jobRepository;

        //    DefaultJobParametersConverter converter = new DefaultJobParametersConverter();
        //    NameValueCollection props2 = new NameValueCollection
        //    {
        //        {"+dateDebut(date)", "1970/07/31"},
        //        {"+everything(long)", "42"},
        //        {"-balance(double)", "1000.0"},
        //        {"+name(string)", "thierry"},
        //        {"-default", "default"}
        //    };
        //    JobParameters jobParameters = converter.GetJobParameters(props2);

        //    IJob job = new SimpleJob("myPowerShellJob");
        //    job.JobParametersValidator = new DummyValidator();
        //    job.Restartable = false;
        //    TaskletStep step = new TaskletStep("simpleTS") {JobRepository = jobRepository};

        //    //=> Prep input for PowerShellTasklet
        //    Dictionary<string, object> parameters = new Dictionary<string, object>();

        //    string scriptFile = @".\TestData\PowerShell\Script1.ps1";
        //    parameters.Add("DateTimeNow", DateTime.Now);
        //    parameters.Add("scriptFile", scriptFile);

        //    PowerShellTasklet tasklet = new PowerShellTasklet
        //    {
        //        ScriptResource = new FileSystemResource(scriptFile),
        //        Parameters = parameters,
        //        Timeout = 10000,
        //        PowerShellExitCodeMapper = new PowerShellExitCodeMapper()
        //    };
        //    step.Tasklet = tasklet;
        //    step.RegisterStepExecutionListener(tasklet);
        //    ((SimpleJob)job).AddStep(step);
        //    ((SimpleJob)job).JobRepository = jobRepository;
        //    ITaskExecutor taskExecutor = new SimpleAsyncTaskExecutor();
        //    launcher.TaskExecutor = taskExecutor;
        //    JobExecution jobExecution = launcher.Run(job, jobParameters);
        //    //wait for execution end (asynchronous)            
        //    Assert.IsFalse(jobExecution.Status.IsUnsuccessful());
        //    Assert.IsFalse(jobExecution.Status.IsRunning());
        //}

        [TestMethod]
        public void PowerShellTimeoutTest()
        {
            XmlJob job = XmlJobParser.LoadJob("JobPowerShellTimeoutTest.xml");
            IJobOperator jobOperator = BatchRuntime.GetJobOperator(new MyUnityLoaderPowerShellTimeoutTest(), job);
            Assert.IsNotNull(jobOperator);
            long? executionId = jobOperator.StartNextInstance(job.Id);
            Assert.IsNotNull(executionId);
            JobExecution jobExecution = ((SimpleJobOperator)jobOperator).JobExplorer.GetJobExecution((long)executionId);
            Assert.IsTrue(jobExecution.Status.IsUnsuccessful());
            Assert.IsFalse(jobExecution.Status.IsRunning());
        }

        [TestMethod]
        public void PowerShellTimeoutTestThrowException()
        {
            XmlJob job = XmlJobParser.LoadJob("JobPowerShellTimeoutTest.xml");
            IJobOperator jobOperator = BatchRuntime.GetJobOperator(new MyUnityLoaderPowerShellTimeoutTestThrowException(), job);
            Assert.IsNotNull(jobOperator);
            long? executionId = jobOperator.StartNextInstance(job.Id);
            Assert.IsNotNull(executionId);
            JobExecution jobExecution = ((SimpleJobOperator)jobOperator).JobExplorer.GetJobExecution((long)executionId);
            Assert.IsTrue(jobExecution.Status.IsUnsuccessful());
            Assert.IsFalse(jobExecution.Status.IsRunning());
        }

        [TestMethod]
        public void PowerShellSampleBatch1()
        {
            XmlJob job = XmlJobParser.LoadJob("JobPowerShellSampleBatch1.xml");
            IJobOperator jobOperator = BatchRuntime.GetJobOperator(new MyUnityLoaderPowerShellSampleBatch1(), job);
            Assert.IsNotNull(jobOperator);
            long? executionId = jobOperator.StartNextInstance(job.Id);
            Assert.IsNotNull(executionId);
            JobExecution jobExecution = ((SimpleJobOperator)jobOperator).JobExplorer.GetJobExecution((long)executionId);
            Assert.IsFalse(jobExecution.Status.IsUnsuccessful());
            Assert.IsFalse(jobExecution.Status.IsRunning());
        }

        [TestMethod]
        public void PowerShellSampleBatch1WithError()
        {
            XmlJob job = XmlJobParser.LoadJob("JobPowerShellSampleBatch1.xml");
            IJobOperator jobOperator = BatchRuntime.GetJobOperator(new MyUnityLoaderPowerShellSampleBatch1WithError(), job);
            Assert.IsNotNull(jobOperator);
            long? executionId = jobOperator.StartNextInstance(job.Id);
            Assert.IsNotNull(executionId);
            JobExecution jobExecution = ((SimpleJobOperator)jobOperator).JobExplorer.GetJobExecution((long)executionId);
            Assert.IsTrue(jobExecution.Status.IsUnsuccessful());
            Assert.IsFalse(jobExecution.Status.IsRunning());
        }

        [TestMethod]
        public void PowerShellExitStatus()
        {
            XmlJob job = XmlJobParser.LoadJob("JobPowerShellExitStatus.xml");
            IJobOperator jobOperator = BatchRuntime.GetJobOperator(new MyUnityLoaderPowerShellExitStatus(), job);
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
        private class MyUnityLoaderPowerShellTimeoutTest : UnityLoader
        {
            public override void LoadArtifacts(IUnityContainer unityContainer)
            {
                //=> Prep input for PowerShellTasklet
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                Dictionary<string, object> variables = new Dictionary<string, object>();

                string scriptFile = @".\TestData\PowerShell\ScriptTimeOut.ps1";
                parameters.Add("DateTimeNow", DateTime.Now);
                parameters.Add("scriptFile", scriptFile);

                //=> Set Variables...
                FileInfo _scriptFileInfo = new FileInfo(scriptFile);
                variables.Add("fileInfo", _scriptFileInfo);

                //Register Tasklet
                unityContainer.RegisterType<ITasklet, PowerShellTasklet>("JobPowerShellTest/TimeoutStep",
                    new InjectionProperty("ScriptResource", new FileSystemResource(scriptFile)),
                    new InjectionProperty("Parameters", parameters), // parameters passed to script 
                    new InjectionProperty("Variables", variables), // variabes available within script
                    new InjectionProperty("Timeout", 5000L), //NOTE : Timeout has to be given in ms and it is a long
                    new InjectionProperty("TimeoutBehavior", PowerShellTasklet.TimeoutBehaviorOption.SetExitStatusToFailed), // this is default if not set
                    new InjectionProperty("PowerShellExitCodeMapper", new PowerShellExitCodeMapper())
                    );

                // 
                //unityContainer.RegisterStepScope<ITasklet, BatchErrorTasklet>("JobFileCompare/BatchError");
                //unityContainer.RegisterStepScope<ITasklet, EndBatchTasklet>("JobFileCompare/EndBatch");
            }
        }

                /// <summary>
        /// Extends UnityLoader and redefines LoadArtifacts() to supply the batch artifacts.
        /// </summary>
        private class MyUnityLoaderPowerShellTimeoutTestThrowException : UnityLoader
        {
            public override void LoadArtifacts(IUnityContainer unityContainer)
            {
                //=> Prep input for PowerShellTasklet
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                Dictionary<string, object> variables = new Dictionary<string, object>();

                string scriptFile = @".\TestData\PowerShell\ScriptTimeOut.ps1";
                parameters.Add("DateTimeNow", DateTime.Now);
                parameters.Add("scriptFile", scriptFile);

                //=> Set Variables...
                FileInfo _scriptFileInfo = new FileInfo(scriptFile);
                variables.Add("fileInfo", _scriptFileInfo);

                //Register Tasklet
                unityContainer.RegisterType<ITasklet, PowerShellTasklet>("JobPowerShellTest/TimeoutStep",
                    new InjectionProperty("ScriptResource", new FileSystemResource(scriptFile)),
                    new InjectionProperty("Parameters", parameters), // parameters passed to script 
                    new InjectionProperty("Variables", variables), // variabes available within script
                    new InjectionProperty("Timeout", 5000L), //NOTE : Timeout has to be given in ms and it is a long
                    new InjectionProperty("TimeoutBehavior", PowerShellTasklet.TimeoutBehaviorOption.ThrowException), // this is default if not set
                    new InjectionProperty("PowerShellExitCodeMapper", new PowerShellExitCodeMapper())
                    );

                // 
                //unityContainer.RegisterStepScope<ITasklet, BatchErrorTasklet>("JobFileCompare/BatchError");
                //unityContainer.RegisterStepScope<ITasklet, EndBatchTasklet>("JobFileCompare/EndBatch");
            }
        }

        /// <summary>
        /// Extends UnityLoader and redefines LoadArtifacts() to supply the batch artifacts.
        /// </summary>
        private class MyUnityLoaderPowerShellSampleBatch1 : UnityLoader
        {
            public override void LoadArtifacts(IUnityContainer unityContainer)
            {
                //=> Prep input for PowerShellTasklet
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                Dictionary<string, object> variables = new Dictionary<string, object>();
                List<FileInfo> filesToCompare = new List<FileInfo>();

                //=> Set Variables...
                string scriptFile = @".\TestData\PowerShell\ScriptSampleBatch1.ps1";
                string fileToCompare = @".\TestData\FileUtils\report0.txt";
                
                filesToCompare.Add(new FileInfo(@".\TestData\FileUtils\report0.txt"));
                filesToCompare.Add(new FileInfo(@".\TestData\FileUtils\report0_copy.txt"));
                filesToCompare.Add(new FileInfo(@".\TestData\FileUtils\report0_modified.txt"));

                variables.Add("scriptFileInfo", new FileInfo(scriptFile));
                variables.Add("fileToCompare", new FileInfo(fileToCompare));
                variables.Add("filesToCompare", filesToCompare);

                //ExitStatus ScriptExitStatus = ExitStatus.Unknown;
                //variables.Add("ScriptExitStatus", ScriptExitStatus);

                //Register Tasklet
                unityContainer.RegisterType<ITasklet, PowerShellTasklet>("JobPowerSampleBatch1/CopyCompareDelete",
                    new InjectionProperty("ScriptResource", new FileSystemResource(scriptFile)),
                    //new InjectionProperty("Parameters", null), // script takes no parameters 
                    new InjectionProperty("Variables", variables), // variabes available within script
                    new InjectionProperty("Timeout", 100000L), //NOTE : Timeout has to be given in ms and it is a long
                    new InjectionProperty("TimeoutBehavior", PowerShellTasklet.TimeoutBehaviorOption.SetExitStatusToFailed), // this is default if not set
                    new InjectionProperty("PowerShellExitCodeMapper", new PowerShellExitCodeMapper())
                    );

                // 
                //unityContainer.RegisterStepScope<ITasklet, BatchErrorTasklet>("JobFileCompare/BatchError");
                //unityContainer.RegisterStepScope<ITasklet, EndBatchTasklet>("JobFileCompare/EndBatch");
            }
        }

        /// <summary>
        /// Extends UnityLoader and redefines LoadArtifacts() to supply the batch artifacts.
        /// </summary>
        private class MyUnityLoaderPowerShellSampleBatch1WithError : UnityLoader
        {
            public override void LoadArtifacts(IUnityContainer unityContainer)
            {
                //=> Prep input for PowerShellTasklet
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                Dictionary<string, object> variables = new Dictionary<string, object>();
                List<FileInfo> filesToCompare = new List<FileInfo>();

                //=> Set Variables...
                string scriptFile = @".\TestData\PowerShell\ScriptSampleBatch1.ps1";
                string fileToCompare = @".\TestData\FileUtils\file-does-not-exist.txt";

                filesToCompare.Add(new FileInfo(@".\TestData\FileUtils\report0.txt"));
                filesToCompare.Add(new FileInfo(@".\TestData\FileUtils\report0_copy.txt"));
                filesToCompare.Add(new FileInfo(@".\TestData\FileUtils\report0_modified.txt"));

                variables.Add("scriptFileInfo", new FileInfo(scriptFile));
                variables.Add("fileToCompare", new FileInfo(fileToCompare));
                variables.Add("filesToCompare", filesToCompare);

                //Register Tasklet
                unityContainer.RegisterType<ITasklet, PowerShellTasklet>("JobPowerSampleBatch1/CopyCompareDelete",
                    new InjectionProperty("ScriptResource", new FileSystemResource(scriptFile)),
                    //new InjectionProperty("Parameters", null), // script takes no parameters 
                    new InjectionProperty("Variables", variables), // variabes available within script
                    new InjectionProperty("Timeout", 100000L), //NOTE : Timeout has to be given in ms and it is a long
                    new InjectionProperty("TimeoutBehavior", PowerShellTasklet.TimeoutBehaviorOption.SetExitStatusToFailed), // this is default if not set
                    new InjectionProperty("PowerShellExitCodeMapper", new PowerShellExitCodeMapper())
                    );

                // 
                //unityContainer.RegisterStepScope<ITasklet, BatchErrorTasklet>("JobFileCompare/BatchError");
                //unityContainer.RegisterStepScope<ITasklet, EndBatchTasklet>("JobFileCompare/EndBatch");
            }
        }

        private class MyUnityLoaderPowerShellExitStatus : UnityLoader
        {
            public override void LoadArtifacts(IUnityContainer unityContainer)
            {
                //=> Prep input for PowerShellTasklet
                Dictionary<string, object> variables = new Dictionary<string, object>();

                //=> Set Variables...
                string scriptFile = @".\TestData\PowerShell\ScriptExitStatus.ps1";

                //Register Tasklet
                unityContainer.RegisterType<ITasklet, PowerShellTasklet>("JobPowerExitStatus/TestExitStatus",
                    new InjectionProperty("ScriptResource", new FileSystemResource(scriptFile)),
                    //new InjectionProperty("Parameters", null), // script takes no parameters 
                    //new InjectionProperty("Variables", variables), // variabes available within script
                    new InjectionProperty("Timeout", 100000L), //NOTE : Timeout has to be given in ms and it is a long
                    new InjectionProperty("TimeoutBehavior", PowerShellTasklet.TimeoutBehaviorOption.SetExitStatusToFailed), // this is default if not set
                    new InjectionProperty("PowerShellExitCodeMapper", new PowerShellExitCodeMapper())
                    );

                // 
                //unityContainer.RegisterStepScope<ITasklet, BatchErrorTasklet>("JobFileCompare/BatchError");
                //unityContainer.RegisterStepScope<ITasklet, EndBatchTasklet>("JobFileCompare/EndBatch");
            }
        }
    }
}

