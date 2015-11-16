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
using Summer.Batch.Extra.Copybook;

namespace Summer.Batch.Extra.Ebcdic
{
    /// <summary>
    /// A mapper for reading EBCDIC files with different record formats. It must be
    /// provided with the correct mappers, and will delegate to them according to the
    /// discriminator value.
    /// </summary>
    public class CompositeEbcdicReaderMapper<T> : IEbcdicReaderMapper<T>
    {
        #region Attributes
        /// <summary>
        /// Setter for the underlying mappers
        /// </summary>
        public IEnumerable<IEbcdicReaderMapper<T>> Mappers { private get; set; }

        /// <summary>
        /// RecordFormatMap property.
        /// </summary>
        public RecordFormatMap RecordFormatMap
        {
            set
            {
                foreach (var mapper in Mappers)
                {
                    mapper.RecordFormatMap = value;
                }
            }
        }

        /// <summary>
        /// Date parser property.
        /// </summary>
        public IDateParser DateParser
        {
            set
            {
                foreach (var mapper in Mappers)
                {
                    mapper.DateParser = value;
                }
            }

        }

        /// <summary>
        /// DistinguishedPattern property. Not used here, so returns null;
        /// </summary>
        public string DistinguishedPattern
        {
            get { return null; }
        } 
        #endregion

        /// <summary>
        /// Converts the content of a list of values into a business object using a sub-mappers.
        /// The first item of the list is expected to be the discriminator pattern of the mapper to use.
        /// </summary>
        /// <param name="values">the list of values to map</param>
        /// <param name="itemCount">the record line number, starting at 0.</param>
        /// <returns>The mapped object</returns>
        public T Map(IList<object> values, int itemCount)
        {
            var discriminatorPattern = values[0].ToString();
            values.RemoveAt(0);
            foreach (var mapper in Mappers)
            {
                if (mapper.DistinguishedPattern == discriminatorPattern)
                {
                    return mapper.Map(values, itemCount);
                }
            }
            return default(T);
        }

    }
}