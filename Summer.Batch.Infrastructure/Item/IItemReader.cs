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

namespace Summer.Batch.Infrastructure.Item
{
    /// <summary>
    /// Strategy interface for providing the data. 
    /// Implementations are expected to be stateful and will be called multiple times
    /// for each batch, with each call to #Read() returning a different value
    /// and finally returning <c>null</c> when all input data is exhausted.
    /// Implementations need <b>not</b> be thread-safe and clients of a IItemReader
    /// need to be aware that this is the case.
    /// A richer interface (e.g. with a look ahead or peek) is not feasible because
    /// we need to support transactions in an asynchronous batch.
    /// </summary>
    /// <typeparam name="T">&nbsp;The type of the read items. Must be nullable.</typeparam>
    public interface IItemReader<out T> where T : class
    {
        /// <summary>
        /// Reads a piece of input data and advance to the next one. Implementations
        /// <strong>must</strong> return <c>null</c> at the end of the input
        /// data set. In a transactional setting, caller might get the same item
        /// twice from successive calls (or otherwise), if the first call was in a
        /// transaction that rolled back.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">&nbsp;</exception>
        /// <exception cref="UnexpectedInputException">&nbsp;</exception>
        /// <exception cref="NonTransientResourceException">&nbsp;</exception>
        /// <exception cref="ParseException">&nbsp;</exception>
        T Read();
    }
}