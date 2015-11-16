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

namespace Summer.Batch.Core.Job.Builder
{
    /// <summary>
    ///  Utility exception thrown by builders when they encounter unexpected exceptions.
    /// </summary>
    [Serializable]
    public class FlowBuilderException : Exception
    {
        /// <summary>
        /// Custom constructor with inner message.
        /// </summary>
        /// <param name="msg"></param>
        public FlowBuilderException(string msg) : base(msg) { }

        /// <summary>
        /// Custom constructor with inner exception.
        /// </summary>
        /// <param name="e"></param>
        public FlowBuilderException(Exception e) : base(e.Message, e) { }
        
        /// <summary>
        /// Custom constructor with inner message and exception.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="e"></param>
        public FlowBuilderException(string msg, Exception e) : base(msg, e) { }

    }
}