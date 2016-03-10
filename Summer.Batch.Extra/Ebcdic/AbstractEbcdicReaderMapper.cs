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
using System.Text.RegularExpressions;
using Summer.Batch.Extra.Copybook;
using Summer.Batch.Common.Factory;

namespace Summer.Batch.Extra.Ebcdic
{
    /// <summary>
    /// Abstract super type for implementation of IEbcdicReaderMapper. It
    /// holds information and code required by most mappers, like date parser.
    /// </summary>
    /// <typeparam name="T">&nbsp;the mapped class</typeparam>
    public abstract class AbstractEbcdicReaderMapper<T> : AbstractEbcdicMapper, IInitializationPostOperations, IEbcdicReaderMapper<T>
    {

        private IDateParser _dateParser = new DateParser();

        /// <summary>
        /// Date parser property.
        /// </summary>
        public IDateParser DateParser
        {
            get { return _dateParser; }
            set
            {
                _dateParser = value;
                foreach (var subMapper in SubMappers)
                {
                    subMapper.DateParser = value;
                }
            }
        }

        /// <summary>
        /// RecordFormatMap property.
        /// </summary>
        public RecordFormatMap RecordFormatMap
        {
            set
            {
                foreach (var subMapper in SubMappers)
                {
                    subMapper.RecordFormatMap = value;
                }
            }
        }

        /// <summary>
        /// DistinguishedPattern property. Returns null by default.
        /// Sub-classes should redefine this as needed.
        /// </summary>
        public virtual string DistinguishedPattern { get { return null; } }

        /// <summary>
        /// Retrieves the submappers. Used to set the date parser and the format map
        /// in cascade.
        /// </summary>
        protected virtual IList<IEbcdicReaderMapper<object>> SubMappers
        {
            get
            {
                return new List<IEbcdicReaderMapper<object>>();
            }
        }

        /// <summary>
        /// See <see cref="IInitializationPostOperations.AfterPropertiesSet"/>.
        /// </summary>
        public void AfterPropertiesSet()
        {
            if (DateParser == null)
            {
                DateParser = new DateParser();
            }
        }

        /// <summary>
        /// Converts the content of a list of values into a business object.
        /// To be implemented by sub-classes.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="itemCount"></param>
        /// <returns></returns>
        public abstract T Map(IList<object> values, int itemCount);

        /// <summary>
        ///  Parses a date encoded in a String or a decimal, using the date parser.
        /// </summary>
        /// <param name="encodedDate">the encoded date</param>
        /// <returns>the corresponding date</returns>
        protected DateTime? ParseDate(object encodedDate)
        {
            DateTime? date = null;
            var s = encodedDate as string;
            if (s != null)
            {
                date = _dateParser.Decode(s);
            }
            else
            {
                if (encodedDate is decimal)
                {
                    date = _dateParser.Decode((decimal)encodedDate);
                }
            }
            return date;
        }

        /// <summary>
        /// Parse object as boolean
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual bool ParseBoolean(object value)
        {
            return value is string && Regex.IsMatch((string) value, "[Y1]");
        }

        /// <summary>
        /// Maps a list of lists to a list of business object using the given mapper.
        /// This method is used for field groups that have multiple occurrences.
        /// </summary>
        /// <param name="values">the list of list to map</param>
        /// <param name="itemCount">the record number</param>
        /// <param name="mapper">the mapper to use for mapping the sub lists</param>
        /// <returns>a list of business objects created using the mapper</returns>
        /// <typeparam name="TSub">&nbsp;</typeparam>
        protected IList<TSub> SubMap<TSub>(IList<object> values, int itemCount, IEbcdicReaderMapper<TSub> mapper)
        {
            return values.Select(value => mapper.Map((IList<object>)value, itemCount)).ToList();
        }
    }
}