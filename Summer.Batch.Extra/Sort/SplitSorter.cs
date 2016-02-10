using NLog;
using Summer.Batch.Extra.Sort.Filter;
using Summer.Batch.Extra.Sort.Format;
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
                            while (record != null)
                            {
                                if (Select(record) && Select(record, fileFormat.Filter))
                                {
                                    writer.Write(record);
                                }
                                record = ReadRecord(reader);
                            }
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
                    Merge(tmpFiles, writer);
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
                    foreach (var record in records)
                    {
                        writer.Write(record);
                        
                    }
                    writer.Dispose();
                    _logger.Info("Writing file=== for sorter - " + count);
                }
                 
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
        public void Merge(ICollection<string> tmpFiles, SumWriter<T> writer)
        {
            _logger.Info("Merging temporary files");
            // we use a list of buffers to sort them by
            // their current record
            var buffers =
                tmpFiles.Select(
                    f => new RecordReaderBuffer<T>(RecordAccessorFactory.CreateReader(new FileStream(f, FileMode.Open)), Comparer))
                    .ToList();



            writer.WriteHeader(_header);
            try
            {
                Merge(buffers, writer);
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
    }


    public class OutputFileFormat<T>
    {
        public IFormatter<T> OutputFormatter { get; set; }

        public IFilter<T> Filter { get; set; }
    }
}