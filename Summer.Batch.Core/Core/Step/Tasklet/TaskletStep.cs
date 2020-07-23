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

using NLog;
using Summer.Batch.Core.Scope.Context;
using Summer.Batch.Infrastructure.Item;
using Summer.Batch.Infrastructure.Item.Support;
using Summer.Batch.Infrastructure.Repeat;
using Summer.Batch.Infrastructure.Repeat.Support;
using Summer.Batch.Common.Transaction;
using Summer.Batch.Common.Transaction.Support;
using System;
using System.Linq;
using System.Threading;
using System.Transactions;
using ExecutionContext = Summer.Batch.Infrastructure.Item.ExecutionContext;

namespace Summer.Batch.Core.Step.Tasklet
{
    /// <summary>
    /// Simple implementation of executing the step as a call to a <see cref="ITasklet"/>,
    /// possibly repeated, and each call surrounded by a transaction. The structure
    /// is therefore that of a loop with transaction boundary inside the loop. The
    /// loop is controlled by the step operations (StepOperations=RepeatOperations)).
    /// 
    /// Clients can use interceptors in the step operations to intercept or listen to
    /// the iteration on a step-wide basis, for instance to get a callback when the
    /// step is complete. Those that want callbacks at the level of an individual
    /// tasks, can specify interceptors for the chunk operations.
    /// </summary>
    public class TaskletStep : AbstractStep, IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Tasklet type constant.
        /// </summary>
        public const string TaskletTypeKey = "batch.taskletType";

        #region attributes
        private IRepeatOperations _stepOperations = new RepeatTemplate();

        /// <summary>
        /// Step operations property.
        /// </summary>
        public IRepeatOperations StepOperations { set { _stepOperations = value; } }
        private IStepInterruptionPolicy _interruptionPolicy = new ThreadStepInterruptionPolicy();

        /// <summary>
        /// Interruption policy property.
        /// </summary>
        public IStepInterruptionPolicy InterruptionPolicy { set { _interruptionPolicy = value; } }
        private CompositeItemStream _stream = new CompositeItemStream();
        private ITasklet _tasklet;

        /// <summary>
        /// Tasklet property.
        /// </summary>
        public ITasklet Tasklet
        {
            get { return _tasklet; }

            set
            {
                if (Logger.IsTraceEnabled)
                {
                    Logger.Trace("TaskletStep : Setting Tasklet [{0}] - is StepExecutionListener ? [{1}]", 
                        value.GetType().FullName,
                        (value is IStepExecutionListener));
                    Logger.Trace("TaskletStep : given tasklet implements the following interfaces : {0}",
                        String.Join("\n,",value.GetType().GetInterfaces().Select(i=>i.FullName))
                        );
                }
                _tasklet = value;
                if (value is IStepExecutionListener)
                {
                    RegisterStepExecutionListener((IStepExecutionListener)value);
                }
            }
        }
        #endregion

        #region Constructors

