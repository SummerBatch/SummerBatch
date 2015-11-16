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

using System.Collections.Generic;
using System.Linq;
using NLog;
using Summer.Batch.Core.Repository;

namespace Summer.Batch.Core.Job.Builder
{
    /// <summary>
    /// A base class and utility for other job builders providing access to common properties like job repository.
    /// </summary>
    public abstract class JobBuilderHelper
    {

        /// <summary>
        /// Logger
        /// </summary>
        protected readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Common job properties.
        /// </summary>
        protected readonly CommonJobProperties Properties;

        /// <summary>
        /// Custom constructor using a name.
        /// </summary>
        /// <param name="name"></param>
        protected JobBuilderHelper(string name)
        {
            Properties = new CommonJobProperties { Name = name };
        }

        /// <summary>
        /// Create a new builder initialized with any properties in the parent. 
        /// The parent is copied, so it can be re-used
        /// </summary>
        /// <param name="parent"></param>
        protected JobBuilderHelper(JobBuilderHelper parent)    
        {
            Properties = new CommonJobProperties(parent.Properties);
        }

        /// <summary>
        /// Sets the job repository for the job.
        /// </summary>
        /// <param name="jobRepository"></param>
        /// <returns></returns>
        public JobBuilderHelper Repository(IJobRepository jobRepository)
        {
            Properties.JobRepository = jobRepository;
            return this;
        }

        /// <summary>
        /// Add a job parameters incrementer.
        /// </summary>
        /// <param name="jobParametersIncrementer"></param>
        /// <returns></returns>
        public JobBuilderHelper Incrementer(IJobParametersIncrementer jobParametersIncrementer)
        {
            Properties.JobParametersIncrementer = jobParametersIncrementer;
            return this;
        }

        /// <summary>
        /// Add a job parameters validator.
        /// </summary>
        /// <param name="jobParametersValidator"></param>
        /// <returns></returns>
        public JobBuilderHelper Validator(IJobParametersValidator jobParametersValidator)
        {
            Properties.JobParametersValidator = jobParametersValidator;
            return this;
        }

        /// <summary>
        /// Register a job execution listener.
        /// </summary>
        /// <param name="listener"></param>
        /// <returns></returns>
        public JobBuilderHelper Listener(IJobExecutionListener listener)
        {
            Properties.AddJobExecutionListener(listener);
            return this;
        }

        /// <summary>
        /// Set a flag to prevent restart an execution of this job even if it has failed.
        /// </summary>
        /// <returns></returns>
        public JobBuilderHelper PreventRestart()
        {
            Properties.Restartable = false;
            return this;
        }

        /// <summary>
        /// Return the name.
        /// </summary>
        /// <returns></returns>
        protected internal string GetName()
        {
            return Properties.Name;
        }

        /// <summary>
        /// Returns the job repository.
        /// </summary>
        /// <returns></returns>
        protected IJobRepository GetJobRepository()
        {
            return Properties.JobRepository;
        }

        /// <summary>
        /// Returns the restartable flag.
        /// </summary>
        /// <returns></returns>
        protected bool IsRestartable()
        {
            return Properties.Restartable;
        }

        /// <summary>
        /// Enhances the given job with incrementer, validator and job execution listeners, if provided.
        /// </summary>
        /// <param name="target"></param>
        protected void Enhance(IJob target)
        {
            var job = target as AbstractJob;
            if (job != null)
            {
                job.Restartable = Properties.Restartable;
                job.JobRepository = Properties.JobRepository;

                IJobParametersIncrementer jobParametersIncrementer = Properties.JobParametersIncrementer;
                if (jobParametersIncrementer != null)
                {
                    job.JobParametersIncrementer = jobParametersIncrementer;
                }

                IJobParametersValidator jobParametersValidator = Properties.JobParametersValidator;
                if (jobParametersValidator != null)
                {
                    job.JobParametersValidator = jobParametersValidator;
                }

                List<IJobExecutionListener> listeners = Properties.JobExecutionListeners;
                if (listeners.Any())
                {
                    job.SetJobExecutionListeners(listeners.ToArray());
                }
            }
        }

        #region CommonJobProperties class
        /// <summary>
        /// common job properties.
        /// </summary>
        public class CommonJobProperties
        {
            /// <summary>
            /// Name property.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Job repository property.
            /// </summary>
            public IJobRepository JobRepository { get; set; }

            /// <summary>
            /// Job parameters incrementer property.
            /// </summary>
            public IJobParametersIncrementer JobParametersIncrementer { get; set; }

            /// <summary>
            /// Job parameters validator property.
            /// </summary>
            public IJobParametersValidator JobParametersValidator { get; set; }

            /// <summary>
            /// Restartable flag property.
            /// </summary>
            public bool Restartable { get; set; }

            private readonly List<IJobExecutionListener> _jobExecutionListeners = new List<IJobExecutionListener>();

            /// <summary>
            /// List of job exeuction listeners property.
            /// </summary>
            public List<IJobExecutionListener> JobExecutionListeners { get { return _jobExecutionListeners; } }

            /// <summary>
            /// Default constructor.
            /// </summary>
            public CommonJobProperties()
            {
                //default value
                Restartable = true;
            }

            /// <summary>
            /// Custom constructor with properties.
            /// </summary>
            /// <param name="properties"></param>
            public CommonJobProperties(CommonJobProperties properties)
            {
                Name = properties.Name;
                Restartable = properties.Restartable;
                JobRepository = properties.JobRepository;
                JobParametersIncrementer = properties.JobParametersIncrementer;
                JobParametersValidator = properties.JobParametersValidator;
                _jobExecutionListeners = new List<IJobExecutionListener>(properties.JobExecutionListeners);
            }

            /// <summary>
            /// Method to add a list of job execution listeners.
            /// </summary>
            /// <param name="jobExecutionListeners"></param>
            public void AddJobExecutionListeners(List<IJobExecutionListener> jobExecutionListeners)
            {
                _jobExecutionListeners.AddRange(jobExecutionListeners);
            }

            /// <summary>
            /// Method to add a single job execution listener.
            /// </summary>
            /// <param name="jobExecutionListener"></param>
            public void AddJobExecutionListener(IJobExecutionListener jobExecutionListener)
            {
                _jobExecutionListeners.Add(jobExecutionListener);
            }

        }
        #endregion
    }
}