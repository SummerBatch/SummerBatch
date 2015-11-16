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
using System.Text;
using System.Text.RegularExpressions;
using Summer.Batch.Extra.Sort.Legacy.Accessor;

namespace Summer.Batch.Extra.Sort.Legacy.Parser
{
    /// <summary>
    /// Base class for parsers of legacy configuration cards.
    /// </summary>
    public abstract class AbstractParser
    {
        #region Constants

        /// <summary>
        /// String format constant.
        /// </summary>
        protected const string StringFormat = "CH";
        /// <summary>
        /// Substring format constant.
        /// </summary>
        protected const string SubstringFormat = "SS";
        /// <summary>
        /// Zoned format constant.
        /// </summary>
        protected const string ZonedFormat = "ZD";
        /// <summary>
        /// Packed format constant.
        /// </summary>
        protected const string PackedFormat = "PD";
        /// <summary>
        /// Signed binary format constant.
        /// </summary>
        protected const string SignedBinaryFormat = "FI";
        /// <summary>
        /// Unsigned binary format constant.
        /// </summary>
        protected const string BinaryFormat = "BI";

        /// <summary>
        /// Opening parenthese constant.
        /// </summary>
        protected const string OpeningPar = "(";
        /// <summary>
        /// Closing parenthese constant
        /// </summary>
        protected const string ClosingPar = ")";

        // Lexer Regex Constants
        private const string Card = "card";
        private const string Format1 = "format1";
        private const string Format2 = "format2";

        #endregion

        /// <summary>
        /// Creates a lexer from a regex and a configuration card. The regex is used to get the default format
        ///  which is then written in <paramref name="defaultFormat"/>.
        /// </summary>
        /// <param name="formatRegex">a regex that can extract the card and the default format</param>
        /// <param name="configuration">the configuration card</param>
        /// <param name="defaultFormat">a variable where the default format will be stored</param>
        /// <returns>a new <see cref="Lexer"/></returns>
        protected Lexer GetLexer(Regex formatRegex, string configuration, out string defaultFormat)
        {
            Lexer lexer;
            defaultFormat = null;
            var matches = formatRegex.Matches(configuration);
            if (matches.Count == 1)
            {
                var groups = matches[0].Groups;
                lexer = new Lexer(groups[Card].Value);
                if (groups[Format1].Value != string.Empty)
                {
                    defaultFormat = groups[Format1].Value;
                }
                if (groups[Format2].Value != string.Empty)
                {
                    defaultFormat = groups[Format2].Value;
                }
            }
            else
            {
                lexer = new Lexer(configuration);
            }
            return lexer;
        }

        /// <summary>
        /// Creates an accessor depending on the format.
        /// </summary>
        /// <param name="start">the zero-based index of the first byte of the accessor</param>
        /// <param name="length">the length of the accessor</param>
        /// <param name="format">the format of the accessor</param>
        /// <param name="encoding">the encoding of the accessed records</param>
        /// <returns>a new accessor with the correct parameters</returns>
        protected object GetAccessor(int start, int length, string format, Encoding encoding)
        {
            object result;
            switch (format)
            {
                case StringFormat:
                case SubstringFormat:
                    result = new StringAccessor { Encoding = encoding, Start = start, Length = length };
                    break;
                case ZonedFormat:
                    result = new ZonedAccessor { Encoding = encoding, Length = length, Start = start };
                    break;
                case PackedFormat:
                    result = new PackedAccessor { Encoding = encoding, Length = length, Start = start };
                    break;
                case SignedBinaryFormat:
                    result = new BinaryAccessor { Encoding = encoding, Length = length, Start = start, Signed = true };
                    break;
                case BinaryFormat:
                    result = new BinaryAccessor { Encoding = encoding, Length = length, Start = start, Signed = false };
                    break;
                default:
                    throw new ParsingException("Unknown format: " + format);
            }
            return result;
        }
    }
}