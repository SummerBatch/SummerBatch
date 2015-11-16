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
using System.Threading;

namespace Summer.Batch.Infrastructure.Repeat.Support
{
    /// <summary>
    /// Abstraction for queue of IResultHolder objects. Acts a bit like a
    /// BlockingQueue with the ability to count the number of items it
    /// expects to ever hold. When clients schedule an item to be added they call
    /// #Expect()}, and then collect the result later with  #Take().
    /// Result providers in another thread call Put(Object) to notify the
    /// expecting client of a new result.
    /// </summary>
    /// <typeparam name="TB"></typeparam>
    public interface IResultQueue<TB>
    {
        /// <summary>
        /// In a master-slave pattern, the master calls this method paired with
        /// Take() to manage the flow of items. Normally a task is submitted
        /// for processing in another thread, at which point the master uses this
        /// method to keep track of the number of expected results. It has the
        /// personality of an counter increment, rather than a work queue, which is
        /// usually managed elsewhere, e.g. by a  IExecutor}.
        /// Implementations may choose to block here, if they need to limit the
        /// number or rate of tasks being submitted.
        /// </summary>
        /// <exception cref="ThreadInterruptedException">if the call blocks and is then interrupted.</exception>
        void Expect();

        /// <summary>
        /// Once it is expecting a result, clients call this method to satisfy the
        /// expectation. In a master-worker pattern, the workers call this method to
        /// deposit the result of a finished task on the queue for collection.
        /// </summary>
        /// <param name="result">the result for later collection.</param>
        /// <exception cref="ArgumentException"></exception>
        void Put(TB result);

        /// <summary>
        /// Gets the next available result, blocking if there are none yet available.
        /// </summary>
        /// <returns>a result previously deposited</returns>
        /// <exception cref="InvalidOperationException">if there is no result expected</exception>
        /// <exception cref="ThreadInterruptedException">if the operation is interrupted while waiting</exception>
        TB Take();

        /// <summary>
        /// Used by master thread to verify that there are results available from
        /// Take() without possibly having to block and wait.
        /// </summary>
        /// <returns> true if there are no results available</returns>
        bool IsEmpty();

        /// <summary>
        /// Checks if any results are expected. Usually used by master thread to drain
        /// queue when it is finished.
        /// </summary>
        /// <returns>true if more results are expected, but possibly not yet available</returns>
        bool IsExpecting();
    }
}