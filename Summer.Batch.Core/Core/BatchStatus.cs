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

namespace Summer.Batch.Core
{
    /// <summary>
    /// Statuses of a batch execution.
    /// </summary>
    [Serializable]
    public sealed class BatchStatus {

        private readonly int _rank;
        private readonly string _label;

        //Prevents instantiation.
        private BatchStatus(int rank, string label) { _rank = rank; _label = label; }

       
        // Status values
       
        /// <summary>
        /// Completed status
        /// </summary>
        public readonly static BatchStatus Completed = new BatchStatus(0, "COMPLETED");

        /// <summary>
        /// Starting status
        /// </summary>
        public readonly static BatchStatus Starting = new BatchStatus(1, "STARTING");

        /// <summary>
        /// Started status
        /// </summary>
        public readonly static BatchStatus Started = new BatchStatus(2, "STARTED");

        /// <summary>
        /// Stopping status
        /// </summary>
        public readonly static BatchStatus Stopping = new BatchStatus(3, "STOPPING");

        /// <summary>
        /// Stopped status.
        /// </summary>
        public readonly static BatchStatus Stopped = new BatchStatus(4, "STOPPED");

        /// <summary>
        /// Failed status.
        /// </summary>
        public readonly static BatchStatus Failed = new BatchStatus(5, "FAILED");

        /// <summary>
        /// Abandoned status.
        /// </summary>
        public readonly static BatchStatus Abandoned = new BatchStatus(6, "ABANDONED");
        
        /// <summary>
        /// Unkonwn status.
        /// </summary>
        public readonly static BatchStatus Unknown = new BatchStatus(7, "UNKNOWN");

        /// <summary>
        /// Possible status values.
        /// </summary>
        public readonly static BatchStatus[] Values = { Completed, Starting, Started, Stopping, Stopped, Failed, Abandoned, Unknown };

        /// <summary>
        /// Redefining equality operator.
        /// </summary>
        /// <param name="status1"></param>
        /// <param name="status2"></param>
        /// <returns></returns>
        public static bool operator ==(BatchStatus status1, BatchStatus status2)
        {
            return (ReferenceEquals(status1, null) && ReferenceEquals(status2, null)) || (!ReferenceEquals(status1, null) && status1.Equals(status2));
        }

        /// <summary>
        /// Redefining non equality operator.
        /// </summary>
        /// <param name="status1"></param>
        /// <param name="status2"></param>
        /// <returns></returns>
        public static bool operator !=(BatchStatus status1, BatchStatus status2)
        {
            return !(status1 == status2);
        }

        /// <summary>
        /// given string, returns corresponding status (if it makes sense)
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static BatchStatus ValueOf(string status)
        {
            BatchStatus result = null;
            if (string.Equals(Completed._label, status, StringComparison.OrdinalIgnoreCase))
            {
                result = Completed;
            }
            if (string.Equals(Starting._label, status, StringComparison.OrdinalIgnoreCase))
            {
                result = Starting;
            }
            if (string.Equals(Started._label, status, StringComparison.OrdinalIgnoreCase))
            {
                result = Started;
            }
            if (string.Equals(Stopping._label, status, StringComparison.OrdinalIgnoreCase))
            {
                result = Stopping;
            }
            if (string.Equals(Stopped._label, status, StringComparison.OrdinalIgnoreCase))
            {
                result = Stopped;
            }
            if (string.Equals(Failed._label, status, StringComparison.OrdinalIgnoreCase))
            {
                result = Failed;
            }
            if (string.Equals(Abandoned._label, status, StringComparison.OrdinalIgnoreCase))
            {
                result = Abandoned;
            }
            if (string.Equals(Unknown._label, status, StringComparison.OrdinalIgnoreCase))
            {
                result = Unknown;
            }
            if (result != null)
            {
                return result;
            }//else
            throw new ArgumentException(string.Format("Unkown status: {0}", status));
        }
        
        /// <summary>
        /// return max BatchStatus from 2
        /// </summary>
        /// <param name="status1"></param>
        /// <param name="status2"></param>
        /// <returns></returns>
        public static BatchStatus Max(BatchStatus status1, BatchStatus status2)
        {
            return status1.IsGreaterThan(status2) ? status1 : status2;
        }

        /// <summary>
        ///  Convenience method to decide if a status indicates work is in progress.
        /// </summary>
        /// <returns> true if the status is STARTING, STARTED </returns>
        public bool IsRunning()
        {
            return this == Starting || this == Started;
        }


        ///
        /// Convenience method to decide if a status indicates execution was
        /// unsuccessful.
        /// <returns> true if the status is FAILED or greater </returns>
        public bool IsUnsuccessful()
        {
            return this == Failed || IsGreaterThan(Failed);
        }

        /// <summary>
        /// 	  Method used to move status values through their logical progression, and
        /// override less severe failures with more severe ones. This value is
        /// compared with the parameter and the one that has higher priority is
        /// returned. If both are STARTED or less than the value returned is the
        /// largest in the sequence STARTING, STARTED, COMPLETED. Otherwise the value
        /// returned is the maximum of the two.
        /// </summary>
        /// <param name="other"> other another status to compare to </param>
        /// <returns> either this or the other status depending on their priority </returns>
        public BatchStatus UpgradeTo(BatchStatus other)
        {
            if (IsGreaterThan(Started) || other.IsGreaterThan(Started))
            {
                return Max(this, other);
            }
            // Both less than or equal to STARTED
            if (this == Completed || other == Completed)
            {
                return Completed;
            }
            return Max(this, other);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"> other value to compare</param>
        /// <returns> true if this is greater than other</returns>
        public bool IsGreaterThan(BatchStatus other)
        {
            return _rank > other._rank;
        }


        /// <summary>
        /// </summary>
        /// <param name="other">other a status value to compare</param>
        /// <returns>true if this is less than other</returns>        
        public bool IsLessThan(BatchStatus other)
        {
            return _rank < other._rank;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other">other a status value to compare</param>
        /// <returns>true if this is less or equal than other</returns>
        public bool IsLessThanOrEqualTo(BatchStatus other)
        {
            return _rank <= other._rank;
        }

        /// <summary>
        ///  Find a BatchStatus that matches the beginning of the given value. If no
        ///  match is found, return COMPLETED as the default because has is low
        ///  precedence.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static BatchStatus Match(string value)
        {
            foreach(BatchStatus status in Values){
                if (value.StartsWith(status._label))
                {
                    return status;
                }
            }
            //DEFAULT TO LOWEST
            return Completed;
        }

        /// <summary>
        /// Equals override.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(object other)
        {
            return other is BatchStatus && ((BatchStatus)other)._rank == _rank;
        }

        /// <summary>
        /// GetHashCode override.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return _rank;
        }

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _label;
        }
    }
}
