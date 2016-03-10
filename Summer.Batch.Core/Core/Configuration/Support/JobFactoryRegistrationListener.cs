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
using NLog;
using System.Collections.Generic;

namespace Summer.Batch.Core.Configuration.Support
{
    /// <summary>
    /// Generic service that can bind and unbind an IJobFactory in an IJobRegistry.
    /// </summary>
    public class JobFactoryRegistrationListener
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Job registry property.
        /// </summary>
        public IJobRegistry JobRegistry { private get; set; }

        /// <summary>
        /// Takes the IJobFactory provided and register it with the IJobRegistry.
        /// </summary>
        /// <param name="jobFactory">an IJobFactory</param>
        /// <param name="parms">not needed by this listener</param>
        /// <exception cref="Exception">&nbsp;if there is a problem</exception>
        public void Bind(IJobFactory jobFactory, IDictionary<string, object> parms)
        {
            _logger.Info("Binding JobFactory: {0}",jobFactory.JobName);
            JobRegistry.Register(jobFactory);
        }

        /// <summary>
        /// Take the IJobFactory provided and unregister it with the IJobRegistry.
        /// </summary>
        /// <param name="jobFactory">an IJobFactory</param>
        /// <param name="parms">not needed by this listener</param>
        /// <exception cref="Exception">&nbsp;if there is a problem</exception>        
        public void Unbind(IJobFactory jobFactory, IDictionary<string, object> parms)
        {
            _logger.Info("Unbinding JobFactory: {0}", jobFactory.JobName);
            JobRegistry.Unregister(jobFactory.JobName);
        }
    }
}
