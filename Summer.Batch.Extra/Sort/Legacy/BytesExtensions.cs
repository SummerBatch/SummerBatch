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

namespace Summer.Batch.Extra.Sort.Legacy
{
    /// <summary>
    /// A utility class for manipulating bytes array
    /// </summary>
    public static class BytesExtensions
    {
        /// <summary>
        /// Compare two byte arrays
        /// </summary>
        /// <param name="bytes">the left byte array</param>
        /// <param name="other">the right byte array</param>
        /// <returns>the result of the comparison</returns>
        public static int CompareTo(this byte[] bytes, byte[] other)
        {
            return bytes.CompareTo(other, 0, bytes.Length);
        }

        /// <summary>
        /// Compare two byte arrays on a given range
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="other"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static int CompareTo(this byte[] bytes, byte[] other, int start, int length)
        {
            for (var i = start; i < start + length; i++)
            {
                if (bytes[i] != other[i])
                {
                    return bytes[i] - other[i];
                }
            }
            return 0;
        }

        /// <summary>
        /// Creates a sub array with the specified size.
        /// </summary>
        /// <param name="bytes">the original array</param>
        /// <param name="start">the index of the first byte to copy</param>
        /// <param name="length">the length to copy</param>
        /// <returns>a new sub array</returns>
        public static byte[] SubArray(this byte[] bytes, int start, int length)
        {
            var toCopy = bytes.Length - start;
            var result = new byte[length];
            if (toCopy > 0)
            {
                Buffer.BlockCopy(bytes, start, result, 0, Math.Min(length, toCopy));
            }
            return result;
        }
    }
}