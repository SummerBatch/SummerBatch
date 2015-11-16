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
using System.Numerics;
using System.Text;
using NLog;
using Summer.Batch.Extra.Copybook;
using Summer.Batch.Extra.Ebcdic.Exception;

namespace Summer.Batch.Extra.Ebcdic.Encode
{
    /// <summary>
    /// Decodes EBCDIC values to their corresponding C# objects.
    /// </summary>
    public class EbcdicDecoder
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Encoding being used for bytes to string operations
        /// </summary>
        private readonly Encoding _encoding;

        #region constructors
        /// <summary>
        /// Custom constructor given encoding
        /// </summary>
        /// <param name="encoding"></param>
        public EbcdicDecoder(Encoding encoding)
        {
            _encoding = encoding;
        }

        /// <summary>
        /// Custom constructor given encoding name
        /// </summary>
        /// <param name="encodingName"></param>
        public EbcdicDecoder(string encodingName)
        {
            _encoding = Encoding.GetEncoding(encodingName);
        }
        #endregion

        #region private static methods
        /// <summary>
        /// Scale result using the given fieldFormat
        /// </summary>
        /// <param name="fieldFormat"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static decimal ScaleResult(FieldFormat fieldFormat, decimal result)
        {
            decimal returnedResult = result;
            if (fieldFormat.Decimal > 0 && fieldFormat.ImpliedDecimal)
            {
                returnedResult = returnedResult * (decimal)Math.Pow(10, -fieldFormat.Decimal);
            }
            return returnedResult;
        }
        #endregion

        #region public static methods
        /// <summary>
        /// Parses a Zoned number
        /// </summary>
        /// <param name="value">a string where a number has been zone encoded</param>
        /// <returns>the encoded number</returns>
        public static decimal ParseZoned(string value)
        {
            char lastChar = value[value.Length - 1];
            bool positive;          

            string lastCharCandidate = GetLastIndex(lastChar, out positive);
            string last = lastCharCandidate != string.Empty ? lastCharCandidate : lastChar.ToString();

            decimal result = Decimal.Parse(value.Substring(0, value.Length - 1) + last,
                System.Globalization.CultureInfo.InvariantCulture);

            return positive ? result : result * -1m;
        }

        private static string  GetLastIndex(char lastChar, out bool positive)
        {
            positive = true;
            int index = Array.IndexOf(EbcdicConstants.ZonedPositives, lastChar);
            if (index > -1)
            {
                return index.ToString();
            }
            index = Array.IndexOf(EbcdicConstants.ZonedNegatives, lastChar);
            if (index > -1)
            {
                positive = false;
                return index.ToString();
            }
            index = Array.IndexOf(EbcdicConstants.MfMinus, lastChar);
            if (index > -1)
            {
                positive = false;
                return index.ToString();
            }
            index = Array.IndexOf(EbcdicConstants.CaMinus, lastChar);
            if (index > -1)
            {
                positive = false;
                return index.ToString();
            }
            
            return string.Empty;

        }

        /// <summary>
        ///  Parses a packed number
        /// </summary>
        /// <param name="bytes">a byte array where a number has been pack encoded</param>
        /// <returns>the encoded number</returns>
        public static decimal ParsePacked(byte[] bytes)
        {
            return ParsePacked(bytes, 0, bytes.Length);
        }

        /// <summary>
        ///  Parses a packed number
        /// </summary>
        /// <param name="bytes">a byte array where a number has been pack encoded</param>
        /// <param name="start">the beginning index, inclusive</param>
        /// <param name="end">the end index, exclusive</param>
        /// <returns>the encoded number</returns>
        public static decimal ParsePacked(byte[] bytes, int start, int end)
        {

            int nibble;
            long tmp = 0;
            byte b = 0;

            for (int i = start; i < end; i++)
            {
                b = i < bytes.Length ? bytes[i] : (byte)0;
                nibble = b & 0xF0;
                nibble = nibble >> 4;
                tmp = tmp * 10 + nibble;

                if (i < end - 1)
                {
                    nibble = b & 0x0F;
                    tmp = tmp * 10 + nibble;
                }
            }
            decimal result = new decimal(tmp);
            nibble = b & 0x0F;
            if (nibble == 0x0D)
            {
                result = result * -1m;
            }
            else if (nibble != 0x0F && nibble != 0x0C)
            {
                Logger.Warn("Unexpected sign nibble, assuming positive number.");
            }
            return result;
        }
        #endregion

        /// <summary>
        /// Main decode method. Switch based on the given FieldFormat
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="fieldFormat"></param>
        /// <returns></returns>
        /// <exception cref="UnexpectedFieldTypeException"></exception>
        public object Decode(byte[] bytes, FieldFormat fieldFormat)
        {
            char type = fieldFormat.Type[0];
            object result;
            switch (type)
            {
                case '9':
                    result = DecodeZoned(bytes, fieldFormat);
                    break;
                case '3':
                    result = DecodePacked(bytes, fieldFormat);
                    break;
                case 'B':
                    Array.Reverse(bytes); // needed as C# does not use same endianness as java on biginteger construction
                    result = ((decimal)new BigInteger(bytes) * (decimal)Math.Pow(10, -fieldFormat.Decimal));
                    break;
                case 'T':
                    result = bytes;
                    break;
                case 'H':
                    result = DecodeHex(bytes);
                    break;
                case 'X':
                    result = _encoding.GetString(bytes);
                    break;
                default:
                    throw new UnexpectedFieldTypeException(type);
            }
            return result;
        }

        #region private methods
        /// <summary>
        /// Decode a byte array using the zoned format
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="fieldFormat"></param>
        /// <returns></returns>
        private decimal DecodeZoned(byte[] bytes, FieldFormat fieldFormat)
        {
            var toParse = (_encoding.GetString(bytes)).Trim();
            if (toParse == string.Empty)
            {
                return decimal.Zero;
            }
            decimal result = ParseZoned(toParse);
            result = ScaleResult(fieldFormat, result);
            return result;
        }


        /// <summary>
        /// Decode a byte array using the packed format
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="fieldFormat"></param>
        /// <returns></returns>
        private static decimal DecodePacked(byte[] bytes, FieldFormat fieldFormat)
        {
            decimal result = ParsePacked(bytes);
            result = ScaleResult(fieldFormat, result);
            return result;
        }

        /// <summary>
        /// Decode byte array as hexadecimal
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private string DecodeHex(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                int nibble = b & 0xF0;
                nibble = nibble >> 4;
                sb.Append(EbcdicConstants.HexTable[nibble]);
                nibble = b & 0x0F;
                sb.Append(EbcdicConstants.HexTable[nibble]);
            }
            return sb.ToString();
        }
        #endregion
    }
}