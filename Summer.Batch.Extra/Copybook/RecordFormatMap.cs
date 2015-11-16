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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Summer.Batch.Extra.Copybook
{
    /// <summary>
    /// Map of record format.
    /// </summary>
    public class RecordFormatMap
    {
        private const string Pattern = "^(?:{0})$";

        private readonly IDictionary<Regex, RecordFormat> _regexDictionary = new Dictionary<Regex, RecordFormat>();
        private readonly IDictionary<string, RecordFormat> _idDictionary = new Dictionary<string, RecordFormat>();
        private readonly ConditionalWeakTable<string, RecordFormat> _cache = new ConditionalWeakTable<string, RecordFormat>();

        /// <summary>
        /// Default record format.
        /// </summary>
        public RecordFormat Default
        {
            get { return _idDictionary.Values.FirstOrDefault(); }
        }

        /// <summary>
        /// MultipleRecordFormats property.
        /// </summary>
        public bool MultipleRecordFormats { get { return _idDictionary.Count > 1; } }

        /// <summary>
        /// Custom constructor using a FileFormat
        /// </summary>
        /// <param name="fileFormat"></param>
        public RecordFormatMap(FileFormat fileFormat)
        {
            foreach (var recordFormat in fileFormat.RecordFormats)
            {
                _regexDictionary[new Regex(string.Format(Pattern, recordFormat.DiscriminatorPattern))] = recordFormat;
                _idDictionary[recordFormat.DiscriminatorPattern] = recordFormat;
            }
        }

        /// <summary>
        /// Return RecordFormat given discriminator.
        /// </summary>
        /// <param name="discriminator"></param>
        /// <returns></returns>
        public RecordFormat GetFromDiscriminator(string discriminator)
        {
            RecordFormat format;

            if (!_cache.TryGetValue(discriminator, out format))
            {
                format = _regexDictionary.FirstOrDefault(pair => pair.Key.IsMatch(discriminator)).Value;
                if (format != null)
                {
                    _cache.Add(discriminator, format);
                }
            }

            return format;
        }

        /// <summary>
        /// Return RecordFormat given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RecordFormat GetFromId(string id)
        {
            RecordFormat result;
            _idDictionary.TryGetValue(id, out result);
            return result;
        }
    }
}