        /// <summary>
        /// Custom constructor with name.
        /// </summary>
        /// <param name="name"></param>
        public TaskletStep(string name) : base(name)
        {
            if(Logger.IsTraceEnabled)
            {
                Logger.Trace("TaskletStep constructor({0})",name);
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TaskletStep() : base(null)
        {
            if (Logger.IsTraceEnabled)
            {
                Logger.Trace("TaskletStep constructor(null)");
            }
        }
        #endregion

        #region public register


        /// <summary>
        /// Register a single IItemStream for callbacks to the stream interface.
        /// </summary>
        /// <param name="stream"></param>
        public void RegisterStream(IItemStream stream)
        {
            _stream.Register(stream);
        }
        #endregion

        #region public setters

        /// <summary>
        /// Register each of the streams for callbacks at the appropriate time in the
        /// step. The  IItemReader and  IItemWriter are automatically
        /// registered, but it doesn't hurt to also register them here. Injected
        /// dependencies of the reader and writer are not automatically registered,
        ///so if you implement ItemWriter using delegation to another object
        /// which itself is a IItemStream, you need to register the delegate
        /// here.
        /// </summary>
        /// <param name="streams"></param>
        public void SetStreams(IItemStream[] streams)
        {
            foreach (IItemStream stream in streams)
            {
                RegisterStream(stream);
            }
        }
        #endregion

        #region stream operations
        /// <summary>
        /// Delegates to composite stream closes.
        /// </summary>
        /// <param name="ctx"></param>
        protected override void Close(ExecutionContext ctx)
        {
            _stream.Close();
        }

        /// <summary>
        /// Delegates to composite stream open
        /// </summary>
        /// <param name="ctx"></param>
        protected override void Open(ExecutionContext ctx)
        {
            _stream.Open(ctx);
        }
        #endregion

        /// <summary>
        /// 	
        /// Extension point mainly for test purposes so that the behaviour of the
        /// lock can be manipulated to simulate various pathologies.
        /// </summary>
        /// <returns>a semaphore for locking access to the JobRepository</returns>
        protected Semaphore CreateSemaphore()
        {
            return new Semaphore(1, 1);
        }

        /// <summary>
        /// Actual taskletstep execution.
        /// </summary>
        /// <param name="stepExecution"></param>
        /// <exception cref="Exception">&nbsp;</exception>
        protected override void DoExecute(StepExecution stepExecution)
        {
            stepExecution.ExecutionContext.Put(TaskletTypeKey, _tasklet.GetType().Name);
            stepExecution.ExecutionContext.Put(StepConstants.StepTypeKey, GetType().Name);

            _stream.Update(stepExecution.ExecutionContext);

            JobRepository.UpdateExecutionContext(stepExecution);
            JobRepository.UpdateExecutionContext(stepExecution.JobExecution);
            // Shared semaphore per step execution, so other step executions can run
            // in parallel without needing the lock
            Semaphore semaphore = CreateSemaphore();

            _stepOperations.Iterate(StepContextRepeatCallback.GetRepeatCallback(stepExecution,
                (context, chunkContext) =>
                {
                    StepExecution lStepExecution = chunkContext.StepContext.StepExecution;
                    _interruptionPolicy.CheckInterrupted(lStepExecution);

                    ChunkTransactionCallback callback =
                        new ChunkTransactionCallback(chunkContext, semaphore, this);

                    RepeatStatus result;

                    using (var scope = TransactionScopeManager.CreateScope())
                    {
                        TransactionScopeManager.RegisterTransactionSynchronization(scope, callback);
                        try
                        {
                            result = callback.DoInTransaction();
                        }
                        catch (Exception e)
                        {
                            //Log and rethrow
                            Logger.Error(e, "Transaction will be rollbacked because of an unexpected exception.");
                            throw; // throw to ensure rollback will occur (no complete)
                        }
                        scope.Complete();
                    }
                    // Release connections since the transaction has ended
                    ConnectionUtil.ReleaseConnections();
                    Thread.Sleep(stepExecution.DelayConfig);
                    _interruptionPolicy.CheckInterrupted(stepExecution);
                    return result;
                }
                ));
        }


        #region ChunkTransactionCallback Private Class
        private class ChunkTransactionCallback : ITransactionSynchronization
        {
            private const string JobRepositoryForcedRollbackMsg = "JobRepository failure forcing rollback";

            #region attributes
            private readonly StepExecution _stepExecution;
            private readonly ChunkContext _chunkContext;
            private bool _rolledBack;
            private bool _stepExecutionUpdated;
            private StepExecution _oldVersion;
            private bool _locked;
            private readonly Semaphore _semaphore;
            private readonly TaskletStep _ownerStep;
            #endregion

            #region Constructors
            /// <summary>
            /// Custom constructor
            /// </summary>
            /// <param name="chunkContext"></param>
            /// <param name="semaphore"></param>
            /// <param name="owner"></param>
            public ChunkTransactionCallback(ChunkContext chunkContext, Semaphore semaphore, TaskletStep owner)
            {
                _chunkContext = chunkContext;
                _stepExecution = chunkContext.StepContext.StepExecution;
                _semaphore = semaphore;
                _ownerStep = owner;
            }
            #endregion

            #region AfterCompletion(Transaction transaction) method
            /// <summary>
            /// see ITransactionSynchronization#AfterCompletion()
            /// </summary>
            public void AfterCompletion(Transaction transaction)
            {
                var transactionInformation = transaction.TransactionInformation;
                try
                {
                    if (transactionInformation.Status != TransactionStatus.Committed && _stepExecutionUpdated)
                    {                        
                        // Wah! the commit failed. We need to rescue the step
                        // execution data.
                        Logger.Info("Commit failed while step execution data was already updated. Reverting to old version.");
                        Copy(_oldVersion, _stepExecution);
                        if (transactionInformation.Status == TransactionStatus.Aborted)
                        {
                            Rollback(_stepExecution);
                        }
                    }

                    if (transactionInformation.Status == TransactionStatus.InDoubt)
                    {
                        Logger.Error("Rolling back with transaction in unknown state");
                        Rollback(_stepExecution);
                        _stepExecution.UpgradeStatus(BatchStatus.Unknown);
                        _stepExecution.SetTerminateOnly();
                    }
                }
                finally
                {
                    // Only release the lock if we acquired it, and release as late
                    // as possible
                    if (_locked)
                    {
                        _semaphore.Release();
                    }
                    _locked = false;
                }
            }
            #endregion

            #region DoInTransaction(TransactionStatus status) method

            /// <summary>
            /// Wraps logic into a transactional context.
            /// </summary>
            /// <returns></returns>
            public RepeatStatus DoInTransaction()
            {
                RepeatStatus result;
                StepContribution contribution = _stepExecution.CreateStepContribution();

                // In case we need to push it back to its old value
                // after a commit fails...
                _oldVersion = new StepExecution(_stepExecution.StepName, _stepExecution.JobExecution);
                Copy(_stepExecution, _oldVersion);

                try
                {
                    try
                    {
                        try
                        {
                            result = _ownerStep._tasklet.Execute(contribution, _chunkContext) ?? RepeatStatus.Finished;
                        }
                        catch (Exception e)
                        {
                            _chunkContext.SetAttribute(StepListenerConstant.RollbackExceptionKey, e);
                            throw;
                        }
                    }
                    finally
                    {
                        // If the step operations are asynchronous then we need
                        // to synchronize changes to the step execution (at a
                        // minimum). Take the lock *before* changing the step
                        // execution.
                        try
                        {
                            _semaphore.WaitOne();
                            _locked = true;
                        }
                        catch (Exception)
                        {
                            Logger.Error("Thread interrupted while locking for repository update");
                            _stepExecution.BatchStatus = BatchStatus.Stopped;
                            _stepExecution.SetTerminateOnly();
                            Thread.CurrentThread.Interrupt();
                        }

                        // Apply the contribution to the step
                        // even if unsuccessful
                        if (Logger.IsDebugEnabled)
                        {
                            Logger.Debug("Applying contribution: {0}", contribution);
                        }
                        _stepExecution.Apply(contribution);

                    }
                    _stepExecutionUpdated = true;
                    _ownerStep._stream.Update(_stepExecution.ExecutionContext);

                    try
                    {
                        // Going to attempt a commit. If it fails this flag will
                        // stay false and we can use that later.
                        _ownerStep.JobRepository.UpdateExecutionContext(_stepExecution);
                        _ownerStep.JobRepository.UpdateExecutionContext(_stepExecution.JobExecution);
                        _stepExecution.IncrementCommitCount();
                        if (Logger.IsDebugEnabled)
                        {
                            Logger.Debug("Saving step execution before commit: {0}", _stepExecution);
                        }
                        _ownerStep.JobRepository.Update(_stepExecution);
                    }
                    catch (Exception e)
                    {
                        // If we get to here there was a problem saving the step
                        // execution and we have to fail.
                        Logger.Error(e, JobRepositoryForcedRollbackMsg);
                        throw new FatalStepExecutionException(JobRepositoryForcedRollbackMsg, e);
                    }
                }
                catch (Exception e)
                {
                    if (Logger.IsDebugEnabled)
                    {
                        Logger.Debug("Rollback for Exception: {0} : {1} ", e.GetType().Name, e.Message);
                    }
                    Rollback(_stepExecution);
                    throw;
                }
                return result;
            }
            #endregion

            #region private methods (Rollback / Copy)
            /// <summary>
            /// Rollback StepExecution
            /// </summary>
            /// <param name="stepExecution"></param>
            private void Rollback(StepExecution stepExecution)
            {
                if (!_rolledBack)
                {
                    stepExecution.IncrementRollbackCount();
                    _rolledBack = true;
                }
            }

            /// <summary>
            /// Copy from source StepExecution to target StepExecution
            /// </summary>
            /// <param name="source"></param>
            /// <param name="target"></param>
            private void Copy(StepExecution source, StepExecution target)
            {
                target.Version = source.Version;
                target.WriteCount = source.WriteCount;
                target.FilterCount = source.FilterCount;
                target.CommitCount = source.CommitCount;
                target.ExecutionContext = new ExecutionContext(source.ExecutionContext);
            }
            #endregion
        }
        #endregion

        #region IDisposable
        /// <summary>
        /// see https://msdn.microsoft.com/fr-fr/library/ms244737.aspx
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// see https://msdn.microsoft.com/fr-fr/library/ms244737.aspx
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (disposing && _stream != null)
            {
                // free managed resources
                _stream.Dispose();
                _stream = null;                
            }
        }
        #endregion
    }
}