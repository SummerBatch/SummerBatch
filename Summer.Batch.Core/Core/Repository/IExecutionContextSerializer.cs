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

//   This file has been modified.
//   Original copyright notice :

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
using System.Collections.Generic;
using System.IO;

namespace Summer.Batch.Core.Repository
{
    /// <summary>
    /// A composite interface that combines both serialization and deserialization
    /// of an execution context into a single implementation.  Implementations of this
    /// interface are used to serialize the execution context for persistence during
    /// the execution of a job.
    /// </summary>
    public interface IExecutionContextSerializer
    {
        /// <summary>
        /// Serialize object.
        /// </summary>
        /// <param name="toSerialize"></param>
        /// <param name="stream"></param>
        void Serialize(IDictionary<string, Object> toSerialize, Stream stream);

        /// <summary>
        /// Deserialize stream to object.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        IDictionary<string, Object> Deserialize(Stream stream);
    }
}
