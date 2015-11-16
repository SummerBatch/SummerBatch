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
using Summer.Batch.Core.Partition.Support;
using Summer.Batch.Core.Unity;
using Summer.Batch.Infrastructure.Item;
using Summer.Batch.Infrastructure.Item.File;
using Summer.Batch.Infrastructure.Item.File.Mapping;
using Summer.Batch.Infrastructure.Item.File.Transform;
using Summer.Batch.Common.TaskExecution;

namespace Summer.Batch.CoreTests.Batch.Partitioner
{
    /// <summary>
    /// Tests a partitioned flat file reader with several input files
    /// </summary>
    [TestClass()]
    public class Job10PartitionerLaunchTests : AbstractPartitionerLaunchTests
    {
        private static readonly string TestPathIn1 = Path.Combine(TestDataDirectoryIn, "Job10In1.txt");
        private static readonly string TestPathIn2 = Path.Combine(TestDataDirectoryIn, "Job10In2.txt");
        private static readonly string TestPathOutPrefix = Path.Combine(TestDataDirectoryOut, "Job10Out");
        private static readonly string TestPathOut0 = TestPathOutPrefix + "0.txt";
        private static readonly string TestPathOut1 = TestPathOutPrefix + "1.txt";

        protected override string[] GetFileNamesIn()
        {
            return new[] { TestPathIn1, TestPathIn2 };
        }

        protected override string[] GetFileNamesOut()
        {
            return new[] { TestPathOut0, TestPathOut1 };
        }

        [TestMethod()]
        public void RunJobWithFlatReadWrite()
        {
            RunJob("Job10.xml", "Job10", new MyUnityLoaderJob10());
            // Post controls
            var outputFile0 = new FileInfo(TestPathOut0);
            var outputFile1 = new FileInfo(TestPathOut1);
            Assert.IsTrue(outputFile0.Exists, "Job output file does not exist, job was not successful");
            Assert.IsTrue(outputFile0.Length > 0, "Job output file is empty, job was not successful");
            Assert.IsTrue(outputFile1.Exists, "Job output file does not exist, job was not successful");
            Assert.IsTrue(outputFile1.Length > 0, "Job output file is empty, job was not successful");
        }

        /// <summary>
        /// Extends UnityLoader and redefines LoadArtifacts() to supply the batch artifacts, and
        /// GetStepLoaders() to supply the step signatures.
        /// </summary>
        private class MyUnityLoaderJob10 : UnityLoader
        {

            public override void LoadArtifacts(IUnityContainer unityContainer)
            {
                unityContainer.RegisterSingleton<ITaskExecutor, SimpleAsyncTaskExecutor>();

                // Step 1
                // Registering reader
                unityContainer.StepScopeRegistration<IItemReader<Person>, FlatFileItemReader<Person>>("reader")
                    .Property("LineMapper").Instance(new DefaultLineMapper<Person>
                    {
                        Tokenizer = new DelimitedLineTokenizer { Delimiter = "," },
                        FieldSetMapper = new PersonMapper()
                    })
                    .Property("Resource").Resource("#{stepExecutionContext['fileName']}")
                    .Register();

                // Registering Processor
                unityContainer.RegisterStepScope<IItemProcessor<Person, Person>, UppercaseProcessor>("processor");

                // Registering writer
                unityContainer.StepScopeRegistration<IItemWriter<Person>, FlatFileItemWriter<Person>>("writer")
                    .Property("LineAggregator").Instance(new DelimitedLineAggregator<Person>
                    {
                        Delimiter = ",",
                        FieldExtractor = new PropertyFieldExtractor<Person> { Names = new List<string> { "Name", "Firstname" } }
                    })
                    .Property("Resource").Resource(TestPathOutPrefix + "#{stepExecutionContext['partitionId']}.txt")
                    .Register();

                unityContainer.SingletonRegistration<IPartitioner, MultiResourcePartitioner>("partitioner")
                    .Property("Resources").Resources(TestDataDirectoryIn + "Job10In?.txt")
                    .Register();
            }
        }
    }
}
