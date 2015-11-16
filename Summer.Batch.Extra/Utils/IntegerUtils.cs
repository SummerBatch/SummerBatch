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
    /// Int helper
    /// </summary>
    public static class IntegerUtils
    {
        /// <summary>
        /// ToString method.
        /// </summary>
        /// <param name="int1">int?</param>  
        /// <returns>string representation of argument. Empty string if null.</returns> 
        public static string ToString(int? int1)
        {
            return int1==null ? "" : int1.Value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Compare two ints.
        /// </summary>
        /// <param name="int1">int?</param>  
        /// <param name="int2">int?</param>  
        /// <returns>the value 0 if int2 is numerically equal int1; a value less than 0 if this int1 is numerically less than int2; and a value greater than 0 if int1 is numerically greater than int2. Null in case of null argument(s).</returns> 
        public static int? CompareTo(int? int1, int? int2)
        {
            return int1 == null || int2 == null ? default(int?) : ((int)int1).CompareTo((int)int2);
        }

        /// <summary>
        /// Check if the first given int is greater than the second.
        /// </summary>
        /// <param name="int1">int?</param>
        /// <param name="int2">int?</param>
        /// <returns>true if int1 is numerically greater than int2. Otherwise false.</returns> 
        public static bool? IsGreaterThan(int? int1, int? int2)
        {
            return int1 == null || int2 == null ? default(bool?) : ((int)int1).CompareTo((int)int2) > 0;
        }

        /// <summary>
        /// Check if the first given int is lower than the second.
        /// </summary>
        /// <param name="int1">int?</param>
        /// <param name="int2">int?</param>
        /// <returns>true if int1 is numerically lower than int2. Otherwise false.</returns>
        public static bool? IsLowerThan(int? int1, int? int2)
        {
            return int1 == null || int2 == null ? default(bool?) : ((int)int1).CompareTo((int)int2) < 0;
        }

        /// <summary>
        /// Compute the absolute value of the given int.
        /// </summary>
        /// <param name="int1">int?</param>
        /// <returns>the absolute value of parameter.</returns> 
        public static decimal? Abs(int? int1)
        {
            return int1 == null ? default(decimal?) : Math.Abs((decimal)int1);
        }

        /// <summary>
        /// Add two ints.
        /// </summary>
        /// <param name="int1">int?</param>
        /// <param name="int2">int?</param>
        /// <returns>the sum of the two arguments, handling null (ex : 3 + null = 3).</returns> 
        public static int? Add(int? int1, int? int2)
        {
            return int1 == null ? int2 : int2 == null ? int1 : int1 + int2;
        }
    
        /// <summary>
        /// Add three ints.
        /// </summary>
        /// <param name="int1">int?</param>
        /// <param name="int2">int?</param>
        /// <param name="int3">int?</param>
        /// <returns>the sum of the three arguments, handling null (ex : 3 + 2 + null = 5).</returns> 
        public static int? Add(int? int1, int? int2, int? int3)
        {
            return int3 == null ? Add(int1, int2) : Add(int1, int2) + int3;
        }

        /// <summary>
        /// Add four ints.
        /// </summary>
        /// <param name="int1">int?</param>
        /// <param name="int2">int?</param>
        /// <param name="int3">int?</param>
        /// <param name="int4">int?</param>
        /// <returns>the sum of the four arguments, handling null (ex : 3 + 2 + 6 + null = 11).</returns> 
        public static int? Add(int? int1, int? int2, int? int3, int? int4)
        {
            return int4 == null ? Add(int1, int2, int3) : Add(int1, int2, int3) + int4;
        }

        /// <summary>
        /// Add five ints.
        /// </summary>
        /// <param name="int1">int?</param>
        /// <param name="int2">int?</param>
        /// <param name="int3">int?</param>
        /// <param name="int4">int?</param>
        /// <param name="int5">int?</param>
        /// <returns>the sum of the five arguments, handling null (ex : 3 + 2 + 6 + 1 + null = 12).</returns> 
        public static int? Add(int? int1, int? int2, int? int3, int? int4, int? int5)
        {
            return int5 == null ? Add(int1, int2, int3, int4) : Add(int1, int2, int3, int4) + int5;
        }

        /// <summary>
        /// Substract two ints.
        /// </summary>
        /// <param name="int1">int?</param>
        /// <param name="int2">int?</param>
        /// <returns>the difference between int1 and int2. Null in case of null argument(s).</returns> 
        public static int? Substract(int? int1, int? int2)
        {
            return int1 == null || int2 == null ? null : int1 - int2;
        }

        /// <summary>
        /// Multiply two ints.
        /// </summary>
        /// <param name="int1">int?</param>
        /// <param name="int2">int?</param>
        /// <returns>the product between int1 and int2. Null in case of null argument.</returns> 
        public static int? Multiply(int? int1, int? int2)
        {
            return int1 == null || int2 == null ? null : int1 * int2;
        }

        /// <summary>
        /// Divide two ints.
        /// </summary>
        /// <param name="int1">int?</param>
        /// <param name="int2">int?</param>
        /// <returns>the division between int1 and int2. Null in case of null argument.</returns> 
        public static int? Divide(int? int1, int? int2)
        {
            return int1 == null || int2 == null ? null : int1 / int2;
        }

        /// <summary>
        /// Check if the given int is zero or null.
        /// </summary>
        /// <param name="int1">int?</param>
        /// <returns>true if argument is null or argument == 0.</returns> 
        public static bool IsNullOrZeroValue(int? int1)
        {
            return int1 == null || int1 == 0;
        }	
    }
}
