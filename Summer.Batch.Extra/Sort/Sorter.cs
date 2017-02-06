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
    /// Records can be filtered using <see cref="Filter"/>. <see cref="InputFormatter"/> allows to
    /// format the records before  they are sorted. If <see cref="Comparer"/> is not set, records are
    /// copied in the order they are read, but they are still filtered or formatted.
    /// </summary>
    /// <typeparam name="T">&nbsp;The type of the records to sort.</typeparam>
    public class Sorter<T> where T : class
    {
        // 1MB in bytes
        private const int MbFactor = 1024 * 1024;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private IEnumerable<T> _header;
        private long _maxInMemorySize = 100 * MbFactor;

        #region Properties

        /// <summary>
        /// The files to sort.
        /// </summary>
        public IList<FileInfo> InputFiles { get; set; }

        /// <summary>
        /// The output files.
        /// </summary>
        public ICollection<IOutputFile<T>> OutputFiles { get; set; }

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
        /// false = unstable sort : the order of the records that have the same sort key value is not guaranteed.
        /// true = stable sort : the order of the records that have the same sort key is presererved from the input files
        /// Note : order is not guaranteed when multi files inputs
        /// </summary>
        public bool StableSort { get; set; }

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
        public void Sort()
        {
            if (Comparer == null)
            {
                // No comparer, records are treated one by one and do not need to be kept in memory
                Copy();
            }
            else
            {
                // compute the size of the input files, in MB
                var inputSize = InputFiles.Sum(f => f.Length) / MbFactor;
                if (inputSize > MaxInMemorySize)
                {
                    // if the input is too big, do an external sort
                    ExternalSort();
                }
                else
                {
                    // otherwise sort in memory
                    InMemorySort();
                }
            }
        }

        /// <summary>
        /// Copies records. This method is used when there is no comparer.
        /// Records are still filtered and formatted as required.
        /// </summary>
        private void Copy()
        {
            _logger.Info("No comparer: copying records to output using filter and formatter");
            var sumWriters = GetSumWriters();
            try
            {
                foreach (var file in InputFiles)
                {
                    using (var reader = RecordAccessorFactory.CreateReader(file.OpenRead()))
                    {
                        _header = reader.ReadHeader(HeaderSize);
                        WriteHeader(sumWriters);
                        var record = ReadRecord(reader);
                        while (record != null)
                        {
                            if (Select(record))
                            {
                                WriteRecord(sumWriters, record);
                            }
                            record = ReadRecord(reader);
                        }
                    }
                }
            }
            finally
            {
                // Dispose all the sum writers
                foreach (var sumWriter in sumWriters)
                {
                    sumWriter.Dispose();
                }
            }
        }

        #region External sort

        /// <summary>
        /// Performs the sort using the external merge sort algorithm.
        /// </summary>
        private void ExternalSort()
        {
            _logger.Info("Input files too big for memory sort, performing an external merge sort");
            var tasks = new List<Task<string>>();
            var records = new List<T>();
            long maxRecordsInMemory = -1;

            foreach (var file in InputFiles)
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
            Merge(tmpFiles);
        }

        /// <summary>
        /// Creates a task that sorts a list of records and saves them in a temporary file.
        /// </summary>
        /// <param name="records">the records to sort</param>
        /// <returns>the task that will perform the sort and return the path to the temporary file</returns>
        private Task<string> SortAndSave(List<T> records)
        {
            return Task.Run(() =>
            {
                // Sort the records
                if (StableSort)
                {
                    var listStableOrdered = records.OrderBy(x => x, Comparer).ToList();
                    records.Clear();
                    records.AddRange(listStableOrdered);
                }
                else
                {
                    records.Sort(Comparer);
                }
                

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
        private void Merge(ICollection<string> tmpFiles)
        {
            _logger.Info("Merging temporary files");
            // we use a list of buffers to sort them by
            // their current record
            //order the files to enable stable sort
            var buffers =
                tmpFiles.Select(
                    (f, i) => new RecordReaderBuffer<T>(RecordAccessorFactory.CreateReader(new FileStream(f, FileMode.Open)), Comparer, StableSort, i))
                    .ToList();

            var sumWriters = GetSumWriters();
            try
            {
                WriteHeader(sumWriters);
                try
                {
                    Merge(buffers, sumWriters);
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
            finally
            {
                // Dispose all the sum writers
                foreach (var sumWriter in sumWriters)
                {
                    sumWriter.Dispose();
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
        /// <param name="sumWriters">the sum writers corresponding to the different output files</param>
        private static void Merge(IEnumerable<RecordReaderBuffer<T>> buffers, ICollection<SumWriter<T>> sumWriters)
        {
            // Buffers are stored in a priority queue to have the buffers with the
            // lowest record (with respect to Comparer) as the first buffer
            var queue = new PriorityQueue<RecordReaderBuffer<T>>(buffers);

            while (queue.Count > 0)
            {
                var buffer = queue.Poll();
                var record = buffer.Read();
                WriteRecord(sumWriters, record);
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
        private void InMemorySort()
        {
            _logger.Info("Sorting in memory");
            var records = new List<T>();

            // Read all the records
            foreach (var file in InputFiles)
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
            if (StableSort)
            {
                var listStableOrdered = records.OrderBy(x => x, Comparer).ToList();
                records.Clear();
                records.AddRange(listStableOrdered);
            }
            else
            {
                records.Sort(Comparer);
            }

            // Write the records
            var sumWriters = GetSumWriters();
            try
            {
                WriteHeader(sumWriters);
                foreach (var record in records)
                {
                    WriteRecord(sumWriters, record);
                }
            }
            finally
            {
                // Dispose all the sum writers
                foreach (var sumWriter in sumWriters)
                {
                    sumWriter.Dispose();
                }
            }
        }

        #region Records reading, writing, and selecting

        /// <summary>
        /// Reads a record using the input formatter if required.
        /// </summary>
        /// <param name="reader">the reader to read from</param>
        /// <returns>the read record</returns>
        private T ReadRecord(IRecordReader<T> reader)
        {
            var record = reader.Read();
            return InputFormatter == null || record == null ? record : InputFormatter.Format(record);
        }

        /// <summary>
        /// Selects a record using <see cref="Filter"/>.
        /// </summary>
        /// <param name="record">the record to check</param>
        /// <returns><c>true</c> if the record has been selected, <c>false</c> otherwise</returns>
        private bool Select(T record)
        {
            return Filter == null || Filter.Select(record);
        }

        /// <summary>
        /// Writes a record to all the given sum writers.
        /// </summary>
        /// <param name="sumWriters">the sum writers to write to</param>
        /// <param name="record">the record to write</param>
        private static void WriteRecord(IEnumerable<SumWriter<T>> sumWriters, T record)
        {
            foreach (var sumWriter in sumWriters)
            {
                sumWriter.Write(record);
            }
        }

        /// <summary>
        /// Writes the header to all the given sum writers.
        /// </summary>
        /// <param name="sumWriters">the sum writers to write to</param>
        private void WriteHeader(IEnumerable<SumWriter<T>> sumWriters)
        {
            _logger.Debug("Writing header");
            foreach (var sumWriter in sumWriters)
            {
                sumWriter.WriteHeader(_header);
            }
        }

        /// <summary>
        /// Creates a sum writer for each output file.
        /// </summary>
        /// <returns>a collection of sum writers corresponding to the different output files</returns>
        private ICollection<SumWriter<T>> GetSumWriters()
        {
            var sumWriters = new List<SumWriter<T>>();
            foreach (var outputFile in OutputFiles)
            {
                var file = outputFile.Output;
                if (file.Directory != null && !file.Directory.Exists)
                {
                    file.Directory.Create();
                }
                outputFile.OpenWriter(RecordAccessorFactory);
                sumWriters.Add(new SumWriter<T>(outputFile, Sum, Comparer));
            }
            return sumWriters;
        }

        #endregion

        /// <summary>
        /// Computes the maximum number of records in memory.
        /// The size of the first record is used as the average record size.
        /// </summary>
        /// <param name="record">the first record</param>
        /// <returns>the maximumu number of records to store in memory</returns>
        private long ComputeMaxInMemoryRecords(T record)
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