using NLog;
using Summer.Batch.Common.Factory;
using Summer.Batch.Common.IO;
using Summer.Batch.Common.Util;
using Summer.Batch.Core;
using Summer.Batch.Core.Scope.Context;
using Summer.Batch.Extra.Sort.Format;
using Summer.Batch.Extra.Sort.Legacy;
using Summer.Batch.Extra.Sort.Legacy.Parser;
using Summer.Batch.Extra.Sort.Sum;
using Summer.Batch.Infrastructure.Repeat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summer.Batch.Extra.Sort
{
    public class CustomSortTasklet : SortTasklet, IInitializationPostOperations
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public IList<OutputFile> outputFile { get; set; }


        /// <summary>
        /// Default constructor.
        /// </summary>
        public CustomSortTasklet()
        {
            Encoding = Encoding.Default;
        }

        /// <summary>
        /// Configures a <see cref="Sorter{T}"/> and executes it.
        /// </summary>
        /// <param name="contribution">ignored</param>
        /// <param name="chunkContext">ignored</param>
        /// <returns><see cref="RepeatStatus.Finished"/></returns>
        public override RepeatStatus Execute(StepContribution contribution, ChunkContext chunkContext)
        {
            Logger.Info("Starting CustomSort tasklet.");
            SplitSorter<byte[]> sorter = (SplitSorter<byte[]>)BuildSorter();

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            sorter.Sort(Input.Select(r => r.GetFileInfo()).ToList(), Output.GetFileInfo());
            stopwatch.Stop();
            Logger.Info("Total sort time: {0:F2}s", stopwatch.ElapsedMilliseconds / 1000d);

            contribution.ExitStatus = ExitStatus.Completed;
            return RepeatStatus.Finished;
        }

        /// <summary>
        /// Builds the sorter.
        /// </summary>
        /// <returns>a <see cref="Sorter{T}"/></returns>
        public SplitSorter<byte[]> BuildSorter()
        {
            Logger.Debug("Building sorter for CustomSort");
            var sorter = new SplitSorter<byte[]>();

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
           
            if (SkipDuplicates)
            {
                sorter.Sum = new SkipSum<byte[]>();
            }
            if (!string.IsNullOrWhiteSpace(Sum))
            {
                var sumParser = new SumParser { Encoding = Encoding };
                sorter.Sum = sumParser.GetSum(Sum);
            }
            var formatterParser = new FormatterParser { Encoding = Encoding };
            if (!string.IsNullOrWhiteSpace(Inrec))
            {
                sorter.InputFormatter = formatterParser.GetFormatter(Inrec);
            }
            if (!string.IsNullOrWhiteSpace(Outrec))
            {
                sorter.OutputFormatter = formatterParser.GetFormatter(Outrec);
            }
            if (!string.IsNullOrWhiteSpace(Include) || !string.IsNullOrWhiteSpace(Omit))
            {
                var filterParser = new FilterParser { Encoding = Encoding, SortEncoding = SortEncoding };
                sorter.Filter = filterParser.GetFilter(Include, Omit);
            }

            sorter._outputWriters = new List<OutputFileFormat<byte[]>>();
            int count = 0;
            foreach (var file in outputFile)
            {
                Logger.Debug("Building sorter - fileformat " + ++count);
                OutputFileFormat<byte[]> writer = new OutputFileFormat<byte[]>();
                if (!string.IsNullOrWhiteSpace(file.Outrec))
                {
                    Logger.Debug("Building sorter - fileformat outrec = " + file.Outrec);
                    writer.OutputFormatter = formatterParser.GetFormatter(file.Outrec);
                }

                if (!string.IsNullOrWhiteSpace(file.Include) || !string.IsNullOrWhiteSpace(file.Omit))
                {
                    Logger.Debug("Building sorter - fileformat Include = " + file.Include);
                    var filterParser = new FilterParser { Encoding = Encoding, SortEncoding = SortEncoding };
                    writer.Filter = filterParser.GetFilter(file.Include, file.Omit);
                }
                sorter._outputWriters.Add(writer);
            }

            if (MaxInMemorySize > 0)
            {
                sorter.MaxInMemorySize = MaxInMemorySize;
            }

            sorter.HeaderSize = HeaderSize;

            return sorter;
        }

        


    }

    public class OutputFile
    {
        /// <summary>
        /// The outrec configuration card
        /// </summary>
        public string Outrec { get; set; }

        /// <summary>
        /// The include configuration card
        /// </summary>
        public string Include { get; set; }

        /// <summary>
        /// The omit configuration card
        /// </summary>
        public string Omit { get; set; }
    }
}
