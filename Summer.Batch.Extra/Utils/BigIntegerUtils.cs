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
using System.Numerics;
using System.Globalization;

namespace Summer.Batch.Extra.Utils
{
    /// <summary>
    /// Big Integer Helper.
    /// </summary>
    public static class BigIntegerUtils
    {
        /// <summary>
        /// ToString method.
        /// </summary>
        /// <param name="int1">BigInteger?</param> 
        /// <returns>string representation of argument. Empty string if null.</returns> 
        public static string ToString(BigInteger? int1)
        {
            return int1 == null ? "" : int1.Value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Check if the first given BigInteger is greater than the second.
        /// </summary>
        /// <param name="int1">BigInteger?</param> 
        /// <param name="int2">BigInteger?</param>
        /// <returns>true if int1 is numerically greater than int2. Otherwise false.</returns>
        public static bool? IsGreaterThan(BigInteger? int1, BigInteger? int2)
        {
            return int1 == null || int2 == null ? default(bool?) : ((BigInteger)int1).CompareTo((BigInteger)int2) > 0;
        }

        /// <summary>
        /// Check if the first given BigInteger is lower than the second.
        /// </summary>
        /// <param name="int1">BigInteger?</param> 
        /// <param name="int2">BigInteger?</param> 
        /// <returns>true if int1 is numerically lower than int2. Otherwise false.</returns> 
        public static bool? IsLowerThan(BigInteger? int1, BigInteger? int2)
        {
            return int1 == null || int2 == null ? default(bool?) : ((BigInteger)int1).CompareTo((BigInteger)int2) < 0;
        }
    }
}
