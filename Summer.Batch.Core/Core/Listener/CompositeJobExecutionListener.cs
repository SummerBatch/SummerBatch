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

using System.Collections.Generic;

namespace Summer.Batch.Core.Listener
{
    /// <summary>
    /// Convenient job execution listener implemented through composite pattern.
    /// </summary>
    public class CompositeJobExecutionListener : IJobExecutionListener
    {

        private readonly OrderedComposite<IJobExecutionListener> _listeners
            = new OrderedComposite<IJobExecutionListener>();

        /// <summary>
        /// Sets the listeners.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listeners"></param>
        public void SetListeners<T>(List<T> listeners) where T : IJobExecutionListener
        {
            _listeners.SetItems(listeners);
        }

        /// <summary>
        /// Registers an additional listener.
        /// </summary>
        /// <param name="jobExecutionListener"></param>
        public void Register(IJobExecutionListener jobExecutionListener)
        {
            _listeners.Add(jobExecutionListener);
        }

        #region IJobExecutionListener methods implementation
        /// <summary>
        /// Call the registered listeners in order, respecting and prioritising those
        /// that hold an order metadata information.
        /// </summary>
        /// <param name="jobExecution"></param>
        public void BeforeJob(JobExecution jobExecution)
        {

            IEnumerator<IJobExecutionListener> enumerator = _listeners.Enumerator();
            while (enumerator.MoveNext())
            {
                IJobExecutionListener jobExecutionListener = enumerator.Current;
                jobExecutionListener.BeforeJob(jobExecution);
            }
        }

        /// <summary>
        /// Call the registered listeners in reverse order, respecting and
        /// prioritising those that hold an order metadata information.
        /// </summary>
        /// <param name="jobExecution"></param>
        public void AfterJob(JobExecution jobExecution)
        {
            IEnumerator<IJobExecutionListener> enumerator = _listeners.Reverse();
            while (enumerator.MoveNext())
            {
                IJobExecutionListener jobExecutionListener = enumerator.Current;
                jobExecutionListener.AfterJob(jobExecution);
            }
        } 
        #endregion
    }
}