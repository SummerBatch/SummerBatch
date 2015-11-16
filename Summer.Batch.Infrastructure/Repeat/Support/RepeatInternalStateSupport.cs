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
using System.Collections.Generic;

namespace Summer.Batch.Infrastructure.Repeat.Support
{
    /// <summary>
    /// Internal state support for Repeat.
    /// Mainly used to store encountered exceptions.
    /// </summary>
    public class RepeatInternalStateSupport : IRepeatInternalState
    {
        /// <summary>
        /// Accumulation of failed results. 
        /// </summary>
        private readonly HashSet<System.Exception> _exceptions = new HashSet<System.Exception>();

        /// <summary>
        /// Return collected exceptions.
        /// </summary>
        /// <returns></returns>
        public ICollection<System.Exception> GetExceptions()
        {
            return _exceptions;
        }
    }
}