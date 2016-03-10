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
using System.Text;
using Summer.Batch.Extra.Sort.Legacy.Accessor;

namespace Summer.Batch.Extra.Sort.Legacy.Format
{
    /// <summary>
    /// Implementation of <see cref="ISubFormatter"/> that allows formatting a number read from the input record into a string.
    /// </summary>
    public class NumericEditFormatter : ISubFormatter
    {
        private const char InsignificantDigit = 'I';
        private const char SignificantDigit = 'T';
        private const char Sign = 'S';

        /// <summary>
        /// The length of the formatted string.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// The zero-based index of the first byte to write in the output record.
        /// </summary>
        public int OutputIndex { get; set; }

        /// <summary>
        /// Accessor for the number to format.
        /// </summary>
        public IAccessor<decimal> Accessor { get; set; }

        /// <summary>
        /// The edit string that specifies the formatting of the output string.
        /// Uses the syntax of the EDIT command of DFSORT.
        /// </summary>
        public string Edit { get; set; }

        /// <summary>
        /// The character(s) for the positive sign.
        /// There should be exactly one or two characters. If there is one character, it is used every time
        /// <see cref="Edit"/> specifies a sign (<c>"S"</c>). If there is two characters, the first
        /// one is used before the number and the second one is used after (e.g., to produce <c>"(123)"</c>,
        /// use <c>"()"</c> as sign).
        /// Default is <c>" "</c>.
        /// </summary>
        public string PositiveSign { get; set; }

        /// <summary>
        /// The character(s) for the negative sign.
        /// There should be exactly one or two characters. If there is one character, it is used every time
        /// <see cref="Edit"/> specifies a sign (<c>"S"</c>). If there is two characters, the first
        /// one is used before the number and the second one is used after (e.g., to produce <c>"(123)"</c>,
        /// use <c>"()"</c> as sign).
        /// Default is <c>"-"</c>.
        /// </summary>
        public string NegativeSign { get; set; }

        /// <summary>
        /// The encoding to use to write the output string in the byte array.
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Formats a number read from the input record as a string and writes it in the output record.
        /// </summary>
        /// <param name="input">the input record</param>
        /// <param name="output">the output record</param>
        public void Format(byte[] input, byte[] output)
        {
            var value = Accessor.Get(input);
            var digitStack = new DigitStack(value);
            var positive = value >= 0;

            var sb = new StringBuilder();

            // characters are looped from last to first
            for (var i = Edit.Length - 1; i >= 0; i--)
            {
                var c = Edit[i];
                switch (c)
                {
                    case InsignificantDigit:
                    case SignificantDigit:
                        var d = digitStack.Pop();
                        if (c == SignificantDigit || d != 0)
                        {
                            if (d == 0)
                            {
                                d = '0';
                            }
                            sb.Insert(0, d);
                        }
                        break;
                    case Sign:
                        sb.Insert(0, GetSign(positive, sb.Length > 0));
                        break;
                    case 'C':
                    case 'R':
                    case 'D':
                    case 'B':
                        // "CR" (for credit) or "DB" (for debit) at the end is copied if
                        // the number is negative, or replaced whitespaces otherwise
                        sb.Insert(0, positive ? ' ' : c);
                        break;
                    default:
                        // Other characters are only printed if more digits will be printed
                        if (digitStack.HasMoreDigits || HasMoreSignificantDigits(i))
                        {
                            sb.Insert(0, c);
                        }
                        break;
                }
            }

            while (sb.Length < Length)
            {
                sb.Insert(0, ' ');
            }

            Buffer.BlockCopy(Encoding.GetBytes(sb.ToString()), 0, output, OutputIndex, Length);
        }

        /// <param name="index">the index of the current character in the <see cref="Edit"/> string.</param>
        /// <returns><c>true</c> if there are more significant digits to print (i.e., before the current position).</returns>
        private bool HasMoreSignificantDigits(int index)
        {
            for (var i = index; i >= 0; i--)
            {
                if (Edit[i] == SignificantDigit)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the sign to print.
        /// </summary>
        /// <param name="positive">whether the number is positive or not</param>
        /// <param name="prefix">whether the sign is a prefix or suffix</param>
        /// <returns>the correct sign.</returns>
        private string GetSign(bool positive, bool prefix)
        {
            var sign = positive ? PositiveSign : NegativeSign;
            if (sign.Length == 0)
            {
                return " ";
            }
            if (prefix || sign.Length == 1)
            {
                return sign.Substring(0, 1);
            }
            return sign.Substring(1);
        }

        /// <summary>
        /// A class to get the digits from a number, in reverse order.
        /// </summary>
        private class DigitStack
        {
            private readonly string _digits;
            private int _index;

            /// <summary>
            /// Constructs a new <see cref="DigitStack"/> from a decimal.
            /// </summary>
            /// <param name="value">the number to get the digits from</param>
            public DigitStack(decimal value)
            {
                _digits = decimal.Truncate(Math.Abs(value)).ToString(CultureInfo.InvariantCulture).Split('.')[0];
                _index = _digits.Length - 1;
            }

            /// <summary>
            /// Returns the current digit and move the cursor to the next digit.
            /// </summary>
            /// <returns>the current digit</returns>
            public char Pop()
            {
                if (HasMoreDigits)
                {
                    return _digits[_index--];
                }
                return (char)0;
            }

            /// <summary>
            /// Whether there are more digits in the stack.
            /// </summary>
            public bool HasMoreDigits { get { return _index >= 0; } }
        }
    }
}