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
#region using

using Summer.Batch.Common.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using ExecutionContext = Summer.Batch.Infrastructure.Item.ExecutionContext;

#endregion

namespace Summer.Batch.Core
{
    /// <summary>
    /// Batch domain object representation the execution of a step. Unlike
    /// <see cref="JobExecution"/>, there are additional properties related the processing
    /// of items such as commit count, etc.
    /// </summary>
    [Serializable]
    public class StepExecution : Entity
    {
        private readonly JobExecution _jobExecution;
        
        /// <summary>
        ///  Owning Job execution.
        /// </summary>
        public JobExecution JobExecution
        {
            get { return _jobExecution; }
        }

        private readonly string _stepName;
        
        /// <summary>
        /// Step name.
        /// </summary>
        public string StepName
        {
            get { return _stepName; }
        }

        private volatile BatchStatus _batchStatus = BatchStatus.Starting;

        /// <summary>
        /// Batch status.
        /// </summary>
        public BatchStatus BatchStatus
        {
            get { return _batchStatus; }
            set { _batchStatus = value; }
        }

        private volatile int _readCount;
        
        /// <summary>
        /// Read count.
        /// </summary>
        public int ReadCount
        {
            get { return _readCount; }
            set { _readCount = value; }
        }

        private volatile int _writeCount;
        
        /// <summary>
        /// Write count.
        /// </summary>
        public int WriteCount
        {
            get { return _writeCount; }
            set { _writeCount = value; }
        }

        private volatile int _commitCount;
        
        /// <summary>
        /// Commit count.
        /// </summary>
        public int CommitCount
        {
            get { return _commitCount; }
            set { _commitCount = value; }
        }

        private volatile int _rollbackCount;

        /// <summary>
        /// Rollback count.
        /// </summary>
        public int RollbackCount
        {
            get { return _rollbackCount; }
            set { _rollbackCount = value; }
        }

        private volatile int _readSkipCount;

        /// <summary>
        /// Read skip count.
        /// </summary>
        public int ReadSkipCount
        {
            get { return _readSkipCount; }
            set { _readSkipCount = value; }
        }

        /// <summary>
        /// Skip count calculation.
        /// </summary>
        public int SkipCount
        {
            get { return _readSkipCount + _processSkipCount + _writeSkipCount; }
        }

        private volatile int _processSkipCount;

        /// <summary>
        /// Process skip count.
        /// </summary>
        public int ProcessSkipCount
        {
            get { return _processSkipCount; }
            set { _processSkipCount = value; }
        }

        private volatile int _writeSkipCount;

        /// <summary>
        /// Write skip count.
        /// </summary>
        public int WriteSkipCount
        {
            get { return _writeSkipCount; }
            set { _writeSkipCount = value; }
        }


        // NOTE TMA : DateTime not supported as volatile by C#
        private long _startTime = DateTime.Now.Ticks;

        /// <summary>
        /// Start time.
        /// </summary>
        public DateTime StartTime
        {
            get { return new DateTime(Interlocked.Read(ref _startTime)); }
            set { Interlocked.Exchange(ref _startTime, value.Ticks); }
        }

        //use MinValue rather than null ...
        private long _endTime = DateTime.MinValue.Ticks;

        /// <summary>
        /// End time.
        /// </summary>
        public DateTime? EndTime
        {
            get
            {
                var endTime = Interlocked.Read(ref _endTime);
                return endTime == DateTime.MinValue.Ticks ? (DateTime?)null : new DateTime(endTime);
            }
            set
            {
                var newValue = value == null ? DateTime.MinValue.Ticks : ((DateTime)value).Ticks;
                Interlocked.Exchange(ref _endTime, newValue);
            }
        }

        //use MinValue rather than null ...
        private long _lastUpdated = DateTime.MinValue.Ticks;

        /// <summary>
        /// Last updated time.
        /// </summary>
        public DateTime? LastUpdated
        {
            get
            {
                var lastUpdated = Interlocked.Read(ref _lastUpdated);
                return lastUpdated == DateTime.MinValue.Ticks ? (DateTime?)null : new DateTime(lastUpdated);
            }
            set
            {
                var newValue = value == null ? DateTime.MinValue.Ticks : ((DateTime)value).Ticks;
                Interlocked.Exchange(ref _lastUpdated, newValue);
            }
        }

        private volatile ExecutionContext _executionContext = new ExecutionContext();

        /// <summary>
        /// Execution context.
        /// </summary>
        public ExecutionContext ExecutionContext
        {
            get { return _executionContext; }
            set { _executionContext = value; }
        }

        private volatile ExitStatus _exitStatus = ExitStatus.Executing;
        
        /// <summary>
        /// Exit status.
        /// </summary>
        public ExitStatus ExitStatus
        {
            get { return _exitStatus; }
            set { _exitStatus = value; }
        }

        private volatile bool _terminateOnly;
        /// <summary>
        /// Terminate only flag; getter.
        /// </summary>
        public bool TerminateOnly
        {
            get { return _terminateOnly; }
        }

        /// <summary>
        /// Terminate only flag; setter.
        /// </summary>
        public void SetTerminateOnly()
        {
            _terminateOnly = true;
        }

        /// <summary>
        /// Filter count.
        /// </summary>
        public int FilterCount { get; set; }

