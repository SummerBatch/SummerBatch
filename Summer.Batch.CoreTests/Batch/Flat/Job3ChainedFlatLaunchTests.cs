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
using Summer.Batch.Core.Unity;
using Summer.Batch.Common.IO;
using Summer.Batch.Infrastructure.Item;
using Summer.Batch.Infrastructure.Item.File;
using Summer.Batch.Infrastructure.Item.File.Mapping;
using Summer.Batch.Infrastructure.Item.File.Transform;

namespace Summer.Batch.CoreTests.Batch.Flat
{
    /// <summary>
    /// Tests two flat file steps chained with each other
    /// </summary>
    [TestClass()]
    public class Job3ChainedLaunchTests : AbstractFlatLaunchTests
    {
        private static readonly string TestPathIn = Path.Combine(TestDataDirectoryIn, "Job3In.txt");
        private static readonly string TestPathOut = Path.Combine(TestDataDirectoryOut, "Job3Out.txt");
        private static readonly string TestPathInStep2 = Path.Combine(TestDataDirectoryIn, "Job3InStep2.txt");
        private static readonly string TestPathOutStep2 = Path.Combine(TestDataDirectoryOut, "Job3OutStep2.txt");

        protected override string[] GetFileNamesIn()
        {
            return new[] { TestPathIn, TestPathInStep2 };
        }

        protected override string[] GetFileNamesOut()
        {
            return new[] { TestPathOut, TestPathOutStep2 };
        }

        [TestMethod()]
        public void RunJobWithFlatReadWrite()
        {
            RunJob("Job3.xml", "Job3", new MyUnityLoaderJob3());
            // Post controls
            FileInfo outputFile = new FileInfo(TestPathOut);
            Assert.IsTrue(outputFile.Exists, "Job output file does not exist, job was not successful");
            Assert.IsTrue(outputFile.Length > 0, "Job output file is empty, job was not successful");
            FileInfo outputFile2 = new FileInfo(TestPathOutStep2);
            Assert.IsTrue(outputFile2.Exists, "Job output file does not exist, job was not successful");
            Assert.IsTrue(outputFile2.Length > 0, "Job output file is empty, job was not successful");
        }

        [TestMethod()]
        public void RunJobFlowWithFlatReadWrite()
        {
            RunJob("Job3Flow.xml", "Job3", new MyUnityLoaderJob3());
            // Post controls
            FileInfo outputFile = new FileInfo(TestPathOut);
            Assert.IsTrue(outputFile.Exists, "Job output file does not exist, job was not successful");
            Assert.IsTrue(outputFile.Length > 0, "Job output file is empty, job was not successful");
            FileInfo outputFile2 = new FileInfo(TestPathOutStep2);
            Assert.IsTrue(outputFile2.Exists, "Job output file does not exist, job was not successful");
            Assert.IsTrue(outputFile2.Length > 0, "Job output file is empty, job was not successful");
        }

        [TestMethod()]
        public void RunJobFlowStartWithFlatReadWrite()
        {
            RunJob("Job3FlowStart.xml", "Job3", new MyUnityLoaderJob3());
            // Post controls
            FileInfo outputFile = new FileInfo(TestPathOut);
            Assert.IsTrue(outputFile.Exists, "Job output file does not exist, job was not successful");
            Assert.IsTrue(outputFile.Length > 0, "Job output file is empty, job was not successful");
            FileInfo outputFile2 = new FileInfo(TestPathOutStep2);
            Assert.IsTrue(outputFile2.Exists, "Job output file does not exist, job was not successful");
            Assert.IsTrue(outputFile2.Length > 0, "Job output file is empty, job was not successful");
        }

        [TestMethod()]
        public void RunJobFlowEndWithFlatReadWrite()
        {
            RunJob("Job3FlowEnd.xml", "Job3", new MyUnityLoaderJob3());
            // Post controls
            FileInfo outputFile = new FileInfo(TestPathOut);
            Assert.IsTrue(outputFile.Exists, "Job output file does not exist, job was not successful");
            Assert.IsTrue(outputFile.Length > 0, "Job output file is empty, job was not successful");
            FileInfo outputFile2 = new FileInfo(TestPathOutStep2);
            Assert.IsTrue(outputFile2.Exists, "Job output file does not exist, job was not successful");
            Assert.IsTrue(outputFile2.Length > 0, "Job output file is empty, job was not successful");
        }

        /// <summary>
        /// Extends UnityLoader and redefines LoadArtifacts() to supply the batch artifacts, and
        /// GetStepLoaders() to supply the step signatures.
        /// </summary>
        private class MyUnityLoaderJob3 : UnityLoader
        {

            public override void LoadArtifacts(IUnityContainer unityContainer)
            {
                // Step 1
                // Registering reader
                unityContainer.RegisterStepScope<IItemReader<Person>, FlatFileItemReader<Person>>("job3Reader",
                    new InjectionProperty("LineMapper", new DefaultLineMapper<Person>
                        {
                            Tokenizer = new DelimitedLineTokenizer { Delimiter = "," },
                            FieldSetMapper = new PersonMapper()
                        }),
                    new InjectionProperty("Resource", new FileSystemResource(TestPathIn)));

                // Registering Processor
                unityContainer.RegisterStepScope<IItemProcessor<Person, Person>, MyFlatFileProcessor>("job3Processor");

                // Registering writer
                unityContainer.RegisterStepScope<IItemWriter<Person>, FlatFileItemWriter<Person>>("job3Writer",
                    new InjectionProperty("LineAggregator", new DelimitedLineAggregator<Person>
                        {
                            Delimiter = ";",
                            FieldExtractor = new PropertyFieldExtractor<Person> { Names = new List<string> { "Firstname", "Name", "BirthYear" } }
                        }),
                    new InjectionProperty("Resource", new FileSystemResource(TestPathOut)));

                // Step 2
                // Registering reader
                unityContainer.RegisterStepScope<IItemReader<Person2>, FlatFileItemReader<Person2>>("job3ReaderStep2",
                    new InjectionProperty("LineMapper", new DefaultLineMapper<Person2>
                    {
                        Tokenizer = new DelimitedLineTokenizer { Delimiter = "," },
                        FieldSetMapper = new PersonMapper2()
                    }),
                    new InjectionProperty("Resource", new FileSystemResource(TestPathInStep2)));

                // Registering Processor
                unityContainer.RegisterStepScope<IItemProcessor<Person2, Person2>, MyFlatFileProcessor2>("job3ProcessorStep2");

                // Registering writer
                unityContainer.RegisterStepScope<IItemWriter<Person2>, FlatFileItemWriter<Person2>>("job3WriterStep2",
                    new InjectionProperty("LineAggregator", new DelimitedLineAggregator<Person2>
                    {
                        Delimiter = ";",
                        FieldExtractor = new PropertyFieldExtractor<Person2> { Names = new List<string> { "Firstname", "Name", "BirthYear" } }
                    }),
                    new InjectionProperty("Resource", new FileSystemResource(TestPathOutStep2)));
            }
        }
    }
}
