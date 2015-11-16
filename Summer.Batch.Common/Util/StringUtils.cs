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

// This file has been modified.
// Original copyright notice :

/*
 * Copyright 2002-2014 the original author or authors.
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
using System.Text;
using System.Text.RegularExpressions;

namespace Summer.Batch.Common.Util
{
    /// <summary>
    /// Class providing utility methods on character strings.
    /// </summary>
    public static class StringUtils
    {

        /// <summary>
        /// Counts the occurrences of a substring in a string.
        /// </summary>
        /// <param name="str">The string to search.</param>
        /// <param name="sub">The substring to search for.</param>
        /// <returns>
        /// The number of occurrences of <paramref name="sub"/> in <paramref name="str"/>; <c>0</c> if
        /// either is <c>null</c>.
        /// </returns>
        public static int CountOccurrencesOf(string str, string sub)
        {
            if (str == null || sub == null || str.Length == 0 || sub.Length == 0)
            {
                return 0;
            }
            var count = 0;
            var pos = 0;
            int idx;
            while ((idx = str.IndexOf(sub, pos, StringComparison.Ordinal)) != -1)
            {
                ++count;
                pos = idx + sub.Length;
            }
            return count;
        }

        /// <summary>
        /// Checks if a string ends with a specified suffix, ignoring case.
        /// </summary>
        /// <param name="str">The string to search.</param>
        /// <param name="suffix">The suffix to search for.</param>
        /// <returns>Whether <paramref name="str"/> ends with <paramref name="suffix"/>.</returns>
        public static bool EndsWithIgnoreCase(string str, string suffix)
        {
            if (str == null || suffix == null)
            {
                return false;
            }
            if (str.EndsWith(suffix))
            {
                return true;
            }
            if (str.Length < suffix.Length)
            {
                return false;
            }

            var lcStr = str.Substring(str.Length - suffix.Length).ToLower();
            var lcSuffix = suffix.ToLower();
            return lcStr.Equals(lcSuffix);
        }

        /// <summary>
        /// Replaces the first occurences of a regular expression.
        /// </summary>
        /// <param name="text">The string to search.</param>
        /// <param name="regex">The regular expression to search for.</param>
        /// <param name="replace">The replacing text.</param>
        /// <returns>The transformed string.</returns>
        public static string ReplaceFirst(string text, string regex, string replace)
        {
            var regexExpr = new Regex(Regex.Escape(regex)); 
            return regexExpr.Replace(text,replace,1);
        }

        /// <summary>
        /// Aggregates elements into a string using a delimiter.
        /// </summary>
        /// <typeparam name="T">the type of the elements to aggregate</typeparam>
        /// <param name="enumerable">the enumerable containing the elements to aggregate</param>
        /// <param name="delimiter">the delimiter to use</param>
        /// <returns>a string containing the elements of enumerable separated by the delimiter</returns>
        public static string ToDelimitedString<T>(this IEnumerable<T> enumerable, string delimiter)
        {
            var builder = new StringBuilder();
            foreach (var t in enumerable)
            {
                builder.Append(t).Append(delimiter);
            }
            builder.Remove(builder.Length - delimiter.Length, delimiter.Length);
            return builder.ToString();
        }
    }
}