        private readonly IProducerConsumerCollection<Exception> _failureExceptions = new ConcurrentBag<Exception>();

        /// <summary>
        ///  Constructor with mandatory properties.
        /// </summary>
        /// <param name="stepName"></param>
        /// <param name="jobExecution"></param>
        /// <param name="id"></param>
        public StepExecution(string stepName, JobExecution jobExecution, long id)
            : this(stepName, jobExecution)
        {
            Assert.NotNull(jobExecution, "JobExecution must be provided to re-hydrate an existing StepExecution");
            Assert.NotNull(id, "The entity Id must be provided to re-hydrate an existing StepExecution");
            Id = id;
            jobExecution.AddStepExecution(this);
        }

        /// <summary>
        /// Constructor that substitues in null for execution id.
        /// </summary>
        /// <param name="stepName"></param>
        /// <param name="jobExecution"></param>
        public StepExecution(string stepName, JobExecution jobExecution)
        {
            Assert.HasLength(stepName);
            _stepName = stepName;
            _jobExecution = jobExecution;
        }

        /// <summary>
        ///     Upgrade the status field if the provided value is greater than the
        ///     existing one. Clients using this method to set the status can be sure
        ///     that they don't overwrite a failed status with an successful one.
        /// </summary>
        /// <param name="status"></param>
        public void UpgradeStatus(BatchStatus status)
        {
            _batchStatus = _batchStatus.UpgradeTo(status);
        }

        /// <summary>
        /// Returns the owning job execution id.
        /// </summary>
        /// <returns></returns>
        public long? GetJobExecutionId()
        {
            if (_jobExecution != null)
            {
                return _jobExecution.Id;
            }
            return null;
        }


        /// <summary>
        /// Factory method for StepContribution.
        /// </summary>
        /// <returns></returns>
        public StepContribution CreateStepContribution()
        {
            return new StepContribution(this);
        }


        /// <summary>
        ///     On successful execution just before a chunk commit, this method should be
        ///     called. Synchronizes access to the StepExecution so that changes
        ///     are atomic.
        ///     Synchronized method.
        /// </summary>
        /// <param name="contribution"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Apply(StepContribution contribution)
        {
            _readSkipCount += contribution.ReadSkipCount;
            _writeSkipCount += contribution.WriteSkipCount;
            _processSkipCount += contribution.ProcessSkipCount;
            FilterCount += contribution.FilterCount;
            _readCount += contribution.ReadCount;
            _writeCount += contribution.WriteCount;
            _exitStatus = _exitStatus.And(contribution.ExitStatus);
        }

        /// <summary>
        ///     On unsuccessful execution after a chunk has rolled back.
        ///    Synchronized method.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void IncrementRollbackCount()
        {
            _rollbackCount++;
        }

        /// <summary>
        ///     Increments the number of commits
        /// </summary>
        public void IncrementCommitCount()
        {
            _commitCount++;
        }

        /// <summary>
        ///     Convenience method to get the current job parameters.
        /// </summary>
        /// <returns></returns>
        public JobParameters GetJobParameters()
        {
            if (_jobExecution == null)
            {
                return new JobParameters();
            }
            return _jobExecution.JobParameters;
        }

        /// <summary>
        /// Returns the failure exceptions collection.
        /// </summary>
        /// <returns></returns>
        public ICollection<Exception> GetFailureExceptions()
        {
            return new ReadOnlyCollection<Exception>(_failureExceptions.ToList());
        }

        /// <summary>
        /// Adds exception to failure exceptions collection.
        /// </summary>
        /// <param name="exception"></param>
        public void AddFailureException(Exception exception)
        {
            _failureExceptions.TryAdd(exception);
        }

        /// <summary>
        /// Equals override.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(Object obj)
        {
            Object jobExecutionId = GetJobExecutionId();
            if (jobExecutionId == null || !(obj is StepExecution) || Id == null)
            {
                return base.Equals(obj);
            }
            StepExecution other = (StepExecution)obj;

            return _stepName.Equals(other.StepName) && (jobExecutionId.Equals(other.GetJobExecutionId())
                                                       && Id.Equals(other.Id));
        }

        /// <summary>
        ///     GetHashCode override.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            Object jobExecutionId = GetJobExecutionId();
            long? id = Id;
            return base.GetHashCode() + 31 * (_stepName != null ? _stepName.GetHashCode() : 0) + 91
                   * (jobExecutionId != null ? jobExecutionId.GetHashCode() : 0)
                   + 59 * (id != null ? id.GetHashCode() : 0);
        }

        /// <summary>
        ///     ToString override.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(GetSummary() + ", exitDescription={0}", _exitStatus.ExitDescription);
        }

        /// <summary>
        ///     GetSummary
        /// </summary>
        /// <returns></returns>
        public string GetSummary()
        {
            return base.ToString()
                   + string.Format(
                       ", name={0}, status={1}, exitStatus={2}, readCount={3}, filterCount={4}, writeCount={5} readSkipCount={6}, writeSkipCount={7}"
                       + ", processSkipCount={8}, commitCount={9}, rollbackCount={10}", StepName, BatchStatus,
                       ExitStatus.ExitCode, ReadCount, FilterCount, WriteCount, ReadSkipCount, WriteSkipCount,
                       ProcessSkipCount, CommitCount, RollbackCount);
        }
    }
}