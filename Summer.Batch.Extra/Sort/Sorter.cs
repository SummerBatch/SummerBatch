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
using System.Linq;
using System.Threading.Tasks;
using NLog;
using Summer.Batch.Common.Collections;
using Summer.Batch.Extra.Sort.Filter;
using Summer.Batch.Extra.Sort.Format;
using Summer.Batch.Extra.Sort.Sum;

namespace Summer.Batch.Extra.Sort
{
    /// <summary>
    /// Sorts a collection of files. Files are read as records using a <see cref="IRecordReader{T}"/>.
    /// Records can be filtered using <see cref="Filter"/>. <see cref="InputFormatter"/> (respectively
    /// <see cref="OutputFormatter"/>) allows to format the records before (respectively after) they are
    /// sorted. If <see cref="Comparer"/> is not set, records are copied in the order they are read,
    /// but they are still filtered or formatted.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Sorter<T> where T : class
    {
        // 1MB in bytes
        protected const int MbFactor = 1024 * 1024;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected IEnumerable<T> _header;
        private long _maxInMemorySize = 100 * MbFactor;

        #region Properties

        /// <summary>
        /// The <see cref="IComparer{T}"/> used for sorting the files.
        /// </summary>
        public IComparer<T> Comparer { get; set; }

        /// <summary>
        /// The filter used for selecting the records.
        /// </summary>
        public IFilter<T> Filter { get; set; }

        /// <summary>
        /// Formatter used while reading records.
        /// </summary>
        public IFormatter<T> InputFormatter { get; set; }

        /// <summary>
        /// Formatter used while writing records.
        /// </summary>
        public IFormatter<T> OutputFormatter { get; set; }

        /// <summary>
        /// If not null, used to sum records that are identical according to <see cref="Comparer"/>.
        /// </summary>
        public ISum<T> Sum { get; set; }

        /// <summary>
        /// The size of the header at the begining of the file.
        /// The header is copied as is at the begining of the resulting file.
        /// </summary>
        public int HeaderSize { get; set; }

        /// <summary>
        /// Factory for creating the record readers and writers.
        /// </summary>
        public IRecordAccessorFactory<T> RecordAccessorFactory { get; set; }

        /// <summary>
        /// The maximum size in MB of the input files for the in memory sort.
        /// If the input files are over this limit, external merge sort is used.
        /// Default is 100.
        /// </summary>
        public long MaxInMemorySize
        {
            get { return _maxInMemorySize / MbFactor; }
            set { _maxInMemorySize = value * MbFactor; }
        }

        #endregion

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
        private void Copy(IEnumerable<FileInfo> inputFiles, FileInfo outputFile)
        {
            _logger.Info("No comparer: copying records to output using filter and formatter");
            using (var writer = GetSumWriter(outputFile))
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
                            if (Select(record))
                            {
                                writer.Write(record);
                            }
                            record = ReadRecord(reader);
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
        private void ExternalSort(IEnumerable<FileInfo> inputFiles, FileInfo outputFile)
        {
            _logger.Info("Input files too big for memory sort, performing an external merge sort");
            var tasks = new List<Task<string>>();
            var records = new List<T>();
            long maxRecordsInMemory = -1;

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
                            tasks.Add(SortAndSave(records));
                            records = new List<T>();
                        }
                        if (Select(record))
                        {
                            records.Add(record);
                        }
                        record = ReadRecord(reader);
                    }
                }
            }

            // Sort and save current records
            tasks.Add(SortAndSave(records));

            var tmpFiles = tasks.Select(task => task.Result).ToList();

            // Merge
            Merge(tmpFiles, outputFile);
        }

        /// <summary>
        /// Creates a task that sorts a list of records and saves them in a temporary file.
        /// </summary>
        /// <param name="records">the records to sort</param>
        /// <returns>the task that will perform the sort and return the path to the temporary file</returns>
        protected Task<string> SortAndSave(List<T> records)
        {
            return Task.Run(() =>
            {
                // Sort the records
                records.Sort(Comparer);

                // Save in temporary file
                var tmpFilename = Path.GetTempFileName();
                _logger.Debug("Saving temp file: {0}", tmpFilename);

                using (var writer = RecordAccessorFactory.CreateWriter(new FileStream(tmpFilename, FileMode.Create)))
                {
                    foreach (var record in records)
                    {
                        writer.Write(record);
                    }
                }

                return tmpFilename;
            });
        }

        /// <summary>
        /// Merges the temporary files to the final output file
        /// </summary>
        /// <param name="tmpFiles"></param>
        /// <param name="outputFile"></param>
        private void Merge(ICollection<string> tmpFiles, FileInfo outputFile)
        {
            _logger.Info("Merging temporary files");
            // we use a list of buffers to sort them by
            // their current record
            var buffers =
                tmpFiles.Select(
                    f => new RecordReaderBuffer<T>(RecordAccessorFactory.CreateReader(new FileStream(f, FileMode.Open)), Comparer))
                    .ToList();

            using (var writer = GetSumWriter(outputFile))
            {
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
            }

            // Delete temp files
            _logger.Info("Deleting temporary files");
            foreach (var file in tmpFiles)
            {
                File.Delete(file);
            }
        }

        /// <summary>
        /// Merges the temporary files to the final output file
        /// </summary>
        /// <param name="buffers">the buffers to read the temporary files</param>
        /// <param name="writer">the writer for the output file</param>
        private static void Merge(IEnumerable<RecordReaderBuffer<T>> buffers, IRecordWriter<T> writer)
        {
            // Buffers are stored in a priority queue to have the buffers with the
            // lowest record (with respect to Comparer) as the first buffer
            var queue = new PriorityQueue<RecordReaderBuffer<T>>(buffers);

            while (queue.Count > 0)
            {
                var buffer = queue.Poll();
                var record = buffer.Read();
                writer.Write(record);
                // If the buffer has still records, we put it back in the queue
                // so that is is correctly sorted
                if (buffer.HasNext())
                {
                    queue.Add(buffer);
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
            _logger.Info("Sorting in memory - from SortTasklet");
            var records = new List<T>();

            // Read all the records
            foreach (var file in inputFiles)
            {
                using (var reader = RecordAccessorFactory.CreateReader(file.OpenRead()))
                {
                    _header = reader.ReadHeader(HeaderSize);
                    var record = ReadRecord(reader);
                    while (record != null)
                    {
                        if (Select(record))
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
            using (var writer = GetSumWriter(outputFile))
            {
                writer.WriteHeader(_header);
                foreach (var record in records)
                {
                    writer.Write(record);
                }
            }
        }

        #region Records reading, writing, and selecting

        /// <summary>
        /// Reads a record using the input formatter if required.
        /// </summary>
        /// <param name="reader">the reader to read from</param>
        /// <returns>the read record</returns>
        protected T ReadRecord(IRecordReader<T> reader)
        {
            var record = reader.Read();
            return InputFormatter == null || record == null ? record : InputFormatter.Format(record);
        }

        /// <summary>
        /// Selects a record using <see cref="Filter"/>.
        /// </summary>
        /// <param name="record">the record to check</param>
        /// <returns><code>true</code> if the record has been selected, <code>false</code> otherwise</returns>
        protected bool Select(T record)
        {
            return Filter == null || Filter.Select(record);
        }

        /// <summary>
        /// Returns a record writer that uses <see cref="Sum"/> to sum similar records.
        /// </summary>
        /// <param name="file">the file to write to</param>
        /// <returns>the sum writer</returns>
        protected SumWriter<T> GetSumWriter(FileInfo file)
        {
            file.Directory.Create();
            return new SumWriter<T>(RecordAccessorFactory.CreateWriter(file.Create()), Sum, Comparer, OutputFormatter);
        }

        #endregion

        /// <summary>
        /// Computes the maximum number of records in memory.
        /// The size of the first record is used as the average record size.
        /// </summary>
        /// <param name="record">the first record</param>
        /// <returns>the maximumu number of records to store in memory</returns>
        protected long ComputeMaxInMemoryRecords(T record)
        {
            // We write the record in a memory stream just to check its size,
            // then compare with the max file size.
            // This is a very rough approximation, but it is fast to compute.
            using (var stream = new MemoryStream())
            using (var writer = RecordAccessorFactory.CreateWriter(stream))
            {
                writer.Write(record);
                return _maxInMemorySize / stream.Length;
            }
        }
    }
}