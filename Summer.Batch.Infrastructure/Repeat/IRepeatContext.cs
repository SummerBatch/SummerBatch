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
using Summer.Batch.Common.Util;
using System.Threading.Tasks;

namespace Summer.Batch.Infrastructure.Repeat
{
    /// <summary>
    /// Base interface for context which controls the state and completion /
    /// termination of a batch step. A new context is created for each call to the
    /// IRepeatOperations. Within a batch callback code can communicate via
    /// the IAttributeAccessor interface.
    /// </summary>
    public interface IRepeatContext : IAttributeAccessor
    {
        /// <summary>
        /// If batches are nested, then the inner batch will be created with the
        /// outer one as a parent. This is an accessor for the parent if it exists.
        /// </summary>
        /// <returns> the parent context or null if there is none</returns>
        IRepeatContext Parent { get; }

        /// <summary>
        ///  Public access to a counter for the number of operations attempted.
        /// </summary>
        /// <returns>the number of batch operations started.</returns>
        int GetStartedCount();

        /// <summary>
        ///  Signal to the framework that the current batch should complete normally,
        /// independent of the current CompletionPolicy
        /// </summary>
        void SetCompleteOnly();

        /// <summary>
        ///  Public accessor for the complete flag.
        /// </summary>
        /// <returns></returns>
        bool IsCompleteOnly();

        /// <summary>
        /// Signal to the framework that the current batch should complete
        /// abnormally, independent of the current CompletionPolicy.
        /// </summary>
        void SetTerminateOnly();

        /// <summary>
        /// Public accessor for the termination flag. If this flag is set then the complete flag will also be.
        /// </summary>
        /// <returns></returns>
        bool IsTerminateOnly();

        /// <summary>
        /// Register a callback to be executed on close, associated with the
        /// attribute having the given name. The ThreadStart callback should not
        /// throw any exceptions.
        /// </summary>
        /// <param name="name">the name of the attribute to associated this callback with. If this attribute is removed the callback should never be called.</param>
        /// <param name="callback">a Task to execute when the context is closed.</param>
        void RegisterDestructionCallback(string name, Task callback);


        /// <summary>
        /// Allow resources to be cleared, especially in destruction callbacks.
        ///Implementations should ensure that any registered destruction callbacks
        /// are executed here, as long as the corresponding attribute is still
        /// available.
        ///</summary>
        void Close();
    }
}
