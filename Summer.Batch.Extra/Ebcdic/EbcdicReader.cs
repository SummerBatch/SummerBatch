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
using System.Text;
using NLog;
using Summer.Batch.Common.Extensions;
using Summer.Batch.Extra.Copybook;
using Summer.Batch.Extra.Ebcdic.Encode;
using Summer.Batch.Extra.Ebcdic.Exception;
using Summer.Batch.Common.Util;

namespace Summer.Batch.Extra.Ebcdic
{
    /// <summary>
    /// An EbcdicReader reads bytes from an input stream and returns
    /// records, according to a copybook. Each call to NextRecord
    /// returns a list of objects, containing the decoded values of the fields. When
    /// there are no more records to read, it returns null.
    /// Copybooks with multiple records are supported. The reader relies on the
    /// position get/set methods of the input stream to detect
    /// the format of the current record.
    /// </summary>
    public class EbcdicReader
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #region Attributes
        private static readonly int RdwSize = 4;
        private readonly FileFormat _fileFormat;
        private readonly EbcdicDecoder _decoder;
        private readonly RecordFormatMap _recordFormatMap;
        private readonly bool _hasRdw;
        private readonly BufferedStream _stream;
        private int _readRecords;
        #endregion

        /// <summary>
        ///  Constructs an EbcdicReader.
        /// </summary>
        /// <param name="inputStream">the stream to read the data from</param>
        /// <param name="fileFormat">the copybook to use for decoding records</param>
        /// <param name="hasRdw">true if records have a record descriptor word (RDW)</param>
        public EbcdicReader(BufferedStream inputStream, FileFormat fileFormat, bool hasRdw)
        {
            _stream = inputStream;
            _fileFormat = fileFormat;
            _decoder = new EbcdicDecoder(fileFormat.Charset);
            _recordFormatMap = new RecordFormatMap(fileFormat);
            _hasRdw = hasRdw;
        }

        /// <summary>
        ///  Returns the next record as a list of objects.
        /// </summary>
        /// <returns>a list containing the decoded fields of the record. If there are
        /// multiple record formats, the first item in the list is the
        /// discriminator value. Returns null if there are no
        /// more records.</returns>
        /// <exception cref="IOException"></exception>
        /// <exception cref="EbcdicException"></exception>
        public List<object> NextRecord()
        {
            List<object> result = new List<object>();
            if (_readRecords == 0 && _fileFormat.HeaderSize > 0)
            {
                _stream.Seek(_fileFormat.HeaderSize + _fileFormat.NewLineSize, SeekOrigin.Begin);
            }
            if (_hasRdw)
            {
                _stream.Seek(RdwSize, SeekOrigin.Current);
            }
            _readRecords++;
            try
            {
                var recordFormat = RetrieveRecordFormat(result);
                result.AddRange(ReadFields(recordFormat));
            }
            catch (EndOfFileException)
            {
                return null;
            }

            if (_fileFormat.NewLineSize > 0)
            {
                _stream.Seek(_fileFormat.NewLineSize, SeekOrigin.Current);
            }

            if (Logger.IsTraceEnabled)
            {
            //NOTE : WARNING - THIS MIGHT EXPOSE SENSITIVE INFORMATION TO THE VIEW -- 
            //PLEASE PROCEED WITH CAUTION WHEN SETTING LOG LEVEL TO DEBUG.
                Logger.Trace("Reading record #{0} ( | is field sep., [] are collection delimiters ) =\n {1}", _readRecords,
                    _fileFormat.DiscriminatorSize > 0
                        ? ObjectUtils.Dump(result.GetRange(1, result.Count - 1))
                        : ObjectUtils.Dump(result));
            }

            return result;
        }

        #region private methods
        /// <summary>
        /// read a fields list
        /// </summary>
        /// <param name="fieldsList"></param>
        /// <returns></returns>
        /// <exception cref="IOException"></exception>
        /// <exception cref="EbcdicException"></exception>
        private List<object> ReadFields(IFieldsList fieldsList)
        {
            List<object> values = new List<object>();
            IDictionary<string, decimal> readNumericValues = new Dictionary<string, decimal>();
            foreach (CopybookElement fieldFormat in fieldsList.Elements)
            {
                var format = fieldFormat as FieldFormat;
                values.Add(format != null
                    ? ReadField(format, readNumericValues)
                    : ReadFieldsGroup((FieldsGroup) fieldFormat, readNumericValues));
            }
            return values;
        }

