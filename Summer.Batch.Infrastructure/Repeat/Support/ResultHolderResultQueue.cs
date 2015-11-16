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

using System;
using System.Collections.Generic;
using System.Threading;
using Summer.Batch.Common.Collections;

namespace Summer.Batch.Infrastructure.Repeat.Support
{
    /// <summary>
    /// An implementation of the <see cref="Summer.Batch.Infrastructure.Repeat.Support.IResultQueue&lt;TB&gt;"/> that throttles the number of
    /// expected results, limiting it to a maximum at any given time.
    /// </summary>
    public class ResultHolderResultQueue : IResultQueue<IResultHolder>, IDisposable
    {
        private Semaphore _waits;
        //Accumulation of result objects as they finish.
        private readonly PriorityBlockingQueue<IResultHolder> _results;
        private volatile int _count; // def to 0
        private readonly object _lock = new object();

        /// <summary>
        /// Custom constructor.
        /// </summary>
        /// <param name="throttleLimit">throttleLimit the maximum number of results that can be expected at any given time.</param>
        public ResultHolderResultQueue(int throttleLimit)
        {
            _results = new PriorityBlockingQueue<IResultHolder>(throttleLimit, new ResultHolderComparer());
            _waits = new Semaphore(throttleLimit, throttleLimit);
        }

        /// <summary>
        /// see IResultQueue#Expect() .
        /// </summary>
        public void Expect()
        {
            _waits.WaitOne();
            // Don't acquire the lock in a synchronized block - might deadlock
            lock (_lock)
            {
                _count++;
            }

        }

        /// <summary>
        /// see IResultQueue#Put() .
        /// </summary>
        /// <param name="result"></param>
        public void Put(IResultHolder result)
        {
            if (!IsExpecting())
            {
                throw new ArgumentException("Not expecting a result. Call Expect() before Put().");
            }
            _results.Add(result);
            _waits.Release();
            lock (_lock)
            {
                Monitor.PulseAll(_lock);
            }
        }

        /// <summary>
        /// Wrapper around IsContinuable .
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool IsContinuable(IResultHolder value)
        {
            return value.Result != null && value.Result.IsContinuable();
        }

        /// <summary>
        /// see IResultQueue#Take() .
        /// </summary>
        /// <returns></returns>
        public IResultHolder Take()
        {
            if (!IsExpecting())
            {
                throw new InvalidOperationException("Not expecting a result.  Call expect() before take().");
            }
            IResultHolder value;
            lock (_lock)
            {
                value = _results.Take();
                if (IsContinuable(value))
                {
                    // Decrement the counter only when the result is collected.
                    _count--;
                    return value;
                }
            }
            _results.Add(value);
            lock (_lock)
            {
                while (_count > _results.Count)
                {
                    Monitor.Wait(_lock);
                }
                value = _results.Take();
                _count--;
            }
            return value;
        }

        /// <summary>
        /// see IResultQueue#IsEmpty() .
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return _results.Count == 0;
        }

        /// <summary>
        /// see IResultQueue#IsExpecting() .
        /// </summary>
        /// <returns></returns>
        public bool IsExpecting()
        {
            // Base the decision about whether we expect more results on a
            // counter of the number of expected results actually collected.
            // Do not synchronize! Otherwise put and expect can deadlock.
            return _count > 0;
        }


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
            if (disposing && _waits != null)
            {
                // free managed resources
                _waits.Dispose();
                _waits = null;
            }
        }

        #endregion

        private class ResultHolderComparer : IComparer<IResultHolder>
        {
            public int Compare(IResultHolder x, IResultHolder y)
            {
                var xResult = x.Result;
                var yResult = y.Result;
                if (xResult == null && yResult == null)
                {
                    return 0;
                }
                if (xResult == null)
                {
                    return -1;
                }
                if (yResult == null)
                {
                    return 1;
                }
                if ((xResult.IsContinuable() && yResult.IsContinuable())
                        || (!xResult.IsContinuable() && !yResult.IsContinuable()))
                {
                    return 0;
                }
                if (xResult.IsContinuable())
                {
                    return -1;
                }
                return 1;
            }
        }
    }
}