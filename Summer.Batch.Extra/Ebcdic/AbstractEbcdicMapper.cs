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
using System.Text;

namespace Summer.Batch.Extra.Ebcdic
{
    /// <summary>
    /// Abstract super type for all mappers. It only contains the implementation of
    /// ToCamelCase, which is required by both reader and writer mappers.
    /// </summary>
    public class AbstractEbcdicMapper
    {
        /// <summary>
        /// protected constructor
        /// </summary>
        protected AbstractEbcdicMapper() { }

        /// <summary>
        /// Converts a cobol name to a camel case name.
        /// </summary>
        /// <param name="name">the name to convert</param>
        /// <returns>the camel case equivalent of the given name</returns>
        protected static string ToCamelCase(string name)
        {
            StringBuilder sb = new StringBuilder(name.Length);
            bool toUpper = false;
            foreach (char c in name)
            {
                switch (c)
                {
                    case '_':
                    case '-':
                        toUpper = true;
                        break;
                    default:
                        sb.Append(toUpper ? Char.ToUpperInvariant(c) : Char.ToLowerInvariant(c));
                        toUpper = false;
                        break;
                }
            }
            return sb.ToString();
        }
    }
}