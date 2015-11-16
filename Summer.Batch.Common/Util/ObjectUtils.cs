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
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
using System.Collections.Generic;
using System.Text;

namespace Summer.Batch.Common.Util
{
    /// <summary>
    /// Class containing utility methods on objects.
    /// </summary>
    public static class ObjectUtils
    {

        /// <summary>
        /// Returns a String representation of an object's overall identity.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string IdentityToString(object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            return obj.GetType().Name + "@" + GetIdentityHexString(obj);
        }


        /// <summary>
        /// Return a hex String form of an object's identity hash code.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetIdentityHexString(object obj)
        {
            return obj.GetHashCode().ToString("X");

        }

        /// <summary>
        /// Dumps an objects.
        /// Instances of <see cref="IEnumerable{T}"/> have their content dumped around brackets and separated by vertical bars.
        /// Others objects are dumped using <see cref="object.ToString"/>.
        /// Exemple:
        /// [Name_0|1|[[Rue de la victoire|Bordeaux|[87654320|01234567]]]|2|[email0test.com|altemail0test.com]]
        /// </summary>
        /// <param name="obj">the object to dump</param>
        /// <returns>a string containing the dump of obj</returns>
        public static string Dump(object obj)
        {
            var sb = new StringBuilder();
            DoDump(sb, (dynamic) obj);
            return sb.ToString();
        }

        /// <summary>
        /// Dumps an object in a string builder.
        /// </summary>
        /// <param name="sb">the string builder to write to</param>
        /// <param name="obj">the object to dump</param>
        private static void DoDump(StringBuilder sb, object obj)
        {
            sb.Append(obj);
        }

        /// <summary>
        /// Dumps an enumerable in a string builder.
        /// </summary>
        /// <typeparam name="T">the type of the elements in the enumerable</typeparam>
        /// <param name="sb">the string builder to write to</param>
        /// <param name="enumerable">the enumerable to dump</param>
        private static void DoDump<T>(StringBuilder sb, IEnumerable<T> enumerable)
        {
            sb.Append('[');
            var first = true;
            foreach (dynamic obj in enumerable)
            {
                if (!first)
                {
                    sb.Append('|');
                }
                first = false;
                DoDump(sb, obj);
            }
            sb.Append(']');
        }
    }
}
