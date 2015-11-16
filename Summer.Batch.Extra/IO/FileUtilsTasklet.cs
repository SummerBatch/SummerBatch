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
using System.Diagnostics;
using System.IO;
using System.Linq;
using NLog;
using Summer.Batch.Common.Factory;
using Summer.Batch.Common.IO;
using Summer.Batch.Common.Util;
using Summer.Batch.Core;
using Summer.Batch.Core.Scope.Context;
using Summer.Batch.Core.Step.Tasklet;
using Summer.Batch.Infrastructure.Item.Util;
using Summer.Batch.Infrastructure.Repeat;

namespace Summer.Batch.Extra.IO
{
    /// <summary>
    /// A tasklet for basic file manipulations.
    /// 
    /// The operation is specified with the <see cref="Mode"/> property. Possible values are:
    /// 
    /// <list type="bullet">
    ///     <item>
    ///         <term>COPY</term>
    ///         <description>copies file to another location</description>
    ///     </item>
    ///     <item>
    ///         <term>DELETE</term>
    ///         <description>deletes a list of files</description>
    ///     </item>
    ///     <item>
    ///         <term>MERGE</term>
    ///         <description>appends the source files to the target file</description>
    ///     </item>
    ///     <item>
    ///         <term>MERGE_COPY</term>
    ///         <description>merges the source files to the target file (which is overwritten if it exists)</description>
    ///     </item>
    ///     <item>
    ///         <term>RESET</term>
    ///         <description>Creates new empty files, overwriting any existing file</description>
    ///     </item>
    /// </list>
    /// 
    /// </summary>
    public class FileUtilsTasklet : ITasklet, IInitializationPostOperations
    {
        /// <summary>
        /// Enumeration of the possible file operations.
        /// </summary>
        public enum FileUtilsMode
        {
            /// <summary>
            /// Copy operation.
            /// </summary>
            Copy,
            /// <summary>
            /// Delete operation.
            /// </summary>
            Delete,
            /// <summary>
            /// Merge operation (sources files are appended to the target file).
            /// </summary>
            Merge,
            /// <summary>
            /// Merges the sources file and writes the result to the target file (which is overwritten).
            /// </summary>
            MergeCopy,
            /// <summary>
            /// Resets a file (the file is emptied).
            /// </summary>
            Reset
        };

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private bool _strict = true;

        /// <summary>
        /// Strict mode flag property.
        /// </summary>
        public bool Strict
        {
            get { return _strict; }
            set { _strict = value; }
        }

        /// <summary>
        /// FileUtilsMode property.
        /// </summary>
        public FileUtilsMode Mode { private get; set; }

        /// <summary>
        /// List of source resources property.
        /// </summary>
        public IList<IResource> Sources { private get; set; }

        /// <summary>
        /// List of target resources property.
        /// </summary>
        public IList<IResource> Targets { private get; set; }

        /// <summary>
        /// @see ITasklet#Execute()
        /// </summary>
        /// <param name="contribution"></param>
        /// <param name="chunkContext"></param>
        /// <returns></returns>
        public RepeatStatus Execute(StepContribution contribution, ChunkContext chunkContext)
        {
            switch (Mode)
            {
                case FileUtilsMode.Copy:
                    Copy();
                    break;
                case FileUtilsMode.Delete:
                    foreach (IResource target in Targets)
                    {
                        if (target.Exists())
                        {
                            Delete(target);
                        }
                        else
                        {
                            Error(target);
                        }
                    }
                    break;
                case FileUtilsMode.Merge:
                    Merge(true);
                    break;
                case FileUtilsMode.MergeCopy:
                    Merge(false);
                    break;
                case FileUtilsMode.Reset:
                    Reset();
                    break;
                default:
                    throw new InvalidOperationException("This mode is not supported :[" + Mode + "]");
            }
            return RepeatStatus.Finished;
        }

        /// <summary>
        /// Do Reset
        /// </summary>
        private void Reset()
        {
            foreach (IResource target in Targets)
            {
                if (target.Exists())
                {
                    target.GetFileInfo().Open(FileMode.Truncate).Close();
                }
                else
                {
                    target.GetFileInfo().OpenWrite().Close();
                }
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Resetted file [{0}]", target.GetFullPath());
                }
            }
        }

