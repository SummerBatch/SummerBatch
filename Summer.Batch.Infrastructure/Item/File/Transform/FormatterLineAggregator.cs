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
using System.Globalization;
using Summer.Batch.Common.Util;

namespace Summer.Batch.Infrastructure.Item.File.Transform
{
    /// <summary>
    /// Implementation of <see cref="T:ILineAggregator"/> that produces a string by aggregating
    /// the provided item with <see cref="string.Format(System.IFormatProvider,string,object[])"/>.
    /// </summary>
    public class FormatterLineAggregator<T> : ExtractorLineAggregator<T> where T : class
    {
        /// <summary>
        /// The string to use for formatting.
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// The culture to use when formatting. Default is <see cref="P:CultureInfo.CurrentCulture"/>.
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// The minimum allowed length for the formatted string. Zero means that there are no minimum.
        /// </summary>
        public int MinimumLength { get; set; }

        /// <summary>
        /// The maximum allowed length for the formatted string. Zero means that there are no maximum.
        /// </summary>
        public int MaximumLength { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public FormatterLineAggregator()
        {
            CultureInfo = CultureInfo.CurrentCulture;
        }

        /// <summary>
        /// Aggregates the extracted fields to a string.
        /// </summary>
        /// <param name="fields">the extracted fields</param>
        /// <returns>the aggregated line</returns>
        /// <exception cref="System.InvalidOperationException">if the formatted string does not have the expected length</exception>
        protected override string DoAggregate(object[] fields)
        {
            Assert.NotNull(Format);

            var value = string.Format(CultureInfo, Format, fields);

            if (MaximumLength > 0)
            {
                Assert.State(value.Length <= MaximumLength,
                    string.Format("String overflowed in formatter - longer than {0} character: {1}", MaximumLength, value));
            }
            if (MinimumLength > 0)
            {
                Assert.State(value.Length >= MinimumLength,
                    string.Format("String underflowed in formatter - shorter than {0} character: {1}", MinimumLength, value));
            }

            return value;
        }
    }
}