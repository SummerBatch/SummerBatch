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
using System.Text;
using System.Text.RegularExpressions;
using Summer.Batch.Extra.Sort.Legacy.Accessor;
using Summer.Batch.Extra.Sort.Sum;

namespace Summer.Batch.Extra.Sort.Legacy.Parser
{
    /// <summary>
    /// Parser for sum configuration cards that produces instances of <see cref="ISum{T}"/>.
    /// </summary>
    public class SumParser : AbstractParser
    {
        private static readonly Regex SumRegex
            = new Regex("(?:FORMAT=(?<format1>\\w+),)?FIELDS=\\((?<card>.+)\\)(?:,FORMAT=(?<format2>\\w+))?");

        private static readonly string[] Formats = { ZonedFormat, PackedFormat, BinaryFormat, SignedBinaryFormat };

        /// <summary>
        /// The encoding of the records to sum.
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Gets a sum from its configuration card.
        /// </summary>
        /// <param name="configuration">a sum configuration card</param>
        /// <returns>the corresponding <see cref="ISum{T}"/></returns>
        public ISum<byte[]> GetSum(string configuration)
        {
            string defaultFormat;
            var lexer = GetLexer(SumRegex, configuration, out defaultFormat);

            if (lexer.MoveNext())
            {
                if (string.Equals(lexer.Current, "NONE", StringComparison.OrdinalIgnoreCase))
                {
                    return new SkipSum<byte[]>();
                }
                var accessors = new List<IAccessor<decimal>>();
                var parentheses = lexer.Current == OpeningPar;
                if (parentheses)
                {
                    lexer.MoveNext();
                }
                while (lexer.Current != null)
                {
                    accessors.Add(ParseAccessor(lexer, defaultFormat));
                }
                if (parentheses)
                {
                    lexer.Parse(ClosingPar);
                }
                return new BytesSum { Accessors = accessors };
            }

            return null;
        }

        /// <summary>
        /// Parses an accessor.
        /// </summary>
        /// <param name="lexer">the lexer to read the tokens from</param>
        /// <param name="defaultFormat">the default format</param>
        /// <returns>the parsed <see cref="IAccessor{T}"/></returns>
        private IAccessor<decimal> ParseAccessor(Lexer lexer, string defaultFormat)
        {
            var start = lexer.ParseInt() - 1;
            var length = lexer.ParseInt();
            var format = defaultFormat;
            if (Formats.Contains(lexer.Current))
            {
                format = lexer.Parse();
            }
            return (IAccessor<decimal>) GetAccessor(start, length, format, Encoding);
        }
    }
}