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

using Summer.Batch.Infrastructure.Item;
using System;
using System.Collections.Generic;

namespace Summer.Batch.Core.Step.Job
{
    /// <summary>
    /// Simple implementation of <see cref="IJobParametersExtractor"/> which pulls
    /// parameters with named keys out of the step execution context and the job
    /// parameters of the surrounding job.
    /// </summary>
    public class DefaultJobParametersExtractor : IJobParametersExtractor
    {

        private HashSet<string> _keys = new HashSet<string>();
        private bool _useAllParentParameters = true;

        /// <summary>
        /// Use all parent parameters flag property.
        /// </summary>
        public bool UseAllParentParameters { set { _useAllParentParameters = value; } }

        /// <summary>
        /// The key names to pull out of the execution context or job parameters, if
        /// they exist. If a key doesn't exist in the execution context then the job
        /// parameters from the enclosing job execution are tried, and if there is
        /// nothing there either then no parameter is extracted. Key names ending
        /// with <code>(long)</code>, <code>(int)</code>, <code>(double)</code>,
        /// <code>(date)</code> or <code>(string)</code> will be assumed to refer to
        /// values of the respective type and assigned to job parameters accordingly
        /// (there will be an error if they are not of the right type). Without a
        ///special suffix in that form a parameter is assumed to be of type String.
        /// </summary>
        /// <param name="keys"></param>
        public void SetKeys(string[] keys)
        {
            _keys = new HashSet<string>(keys);
        }

        /// <summary>
        /// @see IJobParametersExtractor#GetJobParameters(Job, StepExecution).
        /// </summary>
        /// <param name="job"></param>
        /// <param name="stepExecution"></param>
        /// <returns></returns>
        public JobParameters GetJobParameters(IJob job, StepExecution stepExecution)
        {
            JobParametersBuilder builder = new JobParametersBuilder();
            IDictionary<string, JobParameter> jobParameters = stepExecution.GetJobParameters().GetParameters();
            ExecutionContext executionContext = stepExecution.ExecutionContext;
            if (_useAllParentParameters)
            {
                foreach (string key in jobParameters.Keys)
                {
                    builder.AddParameter(key, jobParameters[key]);
                }
            }
            foreach (string akey in _keys)
            {
                string key = akey;
                if (key.EndsWith("(long)") || key.EndsWith("(int)"))
                {
                    HandleLongOrIntKey(key, executionContext, builder, jobParameters);
                }
                else if (key.EndsWith("(double)"))
                {
                    HandleDoubleKey(key, executionContext, builder, jobParameters);
                }
                else if (key.EndsWith("(string)"))
                {
                    HandleStringKey(key, executionContext, builder, jobParameters);
                }
                else if (key.EndsWith("(date)"))
                {
                    HandleDateKey(key, executionContext, builder, jobParameters);
                }
                else
                {
                    DefaultHandle(executionContext, key, builder, jobParameters);
                }
            }
            return builder.ToJobParameters();

        }

        private static void DefaultHandle(ExecutionContext executionContext, string key, JobParametersBuilder builder,
            IDictionary<string, JobParameter> jobParameters)
        {
            if (executionContext.ContainsKey(key))
            {
                builder.AddString(key, executionContext.Get(key).ToString());
            }
            else if (jobParameters.ContainsKey(key))
            {
                builder.AddString(key, jobParameters[key].Value.ToString());
            }
        }

        private static void HandleDateKey(string key, ExecutionContext executionContext, JobParametersBuilder builder,
            IDictionary<string, JobParameter> jobParameters)
        {
            string akey = key.Replace("(date)", "");
            if (executionContext.ContainsKey(akey))
            {
                builder.AddDate(akey, (DateTime)executionContext.Get(akey));
            }
            else if (jobParameters.ContainsKey(akey))
            {
                builder.AddDate(akey, (DateTime)jobParameters[akey].Value);
            }
        }

        private static void HandleStringKey(string key, ExecutionContext executionContext, JobParametersBuilder builder,
            IDictionary<string, JobParameter> jobParameters)
        {
            String akey = key.Replace("(string)", "");
            if (executionContext.ContainsKey(akey))
            {
                builder.AddString(akey, executionContext.GetString(akey));
            }
            else if (jobParameters.ContainsKey(akey))
            {
                builder.AddString(akey, (string)jobParameters[akey].Value);
            }
        }

        private static void HandleDoubleKey(string key, ExecutionContext executionContext, JobParametersBuilder builder,
            IDictionary<string, JobParameter> jobParameters)
        {
            string akey = key.Replace("(double)", "");
            if (executionContext.ContainsKey(akey))
            {
                builder.AddDouble(akey, executionContext.GetDouble(akey));
            }
            else if (jobParameters.ContainsKey(akey))
            {
                builder.AddDouble(akey, (Double)jobParameters[akey].Value);
            }
        }

        private static void HandleLongOrIntKey(string key, ExecutionContext executionContext, JobParametersBuilder builder,
            IDictionary<string, JobParameter> jobParameters)
        {
            bool isLong = key.EndsWith("(long)");
            string akey = isLong ? key.Replace("(long)", "") : key.Replace("(int)", "");
            if (executionContext.ContainsKey(akey))
            {
                builder.AddLong(akey, isLong ? executionContext.GetLong(akey) : executionContext.GetInt(akey));
            }
            else if (jobParameters.ContainsKey(akey))
            {
                builder.AddLong(akey, (long)jobParameters[akey].Value);
            }
        }
    }
}