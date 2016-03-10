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
using Summer.Batch.Common.IO;

namespace Summer.Batch.Infrastructure.Item.File
{
    /// <summary>
    /// Interface for <see cref="T:IItemWriter"/>s that implement <see cref="IItemStream"/> and write
    /// to a <see cref="Summer.Batch.Common.IO.IResource"/>.
    /// </summary>
    /// <typeparam name="T">&nbsp;</typeparam>
    public interface IResourceAwareItemWriterItemStream<T> : IItemStreamWriter<T> where T:class
    {
        /// <summary>
        /// The <see cref="T:Summer.Batch.Common.IO.IResource"/> to write to.
        /// </summary>
        IResource Resource { set; }
    }
}