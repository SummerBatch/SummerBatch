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

using Summer.Batch.Common.Util;

namespace Summer.Batch.Core.Scope.Context
{
    /// <summary>
    ///  Context object for weakly typed data stored for the duration of a chunk
    /// (usually a group of items processed together in a transaction). If there is a
    /// rollback and the chunk is retried the same context will be associated with it.
    /// </summary>
    public class ChunkContext : AttributeAccessorSupport
    {
        private readonly StepContext _stepContext;

        private bool _complete;

        /// <summary>
        /// Complete flag property.
        /// </summary>
        public bool Complete
        {
            get { return _complete; }
        }

        /// <summary>
        /// Custom constructor with a step context.
        /// </summary>
        /// <param name="stepContext"></param>
        public ChunkContext(StepContext stepContext)
        {
            _stepContext = stepContext;
        }

        /// <summary>
        /// Step context property.
        /// </summary>
        public StepContext StepContext
        {
            get { return _stepContext; }
        }

        /// <summary>
        /// Setter for the flag to signal complete processing of a chunk.
        /// </summary>
        public void SetComplete()
        {
            _complete = true;
        }

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("ChunkContext: attributes={0}, complete={1}, stepContext={2}",
                AttributeNames(),
                _complete,
                _stepContext
                );
        }

    }
}
