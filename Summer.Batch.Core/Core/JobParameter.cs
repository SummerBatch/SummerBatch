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

namespace Summer.Batch.Core
{
    /// <summary>
    ///  Domain representation of a parameter to a batch job. Only the following types
    /// can be parameters: String, Long, Date, and Double.  The identifying flag is
    /// used to indicate if the parameter is to be used as part of the identification of
    /// a job instance.
    /// as org.springframework.batch.core.JobParameter
    /// </summary>
    [Serializable]
    public sealed class JobParameter
    {
        private readonly object _parameter;

        private readonly ParameterType _parameterType;

        private readonly bool _identifying;

        /// <summary>
        /// Constructor using a parameter and flag to tell if the given parameter is identifying.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="identifying"></param>
        public JobParameter(string parameter, bool identifying)
        {
            _parameter = parameter;
            _parameterType = ParameterType.String;
            _identifying = identifying;
        }

        /// <summary>
        /// Alternative constructor for a long parameter.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="identifying"></param>
        public JobParameter(long parameter, bool identifying)
        {
            _parameter = parameter;
            _parameterType = ParameterType.Long;
            _identifying = identifying;
        }

        /// <summary>
        /// Alternative constructor for a DateTime parameter.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="identifying"></param>
        public JobParameter(DateTime? parameter, bool identifying)
        {
            _parameter = parameter;
            _parameterType = ParameterType.Date;
            _identifying = identifying;
        }

        /// <summary>
        /// Alternative constructor for a double parameter.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="identifying"></param>
        public JobParameter(double parameter, bool identifying)
        {
            _parameter = parameter;
            _parameterType = ParameterType.Double;
            _identifying = identifying;
        }

        /// <summary>
        /// Constructor with a string parameter.
        /// </summary>
        /// <param name="parameter"></param>
        public JobParameter(String parameter)
        {
            _parameter = parameter;
            _parameterType = ParameterType.String;
            _identifying = true;
        }

        /// <summary>
        /// Alternative constructor with a long parameter.
        /// </summary>
        /// <param name="parameter"></param>
        public JobParameter(long parameter)
        {
            _parameter = parameter;
            _parameterType = ParameterType.Long;
            _identifying = true;
        }

        /// <summary>
        /// Alternative constructor with a DateTime parameter.
        /// </summary>
        /// <param name="parameter"></param>
        public JobParameter(DateTime parameter)
        {
            _parameter = parameter;
            _parameterType = ParameterType.Date;
            _identifying = true;
        }

        /// <summary>
        /// Alternative constructor with a double parameter.
        /// </summary>
        /// <param name="parameter"></param>
        public JobParameter(double parameter)
        {
            _parameter = parameter;
            _parameterType = ParameterType.Double;
            _identifying = true;
        }

        /// <summary>
        /// Parameter value getter.
        /// </summary>
        public object Value
        {
            get
            {
                if (_parameter is DateTime)
                {
                    return new DateTime(((DateTime)_parameter).Ticks);
                }
                else
                {
                    return _parameter;
                }
            }
        }

        /// <summary>
        /// Parameter type getter.
        /// </summary>
        public ParameterType Type { get { return _parameterType; } }

        /// <summary>
        /// Identifying flag.
        /// </summary>
        public bool Identifying { get { return _identifying; } }

        /// <summary>
        /// Equals override.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(Object obj)
        {
            if (!(obj is JobParameter))
            {
                return false;
            }

            if (this == obj)
            {
                return true;
            }

            JobParameter rhs = (JobParameter)obj;
            return _parameter == null ? rhs._parameter == null && _parameterType == rhs._parameterType : _parameter.Equals(rhs._parameter);
        }

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string pValue = _parameter == null
                ? "null"
                : (_parameterType == ParameterType.Date
                    ? "" + ((DateTime) _parameter).Millisecond
                    : _parameter.ToString());
            return string.Format("(Parameter Type={0}, Parameter Value={1})",_parameterType,pValue);
        }

        /// <summary>
        /// GetHashCode override.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return 7 + 21 * (_parameter == null ? _parameterType.GetHashCode() : _parameter.GetHashCode());
        }


        /// <summary>
        ///Enumeration representing the type of a JobParameter. 
        /// </summary>
        /// 
        public enum ParameterType
        {
            /// <summary>
            /// String enum litteral
            /// </summary>
            String, 
            /// <summary>
            /// Date enum litteral
            /// </summary>
            Date, 
            /// <summary>
            /// Long enum litteral
            /// </summary>
            Long, 
            /// <summary>
            /// Double enum litteral
            /// </summary>
            Double
        }

    }
}