        /// <summary>
        /// Do Delete
        /// </summary>
        /// <param name="target"></param>
        private static void Delete(IResource target)
        {
            //if Directory
            if (target.GetFileInfo().Attributes.HasFlag(FileAttributes.Directory))
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Deleting recursively folder [{0}]", target.GetFullPath());
                }
                Stopwatch sw = Stopwatch.StartNew();
                Directory.Delete(target.GetFullPath(), true);
                sw.Stop();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("[{0}] Folder deleted in {1} ms", target.GetFullPath(), sw.ElapsedMilliseconds);
                }
            }
            else // normal file
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Deleting file [{0}]", target.GetFullPath());
                }
                File.Delete(target.GetFullPath());
            }
        }

        /// <summary>
        /// Merge
        /// </summary>
        /// <param name="append"></param>
        private void Merge(bool append)
        {
            FileMode fm = FileMode.Create;
            if (append)
            {
                fm = FileMode.Append;
            }
            bool sourceIsTarget = (Sources.Contains(Targets[0]));
            string tmpFileName = Path.GetTempFileName(); 

            // if source is target, use a temporary file that will be appended in the end. Otherwise, just use the given target.
            using (
                FileStream fs = sourceIsTarget
                    ? new FileStream(tmpFileName, FileMode.Create)
                    : Targets[0].GetFileInfo().Open(fm, FileAccess.Write))
            {
                foreach (var source in Sources)
                {
                    if (source.Exists())
                    {
                        using (Stream inStream = source.GetInputStream())
                        {
                            inStream.CopyTo(fs);
                        }
                    }
                    else
                    {
                        Error(source);
                    }
                }
            }
            if (sourceIsTarget)
            {
                //append tmpFile to existing target
                using(FileStream fsmerged = File.Open(Targets[0].GetFullPath(), fm, FileAccess.Write))
                using (Stream tmpStream = new FileInfo(tmpFileName).OpenRead())
                {
                    tmpStream.CopyTo(fsmerged);
                }
                File.Delete(tmpFileName);
            }
        }

        /// <summary>
        /// do copy
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void Copy()
        {
            if (Sources.Count() == 1)
            {
                IResource source = Sources[0];
                if (source.GetFileInfo().Attributes.HasFlag(FileAttributes.Directory))
                {
                    if (!Targets[0].GetFileInfo().Attributes.HasFlag(FileAttributes.Directory))
                    {
                        throw new Exception("Target must be a directory.");
                    }
                    else
                    {
                        //copy dir
                        FileUtils.CopyDir(source.GetFullPath(), Targets[0].GetFullPath());
                    }
                }
                else if (source.GetFileInfo().Attributes.HasFlag(FileAttributes.Normal))
                {
                    File.Copy(source.GetFullPath(), Targets[0].GetFullPath());
                }
                else
                {
                    Error(source);
                }
            }
            else
            {
                if (!Targets[0].GetFileInfo().Attributes.HasFlag(FileAttributes.Directory))
                {
                    throw new Exception("Target must be a directory.");
                }
                foreach (IResource source in Sources)
                {
                    if (source.Exists())
                    {
                        string targetName = Path.Combine(Targets[0].GetFileInfo().DirectoryName, source.GetFilename());             
                        File.Copy(source.GetFullPath(), targetName);
                    }
                    else
                    {
                        Error(source);
                    }
                }
            }
        }

        /// <summary>
        /// Check proper properties initialization, depending on selected mode.
        /// @see IInitializationPostOperations#AfterPropertiesSet()
        /// </summary>
        public void AfterPropertiesSet()
        {
            Assert.NotNull(Mode, "Mode should be specified");
            switch (Mode)
            {
                case FileUtilsMode.Copy:
                case FileUtilsMode.Merge:
                case FileUtilsMode.MergeCopy:
                    Assert.State(Targets != null && Targets.Count == 1, "one and only one target must be specified.");
                    if (Strict)
                    {
                        Assert.NotEmpty(Sources, "STRICT MODE : at least one source must be specified.");
                    }
                    else if (!Sources.Any())
                    {
                        Logger.Warn("no sources specified.");
                    }
                    break;
                case FileUtilsMode.Delete:
                case FileUtilsMode.Reset:
                    if (Strict)
                    {
                        Assert.NotEmpty(Targets, "STRICT MODE : at least one target must be specified.");
                    }
                    else if (!Targets.Any())
                    {
                        Logger.Warn("no targets specified.");
                    }
                    break;
                default:
                    throw new InvalidOperationException("This mode is not supported :["+Mode+"]");
            }
        }

        /// <summary>
        /// log error or throw exception, depending on Strict attribute value
        /// </summary>
        /// <param name="resource"></param>
        private void Error(IResource resource)
        {
            string message = resource.GetDescription() + " does not exist.";
            if (Strict)
            {
                throw new Exception(message);
            }
            else
            {
                Logger.Warn(message);
            }
        }
    }
}