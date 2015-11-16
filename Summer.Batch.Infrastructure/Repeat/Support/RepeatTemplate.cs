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

// This file has been modified.
// Original copyright notice :

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
using Summer.Batch.Infrastructure.Repeat.Exception;
using Summer.Batch.Infrastructure.Repeat.Policy;
using Summer.Batch.Common.Util;
using System.Collections.Generic;
using System.Linq;

namespace Summer.Batch.Infrastructure.Repeat.Support
{
    /// <summary>
    /// Simple implementation and base class for batch templates implementing
    /// RepeatOperations. Provides a framework including interceptors and
    /// policies. Subclasses just need to provide a method that gets the next result
    /// and one that waits for all the results to be returned from concurrent
    /// processes or threads.
    /// 
    /// N.B. the template accumulates thrown exceptions during the iteration, and
    /// they are all processed together when the main loop ends (i.e. finished
    /// processing the items). Clients that do not want to stop execution when an
    /// exception is thrown can use a specific ICompletionPolicy that does not
    /// finish when exceptions are received. This is not the default behaviour.
    /// Clients that want to take some business action when an exception is thrown by
    /// the IRepeatCallback can consider using a custom IRepeatListener
    /// instead of trying to customise the ICompletionPolicy. This is
    /// generally a friendlier interface to implement, and the
    /// IRepeatListener#After(RepeatContext, RepeatStatus) method is passed in
    /// the result of the callback, which would be an instance of Exception
    /// if the business processing had thrown an exception. If the exception is not
    /// to be propagated to the caller, then a non-default ICompletionPolicy
    /// needs to be provided as well, but that could be off the shelf, with the
    /// business action implemented only in the interceptor.
    /// </summary>
    public class RepeatTemplate : IRepeatOperations
    {
        /// <summary>
        /// Logger
        /// </summary>
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private IRepeatListener[] _listeners = { };

        private ICompletionPolicy _completionPolicy = new DefaultResultCompletionPolicy();

        /// <summary>
        /// Completion policy property.
        /// </summary>
        public ICompletionPolicy CompletionPolicy
        {
            set
            {
                Assert.NotNull(value);
                _completionPolicy = value;
            }
        }

        private IExceptionHandler _exceptionHandler = new DefaultExceptionHandler();

        /// <summary>
        /// Exception handler property.
        /// </summary>
        public IExceptionHandler ExceptionHandler { set { _exceptionHandler = value; } }

        /// <summary>
        /// Registers array of listeners.
        /// </summary>
        /// <param name="listeners"></param>
        public void SetListeners(IRepeatListener[] listeners)
        {
            _listeners = listeners.ToArray();
        }

        /// <summary>
        /// Registers given listener.
        /// </summary>
        /// <param name="listener"></param>
        public void RegisterListener(IRepeatListener listener)
        {
            List<IRepeatListener> list = new List<IRepeatListener>(_listeners) { listener };
            _listeners = list.ToArray();
        }


        /// <summary>
        /// Execute the batch callback until the completion policy decides that we
        /// are finished. Wait for the whole batch to finish before returning even if
        /// the task executor is asynchronous.
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public RepeatStatus Iterate(RepeatCallback callback)
        {
            IRepeatContext outer = RepeatSynchronizationManager.GetContext();
            RepeatStatus result;
            try
            {
                // This works with an asynchronous TaskExecutor: the
                // interceptors have to wait for the child processes.
                result = ExecuteInternal(callback);
            }
            finally
            {
                RepeatSynchronizationManager.Clear();
                if (outer != null)
                {
                    RepeatSynchronizationManager.Register(outer);
                }
            }
            return result;
        }


        /// <summary>
        /// Delegates the start to the Completion policy.
        /// </summary>
        /// <returns></returns>
        protected IRepeatContext Start()
        {
            IRepeatContext parent = RepeatSynchronizationManager.GetContext();
            IRepeatContext context = _completionPolicy.Start(parent);
            RepeatSynchronizationManager.Register(context);
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Starting repeat context.");
            }
            return context;
        }

