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
using Summer.Batch.Extra.Sort.Legacy.Format;

namespace Summer.Batch.Extra.Sort.Legacy.Parser
{
    /// <summary>
    /// Parser for "inrec" and "outrec" configuration cards that produce instances of
    /// <see cref="Summer.Batch.Extra.Sort.Format.IFormatter{T}"/>.
    /// </summary>
    public class FormatterParser : AbstractParser
    {
        private const string Length = "LENGTH=";
        private const string DefaultPositiveSign = " ";
        private const string DefaultNegativeSign = "-";

        private static readonly Regex StringConstantRegex = new Regex("(?<n>\\d+)?C");
        private static readonly Regex HexOfSpaceRegex = new Regex("(?<n>\\d+)?X");

        private static readonly string[] NumberFormats = { "BI", "FI", "ZD", "PD" };

        // Masks for predefined patterns
        private static readonly IDictionary<string, Mask> Masks = new MaskDictionary
        {
            { "M0", "IIIIIIIIIIIIIIIIIIIIIIIIIIIIIITS", " ", "-" },
            { "M1", "TTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTS", " ", "-" },
            { "M2", "II,III,III,III,III,III,III,III,III,IIT.TTS", " ", "-"},
            { "M3", "II,III,III,III,III,III,III,III,III,IIT.TTCR", "", ""},
            { "M4", "SII,III,III,III,III,III,III,III,III,IIT.TT", "+", "-"},
            { "M5", "SII,III,III,III,III,III,III,III,III,IIT.TTS", " ", "()"},
            { "M6", "III-TTT-TTTT", "", ""},
            { "M7", "TTT-TT-TTTT", "", ""},
            { "M8", "IT:TT:TT", "", ""},
            { "M9", "IT/TT/TT", "", ""},
            { "M10", "IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIT", "", ""},
            { "M11", "TTTTTTTTTTTTTTTTTTTTTTTTTTTTTTT", "", ""},
            { "M12", "SI,III,III,III,III,III,III,III,III,III,IIT", " ", "-"},
            { "M13", "SI.III.III.III.III.III.III.III.III.III.IIT", " ", "-"},
            { "M14", "SI III III III III III III III III III IITS", " ", "()"},
            { "M15", "I III III III III III III III III III IITS", " ", "-"},
            { "M16", "SI III III III III III III III III III IIT", " ", "-"},
            { "M17", "SI'III'III'III'III'III'III'III'III'III'IIT", " ", "-"},
            { "M18", "SII,III,III,III,III,III,III,III,III,IIT.TT", " ", "-"},
            { "M19", "SII.III.III.III.III.III.III.III.III.IIT,TT", " ", "-"},
            { "M20", "SI III III III III III III III III IIT,TTS", " ", "()"},
            { "M21", "II III III III III III III III III IIT,TTS", " ", "-"},
            { "M22", "SI III III III III IIII III III III IIT,TT", " ", "-"},
            { "M23", "SII'III'III'III'III'III'III'III'III'IIT.TT", " ", "-"},
            { "M24", "SII'III'III'III'III'III'III'III'III'IIT,TT", " ", "-"},
            { "M25", "SIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIT", " ", "-"},
            { "M26", "STTTTTTTTTTTTTTTTTTTTTTTTTTTTTTT", "+", "-"}
        };

        /// <summary>
        /// The encoding of the output records.
        /// Default is <see cref="System.Text.Encoding.Default"/>.
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Gets a formatter from its configuration card.
        /// </summary>
        /// <param name="configuration">a formatter configuration card</param>
        /// <returns>the corresponding <see cref="LegacyFormatter"/></returns>
        public LegacyFormatter GetFormatter(string configuration)
        {
            var lexer = new Lexer(configuration);
            IList<ISubFormatter> formatters = null;

            if (lexer.MoveNext())
            {
                var parentheses = lexer.Current == OpeningPar;
                if (parentheses)
                {
                    lexer.MoveNext();
                }
                formatters = ParseSubFormatters(lexer);
                if (parentheses)
                {
                    lexer.Parse(ClosingPar);
                }
            }

            return new LegacyFormatter { Formatters = formatters, Encoding = Encoding };
        }

