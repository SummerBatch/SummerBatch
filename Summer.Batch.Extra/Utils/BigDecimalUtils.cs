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
    /// Big Decimal Helper.
    /// </summary>
    public static class BigDecimalUtils
    {
        /// <summary>
        /// Create a BigDecimal.
        /// </summary>
        /// <returns>BigDecimal</returns> 
        public static decimal? Create() 
        {
           return decimal.Zero;
        }

        /// <summary>
        /// Create a BigDecimal from the given string.
        /// </summary>
        /// <param name="val">string</param>  
        /// <returns>BigDecimal</returns> 
        public static decimal? Create(string val) 
        {
            return decimal.Parse(val);
        }

        /// <summary>
        /// Create a BigDecimal from the given double.
        /// </summary>
        /// <param name="val">double</param>  
        /// <returns>BigDecimal</returns> 
        public static decimal? Create(double val) 
        {
            return Convert.ToDecimal(val);
        }

        /// <summary>
        /// Create a BigDecimal from the given int.
        /// </summary>
        /// <param name="val">int</param>  
        /// <returns>BigDecimal</returns> 
        public static decimal? Create(int val) 
        {
            return val;
        }

        /// <summary>
        /// Create a BigDecimal from the given long.
        /// </summary>
        /// <param name="val">long</param>  
        /// <returns>BigDecimal</returns>
        public static decimal? Create(long val) 
        {
            return val;
        }

        /// <summary>
        /// Create a BigDecimal from the given decimal.
        /// </summary>
        /// <param name="val">BigInteger</param>  
        /// <returns>BigDecimal</returns> 
        public static decimal? Create(decimal val) 
        {
        return val;
        }

        /// <summary>
        /// ToString method.
        /// </summary>
        /// <param name="decimal1">BigDecimal</param>  
        /// <returns>string representation of argument. Empty string if null.</returns> 
        public static string ToString(decimal? decimal1)
        {
        return decimal1 == null ? "" : decimal1.Value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Compare two BigDecimals.
        /// </summary>
        /// <param name="decimal1">BigDecimal</param>  
        /// <param name="decimal2">BigDecimal</param>  
        /// <returns>the value 0 if decimal2 is numerically equal decimal1. a value less than 0 if this decimal1 is numerically less than decimal2; and a value greater than 0 if decimal1 is numerically greater than decimal2. Null in case of null argument(s).</returns> 
        public static int? CompareTo(decimal? decimal1, decimal? decimal2)
        {
            return decimal1 == null || decimal2 == null ? default(int?) : ((decimal)decimal1).CompareTo((decimal)decimal2);
        }

        /// <summary>
        /// Check if the first given BigDecimal is greater than the second.
        /// </summary>
        /// <param name="decimal1">BigDecimal</param>  
        /// <param name="decimal2">BigDecimal</param>  
        /// <returns>true if decimal1 is numerically greater than decimal2. Otherwise false.</returns> 
        public static bool? IsGreaterThan(decimal? decimal1, decimal? decimal2)
        {
            return decimal1 == null || decimal2 == null ? default(bool?) : ((decimal)decimal1).CompareTo((decimal)decimal2) > 0;
        }

        /// <summary>
        /// Check if the first given BigDecimal is lower than the second.
        /// </summary>
        /// <param name="decimal1">BigDecimal</param>  
        /// <param name="decimal2">BigDecimal</param>
        /// <returns>true if decimal1 is numerically lower than decimal2. Otherwise false.</returns> 
        public static bool? IsLowerThan(decimal? decimal1, decimal? decimal2)
        {
            return decimal1 == null || decimal2 == null ? default(bool?) : ((decimal)decimal1).CompareTo((decimal)decimal2) < 0;
        }

        /// <summary>
        /// Compute the absolute value of the given BigDecimal.
        /// </summary>
        /// <param name="decimal1">BigDecimal</param>  
        /// <returns>the absolute value of parameter.</returns> 
        public static decimal? Abs(decimal? decimal1)
        {
            return decimal1 == null ? default(decimal?) : Math.Abs((decimal)decimal1);
        }

        /// <summary>
        /// Add two BigDecimals.
        /// </summary>
        /// <param name="decimal1">BigDecimal</param>  
        /// <param name="decimal2">BigDecimal</param>  
        /// <returns>the sum of the two arguments, handling null (ex : 3 + null = 3).</returns> 
        public static decimal? Add(decimal? decimal1, decimal? decimal2)
        {
            return decimal1 == null ? decimal2 : decimal2 == default(decimal?) ? decimal1 : decimal1 + decimal2;
        }

        /// <summary>
        /// Subtract two BigDecimals.
        /// </summary>
        /// <param name="decimal1">BigDecimal</param>  
        /// <param name="decimal2">BigDecimal</param>  
        /// <returns>the difference between decimal1 and decimal2. Null in case of null argument(s).</returns> 
        public static decimal? Subtract(decimal? decimal1, decimal? decimal2)
        {
            return decimal1 == null || decimal2 == null ? default(decimal?) : decimal1 - decimal2;
        }

        /// <summary>
        /// Multiply two BigDecimals.
        /// </summary>
        /// <param name="decimal1">BigDecimal</param>  
        /// <param name="decimal2">BigDecimal</param>  
        /// <returns>the product between decimal1 and decimal2. Null in case of null argument.</returns> 
        public static decimal? Multiply(decimal? decimal1, decimal? decimal2)
        {
            return decimal1 == null || decimal2 == null ? default(decimal?) : decimal1 * decimal2;
        }

        /// <summary>
        /// Divide two BigDecimals with the given scale.
        /// </summary>
        /// <param name="decimal1">BigDecimal</param>  
        /// <param name="decimal2">BigDecimal</param>  
        /// <param name="scale">int</param>  
        /// <returns>the division between decimal1 and decimal2, with specified scale, rounded up for .5 or more, down otherwise. Null in case of null argument.</returns> 
        public static decimal? Divide(decimal? decimal1, decimal? decimal2, int scale)
        {
            return decimal1 == null || decimal2 == null ? default(decimal?) : Math.Round(decimal.Divide((decimal)decimal1, (decimal)decimal2),scale);
        }

        /// <summary>
        /// Divide two BigDecimals.
        /// </summary>
        /// <param name="decimal1">BigDecimal</param>  
        /// <param name="decimal2">BigDecimal</param>  
        /// <returns>the division between decimal1 and decimal2.</returns> 
        public static decimal? Divide(decimal? decimal1, decimal? decimal2)
        {
            return (decimal1 == null) || (decimal2 == null) ? default(decimal?) : decimal1 / decimal2;
        }

        /// <summary>
        /// Check if the given BigDecimal is zero or null.
        /// </summary>
        /// <param name="decimal1">BigDecimal</param>  
        /// <returns>true if argument is null or argument == 0.</returns> s
        public static bool IsNullOrZeroValue(decimal? decimal1)
        {
            return decimal1 == null || decimal.Zero.CompareTo(decimal1) == 0;
        }	
    }
}
