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
using Summer.Batch.Infrastructure.Item;
using Summer.Batch.Infrastructure.Item.File;
using Summer.Batch.Infrastructure.Item.File.Mapping;
using Summer.Batch.Infrastructure.Item.File.Transform;

namespace Summer.Batch.CoreTests.Batch.Flat
{
    /// <summary>
    /// Tests a simple flat file reader, processor, writer with a rollbacking error
    /// </summary>
    [TestClass()]
    public class Job2FlatLaunchRollbackTests : AbstractFlatLaunchTests
    {
        private static readonly string TestPathIn = Path.Combine(TestDataDirectoryIn, "Job2In.txt");
        private static readonly string TestPathOut = Path.Combine(TestDataDirectoryOut, "Job2RollbackedOut.txt");

        protected override string[] GetFileNamesIn()
        {
            return new[] { TestPathIn };
        }

        protected override string[] GetFileNamesOut()
        {
            return new[] { TestPathOut };
        }

        [TestMethod()]
        public void RunJobWithFlatReadWriteAndRollback()
        {
            RunJob("Job2.xml", "Job2", new MyUnityLoaderJob2(),true);
            // Post controls
            FileInfo outputFile = new FileInfo(TestPathOut);
            Assert.IsTrue(outputFile.Exists, "Job output file does not exist, job was not successful");
            Assert.IsTrue(outputFile.Length > 0, "Job output file is empty, job was not successful");
        }

        /// <summary>
        /// Extends UnityLoader and redefines LoadArtifacts() to supply the batch artifacts, and
        /// GetStepLoaders() to supply the step signatures.
        /// </summary>
        private class MyUnityLoaderJob2 : UnityLoader
        {

            public override void LoadArtifacts(IUnityContainer unityContainer)
            {
                // Step 1
                // Registering reader
                unityContainer.StepScopeRegistration<IItemReader<Person>, FlatFileItemReader<Person>>("job2Reader")
                    .Property("LineMapper").Instance(new DefaultLineMapper<Person>
                    {
                        Tokenizer = new DelimitedLineTokenizer { Delimiter = "," },
                        FieldSetMapper = new PersonMapper()
                    })
                    .Property("Resource").Resource(TestPathIn)
                    .Register();

                // Registering Processor
                unityContainer.RegisterStepScope<IItemProcessor<Person, Person>, MyFaultyFlatFileProcessor3>("job2Processor");

          

                // Registering writer
                unityContainer.StepScopeRegistration<IItemWriter<Person>, FlatFileItemWriter<Person>>("job2Writer")
                    .Property("LineAggregator").Instance(new DelimitedLineAggregator<Person>
                    {
                        Delimiter = ";",
                        FieldExtractor = new PropertyFieldExtractor<Person> { Names = new List<string> { "Firstname", "Name", "BirthYear" } }
                    })
                    .Property("Resource").Resource(TestPathOut)
                    .Register();
            }
        }
    }
}