        /// <summary>
        /// Parses all the sub-formatters.
        /// </summary>
        /// <param name="lexer">the lexer to read the tokens from</param>
        /// <returns>a list of <see cref="ISubFormatter"/></returns>
        private IList<ISubFormatter> ParseSubFormatters(Lexer lexer)
        {
            var outputIndex = 0;
            var subFormatters = new List<ISubFormatter>();

            do
            {
                subFormatters.Add(ParseSubFormatter(lexer, ref outputIndex));
            } while (lexer.Current != null && lexer.Current != ClosingPar);

            return subFormatters;
        }

        /// <summary>
        /// Parses a sub-formatter
        /// </summary>
        /// <param name="lexer">the lexer to read the tokens from</param>
        /// <param name="outputIndex">the index in the output record for the sub-formatter</param>
        /// <returns>the parsed <see cref="ISubFormatter"/></returns>
        private ISubFormatter ParseSubFormatter(Lexer lexer, ref int outputIndex)
        {
            var element = lexer.Parse();
            var column = element.IndexOf(':');
            if (column != -1)
            {
                outputIndex = int.Parse(element.Substring(0, column)) - 1;
                element = element.Substring(column + 1);
            }
            ISubFormatter subFormatter;
            Match match;
            if ((match = StringConstantRegex.Match(element)).Success)
            {
                var constant = lexer.Parse();
                constant = constant.Substring(1, constant.Length - 2);
                subFormatter = GetStringConstantFormatter(GetN(match), constant, outputIndex);
            }
            else if ((match = HexOfSpaceRegex.Match(element)).Success)
            {
                if (lexer.Current != null && lexer.Current.StartsWith("'"))
                {
                    subFormatter = GetHexConstantFormatter(GetN(match), lexer.Parse(), outputIndex);
                }
                else
                {
                    subFormatter = GetStringConstantFormatter(GetN(match), " ", outputIndex);
                }
            }
            else
            {
                var start = int.Parse(element) - 1;
                var length = lexer.ParseInt();
                if (NumberFormats.Contains(lexer.Current))
                {
                    subFormatter = ParseEditFormatter(start, length, outputIndex, lexer);
                }
                else
                {
                    subFormatter = new CopyFormatter
                    {
                        InputIndex = start,
                        Length = length,
                        OutputIndex = outputIndex
                    };
                }
            }
            outputIndex += subFormatter.Length;

            return subFormatter;
        }

