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
using System.Text.RegularExpressions;
using Summer.Batch.Extra.Sort.Comparer;
using Summer.Batch.Extra.Sort.Legacy.Accessor;
using Summer.Batch.Extra.Sort.Legacy.Comparer;

namespace Summer.Batch.Extra.Sort.Legacy.Parser
{
    /// <summary>
    /// Parser for sort configuration cards that creates an <see cref="IComparer{T}"/>.
    /// </summary>
    public class ComparerParser : AbstractParser
    {
        private const string AscendingOrder = "A";
        private const string DescendingOrder = "D";

        private static readonly Regex SortCardRegex =
            new Regex("^(?:FORMAT=(?<format1>\\w+),)?FIELDS=\\((?<card>[^\\)]+)\\)(?:,FORMAT=(?<format2>\\w+))?$");

        /// <summary>
        /// The encoding of the records. Default is <see cref="System.Text.Encoding.Default"/>.
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// The encoding to use when sorting, if different from <see cref="Encoding"/>. Default is <code>null</code>.
        /// </summary>
        public Encoding SortEncoding { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ComparerParser()
        {
            Encoding = Encoding.Default;
        }

        /// <summary>
        /// Constructs a new <see cref="IComparer{T}"/> from a sort configuration card.
        /// </summary>
        /// <param name="configuration">the sort card</param>
        /// <returns>the corresponding <see cref="IComparer{T}"/></returns>
        public IComparer<byte[]> GetComparer(string configuration)
        {
            string defaultFormat;
            var lexer = GetLexer(SortCardRegex, configuration, out defaultFormat);

            IComparer<byte[]> comparer = null;

            if (lexer.MoveNext())
            {
                var parentheses = lexer.Current == OpeningPar;
                if (parentheses)
                {
                    lexer.MoveNext();
                }
                comparer = ParseComparers(lexer, defaultFormat);
                if (parentheses)
                {
                    lexer.Parse(ClosingPar);
                }
            }

            return comparer;
        }

        /// <summary>
        /// Parse all the individual comparers and combine them in a <see cref="ComparerChain{T}"/>.
        /// </summary>
        /// <param name="lexer">the lexer to read the tokens from</param>
        /// <param name="defaultFormat">the default format</param>
        /// <returns>
        /// a single <see cref="IComparer{T}"/> if there is only one comparer, or a <see cref="ComparerChain{T}"/>
        /// if there are more than one comparer.
        /// </returns>
        private IComparer<byte[]> ParseComparers(Lexer lexer, string defaultFormat)
        {
            IList<IComparer<byte[]>> comparers = new List<IComparer<byte[]>>();

            do
            {
                comparers.Add(ParseComparer(lexer, defaultFormat));
            } while (lexer.MoveNext() && lexer.Current != ClosingPar);

            if (comparers.Count == 0)
            {
                return null;
            }
            return comparers.Count > 1 ? new ComparerChain<byte[]> { Comparers = comparers } : comparers[0];
        }

        /// <summary>
        /// Parses an individual comparer.
        /// </summary>
        /// <param name="lexer">the lexer to read the tokens from</param>
        /// <param name="defaultFormat">the default format</param>
        /// <returns>a single <see cref="IComparer{T}"/></returns>
        private IComparer<byte[]> ParseComparer(Lexer lexer, string defaultFormat)
        {
            var start = lexer.ParseInt() - 1;
            var length = lexer.ParseInt();
            string format;

            if (lexer.Current == AscendingOrder || lexer.Current == DescendingOrder)
            {
                // if we have the order token no format is specified, thus use the default one
                format = defaultFormat;
            }
            else
            {
                // otherwise the current token is the format
                format = lexer.Parse();
            }
            var ascending = lexer.Current == AscendingOrder;

            IComparer<byte[]> comparer;
            if (format == StringFormat)
            {
                comparer = new StringComparer
                {
                    Encoding = Encoding,
                    SortEncoding = SortEncoding,
                    Ascending = ascending,
                    Start = start,
                    Length = length
                };
            }
            else
            {
                var accessor = GetAccessor(start, length, format, Encoding) as IAccessor<decimal>;
                comparer = new DefaultComparer<decimal> { Ascending = ascending, Accessor = accessor };
            }

            return comparer;
        }
    }
}