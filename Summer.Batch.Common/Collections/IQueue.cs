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
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
using System.Collections.Generic;

namespace Summer.Batch.Common.Collections
{
    /// <summary>
    /// A collection designed for holding elements prior to processing.
    /// The order in which elements are retrieved depends on the implementation.
    /// </summary>
    /// <typeparam name="T">&nbsp;</typeparam>
    public interface IQueue<T> : ICollection<T>
    {
        /// <summary>
        /// Removes the head of the queue and returns it.
        /// </summary>
        /// <returns>
        /// the head of the queue, or default(<typeparamref name="T"/>) if the queue is empty
        /// </returns>
        T Poll();

        /// <summary>
        /// Returns the head of the queue without removing it.
        /// </summary>
        /// <returns>
        /// the head of the queue, or default(<typeparamref name="T"/>) if the queue is empty
        /// </returns>
        T Peek();
    }
}