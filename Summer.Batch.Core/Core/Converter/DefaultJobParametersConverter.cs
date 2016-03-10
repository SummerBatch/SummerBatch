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
using System.Globalization;

namespace Summer.Batch.Core.Converter
{
    /// <summary>
    ///  Converter for JobParameters instances using a simple naming
    /// convention for property keys. Key names that are prefixed with a '-' are
    /// considered non-identifying and will not contribute to the identity of a
    /// JobInstance.  Key names ending with "&lt;type&gt;" where
    /// type is one of string, date, long are converted to the corresponding type.
    /// The default type is string. E.g.
    ///
    /// schedule.date(date)=2007/12/11
    /// department.id(long)=2345
    ///
    /// The literal values are converted to the correct type using the default 
    /// strategies, augmented if necessary by the custom editors provided.
    ///
    /// If you need to be able to parse and format local-specific dates and numbers,
    /// you can inject formatters (NumberStyles,LongNumberStyles,NumberFormat,DecimalFormat)
    /// 
    /// </summary>
    public class DefaultJobParametersConverter : IJobParametersConverter
    {
        #region Constants
        /// <summary>
        /// Date type constant.
        /// </summary>
        public const string DateType = "(date)";

        /// <summary>
        /// String type constant.
        /// </summary>
        public const string StringType = "(string)";

        /// <summary>
        /// Long type constant.
        /// </summary>
        public const string LongType = "(long)";

        private const string DoubleType = "(double)";
        private const string NonIdentifyingFlag = "-";
        private const string IdentifyingFlag = "+"; 
        #endregion

        #region Attributes
        private string _dateFormat = "yyyy/MM/dd";
        /// <summary>
        /// Date format property.
        /// </summary>
        public string DateFormat
        {
            set { _dateFormat = value; }
        }

        private NumberStyles _numberStyles = NumberStyles.Float;

        /// <summary>
        /// Number style property.
        /// </summary>
        public NumberStyles NumberStyles { set { _numberStyles = value; } }

        private NumberStyles _longNumberStyles = NumberStyles.Integer;

        /// <summary>
        /// Long number style property.
        /// </summary>
        public NumberStyles LongNumberStyles { set { _longNumberStyles = value; } }

        private string _numberFormat = "{0,1:N0}";
        
        /// <summary>
        /// Numer format property.
        /// </summary>
        public string NumberFormat { set { _numberFormat = value; } }

        private string _decimalFormat = "{0:#.#}";

        /// <summary>
        /// Decimal format property.
        /// </summary>
        public string DecimalFormat { set { _decimalFormat = value; } } 
        #endregion


        private static bool IsIdentifyingKey(string key)
        {
            return !key.StartsWith(NonIdentifyingFlag); 
        }

        /// <summary>
        /// Parses given value as a long.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">&nbsp;</exception>
        private long ParseLong(string value)
        {
            long result;
            bool parsed = long.TryParse(value, _longNumberStyles, CultureInfo.InvariantCulture, out result);
            if (!parsed)
            {
                throw new ArgumentException(string.Format("Number format is invalid :[{0}] {1}" ,value, _numberStyles));
            }
            return result;
        }

        /// <summary>
        /// Parses given value as a double.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private double ParseDouble(string value)
        {
            double result;
            bool parsed = double.TryParse(value, _numberStyles, CultureInfo.InvariantCulture, out result);
            if (!parsed)
            {
                throw new ArgumentException(string.Format("Number format is invalid :[{0}] {1}" ,value, _numberStyles));
            }
            return result;
        }

        /// <summary>
        ///  Checks for suffix on keys and use those to decide how to convert the value.
        /// </summary>
        /// <param name="props"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">&nbsp; if a number or date is passed in that cannot be parsed, or cast to the correct type.</exception>
        public JobParameters GetJobParameters(NameValueCollection props)
        {
            if (props == null || props.Count == 0)
            {
                return new JobParameters();
            }

            JobParametersBuilder propertiesBuilder = new JobParametersBuilder();
            foreach (string pkey in props.Keys)
            {
                string key = pkey;
                string value = props[pkey];
                bool identifying = IsIdentifyingKey(key);
                if (!identifying)
                {
                    key = StringUtils.ReplaceFirst(key, NonIdentifyingFlag, "");
                }
                else if (key.StartsWith(IdentifyingFlag))
                {
                    key = StringUtils.ReplaceFirst(key, IdentifyingFlag, "");
                }

                if (key.EndsWith(DateType))
                {
                    DateTime? date;
                    try
                    {
                        date = DateTime.ParseExact(value, _dateFormat, CultureInfo.InvariantCulture);
                    }
                    catch (FormatException)
                    {
                        throw new ArgumentException(string.Format("Date Format is invalid :[{0}] {1}" ,value, _dateFormat));
                    }
                    propertiesBuilder.AddDate(key.Replace(DateType, string.Empty), date, identifying);
                }
                else if (key.EndsWith(LongType))
                {
                    long result = ParseLong(value);
                    propertiesBuilder.AddLong(key.Replace(LongType, string.Empty), result, identifying);
                }
                else if (key.EndsWith(DoubleType))
                {
                    double result = ParseDouble(value);
                    propertiesBuilder.AddDouble(key.Replace(DoubleType, string.Empty), result, identifying);
                }
                else if (StringUtils.EndsWithIgnoreCase(key, StringType))
                {
                    propertiesBuilder.AddString(key.Replace(StringType, string.Empty), value, identifying);
                }
                else
                {
                    propertiesBuilder.AddString(key, value, identifying);
                }
            }

            return propertiesBuilder.ToJobParameters();
        }

        /// <summary>
        /// Use the same suffixes to create properties (omitting the string suffix
        /// because it is the default).  Non-identifying parameters will be prefixed
        /// with the <see cref="NonIdentifyingFlag"/>.  However, since parameters are
        /// identifying by default, they will <em>not</em> be prefixed with the
        /// <see cref="IdentifyingFlag"/>.
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public NameValueCollection GetProperties(JobParameters parms)
        {
            if (parms == null || parms.IsEmpty())
            {
                return new NameValueCollection();
            }

            IDictionary<string, JobParameter> parameters = parms.GetParameters();
            NameValueCollection result = new NameValueCollection();

            foreach (KeyValuePair<string, JobParameter> entry in parameters)
            {
                string key = entry.Key;
                JobParameter jobParameter = entry.Value;
                Object value = jobParameter.Value;
                if (value != null)
                {
                    key = (!jobParameter.Identifying ? NonIdentifyingFlag : "") + key;
                    if (jobParameter.Type == JobParameter.ParameterType.Date)
                    {
                        result.Set(key + DateType, string.Format(_dateFormat, value));
                    }
                    else if (jobParameter.Type == JobParameter.ParameterType.Long)
                    {
                        result.Set(key + LongType, string.Format(_numberFormat, value));
                    }
                    else if (jobParameter.Type == JobParameter.ParameterType.Double)
                    {
                        result.Set(key + DoubleType, string.Format(_decimalFormat, (double)value));
                    }
                    else
                    {
                        result.Set(key, "" + value);
                    }
                }
            }

            return result;
        }
    }
}
