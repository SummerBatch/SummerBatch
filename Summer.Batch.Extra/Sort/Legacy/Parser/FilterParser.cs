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
using Summer.Batch.Extra.Sort.Filter;
using Summer.Batch.Extra.Sort.Legacy.Accessor;
using Summer.Batch.Extra.Sort.Legacy.Filter;

namespace Summer.Batch.Extra.Sort.Legacy.Parser
{
    /// <summary>
    /// Parser for include and omit configuration cards that produce instances of <see cref="IFilter{T}"/>.
    /// </summary>
    public class FilterParser : AbstractParser
    {
        private const string And = "AND";
        private const string Or = "OR";

        private static readonly Regex FilterCardRegex
            = new Regex("(?:FORMAT=(?<format1>\\w+),)?COND=\\((?<card>.+)\\)(?:,FORMAT=(?<format2>\\w+))?");

        private static readonly string[] ComparisonOperators = { "EQ", "NE", "GT", "GE", "LT", "LE" };
        private static readonly string[] LogicalOperators = { And, Or };

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
        public FilterParser()
        {
            Encoding = Encoding.Default;
        }

        /// <summary>
        /// Constructs a new <see cref="IFilter{T}"/> from the include an omit configuration cards.
        /// </summary>
        /// <param name="include">the include configuration card</param>
        /// <param name="omit">the omit configuration card</param>
        /// <returns>the corresponding <see cref="IFilter{T}"/></returns>
        public IFilter<byte[]> GetFilter(string include, string omit)
        {
            IFilter<byte[]> filter;
            if (include != null && omit != null)
            {
                filter = new ConjunctionFilter<byte[]>
                {
                    Filters = new List<IFilter<byte[]>>
                    {
                        ParseFilter(include), new NegationFilter<byte[]> { Filter = ParseFilter(omit) }
                    }
                };
            }
            else if (include != null)
            {
                filter = ParseFilter(include);
            }
            else
            {
                filter = new NegationFilter<byte[]> { Filter = ParseFilter(omit) };
            }
            return filter;
        }

        /// <summary>
        /// Constructs a new <see cref="IFilter{T}"/> from the include an omit configuration cards.
        /// </summary>
        /// <param name="configuration">the configuration card</param>
        /// <returns>the corresponding <see cref="IFilter{T}"/></returns>
        private IFilter<byte[]> ParseFilter(string configuration)
        {
            string defaultFormat;
            var lexer = GetLexer(FilterCardRegex, configuration, out defaultFormat);
            return ParseFilter(lexer, defaultFormat);
        }

        /// <summary>
        /// Parses an <see cref="IFilter{T}"/>.
        /// </summary>
        /// <param name="lexer">the lexer to read the tokens from</param>
        /// <param name="defaultFormat">the default format</param>
        /// <returns>an <see cref="IFilter{T}"/></returns>
        private IFilter<byte[]> ParseFilter(Lexer lexer, string defaultFormat)
        {
            IFilter<byte[]> filter = null;

            if (lexer.MoveNext())
            {
                if (lexer.Current == OpeningPar)
                {
                    filter = ParseFilter(lexer, defaultFormat);
                    lexer.Parse(ClosingPar);
                }
                else
                {
                    string format1;
                    string format2;
                    var leftAccessor = ParseAccessor(lexer, defaultFormat, out format1);
                    var op = lexer.Parse();
                    var rightAccessor = ParseAccessor(lexer, defaultFormat, out format2);
                    var format = format1 ?? format2;
                    filter = GetFilter(leftAccessor, rightAccessor, format, op);
                }

                if (lexer.Current == Or)
                {
                    var disjunction = new DisjunctionFilter<byte[]>
                    {
                        Filters = new List<IFilter<byte[]>>
                        {
                            filter, ParseFilter(lexer, defaultFormat)
                        }
                    };
                    filter = disjunction;
                }
                else if (lexer.Current == And)
                {
                    var conjunction = new ConjunctionFilter<byte[]>
                    {
                        Filters = new List<IFilter<byte[]>>
                        {
                            filter, ParseFilter(lexer, defaultFormat)
                        }
                    };
                    filter = conjunction;
                }
            }

            return filter;
        }

