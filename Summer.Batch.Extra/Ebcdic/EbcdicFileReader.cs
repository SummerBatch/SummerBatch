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
using NLog;
using Summer.Batch.Extra.Copybook;
using Summer.Batch.Extra.Ebcdic.Exception;
using Summer.Batch.Common.Factory;
using Summer.Batch.Common.IO;
using Summer.Batch.Infrastructure.Item.File;
using Summer.Batch.Infrastructure.Item.Support;
using Summer.Batch.Common.Util;

namespace Summer.Batch.Extra.Ebcdic
{
    /// <summary>
    /// A Summer.Batch reader for EBCDIC files.Given a file and a copybook it
    /// returns the records in the file. An IEbcdicReaderMapper is also
    /// required to match records to actual business objects
    /// </summary>
    /// <typeparam name="T">&nbsp;the type of the business objects to read</typeparam>
    public class EbcdicFileReader<T> : AbstractItemCountingItemStreamItemReader<T>,
        IResourceAwareItemReaderItemStream<T>, IInitializationPostOperations where T : class
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #region Attributes
        /// <summary>
        /// Resource property.
        /// </summary>
        public IResource Resource { private get; set; }

        /// <summary>
        /// Copybook property.
        /// </summary>
        public IResource Copybook { private get; set; }

        /// <summary>
        /// EbcdicReaderMapper property.
        /// </summary>
        public IEbcdicReaderMapper<T> EbcdicReaderMapper { private get; set; }

        /// <summary>
        /// Record descriptor word property.
        /// For file with variable size records, a word is used to give the record size.
        /// This flag indicates whether such a word is present or not.
        /// </summary>
        public bool Rdw { private get; set; }

        /// <summary>
        /// Dispose method which will dispose this stream is implemented in the
        /// ItemStreamSupport abstract class, from which this class inherits (via a complex
        /// inheritance scheme). Sadly, sonar is not smart enough to detect this scheme, and will complain
        /// about this IDisposable not being properly disposed. We'll ignore this false positive issue.
        /// </summary>
        private BufferedStream _inputStream;
        
        private EbcdicReader _reader;
        private int _nbRead;
        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public EbcdicFileReader()
        {
            Name = "EbcdicFileReader";
        }

        /// <summary>
        /// @see AbstractItemCountingItemStreamItemReader#DoRead
        /// </summary>
        /// <returns></returns>
        protected override T DoRead()
        {
            T record = null;
            List<object> fields;
            try
            {
                fields = _reader.NextRecord();
            }
            catch (EbcdicException e)
            {
                throw new EbcdicParseException("Error while parsing item number " + _nbRead, e);
            }
            if (fields != null)
            {
                record = EbcdicReaderMapper.Map(fields, _nbRead);
                if (_logger.IsTraceEnabled)
                {
                    //NOTE : WARNING - THIS MIGHT EXPOSE SENSITIVE INFORMATION TO THE VIEW -- 
                    //PLEASE PROCEED WITH CAUTION WHEN SETTING LOG LEVEL TO DEBUG.
                    _logger.Trace("Read record #{0} from ebcdic file : \n {1}", _nbRead + 1, record);
                }
                _nbRead++;
            }
            return record;
        }

        /// <summary>
        /// @see AbstractItemCountingItemStreamItemReader#DoOpen
        /// </summary>
        protected override void DoOpen()
        {
            Assert.IsTrue(Resource.Exists(), "The input file must exist.");
            _inputStream = new BufferedStream(Resource.GetInputStream());

            FileFormat fileFormat = CopybookLoader.LoadCopybook(Copybook.GetInputStream());
            _reader = new EbcdicReader(_inputStream, fileFormat, Rdw);
            EbcdicReaderMapper.RecordFormatMap = new RecordFormatMap(fileFormat);

            _nbRead = 0;
            Name = string.Concat("EbcdicFileReader.", Resource.GetFilename());
        }

        /// <summary>
        /// @see AbstractItemCountingItemStreamItemReader#DoClose
        /// </summary>
        protected override void DoClose()
        {
            if (_inputStream != Stream.Null)
            {
                _inputStream.Close();
            }
        }


        /// <summary>
        /// @see IInitializationPostOperations#AfterPropertiesSet
        /// </summary>
        public void AfterPropertiesSet()
        {
            Assert.NotNull(Resource, "The input file must be set");
            Assert.NotNull(Copybook, "The copybook must be set");
            Assert.NotNull(EbcdicReaderMapper, "The mapper must be set");
        }
    }
}