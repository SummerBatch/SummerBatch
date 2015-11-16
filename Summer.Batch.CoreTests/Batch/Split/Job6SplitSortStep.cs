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
using Summer.Batch.Core.Step.Tasklet;
using Summer.Batch.Core.Unity;
using Summer.Batch.Extra.Sort;
using Summer.Batch.Common.IO;
using Summer.Batch.Infrastructure.Item;
using Summer.Batch.Infrastructure.Item.File;
using Summer.Batch.Infrastructure.Item.File.Mapping;
using Summer.Batch.Infrastructure.Item.File.Transform;
using Summer.Batch.Common.TaskExecution;

namespace Summer.Batch.CoreTests.Batch.Split
{
    /// <summary>
    /// Tests a split processing of a sort step and a read/process/write step
    /// </summary>
    [TestClass()]
    public class Job6SplitSortStep : AbstractSplitLaunchTests
    {
        private static readonly string TestPathIn = Path.Combine(TestDataDirectoryIn, "Job6In.txt");
        private static readonly string TestPathOutSorted = Path.Combine(TestDataDirectoryOut, "Job6Sorted.txt");
        private static readonly string TestPathOutUpper = Path.Combine(TestDataDirectoryOut, "Job6Upper.txt");

        protected override string[] GetFileNamesIn()
        {
            return new[] { TestPathIn };
        }

        protected override string[] GetFileNamesOut()
        {
            return new[] { TestPathOutSorted, TestPathOutUpper };
        }

        [TestMethod()]
        public void RunJobWithSplitSync()
        {
            RunJob("Job6.xml", "Job6", new MyUnityLoaderJob6Sync());
            // Post controls
            FileInfo outputFile = new FileInfo(TestPathOutSorted);
            Assert.IsTrue(outputFile.Exists, "Job output file does not exist, job was not successful");
            Assert.IsTrue(outputFile.Length > 0, "Job output file is empty, job was not successful");
            FileInfo outputFile2 = new FileInfo(TestPathOutUpper);
            Assert.IsTrue(outputFile2.Exists, "Job output file does not exist, job was not successful");
            Assert.IsTrue(outputFile2.Length > 0, "Job output file is empty, job was not successful");
        }

        [TestMethod()]
        public void RunJobWithSplitAsync()
        {
            RunJob("Job6.xml", "Job6", new MyUnityLoaderJob6Async());
            // Post controls
            FileInfo outputFile = new FileInfo(TestPathOutSorted);
            Assert.IsTrue(outputFile.Exists, "Job output file does not exist, job was not successful");
            Assert.IsTrue(outputFile.Length > 0, "Job output file is empty, job was not successful");
            FileInfo outputFile2 = new FileInfo(TestPathOutUpper);
            Assert.IsTrue(outputFile2.Exists, "Job output file does not exist, job was not successful");
            Assert.IsTrue(outputFile2.Length > 0, "Job output file is empty, job was not successful");
        }

        /// <summary>
        /// Extends UnityLoader and redefines LoadArtifacts() to supply the batch artifacts, and
        /// GetStepLoaders() to supply the step signatures.
        /// </summary>
        private abstract class AbstractUnityLoaderJob6 : UnityLoader
        {

            public override void LoadArtifacts(IUnityContainer unityContainer)
            {
                unityContainer.RegisterType<ITasklet, SortTasklet>("tasklet1", 
                    new InjectionProperty("Separator", "\r\n"),
                    new InjectionProperty("SortCard", "FIELDS=(1,1,CH,A)"),
                    new InjectionProperty("Input", new List<IResource> { new FileSystemResource(TestPathIn) }),
                    new InjectionProperty("Output", new FileSystemResource(TestPathOutSorted)));

                // Step 1
                // Registering reader
                unityContainer.RegisterStepScope<IItemReader<Person>, FlatFileItemReader<Person>>("reader",
                    new InjectionProperty("LineMapper", new DefaultLineMapper<Person>
                    {
                        Tokenizer = new DelimitedLineTokenizer { Delimiter = "," },
                        FieldSetMapper = new PersonMapper()
                    }),
                    new InjectionProperty("Resource", new FileSystemResource(TestPathIn)));

                // Registering Processor
                unityContainer.RegisterStepScope<IItemProcessor<Person, Person>, UppercaseProcessor>("processor");

                // Registering writer
                unityContainer.RegisterStepScope<IItemWriter<Person>, FlatFileItemWriter<Person>>("writer",
                    new InjectionProperty("LineAggregator", new DelimitedLineAggregator<Person>
                    {
                        Delimiter = ",",
                        FieldExtractor = new PropertyFieldExtractor<Person> { Names = new List<string> { "Name", "Firstname" } }
                    }),
                    new InjectionProperty("Resource", new FileSystemResource(TestPathOutUpper)));
            }
        }

        private class MyUnityLoaderJob6Sync : AbstractUnityLoaderJob6
        {
            protected override void LoadConfiguration(IUnityContainer unityContainer)
            {
                base.LoadConfiguration(unityContainer);
                unityContainer.RegisterSingleton<ITaskExecutor, SimpleAsyncTaskExecutor>();
            }
        }

        private class MyUnityLoaderJob6Async : AbstractUnityLoaderJob6
        {
            protected override void LoadConfiguration(IUnityContainer unityContainer)
            {
                base.LoadConfiguration(unityContainer);
                unityContainer.RegisterSingleton<ITaskExecutor, SyncTaskExecutor>();
            }
        }
    }
}