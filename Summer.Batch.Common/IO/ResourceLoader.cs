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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NLog;

namespace Summer.Batch.Common.IO
{
    /// <summary>
    /// Class responsible for loading resources. Can be extended to change or expand the resolving mechanism.
    /// 
    /// This base implementation only supports file system resources.
    /// A path can start with the file URI protocol ('file://'), or with no URI protocol at all.
    /// 
    /// Several paths can be resolved at the same time, by separating them using <see cref="Path.PathSeparator"/>.
    /// Paths can also contain the '?', '*', and '**' wildcards (see <see cref="AntPathResolver"/>).
    /// </summary>
    public class ResourceLoader
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly Regex UriRegex = new Regex(@"^(?<scheme>\w+)://(?<path>.+)");

        private readonly AntPathResolver _antPathResolver = new AntPathResolver();

        /// <summary>
        /// Resolves a path as a single resource. If the path matches several resources, one is returned arbitrarily.
        /// </summary>
        /// <param name="path">the path to resolve</param>
        /// <returns>the resolved resource, or null if no resources were matched</returns>
        public IResource GetResource(string path)
        {
            var resources = GetResources(path);
            if (resources.Count > 1)
            {
                Logger.Warn("Several resources were matched but only one was expected: {0}", path);
            }
            return resources.Count == 0 ? null : resources[0];
        }

        /// <summary>
        /// Resolves a path and returns all the matched resources.
        /// </summary>
        /// <param name="paths">the path to resolve</param>
        /// <returns>the resolved resources</returns>
        public IList<IResource> GetResources(string paths)
        {
            var resources = new List<IResource>();
            if (!string.IsNullOrWhiteSpace(paths))
            {
                foreach (var path in paths.Split(Path.PathSeparator).Select(s => s.Trim()))
                {
                    resources.AddRange(DoGetResources(path));
                }
            }
            return resources;
        }

        /// <summary>
        /// Resolves a single path to resources.
        /// </summary>
        /// <param name="path">the path to resolve</param>
        /// <returns>the matched resources</returns>
        protected virtual IEnumerable<IResource> DoGetResources(string path)
        {
            string aPath = path;
            Logger.Debug("Getting resource for path: {0}", aPath);
            var uriMatch = UriRegex.Match(aPath);
            if (uriMatch.Success)
            {
                var uriScheme = uriMatch.Groups["scheme"].Value;
                if (uriScheme == Uri.UriSchemeFile)
                {
                    var uriPath = uriMatch.Groups["path"].Value;
                    aPath = uriPath.StartsWith("/")
                        ? new Uri(aPath).AbsolutePath
                        : uriPath;
                }
                else
                {
                    throw new ArgumentException(string.Format("Unsupported URI scheme: {0}" , uriScheme));
                }
            }
            return AntPathResolver.IsPattern(aPath)
                ? _antPathResolver.FindMatchingResources(aPath)
                : new IResource[] { new FileSystemResource(aPath) };
        }
    }
}