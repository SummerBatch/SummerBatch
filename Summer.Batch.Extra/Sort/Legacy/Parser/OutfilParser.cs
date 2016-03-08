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
using System.Collections.Generic;
using System.Text;

namespace Summer.Batch.Extra.Sort.Legacy.Parser
{
    /// <summary>
    /// Parser for outfil cards. It expects outfil configurations containing 'include', 'omit',
    /// and 'outrec' cards. Each configuration must be separated by a semi-colon.
    /// </summary>
    public class OutfilParser : AbstractParser
    {
        private const string Outrec = "outrec=";
        private const string Include = "include=";
        private const string Omit = "omit=";
        private const string SemiColon = ";";

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
        public OutfilParser()
        {
            Encoding = Encoding.Default;
        }

        /// <summary>
        /// Parses the outfil configurations and computes a list of <see cref="IOutputFile{T}"/>.
        /// </summary>
        /// <param name="outfils">The outfil configurations, separated by commas.</param>
        /// <returns></returns>
        public IList<IOutputFile<byte[]>> GetOutputFiles(string outfils)
        {
            var outputFiles = new List<IOutputFile<byte[]>>();
            var lexer = new Lexer(outfils);
            var outputFile = ParseOutfil(lexer);
            while (outputFile != null)
            {
                outputFiles.Add(outputFile);
                outputFile = ParseOutfil(lexer);
            }
            return outputFiles;
        }

        // Parses a single outfil configuration.
        private LegacyOutputFile ParseOutfil(Lexer lexer)
        {
            LegacyOutputFile outputFile = null;
            if (lexer.MoveNext())
            {
                outputFile = new LegacyOutputFile();
                string outrec = null;
                string include = null;
                string omit = null;

                while (lexer.Current != null)
                {
                    // Look for the supported commands
                    if (string.Equals(Outrec, lexer.Current, StringComparison.InvariantCultureIgnoreCase))
                    {
                        lexer.MoveNext();
                        outrec = ParseCommandParameters(lexer);
                    }
                    else if (string.Equals(Include, lexer.Current, StringComparison.InvariantCultureIgnoreCase))
                    {
                        lexer.MoveNext();
                        include = ParseCommandParameters(lexer);
                    }
                    else if (string.Equals(Omit, lexer.Current, StringComparison.InvariantCultureIgnoreCase))
                    {
                        lexer.MoveNext();
                        omit = ParseCommandParameters(lexer);
                    }
                    else if (lexer.Current != SemiColon)
                    {
                        throw new ParsingException(string.Format("Unexpected token at index {0}: {1}", lexer.Index,
                            lexer.Current));
                    }
                    else
                    {
                        break;
                    }
                }

                // Set the required properties of the output file
                if (!string.IsNullOrEmpty(outrec))
                {
                    var formatterParser = new FormatterParser { Encoding = Encoding };
                    outputFile.Formatter = formatterParser.GetFormatter(outrec);
                }
                if (!string.IsNullOrWhiteSpace(include) || !string.IsNullOrWhiteSpace(omit))
                {
                    var filterParser = new FilterParser { Encoding = Encoding, SortEncoding = SortEncoding };
                    outputFile.Filter = filterParser.GetFilter(include, omit);
                }
            }

            return outputFile;
        }

        // Retrieves the parameter of a command so that it can be parsed by a specific parser.
        private static string ParseCommandParameters(Lexer lexer)
        {
            var start = lexer.Index;
            lexer.Parse(OpeningPar);

            while (lexer.MoveNext() && lexer.Current != ClosingPar) { }

            var length = lexer.Index - start - 1;
            lexer.Parse(ClosingPar);

            return lexer.SubString(start, length);
        }
    }
}