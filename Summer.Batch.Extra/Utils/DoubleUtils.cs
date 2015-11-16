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

namespace Summer.Batch.Extra.Utils
{
    /// <summary>
    /// Double helper.
    /// </summary>
    public static class DoubleUtils
    {

        private static readonly double DefaultEpsilon = Math.Pow(1,-9);

        /// <summary>
        /// ToString method.
        /// </summary>
        /// <param name="double1">double?</param>  
        /// <returns>string representation of double1. Empty string if null.</returns> 
        public static string ToString(double? double1)
        {
            return double1 == null ? "" : double1.Value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// ToString method, with specified format and culture.
        /// </summary>
        /// <param name="double1">double?</param>
        /// <param name="format">string</param>
        /// <param name="culture">string</param>
        /// <returns>string representation of double1 with the specified format and culture.</returns>
        public static string ToString(double? double1, string format, string culture)
        {
            return double1 == null ? "" : double1.Value.ToString(format, CultureInfo.GetCultureInfo(culture));
        }

        /// <summary>
        /// Compare two doubles.
        /// </summary>
        /// <param name="double1">double?</param>
        /// <param name="double2">double?</param>
        /// <returns>the value 0 if double2 is numerically equal double1; a value less than 0 if this double1 is numerically less than double2; and a value greater than 0 if double1 is numerically greater than double2. Null in case of null argument(s).</returns> 
        public static int? CompareTo(double? double1, double? double2)
        {
            return double1 == null || double2 == null ? default(int?) : ((double)double1).CompareTo((double)double2);
        }

        /// <summary>
        /// Check if the first given double is greater than the second.
        /// </summary>
        /// <param name="double1">double?</param>
        /// <param name="double2">double?</param>
        /// <returns>true if double1 is numerically greater than double2. Otherwise false.</returns>
        public static bool? IsGreaterThan(double? double1, double? double2)
        {
            return double1 == null || double2 == null ? default(bool?) : ((double)double1).CompareTo((double)double2) > 0;
        }

        /// <summary>
        /// Check if the first given double is lower than the second.
        /// </summary>
        /// <param name="double1">double?</param>
        /// <param name="double2">double?</param>
        /// <returns>true if double1 is numerically lower than double2. Otherwise false.</returns> 
        public static bool? IsLowerThan(double? double1, double? double2)
        {
            return double1 == null || double2 == null ? default(bool?) : ((double)double1).CompareTo((double)double2) < 0;
        }

        /// <summary>
        /// Compute the absolute value of the given double.
        /// </summary>
        /// <param name="double1">double?</param>
        /// <returns>the absolute value of parameter.</returns> 
        public static double? Abs(double? double1)
        {
            return double1 == null ? default(double?) : Math.Abs((double)double1);
        }

        /// <summary>
        /// Compute the floored value of the given double.
        /// </summary>
        /// <param name="double1">double?</param>
        /// <returns>the largest (closest to positive infinity) floating-point value that less than or equal to the argument and is equal to a mathematical integer.</returns> 
        public static double? Floor(double? double1)
        {
            return double1 == null ? default(double?) : Math.Floor((double)double1);
        }

        /// <summary>
        /// Compute the ceiled value of the given double.
        /// </summary>
        /// <param name="double1">double?</param>
        /// <returns>the smallest (closest to negative infinity) floating-point value that is greater than or equal to the argument and is equal to a mathematical integer.</returns> 
        public static double? Ceil(double? double1)
        {
            return double1 == null ? default(double?) : Math.Ceiling((double)double1);
        }

        /// <summary>
        /// Compute the rounded value of the given double.
        /// </summary>
        /// <param name="double1">double?</param>
        /// <returns>the value of the argument rounded to the nearest long value.</returns> 
        public static long? Round(double? double1)
        {
            return double1 == null ? default(long?) : (long?)Math.Round((double)double1);
        }

        /// <summary>
        /// Add two doubles.
        /// </summary>
        /// <param name="double1">double?</param>
        /// <param name="double2">double?</param>
        /// <returns>the sum of the two arguments, handling null (ex : 3 + null = 3).</returns> 
        public static double? Add(double? double1, double? double2)
        {
            return double1 == null ? double2 : double2 == null ? double1 : double1 + double2;
        }

        /// <summary>
        /// Add three doubles.
        /// </summary>
        /// <param name="double1">double?</param>
        /// <param name="double2">double?</param>
        /// <param name="double3">double?</param>
        /// <returns>the sum of the three arguments, handling null (ex : 3 + 2 + null = 5).</returns> 
        public static double? Add(double? double1, double? double2, double? double3)
        {
            return double3 == null ? Add(double1, double2) : Add(double1, double2) + double3;
        }

        /// <summary>
        /// Add four doubles.
        /// </summary>
        /// <param name="double1">double?</param>
        /// <param name="double2">double?</param>
        /// <param name="double3">double?</param>  
        /// <param name="double4">double?</param>
        /// <returns>the sum of the four arguments, handling null (ex : 3 + 2 + 6 + null = 11).</returns> 
        public static double? Add(double? double1, double? double2, double? double3, double? double4)
        {
            return double4 == null ? Add(double1, double2, double3) : Add(double1, double2, double3) + double4;
        }

        /// <summary>
        /// Add five doubles.
        /// </summary>
        /// <param name="double1">double?</param>
        /// <param name="double2">double?</param>
        /// <param name="double3">double?</param>
        /// <param name="double4">double?</param>
        /// <param name="double5">double?</param>
        /// <returns>the sum of the five arguments, handling null (ex : 3 + 2 + 6 + 1 + null = 12).</returns>        
        public static double? Add(double? double1, double? double2, double? double3, double? double4, double? double5)
        {
            return double5 == null ? Add(double1, double2, double3, double4) : Add(double1, double2, double3, double4) + double5;
        }

        /// <summary>
        /// Substract two doubles.
        /// </summary>
        /// <param name="double1">double?</param>
        /// <param name="double2">double?</param>
        /// <returns>the difference between double1 and double2. Null in case of null argument(s).</returns> 
        public static double? Substract(double? double1, double? double2)
        {
            return double1 == null || double2 == null ? null : double1 - double2;
        }

        /// <summary>
        /// Multiply two doubles.
        /// </summary>
        /// <param name="double1">double?</param>
        /// <param name="double2">double?</param>
        /// <returns>the product between double1 and double2. Null in case of null argument.</returns> 
        public static double? Multiply(double? double1, double? double2)
        {
            return double1 == null || double2 == null ? null : double1 * double2;
        }

        /// <summary>
        /// Divide two doubles.
        /// </summary>
        /// <param name="double1">double?</param>
        /// <param name="double2">double?</param>
        /// <returns>the division between double1 and double2. Null in case of null argument.</returns> 
        public static double? Divide(double? double1, double? double2)
        {
            return double1 == null || double2 == null ? null : double1 / double2;
        }

        /// <summary>
        /// Check if the given double is zero or null.
        /// </summary>
        /// <param name="double1">double?</param> 
        /// <returns>true if argument is null or argument == 0.</returns>
        public static bool IsNullOrZeroValue(double? double1)
        {
            return double1 == null || DoubleEquals(double1,0d, DefaultEpsilon);
        }

        /// <summary>
        /// Check if the given double is zero or null.
        /// </summary>
        /// <param name="double1">double?</param>
        /// <param name="epsilon">tolerance</param>
        /// <returns>true if argument is null or argument == 0.</returns>
        public static bool IsNullOrZeroValue(double? double1, double epsilon)
        {
            return double1 == null || DoubleEquals(double1, 0d, epsilon);
        }

        /// <summary>
        /// Double equality, using a given epsilon tolerance.
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        private static bool DoubleEquals(double? value1, double? value2, double epsilon)
        {
            if (value1 == null && value2 == null)
            {
                return true;
            }
            if (value1 == null || value2 == null)
            {
                return false;
            }
            //Do actual comparison to epsilon
            return Math.Abs(value1.Value - value2.Value) < epsilon;
        }
    }
}
