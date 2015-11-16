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

using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using Summer.Batch.Core.Explore;
using Summer.Batch.Core.Launch;
using Summer.Batch.Core.Launch.Support;
using Summer.Batch.Core.Unity;
using Summer.Batch.Core.Unity.Xml;

namespace Summer.Batch.Core
{
    /// <summary>
    /// Job starter. Used to start or re-start jobs
    /// </summary>
    public static class JobStarter
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Starts given job.
        /// </summary>
        /// <param name="xmlJobFile"></param>
        /// <param name="loader"></param>
        /// <returns></returns>
        public static JobExecution Start(string xmlJobFile, UnityLoader loader)
        {
            var job = XmlJobParser.LoadJob(xmlJobFile);
            loader.Job = job;
            var jobOperator = (SimpleJobOperator)BatchRuntime.GetJobOperator(loader);
            var executionId = jobOperator.StartNextInstance(job.Id);
            return jobOperator.JobExplorer.GetJobExecution((long)executionId);
        }

        /// <summary>
        /// Restarts given job.
        /// </summary>
        /// <param name="xmlJobFile"></param>
        /// <param name="loader"></param>
        /// <returns></returns>
        public static JobExecution ReStart(string xmlJobFile, UnityLoader loader)
        {
            var job = XmlJobParser.LoadJob(xmlJobFile);
            loader.Job = job;
            var jobOperator = (SimpleJobOperator)BatchRuntime.GetJobOperator(loader);
            var jobExecution = GetLastFailedJobExecution(job.Id, jobOperator.JobExplorer);
            if (jobExecution == null)
            {
                throw new JobExecutionNotFailedException(
                    String.Format("No failed or stopped execution found for job={0}" , job.Id));
            }
            var executionId = jobOperator.Restart(jobExecution.Id.Value);
            return jobOperator.JobExplorer.GetJobExecution((long)executionId);
        }

        /// <summary>
        /// Stops a given running job.
        /// </summary>
        /// <param name="xmlJobFile"></param>
        /// <param name="loader"></param>
        public static void Stop(string xmlJobFile, UnityLoader loader)
        {
            var job = XmlJobParser.LoadJob(xmlJobFile);
            loader.Job = job;
            var jobOperator = (SimpleJobOperator)BatchRuntime.GetJobOperator(loader);
            var jobExecutions = GetRunningJobExecutions(job.Id, jobOperator.JobExplorer);
            if (jobExecutions == null || !jobExecutions.Any())
            {
                throw new JobExecutionNotFailedException(
                    string.Format("No running execution found for job={0}", job.Id));
            }
            foreach (var jobExecution in jobExecutions)
            {
                jobExecution.Status = BatchStatus.Stopping;
                jobOperator.JobRepository.Update(jobExecution);
            }
            Logger.Info("Job {0} was stopped.", job.Id);
        }

        /// <summary>
        /// Abandons a given running job.
        /// </summary>
        /// <param name="xmlJobFile"></param>
        /// <param name="loader"></param>
        public static void Abandon(string xmlJobFile, UnityLoader loader)
        {
            var job = XmlJobParser.LoadJob(xmlJobFile);
            loader.Job = job;
            var jobOperator = (SimpleJobOperator)BatchRuntime.GetJobOperator(loader);
            var jobExecutions = GetStoppedJobExecutions(job.Id, jobOperator.JobExplorer);
            if (jobExecutions == null || !jobExecutions.Any())
            {
                throw new JobExecutionNotFailedException(
                    String.Format("No stopped execution found for job={0}", job.Id));
            }
            foreach (var jobExecution in jobExecutions)
            {
                jobExecution.Status = BatchStatus.Abandoned;
                jobOperator.JobRepository.Update(jobExecution);
            }
            Logger.Info("Job {0} was abandoned.", job.Id);
        }

        /// <summary>
        /// Retrieves the last failed job execution for the given job identifier;
        /// Might be null if not found.
        /// </summary>
        /// <param name="jobIdentifier"></param>
        /// <param name="jobExplorer"></param>
        /// <returns></returns>
        private static JobExecution GetLastFailedJobExecution(string jobIdentifier, IJobExplorer jobExplorer)
        {
            List<JobExecution> jobExecutions =
                GetJobExecutionsWithStatusGreaterThan(jobIdentifier, BatchStatus.Stopping, jobExplorer);
            if (!jobExecutions.Any())
            {
                return null;
            }
            return jobExecutions[0];
        }

