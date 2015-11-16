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
using System.Text;

namespace Summer.Batch.Extra.Sort.Legacy.Comparer
{
    /// <summary>
    /// Implementation of <see cref="AbstractBytesComparer"/> that compares byte arrays
    /// by extracting a string value.
    /// </summary>
    public class StringComparer : AbstractBytesComparer
    {
        /// <summary>
        /// The zero-based index of the first character of the string value.
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// The length (in bytes) of the string value.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// The encoding of the string value. Default is <see cref="System.Text.Encoding.Default"/>.
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// The encoding to use for the comparison. It is used when emulating comparison in legacy systems.
        /// Default is <code>null</code>.
        /// </summary>
        public Encoding SortEncoding { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public StringComparer()
        {
            Encoding = Encoding.Default;
        }

        /// <summary>
        /// Method that does the actual comparison.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>The comparison of <paramref name="x"/> and <paramref name="y"/></returns>
        protected override int DoCompare(byte[] x, byte[] y)
        {
            if (SortEncoding == null)
            {
                return x.CompareTo(y, Start, Length);
            }
            byte[] b1 = SortEncoding.GetBytes(Encoding.GetString(x, Start, Length));
            byte[] b2 = SortEncoding.GetBytes(Encoding.GetString(y, Start, Length));
            return b1.CompareTo(b2);
        }
    }
}