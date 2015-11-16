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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using ExecutionContext = Summer.Batch.Infrastructure.Item.ExecutionContext;

namespace Summer.Batch.Core
{
    /// <summary>
    ///  Batch domain object representing the execution of a job.
    /// </summary>
    [Serializable]
    public sealed class JobExecution : Entity
    {
        #region Members
        /// <summary>
        /// Job parameters.
        /// </summary>
        public JobParameters JobParameters { get; private set; }

        /// <summary>
        /// Job instance.
        /// </summary>
        public JobInstance JobInstance { get; set; }

        private readonly IProducerConsumerCollection<StepExecution> _stepExecutions = new ConcurrentBag<StepExecution>();
        
        /// <summary>
        /// Step executions.
        /// </summary>
        public ICollection<StepExecution> StepExecutions
        {
            get
            {
                return new ReadOnlyCollection<StepExecution>(_stepExecutions.Distinct().ToList());
            }
        }

        private volatile BatchStatus _status = BatchStatus.Starting;
        
        /// <summary>
        /// Batch status.
        /// </summary>
        public BatchStatus Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }

        //use MinValue rather than null ...
        private long _startTime = DateTime.MinValue.Ticks;
        
        /// <summary>
        /// Start time.
        /// </summary>
        public DateTime? StartTime
        {
            get
            {
                var startTime = Interlocked.Read(ref _startTime);
                return startTime == DateTime.MinValue.Ticks ? (DateTime?)null : new DateTime(startTime);
            }
            set
            {
                var newValue = value == null ? DateTime.MinValue.Ticks : ((DateTime)value).Ticks;
                Interlocked.Exchange(ref _startTime, newValue);
            }
        }

        private long _createTime = DateTime.Now.Ticks;
        
        /// <summary>
        /// Create time.
        /// </summary>
        public DateTime CreateTime
        {
            get
            {
                return new DateTime(Interlocked.Read(ref _createTime));
            }
            set
            {
                Interlocked.Exchange(ref _createTime, value.Ticks);
            }
        }

        //Use MinValue rather than null
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

        //Use MinValue rather than null
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

        private volatile ExitStatus _exitStatus = ExitStatus.Unknown;
        
        /// <summary>
        /// Exit status.
        /// </summary>
        public ExitStatus ExitStatus
        {
            get
            {
                return _exitStatus;
            }
            set
            {
                _exitStatus = value;
            }
        }


        private volatile ExecutionContext _executionContext = new ExecutionContext();
        
        /// <summary>
        /// Execution context.
        /// </summary>
        public ExecutionContext ExecutionContext
        {
            get
            {
                return _executionContext;
            }
            set
            {
                _executionContext = value;
            }
        }

        [NonSerialized]
        private readonly ICollection<Exception> _failureExceptions = new SynchronizedCollection<Exception>();
        
        /// <summary>
        /// Failure exceptions.
        /// </summary>
        public ICollection<Exception> FailureExceptions
        {
            get { return _failureExceptions; }

        }

        private readonly string _jobConfigurationName;

