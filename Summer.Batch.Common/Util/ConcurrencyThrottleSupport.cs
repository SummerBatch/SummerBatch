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

// This file has been modified.
// Original copyright notice :
/*
 * Copyright 2002-2012 the original author or authors.
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
using System.Threading;
using NLog;

namespace Summer.Batch.Common.Util
{
    /// <summary>
    /// Support class for throttling concurrent access to a specific resource.
    /// Designed for use as a base class, with the subclass invoking
    /// <see cref="BeforeAccess"/> and <see cref="AfterAccess"/> methods at
    /// appropriate points of its workflow. Note that AfterAccess
    /// should usually be called in a finally block!
    /// The default concurrency limit of this support class is -1
    /// ("unbounded concurrency"). Subclasses may override this default;
    /// check the javadoc of the concrete class that you're using.
    /// </summary>
    [Serializable]
    public abstract class ConcurrencyThrottleSupport
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Allows any number of concurrent invocations: that is, does not throttle concurrency.
        /// </summary>
        public const int UnboundedConcurrency = -1;

        /// <summary>
        /// Switch concurrency 'off': that is, does not allow any concurrent invocations.
        /// </summary>
        public const int NoConcurrency = 0;

        [NonSerialized]
        private readonly object _monitor = new object();

        private int _concurrencyLimit = UnboundedConcurrency;

        private int _concurrencyCount; //default to 0

        /// <summary>
        /// The maximum number of concurrent access attempts allowed.
        /// -1 indicates unbounded concurrency.
        /// In principle, this limit can be changed at runtime,
        /// although it is generally designed as a config time setting.
        /// NOTE: Do not switch between -1 and any concrete limit at runtime,
        /// as this will lead to inconsistent concurrency counts: A limit
        /// of -1 effectively turns off concurrency counting completely.
        /// </summary>
        public int ConcurrencyLimit { get { return _concurrencyLimit; } set { _concurrencyLimit = value; } }

        /// <summary>
        /// Returns whether this throttle is currently active.
        /// </summary>
        /// <returns>true if the concurrency limit for this instance is active</returns>
        public bool IsThrottleActive()
        {
            return ConcurrencyLimit > 0;
        }

        /// <summary>
        /// To be invoked before the main execution logic of concrete subclasses.
        /// This implementation applies the concurrency throttle.
        /// </summary>
        protected void BeforeAccess()
        {
            if (ConcurrencyLimit == NoConcurrency)
            {
                throw new InvalidOperationException(
                        "Currently no invocations allowed - concurrency limit set to NoConcurrency");
            }
            if (ConcurrencyLimit > 0)
            {
                lock (_monitor)
                {
                    bool interrupted = false;
                    while (_concurrencyCount >= _concurrencyLimit)
                    {
                        if (interrupted)
                        {
                            throw new InvalidOperationException(
                                "Thread was interrupted while waiting for invocation access, " +
                                "but concurrency limit still does not allow for entering");
                        }
                        Logger.Debug("Concurrency count {0} has reached limit {1} - blocking", _concurrencyCount,
                            _concurrencyLimit);
                        try
                        {
                            Monitor.Wait(_monitor);
                        }
                        catch (ThreadInterruptedException)
                        {
                            // Re-interrupts current thread, to allow other threads to react.
                            Thread.CurrentThread.Interrupt();
                            interrupted = true;
                        }
                    }
                    Logger.Debug("Entering throttle at concurrency count {0}", _concurrencyCount);
                    _concurrencyCount++;
                }
            }
        }


        /// <summary>
        /// To be invoked after the main execution logic of concrete subclasses.
        /// </summary>
        protected void AfterAccess()
        {
            if (_concurrencyLimit >= 0)
            {
                lock (_monitor)
                {
                    _concurrencyCount--;
                    Logger.Debug("Returning from throttle at concurrency count {0}", _concurrencyCount);
                    Monitor.Pulse(_monitor);
                }
            }
        }

    }
}
