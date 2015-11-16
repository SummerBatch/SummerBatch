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
    /// Base implementations for all concrete <see cref="T:Summer.Batch.IO.IResource"/> implementations.
    /// </summary>
    public abstract class AbstractResource : IResource
    {
        /// <summary>
        /// Returns the input stream.
        /// </summary>
        /// <returns></returns>
        public abstract Stream GetInputStream();

        /// <summary>
        /// Test if resource exists.
        /// </summary>
        /// <returns></returns>
        public abstract bool Exists();

        /// <summary>
        /// Returns the resource uri.
        /// </summary>
        /// <returns></returns>
        public virtual Uri GetUri()
        {
            throw new NotSupportedException(string.Format("{0} cannot be resolved to a URI", GetDescription()));
        }

        /// <summary>
        /// Returns the resource FileInfo.
        /// </summary>
        /// <returns></returns>
        public virtual FileInfo GetFileInfo()
        {
            throw new FileNotFoundException(string.Format("{0} cannot be resolved to a file", GetDescription()));
        }

        /// <summary>
        /// Returns the resource full path.
        /// </summary>
        /// <returns></returns>
        public abstract string GetFullPath();
        
        /// <summary>
        /// Returns the resource's last modified time.
        /// </summary>
        /// <returns></returns>
        public abstract DateTime GetLastModified();

        /// <summary>
        /// Returns the resource file name.
        /// </summary>
        /// <returns></returns>
        public virtual string GetFilename()
        {
            return null;
        }

        /// <summary>
        /// Returns the resource description.
        /// </summary>
        /// <returns></returns>
        public abstract string GetDescription();

        /// <summary>
        /// Creates the resource for given relative path.
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public virtual IResource CreateRelative(string relativePath)
        {
            throw new IOException(string.Format("Cannot create a relative resource for {0}", GetDescription()));
        }

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return GetDescription();
        }

        /// <summary>
        /// Equals override.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj == this || (obj is IResource && ((IResource)obj).GetDescription() == GetDescription());
        }

        /// <summary>
        /// GetHashCode override.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return GetDescription().GetHashCode();
        }
    }
}