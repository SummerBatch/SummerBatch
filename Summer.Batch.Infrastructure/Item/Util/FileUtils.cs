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
using System.Diagnostics;
using Summer.Batch.Common.Util;
using System.IO;
using System.Linq;
using NLog;

namespace Summer.Batch.Infrastructure.Item.Util
{
    /// <summary>
    /// File operations helper.
    /// </summary>
    public static class FileUtils
    {

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Sets up an output file for batch processing. This method implements common logic for handling output files
        ///  when starting or restarting file I/O. When starting output file processing, creates/overwrites new file.
        ///  When restarting output file processing, checks whether file is writable.
        /// </summary>
        /// <param name="file">the FileInfo for the file to set up</param>
        /// <param name="restarted">whether the file processing is restarting</param>
        /// <param name="append">whether the output must be append to the file if it already exists</param>
        /// <param name="overwrite">whether the file should be overwritten if it exists (ignored during restart)</param>
        public static void SetUpOutputFile(FileInfo file, bool restarted, bool append, bool overwrite)
            {
            Assert.NotNull(file, "The file must be specified.");

            if (!restarted)
            {
                if (!append)
                {
                    if (file.Exists)
                    {
                        if (!overwrite)
                        {
                            throw new ItemStreamException(string.Format("File already exists: {0}",file.FullName));
                        }
                        file.Delete();
                        file.Refresh();
                    }
                    CreateFileAndDirectoryIfNeeded(file);                    
                }
                else
                {
                    if (!file.Exists)
                    {
                        CreateFileAndDirectoryIfNeeded(file);
                    }
                }
            }
            if (file.IsReadOnly)
            {
                throw new ItemStreamException(string.Format("File is not writable: {0}", file.FullName));
            }
        }

        /// <summary>
        /// Create file and owning directory if needed.
        /// </summary>
        /// <param name="file"></param>
        private static void CreateFileAndDirectoryIfNeeded(FileInfo file)
        {
            if (file == null)
            {
                throw new InvalidOperationException("Given FileInfo is null !!");
            }
            else
            {
                if (file.Directory!=null && ! file.Directory.Exists)
                {
                    file.Directory.Create();
                }
                if (!CreateNewFile(file))
                {
                    throw new ItemStreamException(string.Format("Output file was not created: {0}",file.FullName));
                }
            }
        }

        /// <summary>
        /// Crates a new empty file if it does not already exist.
        /// </summary>
        /// <param name="file">the FileInfo for the file to create</param>
        /// <returns>true if the file was actually created, false otherwise</returns>
        public static bool CreateNewFile(FileInfo file)
        {
            if (file.Exists)
            {
                return false;
            }
            file.Create().Close();
            file.Refresh();
            return file.Exists;
        }

        /// <summary>
        /// Copy a directory to another directory.
        /// 
        /// </summary>
        /// <param name="sourceDirectory"></param>
        /// <param name="targetDirectory"></param>
        public static void CopyDir(string sourceDirectory, string targetDirectory)
        {
            long bytecounter = 0;
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);
            Stopwatch sw = Stopwatch.StartNew();
            bytecounter=CopyAll(diSource, diTarget,bytecounter,false);
            sw.Stop();
            //evaluate in Mo
            double moTransfered = bytecounter/(1024.0*1024.0);
            //evaluate in seconds
            double elapsedInsec = sw.Elapsed.TotalSeconds;
            //evaluate in Mo per second
            double throughput = moTransfered/elapsedInsec;
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Copydir took {0:F3} s; transfered {1:F3} Mo of data; throughput = {2:F3} Mo/s",
                    elapsedInsec,moTransfered,throughput);
            }
        }

        /// <summary>
        /// Copy a directory to another directory. Recursive.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="byteCounter"></param>
        /// <param name="resetCounter"></param>
        public static long CopyAll(DirectoryInfo source, DirectoryInfo target, long byteCounter, bool resetCounter)
        {
            var lByteCounter = byteCounter;
            if (resetCounter)
            {
                lByteCounter = 0;
            }
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Copy all from {0} to {1}",source.FullName,target.FullName);
            }

            // Check if the target directory exists; if not, create it.
            if (!Directory.Exists(target.FullName))
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Target directory {0} does not exist; Creating it.",target.FullName);
                }
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Copying file {0}/{1} to {2}/{3}",source.FullName,fi.Name,target.FullName,fi.Name);
                }                
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
                lByteCounter += fi.Length;
            }

            // Copy each subdirectory using recursion.
            lByteCounter += (from diSourceSubDir in source.GetDirectories() let nextTargetSubDir = 
                                 target.CreateSubdirectory(diSourceSubDir.Name) select CopyAll(diSourceSubDir, nextTargetSubDir, byteCounter, true)).Sum();

            return lByteCounter;
        }
    }
}