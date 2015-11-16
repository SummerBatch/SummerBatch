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
using System.Text;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Summer.Batch.Core.Unity;
using Summer.Batch.CoreTests.Batch.Flat;
using Summer.Batch.Extra;
using Summer.Batch.Extra.Delegating;
using Summer.Batch.Extra.Template;
using Summer.Batch.Common.IO;
using Summer.Batch.Infrastructure.Item;
using Summer.Batch.Infrastructure.Item.File;
using Summer.Batch.Infrastructure.Item.File.Mapping;
using Summer.Batch.Infrastructure.Item.File.Transform;

namespace Summer.Batch.CoreTests.Delegating
{
    [TestClass]
    public class DelegatingGroupReaderTests : AbstractFlatLaunchTests

    {
        private static readonly string TestPathIn = Path.Combine(TestDataDirectoryIn, "commands.csv");
        private static readonly string TestTemplateIn = Path.Combine(TestDataDirectoryIn, "template.txt");
        private static readonly string TestPathOut = Path.Combine(TestDataDirectoryOut, "report.txt");

        [TestMethod]
        public void DelegatingGroupReaderTestsTestRead()
        {
            //run job is enough since the writer does not serialize anything
            RunJob("JobGroupReader.xml", "CommandJob", new MyUnityLoaderVolatile());
            
        }

        [TestMethod]
        public void DelegatingGroupReaderTestsTestReport()
        {
            RunJob("JobGroupReaderReport.xml", "CommandJob", new MyUnityLoaderReport());
            // Post controls
            FileInfo outputFile = new FileInfo(TestPathOut);
            Assert.IsTrue(outputFile.Exists, "Job output file does not exist, job was not successful");
            Assert.IsTrue(outputFile.Length > 0, "Job output file is empty, job was not successful");
        }

        #region Unity Loader - Test Case #1
        private class MyUnityLoaderVolatile : UnityLoader
        {
            public override void LoadArtifacts(IUnityContainer unityContainer)
            {
                // Registering reader
                unityContainer.RegisterStepScope<IItemReader<CommandItem>, FlatFileItemReader<CommandItem>>("delegate",
                    new InjectionProperty("LineMapper", new DefaultLineMapper<CommandItem>
                    {
                        Tokenizer = new DelimitedLineTokenizer { Delimiter = ";" },
                        FieldSetMapper = new CommandItemMapper()
                    }),
                    new InjectionProperty("Resource", new FileSystemResource(TestPathIn)));

                // Delegating group reader                
                unityContainer.RegisterStepScope<IItemReader<List<CommandItem>>, DelegatingGroupReader<CommandItem>>
                    ("readCsv",
                    new InjectionProperty("RuptureFields", "User"),
                    new InjectionProperty("Delegate", new ResolvedParameter<IItemReader<CommandItem>>("delegate")));

                //Inner writer
                unityContainer.RegisterStepScope<IItemWriter<CommandItem>, VolatileWriter>("delegateWriter");

                //Writer
                unityContainer.RegisterStepScope<IItemWriter<ICollection<CommandItem>>, DelegatingListItemWriter<ICollection<CommandItem>, CommandItem>>(
                    "commandWriter",
                    new InjectionProperty("Delegate", new ResolvedParameter<IItemWriter<CommandItem>>("delegateWriter")));
            }
        } 
        #endregion

        #region Unity Loader - Test Case #2
        private class MyUnityLoaderReport : UnityLoader
        {
            public override void LoadArtifacts(IUnityContainer unityContainer)
            {
                // Context Managers x 2 - Those should come first
                unityContainer.RegisterSingleton<IContextManager, ContextManager>(BatchConstants.JobContextManagerName);
                unityContainer.RegisterSingleton<IContextManager, ContextManager>(BatchConstants.StepContextManagerName);

                // Registering reader
                unityContainer.RegisterStepScope<IItemReader<CommandItem>, FlatFileItemReader<CommandItem>>("delegate",
                    new InjectionProperty("LineMapper", new DefaultLineMapper<CommandItem>
                    {
                        Tokenizer = new DelimitedLineTokenizer { Delimiter = ";" },
                        FieldSetMapper = new CommandItemMapper()
                    }),
                    new InjectionProperty("Resource", new FileSystemResource(TestPathIn)));

                // Delegating group reader                
                unityContainer.RegisterStepScope<IItemReader<List<CommandItem>>, DelegatingGroupReader<CommandItem>>
                    ("readCsv",
                    new InjectionProperty("RuptureFields", "User"),
                    new InjectionProperty("Delegate", new ResolvedParameter<IItemReader<CommandItem>>("delegate")));

                //Processor
                //Note : IContextManager x 2 injected by name through attribute [Dependency]
                //No need for new InjectionProperty here (already handled)
                unityContainer.RegisterStepScope<IItemProcessor<List<CommandItem>, UserTotal>, ReportExecutionListener>
                    ("reportListener");

                //LineAggregator + HeaderWriter + FooterWriter                
                unityContainer.RegisterStepScope<AbstractTemplateLineAggregator<UserTotal>, ReportWriteReportAggregator>
                    ("report/WriteReport/Aggregator",
                    new InjectionProperty("TemplateId", "userTotal"),
                    new InjectionProperty("FooterId", "footer"),
                    new InjectionProperty("HeaderId", "header"),
                    new InjectionProperty("Template", new FileSystemResource(TestTemplateIn)),
                    new InjectionProperty("InputEncoding", Encoding.UTF8)
                    );

                //Inner writer
                unityContainer.RegisterStepScope<IItemWriter<UserTotal>, FlatFileItemWriter<UserTotal>>("reportWriter",
                    new InjectionProperty("Resource", new FileSystemResource(TestPathOut)),
                    new InjectionProperty("Encoding", Encoding.UTF8),
                    new InjectionProperty("LineAggregator",
                        new ResolvedParameter<AbstractTemplateLineAggregator<UserTotal>>("report/WriteReport/Aggregator")),
                    new InjectionProperty("HeaderWriter",
                        new ResolvedParameter<AbstractTemplateLineAggregator<UserTotal>>("report/WriteReport/Aggregator")),
                    new InjectionProperty("FooterWriter",
                        new ResolvedParameter<AbstractTemplateLineAggregator<UserTotal>>("report/WriteReport/Aggregator"))
                    );
            }
        } 
        #endregion

        protected override string[] GetFileNamesIn()
        {
            return new[] { TestPathIn };
        }

        protected override string[] GetFileNamesOut()
        {
            return new[] { TestPathOut };
        }

        
    }
}
