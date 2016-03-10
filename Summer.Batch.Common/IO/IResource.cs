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

namespace Summer.Batch.Common.IO
{
    /// <summary>
    /// Interface for a resource descriptors that abstracts from the actual type of the underlying resource.
    /// 
    /// All resource types can provide a reading <see cref="T:System.IO.Stream"/>, but <see cref="T:System.URI"/> or
    /// <see cref="T:System.IO.FileInfo"/> may not be available for some resource types.
    /// </summary>
    public interface IResource
    {
        /// <summary>
        /// Opens a new read <see cref="T:System.IO.Stream"/> for the underlying resource. Each call is expected
        /// to return a new instance.
        /// </summary>
        /// <returns>a <see cref="T:System.IO.Stream"/> for reading the underlying resource (cannot be null)</returns>
        /// <exception cref="T:System.IO.IOException">if the stream cannot be open</exception>
        Stream GetInputStream();

        /// <summary>
        /// Checks if a resource actually exists.
        /// </summary>
        /// <returns><c>true</c> if the underlying resource exists, <c>false</c> otherwise</returns>
        bool Exists();
        
        /// <returns>a <see cref="T:System.Uri"/> for this resource</returns>
        /// <exception cref="T:System.NotSupportedException">if the resource cannot be resolved as a URI</exception>
        Uri GetUri();

        /// <returns>a <see cref="T:System.IO.FileInfo"/> for this resource</returns>
        /// <exception cref="T:System.IO.FileNotFoundException">if the resource cannot be resolved as a file</exception>
        FileInfo GetFileInfo();

        /// <summary>
        /// returns the full path for this resource
        /// </summary>
        /// <returns></returns>
        string GetFullPath();

        /// <summary>
        /// Determines when this resource was last modified.
        /// </summary>
        /// <returns>a <see cref="T:System.DateTime"/> for the last modification</returns>
        /// <exception cref="T:System.IO.IOException">if the resource cannot be resolved</exception>
        DateTime GetLastModified();

        /// <summary>
        /// Determines the filename for this resource.
        /// </summary>
        /// <returns>the filename for this resource, or <c>null</c> if it does not have one</returns>
        string GetFilename();

        /// <summary>
        /// Computes a literal description for this resource.
        /// Implementations are encouraged to return this value for their ToString() method.
        /// </summary>
        /// <returns>a string describing the resource</returns>
        string GetDescription();

        /// <summary>
        /// Creates a new resource relative to this resource.
        /// </summary>
        /// <param name="relativePath">a path relative to this resource</param>
        /// <returns>a resource for the given path</returns>
        /// <exception cref="T:System.IO.IOException">if the relative resource cannot be determined</exception>
        IResource CreateRelative(string relativePath);
    }
}