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
using System.Globalization;

namespace Summer.Batch.Extra.Utils
{
    /// <summary>
    /// Long Helper.
    /// </summary>
    public static class LongUtils
    {
        /// <summary>
        /// ToString method.
        /// </summary>
        /// <param name="long1">long?</param>  
        /// <returns>String representation of argument. Empty string if null.</returns>
        public static string ToString(long? long1)
        {
            return long1 == null ? "" : long1.Value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Compare two longs.
        /// </summary>
        /// <param name="long1">long?</param>  
        /// <param name="long2">long?</param>  
        /// <returns>the value 0 if long2 is numerically equal long1; a value less than 0 if this long1 is numerically less than long2; and a value greater than 0 if long1 is numerically greater than long2. Null in case of null argument(s).</returns> 
        public static int? CompareTo(long? long1, long? long2)
        {
            return long1 == null || long2 == null ? default(int?) : ((long)long1).CompareTo((long)long2);
        }

        /// <summary>
        /// Check if the first given long is greater than the second.
        /// </summary>
        /// <param name="long1">long?</param>  
        /// <param name="long2">long?</param>  
        /// <returns>true if long1 is numerically greater than long2. Otherwise false.</returns> 
        public static bool? IsGreaterThan(long? long1, long? long2)
        {
            return long1 == null || long2 == null ? default(bool?) : ((long)long1).CompareTo((long)long2) > 0;
        }

        /// <summary>
        /// Check if the first given long is lower than the second.
        /// </summary>
        /// <param name="long1">long?</param>  
        /// <param name="long2">long?</param>  
        /// <returns>true if long1 is numerically lower than long2. Otherwise false.</returns>  
        public static bool? IsLowerThan(long? long1, long? long2)
        {
            return long1 == null || long2 == null ? default(bool?) : ((long)long1).CompareTo((long)long2) < 0;
        }

        /// <summary>
        /// Add two longs.
        /// </summary>
        /// <param name="long1">long?</param>  
        /// <param name="long2">long?</param>  
        /// <returns>the sum of the two arguments, handling null (ex : 3 + null = 3).</returns> 
        public static long? Add(long? long1, long? long2)
        {
            return long1 == null ? long2 : long2 == null ? long1 : long1 + long2;
        }

        /// <summary>
        /// Add three longs.
        /// </summary>
        /// <param name="long1">long?</param>  
        /// <param name="long2">long?</param>
        /// <param name="long3">long?</param>
        /// <returns>the sum of the three arguments, handling null (ex : 3 + 2 + null = 5).</returns> 
        public static long? Add(long? long1, long? long2, long? long3)
        {
            return long3 == null ? Add(long1, long2) : Add(long1, long2) + long3;
        }

        /// <summary>
        /// Add four longs.
        /// </summary>
        /// <param name="long1">long?</param>
        /// <param name="long2">long?</param>
        /// <param name="long3">long?</param>
        /// <param name="long4">long?</param>
        /// <returns>the sum of the four arguments, handling null (ex : 3 + 2 + null = 5).</returns> 
        public static long? Add(long? long1, long? long2, long? long3, long? long4)
        {
            return long4 == null ? Add(long1, long2, long3) : Add(long1, long2, long3) + long4;
        }

        /// <summary>
        /// Add five longs.
        /// </summary>
        /// <param name="long1">long?</param>
        /// <param name="long2">long?</param>
        /// <param name="long3">long?</param>
        /// <param name="long4">long?</param>
        /// <param name="long5">long?</param>
        /// <returns>the sum of the five arguments, handling null (ex : 3 + 2 + null = 5).</returns> 
        public static long? Add(long? long1, long? long2, long? long3, long? long4, long? long5)
        {
            return long5 == null ? Add(long1, long2, long3, long4) : Add(long1, long2, long3, long4) + long5;
        }

        /// <summary>
        /// Substract two longs.
        /// </summary>
        /// <param name="long1">long?</param>
        /// <param name="long2">long?</param>
        /// <returns>the difference between long1 and long2. Null in case of null argument(s).</returns> 
        public static long? Substract(long? long1, long? long2)
        {
            return long1 == null || long2 == null ? default(long?) : long1 - long2;
        }

        /// <summary>
        /// Multiply two longs.
        /// </summary>
        /// <param name="long1">long?</param>  
        /// <param name="long2">long?</param>  
        /// <returns>the product between long1 and long2. Null in case of null argument.</returns> 
        public static long? Multiply(long? long1, long? long2)
        {
            return long1 == null || long2 == null ? default(long?) : long1 * long2;
        }

        /// <summary>
        /// Divide two longs.
        /// </summary>
        /// <param name="long1">long?</param>
        /// <param name="long2">long?</param>
        /// <returns>the division between long1 and long2. Null in case of null argument.</returns> 
        public static long? Divide(long? long1, long? long2)
        {
            return long1 == null || long2 == null ? default(long?) : long1 / long2;
        }

        /// <summary>
        /// Check if the given long is zero or null.
        /// </summary>
        /// <param name="value">long?</param>
        /// <returns>true if argument is null or argument == 0.</returns> 
        public static bool IsNullOrZeroValue(long? value)
        {
            return value == null || value == 0;
        }	
    }
}
