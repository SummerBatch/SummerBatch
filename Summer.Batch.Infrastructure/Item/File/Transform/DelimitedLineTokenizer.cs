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
using System.Collections.Generic;

namespace Summer.Batch.Infrastructure.Item.File.Transform
{
    /// <summary>
    /// Implementation of <see cref="ILineTokenizer"/> that splits the input string
    /// using a configurable delimiter. A column can be surrounded by a configurable
    /// quote character to include the delimiter.
    /// </summary>
    public class DelimitedLineTokenizer : AbstractLineTokenizer
    {
        private const string DefaultDelimiter = ",";
        private const char DefaultQuoteCharacter = '"';

        private ISet<int> _includedFields;

        /// <summary>
        /// The delimiter that separates columns. Default is ",".
        /// </summary>
        public string Delimiter { get; set; }

        /// <summary>
        /// The quote character. Default is '"'.
        /// </summary>
        public char QuoteCharacter { get; set; }

        /// <summary>
        /// Sets the zero-based index of the columns to include in the returned <see cref="IFieldSet"/>.
        /// By default all columns are included.
        /// </summary>
        public IEnumerable<int> IncludedFields { set { _includedFields = new HashSet<int>(value); } }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DelimitedLineTokenizer()
        {
            Delimiter = DefaultDelimiter;
            QuoteCharacter = DefaultQuoteCharacter;
        }

        /// <summary>
        /// Method that does the actual tokenizing.
        /// </summary>
        /// <param name="line">the line to tokenize</param>
        /// <returns>a list of field values</returns>
        protected override IList<string> DoTokenize(string line)
        {
            var tokens = new List<string>();

            var chars = line.ToCharArray();
            var length = chars.Length;
            var inQuote = false;
            var lastCut = 0;
            var endIndexLastDelimiter = -1;
            int fieldCount = 0;

            for (var i = 0; i < length; i++)
            {
                var isEnd = (i == (length - 1));

                var isDelimiter = IsDelimiter(chars, i, endIndexLastDelimiter);

                if ((isDelimiter && !inQuote) || isEnd)
                {
                    endIndexLastDelimiter = i;
                    var endPosition = i;

                    if (isDelimiter)
                    {
                        endPosition -= Delimiter.Length;
                    }

                    if (_includedFields == null || _includedFields.Contains(fieldCount))
                    {
                        tokens.Add(RetrieveToken(chars, lastCut, endPosition));
                    }

                    fieldCount++;

                    if (isEnd && isDelimiter)
                    {
                        if (_includedFields == null || _includedFields.Contains(fieldCount))
                        {
                            tokens.Add(string.Empty);
                        }
                        fieldCount++;
                    }

                    lastCut = i + 1;
                }
                else if (chars[i] == QuoteCharacter)
                {
                    inQuote = !inQuote;
                }
            }

            return tokens;
        }

        /// <summary>
        /// Checks if the delimiter string has been encountered.
        /// </summary>
        /// <param name="chars">the character array to search</param>
        /// <param name="i">the current index</param>
        /// <param name="endIndexLastDelimiter">the index of the last encountered delimiter</param>
        /// <returns><code>true</code>if the delimiter string has been encountered</returns>
        private bool IsDelimiter(char[] chars, int i, int endIndexLastDelimiter)
        {
            if (i - endIndexLastDelimiter >= Delimiter.Length && i >= Delimiter.Length - 1)
            {
                for (var j = 0; j < Delimiter.Length; j++)
                {
                    if (Delimiter[j] != chars[i - Delimiter.Length + 1 + j])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Retrieves a token from a character array.
        /// </summary>
        /// <param name="chars">the character array containing the token</param>
        /// <param name="start">the index of the first character of the token</param>
        /// <param name="end">the index of the last character of the token</param>
        /// <returns>the token without the surrounding quotes, or the token itself if there are no surrounding quotes</returns>
        private string RetrieveToken(char[] chars, int start, int end)
        {
            var startQuote = start;
            var endQuote = end;
            // Remove quotes if necessary
            if (SearchQuote(chars, ref startQuote, ref endQuote))
            {
                var token = new string(chars, startQuote + 1, endQuote - startQuote - 1);
                token = token.Replace(QuoteCharacter.ToString() + QuoteCharacter, QuoteCharacter.ToString());
                return token;
            }
            return new string(chars, start, end - start + 1);
        }



        /// <summary>
        /// Search quote in given range.
        /// </summary>
        /// <param name="chars"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private bool SearchQuote(IReadOnlyList<char> chars, ref int start, ref int end)
        {
            while (chars[start] == ' ')
            {
                start++;
            }
            if (chars[start] != QuoteCharacter)
            {
                return false;
            }
            while (chars[end] == ' ')
            {
                end--;
            }
            return chars[end] == QuoteCharacter;
        }
    }
}