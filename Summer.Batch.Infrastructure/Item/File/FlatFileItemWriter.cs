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

// This file has been modified.
// Original copyright notice :

/*
 * Copyright 2006-2013 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NLog;
using Summer.Batch.Common.Factory;
using Summer.Batch.Common.IO;
using Summer.Batch.Infrastructure.Item.Util;
using Summer.Batch.Infrastructure.Item.File.Transform;
using Summer.Batch.Infrastructure.Item.Support;
using Summer.Batch.Infrastructure.Support.Transaction;
using Summer.Batch.Common.Util;

namespace Summer.Batch.Infrastructure.Item.File
{
    /// <summary>
    /// A restartable <see cref="T:Summer.Batch.Item.ItemWriter"/>that writes to a <see cref="T:Summer.Batch.IO.IResource"/>.
    /// A <see cref="T:LineAggregator"/> is used to write lines from items.
    /// </summary>
    /// <typeparam name="T">&nbsp;</typeparam>
    public class FlatFileItemWriter<T> : AbstractItemStreamItemWriter<T>, IResourceAwareItemWriterItemStream<T>, IInitializationPostOperations where T:class
    {
        private const string RestartDataName = "current.count";
        private const string WrittenStatisticsName = "written";
        private const string WriteInProcess = "batch.writeInProcess";
        private const string ProcessWriterPreFix = "WriteInProcess/";
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The <see cref="T:Summer.Batch.IO.IResource"/> to write to.
        /// </summary>
        public IResource Resource { private get; set; }

        /// <summary>
        /// The <see cref="T:LineAggregator"/> that transforms an item into a line.
        /// </summary>
        public ILineAggregator<T> LineAggregator { private get; set; }

        /// <summary>
        /// The <see cref="T:HeaderWriter"/> to use. Can be null if there are no header to write.
        /// </summary>
        public IHeaderWriter HeaderWriter { private get; set; }

        /// <summary>
        /// The <see cref="T:FooterWriter"/> to use. Can be null if there are no footer to write.
        /// </summary>
        public IFooterWriter FooterWriter { private get; set; }

        /// <summary>
        /// The line separator to use.
        /// Default is <see cref="P:System.Environment.NewLine"/>.
        /// </summary>
        public string LineSeparator { private get; set; }

        /// <summary>
        /// Indicates that the target resource should be appended if it already exists.
        /// Default is <c>false</c>.
        /// </summary>
        public bool AppendAllowed { private get; set; }

        /// <summary>
        /// Indicates that the target resource should be deleted if it exists. Ignored if <see cref="P:AppendAllowed"/> is <c>true</c>.
        /// Default is <c>false</c>. 
        /// </summary>
        public bool DeleteIfExists { private get; set; }

        /// <summary>
        /// Indicates that the target resource should be deleted if no lines were written.
        /// Default is <c>false</c>.
        /// </summary>
        public bool DeleteIfEmpty { private get; set; }

        /// <summary>
        /// The encoding to use to write to the resource.
        /// Default is <see cref="P:System.Text.Encoding.Default"/>.
        /// </summary>
        public Encoding Encoding { private get; set; }

        /// <summary>
        /// Indicates that the writer should take part in the active transaction. During a transaction, data are written at commit.
        /// Defaults is <c>true</c>.
        /// </summary>
        public bool Transactional { private get; set; }

        /// <summary>
        /// Indicates that the buffer should be flushed after each write
        /// Default is <c>false</c>.
        /// </summary>
        public bool AutoFlush { private get; set; }

        /// <summary>
        /// Indicates that the state of the item writer should be savec in the execution context when
        /// <see cref="M:Update"/> is called.
        /// </summary>
        public bool SaveState { private get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public FlatFileItemWriter()
        {
            Name = "FlatFileItemWriter";
            LineSeparator = Environment.NewLine;
            Transactional = true;
            SaveState = true;
            DeleteIfExists = true;
            Encoding = Encoding.Default;
        }

        /// <summary>
        /// @see IInitializationPostOperations#AfterPropertiesSet
        /// </summary>
        public void AfterPropertiesSet()
        {
            Assert.NotNull(LineAggregator, "A LineAggregator must be provided.");
            if (AppendAllowed)
            {
                DeleteIfExists = false;
            }
        }

        /// <summary>
        /// Writes items to the target resource.
        /// </summary>
        /// <param name="items">an enumerable containing the items to write</param>
        /// <exception cref="Summer.Batch.Infrastructure.Item.ItemWriterException"></exception>
        public override void Write(IList<T> items)
        {
            if (!_initialized)
            {
                throw new WriterNotOpenException("The writer must be open before it can be written to.");
            }

            _logger.Debug("Writing to flat file");

            var lines = new StringBuilder();
            var lineCount = 0;
            foreach (var item in items)
            {
                lines.Append(LineAggregator.Aggregate(item)).Append(LineSeparator);
                lineCount++;
            }
            Write(lines.ToString());
            _linesWritten += lineCount;
            _logger.Debug("{0} lines written", lineCount);
        }

        /// <summary>
        /// Open the stream for the provided ExecutionContext.
        /// </summary>
        /// <param name="executionContext">current step's ExecutionContext.  Will be the
        /// executionContext from the last run of the step on a restart.</param>
        /// <exception cref="ItemStreamException">&nbsp;</exception>
        /// <exception cref="ArgumentException">&nbsp;if execution context is null</exception>
        public override void Open(ExecutionContext executionContext)
        {
            Assert.NotNull(Resource, "The resource must be set.");
            Assert.IsTrue(!Resource.Exists() || (Resource.GetFileInfo().Attributes & FileAttributes.Directory) == 0,
                "Cannot write write, resource is a directory: " + Resource);

            if (_initialized) { return; }

            if (executionContext.ContainsKey(WriteInProcess) && executionContext.ContainsKey(ProcessWriterPreFix+GetExecutionContextKey(RestartDataName)))
            {
                RestoreWriteInProcessFrom(executionContext);
            }
            if (executionContext.ContainsKey(GetExecutionContextKey(RestartDataName)))
            {
                RestoreFrom(executionContext);
            }
            InitializeWriter();

            if (_lastMarkedByteOffsetPosition == 0 && !_appending && HeaderWriter != null)
            {
                HeaderWriter.WriteHeader(_writer);
                Write(LineSeparator);
            }
        }

        /// <summary>
        /// If any resources are needed for the stream to operate they need to be destroyed here. Once this method has been
        /// called all other methods (except open) may throw an exception.
        /// </summary>
        /// <exception cref="ItemStreamException">&nbsp;</exception>
        public override void Close()
        {
            if (_initialized && FooterWriter != null)
            {
                FooterWriter.WriteFooter(_writer);
                _writer.Flush();
            }
            CloseState();
            if (_linesWritten == 0 && DeleteIfEmpty)
            {
                Resource.GetFileInfo().Delete();
            }
        }

        /// <summary>
        /// Indicates that the execution context provided during open is about to be saved. If any state is remaining, but
        /// has not been put in the context, it should be added here.
        /// </summary>
        /// <param name="executionContext">to be updated</param>
        /// <exception cref="ItemStreamException">&nbsp;</exception>
        /// <exception cref="ArgumentException">&nbsp;if execution context is null</exception>
        public override void Update(ExecutionContext executionContext)
        {
            if (!_initialized)
            {
                throw new ItemStreamException("ItemStream not open or already closed.");
            }

            Assert.NotNull(executionContext, "Execution context must not be null");

            if (executionContext.ContainsKey(WriteInProcess) && (bool)executionContext.Get(WriteInProcess) && SaveState)
            {
                executionContext.PutLong(ProcessWriterPreFix+GetExecutionContextKey(RestartDataName), GetPosition());
                executionContext.PutLong(ProcessWriterPreFix+GetExecutionContextKey(WrittenStatisticsName), _linesWritten);
            }
            else
            {
                executionContext.PutLong(GetExecutionContextKey(RestartDataName), GetPosition());
                executionContext.PutLong(GetExecutionContextKey(WrittenStatisticsName), _linesWritten);
            }
        }

        /// <summary>
        /// Flushes the underlying stream.
        /// </summary>
        public override void Flush()
        {
            if (_initialized)
            {
                _fileStream.Flush();
            }
        }

        #region Output state
        // Fields and methods that pertain to the output state of the writer

        private FileStream _fileStream;

        private TextWriter _writer;

        private long _lastMarkedByteOffsetPosition;

        private long _linesWritten;

        private bool _restarted;

        private bool _initialized;

        private bool _appending;

        private long GetPosition()
        {
            if (_fileStream == null)
            {
                return 0;
            }
            _writer.Flush();
            return _fileStream.Position;            
        }

        private void RestoreFrom(ExecutionContext executionContext)
        {
            _lastMarkedByteOffsetPosition = executionContext.GetLong(GetExecutionContextKey(RestartDataName));
            _linesWritten = executionContext.GetLong(GetExecutionContextKey(WrittenStatisticsName));
            if (DeleteIfEmpty && _linesWritten == 0)
            {
                _restarted = false;
                _lastMarkedByteOffsetPosition = 0;
            }
            else
            {
                _restarted = true;
            }
        }
        private void RestoreWriteInProcessFrom(ExecutionContext executionContext)
        {
            _lastMarkedByteOffsetPosition = executionContext.GetLong(ProcessWriterPreFix+GetExecutionContextKey(RestartDataName));
            _linesWritten = executionContext.GetLong(ProcessWriterPreFix+GetExecutionContextKey(WrittenStatisticsName));
            if (DeleteIfEmpty && _linesWritten == 0)
            {
                _restarted = false;
                _lastMarkedByteOffsetPosition = 0;
            }
            else
            {
                _restarted = true;
            }
        }
        private void InitializeWriter()
        {
            var file = Resource.GetFileInfo();
            FileUtils.SetUpOutputFile(file, _restarted, AppendAllowed, DeleteIfExists);
            //Either use TransactionAwareFileStream or regular FileStream
            _fileStream = Transactional ? new TransactionAwareFileStream(file.FullName,FileMode.Append) : file.Open(FileMode.Append, FileAccess.Write);
            _writer= new StreamWriter(_fileStream, Encoding) { AutoFlush = AutoFlush };

            if (AppendAllowed && file.Length > 0)
            {
                _appending = true;
            }

            if (_restarted)
            {
                if (file.Length < _lastMarkedByteOffsetPosition)
                {
                    throw new ItemStreamException("Current file size is smaller that size at last commit.");
                }
                _fileStream.SetLength(_lastMarkedByteOffsetPosition);
                _fileStream.Position = _lastMarkedByteOffsetPosition;
            }

            _initialized = true;
        }

        private void CloseState()
        {
            _initialized = false;
            _restarted = false;
            if (_writer != null)
            {
                _writer.Close();
            }
        }

        private void Write(string line)
        {
            _writer.Write(line);
            _writer.Flush();
        }

        #endregion

        /// <summary>
        /// Implements IDisposable.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close(); 
                _writer.Dispose();
            }
        }

    }
}