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


#region Usings
using NLog;
using Summer.Batch.Core.Explore;
using Summer.Batch.Core.Listener;
using Summer.Batch.Infrastructure.Repeat;
using Summer.Batch.Common.TaskExecution;
using Summer.Batch.Common.Util;
using Summer.Batch.Common.IO;
using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Summer.Batch.Common.Factory;
using Summer.Batch.Core.Scope.Context;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Transactions;

#endregion
namespace Summer.Batch.Core.Step.Tasklet
{
    /// <summary>
    /// An <see cref="ITasklet"/> that runs a powershell script.
    /// The script is executed asynchronously using injected <see cref="ITaskExecutor"/> - 
    /// timeout value is required to be set, so that the batch job does not hang forever 
    /// if the external process hangs.
    /// Tasklet periodically checks for termination status (i.e.
    /// Script finished its execution or timeout expired or job was interrupted). 
    /// The check interval is given by TerminationCheckInterval.
    /// When job interrupt is detected tasklet's execution is terminated immediately
    /// by throwing JobInterruptedException.
    /// 
    /// NOTE : InterruptOnCancel is not being supported for now.
    /// \since 1.1.0
    /// </summary>
    public class PowerShellTasklet : StepExecutionListenerSupport, IStoppableTasklet, IInitializationPostOperations
    {
        #region Attributes
        /// <summary>
        /// Logger.
        /// </summary>
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Enumeration of the possible Timeout Bahavior Options...
        /// </summary>
        public enum TimeoutBehaviorOption
        {
            /// <summary>
            /// Will Set Step ExitStatus to ExitStatus.Failed if script execution times out...
            /// </summary>
            SetExitStatusToFailed,
            /// <summary>
            /// Will thow exception if script execution times out...
            /// </summary>
            ThrowException
        }

        /// <summary>
        /// ScriptResource (i.e. Script File to be exeuted in PowerShell runspace)
        /// </summary>
        public IResource ScriptResource { private get; set; }

        /// <summary>
        /// Parameters for powershell script file being executed...i.e. passed on the command line, accessed via Param()
        /// </summary>
        public IDictionary<string, object> Parameters { private get; set; }

        /// <summary>
        /// Variables for powershell script file being executed...available in PowerShell Runspace to the script
        /// </summary>
        public IDictionary<string, object> Variables { private get; set; }

        /// <summary>
        /// System process exit code mapper property.
        /// </summary>
        public IPowerShellExitCodeMapper PowerShellExitCodeMapper { private get; set; }

        private long _timeout;//defaults to 0

        //NOTE : Timeout has to be given in ms
        /// <summary>
        /// Timeout property.
        /// </summary>
        public long Timeout { set { _timeout = value; } }

        private TimeoutBehaviorOption _timeoutBehavior = TimeoutBehaviorOption.SetExitStatusToFailed;
        /// <summary>
        /// FileType flag property.
        /// </summary>
        public TimeoutBehaviorOption TimeoutBehavior
        {
            get { return _timeoutBehavior; }
            set { _timeoutBehavior = value; }
        }

        private long _checkInterval = 1000;
        /// <summary>
        /// Termination check interval property.
        /// </summary>
        public long TerminationCheckInterval { set { _checkInterval = value; } }

        private StepExecution _execution;//defaults to null
        private volatile bool _stopped; //defaults to false

        /// <summary>
        /// Job explorer property.
        /// </summary>
        public IJobExplorer JobExplorer { private get; set; }
        private bool _stoppable; //defaults to false

        //=> Exit status to pass to powershell runspace...
        private ExitStatus _scriptExitStatus;
        
        //=> string buiders...
        private readonly StringBuilder _sbOutput = new StringBuilder();
        private readonly StringBuilder _sbVerbose = new StringBuilder();
        private readonly StringBuilder _sbError = new StringBuilder();
        private readonly StringBuilder _sbWarning = new StringBuilder();
        private readonly StringBuilder _sbDebug = new StringBuilder();

        #endregion