        /// <summary>
        /// Retrieves the running job executions for the given job identifier;
        /// Might be null if nothing found.
        /// </summary>
        /// <param name="jobIdentifier"></param>
        /// <param name="jobExplorer"></param>
        /// <returns></returns>
        private static List<JobExecution> GetRunningJobExecutions(string jobIdentifier, IJobExplorer jobExplorer)
        {
            List<JobExecution> jobExecutions =
                GetJobExecutionsWithStatusGreaterThan(jobIdentifier, BatchStatus.Completed, jobExplorer);
            if (!jobExecutions.Any())
            {
                return null;
            }
            return jobExecutions.Where(jobExecution => jobExecution.IsRunning()).ToList();
        }

        /// <summary>
        /// Retrieves the stopped job executions for the given job identifier;
        /// Might be null if nothing found.
        /// </summary>
        /// <param name="jobIdentifier"></param>
        /// <param name="jobExplorer"></param>
        /// <returns></returns>
        private static List<JobExecution> GetStoppedJobExecutions(string jobIdentifier, IJobExplorer jobExplorer)
        {
            List<JobExecution> jobExecutions =
                GetJobExecutionsWithStatusGreaterThan(jobIdentifier, BatchStatus.Started, jobExplorer);
            if (!jobExecutions.Any())
            {
                return null;
            }
            return jobExecutions.Where(jobExecution => jobExecution.Status != BatchStatus.Abandoned).ToList();
        }

        /// <summary>
        /// Returns the given jobidentifier as long or null if conversion cannot be completed.
        /// </summary>
        /// <param name="jobIdentifier"></param>
        /// <returns></returns>
        private static long? GetLongIdentifier(string jobIdentifier)
        {
            try
            {
                return Convert.ToInt64(jobIdentifier);
            }
            catch (Exception)
            {
                // Not an ID - must be a name
                return null;
            }
        }

        /// <summary>
        /// Retrieves all job executions for a given minimal status 
        /// </summary>
        /// <param name="jobIdentifier"></param>
        /// <param name="minStatus"></param>
        /// <param name="jobExplorer"></param>
        /// <returns></returns>
        private static List<JobExecution> GetJobExecutionsWithStatusGreaterThan(string jobIdentifier, BatchStatus minStatus, IJobExplorer jobExplorer)
        {

            long? executionId = GetLongIdentifier(jobIdentifier);
            if (executionId != null)
            {
                JobExecution jobExecution = jobExplorer.GetJobExecution(executionId.Value);
                if (jobExecution.Status.IsGreaterThan(minStatus))
                {
                    return new List<JobExecution> { jobExecution };
                }
                //empmty list
                return new List<JobExecution>();
            }

            int start = 0;
            int count = 100;
            List<JobExecution> executions = new List<JobExecution>();
            IList<JobInstance> lastInstances = jobExplorer.GetJobInstances(jobIdentifier, start, count);

            while (lastInstances.Any())
            {

                foreach (JobInstance jobInstance in lastInstances)
                {
                    IList<JobExecution> jobExecutions = jobExplorer.GetJobExecutions(jobInstance);
                    if (jobExecutions == null || !jobExecutions.Any())
                    {
                        continue;
                    }
                    executions.AddRange(jobExecutions.Where(jobExecution => jobExecution.Status.IsGreaterThan(minStatus)));
                }

                start += count;
                lastInstances = jobExplorer.GetJobInstances(jobIdentifier, start, count);

            }

            return executions;

        }

        /// <summary>
        /// Enum of possible results.
        /// </summary>
        public enum Result
        {
            /// <summary>
            /// Success enum litteral
            /// </summary>
            Success = 0,
            /// <summary>
            /// Failed enum litteral
            /// </summary>
            Failed = 1,
            /// <summary>
            /// InvalidOption enum litteral
            /// </summary>
            InvalidOption = 2
        }
    }
}
