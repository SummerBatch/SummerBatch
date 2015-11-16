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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Summer.Batch.Common.Util.AtomicTypes;

namespace Summer.Batch.Core.Scope.Context
{
    /// <summary>
    ///  Central convenience class for framework use in managing the scope context.
    /// </summary>
    /// <typeparam name="TExecution">the type of the execution</typeparam>
    /// <typeparam name="TContext">the type of the stored context</typeparam>
    public abstract class SynchronizationManagerSupport<TExecution, TContext> : IDisposable where TContext : class where TExecution:class
    {
        // We have to deal with single and multi-threaded execution, with a single
        // and with multiple step execution instances. That's 2x2 = 4 scenarios.

        ///<summary>
        /// Storage for the current execution; has to be ThreadLocal because it
        /// is needed to locate a context in components that are not part of a
        /// step/job (like when re-hydrating a scoped proxy). The Stack is used
        /// to cover the single threaded case, so that the API is the same as
        /// multi-threaded.
        ///</summary>
        private ThreadLocal<Stack<TExecution>> _executionHolder = new ThreadLocal<Stack<TExecution>>(() => new Stack<TExecution>());

        private readonly Dictionary<TExecution, AtomicInteger> _counts = new Dictionary<TExecution, AtomicInteger>();

        /// <summary>
        /// Simple map from a running execution to the associated context.
        /// </summary>
        private readonly Dictionary<TExecution, TContext> _contexts = new Dictionary<TExecution, TContext>();

        /// <summary>
        /// Convenience property to get the current execution stack.
        /// </summary>
        private Stack<TExecution> Current { get { return _executionHolder.Value; } }

        /// <summary>
        /// Getter for the current context if there is one, otherwise returns null.
        /// </summary>
        /// <returns>the current context or null if there is none (if one has not been registered for this thread)</returns>
        public TContext GetContext()
        {
            if (!Current.Any())
            {
                return null;
            }
            lock (_contexts)
            {
                TContext context;
                _contexts.TryGetValue(Current.Peek(), out context);
                return context;
            }
        }

        /// <summary>
        /// Register a context with the current thread - always put a matching <see cref="Close()"/> call
        /// in a finally block to ensure that the correct context is available in the enclosing block.
        /// </summary>
        /// <param name="execution"></param>
        /// <returns></returns>
        public TContext Register(TExecution execution)
        {
            if (execution == null)
            {
                return null;
            }
            Current.Push(execution);
            TContext context;
            lock (_contexts)
            {
                if (!_contexts.TryGetValue(execution, out context))
                {
                    context = CreateNewContext(execution);
                    _contexts[execution] = context;
                }
            }
            Increment();
            return context;
        }

        /// <summary>
        /// Method for unregistering the current context - should always and only be used by in conjunction
        /// with a matching <see cref="Register(TExecution)"/> to ensure that <see cref="GetContext"/> always
        /// returns the correct value.
        /// Does not call close on the context - that is left up to the caller because he has a reference to
        /// the context (having registered it) and only he has knowledge of when the execution actually ended. 
        /// </summary>
        public void Close()
        {
            var oldSession = GetContext();
            if (oldSession == null)
            {
                return;
            }
            Decrement();
        }

        private void Decrement()
        {
            var current = Current.Pop();
            if (current != null)
            {
                lock (_counts)
                {
                    AtomicInteger atRemaining;
                    if (_counts.TryGetValue(current, out atRemaining))
                    {
                        var remaining = atRemaining.DecrementValueAndReturn();
                        if (remaining <= 0)
                        {
                            lock (_contexts)
                            {
                                _contexts.Remove(current);
                                _counts.Remove(current);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Increment execution counter.
        /// </summary>
        public void Increment()
        {
            var current = Current.Peek();
            if (current != null)
            {
                AtomicInteger count;
                lock (_counts)
                {
                    if (!_counts.TryGetValue(current, out count))
                    {
                        count = new AtomicInteger();
                        _counts[current] = count;
                    }
                }
                count.IncrementValueAndReturn();
            }
        }

        /// <summary>
        /// A convenient "deep" close operation. Call this instead of <see cref="Close()"/> if the execution for the current
        /// context is ending.
        /// Delegates to <see cref="Close(TContext)"/> and then ensures that <see cref="Close()"/> is also called in a finally block.
        /// </summary>
        public void Release()
        {
            var context = GetContext();
            try
            {
                if (context != null)
                {
                    Close(context);
                }
            }
            finally
            {
                Close();
            }
        }

        /// <summary>
        /// To be overriden by subclasses.
        /// </summary>
        /// <param name="context"></param>
        protected abstract void Close(TContext context);

        /// <summary>
        /// To be overriden by subclasses.
        /// </summary>
        /// <param name="execution"></param>
        /// <returns></returns>
        protected abstract TContext CreateNewContext(TExecution execution);

        /// <summary>
        /// IDisposable#Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Effective dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_executionHolder != null)
                {
                    _executionHolder.Dispose();
                }
                _executionHolder = null;
            }
        }
    }
}
