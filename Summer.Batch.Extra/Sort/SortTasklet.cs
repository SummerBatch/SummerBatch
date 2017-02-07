//   Copyright 2015 Blu Age Corporation - Plano, Texas
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NLog;
using Summer.Batch.Common.Factory;
using Summer.Batch.Common.IO;
using Summer.Batch.Common.Util;
using Summer.Batch.Core;
using Summer.Batch.Core.Scope.Context;
using Summer.Batch.Core.Step.Tasklet;
using Summer.Batch.Extra.Sort.Legacy;
using Summer.Batch.Extra.Sort.Legacy.Parser;
using Summer.Batch.Extra.Sort.Sum;
using Summer.Batch.Infrastructure.Repeat;

namespace Summer.Batch.Extra.Sort
{
    /// <summary>
    /// A tasklet that sorts files. 
    /// Uses legacy sort cards. 
    /// </summary>
    public class SortTasklet : ITasklet, IInitializationPostOperations
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The input resources.
        /// </summary>
        public IList<IResource> Input { get; set; }

        /// <summary>
        /// The output resource.
        /// </summary>
        public IList<IResource> Output { get; set; }

        /// <summary>
        /// The encoding of the input files. Default is <see cref="System.Text.Encoding.Default"/>.
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// The encoding to use when sorting, if different from <see cref="Encoding"/>. Default is <c>null</c>.
        /// </summary>
        public Encoding SortEncoding { get; set; }

        /// <summary>
        /// The length of the records for fixed-length block records.
        /// </summary>
        public int RecordLength { get; set; }

        /// <summary>
        /// The separator for separated variable length records.
        /// </summary>
        public string Separator { get; set; }

        /// <summary>
        /// The sort configuration card
        /// </summary>
        public string SortCard { get; set; }

        /// <summary>
        /// The include configuration card
        /// </summary>
        public string Include { get; set; }

        /// <summary>
        /// The omit configuration card
        /// </summary>
        public string Omit { get; set; }

        /// <summary>
        /// The inrec configuration card
        /// </summary>
        public string Inrec { get; set; }

        /// <summary>
        /// The outrec configuration card
        /// </summary>
        public string Outrec { get; set; }

        /// <summary>
        /// The sum configuration card
        /// </summary>
        public string Sum { get; set; }

        /// <summary>
        /// Configuration cards for outfils, separated by semi-colons.
        /// </summary>
        public string Outfils { get; set; }

        /// <summary>
        /// Whether duplicates should be skipped or kept.
        /// </summary>
        public bool SkipDuplicates { get; set; }

        /// <summary>
        /// The size of the header.
        /// </summary>
        public int HeaderSize { get; set; }

        /// <summary>
        /// The maximum size (in MB) of a file for it to be sorted in memory. Default is 100MB.
        /// </summary>
        public long MaxInMemorySize { get; set; }

        /// <summary>
        /// false - default = unstable sort : the order of the records that have the same sort key value is not guaranteed.
        /// true = stable sort : the order of the records that have the same sort key is presererved from the input files
        /// Note : order is not guaranteed when multi files inputs
        /// </summary>
        public bool StableSort { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SortTasklet()
        {
            Encoding = Encoding.Default;
        }

        /// <summary>
        /// Post-initialization operation.
        /// </summary>
        public void AfterPropertiesSet()
        {
            Assert.NotNull(Input, "Input must not be null.");
            Assert.NotEmpty(Input, "Input must not be empty.");
            Assert.NotNull(Output, "Output must not be null.");
        }

        /// <summary>
        /// Configures a <see cref="Sorter{T}"/> and executes it.
        /// </summary>
        /// <param name="contribution">ignored</param>
        /// <param name="chunkContext">ignored</param>
        /// <returns><see cref="RepeatStatus.Finished"/></returns>
        public RepeatStatus Execute(StepContribution contribution, ChunkContext chunkContext)
        {
            Logger.Info("Starting sort tasklet.");
            var sorter = BuildSorter();

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            sorter.Sort();
            stopwatch.Stop();
            Logger.Info("Total sort time: {0:F2}s", stopwatch.ElapsedMilliseconds / 1000d);

            contribution.ExitStatus = ExitStatus.Completed;
            return RepeatStatus.Finished;
        }

        /// <summary>
        /// Builds the sorter.
        /// </summary>
        /// <returns>a <see cref="Sorter{T}"/></returns>
        private Sorter<byte[]> BuildSorter()
        {
            Logger.Debug("Building sorter");
            var sorter = new Sorter<byte[]>();

            sorter.StableSort = StableSort;

            var formatterParser = new FormatterParser { Encoding = Encoding };

            sorter.InputFiles = Input.Select(r => r.GetFileInfo()).ToList();
            if (Output.Count == 1)
            {
                var outputFiles = new List<IOutputFile<byte[]>>();
                var outputFile = new LegacyOutputFile { Output = Output[0].GetFileInfo() };
                if (!string.IsNullOrWhiteSpace(Outrec))
                {
                    outputFile.Formatter = formatterParser.GetFormatter(Outrec);
                }
                outputFiles.Add(outputFile);
                sorter.OutputFiles = outputFiles;
            }
            if (Output.Count > 1)
            {
                var outfilParser = new OutfilParser { Encoding = Encoding, SortEncoding = SortEncoding };
                var outputFiles = outfilParser.GetOutputFiles(Outfils);
                if (Output.Count != outputFiles.Count)
                {
                    throw new SortException("The number of output files must match the number of outfil configurations.");
                }
                for (var i = 0; i < Output.Count; i++)
                {
                    outputFiles[i].Output = Output[i].GetFileInfo();
                }
                sorter.OutputFiles = outputFiles;
            }

            if (RecordLength > 0 || Separator == null)
            {
                sorter.RecordAccessorFactory = new BlockAccessorFactory { RecordLength = RecordLength };
            }
            else
            {
                sorter.RecordAccessorFactory = new SeparatorAccessorFactory { Separator = Encoding.GetBytes(Separator) };
            }

            if (!string.IsNullOrWhiteSpace(SortCard))
            {
                var comparerParser = new ComparerParser { Encoding = Encoding, SortEncoding = SortEncoding };
                sorter.Comparer = comparerParser.GetComparer(SortCard);
            }
            if (!string.IsNullOrWhiteSpace(Include) || !string.IsNullOrWhiteSpace(Omit))
            {
                var filterParser = new FilterParser { Encoding = Encoding, SortEncoding = SortEncoding };
                sorter.Filter = filterParser.GetFilter(Include, Omit);
            }
            if (SkipDuplicates)
            {
                sorter.Sum = new SkipSum<byte[]>();
            }
            if (!string.IsNullOrWhiteSpace(Sum))
            {
                var sumParser = new SumParser { Encoding = Encoding };
                sorter.Sum = sumParser.GetSum(Sum);
            }
            if (!string.IsNullOrWhiteSpace(Inrec))
            {
                sorter.InputFormatter = formatterParser.GetFormatter(Inrec);
            }

            if (MaxInMemorySize > 0)
            {
                sorter.MaxInMemorySize = MaxInMemorySize;
            }

            sorter.HeaderSize = HeaderSize;

            return sorter;
        }
    }
}