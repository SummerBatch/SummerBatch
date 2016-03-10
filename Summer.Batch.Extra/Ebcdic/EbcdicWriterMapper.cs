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
using System.Linq;
using Summer.Batch.Extra.Copybook;
using Summer.Batch.Common.Factory;
using Summer.Batch.Common.Property;

namespace Summer.Batch.Extra.Ebcdic
{
    /// <summary>
    /// An EbcdicWriterMapper maps an item to a list of objects that can be written
    /// as an EBCDIC record.
    /// It requires a boolean encoder and a date parser to correctly manage booleans
    /// and dates. For each, a default implementation is used if none is provided.
    /// </summary>
    public class EbcdicWriterMapper : AbstractEbcdicMapper, IInitializationPostOperations
    {
        private static readonly List<string> DecimalTypes = new List<string>() { "B", "9", "3" };

        /// <summary>
        /// Date parser property.
        /// </summary>
        public IDateParser DateParser { private get; set; }

        /// <summary>
        /// Boolean encoder property.
        /// </summary>
        public IBooleanEncoder BooleanEncoder { private get; set; }

        /// <summary>
        /// RecordFormatMap property.
        /// </summary>
        public RecordFormatMap RecordFormatMap { private get; set; }

        /// <summary>
        /// @see IInitializationPostOperations#AfterPropertiesSet
        /// </summary>
        public void AfterPropertiesSet()
        {
            if (DateParser == null)
            {
                DateParser = new DateParser();
            }
            if (BooleanEncoder == null)
            {
                BooleanEncoder = new ZeroOneBooleanEncoder();
            }
        }

        /// <summary>
        /// Converts the content of a business object into a list of values.
        /// </summary>
        /// <param name="item">the object to convert</param>
        /// <returns> a list of values that can be written in an EBCDIC record</returns>
        public List<object> Map(object item)
        {
            RecordFormat recordFormat;
            List<object> values = new List<object>();
            if (RecordFormatMap.MultipleRecordFormats)
            {
                recordFormat = RecordFormatMap.GetFromId(((IEbcdicBusinessObject)item).DistinguishedValue);
                values.Add(((IEbcdicBusinessObject) item).DistinguishedValue);
            }
            else
            {
                recordFormat = RecordFormatMap.Default;
            }
            values.AddRange(MapFieldsList(item, recordFormat, new Dictionary<string, decimal>()));
            return values;
        }

        #region private utility methods
        /// <summary>
        /// Map fields list object
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldsList"></param>
        /// <param name="writtenNumbers"></param>
        /// <returns></returns>
        private List<object> MapFieldsList(object item, IFieldsList fieldsList, IDictionary<string, decimal> writtenNumbers)
        {
            List<object> values = new List<object>();
            PropertyAccessor wrapper = new PropertyAccessor(item);
            foreach (CopybookElement element in fieldsList.Elements)
            {
                var format = element as FieldFormat;
                if (format != null)
                {
                    values.Add(Convert(wrapper.GetProperty(GetName(format)), format, writtenNumbers));
                }
                else
                {
                    values.Add(MapFieldsGroup(wrapper.GetProperty(GetName(element)), (FieldsGroup)element, writtenNumbers));
                }
            }
            return values;
        }

        /// <summary>
        /// Map fields group object
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fieldsGroup"></param>
        /// <param name="writtenNumbers"></param>
        /// <returns></returns>
        private List<object> MapFieldsGroup(object value, FieldsGroup fieldsGroup, IDictionary<string, decimal> writtenNumbers)
        {
            List<object> result = new List<object>();
            if (fieldsGroup.HasDependencies() || fieldsGroup.Occurs > 1)
            {
                MapCollection(result,fieldsGroup,writtenNumbers,(dynamic) value );
            }
            else
            {
                result = MapFieldsList(value, fieldsGroup, writtenNumbers);
            }
            return result;
        }

        /// <summary>
        /// dynamic types handling
        /// </summary>
        /// <typeparam name="T">&nbsp;</typeparam>
        /// <param name="result"></param>
        /// <param name="fieldsGroup"></param>
        /// <param name="writtenNumbers"></param>
        /// <param name="value"></param>
        private void MapCollection<T>(List<object> result, FieldsGroup fieldsGroup, IDictionary<string, decimal> writtenNumbers, List<T> value)
        {
            result.AddRange(value.Select(item => MapFieldsList(item, fieldsGroup, writtenNumbers)));
        }

        /// <summary>
        /// Get name from object
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private string GetName(object item)
        {
            var format = item as FieldFormat;
            var name = format != null ? format.Name : ((FieldsGroup)item).Name;
            return ToCamelCase(name);
        }

        /// <summary>
        /// Convert object
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        /// <param name="writtenNumbers"></param>
        /// <returns></returns>
        private object Convert(object value, FieldFormat format, IDictionary<string, decimal> writtenNumbers)
        {
            object result;
            if (value is int)
            {
                result = new decimal((int)value);
                writtenNumbers[format.Name] = (decimal)result;
            }
            else if (value is long)
            {
                result = new decimal((long)value);
                writtenNumbers[format.Name] = (decimal)result;
            }
            else if (value is float)
            {
                result = new decimal((float)value);
                writtenNumbers[format.Name] = (decimal)result;
            }
            else if (value is double)
            {
                result = new decimal((double)value);
                writtenNumbers[format.Name] = (decimal)result;
            }
            else if (value is DateTime)
            {
                result = EncodeDate((DateTime)value, format);
            }
            else if (value is bool)
            {
                result = BooleanEncoder.Encode((bool)value);
            }
            else
            {
                var list = value as List<object>;
                result = list != null ? ConvertList(list, format, writtenNumbers) : value;
            }
            return result;
        }

        /// <summary>
        /// Convert list of objects 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="format"></param>
        /// <param name="writtenNumbers"></param>
        /// <returns></returns>
        private List<object> ConvertList(List<object> values, FieldFormat format, IDictionary<string, decimal> writtenNumbers)
        {
            List<object> result = values.Select(element => Convert(element, format, writtenNumbers)).ToList();
            int occurs;
            if (format.HasDependencies())
            {
                occurs = (int)writtenNumbers[format.DependingOn];
            }
            else
            {
                occurs = format.Occurs;
            }
            int toFill = occurs - values.Count;
            for (int i = 0; i < toFill; i++)
            {
                result.Add(GetFillValue(format));
            }
            return result;
        }

        /// <summary>
        /// Encode date
        /// </summary>
        /// <param name="date"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        private object EncodeDate(DateTime date, FieldFormat format)
        {
            if (DecimalTypes.Contains(format.Type))
            {
                return DateParser.EncodeDecimal(date);
            }            
            return DateParser.EncodeString(date);
            
        }

        /// <summary>
        /// return filler
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        private object GetFillValue(FieldFormat format)
        {
            if (DecimalTypes.Contains(format.Type))
            {
                return 0m;
            }
           return "";
           
        }
        #endregion



    }
}