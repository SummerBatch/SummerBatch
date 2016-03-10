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
using System.Collections.Generic;

namespace Summer.Batch.Infrastructure.Item
{
    /// <summary>
    /// Basic interface for generic output operations. Class implementing this
    /// interface will be responsible for serializing objects as necessary.
    /// Generally, it is responsibility of implementing class to decide which
    /// technology to use for mapping and how it should be configured.
    ///
    /// The write method is responsible for making sure that any internal buffers are
    /// flushed. If a transaction is active it will also usually be necessary to
    /// discard the output on a subsequent rollback. The resource to which the writer
    /// is sending data should normally be able to handle this itself.
    /// </summary>
    /// <typeparam name="T">&nbsp;</typeparam>
    public interface IItemWriter<T> where T:class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <exception cref="Exception">&nbsp;</exception>
        void Write(IList<T> items);
    }
}