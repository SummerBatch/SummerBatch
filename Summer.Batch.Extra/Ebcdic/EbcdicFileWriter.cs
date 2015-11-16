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

using System;
using System.Collections.Generic;
using System.IO;
using NLog;
using Summer.Batch.Common.Factory;
using Summer.Batch.Common.IO;
using Summer.Batch.Common.Util;
using Summer.Batch.Extra.Copybook;
using Summer.Batch.Extra.Ebcdic.Encode;
using Summer.Batch.Extra.Ebcdic.Exception;
using Summer.Batch.Infrastructure.Item;
using Summer.Batch.Infrastructure.Item.File;
using Summer.Batch.Infrastructure.Item.Support;
using Summer.Batch.Infrastructure.Item.Util;
using Summer.Batch.Infrastructure.Support.Transaction;

namespace Summer.Batch.Extra.Ebcdic
{
    /// <summary>
    /// A Spring Batch writer for a EBCDIC files. It writes business object as EBCDIC
    /// records, according to a copybook and a mapper.
    /// </summary>
    /// <typeparam name="T">the type of the business objects to read</typeparam>
    public class EbcdicFileWriter<T> : AbstractItemStreamItemWriter<T>, ICopybookIo
        , IResourceAwareItemWriterItemStream<T>, IInitializationPostOperations where T : class
    {

        private const string RestartDataName = "current.count";
        private const string WrittenStatisticsName = "written";

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #region Attributes

        /// <summary>
        /// Resource property.
        /// </summary>
        public IResource Resource { private get; set; }

        /// <summary>
        /// List of copybooks property.
        /// </summary>
        public IList<IResource> Copybooks { get; set; }

        /// <summary>
        /// EbcdicWriterMapper property.
        /// </summary>
        public EbcdicWriterMapper EbcdicWriterMapper { private get; set; }

        /// <summary>
        /// Indicates that the target resource should be appended if it already exists.
        /// Default is <code>false</code>.
        /// </summary>
        public bool AppendAllowed { private get; set; }

        /// <summary>
        /// Indicates that the target resource should be deleted if it exists. Ignored if <see cref="P:AppendAllowed"/> is <code>true</code>.
        /// Default is <code>false</code>. 
        /// </summary>
        public bool DeleteIfExist { private get; set; }

        /// <summary>
        /// Indicates that the target resource should be deleted if no lines were written.
        /// Default is <code>false</code>.
        /// </summary>
        public bool DeleteIfEmpty { private get; set; }

        /// <summary>
        /// Indicates that the state of the item writer should be savec in the execution context when
        /// <see cref="M:Update"/> is called.
        /// </summary>
        public bool SaveState { private get; set; }

        /// <summary>
        /// Whether to write a Record descriptor word or not.
        /// </summary>
        public bool WriteRdw { private get; set; }

        /// <summary>
        /// Default value for encoder.
        /// </summary>
        public EbcdicEncoder.DefaultValue DefaultValue { private get; set; }

        /// <summary>
        /// Dispose method which will dispose this stream is implemented in the
        /// ItemStreamSupport abstract class, from which this class inherits (via a complex
        /// inheritance scheme). Sadly, sonar is not smart enough to detect this scheme, and will complain
        /// about this IDisposable not being properly disposed. We'll ignore this false positive issue.
        /// </summary>
        private Stream _outputStream;
        private EbcdicWriter _writer;
        private readonly IDictionary<string, RecordFormatMap> _recordFormatMaps = new Dictionary<string, RecordFormatMap>();
        private string _charset;
        private long _lastMarkedByteOffsetPosition;
        private long _recordsWritten;
        private bool _restarted;
        private bool _initialized;
        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public EbcdicFileWriter()
        {
            DeleteIfExist = true;
            SaveState = true;
            Name = "EbcdicFileWriter";
        }

        /// <summary>
        /// @see ItemStream#Open
        /// </summary>
        /// <param name="executionContext"></param>
        public override void Open(ExecutionContext executionContext)
        {

            if (_initialized) { return; }

            var file = Resource.GetFileInfo();

            if (executionContext.ContainsKey(GetExecutionContextKey(RestartDataName)))
            {
                RestoreFrom(executionContext);
            }

            try
            {
                FileUtils.SetUpOutputFile(file, _restarted, AppendAllowed, DeleteIfExist);
                _outputStream = new TransactionAwareFileStream(file.FullName, FileMode.Append);

                // load copybooks
                LoadCopyBooks();

                //set up writer
                _writer = new EbcdicWriter(_outputStream, _charset, WriteRdw, DefaultValue);
                RecordFormatMap recordFormatMap;
                _recordFormatMaps.TryGetValue(Copybooks[0].GetFileInfo().Name, out recordFormatMap);
                _writer.RecordFormatMap = recordFormatMap;
                EbcdicWriterMapper.RecordFormatMap = recordFormatMap;

                if (_restarted)
                {
                    if (file.Length < _lastMarkedByteOffsetPosition)
                    {
                        throw new ItemStreamException("Current file size is maller that size at last commit.");
                    }
                    _outputStream.SetLength(_lastMarkedByteOffsetPosition);
                    _outputStream.Position = _lastMarkedByteOffsetPosition;
                }

                _initialized = true;
            }
            catch (System.Exception e)
            {
                throw new ItemStreamException("Failed to initialize the writer", e);
            }
        }

        /// <summary>
        /// @see ItemStream#Update
        /// </summary>
        /// <param name="executionContext"></param>
        public override void Update(ExecutionContext executionContext)
        {
            if (!_initialized)
            {
                throw new ItemStreamException("ItemStream not open or already closed.");
            }

            Assert.NotNull(executionContext, "Execution context must not be null");

            if (SaveState)
            {
                executionContext.PutLong(GetExecutionContextKey(RestartDataName), GetPosition());
                executionContext.PutLong(GetExecutionContextKey(WrittenStatisticsName), _recordsWritten);
            }
        }

        /// <summary>
        /// @see ItemStream#Close
        /// </summary>
        public override void Close()
        {
            _restarted = false;
            _initialized = false;

            try
            {
                if (_outputStream != null)
                {
                    _outputStream.Close();
                }
            }
            catch (IOException e)
            {
                throw new ItemStreamException("Error while closing item writer", e);
            }

            if (_recordsWritten == 0 && DeleteIfEmpty)
            {
                Resource.GetFileInfo().Delete();
            }
        }

        /// <summary>
        /// Flushes the underlying stream.
        /// </summary>
        public override void Flush()
        {
            _outputStream.Flush();
        }

        /// <summary>
        /// @see AbstractItemStreamItemWriter#Write
        /// </summary>
        /// <param name="items"></param>
        public override void Write(IList<T> items)
        {

            if (!_initialized)
            {
                throw new WriterNotOpenException("The writer must be open before it can be written to.");
            }

            if (_logger.IsDebugEnabled)
            {
                _logger.Debug("Writing to file using ebcdic format with {0} items", items.Count);
            }

            foreach (var item in items)
            {
                if (item != null)
                {
                    if (_logger.IsTraceEnabled)
                    {
                        //NOTE : WARNING - THIS MIGHT EXPOSE SENSITIVE INFORMATION TO THE VIEW -- 
                        //PLEASE PROCEED WITH CAUTION WHEN SETTING LOG LEVEL TO DEBUG.
                        _logger.Trace("Writing to file using ebcdic format given item: \n {0} ", item.ToString());
                    }

                    List<object> fields = EbcdicWriterMapper.Map(item);
                    _writer.WriteRecord(fields);
                }
            }
        }

        /// <summary>
        ///  Changes the current copybook
        /// @see ICopybookIo#ChangeCopyBook
        /// </summary>
        /// <param name="copybook">the simple name (without the extension) of the copybook to use</param>
        public void ChangeCopyBook(string copybook)
        {
            RecordFormatMap recordFormatMap;
            _recordFormatMaps.TryGetValue(copybook, out recordFormatMap);
            if (recordFormatMap == null)
            {
                throw new ArgumentException("Unknown copybook: " + copybook);
            }
            _writer.RecordFormatMap = recordFormatMap;
            EbcdicWriterMapper.RecordFormatMap = recordFormatMap;
        }

        /// <summary>
        /// @see IInitializationPostOperations#AfterPropertiesSet
        /// </summary>
        public void AfterPropertiesSet()
        {
            Assert.NotNull(Resource, "The output file must be set");
            Assert.NotNull(Copybooks, "The copybook must be set");
            Assert.NotNull(EbcdicWriterMapper, "The mapper must be set");
            if (AppendAllowed)
            {
                DeleteIfExist = false;
            }
        }

        /// <summary>
        /// Load the copybooks
        /// </summary>
        /// <exception cref="CopybookParsingException"></exception>
        /// <exception cref="IOException"></exception>
        private void LoadCopyBooks()
        {
            foreach (IResource copybook in Copybooks)
            {
                FileFormat fileFormat = CopybookLoader.LoadCopybook(copybook.GetInputStream());
                _charset = fileFormat.Charset;
                _recordFormatMaps[copybook.GetFileInfo().Name] = new RecordFormatMap(fileFormat);
            }
        }

        /// <summary>
        /// Restore the writer position at the last commit position.
        /// </summary>
        /// <param name="executionContext"></param>
        private void RestoreFrom(ExecutionContext executionContext)
        {
            _lastMarkedByteOffsetPosition = executionContext.GetLong(GetExecutionContextKey(RestartDataName));
            _recordsWritten = executionContext.GetLong(GetExecutionContextKey(WrittenStatisticsName));
            if (DeleteIfEmpty && _recordsWritten == 0)
            {
                _restarted = false;
                _lastMarkedByteOffsetPosition = 0;
            }
            else
            {
                _restarted = true;
            }
        }

        private long GetPosition()
        {
            if (_outputStream == null)
            {
                return 0;
            }
            return _outputStream.Position;
        }
    }
}