        /// <summary>
        /// Parses an accessor.
        /// </summary>
        /// <param name="lexer">the lexer to read the tokens from</param>
        /// <param name="defaultFormat">the default format</param>
        /// <param name="format">the format of the parsed accessor</param>
        /// <returns>an <see cref="IAccessor{T}"/></returns>
        private object ParseAccessor(Lexer lexer, string defaultFormat, out string format)
        {
            var elements = new List<string>();
            format = null;

            do
            {
                elements.Add(lexer.Current);
            } while (lexer.MoveNext() && lexer.Current != ClosingPar &&
                     !LogicalOperators.Contains(lexer.Current) && !ComparisonOperators.Contains(lexer.Current));

            if (elements.Count == 1)
            {
                // decimal constant
                return new ConstantAccessor<decimal> { Constant = decimal.Parse(elements[0]) };
            }
            if (elements.Count == 2 && elements[0] == "C")
            {
                // string constant
                var constant = elements[1].Substring(1, elements[1].Length - 2);
                return new ConstantAccessor<string> { Constant = constant };
            }
            if (elements.Count == 2 && elements[0] == "X")
            {
                // HexaDecimal constant
                var constant = elements[1].Substring(1, elements[1].Length - 2);
                BinaryAccessor accessor =  new BinaryAccessor ();
                int value = Convert.ToInt32(constant, 16);
                byte[] bytes = Enumerable.Range(0, constant.Length).Where(x => x % 2 == 0).Select(x => Convert.ToByte(constant.Substring(x, 2), 16)).ToArray();
                accessor.SetBytes(bytes, bytes, 0);
                decimal constant1 = Convert.ToInt32(constant, 16);

                return new ConstantAccessor<decimal> { Constant = constant1 };     
            }
            // field accesor
            var start = int.Parse(elements[0]) - 1;
            var length = int.Parse(elements[1]);
            format = elements.Count == 3 ? elements[2] : defaultFormat;
            return GetAccessor(start, length, format, Encoding);
        }

        /// <summary>
        /// Creates a new <see cref="IFilter{T}"/> depending on its format.
        /// </summary>
        /// <param name="leftAccessor">the left value accessor</param>
        /// <param name="rightAccessor">the right value accessor</param>
        /// <param name="format">the format of the filter</param>
        /// <param name="op">the comparison operator</param>
        /// <returns>an <see cref="IFilter{T}"/></returns>
        private IFilter<byte[]> GetFilter(object leftAccessor, object rightAccessor, string format, string op)
        {
            ComparisonOperator comparisonOperator;
            Enum.TryParse(op, true, out comparisonOperator);
            switch (format)
            {
                case StringFormat:
                    return new StringFilter
                    {
                        Left = (IAccessor<string>) leftAccessor,
                        Right = (IAccessor<string>) rightAccessor,
                        Operator = comparisonOperator,
                        SortEncoding = SortEncoding
                    };
                case BinaryFormat:
                    return new DecimalFilter
                    {
                        Left = (IAccessor<decimal>)leftAccessor,
                        Right = (IAccessor<decimal>)rightAccessor,
                        Operator = comparisonOperator
                        
                    };
                case SubstringFormat:
                    return new SubstringFilter { Left = (IAccessor<string>) leftAccessor, Right = (IAccessor<string>) rightAccessor };
                default:
                    return new DecimalFilter
                    {
                        Left = (IAccessor<decimal>) leftAccessor,
                        Right = (IAccessor<decimal>) rightAccessor,
                        Operator = comparisonOperator
                    };
            }
        }
    }
}