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
using System.Linq;
using System.Text.RegularExpressions;
using NLog;
using Summer.Batch.Common.IO;
using Summer.Batch.Common.Settings;
using Summer.Batch.Core.Scope.Context;
using Summer.Batch.Infrastructure.Item;

namespace Summer.Batch.Extra.IO
{
    /// <summary>
    /// Resource loader that supports generation data groups (GDG) with the "gdg:" URI protocol.
    /// Other URIs or paths without protocol are delegated to <see cref="ResourceLoader"/>.
    /// 
    /// Generation data groups are supported with the following syntax:
    /// 
    /// <code>
    /// gdg:path/to/file(&lt;number&gt;)[.&lt;extension&gt;]
    /// </code>
    /// 
    /// &lt;number&gt; refers to a file in the group. The latest version existing before the job
    /// is start is version 0. The previous one is -1, and so on. The version that has being written
    /// in the current job can be referenced with 1 (or +1).
    /// 
    /// It is possible to limit the number of files in a group by using a setting named "gdg-options".
    /// This setting must be a string containing comma separated values with the following repeatable
    /// pattern:
    /// 
    /// <code>
    /// path/to/file.txt,limit=&lt;limit&gt;[,mode=&lt;mode&gt;]
    /// </code>
    /// 
    /// &lt;limit&gt; is the maximum number of files in the group and &lt;mode&gt; is either "empty"
    /// or "noempty". If no mode is specified, "noempty" is the default. In the "empty" mode, all
    /// previous files are moved when the limit is reached, whereas in "noempty" mode the oldest files
    /// are removed to keep the number of files at the limit.
    /// </summary>
    public class GdgResourceLoader : ResourceLoader
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private const int Max = 10000;
        private const string OptionsSetting = "gdg-options";
        private const string LimitOption = "limit=";
        private const string ModeOption = "mode=";
        private const string ContextSuffix = "_gdg";

        private static readonly string GdgProtocol = @"gdg" + Uri.SchemeDelimiter;

        private static readonly Regex GdgRegex =
            new Regex(@"^gdg:\/\/(?<fullPath>[^\(]*?(?<name>[\w-.]+))\((?<parameter>[+-]?\d+|\*)\)(?<extension>\.\w+)?$");

        private static readonly Regex GenerationNumbeRegex = new Regex(@"[\w-.]+G(?<G>\d{4})V(?<V>\d{2})(?:\.\w+)?");

        private readonly SettingsManager _settings;

        private readonly IDictionary<string, GdgOptions> _gdgOptions = new Dictionary<string, GdgOptions>();

        private readonly Dictionary<GdgPath, IDictionary<int, IResource>> _startingResources =
            new Dictionary<GdgPath, IDictionary<int, IResource>>();

        private ExecutionContext _jobContext;

        /// <summary>
        /// Default constructor. Checks the options in the settings manager.
        /// </summary>
        public GdgResourceLoader(SettingsManager settings)
        {
            _settings = settings;
            ParseOptions();
        }

        /// <summary>
        /// Destructor that deletes the obsolete files.
        /// </summary>
        ~GdgResourceLoader()
        {
            foreach (var resource in GetResourcesToDelete().Where(resource => resource.Exists()))
            {
                try
                {
                    resource.GetFileInfo().Delete();
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Could not delete GDG resource " + resource.GetDescription());
                }
            }
        }

        /// <summary>
        /// Resolves a single path to resources.
        /// </summary>
        /// <param name="path">the path to resolve</param>
        /// <returns>the matched resources</returns>
        protected override IEnumerable<IResource> DoGetResources(string path)
        {
            if (!path.StartsWith(GdgProtocol))
            {
                return base.DoGetResources(path);
            }
            var gdgPath = GetPath(path);
            return gdgPath.Wildcard
                ? GetResources(gdgPath)
                : new[] { GetGdgResource(gdgPath) };
        }

        // Gets a resource from a GDG path
        private IResource GetGdgResource(GdgPath path)
        {
            var number = GetGenerationNumber(path.FullPath);
            if (number == int.MinValue)
            {
                var resources = GetResourcesDictionary(path);
                number = GetGenerationNumber(resources);
                _startingResources[path] = resources;
                SaveGenerationNumber(path.FullPath, number);
            }
            var resource = GetResource(path, number);
            if (path.N <= 0 && !resource.Exists())
            {
                Logger.Warn("GDG resource accessed with negative reference does not exist: {0}({1}){2}",
                    path.FullPath, path.N, path.Extension);
            }
            return resource;
        }

        // Retrieves the current generation number from a collection of the existing resources in the group
        private int GetGenerationNumber(SortedDictionary<int, IResource> resources)
        {
            int number;
            if (!resources.Any())
            {
                number = 0;
            }
            else if (resources.ContainsKey(Max - 1) && resources.ContainsKey(0))
            {
                number = 0;
                while (resources.ContainsKey(number + 1))
                {
                    number++;
                }
            }
            else
            {
                number = resources.Last().Key;
            }
            return number;
        }

        // Retrieves the generation number of a path from the job context.
        private int GetGenerationNumber(string path)
        {
            var key = path + ContextSuffix;
            var jobContext = GetJobContext();
            return jobContext.ContainsKey(key)
                ? jobContext.GetInt(key)
                : int.MinValue;
        }

        // Saves the generation number in the job context.
        private void SaveGenerationNumber(string path, int number)
        {
            GetJobContext().PutInt(path + ContextSuffix, number);
        }

        // Retrieves the job context.
        private ExecutionContext GetJobContext()
        {
            if (_jobContext == null)
            {
                _jobContext = JobSynchronizationManager.GetContext().JobExecution.ExecutionContext;
            }
            return _jobContext;
        }

