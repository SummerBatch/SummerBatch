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
using System.Linq;
using System.Text;
using Summer.Batch.Data.Parameter;

namespace Summer.Batch.Data
{
    /// <summary>
    /// A class for parsing parameters of an SQL query. Parameters are expected to be named and have '@' or ':' as a prefix.
    /// The specified <see cref="IPlaceholderGetter"/> is used to replace the parameters in the substituted query.
    /// </summary>
    public class ParsedQuery
    {
        #region Parsing constants

        private static readonly char[] ParameterHolderPrefix = { '@', ':' };
        private static readonly char[] ParameterHolderEnd = { ' ', ',', ')', '"', '\'', '|', ';', '\n', '\r', '\t' };

        #endregion

        private readonly string _originalQuery;
        private readonly IPlaceholderGetter _placeholderGetter;
        private readonly IList<string> _parameterNames = new List<string>();
        private readonly IList<Position> _parameterPositions = new List<Position>();

        /// <summary>
        /// The original query, without any substitution.
        /// </summary>
        public string OriginalQuery
        {
            get { return _originalQuery; }
        }

        /// <summary>
        /// Whether the returned substituted query will have named parameters or not.
        /// </summary>
        public bool Named { get { return _placeholderGetter.Named; } }

        /// <summary>
        /// A list containing the parameter names in the order they are used. If a parameter is used several times,
        /// it will also appear several times in this list. It is useful to set parameter values when using unnamed
        /// placeholders (e.g., '?').
        /// </summary>
        public IList<string> ParameterNames { get { return _parameterNames; } }

        /// <summary>
        /// The query with its parameter substituted using the <see cref="IPlaceholderGetter"/>.
        /// </summary>
        public string SubstitutedQuery
        {
            get
            {
                var builder = new StringBuilder();

                var lastIndex = 0;
                for (var i = 0; i < _parameterNames.Count; i++)
                {
                    var position = _parameterPositions[i];
                    builder.Append(_originalQuery.Substring(lastIndex, position.Start - lastIndex));
                    builder.Append(_placeholderGetter.GetPlaceholder(_parameterNames[i]));
                    lastIndex = position.End + 1;
                }
                builder.Append(_originalQuery.Substring(lastIndex));

                return builder.ToString();
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="originalQuery">an SQL query</param>
        /// <param name="placeholderGetter">the <see cref="IPlaceholderGetter"/> to use for parameter substitution</param>
        public ParsedQuery(string originalQuery, IPlaceholderGetter placeholderGetter)
        {
            _originalQuery = originalQuery.Trim();
            _placeholderGetter = placeholderGetter;
            ParseQuery();
        }

        /// <summary>
        /// Parses the query to compute the name and position of parameters.
        /// </summary>
        private void ParseQuery()
        {
            var chars = _originalQuery.ToCharArray();
            var parsingParameter = false;
            var parameterStart = -1;
            for (var i = 0; i < chars.Length; i++)
            {
                var c = chars[i];
                if (parsingParameter && ParameterHolderEnd.Contains(c))
                {
                    _parameterNames.Add(_originalQuery.Substring(parameterStart + 1, i - parameterStart - 1));
                    _parameterPositions.Add(new Position(parameterStart, i - 1));
                    parsingParameter = false;

                }
                if (!parsingParameter && ParameterHolderPrefix.Contains(c))
                {
                    parsingParameter = true;
                    parameterStart = i;
                }
            }
            if (parsingParameter)
            {
                _parameterNames.Add(_originalQuery.Substring(parameterStart + 1, chars.Length - parameterStart - 1));
                _parameterPositions.Add(new Position(parameterStart, chars.Length - 1));
            }
        }

        #region Position struct

        /// <summary>
        /// A simple struct to hold parameter position.
        /// </summary>
        public struct Position
        {
            /// <summary>
            /// start position
            /// </summary>
            public readonly int Start;

            /// <summary>
            /// End position
            /// </summary>
            public readonly int End;

            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="start">the index of the start of the parameter in the original query</param>
            /// <param name="end">the index of the end of the parameter in the original query (inclusive)</param>
            public Position(int start, int end)
            {
                Start = start;
                End = end;
            }
        }

        #endregion

    }
}