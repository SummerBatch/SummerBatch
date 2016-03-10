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
    /// Item processor. This is where item given as input (TIn) is being processed to be transformed
    /// into the output (TOut). Input and Output types can be different.
    /// Convention: Returning null indicates that this item should not be continued to be processed.
    /// </summary>
    /// <typeparam name="TIn">&nbsp;</typeparam>
    /// <typeparam name="TOut">&nbsp;</typeparam>
    public interface IItemProcessor<in TIn, out TOut> where TIn:class where TOut:class
    {
        /// <summary>
        /// Process the provided item, returning a potentially modified or new item for continued
        /// processing.  If the returned result is null, it is assumed that processing of the item
        /// should not continue.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="Exception">&nbsp;</exception>
        TOut Process(TIn item);
    }
}