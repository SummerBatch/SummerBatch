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
namespace Summer.Batch.Infrastructure.Repeat.Support
{
    /// <summary>
    /// Interface for result holder.
    /// </summary>
    public interface IResultHolder
    {
        /// <summary>
        /// Gets the result for client from this holder. Does not block if none is
        /// available yet.
        /// </summary>
        /// <returns> the result, or null if there is none.</returns>
        RepeatStatus Result { get;}

        /// <summary>
        /// Gets the error for client from this holder if any. Does not block if
        /// none is available yet.
        /// </summary>
        /// <returns></returns>
        System.Exception Error { get; }

        /// <summary>
        /// Gets the context in which the result evaluation is executing.
        /// </summary>
        /// <returns>the context of the result evaluation.</returns>
        IRepeatContext Context { get; }
    }
}