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
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Summer.Batch.Extra.Utils
{
    /// <summary>
    /// Reverse Helper.
    /// </summary>
    public static class ReverseUtils
    {
        // Logger declaration.
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    
        private static readonly IList<string> METHODSTOBEIGNORED = new List<string>() {"getIdentifier", "getVersion"};
        private static readonly IList<Type> ATTRIBUTETYPES = new List<Type>() { typeof(string), typeof(DateTime), typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) };
        private static readonly string GET = "get";

        /// <summary>
        /// Check if the given character is <c>'\\u0000'</c> ASCII value.
        /// </summary>
        /// <param name="character">char</param>  
        /// <returns><c>true</c> if char is matching <c>'\\u0000'</c> ASCII value. Otherwise <c>false</c>.</returns> 
        public static bool IsCharLowValue(char character)
        {
            return character == '\u0000';
        }

        /// <summary>
        /// Check if the given character is <c>'\\u009F'</c> ASCII value.
        /// </summary>
        /// <param name="character"> character char</param>
        /// <returns><c>true</c> if char is matching <c>'\\u009F'</c> ASCII value. Otherwise <c>false</c>.</returns>
        public static bool IsCharHighValue(char character) 
        {
            return character == '\u009F';
        }

        /// <summary>
        /// Check if characters of the given string are all <c>'\\u0000'</c> ASCII value.
        /// </summary>
        /// <param name="str">string</param>  
        /// <returns><c>true</c> if all characters of given string are matching <c>'\\u0000'</c> ASCII value. Otherwise <c>false</c>.</returns>
        public static bool IsStringLowValue(string str) 
        {
            var result = true;
            var stringAsArray = str.ToCharArray();
            if(StringUtils.IsBlank(str)) 
            {
                result = false;
            } 
            else
            {
                if (stringAsArray.Any(currentchar => !IsCharLowValue(currentchar)))
                {
                    result = false;
                }
            }
            return result;		
        }

        /// <summary>
        /// Check if characters of the given string are all <c>'\\u009F'</c> ASCII value.
        /// </summary>
        /// <param name="str">string</param>  
        /// <returns><c>true</c> if all characters of given string are matching <c>'\\u009F'</c> ASCII value. Otherwise <c>false</c>.</returns>
        public static bool IsStringHighValue(string str) 
        {
            var result = true;
            var stringAsArray = str.ToCharArray();
            if(StringUtils.IsBlank(str))
            {
                result = false;
            }
            else
            {
                if (stringAsArray.Any(currentchar => !IsCharHighValue(currentchar)))
                {
                    result = false;
                }
            }
            return result;		
        }

        /// <summary>
        /// Convert a bool to <c>"Y"</c> or <c>"N"</c> string.
        /// </summary>
        /// <param name="boolean">bool</param>
        /// <returns><c>"Y"</c> if <c>boolean == true</c>, <c>"N"</c> if <c>boolean == false</c>. Otherwise <c>null</c>.</returns> 
        public static string ConvertBooleanToStringYN(bool? boolean)
        {
            string stringValue = null;
            if(boolean != null)
            {
                stringValue = boolean.Value ? "Y" : "N";
            }
            return stringValue;
        }

        /// <summary>
        /// Convert a bool to <c>"O"</c> or <c>"N"</c> string.
        /// </summary>
        /// <param name="boolean">bool</param>
        /// <returns><c>"O"</c> if parameter is <c>true</c>, <c>"N"</c> if parameter is <c>false</c>. Otherwise <c>null</c>.</returns> 
        public static string ConvertBooleanToStringON(bool? boolean)
        {
            string stringValue = null;
            if(boolean != null)
            {
                stringValue = boolean.Value ? "O" : "N";
            }
            return stringValue;
        }

        /// <summary>
        /// Convert a <c>"Y"</c> or <c>"N"</c> string to bool.
        /// </summary>
        /// <param name="str">string</param>  
        /// <returns><c>true</c> if parameter matches <c>"Y"</c>, <c>false</c> if parameter matches <c>"N"</c>. Otherwise <c>null</c>.</returns> 
        public static bool? ConvertStringToBooleanYN(string str)
        {
            bool? boolean = null;
            if(str != null)
            {
                if("Y".Equals(str))
                {
                    boolean = true;
                } 
                else if ("N".Equals(str))
                {
                    boolean = false;
                }
            }
            return boolean;
        }

        /// <summary>
        /// Convert a <c>"O"</c> or <c>"N"</c> string to bool.
        /// </summary>
        /// <param name="str">string</param>
        /// <returns><c>true</c> if parameter matches <c>"O"</c>, false if parameter matches <c>"N"</c>. Otherwise <c>null</c>.</returns> 
        public static bool? ConvertStringToBooleanON(string str)
        {
            bool? boolean = null;
            if(str != null)
            {
                if("O".Equals(str))
                {
                    boolean = true;
                }
                else if ("N".Equals(str))
                {
                    boolean = false;
                }
            }
            return boolean;
        }

        /// <summary>
        /// Check if all attributes of the object, whose types are Number, String or Date, are <c>null</c> or are empty/whitespace strings.
        /// </summary>
        /// <param name="obj">objet</param>
        /// <returns><c>true</c> if all attributes of the object, whose types are Number, String or Date, are <c>null</c> or are empty/whitespace strings.</returns>
        public static bool IsSpaces(object obj)
        {
            bool isSpaces = true;
            foreach (MethodInfo m in obj.GetType().GetMethods(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Instance)) {
                if (IsMethodToBeIgnored(m) || !IsAttribute(m.ReturnType))
                {
                    continue;
                }
                isSpaces = IsAttributeNull(m, obj);
                if (!isSpaces)
                {
                    break;
                }
            }
            return isSpaces;
        }

        /// <summary>
        /// IsMethodToBeIgnored.
        /// </summary>
        /// <param name="m">MethodInfo</param>
        /// <returns>bool</returns> 
        private static bool IsMethodToBeIgnored(MethodInfo m)
        {
            var result = true;
            if (m.Name.StartsWith(GET))
            {
                result = METHODSTOBEIGNORED.Contains(m.Name);
            } 
            return result;
        }

        /// <summary>
        /// isAttribute.
        /// </summary>
        /// <param name="clazz">Type</param>
        /// <returns>bool</returns>
        private static bool IsAttribute(Type clazz)
        {
            return ATTRIBUTETYPES.Any(c => c.IsAssignableFrom(clazz));
        }

        /// <summary>
        /// isAttributeNull.
        /// </summary>
        /// <param name="m">MethodInfo</param>
        /// <param name="obj">object</param>
        /// <returns>bool</returns>
        private static bool IsAttributeNull(MethodInfo m, object obj)
        {
            object value = null;
            try
            {
                value = m.Invoke(obj,null);
            }
            catch (Exception e)
            {
                if (e is TargetException || e is ArgumentException || e is TargetInvocationException || e is TargetParameterCountException
                      || e is MethodAccessException || e is InvalidOperationException || e is NotSupportedException)
                {
                    Logger.Error(e,"An error occured while executing method: isAttributeNull ");
                }
            }
            bool result;
            if (typeof(string).IsAssignableFrom(m.ReturnType))
            {
                result = StringUtils.IsBlank((string)value);
            }
            else
            {
                result = (value == null);
            }
            return result;
        }
    }
}
