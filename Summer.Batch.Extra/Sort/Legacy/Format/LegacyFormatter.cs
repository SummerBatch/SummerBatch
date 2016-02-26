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
using System.Text;
using Summer.Batch.Extra.Sort.Format;

namespace Summer.Batch.Extra.Sort.Legacy.Format
{
    /// <summary>
    /// Implementation of <see cref="IFormatter{T}"/> that emulates legacy formatting.
    /// It relies on a list of <see cref="ISubFormatter"/> that each create a part of the output record.
    /// </summary>
    public class LegacyFormatter : IFormatter<byte[]>
    {
        /// <summary>
        /// The <see cref="ISubFormatter"/>s that will create the output record.
        /// </summary>
        public IList<ISubFormatter> Formatters { get; set; }

        /// <summary>
        /// The encoding to use to write the output string in the byte array.
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public LegacyFormatter()
        {
            Encoding = Encoding.Default;
        }

        /// <summary>
        /// Formats a record using the sub formatters.
        /// </summary>
        /// <param name="record">the record to format</param>
        /// <returns>the formatted record</returns>
        public byte[] Format(byte[] record)
        {
            var lastFormatter = Formatters[Formatters.Count - 1];
            var result = new byte[lastFormatter.OutputIndex + lastFormatter.Length];
            var whitespace = Encoding.GetBytes(" ")[0];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = whitespace;
            }

            foreach (var formatter in Formatters)
            {
                formatter.Format(record, result);
            }

            return result;
        }


        public byte[] Format(string p)
        {
            byte[] newline = Encoding.GetBytes(p);
            return newline;
        }
    }
}