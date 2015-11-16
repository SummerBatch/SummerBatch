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
using System;
using System.Collections.Generic;
using System.Linq;

namespace Summer.Batch.Infrastructure.Item.File.Transform
{
    /// <summary>
    /// Abstract base class for <see cref="ILineTokenizer"/> implementations.
    /// </summary>
    public abstract class AbstractLineTokenizer : ILineTokenizer
    {
        /// <summary>
        /// Column names. May be null.
        /// </summary>
        public string[] Names { get; set; }

        /// <summary>
        /// Property for strict mode. In strict mode the resulting field set must have
        /// the specified number of columns. If not in strict mode, the field set
        /// will be padded with empty columns or truncated to adjust its size.
        /// </summary>
        public bool Strict { get; set; }

        /// <summary>
        /// The factory that creates field sets. Default is an instance of <see cref="DefaultFieldSetFactory"/>.
        /// </summary>
        public IFieldSetFactory FieldSetFactory { get; set; }

        /// <summary>
        /// Whether column names are specified.
        /// </summary>
        public bool HasNames { get { return Names != null && Names.Length > 0; } }

        /// <summary>
        /// Default parameterless constructor to create default values.
        /// </summary>
        protected AbstractLineTokenizer()
        {
            FieldSetFactory = new DefaultFieldSetFactory();
            Strict = true;
        }

        /// <summary>
        /// Split a line into tokens.
        /// </summary>
        /// <param name="line">the line to tokenize</param>
        /// <returns>a fieldset containing the tokens</returns>
        public IFieldSet Tokenize(string line)
        {
            var aLine = line ?? string.Empty;

            var tokens = DoTokenize(aLine);

            if (HasNames && !Strict)
            {
                AdjustTokenCount(tokens);
            }

            var values = tokens.ToArray();

            if (HasNames && Names.Length != values.Length)
            {
                throw new InvalidOperationException(string.Format("Incorrect token count. There are {0} names, but there are {1} tokens.",
                                                                  Names.Length, values.Length));
            }
            return FieldSetFactory.Create(values, HasNames ? Names : null);
        }

        /// <summary>
        /// Abstract method that does the actual tokenizing.
        /// </summary>
        /// <param name="line">the line to tokenize</param>
        /// <returns>a list of field values</returns>
        protected abstract IList<string> DoTokenize(string line);

        /// <summary>
        /// Adjusts the size of a list of tokens.
        /// </summary>
        /// <param name="tokens">the list of tokens to adjust</param>
        private void AdjustTokenCount(IList<string> tokens)
        {
            var nameLength = Names.Length;
            var tokensCount = tokens.Count;

            if (nameLength > tokensCount)
            {
                for (var i = 0; i < nameLength - tokensCount; i++)
                {
                    tokens.Add(string.Empty);
                }
            }
            else
            {
                for (var i = tokensCount - 1; i >= nameLength; i--)
                {
                    tokens.RemoveAt(i);
                }
            }
        }
    }
}