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
using System.Runtime.Serialization;

namespace Summer.Batch.Core
{
    /// <summary>
    ///  Exception to indicate the the job has been interrupted. The exception state
    /// indicated is not normally recoverable by batch application clients, but
    /// internally it is useful to force a check. The exception will often be wrapped
    /// in a runtime exception (usually UnexpectedJobExecutionException before
    /// reaching the client.
    /// </summary>
    [Serializable]
    public class JobInterruptedException : JobExecutionException
    {

        private readonly BatchStatus _batchStatus = BatchStatus.Stopped;
        
        /// <summary>
        /// Batch status.
        /// </summary>
        public BatchStatus Status { get { return _batchStatus; } }

        /// <summary>
        /// Construct a JobExecutionException with a generic message.
        /// </summary>
        /// <param name="msg"></param>
        public JobInterruptedException(string msg)
            : base(msg)
        { }

        /// <summary>
        /// Custom constructor with message and Status.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="status"></param>
        public JobInterruptedException(String msg, BatchStatus status)
            : base(msg)
        {
           _batchStatus = status;
        }

        /// <summary>
        /// Serialization constructor.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected JobInterruptedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
         
        /// <summary>
        /// Serialization implementation.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info,context);
            info.AddValue("BatchStatus",Status);
        }
    }
}
