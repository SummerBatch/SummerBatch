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

#region Usings
using NLog;
using Summer.Batch.Core.Explore;
using Summer.Batch.Core.Listener;
using Summer.Batch.Infrastructure.Repeat;
using Summer.Batch.Common.TaskExecution;
using Summer.Batch.Common.Util;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Summer.Batch.Common.Factory;
using Summer.Batch.Core.Scope.Context;

#endregion
namespace Summer.Batch.Core.Step.Tasklet
{
    /// <summary>
    ///<see cref="ITasklet"/>that executes a system command.
    /// The system command is executed asynchronously using injected <see cref="ITaskExecutor"/> - 
    /// timeout value is required to be set, so that the batch job does not hang forever 
    /// if the external process hangs.
    /// Tasklet periodically checks for termination status (i.e.
    /// Command finished its execution or timeout expired or job was interrupted). 
    /// The check interval is given by TerminationCheckInterval.
    /// When job interrupt is detected tasklet's execution is terminated immediately
    /// by throwing JobInterruptedException.
    /// 
    /// NOTE : InterruptOnCancel is not being supported for now.
    /// </summary>
    public class SystemCommandTasklet : StepExecutionListenerSupport, IStoppableTasklet, IInitializationPostOperations
    {
        #region Attributes
        /// <summary>
        /// Logger.
        /// </summary>
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// (System) Command property.
        /// </summary>
        public string Command { private get; set; }

        // arrays of strings, using the name=value pattern, to define environment variables for the process
        /// <summary>
        /// Environment Params
        /// </summary>
        public string[] EnvironmentParams { private get; set; }

        private string _workingDirectory;

        /// <summary>
        /// Working directory property.
        /// </summary>
        public string WorkingDirectory
        {
            set
            {
                if (value == null)
                {
                    _workingDirectory = null;
                    return;
                }
                _workingDirectory = value;
                //Test if Directory path exists and is a directory (single test does both)
                Assert.IsTrue(Directory.Exists(value));
            }
        }

        /// <summary>
        /// System process exit code mapper property.
        /// </summary>
        public ISystemProcessExitCodeMapper SystemProcessExitCodeMapper { private get; set; }
        private long _timeout;//defaults to 0

        //NOTE : Timeout has to be given in ms
        /// <summary>
        /// Timeout property.
        /// </summary>
        public long Timeout { set { _timeout = value; } }

        private long _checkInterval = 1000;

        /// <summary>
        /// Termination check interval property.
        /// </summary>
        public long TerminationCheckInterval { set { _checkInterval = value; } }
        private StepExecution _execution;//defaults to null
        private ITaskExecutor _taskExecutor = new SimpleAsyncTaskExecutor();

        /// <summary>
        /// Task executor property.
        /// </summary>
        public ITaskExecutor TaskExecutor { set { _taskExecutor = value; } }
        private volatile bool _stopped; //defaults to false

        /// <summary>
        /// Job explorer property.
        /// </summary>
        public IJobExplorer JobExplorer { private get; set; }
        private bool _stoppable; //defaults to false
        #endregion

        /// <summary>
        /// @see IInitializationPostOperations#AfterPropertiesSet.
        /// </summary>
        public void AfterPropertiesSet()
        {
            Assert.HasLength(Command, "'command' property value is required");
            Assert.NotNull(SystemProcessExitCodeMapper, "SystemProcessExitCodeMapper must be set");
            Assert.IsTrue(_timeout > 0, "timeout value must be greater than zero");
            Assert.NotNull(_taskExecutor, "taskExecutor is required");
            _stoppable = (JobExplorer != null);
        }

        /// <summary>
        /// IStoppableTasklet#Stop.
        /// </summary>
        public void Stop()
        {
            _stopped = true;
        }

        /// <summary>
        /// Wraps command execution into system process call.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">&nbsp;</exception>
        private int ExecuteCommand()
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe", "/C " + Command)
            {
                UseShellExecute = false,
                WorkingDirectory = _workingDirectory
            };
            if (EnvironmentParams != null)
            {
                foreach (string kvp in EnvironmentParams)
                {
                    //the environnment variables are given using a 'name=value' pattern
                    string[] sep = { "=" };
                    string[] splits = kvp.Split(sep, 2, StringSplitOptions.None);
                    processStartInfo.EnvironmentVariables.Add(splits[0], splits[1]);
                }
            }

            Process process = Process.Start(processStartInfo);
            process.WaitForExit();
            if (Logger.IsTraceEnabled)
            {
                Logger.Trace("Executing the command : {0}", Command);
            }
            if (Logger.IsInfoEnabled)
            {
                Logger.Info("Process execution end with exit status [{0}]", process.ExitCode);
            }
            return process.ExitCode;
        }

        /// <summary>
        /// Execute system command and map its exit code to ExitStatus using <see cref="SystemProcessExitCodeMapper"/>.
        /// </summary>
        /// <param name="contribution"></param>
        /// <param name="chunkContext"></param>
        /// <returns></returns>
        public RepeatStatus Execute(StepContribution contribution, Scope.Context.ChunkContext chunkContext)
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                CancellationToken cancellationToken = cancellationTokenSource.Token;
                using (Task<int> systemCommandTask = new Task<int>(ExecuteCommand, cancellationToken))
                {

                    long t0 = DateTime.Now.Ticks;
                    _taskExecutor.Execute(systemCommandTask);

                    while (true)
                    {
                        Thread.Sleep(new TimeSpan(_checkInterval));

                        CheckStoppingState(chunkContext);

                        if (systemCommandTask.IsCompleted)
                        {
                            return HandleCompletion(contribution, systemCommandTask);
                        }
                        else if (new TimeSpan(DateTime.Now.Ticks - t0).TotalMilliseconds > _timeout)
                        {
                            cancellationTokenSource.Cancel();
                            throw new SystemCommandException(
                                "Execution of system command did not finish within the timeout");
                        }
                        else if (_execution.TerminateOnly)
                        {
                            cancellationTokenSource.Cancel();
                            throw new JobInterruptedException(
                                string.Format("Job interrupted while executing system command '{0}'",Command));
                        }
                        else if (_stopped)
                        {
                            cancellationTokenSource.Cancel();
                            contribution.ExitStatus = ExitStatus.Stopped;
                            return RepeatStatus.Finished;
                        }
                    }
                }
            }

        }

        private RepeatStatus HandleCompletion(StepContribution contribution, Task<int> systemCommandTask)
        {
            contribution.ExitStatus = SystemProcessExitCodeMapper.GetExitStatus(systemCommandTask.Result);
            if (Logger.IsInfoEnabled)
            {
                Logger.Info(
                    "SystemCommandTasklet : System command execution end with exit status [{0}]",
                    contribution.ExitStatus);
            }
            return RepeatStatus.Finished;
        }

        private void CheckStoppingState(ChunkContext chunkContext)
        {
            if (_stoppable)
            {
                JobExecution jobExecution =
                    JobExplorer.GetJobExecution(
                        chunkContext.StepContext.StepExecution.GetJobExecutionId().Value);
                if (jobExecution.IsStopping())
                {
                    _stopped = true;
                }
            }
        }

        /// <summary>
        /// @see IStepExecutionListener#BeforeStep.
        /// </summary>
        /// <param name="stepExecution"></param>
        public override void BeforeStep(StepExecution stepExecution)
        {
            _execution = stepExecution;
        }

    }
}
