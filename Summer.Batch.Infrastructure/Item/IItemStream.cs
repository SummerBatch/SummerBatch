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
    /// Interface for periodically storing and restoring state should an error occur.
    /// </summary>
    public interface IItemStream : IDisposable
    {
        /// <summary>
        /// Open the stream for the provided ExecutionContext.
        /// </summary>
        /// <param name="executionContext">current step's ExecutionContext.  Will be the
        /// executionContext from the last run of the step on a restart.</param>
        /// <exception cref="ItemStreamException"></exception>
        /// <exception cref="ArgumentException">if execution context is null</exception>
        void Open(ExecutionContext executionContext);

        /// <summary>
        /// Indicates that the execution context provided during open is about to be saved. If any state is remaining, but
        /// has not been put in the context, it should be added here.
        /// </summary>
        /// <param name="executionContext">to be updated</param>
        /// <exception cref="ItemStreamException"></exception>
        /// <exception cref="ArgumentException">if execution context is null</exception>
        void Update(ExecutionContext executionContext);

        /// <summary>
        /// Flushes the stream if able.
        /// </summary>
        void Flush();

        /// <summary>
        /// If any resources are needed for the stream to operate they need to be destroyed here. Once this method has been
        /// called all other methods (except open) may throw an exception.
        /// </summary>
        /// <exception cref="ItemStreamException"></exception>
        void Close();
    }
}