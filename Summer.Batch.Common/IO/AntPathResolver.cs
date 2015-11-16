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
 * Copyright 2002-2013 the original author or authors.
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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NLog;

namespace Summer.Batch.Common.IO
{
    /// <summary>
    /// Utility class to resolve Ant style paths as resources.
    /// 
    /// The mapping matches resources with the following rules:
    /// 
    /// <list type="table">
    ///     <item>
    ///         <term>?</term>
    ///         <description>matches one character</description>
    ///     </item>
    ///     <item>
    ///         <term>*</term>
    ///         <description>matches zero or more character in a directory name</description>
    ///     </item>
    ///     <item>
    ///         <term>**</term>
    ///         <description>matches zero or more directories in a path</description>
    ///     </item>
    /// </list>
    /// </summary>
    public class AntPathResolver
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly string PathSeparator = Path.DirectorySeparatorChar.ToString();

        private static readonly Regex DirRegex = new Regex(@"\?|\*|\.");

        private readonly IDictionary<string, Regex> _dirRegexCache = new Dictionary<string, Regex>();

        /// <summary>
        /// Checks if a path contains a wildcard.
        /// </summary>
        /// <param name="path">a path describing resources</param>
        /// <returns>true if the path is a pattern with a wildcard; false otherwise</returns>
        public static bool IsPattern(string path)
        {
            return path.IndexOf('*') != -1 || path.IndexOf('?') != -1;
        }

        /// <summary>
        /// Resolves the pattern and gets all the existing resources that match it.
        /// </summary>
        /// <param name="pattern">a pattern matching resources</param>
        /// <returns>the matched resources</returns>
        public IEnumerable<IResource> FindMatchingResources(string pattern)
        {
            pattern = pattern.Replace('/', Path.DirectorySeparatorChar);
            var rootDir = GetRootDir(pattern);
            if (!Directory.Exists(rootDir))
            {
                // Ignore non-existing directories
                Logger.Debug("Skipping [{0}] because it does not exist.", rootDir);
                return new List<IResource>();
            }
            var result = new HashSet<IResource>();
            RetrieveMatchingFiles(pattern, rootDir, result);
            return result;
        }

        private void RetrieveMatchingFiles(string pattern, string directory, ISet<IResource> resources)
        {
            Logger.Debug("Searching directory [{0}] for files matching pattern [{1}]", directory, pattern);
            foreach (var file in Directory.GetFiles(directory).Where(f => Matches(pattern, f)))
            {
                resources.Add(new FileSystemResource(file));
            }
            foreach (var subDirectory in Directory.GetDirectories(directory).Where(d => MatchesStart(pattern, d)))
            {
                RetrieveMatchingFiles(pattern, subDirectory, resources);
            }
        }

        private string GetRootDir(string path)
        {
            var rootDirEnd = path.Length;
            while (rootDirEnd > 0 && IsPattern(path.Substring(0, rootDirEnd)))
            {
                rootDirEnd = path.LastIndexOf(Path.DirectorySeparatorChar, rootDirEnd - 2) + 1;
            }
            return path.Substring(0, rootDirEnd);
        }

        #region Matches methods

        private bool MatchesStart(string pattern, string path)
        {
            return Matches(pattern, path, false);
        }

