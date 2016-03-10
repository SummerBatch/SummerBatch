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
using Summer.Batch.Common.Factory;
using Summer.Batch.Common.Util;

namespace Summer.Batch.Core.Job
{
    /// <summary>
    /// Composite <see cref="IJobParametersValidator"/> that passes the job parameters through a sequence of
    /// IJobParametersValidator's
    /// </summary>
    public class CompositeJobParametersValidator : IJobParametersValidator, IInitializationPostOperations
    {
        /// <summary>
        /// List of validators property.
        /// </summary>
        public List<IJobParametersValidator> Validators { private get; set; }

        /// <summary>
        /// Validate job parameters (delegates to each validator).
        /// </summary>
        /// <param name="parameters"></param>
        /// <exception cref="JobParametersInvalidException">&nbsp;</exception>
        public void Validate(JobParameters parameters)
        {
            foreach (var validator in Validators)
            {
                validator.Validate(parameters);
            }
        }

        /// <summary>
        /// Post-init. checks.
        /// @see IInitializationPostOperations#AfterPropertiesSet
        /// </summary>
        public void AfterPropertiesSet() {
            Assert.NotNull(Validators, "The 'validators' may not be null");
            Assert.NotEmpty(Validators, "The 'validators' may not be empty");
        }


    }
}