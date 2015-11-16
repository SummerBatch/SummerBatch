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
using System.IO;
using System.Text;
using NLog;
using Summer.Batch.Common.Factory;
using Summer.Batch.Common.IO;
using Summer.Batch.Infrastructure.Item.Support;
using Summer.Batch.Common.Util;

namespace Summer.Batch.Infrastructure.Item.File
{
    /// <summary>
    /// A restartable <see cref="T:Summer.Batch.Item.ItemReader"/> that reads lines from a <see cref="T:Summer.Batch.IO.Resource"/>.
    /// A line is mapped using a <see cref="T:Summer.Batch.Item.File.LineMapper"/>.
    /// </summary>
    /// <typeparam name="T">the type of the entities read from the resource</typeparam>
    public class FlatFileItemReader<T> : AbstractItemCountingItemStreamItemReader<T>, IResourceAwareItemReaderItemStream<T>, IInitializationPostOperations
        where T : class
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private StreamReader _reader;
        private int _lineCount;
        private bool _noInput;

        /// <summary>
        /// Determines if the reader is in strict mode.
        /// In strict mode it will throw an exception if the resource cannot be read.
        /// Default is <code>true</code>.
        /// </summary>
        public bool Strict { private get; set; }

        /// <summary>
        /// The number of lines to skip at the start of the resource.
        /// </summary>
        public int LinesToSkip { private get; set; }

        /// <summary>
        /// The <see cref="T:Summer.Batch.Item.File.LineMapper"/> responsible for mapping lines to entities.
        /// </summary>
        public ILineMapper<T> LineMapper { private get; set; }

        /// <summary>
        /// The encoding of the resource.
        /// Default is <see cref="P:System.Text.Encoding.Default"/>.
        /// </summary>
        public Encoding Encoding { private get; set; }

        /// <summary>
        /// The resource to read from. In strict mode it should exist and be readable.
        /// </summary>
        public IResource Resource { private get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public FlatFileItemReader()
        {
            Name = typeof(FlatFileItemReader<T>).Name;
            Strict = true;
            Encoding = Encoding.Default;
        }

        /// <summary>
        /// @see IInitializationPostOperations#AfterPropertiesSet
        /// </summary>
        public void AfterPropertiesSet()
        {
            Assert.NotNull(LineMapper, "LineMapper is required");
        }

        /// <summary>
        /// Reads the next item from input.
        /// </summary>
        /// <returns>the read item or null if the end of the stream has been reached</returns>
        protected override T DoRead()
        {
            if (_noInput)
            {
                return null;
            }

            var line = ReadLine();

            if (line == null)
            {
                return null;
            }

            try
            {
                return LineMapper.MapLine(line, _lineCount);
            }
            catch (Exception e)
            {

                throw new FlatFileParseException(string.Format("Parsing error at line: {0} in resource=[{1}], input=[{2}]",
                                                               _lineCount, Resource.GetDescription(), line),
                                                 e, line, _lineCount);
            }
        }

        /// <summary>
        /// Opens the stream.
        /// </summary>
        protected override void DoOpen()
        {
            Assert.NotNull(Resource, "Input resource must be set");

            _noInput = true;
            if (!Resource.Exists())
            {
                if (Strict)
                {
                    throw new InvalidOperationException(string.Format("Input resource must exist (reader is in 'strict' mode): {0}", Resource));
                }
                _logger.Warn("Input resource does not exist: " + Resource);
                return;
            }

            try
            {
                _reader = new StreamReader(Resource.GetInputStream(), Encoding);
            }
            catch (IOException e)
            {
                if (Strict)
                {
                    throw new InvalidOperationException(string.Format("Input resource must be readable (reader is in 'strict' mode): {0} ", Resource), e);
                }
                _logger.Warn("Input resource is not readable: {0}", Resource);
                return;
            }

            for (var i = 0; i < LinesToSkip; i++)
            {
                ReadLine();
            }
            _noInput = false;
        }

        /// <summary>
        /// Closes the stream.
        /// </summary>
        protected override void DoClose()
        {
            _lineCount = 0;
            if (_reader != null)
            {
                // Note : see https://msdn.microsoft.com/fr-fr/library/system.io.streamreader.close%28v=vs.110%29.aspx
                // Close calls Dispose
                // hence, call to ItemStreamSupport.Dispose will effectively dispose the StreamReader
                // Sonar is not smart enough to evaluate that and will lift unappropriate issues...
                _reader.Close();
            }
        }

        private string ReadLine()
        {
            if (_reader == null)
            {
                throw new ReaderNotOpenException("Reader must be open before it can be read.");
            }

            string line = null;

            try
            {
                line = _reader.ReadLine();
                if (line == null)
                {
                    return null;
                }
                _lineCount++;
            }
            catch (IOException e)
            {
                _noInput = true;
                throw new NonTransientFlatFileException(string.Format("Unable to read from resource: [{0}]", Resource), e, line, _lineCount);
            }
            return line;
        }

        /// <summary>
        /// Jump to item with given index.
        /// </summary>
        /// <param name="itemIndex"></param>
        protected override void JumpToItem(int itemIndex)
        {
            for (var i = 0; i < itemIndex; i++)
            {
                ReadLine();
            }
        }

        /// <summary>
        /// Implementing IDisposable.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
                _reader.Dispose();
            }
        }


    }
}