        private bool Matches(string pattern, string path, bool fullMatch = true)
        {
            if (pattern.StartsWith(PathSeparator) != path.StartsWith(PathSeparator))
            {
                return false;
            }

            var patternDirs = TokenizePath(pattern).ToArray();
            var pathDirs = TokenizePath(path).ToArray();

            var patternIndexStart = 0;
            var patternIndexEnd = patternDirs.Length - 1;
            var pathIndexStart = 0;
            var pathIndexEnd = pathDirs.Length - 1;

            // Match all elements up to the first '**'
            while (patternIndexStart <= patternIndexEnd && pathIndexStart <= pathIndexEnd)
            {
                var patternDir = patternDirs[patternIndexStart];
                if (patternDir == "**")
                {
                    break;
                }
                if (!MatchesDirectory(patternDir, pathDirs[pathIndexStart]))
                {
                    return false;
                }
                patternIndexStart++;
                pathIndexStart++;
            }

            if (pathIndexStart > pathIndexEnd)
            {
                // Path is exhausted, only match if rest of pattern is * or **'s
                if (patternIndexStart > patternIndexEnd)
                {
                    return (pattern.EndsWith(PathSeparator)
                        ? path.EndsWith(PathSeparator)
                        : !path.EndsWith(PathSeparator));
                }
                if (!fullMatch)
                {
                    return true;
                }
                if (patternIndexStart == patternIndexEnd && patternDirs[patternIndexStart] == "*" &&
                    path.EndsWith(PathSeparator))
                {
                    return true;
                }
                for (var i = patternIndexStart; i <= patternIndexEnd; i++)
                {
                    if (patternDirs[i] != "**")
                    {
                        return false;
                    }
                }
                return true;
            }
            if (patternIndexStart > patternIndexEnd)
            {
                // Path not exhausted, but pattern is. Failure.
                return false;
            }
            if (!fullMatch && patternDirs[patternIndexStart] == "**")
            {
                // Path start definitely matches due to "**" part in pattern.
                return true;
            }

            // up to last '**'
            while (patternIndexStart <= patternIndexEnd && pathIndexStart <= pathIndexEnd)
            {
                var patternDir = patternDirs[patternIndexEnd];
                if (patternDir == "**")
                {
                    break;
                }
                if (!MatchesDirectory(patternDir, pathDirs[pathIndexEnd]))
                {
                    return false;
                }
                patternIndexEnd--;
                pathIndexEnd--;
            }

            if (pathIndexStart > pathIndexEnd)
            {
                // Path is exhausted
                for (var i = patternIndexStart; i <= patternIndexEnd; i++)
                {
                    if (patternDirs[i] != "**")
                    {
                        return false;
                    }
                }
                return true;
            }

            while (patternIndexStart != patternIndexEnd && pathIndexStart <= pathIndexEnd)
            {
                var patternIndexTemp = -1;
                for (var i = patternIndexStart + 1; i <= patternIndexEnd; i++)
                {
                    if (patternDirs[i] == "**")
                    {
                        patternIndexTemp = i;
                        break;
                    }
                }
                if (patternIndexTemp == patternIndexStart + 1)
                {
                    // '**/**' situation, so skip one
                    patternIndexStart++;
                    continue;
                }
                // Find the pattern between pathIndexStart & patternIndexTemp in path between
                // pathIndexStart & pathIndexEnd
                var patternLength = (patternIndexTemp - patternIndexStart - 1);
                var pathLength = (pathIndexEnd - pathIndexStart + 1);
                var foundIndex = -1;

                for (var i = 0; i <= pathLength - patternLength; i++)
                {
                    for (var j = 0; j < patternLength; j++)
                    {
                        var subPattern = patternDirs[patternIndexStart + j + 1];
                        var subPath = pathDirs[pathIndexStart + i + j];
                        if (!MatchesDirectory(subPattern, subPath))
                        {
                            goto strLoop;
                        }
                    }
                    foundIndex = pathIndexStart + i;
                    break;
                    strLoop:
                    ;
                }

                if (foundIndex == -1)
                {
                    return false;
                }

                patternIndexStart = patternIndexTemp;
                pathIndexStart = foundIndex + patternLength;
            }

            for (var i = patternIndexStart; i <= patternIndexEnd; i++)
            {
                if (patternDirs[i] != "**")
                {
                    return false;
                }
            }

            return true;
        }

        private bool MatchesDirectory(string pattern, string directory)
        {
            Regex dirRegex;
            if (!_dirRegexCache.TryGetValue(pattern, out dirRegex))
            {
                var replacedPattern = DirRegex.Replace(pattern, m =>
                {
                    switch (m.Value)
                    {
                        case "?":
                            return ".";
                        case "*":
                            return ".*";
                        default:
                            return @"\.";
                    }
                });
                dirRegex = new Regex("^(" + replacedPattern + ")$");
                _dirRegexCache.Add(pattern, dirRegex);
            }
            return dirRegex.IsMatch(directory);
        }

        #endregion

        private static IEnumerable<string> TokenizePath(string path)
        {
            var chars = path.ToCharArray();
            var current = 0;
            for (var i = 0; i < chars.Length; i++)
            {
                if (chars[i] == Path.DirectorySeparatorChar)
                {
                    yield return new string(chars, current, i - current);
                    current = i + 1;
                }
            }
            if (current < chars.Length - 1)
            {
                yield return new string(chars, current, chars.Length - current);
            }
        }
    }
}