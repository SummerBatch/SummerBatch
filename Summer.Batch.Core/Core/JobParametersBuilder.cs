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

using Summer.Batch.Common.Util;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Summer.Batch.Common.Collections;

namespace Summer.Batch.Core
{
    /// <summary>
    ///   Helper class for creating <see cref="JobParameters"/>. Useful because all
    /// JobParameter objects are immutable, and must be instantiated separately
    /// to ensure typesafety. Once created, it can be used in the
    /// same was a StringBuilder (except, order is irrelevant), by adding
    /// various parameter types and creating a valid JobParameters once
    /// finished.
    /// Using the identifying flag indicates if the parameter will be used
    /// in the identification of a JobInstance.  That flag defaults to true.
    /// </summary>
    public class JobParametersBuilder : IDisposable
    {
        private IDictionary<string, JobParameter> _parameterMap;

        #region Constructors
        /// <summary>
        /// Default constructor. Initializes the builder with empty parameters. 
        /// </summary>
        public JobParametersBuilder()
        {
            _parameterMap = new OrderedDictionary<string, JobParameter>(16);
        }


        /// <summary>
        /// Copy constructor. Initializes the builder with the supplied parameters.
        /// THIS SHOULD PRESERVE PRIOR EXISTING ORDER.
        /// </summary>
        /// <param name="jobParameters"></param>
        public JobParametersBuilder(JobParameters jobParameters)
        {
            _parameterMap = new OrderedDictionary<string, JobParameter>(jobParameters.GetParameters().Count);
            IDictionary<string, JobParameter> givenJobParameters = jobParameters.GetParameters();
            KeyValuePair<string, JobParameter>[] jbArray =
                new KeyValuePair<string, JobParameter>[givenJobParameters.Count];
            jobParameters.GetParameters().CopyTo(jbArray, 0);
            foreach (KeyValuePair<string, JobParameter> mapEntry in jbArray)
            {
                _parameterMap.Add(mapEntry);
            }
        }

        /// <summary>
        /// Constructor to add conversion capabilities to support JSR-352.  Per the spec, it is expected that all
        /// keys and values in the provided Properties instance are Strings
        /// </summary>
        /// <param name="properties">the job parameters to be used</param>
        public JobParametersBuilder(NameValueCollection properties)
        {
            _parameterMap = new OrderedDictionary<string, JobParameter>(properties.Count);

            foreach (string key in properties.Keys)
            {
                _parameterMap[key] = new JobParameter(properties[key], false);
            }
            
        } 
        #endregion

        #region public methods
        /// <summary>
        /// Adds a new identifying String parameter for the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>

        public JobParametersBuilder AddString(string  key, string parameter)
        {
            _parameterMap[key] = new JobParameter(parameter, true);
            return this;
        }

        /// <summary>
        /// Adds a new String parameter for the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parameter"></param>
        /// <param name="identifying"></param>
        /// <returns></returns>

        public JobParametersBuilder AddString(string key, string parameter, bool identifying)
        {
            _parameterMap[key] = new JobParameter(parameter, identifying);
            return this;
        }

        /// <summary>
        ///Adds a new identifying DateTime parameter for the given key. 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public JobParametersBuilder AddDate(string key, DateTime? parameter)
        {
            _parameterMap[key] = new JobParameter(parameter, true);
            return this;
        }

        /// <summary>
        /// Adds a new DateTime parameter for the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parameter"></param>
        /// <param name="identifying"></param>
        /// <returns></returns>
        public JobParametersBuilder AddDate(string key, DateTime? parameter, bool identifying)
        {
            _parameterMap[key] = new JobParameter(parameter, identifying);
            return this;
        }

        /// <summary>
        /// Add a new identifying Long parameter for the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public JobParametersBuilder AddLong(string key, long parameter)
        {
            _parameterMap[key] = new JobParameter(parameter, true);
            return this;
        }

        /// <summary>
        /// Add a new Long parameter for the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parameter"></param>
        /// <param name="identifying"></param>
        /// <returns></returns>

        public JobParametersBuilder AddLong(string key, long parameter, bool identifying)
        {
            _parameterMap[key] = new JobParameter(parameter, identifying);
            return this;
        }

        /// <summary>
        /// Add a new identifying Double parameter for the given key. 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public JobParametersBuilder AddDouble(string key, double parameter)
        {
            _parameterMap[key] = new JobParameter(parameter, true);
            return this;
        }

        /// <summary>
        /// Adds a new Double parameter for the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parameter"></param>
        /// <param name="identifying"></param>
        /// <returns></returns>
        public JobParametersBuilder AddDouble(string key, double parameter, bool identifying)
        {
            _parameterMap[key] = new JobParameter(parameter, identifying);
            return this;
        }



        /// <summary>
        /// Conversion method that takes the current state of this builder and
        /// returns it as a JobruntimeParameters object.
        /// </summary>
        /// <returns></returns>
        public JobParameters ToJobParameters()
        {
            return new JobParameters(_parameterMap);
        }


        /// <summary>
        /// Adds a new  JobParameter for the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="jobParameter"></param>
        /// <returns></returns>
        public JobParametersBuilder AddParameter(string key, JobParameter jobParameter)
        {
            Assert.NotNull(jobParameter, "JobParameter must not be null");
            _parameterMap[key] = jobParameter;
            return this;
        } 
        #endregion

        #region IDisposable implementation
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
            if (disposing)
            {
                // free managed resources
                _parameterMap = null;
            }
        } 
        #endregion
    }
}