        /// <summary>
        /// Job configuration name.
        /// </summary>
        public string JobConfigurationName
        {
            get { return _jobConfigurationName; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Custom constructor using JobExecution.
        /// </summary>
        /// <param name="original"></param>
        public JobExecution(JobExecution original)
        {
            JobParameters = original.JobParameters;
            JobInstance = original.JobInstance;
            _stepExecutions = original._stepExecutions;
            _status = original.Status;
            StartTime = original.StartTime;
            CreateTime = original.CreateTime;
            EndTime = original.EndTime;
            LastUpdated = original.LastUpdated;
            _exitStatus = original.ExitStatus;
            _executionContext = original.ExecutionContext;
            _failureExceptions = original.FailureExceptions;
            _jobConfigurationName = original.JobConfigurationName;
            Id = original.Id;
            Version = original.Version;
        }

        /// <summary>
        /// Because a JobExecution isn't valid unless the job is set, this
        /// constructor is the only valid one from a modeling point of view.
        /// </summary>
        /// <param name="job"></param>
        /// <param name="id"></param>
        /// <param name="jobParameters"></param>
        /// <param name="jobConfigurationName"></param>
        public JobExecution(JobInstance job, long? id, JobParameters jobParameters, string jobConfigurationName)
            : base(id)
        {
            JobInstance = job;
            JobParameters = jobParameters ?? new JobParameters();
            _jobConfigurationName = jobConfigurationName;
        }

        /// <summary>
        /// Custom constructor using a JobInstance, JobParameters and a Job configuration name.
        /// </summary>
        /// <param name="job"></param>
        /// <param name="jobParameters"></param>
        /// <param name="jobConfigurationName"></param>
        public JobExecution(JobInstance job, JobParameters jobParameters, string jobConfigurationName) :
            this(job, null, jobParameters, jobConfigurationName) { }

        /// <summary>
        /// Custom constructor using an id, JobParameters and a job configuration name.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="jobParameters"></param>
        /// <param name="jobConfigurationName"></param>
        public JobExecution(long id, JobParameters jobParameters, string jobConfigurationName) :
            this(null, id, jobParameters, jobConfigurationName) { }

        /// <summary>
        ///  Constructor for transient (unsaved) instances.
        /// </summary>
        /// <param name="job"></param>
        /// <param name="jobParameters"></param>
        public JobExecution(JobInstance job, JobParameters jobParameters) : this(job, null, jobParameters, null) { }

        /// <summary>
        /// Custom constructor using an id and JobParameters.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="jobParameters"></param>
        public JobExecution(long id, JobParameters jobParameters) : this(null, id, jobParameters, null) { }

        /// <summary>
        /// Custom constructor using an id.
        /// </summary>
        /// <param name="id"></param>
        public JobExecution(long id) : this(null, id, null, null) { }
        #endregion

        #region Methods
        /// <summary>
        ///Upgrade the status field if the provided value is greater than the
        /// existing one. Clients using this method to set the status can be sure
        /// that they don't overwrite a failed status with an successful one. 
        /// </summary>
        /// <param name="status"></param>
        public void UpgradeStatus(BatchStatus status)
        {
            _status = _status.UpgradeTo(status);
        }

        /// <summary>
        /// Convenience getter for for the id of the enclosing job. Useful for DAO
        /// implementations.
        /// </summary>
        /// <returns></returns>
        public long? GetJobId()
        {
            return JobInstance != null ? JobInstance.Id : null;
        }

        /// <summary>
        /// Registers a step execution with the current job execution.
        /// </summary>
        /// <param name="stepName"></param>
        /// <returns>the name of the step the new execution is associated with</returns>
        public StepExecution CreateStepExecution(string stepName)
        {
            StepExecution stepExecution = new StepExecution(stepName, this);
            _stepExecutions.TryAdd(stepExecution);
            return stepExecution;
        }

        /// <summary>
        /// Tests if this JobExecution indicates that it is running. It should
        /// be noted that this does not necessarily mean that it has been persisted
        /// as such yet. 
        /// </summary>
        /// <returns>true if the job is running</returns>
        public bool IsRunning()
        {
            return _endTime == DateTime.MinValue.Ticks;
        }

        /// <summary>
        /// Tests if this JobExecution indicates that it has been signalled to
        /// stop.
        /// </summary>
        /// <returns>true if the status is BatchStatus#STOPPING</returns>
        public bool IsStopping()
        {
            return _status == BatchStatus.Stopping;
        }

        /// <summary>
        ///  Signals the JobExecution to stop. Iterates through the associated
        /// StepExecutions, calling StepExecution#SetTerminateOnly().
        /// </summary>
        public void Stop()
        {
            foreach (StepExecution stepExecution in _stepExecutions)
            {
                stepExecution.SetTerminateOnly();
            }
            _status = BatchStatus.Stopping;
        }

        /// <summary>
        /// Method for re-constituting the step executions from
        /// existing instances.
        /// </summary>
        /// <param name="stepExecution"></param>
        public void AddStepExecution(StepExecution stepExecution)
        {
            _stepExecutions.TryAdd(stepExecution);
        }

        /// <summary>
        /// Add the provided exception to the failure exception list.
        /// </summary>
        /// <param name="t"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddFailureException(Exception t)
        {
            _failureExceptions.Add(t);
        }

        /// <summary>
        /// Return all failure causing exceptions for this JobExecution, including
        /// step executions. 
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public List<Exception> GetAllFailureExceptions()
        {

            HashSet<Exception> allExceptions = new HashSet<Exception>(_failureExceptions);
            foreach (StepExecution stepExecution in _stepExecutions)
            {
                foreach (Exception ex in stepExecution.GetFailureExceptions())
                {
                    allExceptions.Add(ex);
                }
            }

            return new List<Exception>(allExceptions);
        }

        /// <summary>
        /// Add some step executions.  For internal use only.
        /// </summary>
        /// <param name="stepExecutions">step executions to add to the current list</param>
        public void AddStepExecutions(IList<StepExecution> stepExecutions)
        {
            if (stepExecutions != null)
            {
                foreach (StepExecution item in stepExecutions)
                {
                    StepExecution removedItem = item;
                    _stepExecutions.TryTake(out removedItem);
                }
                foreach (StepExecution item in stepExecutions)
                {
                    _stepExecutions.TryAdd(item);
                }
            }
        }
        #endregion

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return
                string.Format(
                    "{0}, startTime={1:O}, endTime={2:O}, lastUpdated={3:O}, status={4}, exitStatus={5}, job=[{6}], jobParameters=[{7}]",
                    base.ToString(), StartTime, EndTime, LastUpdated, _status, _exitStatus, JobInstance, JobParameters);
        }
    }
}
