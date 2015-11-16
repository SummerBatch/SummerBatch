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

namespace Summer.Batch.Core.Job.Flow
{ 
    /// <summary>
    /// Represents a flow execution.
    /// </summary>
    public class FlowExecution : IComparable<FlowExecution>
    {

        /// <summary>
        /// Name property.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Flow execution status property.
        /// </summary>
        public FlowExecutionStatus Status { get; private set; }

        /// <summary>
        /// Custom constructor using a name and a flow execution status.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="status"></param>
        public FlowExecution(string name, FlowExecutionStatus status)
        {
            Name = name;
            Status = status;
        }

        /// <summary>
        /// Create an ordering on FlowExecution instances by comparing their statuses.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(FlowExecution other)
        {
            return Status.CompareTo(other.Status);
        }

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("FlowExecution: name={0}, status={1}", Name, Status);
        }
    }
}