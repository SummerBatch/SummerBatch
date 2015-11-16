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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Summer.Batch.Infrastructure.Repeat.Context
{
    /// <summary>
    /// Support for repeat context.
    /// </summary>
    public class RepeatContextSupport : SynchronizedAttributeAccessor, IRepeatContext
    {
        private readonly IRepeatContext _parent;
        
        /// <summary>
        /// Parent repeat context.
        /// </summary>
        public IRepeatContext Parent { get { return _parent; } }
        private int _count;
        private volatile bool _completeOnly;
        private volatile bool _terminateOnly;
        private readonly Dictionary<string, HashSet<Task>> _callbacks = new Dictionary<string, HashSet<Task>>();

        /// <summary>
        ///  Constructor for RepeatContextSupport. The parent can be null, but
        /// should be set to the enclosing repeat context if there is one, e.g. if
        /// this context is an inner loop.
        /// </summary>
        /// <param name="parent"></param>
        public RepeatContextSupport(IRepeatContext parent)
        {
            _parent = parent;
        }

        /// <summary>
        /// Used by clients to increment the started count.
        /// Method is synchronized (see the [MethodImpl(MethodImplOptions.Synchronized)] attribute).
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Increment()
        {
            _count++;
        }

        /// <summary>
        /// Returns counter value.
        /// </summary>
        /// <returns></returns>
        public int GetStartedCount()
        {
            return _count;
        }

        /// <summary>
        /// @see IRepeatContext#SetCompleteOnly .
        /// </summary>
        public void SetCompleteOnly()
        {
            _completeOnly = true;
        }

        /// <summary>
        /// @see IRepeatContext#IsCompleteOnly .
        /// </summary>
        /// <returns></returns>
        public bool IsCompleteOnly()
        {
            return _completeOnly;
        }

        /// <summary>
        /// @see IRepeatContext#SetTerminateOnly .
        /// </summary>
        public void SetTerminateOnly()
        {
            _terminateOnly = true;
            SetCompleteOnly();
        }

        /// <summary>
        /// @see IRepeatContext#IsTerminateOnly .
        /// </summary>
        /// <returns></returns>
        public bool IsTerminateOnly()
        {
            return _terminateOnly;
        }

        /// <summary>
        /// @see IRepeatContext#RegisterDestructionCallback .
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        public void RegisterDestructionCallback(string name, Task callback)
        {
            lock (_callbacks)
            {
                HashSet<Task> set;
                _callbacks.TryGetValue(name, out set);
                if (set == null)
                {
                    set = new HashSet<Task>();
                    _callbacks.Add(name, set);
                }
                set.Add(callback);
            }
        }

        /// <summary>
        /// @see IRepeatContext#Close .
        /// </summary>
        public void Close()
        {
            List<System.Exception> errors = new List<System.Exception>();

            HashSet<KeyValuePair<string, HashSet<Task>>> copy;

            lock (_callbacks)
            {
                copy = new HashSet<KeyValuePair<string, HashSet<Task>>>(_callbacks);
            }

            foreach (KeyValuePair<string, HashSet<Task>> entry in copy)
            {

                foreach (Task callback in entry.Value)
                {                    
                    if (callback != null)
                    {
                        // Collect all thrown exceptions into the Errors collection.
                        // The first one will be rethrown (if any ...)
                        try
                        {
                            callback.Start();
                        }
                        catch (System.Exception t)
                        {
                            errors.Add(t);
                        }
                    }
                }
            }

            if (!errors.Any())
            {
                return;
            }

            throw errors[0];
        }
    }
}