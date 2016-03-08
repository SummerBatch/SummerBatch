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
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Diagnostics;
using System.Text;

namespace Summer.Batch.Extra.Sort.Legacy.Parser
{
    /// <summary>
    /// Lexer for DFSORT configuration strings.
    /// </summary>
    [DebuggerDisplay("Index: {_index}, Current: {Current}")]
    public class Lexer 
    {
        private const char Quote = '\'';

        private readonly char[] _chars;
        private int _index;

        /// <summary>
        /// The current token
        /// </summary>
        public string Current { get; private set; }

        /// <summary>
        /// An index that points to the next character to read.
        /// </summary>
        public int Index { get { return _index; } }

        public string SubString(int start, int length)
        {
            var subArray = new char[length];
            Array.Copy(_chars, start, subArray, 0, length);
            return new string(subArray);
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="configuration">the configuration string to parse</param>
        public Lexer(string configuration)
        {
            _chars = configuration.ToCharArray();
        }

        /// <summary>
        /// Advances the lexer to the next token.
        /// </summary>
        /// <returns>
        /// true if a token was successfully parsed; false if the end of the configuration string has been reached
        /// </returns>
        public bool MoveNext()
        {
            Current = ParseNext();
            return Current != null;
        }

        /// <summary>
        /// Parse - move to next token.
        /// </summary>
        /// <returns></returns>
        public string Parse()
        {
            var result = Current;
            MoveNext();
            return result;
        }

        /// <summary>
        /// Parse given token
        /// </summary>
        /// <param name="token"></param>
        public void Parse(string token)
        {
            if (Current != token)
            {
                throw new ParsingException(string.Format("Parsing error around index {0} - expecting: {1}, actual: {2}", _index, token, Current),
                                           new string(_chars), _index);
            }
            MoveNext();
        }

        /// <summary>
        /// Try parsing current as int
        /// </summary>
        /// <returns></returns>
        public int ParseInt()
        {
            if (Current != null)
            {
                int i;
                if (int.TryParse(Current, out i))
                {
                    MoveNext();
                    return i;
                }
            }
            throw new ParsingException(string.Format("Parsing error around index {0} - expecting an integer, actual: {1}", _index, Current),
                                       new string(_chars), _index);
        }

        /// <summary>
        /// Parses the next token
        /// </summary>
        private string ParseNext()
        {
            var sb = new StringBuilder();
            for (; _index < _chars.Length; _index++)
            {
                var c = _chars[_index];
                switch (c)
                {
                    case ',':
                    case ' ':
                    case '\t':
                    case '\n':
                    case '\r':
                        // token separator are ignored
                        // if there were already read characters, stop
                        if (sb.Length > 0)
                        {
                            _index++;
                            return sb.ToString();
                        }
                        break;
                    case '(':
                    case ')':
                        // parentheses are token seprators, but also tokens
                        if (sb.Length > 0)
                        {
                            // there is already a token in the buffer use it
                            // but do not increase the index to read the parenthese next time
                            return sb.ToString();
                        }
                        // no other token, the parenthese is the new token
                        _index++;
                        return c.ToString();
                    case ';':
                        _index++;
                        return ";";
                    case Quote:
                        // a quote ends any read token and starts a new string literal token
                        return sb.Length > 0 ? sb.ToString() : ParseString();
                    default:
                        // default case: append the character to the current token
                        sb.Append(c);
                        break;
                }
            }
            if (sb.Length > 0)
            {
                // we reached the end while parsing a token
                return sb.ToString();
            }
            return null;
        }

        /// <summary>
        /// Parses a string literal
        /// </summary>
        /// <returns>a string literal (with the surrounding quotes)</returns>
        private string ParseString()
        {
            _index++;
            var sb = new StringBuilder().Append(Quote);
            for (; _index < _chars.Length; _index++)
            {
                var c = _chars[_index];
                sb.Append(c);
                if (c == Quote)
                {
                    _index++;
                    if (_index >= _chars.Length - 1 || _chars[_index + 1] != Quote)
                    {
                        // this is not a double quote, the string literal has ended
                        return sb.ToString();
                    }
                }
            }
            return null;
        }
    }
}