        /// <summary>
        /// @see IInitializationPostOperations#AfterPropertiesSet.
        /// </summary>
        public void AfterPropertiesSet()
        {
            //=> make sure source exists...
            Assert.State(ScriptResource.Exists(), ScriptResource.GetDescription() + " does not exist.");
            Assert.IsTrue(_timeout > 0, "timeout value must be greater than zero");
            _stoppable = (JobExplorer != null);

            //=> initialize string builders...
            _sbOutput.AppendLine(Environment.NewLine + "*** PowerShell Write-Output Stream ***");
            _sbVerbose.AppendLine(Environment.NewLine + "*** PowerShell Write-Verbose Stream ***");
            _sbError.AppendLine(Environment.NewLine + "*** PowerShell Write-Error Stream ***");
            _sbWarning.AppendLine(Environment.NewLine + "*** PowerShell Write-Warning Stream ***");
            _sbDebug.AppendLine(Environment.NewLine + "*** PowerShell Write-Debug Stream ***");
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
        /// <exception cref="Exception"></exception>
        public RepeatStatus Execute(StepContribution contribution, ChunkContext chunkContext)
        {
            if (Logger.IsTraceEnabled)
            {
                Logger.Trace("*** Executing PowerShell Script File: {0}", ScriptResource.GetFullPath());
            }

            //=> PowerShell will throw an error if we do not Suppress ambient transaction...
            //   see https://msdn.microsoft.com/en-us/library/system.transactions.transaction.current(v=vs.110).aspx#NotExistJustToMakeTheAElementVisible
            using (var transactionScope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                //=> Runspace configuration information includes the assemblies, commands, format and type files, 
                //   providers, and scripts that are available within the runspace.
                RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();
                
                //Creates a single runspace that uses the default host and runspace configuration
                using (Runspace runSpace = RunspaceFactory.CreateRunspace(runspaceConfiguration))
                {
                    //=> When this runspace is opened, the default host and runspace configuration 
                    //   that are defined by Windows PowerShell will be used. 
                    runSpace.Open();

                    //=> Set Variables so they are available to user script...
                    if (Variables != null  && Variables.Any())
                    {
                        foreach (KeyValuePair<string, object> variable in Variables)
                        {
                            runSpace.SessionStateProxy.SetVariable(variable.Key, variable.Value);
                        }
                    }

                    //=> this is exit status variables to be tested on exit from power shell script...
                    //   it is defined in PwerShell global scope...and must be set by scipt writer on exit...
                    runSpace.SessionStateProxy.SetVariable("ScriptExitStatus", _scriptExitStatus);

                    //=> Allows the execution of commands from a CLR
                    //RunspaceInvoke scriptInvoker = new RunspaceInvoke(runSpace);
                    //scriptInvoker.Invoke("Set-ExecutionPolicy Unrestricted"); 

                    using (PowerShell psInstance = PowerShell.Create())
                    {
                        try
                        {
                            // prepare a new collection to store output stream objects
                            PSDataCollection<PSObject> outputCollection = new PSDataCollection<PSObject>();
                            outputCollection.DataAdded += AllStreams_DataAdded;
                            psInstance.Streams.Error.DataAdded += AllStreams_DataAdded;
                            psInstance.Streams.Verbose.DataAdded += AllStreams_DataAdded;
                            psInstance.Streams.Warning.DataAdded += AllStreams_DataAdded;
                            psInstance.Streams.Debug.DataAdded += AllStreams_DataAdded;

                            psInstance.Runspace = runSpace;

                            //=> This tasklet should be in the same dll as ExitStatus, i.e. Summer.Batch.Core.dll 
                            //   we need to get the path to loaded Summer.Batch.Core.dll so we can load it in PowerShell
                            var assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;

                            //=> need to load Summer.Batch.Core into runspace so we can reference ExitStatus
                            psInstance.AddScript("[System.Reflection.Assembly]::LoadFrom(\""+assemblyLocation+"\")").AddStatement();

                            //=> add user command and its parameters...
                            psInstance.AddCommand(ScriptResource.GetFullPath());
                            if (Parameters != null && Parameters.Any())
                            {
                                foreach (KeyValuePair<string, object> variable in Parameters)
                                {
                                    psInstance.AddParameter(variable.Key, variable.Value);
                                }
                            }

                            //=> Invoke Asynchronously...
                            IAsyncResult asyncResult = psInstance.BeginInvoke<PSObject, PSObject>(null, outputCollection);

                            // do something else until execution has completed.
                            long t0 = DateTime.Now.Ticks;
                            while (!asyncResult.IsCompleted)
                            {
                                //=> take a nap and let script do its job...
                                Thread.Sleep(new TimeSpan(_checkInterval));
                                
                                //=> to check if job was told to stop...
                                CheckStoppingState(chunkContext);

                                //=> lets make sure we did not exceed alloted time...
                                long timeFromT0 = (long)(new TimeSpan(DateTime.Now.Ticks - t0)).TotalMilliseconds;
                                if (timeFromT0 > _timeout)
                                {
                                    //=> Stop PowerShell...
                                    psInstance.Stop();

                                    //=> behave based on TimeoutBehaviorOption
                                    if (_timeoutBehavior.Equals(TimeoutBehaviorOption.SetExitStatusToFailed))
                                    {
                                        contribution.ExitStatus = ExitStatus.Failed;
                                        break;
                                    }
                                    else if (_timeoutBehavior.Equals(TimeoutBehaviorOption.ThrowException))
                                    {
                                        //=> lets dump what we got before throwing an error...
                                        LogStreams();
                                        throw new FatalStepExecutionException("Execution of PowerShell script exceeded allotted time.", null);
                                    }
                                }
                                else if (_execution.TerminateOnly)
                                {
                                    //=> Stop PowerShell...
                                    psInstance.Stop();

                                    //=> lets dump what we got before throwing an error...
                                    LogStreams();

                                    throw new JobInterruptedException(
                                        string.Format("Job interrupted while executing PowerShell script '{0}'", ScriptResource.GetFilename()));
                                }
                                else if (_stopped)
                                {
                                    psInstance.Stop();
                                    contribution.ExitStatus = ExitStatus.Stopped;
                                    break;
                                }
                            } // end while scope

                            //=> Wait to the end of execution...
                            //psInstance.EndInvoke(_asyncResult);

                            //NOTE: asyncResult.IsCompleted will be set to true if PowerShell.Stop was called or
                            //      PowerShell completed its work

                            //=> if status not yet set (script completed)...handle completion...
                            if (contribution.ExitStatus.IsRunning())
                            {
                                //=> script needs to set exit code...if exit code not set we assume 0
                                var lastExitCode = (int)runSpace.SessionStateProxy.PSVariable.GetValue("LastExitCode", 0);

                                _scriptExitStatus = runSpace.SessionStateProxy.GetVariable("ScriptExitStatus") as ExitStatus;

                                //=> set exit status...
                                if (_scriptExitStatus != null && !_scriptExitStatus.IsRunning())
                                {
                                    if (Logger.IsTraceEnabled)
                                    {
                                        Logger.Trace("***> ScriptExitStatus returned by script => {0}", _scriptExitStatus);
                                    }

                                    contribution.ExitStatus = _scriptExitStatus;
                                }
                                else //=> let user decide on ExitStatus
                                {
                                    if (Logger.IsTraceEnabled)
                                    {
                                        if (_scriptExitStatus == null)
                                        {
                                            Logger.Trace("***> ScriptExitStatus is null. Using PowerShellExitCodeMapper to determine ExitStatus.");
                                        }
                                        else if (_scriptExitStatus.IsRunning())
                                        {
                                            Logger.Trace("***> ScriptExitStatus is EXECUTING or UNKNOWN. Using PowerShellExitCodeMapper to determine ExitStatus.");
                                        }                        
                                    }

                                    if (PowerShellExitCodeMapper != null)
                                    {
                                        //=> determine exit status using User Provided PowerShellExitCodeMapper
                                        contribution.ExitStatus = PowerShellExitCodeMapper.GetExitStatus(lastExitCode);
                                    }
                                    else //at this point we are not able to determine exit status, user needs to fix this...
                                    {
                                        //=> lets dump what we got before throwing an error...
                                        LogStreams();
                                        throw new FatalStepExecutionException(
                                            "PowerShellTasklet is not able to determine ExitStatus. ScriptExitStatus is null or (is EXECUTING or UNKNOWN) and "+
                                            "PowerShellExitCodeMapper is NOT defined. Please set $global:ScriptExitStatus or define PowerShellExitCodeMapper.", null);
                                    }

                                }
                            }

                            if (Logger.IsInfoEnabled)
                            {
                                Logger.Info("PowerShell execution exit status [{0}]", contribution.ExitStatus);
                            }

                            //=> output captured stream data to Log...
                            LogStreams();
                        }
                        catch (RuntimeException ex)
                        {
                            Logger.Error(ex.Message);
                            throw;
                        }

                    } // end PowerShell Scope

                    //=> close Runspace...
                    runSpace.Close();

                    //=> we are done...
                    return RepeatStatus.Finished;

                } // end of Runspace Scope

            }// end of TransactionScope
        }

        private void LogStreams()
        {
            if (Logger.IsTraceEnabled)
            {
                Logger.Trace(_sbVerbose.ToString());
            }

            if (Logger.IsInfoEnabled)
            {
                Logger.Info(_sbOutput.ToString());
            }

            if (Logger.IsWarnEnabled)
            {
                Logger.Warn(_sbWarning.ToString());
            }

            if (Logger.IsErrorEnabled)
            {
                Logger.Error(_sbError.ToString());
            }

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug(_sbDebug.ToString());
            }
        }

