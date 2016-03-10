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
using Summer.Batch.Common.Util;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;

namespace Summer.Batch.Infrastructure.Support
{
    /// <summary>
    /// Properties converter helper (NameValueCollection to String, and back).
    /// </summary>
    public static class PropertiesConverter
    {
        private static readonly IPropertiesPersister PropertiesPersister = new DefaultPropertiesPersister();

        /// <summary>
        /// Parse a String to a NameValueCollection object. If string is null, an empty
        /// NameValueCollection object will be returned. The input String is a set of
        /// name=value pairs, delimited by either newline or comma (for brevity). If
        /// the input String contains a newline it is assumed that the separator is
        /// newline, otherwise comma.
        /// </summary>
        /// <param name="stringToParse">String to parse</param>
        /// <returns>NameValueCollection parsed from each string.</returns>
        /// <exception cref="IOException">&nbsp;in case of I/O errors</exception>
        public static NameValueCollection StringToProperties(string stringToParse)
        {
            if (stringToParse == null)
            {
                return new NameValueCollection();
            }

            var aStringToParse = stringToParse;

            if (!aStringToParse.Contains(Environment.NewLine))
            {
                aStringToParse = aStringToParse.Replace(",", Environment.NewLine);
            }

            var properties = new NameValueCollection();
            using (StringReader stringReader = new StringReader(aStringToParse))
            {                
                PropertiesPersister.Load(properties, stringReader);
            }

            return properties;
        }

        /// <summary>
        /// Convert NameValueCollection object to String. This is only necessary for
        /// compatibility with converting the String back to a NameValueCollection object. If
        /// an empty properties object is passed in, nempty string is returned,
        /// otherwise its string representation is returned.
        /// </summary>
        /// <param name="propertiesToParse">the NameValueCollection to load into</param>
        /// <returns>String representation of NameValueCollection object</returns>
        /// <exception cref="IOException">&nbsp;in case of I/O errors</exception>
        public static string PropertiesToString(NameValueCollection propertiesToParse)
        {
            // If properties is empty, return a blank string.
            if (propertiesToParse == null || propertiesToParse.Count == 0)
            {
                return "";
            }

            string value;
            using (var stringWriter = new StringWriter())
            {
                PropertiesPersister.Store(propertiesToParse, stringWriter);
                value = stringWriter.ToString();
            }
            value = value.TrimEnd('\r', '\n');
            // If the value is short enough (and doesn't contain commas), convert to
            // comma-separated...
            if (value.Length < 160 && !value.Contains(','))
            {
                value = value.Replace(Environment.NewLine, ",");
            }
            return value;
        }

    }
}