        // Retrieves a resource from its GDG path and number
        private IResource GetResource(GdgPath path, int currentNumber)
        {
            var number = currentNumber + path.N;
            if (number < 0)
            {
                number += Max;
            }
            if (number >= Max)
            {
                number -= Max;
            }
            return GetResource(string.Format("file://{0}G{1:0000}V00{2}", path.FullPath, number, path.Extension ?? ""));
        }

        // Retrieves all the existing resources for a GDG and puts them in a
        // sorted dictionary with the generation numbers as keys.
        private SortedDictionary<int, IResource> GetResourcesDictionary(GdgPath path)
        {
            var result = new SortedDictionary<int, IResource>();
            foreach (var resource in GetResources(path))
            {
                result[GetGenerationNumber(resource)] = resource;
            }
            return result;
        }

        // Retrieves existing resources for a GDG path.
        private IEnumerable<IResource> GetResources(GdgPath path)
        {
            return base.DoGetResources(string.Format("file://{0}G*V*{1}", path.FullPath, (path.Extension ?? "")));
        }

        // Extract the generation number from a file name.
        private int GetGenerationNumber(IResource resource)
        {
            var match = GenerationNumbeRegex.Match(resource.GetFilename());
            return match.Success
                ? int.Parse(match.Groups["G"].Value)
                : int.MinValue;
        }

        // Creates an instance of GdgPath from a gdg URI
        private static GdgPath GetPath(string path)
        {
            var match = GdgRegex.Match(path);
            if (!match.Success)
            {
                throw new ArgumentException("Invalid GDG path: " + path);
            }
            var name = match.Groups["name"].Value;
            var fullPath = match.Groups["fullPath"].Value;
            var extension = match.Groups["extension"].Value;
            var parameter = match.Groups["parameter"].Value;
            var n = parameter == "*" ? int.MaxValue : int.Parse(parameter);

            return new GdgPath(name, fullPath, extension, n);
        }

        // Parses the options in the settings
        private void ParseOptions()
        {
            var optionsSetting = _settings[OptionsSetting];
            if (optionsSetting != null)
            {
                var split = optionsSetting.Split(',');
                var i = 0;
                while (i < split.Length)
                {
                    var name = split[i++];
                    var limit = 0;
                    var empty = false;
                    while (i < split.Length)
                    {
                        if (split[i].StartsWith(LimitOption, StringComparison.OrdinalIgnoreCase))
                        {
                            limit = int.Parse(split[i++].Substring(LimitOption.Length));
                        }
                        else if (split[i].StartsWith(ModeOption, StringComparison.OrdinalIgnoreCase))
                        {
                            empty = string.Equals("empty", split[i++].Substring(ModeOption.Length),
                                StringComparison.OrdinalIgnoreCase);
                        }
                        else
                        {
                            break;
                        }
                    }
                    _gdgOptions.Add(name, new GdgOptions(limit, empty));
                }
            }
        }

        // Computes the resources that need to be deleted at the end of the job
        private IEnumerable<IResource> GetResourcesToDelete()
        {
            var toDelete = new HashSet<IResource>();
            foreach (var path in _startingResources.Keys)
            {
                GdgOptions options;
                if (_gdgOptions.TryGetValue(path.Name + "(*)" + path.Extension, out options))
                {
                    try
                    {
                        var resources = GetResources(path).ToArray();
                        var startingResources = _startingResources[path];
                        if (options.Empty && resources.Length > options.Limit)
                        {
                            foreach (var resource in startingResources.Values)
                            {
                                toDelete.Add(resource);
                            }
                        }
                        else if (!options.Empty)
                        {
                            foreach (var resource in GetResourcesToDelete(path, options, resources))
                            {
                                toDelete.Add(resource);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e, "Could not get resources for path : " + path.FullPath + path.Extension);
                    }
                }
            }
            return toDelete;
        }

        private IEnumerable<IResource> GetResourcesToDelete(GdgPath path, GdgOptions options, IEnumerable<IResource> resources)
        {
            var toDelete = new HashSet<IResource>();
            var startingResources = _startingResources[path];
            var newResources = resources.Except(startingResources.Values).ToList();
            foreach (var resource in newResources)
            {
                var numberToDelete = GetGenerationNumber(resource) - options.Limit;
                if (numberToDelete < 0)
                {
                    numberToDelete += Max;
                }
                if (startingResources.ContainsKey(numberToDelete))
                {
                    toDelete.Add(startingResources[numberToDelete]);
                }
            }
            return toDelete;
        }

        #region Internal structs

        private struct GdgPath
        {
            private readonly string _name;
            private readonly string _fullPath;
            private readonly string _extension;
            private readonly int _n;

            public GdgPath(string name, string fullPath, string extension, int n)
            {
                _name = name;
                _fullPath = fullPath;
                _extension = extension;
                _n = n;
            }

            public string Name
            {
                get { return _name; }
            }

            public string FullPath
            {
                get { return _fullPath; }
            }

            public string Extension
            {
                get { return _extension; }
            }

            public int N
            {
                get { return _n; }
            }

            public bool Wildcard
            {
                get { return _n == int.MaxValue; }
            }
        }

        private struct GdgOptions
        {
            private readonly int _limit;
            private readonly bool _empty;

            public GdgOptions(int limit, bool empty)
            {
                _limit = limit;
                _empty = empty;
            }

            public int Limit
            {
                get { return _limit; }
            }

            public bool Empty
            {
                get { return _empty; }
            }
        }

        #endregion
    }
}