        /// <summary>
        /// read a field
        /// </summary>
        /// <param name="fieldFormat"></param>
        /// <param name="readNumericValues"></param>
        /// <returns></returns>
        private object ReadField(FieldFormat fieldFormat, IDictionary<string, decimal> readNumericValues)
        {
            List<object> values = new List<object>();
            int occurs;

            if (fieldFormat.HasDependencies())
            {
                if (readNumericValues.ContainsKey(fieldFormat.DependingOn))
                {
                    occurs = Decimal.ToInt32(readNumericValues[fieldFormat.DependingOn]);
                }
                else
                {
                    throw new System.Exception(
                    string.Format("Check your copybook :[{0}] is not present, but field format says it has dependencies ...",
                        fieldFormat.DependingOn));
                }
            }
            else
            {
                occurs = fieldFormat.Occurs;
            }

            for (int i = 0; i < occurs; i++)
            {
                byte[] bytes = Read(fieldFormat.ByteSize, fieldFormat);
                object value = _decoder.Decode(bytes, fieldFormat);
                values.Add(value);
                if (value is decimal)
                {
                    readNumericValues[fieldFormat.Name] = (decimal)value;
                }
            }

            if (fieldFormat.HasDependencies() || occurs > 1)
            {
                // if occurs is variable or greater than one, return results in a list
                return values;
            }
            else
            {
                // otherwise, just return the single item
                return values[0];
            }
        }

        /// <summary>
        /// read a fields group
        /// </summary>
        /// <param name="fieldsGroup"></param>
        /// <param name="readNumericValues"></param>
        /// <returns></returns>
        /// <exception cref="IOException"></exception>
        /// <exception cref="EbcdicException"></exception>
        private List<object> ReadFieldsGroup(FieldsGroup fieldsGroup, IDictionary<string, decimal> readNumericValues)
        {
            List<object> values = new List<object>();
            int occurs;

            if (fieldsGroup.HasDependencies())
            {
                if (readNumericValues.ContainsKey(fieldsGroup.DependingOn))
                {
                    occurs = Decimal.ToInt32(readNumericValues[fieldsGroup.DependingOn]);
                }
                else
                {
                    throw new System.Exception(
                    string.Format("Check your copybook :[{0}] is not present, but field format says it has dependencies ...",
                    fieldsGroup.DependingOn));
                }
            }
            else
            {
                occurs = fieldsGroup.Occurs;
            }

            for (int i = 0; i < occurs; i++)
            {
                values.Add(ReadFields(fieldsGroup));
            }

            if (fieldsGroup.HasDependencies() || occurs > 1)
            {
                // if occurs is more than one or variable, return results in a list
                return values;
            }
            else
            {
                // otherwise, just return the single item
                return (List<object>)values[0];
            }
        }

        /// <summary>
        /// read the discriminator value
        /// </summary>
        /// <returns></returns>
        /// <exception cref="IOException"></exception>
        /// <exception cref="EbcdicException"></exception>
        private string ReadDiscriminatorValue()
        {
            var position = _stream.Position;//mark
            byte[] bytes = Read(_fileFormat.DiscriminatorSize, null);//read
            _stream.Position = position; //reset
            return Encoding.GetEncoding(_fileFormat.Charset).GetString(bytes);
        }

        /// <summary>
        /// Technical read byte array, given length and field format
        /// </summary>
        /// <param name="length"></param>
        /// <param name="fieldFormat"></param>
        /// <returns></returns>
        /// <exception cref="IOException"></exception>
        /// <exception cref="EbcdicException"></exception>
        private byte[] Read(int length, FieldFormat fieldFormat)
        {
            byte[] bytes = new byte[length];
            int readBytes = _stream.Read(bytes);
            if (readBytes == 0) // note : -1 in java
            {
                throw new EndOfFileException();
            }
            if (readBytes < length)
            {
                throw new FieldParsingException(fieldFormat, bytes);
            }
            return bytes;
        }

        /// <summary>
        /// retrieve record format
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        /// <exception cref="IOException"></exception>
        /// <exception cref="EbcdicException"></exception>
        private RecordFormat RetrieveRecordFormat(ICollection<object> values)
        {
            RecordFormat recordFormat;
            if (_fileFormat.DiscriminatorSize > 0)
            {
                string discriminatorValue = ReadDiscriminatorValue();
                recordFormat = _recordFormatMap.GetFromDiscriminator(discriminatorValue);
                values.Add(recordFormat.DiscriminatorPattern);
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Record format detected:" + recordFormat.CobolRecordName);
                }
            }
            else
            {
                recordFormat = _recordFormatMap.Default;
            }
            return recordFormat;
        }
        #endregion


    }
}