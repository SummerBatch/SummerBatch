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
using System.IO;
using NLog;
using Summer.Batch.Common.Util;

namespace Summer.Batch.Common.IO
{
    /// <summary>
    /// Implementation of <see cref="T:IResource"/> for a file in the file system.
    /// </summary>
    public class FileSystemResource : AbstractResource
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        /// <summary>
        /// Path of the file system resource.
        /// </summary>
        private readonly string _path;

        /// <summary>
        /// Constructs a new <see cref="T:FileSystemResource"/> for the given file info.
        /// </summary>
        /// <param name="fileInfo">a file info</param>
        public FileSystemResource(FileInfo fileInfo)
        {
            Assert.NotNull(fileInfo, "File info must not be null");
            _path = fileInfo.FullName;
        }

        /// <summary>
        /// Returns the full path for this resource.
        /// </summary>
        /// <returns></returns>
        public override string GetFullPath()
        {
            return Path.GetFullPath(_path);
        }

        /// <summary>
        /// Constructs a new <see cref="T:FileSystemResource"/> for the given path.
        /// </summary>
        /// <param name="path">the path to the file to reference</param>
        public FileSystemResource(string path)
        {
            Assert.NotNull(path, "Path must not be null");
            _path = path;
        }

        /// <summary>
        /// Checks if a resource actually exists.
        /// </summary>
        /// <returns><c>true</c> if the underlying resource exists, <c>false</c> otherwise</returns>
        public override bool Exists()
        {
            return (File.Exists(_path) || Directory.Exists(_path));
        }

        /// <summary>
        /// Opens a new read <see cref="T:System.IO.Stream"/> for the underlying resource. Each call is expected
        /// to return a new instance.
        /// </summary>
        /// <returns>a <see cref="T:System.IO.Stream"/> for reading the underlying resource (cannot be null)</returns>
        /// <exception cref="T:System.IO.IOException">if the stream cannot be open</exception>
        public override Stream GetInputStream()
        {
            Logger.Debug("Getting input stream for resource: {0}", _path);
            return File.Open(_path,FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            
        }

        /// <returns>a <see cref="T:System.Uri"/> for this resource</returns>
        public override Uri GetUri()
        {
            return new Uri(Path.GetFullPath(_path));
        }

        /// <returns>a <see cref="T:System.IO.FileInfo"/> for this resource</returns>
        public override FileInfo GetFileInfo()
        {
            return new FileInfo(_path);
        }

        /// <summary>
        /// Determines the filename for this resource.
        /// </summary>
        /// <returns>the filename for this resource, or <c>null</c> if it does not have one</returns>
        public override string GetFilename()
        {
            return Path.GetFileName(_path);            
        }

        /// <summary>
        /// Determines when this resource was last modified.
        /// </summary>
        /// <returns>a <see cref="T:System.DateTime"/> for the last modification</returns>
        /// <exception cref="T:System.IO.IOException">if the resource cannot be resolved</exception>
        public override DateTime GetLastModified()
        {
            return File.GetLastWriteTime(_path);
        }

        /// <summary>
        /// Computes a literal description for this resource.
        /// Implementations are encouraged to return this value for their ToString() method.
        /// </summary>
        /// <returns>a string describing the resource</returns>
        public override string GetDescription()
        {
            return string.Format("File[{0}]", Path.GetFullPath(_path));
        }

        /// <summary>
        /// Creates a new resource relative to this resource.
        /// </summary>
        /// <param name="relativePath">a path relative to this resource</param>
        /// <returns>a resource for the given path</returns>
        public override IResource CreateRelative(string relativePath)
        {
            return new FileSystemResource(Path.Combine(_path, relativePath));
        }
    }
}