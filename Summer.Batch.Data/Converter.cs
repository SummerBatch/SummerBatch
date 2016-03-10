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
using System;
using System.Globalization;

namespace Summer.Batch.Data
{
    /// <summary>
    /// Utility class for converting data to unify the type of the values read from
    /// the database providers.
    /// </summary>
    public static class Converter
    {
        private const string ErrorMessage = "Cannot convert from type {0} to type {1}.";

        /// <summary>
        /// Converts an object to the specified type.
        /// </summary>
        /// <typeparam name="T">&nbsp;the type to convert to</typeparam>
        /// <param name="obj">the object to convert</param>
        /// <returns><paramref name="obj"/> converted to type <typeparamref name="T"/>.</returns>
        /// <exception cref="InvalidOperationException">&nbsp;
        /// if <paramref name="obj"/> cannot be converted to type <typeparamref name="T"/>
        /// </exception>
        public static T Convert<T>(object obj)
        {
            return (T)Convert(obj, typeof(T));
        }

        /// <summary>
        /// Converts an object to the specified type.
        /// </summary>
        /// <param name="obj">the object to convert</param>
        /// <param name="type">the type to convert to</param>
        /// <returns><paramref name="obj"/> converted to type <paramref name="type"/>.</returns>
        /// <exception cref="InvalidOperationException">&nbsp;
        /// if <paramref name="obj"/> cannot be converted to type <paramref name="type"/>
        /// </exception>
        public static object Convert(object obj, Type type)
        {
            // If the object is already of the right type, there is no conversion to be done
            if (obj.GetType().IsAssignableFrom(type))
            {
                return obj;
            }
            // If the object is convertible, convert it.
            var convertible = obj as IConvertible;
            if (convertible != null)
            {
                return convertible.ToType(type, CultureInfo.InvariantCulture);
            }
            throw new InvalidOperationException(string.Format(ErrorMessage, obj.GetType().FullName, type.FullName));
        }
    }
}