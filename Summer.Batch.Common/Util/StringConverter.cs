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
using System;
using System.Globalization;

namespace Summer.Batch.Common.Util
{
    /// <summary>
    /// Utility class to convert a string.
    /// </summary>
    public static class StringConverter
    {
        /// <summary>
        /// Convert a string to T.
        /// Supported types are types defined in <see cref="TypeCode"/> and
        /// single dimension arrays of these types.
        /// </summary>
        /// <typeparam name="T">the type to convert to</typeparam>
        /// <param name="toConvert">the string to convert</param>
        /// <returns>the converted object</returns>
        public static T Convert<T>(string toConvert)
        {
            return (T)Convert(typeof(T), toConvert);
        }

        /// <summary>
        /// Convert a string to T. 
        /// Supported types are types defined in <see cref="TypeCode"/> and
        /// single dimension arrays of these types.
        /// </summary>
        /// <param name="type">the type to convert to</param>
        /// <param name="toConvert">the string to convert</param>
        /// <returns>the converted object</returns>
        private static object Convert(Type type, string toConvert)
        {
            object result;
            if (type.IsArray)
            {
                result = ConvertToArray(type, toConvert);
            }
            else
            {
                var typeCode = Type.GetTypeCode(type);
                result = typeCode == TypeCode.Object
                    ? toConvert
                    : System.Convert.ChangeType(toConvert, typeCode, CultureInfo.InvariantCulture);
            }
            return result;
        }

        /// <summary>
        /// Converts a string to an array.
        /// </summary>
        /// <param name="type">the array type</param>
        /// <param name="toConvert">the string to convert</param>
        /// <returns>the converted array</returns>
        private static object ConvertToArray(Type type, string toConvert)
        {
            var split = toConvert.Split(',');
            var elementType = type.GetElementType();
            var array = Array.CreateInstance(elementType, split.Length);
            for (var i = 0; i < split.Length; i++)
            {
                array.SetValue(Convert(elementType, split[i]), i);
            }
            return array;
        }
    }
}
