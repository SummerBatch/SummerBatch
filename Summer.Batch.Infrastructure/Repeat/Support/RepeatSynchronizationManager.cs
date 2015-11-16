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
using System.Threading;

namespace Summer.Batch.Infrastructure.Repeat.Support
{

    /// <summary>
    /// Global variable support for repeat clients. Normally it is not necessary for
    /// clients to be aware of the surrounding environment because a
    /// IRepeatCallback can always use the context it is passed by the
    /// enclosing IRepeatOperations. But occasionally it might be helpful to
    /// have lower level access to the ongoing IRepeatContext so we provide a
    /// global accessor here. The mutator methods (#Clear() and
    /// #Register(IRepeatContext) should not be used except internally by
    /// IRepeatOperations implementations.
    /// </summary>
    public static class RepeatSynchronizationManager
    {
        private static readonly ThreadLocal<IRepeatContext> ContextHolder = new ThreadLocal<IRepeatContext>();

        /// <summary>
        /// Getter for the current context. A context is shared by all items in the
        /// batch, so this method is intended to return the same context object
        /// independent of whether the callback is running synchronously or
        /// asynchronously with the surrounding IRepeatOperations.
        /// </summary>
        /// <returns>the current IRepeatContext or null if there is none (if we are not in a batch).</returns>
        public static IRepeatContext GetContext()
        {
            return ContextHolder.Value;
        }

        /// <summary>
        /// Convenience method to set the current repeat operation to complete if it exists.
        /// </summary>
        public static void SetCompleteOnly()
        {
            IRepeatContext context = GetContext();
            if (context != null)
            {
                context.SetCompleteOnly();
            }
        }

        /// <summary>
        /// Method for registering a context - should only be used by
        /// IRepeatOperations implementations to ensure that
        /// #GetContext() always returns the correct value.
        /// </summary>
        /// <param name="context"> a new context at the start of a batch.</param>
        /// <returns>the old value if there was one.</returns>
        public static IRepeatContext Register(IRepeatContext context)
        {
            IRepeatContext oldSession = GetContext();
            ContextHolder.Value = context;
            return oldSession;
        }

        /// <summary>
        /// Clear the current context at the end of a batch - should only be used by
        /// IRepeatOperations implementations.
        /// </summary>
        /// <returns> the old value if there was one.</returns>
        public static IRepeatContext Clear()
        {
            IRepeatContext context = GetContext();
            ContextHolder.Value = null;
            return context;
        }

        /// <summary>
        /// Set current session and all ancestors (via parent) to complete.,
        /// </summary>
        public static void SetAncestorsCompleteOnly()
        {
            IRepeatContext context = GetContext();
            while (context != null)
            {
                context.SetCompleteOnly();
                context = context.Parent;
            }
        }
    }
}