        /// <summary>
        /// Creates a string constant sub-formatter.
        /// </summary>
        /// <param name="n">the number of occurrences of the constant</param>
        /// <param name="constant">the constant</param>
        /// <param name="outputIndex">the index in the output record for the sub-formatter</param>
        /// <returns>the corresponding <see cref="ConstantFormatter"/></returns>
        private ConstantFormatter GetStringConstantFormatter(int n, string constant, int outputIndex)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < n; i++)
            {
                sb.Append(constant);
            }
            return new ConstantFormatter
            {
                OutputIndex = outputIndex,
                Constant = Encoding.GetBytes(sb.ToString())
            };
        }

        /// <summary>
        /// Creates a constant sub-formatter from a hexadecimal number in a string.
        /// </summary>
        /// <param name="n">the number of occurrences of the constant</param>
        /// <param name="literal">a string containing an hexadecimal number</param>
        /// <param name="outputIndex">the index in the output record for the sub-formatter</param>
        /// <returns>the corresponding <see cref="ConstantFormatter"/></returns>
        private ConstantFormatter GetHexConstantFormatter(int n, string literal, int outputIndex)
        {
            var hexString = literal.Substring(1, literal.Length - 2);
            var constant = new byte[n * hexString.Length / 2];
            for (var i = 0; i < n; i++)
            {
                for (var j = 0; j < hexString.Length; j += 2)
                {
                    constant[i * hexString.Length / 2 + j / 2] = Convert.ToByte(hexString.Substring(j, 2), 16);
                }
            }
            return new ConstantFormatter
            {
                OutputIndex = outputIndex,
                Constant = constant
            };
        }

        /// <summary>
        /// Parses a numeric edit formatter.
        /// </summary>
        /// <param name="start">the index of the first byte to read in the input record</param>
        /// <param name="length">the length of the column to read in the input record</param>
        /// <param name="outputIndex">the index in the output record for the sub-formatter</param>
        /// <param name="lexer">the lexer to read the tokens from</param>
        /// <returns>the parsed <see cref="NumericEditFormatter"/></returns>
        private NumericEditFormatter ParseEditFormatter(int start, int length, int outputIndex, Lexer lexer)
        {
            var format = lexer.Parse();
            var pattern = lexer.Parse();

            string edit;
            string positiveSign = DefaultPositiveSign;
            string negativeSign = DefaultNegativeSign;
            if (pattern == "EDIT=")
            {
                var sb = new StringBuilder();
                lexer.Parse(OpeningPar);
                do
                {
                    sb.Append(",").Append(lexer.Current);
                } while (lexer.MoveNext() && lexer.Current != ClosingPar);
                edit = sb.ToString().Substring(1);
                lexer.Parse(ClosingPar);
            }
            else
            {
                var mask = Masks[pattern];
                edit = mask.Pattern;
                positiveSign = mask.PositiveSign;
                negativeSign = mask.NegativeSign;
            }
            var editLength = edit.Length;

            while (IsPartOfEdit(lexer.Current))
            {
                var element = lexer.Parse();
                if (element.StartsWith(Length))
                {
                    editLength = int.Parse(element.Substring(Length.Length));
                }
                else
                {
                    lexer.Parse(OpeningPar);
                    positiveSign = lexer.Parse();
                    if (lexer.Current == ClosingPar)
                    {
                        // We actually only have the negative sign
                        negativeSign = positiveSign;
                        positiveSign = DefaultPositiveSign;
                    }
                    else
                    {
                        negativeSign = lexer.Parse();
                    }
                    lexer.Parse(ClosingPar);
                }
            }

            return new NumericEditFormatter
            {
                Accessor = (IAccessor<decimal>) GetAccessor(start, length, format, Encoding),
                Edit = edit, Encoding = Encoding, Length = editLength,
                PositiveSign = positiveSign, NegativeSign = negativeSign, OutputIndex = outputIndex
            };
        }

        /// <summary>
        /// Gets the number of occurences of a constant using a regex.
        /// </summary>
        /// <param name="match">the match obtained from the regex</param>
        /// <returns>the number of occurences</returns>
        private static int GetN(Match match)
        {
            var n = match.Groups["n"].Value;
            return n == string.Empty ? 1 : int.Parse(n);
        }

        /// <summary>
        /// Checks if an element is part of the numeric edit formatter.
        /// </summary>
        /// <param name="element">an element of the configuration card</param>
        /// <returns>true if the element is the length or signs option</returns>
        private static bool IsPartOfEdit(string element)
        {
            return element != null && (element.StartsWith(Length) || element.StartsWith("SIGNS="));
        }

        /// <summary>
        /// Simple structure to hold information for predefined masks.
        /// </summary>
        private struct Mask
        {
            /// <summary>
            /// Numeric edit pattern
            /// </summary>
            public readonly string Pattern;

            /// <summary>
            /// Sign for positive numbers
            /// </summary>
            public readonly string PositiveSign;

            /// <summary>
            /// Sign for negative numbers
            /// </summary>
            public readonly string NegativeSign;

            /// <summary>
            /// Creates a new mask
            /// </summary>
            /// <param name="pattern">the pattern</param>
            /// <param name="positiveSign">the positive sign</param>
            /// <param name="negativeSign">the negative sign</param>
            public Mask(string pattern, string positiveSign, string negativeSign)
            {
                Pattern = pattern;
                PositiveSign = positiveSign;
                NegativeSign = negativeSign;
            }
        }

        /// <summary>
        /// A dictionary with an extra <see cref="Add"/> to add <see cref="Mask"/>s.
        /// </summary>
        private class MaskDictionary : Dictionary<string, Mask>
        {
            /// <summary>
            /// Utility method to add a new <see cref="Mask"/> to the dictionary.
            /// </summary>
            /// <param name="name">the name of the mask (used as key in the dictionary)</param>
            /// <param name="pattern">the pattern of the mask</param>
            /// <param name="positiveSign">the positive sign of the mask</param>
            /// <param name="negativeSign">the negative sign of the mask</param>
            public void Add(string name, string pattern, string positiveSign, string negativeSign)
            {
                Add(name, new Mask(pattern, positiveSign, negativeSign));
            }
        }
    }
}