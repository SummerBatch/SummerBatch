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
using Summer.Batch.Common.Util;

namespace Summer.Batch.Infrastructure.Item.File.Transform
{
    /// <summary>
    /// An implementation of <see cref="T:ILineAggregator"/> that converts an object
    /// into a delimited list of strings. The default delimiter is a comma.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DelimitedLineAggregator<T> : ExtractorLineAggregator<T> where T : class
    {
        /// <summary>
        /// The delimiter to use. Default is ",".
        /// </summary>
        public string Delimiter { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DelimitedLineAggregator()
        {
            Delimiter = ",";
        }

        /// <summary>
        /// Aggregates the extracted fields to a string.
        /// </summary>
        /// <param name="fields">the extracted fields</param>
        /// <returns>the aggregated line</returns>
        protected override string DoAggregate(object[] fields)
        {
            return fields.ToDelimitedString(Delimiter);
        }
    }
}