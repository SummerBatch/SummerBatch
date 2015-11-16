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
using System.Linq;

namespace Summer.Batch.Core.Job.Flow
{
    /// <summary>
    /// Represents the status of FlowExecution.
    /// </summary>
    public class FlowExecutionStatus : IComparable<FlowExecutionStatus>
    {
        /// <summary>
        /// Name property.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Custom constructor using a status.
        /// </summary>
        /// <param name="status"></param>
        public FlowExecutionStatus(string status)
        {
            Name = status;
        }

        #region Special Known Values (simili constants)
        /// <summary>
        /// Special well-known status value.
        /// </summary>
        public static readonly FlowExecutionStatus Completed = new FlowExecutionStatus(Status.Completed.ToString().ToUpper());

        /// <summary>
        /// Special well-known status value.
        /// </summary>
        public static readonly FlowExecutionStatus Stopped = new FlowExecutionStatus(Status.Stopped.ToString().ToUpper());

        /// <summary>
        /// Special well-known status value.
        /// </summary>
        public static readonly FlowExecutionStatus Failed = new FlowExecutionStatus(Status.Failed.ToString().ToUpper());

        /// <summary>
        /// Special well-known status value.
        /// </summary>
        public static readonly FlowExecutionStatus Unkown = new FlowExecutionStatus(Status.Unknown.ToString().ToUpper()); 
        #endregion


        /// <summary>
        /// List of well known statuses (for matching purpose).
        /// </summary>
        private static readonly Status[] Statuses = { Status.Completed, Status.Failed, Status.Stopped, Status.Unknown };

        private enum Status
        {
            Completed,
            Stopped,
            Failed,
            Unknown
        }
        
        private static Status Match(string value)
        {
            // Default match should be the lowest priority
            return Statuses.FirstOrDefault(stat => value.StartsWith(stat.ToString().ToUpper()));
        }

        #region Test status methods
        /// <summary>
        /// </summary>
        /// <returns>true if the status starts with "Stopped"</returns>
        public bool IsStop()
        {
            return Name.StartsWith(Stopped.Name);
        }

        /// <summary>
        /// </summary>
        /// <returns>true if the status starts with "Failed"</returns>
        public bool IsFail()
        {
            return Name.StartsWith(Failed.Name);
        }

        /// <summary>
        /// </summary>
        /// <returns>true if this status represents the end of a flow</returns>
        public bool IsEnd()
        {
            return IsStop() || IsFail() || IsComplete();
        }

        /// <summary>
        /// </summary>
        /// <returns>true if the status starts with "Completed"</returns>
        private bool IsComplete()
        {
            return Name.StartsWith(Completed.Name);
        } 
        #endregion


        /// <summary>
        /// Create an ordering on FlowExecutionStatus instances by comparing their statuses.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(FlowExecutionStatus other)
        {
            Status one = Match(Name);
            Status two = Match(other.Name);
            int comparison = one.CompareTo(two);
            if (comparison == 0)
            {
                return string.Compare(Name, other.Name, StringComparison.Ordinal);
            }
            return comparison;
        }

        #region overridden methods
        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// GetHashCode override.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        /// <summary>
        /// Equals override.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            if (!(obj is FlowExecutionStatus))
            {
                return false;
            }
            FlowExecutionStatus other = (FlowExecutionStatus)obj;
            return Name.Equals(other.Name);
        }
        #endregion
    }
}