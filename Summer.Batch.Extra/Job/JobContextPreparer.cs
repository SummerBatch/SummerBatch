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

using System.Collections.Generic;
using Summer.Batch.Core;
using Summer.Batch.Infrastructure.Item;

namespace Summer.Batch.Extra.Job
{
    /// <summary>
    /// Prepares the job context by inserting job parameters in it
    /// </summary>
    public class JobContextPreparer : IJobExecutionListener
    {
        /// <summary>
        /// injected properties
        /// </summary>
        public IDictionary<string, string> Properties { private get; set; } 

        /// <summary>
        /// Launched before the job. Fills the job context with the injected properties.
        /// </summary>
        /// <param name="jobExecution"></param>
        public void BeforeJob(JobExecution jobExecution)
        {
            ExecutionContext context = jobExecution.ExecutionContext;
            if (Properties != null)
            {
                foreach (KeyValuePair<string,string> entry in Properties)
                {
                    context.PutString(entry.Key,entry.Value);
                }
            }
        }

        /// <summary>
        /// Launched after the job. Not used, thus does nothing.
        /// </summary>
        /// <param name="jobExecution"></param>
        public void AfterJob(JobExecution jobExecution)
        {
           //do nothing, on purpose.
        }
    }
}