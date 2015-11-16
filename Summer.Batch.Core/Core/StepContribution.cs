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
    /// Represents a contribution to a <see cref="StepExecution"/>, buffering changes until
    /// they can be applied at a chunk boundary. 
    /// </summary>
    [Serializable]
    public class StepContribution : Entity
    {
        private volatile int _readCount;

        /// <summary>
        /// Read count property.
        /// </summary>
        public int ReadCount
        {
            get { return _readCount; }
            set { _readCount = value; }
        }

        private volatile int _writeCount;
        
        /// <summary>
        /// Write count property.
        /// </summary>
        public int WriteCount
        {
            get { return _writeCount; }
            set { _writeCount = value; }
        }

        private volatile int _filterCount;
        
        /// <summary>
        /// Filter count property.
        /// </summary>
        public int FilterCount
        {
            get { return _filterCount; }
            set { _filterCount = value; }
        }

        private readonly int _parentSkipCount;

        /// <summary>
        /// Read skip count property.
        /// </summary>
        public int ReadSkipCount { get; set; }

        /// <summary>
        /// Write skip count property.
        /// </summary>
        public int WriteSkipCount { get; set; }

        /// <summary>
        /// Process skip count property. 
        /// </summary>
        public int ProcessSkipCount { get; set; }

        /// <summary>
        /// Step skip count calculation.
        /// </summary>
        public int StepSkipCount
        {
            get { return ReadSkipCount + WriteSkipCount + ProcessSkipCount + _parentSkipCount; }
        }

        /// <summary>
        /// Skip count calculation.
        /// </summary>
        public int SkipCount
        {
            get { return ReadSkipCount + WriteSkipCount + ProcessSkipCount; }
        }


        private ExitStatus _exitStatus = ExitStatus.Executing;

        /// <summary>
        /// Exit status property.
        /// </summary>
        public ExitStatus ExitStatus
        {
            get { return _exitStatus; }
            set { _exitStatus = value; }
        }

        /// <summary>
        /// Custom constructor using a step execution.
        /// </summary>
        /// <param name="execution"></param>
        public StepContribution(StepExecution execution)
        {
            _parentSkipCount = execution.SkipCount;
        }

        /// <summary>
        /// Increments the counter for the number of items processed.
        /// </summary>
        /// <param name="count"></param>
        public void IncrementFilterCount(int count)
        {
            _filterCount += count;
        }

        /// <summary>
        /// Increments the counter for the number of items read.
        /// </summary>
        public void IncrementReadCount()
        {
            _readCount++;
        }

        /// <summary>
        /// Increment the counter for the number of items written.
        /// </summary>
        /// <param name="count"></param>
        public void IncrementWriteCount(int count)
        {
            _writeCount += count;
        }

        /// <summary>
        /// Increments the counter for the number of read skips.
        /// </summary>
        public void IncrementReadSkipCount()
        {
            ReadSkipCount++;
        }

        /// <summary>
        /// Increments the counter for the number of read skips by the given amount.
        /// </summary>
        /// <param name="count"></param>
        public void IncrementReadSkipCount(int count)
        {
            ReadSkipCount += count;
        }

        /// <summary>
        /// Increments the counter for the number of write skips.
        /// </summary>
        public void IncrementWriteSkipCount()
        {
            WriteSkipCount++;
        }

        /// <summary>
        ///  Increments the counter for the number of process skips.
        /// </summary>
        public void IncrementProcessSkipCount()
        {
            ProcessSkipCount++;
        }


        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[StepContribution: read={0}, written={1}, filtered={2}, readSkips={3}, writeSkips={4}, processSkips={5}, exitStatus={6}]",
                ReadCount, WriteCount, FilterCount,ReadSkipCount,WriteSkipCount,ProcessSkipCount,ExitStatus.ExitCode);
        }

        /// <summary>
        /// Equals override.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(Object obj)
        {
            if (!(obj is StepContribution))
            {
                return false;
            }
            StepContribution other = (StepContribution)obj;
            return ToString().Equals(other.ToString());
        }



        /// <summary>
        /// GetHashCode override.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return 11 + ToString().GetHashCode() * 43;
        }

    }
}
