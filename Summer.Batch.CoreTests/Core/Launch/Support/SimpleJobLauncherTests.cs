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
using Summer.Batch.Core.Converter;
using Summer.Batch.Core.Job;
using Summer.Batch.Core.Launch.Support;
using Summer.Batch.Core.Repository;
using Summer.Batch.Core.Repository.Dao;
using Summer.Batch.Core.Repository.Support;
using Summer.Batch.Core.Step.Tasklet;
using Summer.Batch.Common.TaskExecution;
using System.Collections.Specialized;
using System.IO;
using System.Threading;
using Summer.Batch.Core.Scope.Context;
using Summer.Batch.Infrastructure.Repeat;

namespace Summer.Batch.CoreTests.Core.Launch.Support
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

        [TestMethod()]
        public void RunTestSynchronousExecutor()
        {            
            SimpleJobLauncher launcher = new SimpleJobLauncher();
            IJobInstanceDao jobInstanceDao = new MapJobInstanceDao();
            IJobExecutionDao jobExecutionDao = new MapJobExecutionDao();
            IStepExecutionDao stepExecutionDao = new MapStepExecutionDao();
            IExecutionContextDao executionContextDao = new MapExecutionContextDao();

            IJobRepository jobRepository =
                new SimpleJobRepository(jobInstanceDao, jobExecutionDao, stepExecutionDao, executionContextDao);
            launcher.JobRepository = jobRepository;

            DefaultJobParametersConverter converter = new DefaultJobParametersConverter();
            NameValueCollection props2 = new NameValueCollection
            {
                {"+dateDebut(date)", "1970/07/31"},
                {"+everything(long)", "42"},
                {"-balance(double)", "1000.0"},
                {"+name(string)", "thierry"},
                {"-default", "default"}
            };
            JobParameters jobParameters = converter.GetJobParameters(props2);

            IJob job = new SimpleJob("myTestJob");
            job.JobParametersValidator = new DummyValidator();
            job.Restartable = false;
            TaskletStep step = new TaskletStep("simpleTS")
            {
                JobRepository = jobRepository,
                Tasklet = new MyDummyTasklet()
            };
            ((SimpleJob)job).AddStep(step);
            ((SimpleJob) job).JobRepository = jobRepository;
            JobExecution jobExecution = launcher.Run(job, jobParameters);
            Assert.IsFalse(jobExecution.Status.IsUnsuccessful());
            Assert.IsFalse(jobExecution.Status.IsRunning()); 
        }


        [TestMethod()]
        public void RunTestASynchronousExecutor()
        {
            SimpleJobLauncher launcher = new SimpleJobLauncher();
            IJobInstanceDao jobInstanceDao = new MapJobInstanceDao();
            IJobExecutionDao jobExecutionDao = new MapJobExecutionDao();
            IStepExecutionDao stepExecutionDao = new MapStepExecutionDao();
            IExecutionContextDao executionContextDao = new MapExecutionContextDao();

            IJobRepository jobRepository =
                new SimpleJobRepository(jobInstanceDao, jobExecutionDao, stepExecutionDao, executionContextDao);
            launcher.JobRepository = jobRepository;

            DefaultJobParametersConverter converter = new DefaultJobParametersConverter();
            NameValueCollection props2 = new NameValueCollection
            {
                {"+dateDebut(date)", "1970/07/31"},
                {"+everything(long)", "42"},
                {"-balance(double)", "1000.0"},
                {"+name(string)", "thierry"},
                {"-default", "default"}
            };
            JobParameters jobParameters = converter.GetJobParameters(props2);

            IJob job = new SimpleJob("myTestJob");
            job.JobParametersValidator = new DummyValidator();
            job.Restartable = false;
            TaskletStep step = new TaskletStep("simpleTS")
            {
                JobRepository = jobRepository,
                Tasklet = new MyDummyTasklet2()
            };
            ((SimpleJob)job).AddStep(step);
            ((SimpleJob)job).JobRepository = jobRepository;
            ITaskExecutor taskExecutor = new SimpleAsyncTaskExecutor();
            launcher.TaskExecutor = taskExecutor;
            JobExecution jobExecution = launcher.Run(job, jobParameters);
            //wait for execution end (asynchronous)            
            Assert.IsFalse(jobExecution.Status.IsUnsuccessful());
            Assert.IsFalse(jobExecution.Status.IsRunning());
        }

        [TestMethod()]
        public void RunTestSystemCommandTasklet()
        {
            SimpleJobLauncher launcher = new SimpleJobLauncher();
            IJobInstanceDao jobInstanceDao = new MapJobInstanceDao();
            IJobExecutionDao jobExecutionDao = new MapJobExecutionDao();
            IStepExecutionDao stepExecutionDao = new MapStepExecutionDao();
            IExecutionContextDao executionContextDao = new MapExecutionContextDao();

            IJobRepository jobRepository =
                new SimpleJobRepository(jobInstanceDao, jobExecutionDao, stepExecutionDao, executionContextDao);
            launcher.JobRepository = jobRepository;

            DefaultJobParametersConverter converter = new DefaultJobParametersConverter();
            NameValueCollection props2 = new NameValueCollection
            {
                {"+dateDebut(date)", "1970/07/31"},
                {"+everything(long)", "42"},
                {"-balance(double)", "1000.0"},
                {"+name(string)", "thierry"},
                {"-default", "default"}
            };
            JobParameters jobParameters = converter.GetJobParameters(props2);

            IJob job = new SimpleJob("myTestJob");
            job.JobParametersValidator = new DummyValidator();
            job.Restartable = false;
            TaskletStep step = new TaskletStep("simpleTS") {JobRepository = jobRepository};
            SystemCommandTasklet tasklet = new SystemCommandTasklet
            {
                Command = "DEL MyDummyTasklet2_out_*.txt",
                WorkingDirectory = "C:/temp",
                Timeout = 10000000,
                SystemProcessExitCodeMapper = new SimpleSystemProcessExitCodeMapper()
            };
            step.Tasklet = tasklet;
            step.RegisterStepExecutionListener(tasklet);
            ((SimpleJob)job).AddStep(step);
            ((SimpleJob)job).JobRepository = jobRepository;
            ITaskExecutor taskExecutor = new SimpleAsyncTaskExecutor();
            launcher.TaskExecutor = taskExecutor;
            JobExecution jobExecution = launcher.Run(job, jobParameters);
            //wait for execution end (asynchronous)            
            Assert.IsFalse(jobExecution.Status.IsUnsuccessful());
            Assert.IsFalse(jobExecution.Status.IsRunning());
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

        private class MyDummyTasklet2 : ITasklet
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
                File.WriteAllLines(@"C:\temp\MyDummyTasklet2_out_" + DateTime.Now.Ticks + ".txt", lines);
                return RepeatStatus.Finished;
            }
        }
    }
}
