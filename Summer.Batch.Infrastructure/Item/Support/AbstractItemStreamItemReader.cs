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
using Summer.Batch.Infrastructure.Item.Util;

namespace Summer.Batch.Infrastructure.Item.Support
{
    /// <summary>
    /// Base class for <see cref="T:IItemReader"/> implementations.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbstractItemStreamItemReader<T> : ItemStreamSupport, IItemStreamReader<T> where T : class
    {
        /// <summary>
        /// Reads a piece of input data and advance to the next one. Implementations
        /// <strong>must</strong> return <code>null</code> at the end of the input
        /// data set. In a transactional setting, caller might get the same item
        /// twice from successive calls (or otherwise), if the first call was in a
        /// transaction that rolled back.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="UnexpectedInputException"></exception>
        /// <exception cref="NonTransientResourceException"></exception>
        /// <exception cref="ParseException"></exception>
        public abstract T Read();
    }
}