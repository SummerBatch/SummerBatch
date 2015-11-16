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
using System.Text.RegularExpressions;

namespace Summer.Batch.Common.Util
{
    /// <summary>
    /// Regular Expressions Helper.
    /// </summary>
    public static class RegexUtils
    {
        private static readonly Regex HasQuestionMarkRegEx = new Regex(@"\?", RegexOptions.Compiled);
        private static readonly Regex IllegalCharactersRegex = new Regex("[" + @"\/:<>|" + "\"]", RegexOptions.Compiled);
        private static readonly Regex CatchExtensionRegex = new Regex(@"^\s*.+\.([^\.]+)\s*$", RegexOptions.Compiled);
        private const string NonDotCharacters = @"[^.]*";
        
        /// <summary>
        /// Converts a filename pattern (as in DirectoryInfo.GetFiles(string)) into a regular expression
        /// that can be used to match file names.
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static Regex ConvertFilenameWildcardPatternToRegex(string pattern)
        {
            if (pattern == null)
            {
                throw new ArgumentNullException();
            }
            var patternCp = pattern.Trim();
            if (patternCp.Length == 0)
            {
                throw new ArgumentException("Pattern is empty.");
            }
            if (IllegalCharactersRegex.IsMatch(patternCp))
            {
                throw new ArgumentException("Patterns contains illegal characters.");
            }
            bool hasExtension = CatchExtensionRegex.IsMatch(patternCp);
            bool matchExact = false;
            if (HasQuestionMarkRegEx.IsMatch(patternCp))
            {
                matchExact = true;
            }
            else if (hasExtension)
            {
                matchExact = CatchExtensionRegex.Match(patternCp).Groups[1].Length != 3;
            }
            string regexString = Regex.Escape(patternCp);
            regexString = "^" + Regex.Replace(regexString, @"\\\*", ".*");
            regexString = Regex.Replace(regexString, @"\\\?", ".");
            if (!matchExact && hasExtension)
            {
                regexString += NonDotCharacters;
            }
            regexString += "$";

            Regex regex = new Regex(regexString, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return regex;
        }
    }
}