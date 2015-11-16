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
        /// Check if the given character is '\u0000' ASCII value.
        /// </summary>
        /// <param name="character">char</param>  
        /// <returns>true if char is matching '\u0000' ASCII value. Otherwise false.</returns> 
        public static bool IsCharLowValue(char character)
        {
            return character == '\u0000';
        }

        /// <summary>
        /// Check if the given character is '\u009F' ASCII value.
        /// </summary>
        /// <param name="character"> character char</param>
        /// <returns>true if char is matching '\u009F' ASCII value. Otherwise false.</returns>
        public static bool IsCharHighValue(char character) 
        {
            return character == '\u009F';
        }

        /// <summary>
        /// Check if characters of the given string are all '\u0000' ASCII value.
        /// </summary>
        /// <param name="str">string</param>  
        /// <returns>true if all characters of given string are matching '\u0000' ASCII value. Otherwise false.</returns>
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
        /// Check if characters of the given string are all '\u009F' ASCII value.
        /// </summary>
        /// <param name="str">string</param>  
        /// <returns>true if all characters of given string are matching '\u009F' ASCII value. Otherwise false.</returns>
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
        /// Convert a bool to "Y" or "N" string.
        /// </summary>
        /// <param name="boolean">bool</param>
        /// <returns>"Y" if boolean == true, "N" if boolean == false. Otherwise null.</returns> 
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
        /// Convert a bool to "O" or "N" string.
        /// </summary>
        /// <param name="boolean">bool</param>
        /// <returns>"O" if parameter is true, "N" if parameter is false. Otherwise null.</returns> 
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
        /// Convert a "Y" or "N" string to bool.
        /// </summary>
        /// <param name="str">string</param>  
        /// <returns>true if parameter matches "Y", false if parameter matches "N". Otherwise null.</returns> 
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
        /// Convert a "O" or "N" string to bool.
        /// </summary>
        /// <param name="str">string</param>
        /// <returns>true if parameter matches "O", false if parameter matches "N". Otherwise null.</returns> 
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
        /// Check if all attributes of the object, whose types are Number, String or Date, are null or are empty/whitespace strings.
        /// </summary>
        /// <param name="obj">objet</param>
        /// <returns>true if all attributes of the object, whose types are Number, String or Date, are null or are empty/whitespace strings.</returns>
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
