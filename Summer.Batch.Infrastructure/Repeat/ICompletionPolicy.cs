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
namespace Summer.Batch.Infrastructure.Repeat
{
    /// <summary>
    ///  Interface for batch completion policies, to enable batch operations to
    /// strategise normal completion conditions. Stateful implementations of batch
    /// iterators should <em>only</em> update state using the update method. If you
    /// need custom behaviour consider extending an existing implementation or using
    /// the composite provided.
    /// </summary>
    public interface ICompletionPolicy
    {

        /// <summary>
        ///Determine whether a batch is complete given the latest result from the
        /// callback. If this method returns true then
        /// #IsComplete(RepeatContext) should also (but not necessarily vice
        /// versa, since the answer here depends on the result).
        /// </summary>
        /// <param name="context"> the current batch context.</param>
        /// <param name="result">the result of the latest batch item processing.</param>
        /// <returns>true if the batch should terminate.</returns>
        bool IsComplete(IRepeatContext context, RepeatStatus result);

        /// <summary>
        /// Allow policy to signal completion according to internal state, without
        ///having to wait for the callback to complete.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>true if the batch should terminate.</returns>
        bool IsComplete(IRepeatContext context);

        /// <summary>
        /// Create a new context for the execution of a batch. N.B. implementations
        /// should <em>not</em> return the parent from this method - they must
        /// create a new context to meet the specific needs of the policy. The best
        /// way to do this might be to override an existing implementation and use
        /// the RepeatContext to store state in its attributes.
        /// </summary>
        /// <param name="parent">the current context if one is already in progress.</param>
        /// <returns> a context object that can be used by the implementation to store internal state for a batch.</returns>
        IRepeatContext Start(IRepeatContext parent);

        /// <summary>
        /// Give implementations the opportunity to update the state of the current
        /// batch. Will be called <em>once</em> per callback, after it has been
        /// launched, but not necessarily after it completes (if the batch is
        /// asynchronous).
        /// </summary>
        /// <param name="context">the value returned by start</param>
        void Update(IRepeatContext context);
    }
}