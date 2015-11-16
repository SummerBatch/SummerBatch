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
using Summer.Batch.Common.Factory;
using Summer.Batch.Common.Util;

namespace Summer.Batch.Core.Job
{
    /// <summary>
    /// Default implementation of IJobParametersValidator.
    /// </summary>
    public class DefaultJobParametersValidator : IJobParametersValidator, IInitializationPostOperations
    {
        private ICollection<string> _requiredKeys;

        /// <summary>
        /// Collection of required keys property.
        /// </summary>
        public ICollection<string> RequiredKeys
        {
            private get { return _requiredKeys; }
            set { _requiredKeys = new HashSet<string>(value); }
        }

        private ICollection<string> _optionalKeys;

        /// <summary>
        /// Collection of optional keys property.
        /// </summary>
        public ICollection<string> OptionalKeys
        {
            private get { return _optionalKeys; }
            set { _optionalKeys = new HashSet<string>(value); }
        }

        /// <summary>
        ///  Convenient default constructor for unconstrained validation.
        /// </summary>
        public DefaultJobParametersValidator() : this(new string[0], new string[0]) { }

        /// <summary>
        /// Create a new validator with the required and optional job parameter keys provided.
        /// </summary>
        /// <param name="requiredKeys"></param>
        /// <param name="optionalKeys"></param>
        public DefaultJobParametersValidator(string[] requiredKeys, string[] optionalKeys)
        {
            RequiredKeys = requiredKeys;
            OptionalKeys = optionalKeys;
        }


        /// <summary>
        /// Check the parameters meet the specification provided. If optional keys
        /// are explicitly specified then all keys must be in that list, or in the
        /// required list. Otherwise all keys that are specified as required must be
        /// present.
        /// </summary>
        /// <param name="parameters"></param>
        /// <exception cref="JobParametersInvalidException"></exception>
        public void Validate(JobParameters parameters)
        {
            if (parameters == null)
            {
                throw new JobParametersInvalidException("The JobParameters can not be null");
            }

            HashSet<string> keys =
                new HashSet<string>(parameters.GetParameters().Keys);

            // If there are explicit optional keys then all keys must be in that
            // group, or in the required group.
            if (OptionalKeys.Any())
            {

                ICollection<string> missingKeys = new HashSet<string>();
                foreach (string key in keys)
                {
                    if (!OptionalKeys.Contains(key) && !RequiredKeys.Contains(key))
                    {
                        missingKeys.Add(key);
                    }
                }
                if (missingKeys.Any())
                {
                    throw new JobParametersInvalidException(
                            string.Format("The JobParameters contains keys that are not explicitly optional or required: {0} "
                            , missingKeys));
                }

            }

            ICollection<string> missingKeys2 = new HashSet<string>();
            foreach (string key in RequiredKeys)
            {
                if (!keys.Contains(key))
                {
                    missingKeys2.Add(key);
                }
            }
            if (missingKeys2.Any())
            {
                throw new JobParametersInvalidException(
                    string.Format("The JobParameters do not contain required keys: {0}", missingKeys2));
            }
        }

        /// <summary>
        /// Check that there are no overlaps between required and optional keys.
        /// #see IInitializationPostOperations#AfterPropertiesSet
        /// </summary>
        public void AfterPropertiesSet()
        {
            foreach (string key in _requiredKeys)
            {
                Assert.State(!_optionalKeys.Contains(key), string.Format("Optional keys canot be required: {0}", key));
            }
        }

    }
}