        private void CheckStoppingState(ChunkContext chunkContext)
        {
            if (_stoppable)
            {
                JobExecution jobExecution = JobExplorer.GetJobExecution(chunkContext.StepContext.StepExecution.GetJobExecutionId().Value);
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
            _scriptExitStatus = _execution.ExitStatus;
        }

        /// <summary>
        /// Event handler for when data is added to the streams, i.e. all streams redirect to this handler.
        /// </summary>
        /// <param name="sender">Contains the complete PSDataCollection of all output items.</param>
        /// <param name="e">Contains the index ID of the added collection item and the ID of the PowerShell instance this event belongs to.</param>
        void AllStreams_DataAdded(object sender, DataAddedEventArgs e)
        {
            var outputStream = sender as PSDataCollection<PSObject>;
            var verboseStream = sender as PSDataCollection<VerboseRecord>;
            var errorStream = sender as PSDataCollection<ErrorRecord>;
            var warningStream = sender as PSDataCollection<WarningRecord>;
            var debugStream = sender as PSDataCollection<DebugRecord>;

            if (outputStream != null)
            {
                if (outputStream[e.Index] != null)
                {
                    _sbOutput.AppendLine(outputStream[e.Index].ToString());
                }
            }
            else if (verboseStream != null)
            {
                if (verboseStream[e.Index] != null)
                {
                    _sbVerbose.AppendLine(verboseStream[e.Index].ToString());
                }
            }
            else if (errorStream != null)
            {
                if (errorStream[e.Index] != null)
                {
                    _sbError.AppendLine(errorStream[e.Index].ToString());
                }
            }
            else if (warningStream != null)
            {
                if (warningStream[e.Index] != null)
                {
                    _sbWarning.AppendLine(warningStream[e.Index].ToString());
                }
            }
            else if (debugStream != null)
            {
                if (debugStream[e.Index] != null)
                {
                    _sbDebug.AppendLine(debugStream[e.Index].ToString());
                }
            }
            else
            {
                _sbOutput.AppendLine("Data is comming from stream: " + sender.GetType().FullName);
            }
        }

    }
}
