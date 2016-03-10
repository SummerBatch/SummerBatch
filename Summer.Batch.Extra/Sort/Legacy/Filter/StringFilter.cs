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
using System.Text;
using Summer.Batch.Extra.Sort.Legacy.Accessor;

namespace Summer.Batch.Extra.Sort.Legacy.Filter
{
    /// <summary>
    /// Implementation of <see cref="AbstractLegacyFilter{T}"/> for comparing strings.
    /// </summary>
    public class StringFilter : AbstractLegacyFilter<string>
    {
        /// <summary>
        /// The encoding to use for sorting. Used for legacy orders (like EBCDIC).
        /// Default is <c>null</c>.
        /// </summary>
        public Encoding SortEncoding { get; set; }

        /// <summary>
        /// Does the actual comparison between the values
        /// </summary>
        /// <returns>the result of the comparison, as an integer</returns>
        /// <param name="leftValue">the left value of the comparison</param>
        /// /// <param name="rightValue">the right value of the comparison</param>
        protected override int DoComparison(string leftValue, string rightValue)
        {
            // In a field-to-field comparison, the shorter field is padded
            // appropriately. In a field-to-constant comparison, the constant is
            // padded or truncated to the length of the field.
            if (Left is ConstantAccessor<string> ||
                (!(Right is ConstantAccessor<string>) && leftValue.Length < rightValue.Length))
            {
                // either left is a constant, or it's a field-to-field comparison and left is shorter
                AdaptSize(ref leftValue, rightValue.Length);
            }
            else
            {
                // either right is a constant, or it's a field-to-field comparison and right is shorter
                AdaptSize(ref rightValue, leftValue.Length);
            }

            if (SortEncoding == null)
            {
                return string.Compare(leftValue, rightValue, StringComparison.Ordinal);
            }
            return SortEncoding.GetBytes(leftValue).CompareTo(SortEncoding.GetBytes(rightValue));
        }

        /// <summary>
        /// Adapts the size of a string. If it is too long, it is truncated on its right.
        /// If it is too short, whitespaces are added on its right.
        /// </summary>
        /// <param name="s">the string to adapt</param>
        /// <param name="size">the expected size of the string</param>
        private static void AdaptSize(ref string s, int size)
        {
            if (s.Length > size)
            {
                s = s.Substring(0, size);
            }
            else if (s.Length < size)
            {
                var builder = new StringBuilder(s);
                for (var i = 0; i < size - s.Length; i++)
                {
                    builder.Append(' ');
                }
                s = builder.ToString();
            }
        }
    }
}