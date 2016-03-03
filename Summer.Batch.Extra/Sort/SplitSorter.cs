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
    /// <summary>
    /// Multiple output sorter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SplitSorter<T> : Sorter<T> where T : class
    {

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public IList<OutputFileFormat<T>> _outputWriters { get; set; }

        /// <summary>
        /// A Dummy variable to write new lines.
        /// </summary>
        private const string NewLine = "";

        private const string CobolCountReference = "&COUNT";

        private const string CobolPageReference = "&PAGE";

        private const string CobolDateReference = "&DATE";



        /// <summary>
        /// Sorts files. If the files are too big to be sorted in memory, they are sorted by blocks
        /// saved on disk in temporary files, which are then merged to produce the final output file.
        /// </summary>
        /// <param name="inputFiles">the files to sort</param>
        /// <param name="outputFile">the output file</param>
        public new void Sort(ICollection<FileInfo> inputFiles, FileInfo outputFile)
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
                            fileFormat.CountForTrailer1 = 0; fileFormat.CountForTrailer2 = 0; fileFormat.Section.CountForTrailer3 = 0;

                            while (record != null)
                            {
                                if (Select(record) && Select(record, fileFormat.Filter))
                                {
                                    prev = WriteSection(fileFormat, writer, prev, record);
                                    WritePageTrailerHeader(fileFormat, writer, noOfRecords);
                                    writer.Write(record);
                                    noOfRecords++;
                                    fileFormat.CountForTrailer1++; fileFormat.CountForTrailer2++; fileFormat.Section.CountForTrailer3++;
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
        private void InMemorySort(IEnumerable<FileInfo> inputFiles, FileInfo outputFile)
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
                    fileFormat.CountForTrailer1 = 0; fileFormat.CountForTrailer2 = 0;
                    if (fileFormat.Section != null)
                    {
                        fileFormat.Section.CountForTrailer3 = 0;
                    }
                   
                    foreach (var record in records)
                    {
                        prev = WriteSection(fileFormat, writer, prev, record);
                        WritePageTrailerHeader(fileFormat, writer, noOfRecords);
                        writer.Write(record);
                        noOfRecords++;
                        fileFormat.CountForTrailer1++; fileFormat.CountForTrailer2++;
                        if (fileFormat.Section != null)
                        {
                            fileFormat.Section.CountForTrailer3++;
                        }
                    }
                    WriteReportTrailer(fileFormat, writer);
                    writer.Dispose();
                    _logger.Info("Writing file=== for sorter - " + count);
                }
            }
        }


        /// <summary>
        /// Writes a report trailer
        /// Replaces Cobol &Count by the number of lines
        /// </summary>
        /// <param name="fileFormat">The output format</param>
        /// <param name="writer">the sum writer</param>
        private static void WriteReportTrailer(OutputFileFormat<T> fileFormat, SumWriter<T> writer)
        {
            if (!string.IsNullOrWhiteSpace(fileFormat.Trailer1))
            {
                writer.Write(fileFormat.Trailer1.Replace(CobolCountReference, fileFormat.CountForTrailer1.ToString()));
            }
        }

        /// <summary>
        /// Writes a report header
        /// </summary>
        /// <param name="fileFormat">the output format</param>
        /// <param name="writer">the sum writer</param>
        private static void WriteReportHeader(OutputFileFormat<T> fileFormat, SumWriter<T> writer)
        {
            if (!string.IsNullOrWhiteSpace(fileFormat.Header1))
            {
                writer.Write(fileFormat.Header1.Replace(CobolDateReference, DateTime.Now.ToString("MM'/'dd'/'yyyy")));
            }
        }

        /// <summary>
        /// Writes Page Trailer and next page Header
        /// </summary>
        /// <param name="fileFormat">the output format</param>
        /// <param name="writer">the sum writer</param>
        /// <param name="noOfRecords">page current number of record</param>
        private static void WritePageTrailerHeader(OutputFileFormat<T> fileFormat, SumWriter<T> writer, decimal noOfRecords)
        {
            if (noOfRecords % fileFormat.MaxPageLines == 0)
            {
                if (!string.IsNullOrWhiteSpace(fileFormat.Trailer2))
                {
                    writer.Write(fileFormat.Trailer2.Replace("T", fileFormat.CountForTrailer2.ToString()));
                }
                if (!string.IsNullOrWhiteSpace(fileFormat.Header2))
                {
                    writer.Write(fileFormat.Header2.Replace(CobolCountReference, ((noOfRecords / fileFormat.MaxPageLines) + 1).ToString()).Replace(CobolDateReference, DateTime.Now.ToString("MM'/'dd'/'yyyy")));
                }
                fileFormat.CountForTrailer2 = 0;
            }
        }

        /// <summary>
        /// Writes a section
        /// A new section is created whenever the key changes
        /// </summary>
        /// <param name="fileFormat">the output format</param>
        /// <param name="writer">the sum writer</param>
        /// <param name="previousKey">The last handled key<param>
        /// <param name="record">the record currently being handled</param>
        /// <returns> the new previous key</returns>
        private string WriteSection(OutputFileFormat<T> fileFormat, SumWriter<T> writer, string previousKey, T record)
        {
            if (null != fileFormat.Section)
            {
                previousKey = (previousKey == "") ? fileFormat.Section.Get(record) : previousKey;
                string current = fileFormat.Section.Get(record);
                if (!previousKey.Equals(current))
                {
                    WriteSection(fileFormat, writer);
                    previousKey = current;
                }
                fileFormat.Section.CountForTrailer3 = 0;
            }
            return previousKey;
        }

        /// <summary>
        /// Writes a complete section
        /// </summary>
        /// <param name="outfileFormat">the output format</param>
        /// <param name="writer">the sum writer</param>
        private void WriteSection(OutputFileFormat<T> outfileFormat, SumWriter<T> writer)
        {
            WriteSectionFooter(outfileFormat.Section, writer);
            WriteSkipLines(outfileFormat.Section, writer);
            WriteSectionHeader(outfileFormat.Section, writer);
        }

        /// <summary>
        /// Creates a new line. Used for section Skip Lines parameters
        /// </summary>
        /// <param name="section">The section format</param>
        /// <param name="writer">the sum writer</param>
        private static void WriteSkipLines(ISection<string> section, SumWriter<T> writer)
        {
            for (int i = 0; i < section.SkipLines; i++)
            {
                writer.Write(NewLine);
            }
        }

        /// <summary>
        /// Writes a section footer 
        /// </summary>
        /// <param name="outfileFormat">the section ourput format</param>
        /// <param name="writer">the sum writer</param>
        private static void WriteSectionFooter(ISection<string> section, SumWriter<T> writer)
        {
            if (!string.IsNullOrWhiteSpace(section.Trailer3))
            {
                writer.Write(section.Trailer3.Replace(CobolCountReference, section.CountForTrailer3.ToString()));
            }
        }

        /// <summary>
        /// Writes a section header
        /// </summary>
        /// <param name="section">the section output format</param>
        /// <param name="writer">the sum writer</param>
        private static void WriteSectionHeader(ISection<string> section, SumWriter<T> writer)
        {
            if (!string.IsNullOrWhiteSpace(section.Header3))
            {
                writer.Write(section.Header3);
            }
        }

        /// <summary>
        /// Gets the current splitted sum writer
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileFormat"></param>
        /// <param name="no"></param>
        /// <returns></returns>
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

        /// <summary>
        ///  Merges the temporary files to the final output file
        /// </summary>
        /// <param name="buffers"></param>
        /// <param name="writer"></param>
        /// <param name="fileFormat"></param>
        private void Merge(List<RecordReaderBuffer<T>> buffers, SumWriter<T> writer, OutputFileFormat<T> fileFormat)
        {
            // Buffers are stored in a priority queue to have the buffers with the
            // lowest record (with respect to Comparer) as the first buffer
            var queue = new PriorityQueue<RecordReaderBuffer<T>>(buffers);
            string prev = "";
            decimal noOfRecords = 0;
            fileFormat.CountForTrailer1 = 0; fileFormat.CountForTrailer2 = 0; fileFormat.Section.CountForTrailer3 = 0;
            while (queue.Count > 0)
            {
                var buffer = queue.Poll();
                var record = buffer.Read();
                prev = WriteSection(fileFormat, writer, prev, record);
                WritePageTrailerHeader(fileFormat, writer, noOfRecords);
                writer.Write(record);
                noOfRecords++;
                fileFormat.CountForTrailer1++; fileFormat.CountForTrailer2++; fileFormat.Section.CountForTrailer3++;
                // If the buffer has still records, we put it back in the queue
                // so that is is correctly sorted
                if (buffer.HasNext())
                {
                    queue.Add(buffer);
                }
            }
        }
    }

    /// <summary>
    /// Output file format. Used to hold the different Cobol Style formatting parameters
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OutputFileFormat<T>
    {
        public IFormatter<T> OutputFormatter { get; set; }

        public IFilter<T> Filter { get; set; }

        /// <summary>
        /// Header format for the whole report
        /// </summary>
        public string Header1 { get; set; }


        /// <summary>
        /// Header format for the page
        /// </summary>
        public string Header2 { get; set; }

        public ISection<string> Section { get; set; }

        /// <summary>
        /// Trailer format for the whole report
        /// </summary>
        public string Trailer1 { get; set; }


        /// <summary>
        /// Trailer format for the page
        /// </summary>
        public string Trailer2 { get; set; }
        
        /// <summary>
        /// Max Number of lines per page
        /// </summary>
        public decimal MaxPageLines { get; set; }

        /// <summary>
        /// Current number of records processed report wide
        /// </summary>
        public decimal CountForTrailer1 { get; set; }

        /// <summary>
        /// Current number of records processed page wide
        /// </summary>
        public decimal CountForTrailer2 { get; set; }

    }

    /// <summary>
    /// Section Cobol style format
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ISection<T>
    {
        public IAccessor<string> Accessor { get; set; }

        /// <summary>
        /// Number of lines to be skipped after a section
        /// </summary>
        public int SkipLines { get; set; }

        /// <summary>
        /// Header format for the section
        /// </summary>
        public string Header3 { get; set; }

        /// <summary>
        /// Trailer format for the section
        /// </summary>
        public string Trailer3 { get; set; }
       
        /// <summary>
        /// Number of records processed section wide
        /// </summary>
        public decimal CountForTrailer3 { get; set; }

        public string Get(object record)
        {
            return Accessor.Get((byte[])record);
        }
    }
}