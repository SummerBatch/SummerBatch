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
using NLog;
using Summer.Batch.Common.Extensions;
using Summer.Batch.Extra.Copybook;
using Summer.Batch.Extra.Ebcdic.Encode;
using Summer.Batch.Extra.Ebcdic.Exception;
using Summer.Batch.Common.Util;

namespace Summer.Batch.Extra.Ebcdic
{
    /// <summary>
    ///  An EbcdicWriter writes records to an output stream, according to a copybook.
    /// Please note that this class is "Stream agnostic"
    /// </summary>
    public class EbcdicWriter
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private const int RdwSize = 4;

        #region Attributes
        private readonly EbcdicEncoder _encoder;
        
        /// <summary>
        /// RecordFormatMap property.
        /// </summary>
        public RecordFormatMap RecordFormatMap { private get; set; }
        private readonly bool _writeRdw;
        private readonly Stream _stream;
        
        #endregion

        /// <summary>
        /// Constructs a new EbcdicWriter
        /// </summary>
        /// <param name="outputStream">the stream to read from</param>
        /// <param name="charset">encoding to use for writing</param>
        /// <param name="writeRdw"> whether a record descriptor word (RDW) should be written with each record</param>
        /// <param name="filler"></param>
        public EbcdicWriter(Stream outputStream, string charset, bool writeRdw, EbcdicEncoder.DefaultValue filler)
        {
            _stream = outputStream;
        
            _encoder = new EbcdicEncoder(charset) { DefVal = filler };
            _writeRdw = writeRdw;
        }

        #region public methods
        /// <summary>
        /// Writes a record to the output.
        /// </summary>
        /// <param name="fields">the list of field values to encode. If there are multiple
        /// record formats, the first item is expected to be the
        /// discriminator value.</param>
        /// <exception cref="IOException">if there are issues writing the data</exception>
        /// <exception cref="EbcdicException">if field values do not conform to the copybook</exception>
        public void WriteRecord(List<object> fields)
        {
            RecordFormat format;
            if (RecordFormatMap.MultipleRecordFormats)
            {
                string discriminatorValue = fields[0].ToString();
                fields.RemoveAt(0);
                format = RecordFormatMap.GetFromId(discriminatorValue);
            }
            else
            {
                format = RecordFormatMap.Default;
            }
            WriteRdw(fields, format);
            WriteFields(fields, format);

            if (Logger.IsTraceEnabled)
            {
                //NOTE : WARNING - THIS MIGHT EXPOSE SENSITIVE INFORMATION TO THE VIEW -- 
                //PLEASE PROCEED WITH CAUTION WHEN SETTING LOG LEVEL TO DEBUG.
                Logger.Trace("Wrote record ( | is field sep., [] are collection delimiters ) :\n {0}", 
                    ObjectUtils.Dump(fields));
            }
        }

        /// <summary>
        ///  Writes a record using the specified record format.
        /// </summary>
        /// <param name="recordKey">the key corresponding to the record format</param>
        /// <param name="fields">the list of field values to encode</param>
        /// <exception cref="IOException">if there are issues writing the data</exception>
        /// <exception cref="EbcdicException">if field values do not conform to the copybook</exception>
        public void WriteRecord(string recordKey, List<object> fields)
        {
            RecordFormat format = RecordFormatMap.GetFromId(recordKey);
            WriteRdw(fields, format);
            WriteFields(fields, format);
        }
        #endregion

        #region private methods
        /// <summary>
        /// Write field groups
        /// </summary>
        /// <param name="field"></param>
        /// <param name="fieldsGroup"></param>
        /// <exception cref="IOException"></exception>
        /// <exception cref="EbcdicException"></exception>
        private void WriteFieldsGroup(object field, FieldsGroup fieldsGroup)
        {
            if (fieldsGroup.HasDependencies() || fieldsGroup.Occurs > 1)
            {
                // if occurs is more than one or variable, we have a list of lists
                foreach (object item in (List<object>)field)
                {
                    WriteFields((List<object>)item, fieldsGroup);
                }
            }
            else
            {
                WriteFields((List<object>)field, fieldsGroup);
            }
        }

        /// <summary>
        /// write list of fields
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="fieldsList"></param>
        /// <exception cref="IOException"></exception>
        /// <exception cref="EbcdicException"></exception>
        private void WriteFields(List<object> fields, IFieldsList fieldsList)
        {
            for (int i = 0; i < fields.Count; i++)
            {
                object fieldFormat = fieldsList.Elements[i];
                var format = fieldFormat as FieldFormat;
                if (format != null)
                {
                    WriteField((dynamic)fields[i], format);
                }
                else
                {
                    WriteFieldsGroup(fields[i], (FieldsGroup)fieldFormat);
                }
            }
        }

        /// <summary>
        /// write a field
        /// </summary>
        /// <param name="field"></param>
        /// <param name="fieldFormat"></param>
        /// <exception cref="IOException"></exception>
        /// <exception cref="EbcdicException"></exception>
        private void WriteField(object field, FieldFormat fieldFormat)
        {
            _stream.Write(_encoder.Encode(field, fieldFormat));
            
        }

        /// <summary>
        /// write a fied that is a collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="fieldFormat"></param>
        private void WriteField<T>(List<T> field, FieldFormat fieldFormat)
        {
            foreach (T item in field)
            {
              _stream.Write(_encoder.Encode(item, fieldFormat));            
            }
        }

        /// <summary>
        /// write the read descriptor word
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="fieldsList"></param>
        /// <exception cref="IOException"></exception>
        private void WriteRdw(List<object> fields, IFieldsList fieldsList)
        {
            if (_writeRdw)
            {
                int size = GetFieldsListSize(fields, fieldsList) + RdwSize;
                _stream.Write(new byte[] { (byte)(size >> 8), (byte)(size & 0xFF), 0, 0 });
                
            }
        }

        /// <summary>
        /// compute the field size
        /// </summary>
        /// <param name="field"></param>
        /// <param name="fieldFormat"></param>
        /// <returns></returns>
        private int GetFieldSize(object field, FieldFormat fieldFormat)
        {
            return GetCount((dynamic) field) * fieldFormat.ByteSize;
        }

        private static int GetCount(object obj)
        {
            return 1;
        }

        private static int GetCount<T>(ICollection<T> collection)
        {
            return collection.Count;
        }

        /// <summary>
        /// Compute the fields group size
        /// </summary>
        /// <param name="field"></param>
        /// <param name="fieldsGroup"></param>
        /// <returns></returns>
        private int GetFieldsGroupSize(object field, FieldsGroup fieldsGroup)
        {
            int size = 0;
            if (fieldsGroup.HasDependencies() || fieldsGroup.Occurs > 1)
            {
                //linq converted
                size += ((List<object>)field).Sum(item => GetFieldsListSize((List<object>)item, fieldsGroup));
            }
            else
            {
                size = GetFieldsListSize((List<object>)field, fieldsGroup);
            }
            return size;
        }

        /// <summary>
        /// compute the fields list size
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="fieldsList"></param>
        /// <returns></returns>
        private int GetFieldsListSize(List<object> fields, IFieldsList fieldsList)
        {
            int size = 0;
            for (int i = 0; i < fields.Count; i++)
            {
                object fieldFormat = fieldsList.Elements[i];
                var format = fieldFormat as FieldFormat;
                if (format != null)
                {
                    size += GetFieldSize(fields[i], format);
                }
                else
                {
                    size += GetFieldsGroupSize(fields[i], (FieldsGroup)fieldFormat);
                }
            }
            return size;
        }
        #endregion
    }
}