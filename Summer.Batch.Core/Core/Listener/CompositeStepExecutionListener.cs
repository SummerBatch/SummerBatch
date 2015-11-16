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
using System.Linq;
using NLog;

namespace Summer.Batch.Core.Listener
{
    /// <summary>
    /// Convenient step execution listener implemented through composite pattern.
    /// </summary>
    public class CompositeStepExecutionListener : IStepExecutionListener
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly OrderedComposite<IStepExecutionListener> _list =
            new OrderedComposite<IStepExecutionListener>();

        /// <summary>
        /// Public setter for the listeners.
        /// </summary>
        /// <param name="listeners"></param>

        public void SetListeners(IStepExecutionListener[] listeners)
        {
            _list.SetItems(listeners.ToList());
            if (Logger.IsTraceEnabled)
            {
                Logger.Trace("CompositeStepExecutionListener : registering new list of stepExecutionListener of size [{0}]",
                    listeners.Length
                    );
            }
        }

        /// <summary>
        /// Registers an additional listener.
        /// </summary>
        /// <param name="stepExecutionListener"></param>

        public void Register(IStepExecutionListener stepExecutionListener)
        {            
            _list.Add(stepExecutionListener);
            if (Logger.IsTraceEnabled)
            {
                Logger.Trace("CompositeStepExecutionListener : registering new stepExecutionListener of type [{0}]",
                    stepExecutionListener.GetType().FullName
                    );
            }
        }

        /// <summary>
        /// Registers an additional step listener.
        /// </summary>
        /// <param name="stepListener"></param>
        public void Register(IStepListener stepListener)
        {
            if (stepListener is IStepExecutionListener)
            {
                Register(stepListener);
            }
            if (Logger.IsTraceEnabled)
            {
                Logger.Trace("CompositeStepExecutionListener : registering new stepListener of type [{0}]",
                    stepListener.GetType().FullName
                    );
            }
        }


        /// <summary>
        /// Calls the registered listeners in order, respecting and prioritizing those
        /// that hold an order metadata information.
        /// </summary>
        /// <param name="stepExecution"></param>
        public void BeforeStep(StepExecution stepExecution)
        {
            IEnumerator<IStepExecutionListener> enumerator = _list.Reverse();
            while (enumerator.MoveNext())
            {
                IStepExecutionListener listener = enumerator.Current;
                listener.BeforeStep(stepExecution);
            }
        }

        /// <summary>
        /// Calls the registered listeners in reverse order, respecting and
        /// prioritizing those that hold an order metadata information.
        /// </summary>
        /// <param name="stepExecution"></param>
        /// <returns></returns>
        public ExitStatus AfterStep(StepExecution stepExecution)
        {
            if (Logger.IsTraceEnabled)
            {
                Logger.Trace("CompositeStepExecutionListener :: Entering AfterStep");
            }
            IEnumerator<IStepExecutionListener> enumerator = _list.Reverse();
            while (enumerator.MoveNext())
            {
                if (Logger.IsTraceEnabled)
                {
                    Logger.Trace("CompositeStepExecutionListener :: Before Executing AfterStep - stepExecution.ExitStatus ={0}", stepExecution.ExitStatus);
                }
                IStepExecutionListener listener = enumerator.Current;
                ExitStatus close = listener.AfterStep(stepExecution);
                stepExecution.ExitStatus = stepExecution.ExitStatus.And(close);
                if (Logger.IsTraceEnabled)
                {
                    Logger.Trace("CompositeStepExecutionListener :: After Executing AfterStep - stepExecution.ExitStatus ={0}", stepExecution.ExitStatus);
                }
            }

            if (Logger.IsTraceEnabled)
            {
                Logger.Trace("CompositeStepExecutionListener :: Exiting AfterStep");
            }

            return stepExecution.ExitStatus;
        }
    }
}