        /// <summary>
        /// Check return value from batch operation.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected bool CanContinue(RepeatStatus value)
        {
            return value.IsContinuable();
        }

        /// <summary>
        /// Check that given context is marked as completed.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool IsMarkedComplete(IRepeatContext context)
        {
            bool complete = context.IsCompleteOnly();
            if (context.Parent != null)
            {
                complete = complete || IsMarkedComplete(context.Parent);
            }
            if (complete && Logger.IsDebugEnabled)
            {
                Logger.Debug("Repeat is complete according to context alone.");
            }
            return complete;
        }

        /// <summary>
        /// Create an internal state object that is used to store data needed
        /// internally in the scope of an iteration. Used by subclasses to manage the
        /// queueing and retrieval of asynchronous results. The default just provides
        /// an accumulation of exceptions instances for processing at the end of the
        /// batch.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual IRepeatInternalState CreateInternalState(IRepeatContext context)
        {
            return new RepeatInternalStateSupport();
        }

        /// <summary>
        /// Delegates to the completion policy.
        /// </summary>
        /// <param name="context"></param>
        protected void Update(IRepeatContext context)
        {
            _completionPolicy.Update(context);
        }

        /// <summary>
        ///  Get the next completed result, possibly executing several callbacks until
        /// one finally finishes. Normally a subclass would have to override both
        /// this method and {@link #CreateInternalState(RepeatContext)} because the
        /// implementation of this method would rely on the details of the internal
        /// state.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="callback"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        protected virtual RepeatStatus GetNextResult(IRepeatContext context, RepeatCallback callback, IRepeatInternalState state)
        {
            Update(context);
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Repeat operation about to start at count={0}", context.GetStartedCount());
            }
            return callback(context); //"DoInInteration"
        }

        /// <summary>
        ///  If necessary, wait for results to come back from remote or concurrent
        /// processes. By default does nothing and returns true.        
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected virtual bool WaitForResults(IRepeatInternalState state)
        {
            // no-op by default
            return true;
        }


        /// <summary>
        ///  Convenience method to execute after interceptors on a callback result.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="value"></param>
        protected void ExecuteAfterInterceptors(IRepeatContext context, RepeatStatus value)
        {
            // Don't re-throw exceptions here: let the exception handler deal with
            // that...
            if (value != null && value.IsContinuable())
            {
                for (int i = _listeners.Length; i-- > 0; )
                {
                    IRepeatListener interceptor = _listeners[i];
                    interceptor.After(context, value);
                }
            }
        }

        /// <summary>
        /// Unwrap the exception id it has been wrapped into a RepeatException.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        private static System.Exception UnwrapIfRethrown(System.Exception exception)
        {
            if (exception is RepeatException)
            {
                return exception.InnerException;
            }
            else
            {
                return exception;
            }
        }

        /// <summary>
        /// Delegates to the Completion policy.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected bool IsComplete(IRepeatContext context, RepeatStatus result)
        {
            bool complete = _completionPolicy.IsComplete(context, result);
            if (complete && Logger.IsDebugEnabled)
            {
                Logger.Debug("Repeat is complete according to policy and result value.");
            }
            return complete;
        }

        /// <summary>
        /// Delegates to the Completion policy.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected bool IsComplete(IRepeatContext context)
        {
            bool complete = _completionPolicy.IsComplete(context);
            if (complete && Logger.IsDebugEnabled)
            {
                Logger.Debug("Repeat is complete according to policy alone not including result.");
            }
            return complete;
        }

        /// <summary>
        /// Handling exceptions.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="context"></param>
        /// <param name="deferred"></param>
        private void DoHandle(System.Exception exception, IRepeatContext context, ICollection<System.Exception> deferred)
        {
            // An exception alone is not sufficient grounds for not continuing
            System.Exception unwrappedException = UnwrapIfRethrown(exception);
            try
            {
                for (int i = _listeners.Length; i-- > 0; )
                {
                    IRepeatListener interceptor = _listeners[i];
                    // This is not an error - only log at debug level.
                    Logger.Debug(unwrappedException, "Exception intercepted ({0} of {1})", (i + 1), _listeners.Length);
                    interceptor.OnError(context, unwrappedException);
                }

                Logger.Debug("Handling exception: {0}, caused by: {1} : {2}",
                    exception.GetType().Name,
                    unwrappedException.GetType().Name,
                     unwrappedException.Message
                    );

                _exceptionHandler.HandleException(context, unwrappedException);

            }
            catch (System.Exception handled)
            {
                deferred.Add(handled);
            }
        }

        /// <summary>
        /// rethrow the exception wrapped into a RepeatException.
        /// </summary>
        /// <param name="exception"></param>
        private static void Rethrow(System.Exception exception)
        {
            throw new RepeatException("Exception in batch process", exception);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        private RepeatStatus ExecuteInternal(RepeatCallback callback)
        {

            // Reset the termination policy if there is one...
            IRepeatContext context = Start();

            // Make sure if we are already marked complete before we start then no
            // processing takes place.
            bool running = !IsMarkedComplete(context);

            foreach (IRepeatListener interceptor in _listeners)
            {
                interceptor.Open(context);
                running = running && !IsMarkedComplete(context);
                if (!running)
                {
                    break;
                }
            }

            // Return value, default is to allow continued processing.
            RepeatStatus result = RepeatStatus.Continuable;

            IRepeatInternalState state = CreateInternalState(context);
            // This is the list of exceptions thrown by all active callbacks
            ICollection<System.Exception> exceptions = state.GetExceptions();
            // Keep a separate list of exceptions we handled that need to be
            // rethrown
            ICollection<System.Exception> deferred = new List<System.Exception>();

            try
            {
                while (running)
                {
                    #region WhileRunning
                    /*
                     * Run the before interceptors here, not in the task executor so
                     * that they all happen in the same thread - it's easier for
                     * tracking batch status, amongst other things.
                     */
                    foreach (IRepeatListener interceptor in _listeners)
                    {
                        interceptor.Before(context);
                        // Allow before interceptors to veto the batch by setting
                        // flag.
                        running = running && !IsMarkedComplete(context);
                    }

                    // Check that we are still running (should always be true) ...
                    if (running)
                    {
                        #region Running
                        try
                        {
                            result = GetNextResult(context, callback, state);
                            ExecuteAfterInterceptors(context, result);
                        }
                        catch (System.Exception exception)
                        {
                            DoHandle(exception, context, deferred);
                        }

                        // N.B. the order may be important here:
                        if (IsComplete(context, result) || IsMarkedComplete(context) || deferred.Any())
                        {
                            running = false;
                        }
                        #endregion
                    }
                    #endregion
                }

                result = result.And(WaitForResults(state));
                foreach (System.Exception exception in exceptions)
                {
                    DoHandle(exception, context, deferred);
                }

                // Explicitly drop any references to internal state...
                // useless ?
                state = null;

            }
            /*
             * No need for explicit catch here - if the business processing threw an
             * exception it was already handled by the helper methods. An exception
             * here is necessarily fatal.
             */
            finally
            {
                #region HandleFinally
                HandleFinally(deferred, _listeners, context);
                #endregion
            }
            return result;
        }

        /// <summary>
        /// Handling the finally from ExecuteInternal.
        /// </summary>
        /// <param name="deferred"></param>
        /// <param name="listeners"></param>
        /// <param name="context"></param>
        private void HandleFinally(ICollection<System.Exception> deferred,IRepeatListener[] listeners,IRepeatContext context)
        {
            try
            {
                if (deferred.Any())
                {
                    System.Exception exception = deferred.First();
                    Logger.Debug("Handling fatal exception explicitly (rethrowing first of {0}): {1} : {2}",
                        deferred.Count,
                        exception.GetType().Name,
                        exception.Message
                        );
                    Rethrow(exception);
                }
            }
            finally
            {
                try
                {
                    foreach (IRepeatListener interceptor in _listeners)
                    {
                        interceptor.Close(context);
                    }
                }
                finally
                {
                    context.Close();
                }

            }
        }

    }



}