using NLog;
using Summer.Batch.Common.Collections;
using Summer.Batch.Extra.Sort.Filter;
using Summer.Batch.Extra.Sort.Format;
using Summer.Batch.Extra.Sort.Legacy.Accessor;
using Summer.Batch.Extra.Sort.Legacy.Format;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Summer.Batch.Extra.Sort
{
    public class SplitSorter<T> : Sorter<T> where T : class
    {
        private const int MbFactor = 1024 * 1024;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private IEnumerable<T> _header;

        public IList<OutputFileFormat<T>> _outputWriters { get; set; }


        public static string newLine = "";
        /// <summary>
        /// Sorts files. If the files are too big to be sorted in memory, they are sorted by blocks
        /// saved on disk in temporary files, which are then merged to produce the final output file.
        /// </summary>
        /// <param name="inputFiles">the files to sort</param>
        /// <param name="outputFile">the output file</param>
        public void Sort(ICollection<FileInfo> inputFiles, FileInfo outputFile)
        {
            if (Comparer == null)
            {
                // No comparer, records are treated one by one and do not need to be kept in memory
                Copy(inputFiles, outputFile);
            }
            else
            {
                // compute the size of the input files, in MB
                var inputSize = inputFiles.Sum(f => f.Length) / MbFactor;
                if (inputSize > MaxInMemorySize)
                {
                    // if the input is too big, do an external sort
                    ExternalSort(inputFiles, outputFile);
                }
                else
                {
                    // otherwise sort in memory
                    InMemorySort(inputFiles, outputFile);
                }
            }
        }

        /// <summary>
        /// Copies records. This method is used when there is no comparer.
        /// Records are still filtered and formatted as required.
        /// </summary>
        /// <param name="inputFiles">the files to copy</param>
        /// <param name="outputFile">the output file</param>
        public void Copy(IEnumerable<FileInfo> inputFiles, FileInfo outputFile)
        {
            _logger.Info("No comparer: copying records to output using filter and formatter");

            int count = 0;
            foreach (var fileFormat in _outputWriters)
            {
                count++;
                using (SumWriter<T> writer = GetSumWriter(outputFile, fileFormat, count))
                {

                    foreach (var file in inputFiles)
                    {
                        using (var reader = RecordAccessorFactory.CreateReader(file.OpenRead()))
                        {

                            _logger.Debug("Writing header");
                            writer.WriteHeader(reader.ReadHeader(HeaderSize));
                            var record = ReadRecord(reader);

                            WriteReportHeader(fileFormat, writer);
                            string prev = "";
                            decimal noOfRecords = 0;
                            fileFormat.countForTrailer1 = 0; fileFormat.countForTrailer2 = 0; fileFormat.countForTrailer3 = 0; 

                            while (record != null)
                            {
                                if (Select(record) && Select(record, fileFormat.Filter))
                                {
                                    prev = WriteSection(fileFormat, writer, prev, record);
                                    writePageHeaderTrailer(fileFormat, writer, noOfRecords);
                                    writer.Write(record);
                                    noOfRecords++;
                                    fileFormat.countForTrailer1++; fileFormat.countForTrailer2++; fileFormat.countForTrailer3++;
                                }
                                record = ReadRecord(reader);
                            }
                            WriteReportTrailer(fileFormat, writer);
                            writer.Dispose();
                        }
                    }
                }
            }

        }

        #region External sort

        /// <summary>
        /// Performs the sort using the external merge sort algorithm.
        /// </summary>
        /// <param name="inputFiles">the files to sort</param>
        /// <param name="outputFile">the output file</param>
        public void ExternalSort(IEnumerable<FileInfo> inputFiles, FileInfo outputFile)
        {
            _logger.Info("Input files too big for memory sort, performing an external merge sort");
           


            long maxRecordsInMemory = -1;
            int count = 0;
            foreach (var fileFormat in _outputWriters)
            {
                count++;
                using (SumWriter<T> writer = GetSumWriter(outputFile, fileFormat, count))
                {
                    var tasks = new List<Task<string>>();


                    var records = new List<T>();
                    foreach (var file in inputFiles)
                    {
                        using (var reader = RecordAccessorFactory.CreateReader(file.OpenRead()))
                        {
                            _header = reader.ReadHeader(HeaderSize);
                            var record = ReadRecord(reader);
                            while (record != null)
                            {
                                if (maxRecordsInMemory == -1)
                                {
                                    // computes the max number of records in memory
                                    // using the first read record
                                    maxRecordsInMemory = ComputeMaxInMemoryRecords(record);
                                }
                                if (records.Count >= maxRecordsInMemory)
                                {
                                    // If we reached the limit, sort the current records
                                    // and save them in a temporary file
                                    tasks.Add(base.SortAndSave(records));
                                }
                                if (Select(record) && Select(record, fileFormat.Filter))
                                {
                                    records.Add(record);
                                }
                                record = ReadRecord(reader);
                            }
                        }
                    }
                    // Sort and save current records
                    tasks.Add(base.SortAndSave(records));
                    var tmpFiles = tasks.Select(task => task.Result).ToList();

                    // Merge
                    Merge(tmpFiles, writer, fileFormat);
                }
            }

        }

        #endregion
        /// <summary>
        /// Sorts the input files in memory.
        /// </summary>
        /// <param name="inputFiles">the files to sort</param>
        /// <param name="outputFile">the output file</param>
        public new void InMemorySort(IEnumerable<FileInfo> inputFiles, FileInfo outputFile)
        {
            _logger.Info("Sorting in memory from CustomSortTasklet");
            
            int count = 0;
            foreach (var fileFormat in _outputWriters)
            {
                
                count++;
                _logger.Info("Sorting in memory for sorter - " + count);
                using (SumWriter<T> writer = GetSumWriter(outputFile, fileFormat, count))
                {
                    // Read all the records
                    var records = new List<T>();
                    foreach (var file in inputFiles)
                    {
                        using (var reader = RecordAccessorFactory.CreateReader(file.OpenRead()))
                        {
                            _header = reader.ReadHeader(HeaderSize);
                            var record = ReadRecord(reader);
                            while (record != null)
                            {
                                if (Select(record) && Select(record, fileFormat.Filter))
                                {
                                    records.Add(record);
                                }
                                record = ReadRecord(reader);
                            }
                        }
                    }
                    // Sort the records
                    records.Sort(Comparer);
                    // Write the records
                    writer.WriteHeader(_header);
                    WriteReportHeader(fileFormat, writer);
                    string prev = "";
                    decimal noOfRecords = 0;
                    fileFormat.countForTrailer1 = 0; fileFormat.countForTrailer2 = 0; fileFormat.countForTrailer3 = 0; 
                    foreach (var record in records)
                    {
                        prev = WriteSection(fileFormat, writer, prev, record);
                        writePageHeaderTrailer(fileFormat, writer, noOfRecords);
                        writer.Write(record);
                        noOfRecords++;
                        fileFormat.countForTrailer1++; fileFormat.countForTrailer2++; fileFormat.countForTrailer3++;
                    }
                    WriteReportTrailer(fileFormat, writer);
                    writer.Dispose();
                    _logger.Info("Writing file=== for sorter - " + count);
                }
            }
        }

        private static void WriteReportTrailer(OutputFileFormat<T> fileFormat, SumWriter<T> writer)
        {
            if (!string.IsNullOrWhiteSpace(fileFormat.trailer1))
            {
                writer.Write(fileFormat.trailer1.Replace("&COUNT", fileFormat.countForTrailer1.ToString()));
            }
        }

        private static void WriteReportHeader(OutputFileFormat<T> fileFormat, SumWriter<T> writer)
        {
            if (!string.IsNullOrWhiteSpace(fileFormat.header1))
            {
                writer.Write(fileFormat.header1.Replace("&DATE", DateTime.Now.ToString("MM'/'dd'/'yyyy")));
            }
        }

        private static void writePageHeaderTrailer(OutputFileFormat<T> fileFormat, SumWriter<T> writer, decimal noOfRecords)
        {
            if (noOfRecords % fileFormat.lines == 0)
            {
                if (!string.IsNullOrWhiteSpace(fileFormat.trailer2))
                {
                    writer.Write(fileFormat.trailer2.Replace("&COUNT", fileFormat.countForTrailer2.ToString()));
                }
                if (!string.IsNullOrWhiteSpace(fileFormat.header2))
                {
                    writer.Write(fileFormat.header2.Replace("&PAGE", ((noOfRecords / fileFormat.lines)+1).ToString()).Replace("&DATE", DateTime.Now.ToString("MM'/'dd'/'yyyy")));
                }
                fileFormat.countForTrailer2 = 0;
            }
        }

        private string WriteSection(OutputFileFormat<T> fileFormat, SumWriter<T> writer, string prev, T record)
        {
            if (null != fileFormat.section)
            {
                prev = (prev == "") ? fileFormat.section.Get(record) : prev;
                string current = fileFormat.section.Get(record);
                if (//!string.IsNullOrWhiteSpace(prev) && 
                    !prev.Equals(current))
                {
                    writeSection(writer, fileFormat);
                    prev = current;
                }
                fileFormat.countForTrailer3 = 0;
            }
            return prev;
        }

        private void writeSection(SumWriter<T> writer, OutputFileFormat<T> outfileFormat)
        {
            writeFooter(writer, outfileFormat);
            writeSkipLines(writer, outfileFormat.section);
            writeHeader(writer, outfileFormat.section);
        }

        private static void writeSkipLines(SumWriter<T> writer, ISection<string> section)
        {
            for (int i = 0; i < section.skipLines; i++)
            {
                writer.Write(newLine);
            }
        }

        private static void writeFooter(SumWriter<T> writer, OutputFileFormat<T> outfileFormat)
        {
            if (!string.IsNullOrWhiteSpace(outfileFormat.section.trailer3))
            {
                writer.Write(outfileFormat.section.trailer3.Replace("&COUNT",outfileFormat.countForTrailer3.ToString()));
            }
        }

        private static void writeHeader(SumWriter<T> writer, ISection<string> section)
        {
            if (!string.IsNullOrWhiteSpace(section.header3))
            {
                writer.Write(section.header3);
            }
        }


        public SumWriter<T> GetSumWriter(FileInfo file, OutputFileFormat<T> fileFormat, int no)
        {
            file = new FileInfo(file.FullName + no);
            file.Directory.Create();

            return new SumWriter<T>(RecordAccessorFactory.CreateWriter(file.Create()), Sum, Comparer, fileFormat.OutputFormatter);
        }

        public bool Select(T record, IFilter<T> SelectFilter)
        {
            return SelectFilter == null || SelectFilter.Select(record);
        }


        /// <summary>
        /// Merges the temporary files to the final output file
        /// </summary>
        /// <param name="tmpFiles"></param>
        /// <param name="outputFile"></param>
        public void Merge(ICollection<string> tmpFiles, SumWriter<T> writer, OutputFileFormat<T> fileFormat)
        {
            _logger.Info("Merging temporary files");
            // we use a list of buffers to sort them by
            // their current record
            var buffers =
                tmpFiles.Select(
                    f => new RecordReaderBuffer<T>(RecordAccessorFactory.CreateReader(new FileStream(f, FileMode.Open)), Comparer))
                    .ToList();



            writer.WriteHeader(_header);
            WriteReportHeader(fileFormat, writer);
            try
            {
                Merge(buffers, writer, fileFormat);
                WriteReportTrailer(fileFormat, writer);
                
            }
            finally
            {
                // Close the buffers
                foreach (var buffer in buffers)
                {
                    buffer.Dispose();
                }
            }


            // Delete temp files
            _logger.Info("Deleting temporary files");
            foreach (var file in tmpFiles)
            {
                File.Delete(file);
            }
        }

        private void Merge(List<RecordReaderBuffer<T>> buffers, SumWriter<T> writer, OutputFileFormat<T> fileFormat)
        {
            // Buffers are stored in a priority queue to have the buffers with the
            // lowest record (with respect to Comparer) as the first buffer
            var queue = new PriorityQueue<RecordReaderBuffer<T>>(buffers);
            string prev = "";
            decimal noOfRecords = 0;
            fileFormat.countForTrailer1 = 0; fileFormat.countForTrailer2 = 0; fileFormat.countForTrailer3 = 0; 
            while (queue.Count > 0)
            {
                var buffer = queue.Poll();
                var record = buffer.Read();
                prev = WriteSection(fileFormat, writer, prev, record);
                writePageHeaderTrailer(fileFormat, writer, noOfRecords);
                writer.Write(record);
                noOfRecords++;
                fileFormat.countForTrailer1++; fileFormat.countForTrailer2++; fileFormat.countForTrailer3++;
                // If the buffer has still records, we put it back in the queue
                // so that is is correctly sorted
                if (buffer.HasNext())
                {
                    queue.Add(buffer);
                }
            }
        }
    }


    public class OutputFileFormat<T>
    {
        public IFormatter<T> OutputFormatter { get; set; }

        public IFilter<T> Filter { get; set; }

        public string header1 { get; set; }

        public string header2 { get; set; }

        public ISection<string> section { get; set; }

        public string trailer1 { get; set; }

        public string trailer2 { get; set; }

        public decimal lines { get; set; }

        public decimal countForTrailer1 { get; set; }

        public decimal countForTrailer2 { get; set; }

        public decimal countForTrailer3 { get; set; }

    }

    public class ISection<T>
    {
        public IAccessor<string> accessor { get; set; }

        public int skipLines { get; set; }

        public string header3 { get; set; }

        public string trailer3 { get; set; }

        public string Get(object record)
        {
            return accessor.Get((byte[]) record);
        }
    }
}