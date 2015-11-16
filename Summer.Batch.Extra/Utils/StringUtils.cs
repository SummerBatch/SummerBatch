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
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace Summer.Batch.Extra.Utils
{
    /// <summary>
    /// String helper.
    /// </summary>
    public static class StringUtils
    {
        // Logger declaration.
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// STANDARD_DATE_FORMAT 
        public static readonly string STANDARD_DATE_FORMAT = "dd/MM/yyyy";

        /// NO_SEPARATOR_SHORT_DATE_FORMAT
        public static readonly string NO_SEPARATOR_SHORT_DATE_FORMAT = "ddMMyy";

        /// LIBELLE_DATE_FORMAT 
        public static readonly string LIBELLE_DATE_FORMAT = "dd MMMMMMMMM yyyy";

        /// UNDERSCORE_DATE_FORMAT
        public static readonly string UNDERSCORE_DATE_FORMAT = "dd_MM_yyyy";

        /// TIRET_DATE_FORMAT
        public static readonly string TIRET_DATE_FORMAT = "dd-MM-yyyy";

        /// STANDARD_HOUR_DATE_FORMAT
        public static readonly string STANDARD_HOUR_DATE_FORMAT = "dd/MM/yyyy HH:mm";

        /// STANDARD_HOUR_DATE_FORMAT_LETTRE
        public static readonly string STANDARD_HOUR_DATE_FORMAT_LETTRE = "dd/MM/yyyy HH'h'mm";

        /// STANDARD_HOUR_DATE_FORMAT_FILE
        public static readonly string STANDARD_HOUR_DATE_FORMAT_FILE = "dd_MM_yyyy_HH'h'mm'm'ss";

        /// DATE_PATERN
        private static readonly Regex DATE_REGEX = new Regex("[0-9]{6}|[0-9]{8}|[0-9]{2}/[0-9]{2}/[0-9]{2}|[0-9]{2}/[0-9]{2}/[0-9]{4}");

        /// <summary>
        /// Concat two strings.
        /// </summary>
        /// <param name="str1">string</param>
        /// <param name="str2">string</param>
        /// <returns>the string that is a concatenation of str1 and str2, handling null parameters ("abc"+null="abc").</returns>
        public static string Concat(string str1, string str2)
        {
            return string.Concat(str1, str2);
        }

        /// <summary>
        /// Check if the given string is a valid email address.
        /// </summary>
        /// <param name="mail">string</param>
        /// <returns>true if string matches format XX@XX.XX.</returns> 
        public static bool IsValidEmailAddress(string mail)
        {
            try
            {
                var email = new MailAddress(mail);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        /// <summary>
        /// Check is the given string matches a valid date.
        /// </summary>
        /// <param name="sDate">string</param>  
        /// <returns>true if string matches "dd/MM/yyyy" and is a valid date (ex : 31/02/2012 forbidden).</returns> 
        public static bool IsDate(string sDate)
        {
            var isValidDate = DATE_REGEX.IsMatch(sDate);
            var result = isValidDate;
            if (isValidDate)
            {
                try
                {
                    var resultDate = DateTime.ParseExact(sDate, STANDARD_DATE_FORMAT, CultureInfo.InvariantCulture);
                    var resultstring = resultDate.ToShortDateString();
                    if (!sDate.Equals(resultstring))
                    {
                        result = false;
                    }
                }
                catch (Exception e)
                {
                    if (e is ArgumentNullException || e is FormatException)
                    {
                        Logger.Debug(e, "stringUtils::isDate");
                        result = false;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Check if the string matches a double value.
        /// </summary>
        /// <param name="chaine">string</param>
        /// <returns>true if string matches a double value.</returns>
        public static bool IsDouble(string chaine)
        {
            var result = true;
            try
            {
                double.Parse(chaine, CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                if (e is ArgumentNullException || e is FormatException || e is OverflowException)
                    Logger.Debug(e, "stringUtils::isDouble");
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Check if the string matches a int value.
        /// </summary>
        /// <param name="chaine">string</param>
        /// <returns>true if string matches an int value.</returns>
        public static bool IsInteger(string chaine)
        {
            var result = true;
            try
            {
                int.Parse(chaine);
            }
            catch (Exception e)
            {
                if (e is ArgumentNullException || e is FormatException || e is OverflowException)
                {
                    Logger.Debug(e, "stringUtils::isInteger");
                    result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// Return the char at the given position from the given string.
        /// </summary>
        /// <param name="str">string</param>
        /// <param name="position">int</param>
        /// <returns>the character at the specified index of this string. The first character is at index 0.</returns>
        public static char CharAt(string str, int position)
        {
            return str.ElementAt(position);
        }

        /// <summary>
        /// Compares two strings lexicographically.
        /// </summary>
        /// <param name="str1">string</param>
        /// <param name="str2">string</param>
        /// <returns>the value 0 if the argument str2 is equal to str1; a value less than 0 if str1 is lexicographically less than str2; and a value greater than 0 if str1 is lexicographically greater than str2.</returns>
        public static int CompareTo(string str1, string str2)
        {
            return string.Compare(str1, str2, StringComparison.Ordinal);
        }

        /// <summary>
        /// Compares two strings lexicographically, ignoring case differences.
        /// </summary>
        /// <param name="str1">string</param>
        /// <param name="str2">string</param>
        /// <returns>the value 0 if the argument str2 is equal to str1; a value less than 0 if str1 is lexicographically less than str2; and a value greater than 0 if str1 is lexicographically greater than str2.</returns>
        public static int CompareToIgnoreCase(string str1, string str2)
        {
            return string.Compare(str1, str2, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Encodes this string into a sequence of bytes using the named charset, storing the result into a new byte array.
        /// </summary>
        /// <param name="str">string</param>
        /// <returns>the resultant byte array.</returns>
        public static byte[] GetBytes(string str)
        {
            return Encoding.Default.GetBytes(str);
        }

        /// <summary>
        /// Return the length of the given string.
        /// </summary>
        /// <param name="str">string</param>  
        /// <returns>the length of this string.</returns>
        public static int Length(string str)
        {
            return str.Length;
        }

        /// <summary>
        /// Check if the givenstring matches the given regex.
        /// </summary>
        /// <param name="str">string</param>
        /// <param name="regexp">string</param>
        /// <returns>true if str matches regexp. Otherwise false.</returns>
        public static bool Matches(string str, string regexp)
        {
            return Regex.IsMatch(str, regexp);
        }

        /// <summary>
        /// Replace every occurrence of the given character by the new one, in the given string.
        /// </summary>
        /// <param name="str">string</param>
        /// <param name="oldChar">char</param>
        /// <param name="newChar">char</param>
        /// <returns>a string derived from this string by replacing every occurrence of oldChar with newChar.</returns>
        public static string Replace(string str, char oldChar, char newChar)
        {
            return str.Replace(oldChar, newChar);
        }

        /// <summary>
        /// Replace every occurence of a given searched string by the replacement one, in the given string.
        /// </summary>
        /// <param name="str">string</param>
        /// <param name="searchstring">string</param>
        /// <param name="replacement">string</param>
        /// <returns>the string with any replacements processed, null if null string input.</returns>
        public static string Replace(string str, string searchstring, string replacement)
        {
            return str.Replace(searchstring, replacement);
        }

        /// <summary>
        /// Replace every occurence of the given string matching the given regex, by the replacement string.
        /// </summary>
        /// <param name="str">string</param>
        /// <param name="regex">string</param>
        /// <param name="replacement">string</param>
        /// <returns>a string derived from this string by replacing every occurrence of regexp with replacement parameter.</returns>
        public static string ReplaceAll(string str, string regex, string replacement)
        {
            return Regex.Replace(str, regex, replacement);
        }

        /// <summary>
        /// Replace the first occurence of the given string matching the given regex, by the replacement string.
        /// </summary>
        /// <param name="str">str</param>
        /// <param name="regex">regex</param>
        /// <param name="replacement">replacement</param>
        /// <returns>a string derived from this string by replacing the first occurrence of regexp with replacement parameter.</returns> 
        public static string ReplaceFirst(string str, string regex, string replacement)
        {
            return (new Regex(regex)).Replace(str, replacement, 1);
        }

        /// <summary>
        /// Split the given string at avery occurence of given regex and return an array of splitted strings.
        /// </summary>
        /// <param name="str">str</param>
        /// <param name="regex">regex</param>
        /// <returns>the array of strings computed by splitting this string around matches of the given regular expression.</returns>
        public static string[] Split(string str, string regex)
        {
            return Regex.Split(str, regex);
        }

        /// <summary>
        /// Return an array of character represention of the given string.
        /// </summary>
        /// <param name="str">string</param>
        /// <returns>a newly allocated character array whose length is the length of this string and whose contents are initialized to contain the character sequence represented by this string.</returns>
        public static char[] ToCharArray(string str)
        {
            return str.ToCharArray();
        }

        /// <summary>
        /// Convert the given int to string of the given size.
        /// </summary>
        /// <param name="input">int?</param>  
        /// <param name="size">int?</param>  
        /// <returns>if size is not nul a new string left padded with zeros until the size of the crafted string is e, if size is null the tostring value.</returns>
        public static string ConvertIntegerTostring(int? input, int? size)
        {
            string inputAsString;
            if (input != null && size != null)
            {
                inputAsString = (input.ToString()).PadLeft((int)size, '0');
            }
            else if (input != null)
            {
                inputAsString = input.ToString();
            }
            else
            {
                inputAsString = "";
            }
            return inputAsString;
        }

        /// <summary>
        /// Convert the given int to string.
        /// </summary>
        /// <param name="input">int?</param>
        /// <returns>the tostring value of the Integer parameter if not null, "" otherwise.</returns>
        public static string ConvertIntegerTostring(int? input)
        {
            return input != null ? input.ToString() : "";
        }

        /// <summary>
        /// Check if the given string is empty.
        /// </summary>
        /// <param name="str">string</param>  
        /// <returns>true if the string is empty or null.</returns> 
        public static bool IsEmpty(string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// Check if the given string is not empty.
        /// </summary>
        /// <param name="str">string</param>
        /// <returns>true if the string is not empty and not null.</returns>
        public static bool IsNotEmpty(string str)
        {
            return !string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// Check if the given string is blank.
        /// </summary>
        /// <param name="str">string</param>
        /// <returns>true if the string is null, empty or whitespace.</returns>
        public static bool IsBlank(string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }


        /// <summary>
        /// Check if the given string is not blank.
        /// </summary>
        /// <param name="str">string</param>
        /// <returns>true if the string is not null, not empty and not whitespace.</returns>
        public static bool IsNotBlank(string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// Trim the string.
        /// </summary>
        /// <param name="str">string</param>
        /// <returns>the trimmed string, null if null string input.</returns>
        public static string Trim(string str)
        {
            return str == null ? null : str.Trim();
        }

        /// <summary>
        /// Strip the string.
        /// </summary>
        /// <param name="str">string</param>
        /// <returns>the stripped string, null if null string input.</returns>
        public static string Strip(string str)
        {
            return str.Trim();
        }

        /// <summary>
        /// Check if the given strings are equal.
        /// </summary>
        /// <param name="str1">string</param>
        /// <param name="str2">string</param>
        /// <returns>true if the strings are equal, case sensitive, or both null.</returns> 
        public static bool AreEqual(string str1, string str2)
        {
            return string.Equals(str1, str2);
        }

        /// <summary>
        /// Check if the given strings are equal, ignoring case.
        /// </summary>
        /// <param name="str1">string</param>
        /// <param name="str2">string</param>
        /// <returns>true if the strings are equal, case insensitive, or both null.</returns>
        public static bool EqualsIgnoreCase(string str1, string str2)
        {
            return string.Equals(str1, str2, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Return the index of the first occurence of the given character in the given string.
        /// </summary>
        /// <param name="str">string</param>
        /// <param name="searchChar">char</param>
        /// <returns>the first index of the search character, -1 if no match or null string input.</returns>
        public static int IndexOf(string str, char searchChar)
        {
            return str == null ? -1 : str.IndexOf(searchChar);
        }

        /// <summary>
        /// Return the index of the last occurence of the given character in the given string.
        /// </summary>
        /// <param name="str">string</param>
        /// <param name="searchChar">char</param>
        /// <returns>the last index of the search character, -1 if no match or null string input.</returns>
        public static int LastIndexOf(string str, char searchChar)
        {           
            return str == null ? -1 : str.LastIndexOf(searchChar);
        }

        /// <summary>
        /// Check if the given string contains the given character.
        /// </summary>
        /// <param name="str">string</param> 
        /// <param name="searchChar">char</param>
        /// <returns>true if the string contains the search character, false if not or null string input.</returns>
        public static bool Contains(string str, char searchChar)
        {
            return str != null && str.Contains(searchChar);
        }

        /// <summary>
        /// Check if the given string contains the given search string.
        /// </summary>
        /// <param name="str">string</param>
        /// <param name="searchStr">string</param>
        /// <returns>true if the string contains the search string, false if not or null string input.</returns>
        public static bool Contains(string str, string searchStr)
        {
            return str.Contains(searchStr);
        }

        /// <summary>
        /// Substring the given string from the given start position.
        /// </summary>
        /// <param name="str">string</param>
        /// <param name="start">int</param>
        /// <returns>substring from start position, null if null string input. A negative start position can be used to start n characters from the end of the string.</returns>
        public static string Substring(string str, int start)
        {
            var substring = "";
            if (str == null)
            {
                substring = null;
            }
            else if (string.Empty.Equals(str) || (start >= 0 && start >= str.Length))
            {
                substring = "";
            }
            else
            {
                if (start >= 0)
                {
                    substring = str.Substring(start);
                }
                else
                {
                    if (start < 0 && -start >= str.Length)
                    {
                        substring = str;
                    }
                    else
                    {
                        substring = str.Substring(str.Length + start);
                    }
                }
            }
            return substring;
        }

        /// <summary>
        /// Substring the given string from the given start position to position plus the given lenght.
        /// </summary>
        /// <param name="str">string</param>
        /// <param name="start">int</param>
        /// <param name="length">int</param>
        /// <returns>substring from start position to start position plus length, null if null string input.</returns>
        public static string Substring(string str, int start, int length)
        {
            return str.Substring(start, length);
        }

        /// <summary>
        /// Chomp the string.
        /// </summary>
        /// <param name="str">string</param>  
        /// <returns>string without newline, null if null string input.</returns> 
        public static string Chomp(string str)
        {
            if (str == null)
            {
                return null;
            }
            else
            {
                foreach (string newLineString in new[] { "\r\n", "\r", "\n" })
                {
                    if (str.EndsWith(newLineString))
                    {
                        return newLineString.Equals("\r\n") ? str.Remove(str.Length - 2) : str.Remove(str.Length - 1);

                    }
                }
                return str;
            }
        }

        /// <summary>
        /// Chop the string.
        /// </summary>
        /// <param name="str">string</param>
        /// <returns>string without last character, null if null string input.</returns> 

        public static string Chop(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            else
            {
                return str.EndsWith("\r\n") ? str.Remove(str.Length - 2) : str.Remove(str.Length - 1);
            }
        }

        /// <summary>
        /// Return a string made of the given string repeated n times.
        /// </summary>
        /// <param name="str">string</param>
        /// <param name="repeat">int</param>
        /// <returns>a new string consisting of the original string repeated n times, null if null string input.</returns>
        public static string Repeat(string str, int repeat)
        {
            if (str == null)
            {
                return null;
            }
            else
            {
                return repeat >= 0 ? string.Concat(Enumerable.Repeat(str, repeat)) : string.Concat(Enumerable.Repeat(str, 0));
            }
        }

        /// <summary>
        /// <param name="str">string</param>  
        /// <returns>the upper cased string, null if null string input.</returns>
        /// </summary>
        public static string UpperCase(string str)
        {
            return str == null ? null : str.ToUpper();
        }

        /// <summary> 
        /// Return the given string with lower case charaters only.
        /// </summary>
        /// <param name="str">string</param>
        /// <returns>the lower cased string, null if null string input.</returns> 
        public static string LowerCase(string str)
        {
            return str == null ? null : str.ToLower();
        }

        /// <summary>
        /// Check if the given string only contains unicode letters.
        /// </summary>
        /// <param name="str">string</param> str string
        /// <returns>true if the string only contains unicode letters.</returns>
        public static bool IsAlpha(string str)
        {
            return !string.IsNullOrWhiteSpace(str) && str.All(char.IsLetter);
        }

        /// <summary>
        /// Check if the given string only contains letters.
        /// </summary>
        /// <param name="str">string</param>
        /// <returns>true if only contains letters, and is non-null.</returns>
        public static bool IsAlphaSpace(string str)
        {
            return str != null && str.All(c => char.IsLetter(c) || char.IsWhiteSpace(c));
        }

        /// <summary>
        /// Check if the given string only contains letters or digits.
        /// </summary>
        /// <param name="str">string</param>
        /// <returns>true if only contains letters or digits, and is non-null.</returns>
        public static bool IsAlphanumeric(string str)
        {
            return !string.IsNullOrWhiteSpace(str) && str.All(char.IsLetterOrDigit);
        }

        /// <summary>
        /// Check if the given string only contains letters, digits or spaces.
        /// </summary>
        /// <param name="str">string</param>
        /// <returns>true if only contains letters, digits or spaces, and is non-null.</returns>
        public static bool IsAlphanumericSpace(string str)
        {
            return str != null && str.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c));
        }

        /// <summary>
        /// Check if the given string only contains digits.
        /// </summary>
        /// <param name="str">string</param>
        /// <returns>true if only contains digits, and is non-null.</returns>
        public static bool IsNumeric(string str)
        {
            return !string.IsNullOrWhiteSpace(str) && str.All(char.IsDigit);
        }

        /// <summary>
        /// Check if the given string only contains digits or spaces.
        /// </summary>
        /// <param name="str">string</param>  
        /// <returns>true if only contains digits or spaces, and is non-null.</returns> 
        public static bool IsNumericSpace(string str)
        {
            return str != null && str.All(c => char.IsDigit(c) || char.IsWhiteSpace(c));
        }

        /// <summary>
        /// Check if the given string only contains whitespaces.
        /// </summary>
        /// <param name="str">string</param>  
        /// <returns>true if only contains whitespaces, and is non-null.</returns> 
        public static bool IsWhitespace(string str)
        {
            return str != null && str.All(char.IsWhiteSpace);
        }

        /// <summary>
        /// Reverse the given string.
        /// </summary>
        /// <param name="str">string</param>  
        /// <returns>the reversed string, null if null string input.</returns> 
        public static string Reverse(string str)
        {
            return str == null ? null : new string(str.ToCharArray().Reverse().ToArray());
        }

        /// <summary>
        /// Strip the given string.
        /// </summary>
        /// <param name="str">string</param>   
        /// <param name="stripChars">string</param>
        /// <returns>the stripped string, <code>null</code> if null string input.</returns> 
        public static string StripEnd(string str, string stripChars)
        {
            if (str == null)
            {
                return null;
            }
            else
            {
                return stripChars == null ? 
                    StripEnd(str, new string((str.ToCharArray().Where(char.IsWhiteSpace)).ToArray())) 
                    : str.TrimEnd(stripChars.ToArray());
            }
        }

        /// <summary>
        /// Returns a formatted string using the specified format string, and arguments.
        /// </summary>
        /// <param name="format">string</param>
        /// <param name="argument">object</param>
        /// <returns>A formatted string.</returns>  
        public static string Format(string format, object[] argument)
        {
            return string.Format(CultureInfo.CurrentUICulture, format, argument);
        }

        /// <summary>
        /// Gets the substring before the first occurrence of a separator. The separator is not returned.
        /// </summary>
        /// <param name="str">string</param>
        /// <param name="separator">string</param>
        /// <returns>the substring before the first occurrence of the separator, null if null string input.</returns> 
        public static string SubstringBefore(string str, string separator)
        {
            if (str == null)
            {
                return null;
            }
            else if (separator == null || str.Equals(string.Empty) || !str.Contains(separator))
            {
                return str;
            }
            else if (separator.Equals(string.Empty))
            {
                return separator;
            }
            else
            {
                return str.Substring(0, str.IndexOf(separator, StringComparison.Ordinal));
            }
        }

        /// <summary>
        /// Gets the substring after the first occurrence of a separator. The separator is not returned. 
        /// </summary>
        /// <param name="str">string</param>
        /// <param name="separator">string</param> 
        /// <returns>the substring after the first occurrence of the separator, null if null string input.</returns>
        public static string SubstringAfter(string str, string separator)
        {
            if (str == null)
            {
                return null;
            }
            else if (separator == null || !str.Contains(separator))
            {
                return string.Empty;
            }
            else if (separator.Equals(string.Empty))
            {
                return str;
            }
            else
            {
                return str.Substring(str.IndexOf(separator, StringComparison.Ordinal) + 1);
            }
        }
    }
}