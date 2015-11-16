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
using System;
using System.Globalization;
using System.Numerics;
using System.Text;
using Summer.Batch.Extra.Copybook;
using Summer.Batch.Extra.Ebcdic.Exception;

namespace Summer.Batch.Extra.Ebcdic.Encode
{
    /// <summary>
    /// Encode C# objects to EBCDIC
    /// </summary>
    public class EbcdicEncoder
    {

        private const string ErrorMessage = "Mismatch type for field {0} - Expecting: {1}, Actual: {2}";
        private DefaultValue _defVal = DefaultValue.LowValue;

        /// <summary>
        /// DefaultValue property;
        /// </summary>
        public DefaultValue DefVal
        {
            set
            {
                _defVal = value;
                _defaultValue = GetByte(_defVal,_encoding);
            }
        }

        #region DefaultValue enumeration related

        /// <summary>
        /// Enumeration that represents what byte should be used as default value when
        /// the encoded value is padded to match the size of the field.
        /// </summary>
        public enum DefaultValue
        {
            /// <summary>
            /// Indicates that low value (0x00) should be used as default value.
            /// </summary>
            LowValue,
            /// <summary>
            /// Indicates that the character zero ('0') should be used as default value.
            /// </summary>
            Zero,
            /// <summary>
            /// Indicates that the space character (' ') should be used as default value.
            /// </summary>
            Space
        }

        /// <summary>
        /// return byte given default value and using given encoding
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        private static byte GetByte(DefaultValue defaultValue, Encoding encoding)
        {
            switch (defaultValue)
            {
                case DefaultValue.Space:
                    return encoding.GetBytes(" ")[0];
                case DefaultValue.Zero:
                    return encoding.GetBytes("0")[0];
                default: //including DefaultValue.LowValue:
                    return 0;
            }
        }
        #endregion

        private readonly Encoding _encoding;
        private byte _defaultValue;
        

        #region constructors
        /// <summary>
        /// Custom constructor using encoding name
        /// </summary>
        /// <param name="encodingName"></param>
        public EbcdicEncoder(string encodingName)
        {
            _encoding = Encoding.GetEncoding(encodingName);
            _defaultValue = GetByte(_defVal, _encoding);
        }

        /// <summary>
        /// Custom constructor using encoding
        /// </summary>
        /// <param name="encoding"></param>
        public EbcdicEncoder(Encoding encoding)
        {
            _encoding = encoding;
            _defaultValue = GetByte(_defVal, _encoding);
        }
        #endregion

        #region private static utility methods

        /// <summary>
        /// Return a significant byte array of unscaled decimal value
        /// </summary>
        /// <param name="dec"></param>
        /// <returns></returns>
        private static byte[] GetUnscaledValueAsByteArray(decimal dec)
        {
            byte[] reversedResult = new BigInteger(GetUnscaledValue(dec)).ToByteArray();
            //reverse endianness (% java) on biginteger
            Array.Reverse(reversedResult);
            return reversedResult;
        }

        /// <summary>
        /// return unscaled value from decimal
        /// </summary>
        /// <param name="dec"></param>
        /// <returns></returns>
        private static decimal GetUnscaledValue(decimal dec)
        {
            int[] parts = Decimal.GetBits(dec);
            //get sign
            bool sign = (parts[3] & 0x80000000) != 0;
            //unscale (set scale to 0)
            return new decimal(parts[0], parts[1], parts[2], sign, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dec"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        private static decimal SetScale(decimal dec, int scale)
        {
            return Math.Round(dec, scale, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// return the number of decimals of a given decimal. Trailing zeros are to be 
        /// kept into account. 
        /// </summary>
        /// <param name="dec"></param>
        /// <returns></returns>
        private static int GetScale(decimal dec)
        {
            string s = dec.ToString(CultureInfo.InvariantCulture);
            int dotPos = s.IndexOf(".", StringComparison.Ordinal);
            return dotPos > 0 ? s.Length - dotPos - 1 : 0;
        }

        /// <summary>
        /// get next digit from a string representing a number
        /// </summary>
        /// <param name="number"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static byte GetNextDigit(string number, ref int index)
        {
            index--;
            if (index >= 0)
            {
                while (!Char.IsDigit(number[index]))
                {
                    index--;
                    if (index < 0)
                    {
                        return 0;
                    }
                }
                return (byte)(Char.GetNumericValue(number[index]));
            }
            else
            {
                return 0;
            }
        }
        #endregion

        /// <summary>
        /// Encodes a C# object to a byte array using EBCDIC format.
        /// Main method.
        /// </summary>
        /// <param name="field">the object to encode</param>
        /// <param name="fieldFormat">the format of the field</param>
        /// <returns>an array of byte with the encoded field value</returns>
        /// <exception cref="ValueTypeMismatchException">if the object to encode is not of the expected type</exception>
        /// <exception cref="UnexpectedFieldTypeException">if the type field is unknown or unsupported</exception>
        public byte[] Encode(object field, FieldFormat fieldFormat)
        {
            byte[] result;

            if (field == null)
            {
                result = new byte[fieldFormat.ByteSize];
                for (int i = 0; i < fieldFormat.ByteSize; i++)
                {
                    result[i] = _defaultValue;
                }
                return result;
            }

            char type = fieldFormat.Type[0];
            switch (type)
            {
                case '9':
                    result = EncodeZoned(field, fieldFormat);
                    break;
                case '3':
                    result = EncodePacked(field, fieldFormat);
                    break;
                case 'B':
                    result = EncodeBinary(field, fieldFormat);
                    break;
                case 'T':
                    result = EncodeTransparent(field, fieldFormat);
                    break;
                case 'X':
                    result = EncodeText(field, fieldFormat);
                    break;
                default:
                    throw new UnexpectedFieldTypeException(type);
            }

            return result;
        }

        #region public static methods
        /// <summary>
        ///  Encodes a decimal as a packed number, using defaults.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static byte[] EncodePacked(decimal value, int size)
        {
            return EncodePacked(value, size, GetScale(value), true);
        }

        /// <summary>
        /// Encodes a decimal as a packed number.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="size"></param>
        /// <param name="nbDecimals"></param>
        /// <param name="signed"></param>
        /// <returns></returns>
        public static byte[] EncodePacked(decimal value, int size, int nbDecimals, bool signed)
        {
            // adjust the value to the expected number of decimal
            decimal internalValue = value;
            if (nbDecimals != GetScale(value))
            {
                // truncate any additional digit
                internalValue = Math.Round(value, nbDecimals);
            }
            var str = internalValue.ToString(CultureInfo.InvariantCulture);
            var bytes = new byte[size];
            var index = str.Length;
            byte odd;
            // for last digit, take care of the sign
            var even = GetNextDigit(str, ref index);
            if (signed)
            {
                odd = (byte)(internalValue >= 0 ? 0x0C : 0x0D);
            }
            else
            {
                odd = 0x0F;
            }
            bytes[size - 1] = (byte)((even << 4) | odd);
            // Pack the rest of the digits
            for (var i = size - 2; i >= 0; i--)
            {
                // Get even digit if exist or zero
                odd = GetNextDigit(str, ref index);
                even = GetNextDigit(str, ref index);
                bytes[i] = (byte)((even << 4) | odd);
            }
            return bytes;
        }


        /// <summary>
        /// Encode decimal as zoned number, using defaults.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string EncodeZoned(decimal value, int size)
        {
            return EncodeZoned(value, size, GetScale(value), true, true);
        }

        /// <summary>
        /// Encode decimal as zoned number. 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="size"></param>
        /// <param name="nbDecimals"></param>
        /// <param name="signed"></param>
        /// <param name="impliedDecimal"></param>
        /// <returns></returns>
        public static string EncodeZoned(decimal value, int size, int nbDecimals, bool signed, bool impliedDecimal)
        {
            decimal internalValue = value;
            if (nbDecimals != GetScale(value))
            {
                internalValue = SetScale(value, nbDecimals);
            }

            // build result by taking absolute value
            // and deleting decimal point if necessary
            string format = "{0:F" + nbDecimals + "}";
            StringBuilder sb = new StringBuilder(string.Format(CultureInfo.InvariantCulture, format, Math.Abs(internalValue)));
            if (impliedDecimal)
            {
                var dotPos = sb.ToString().LastIndexOf(".", StringComparison.Ordinal);
                if (dotPos > 0) // dot may not be present in some cases; this is a normal behaviour
                {
                    sb.Remove(dotPos, 1);
                }
            }
            
            // Adding the sign information at the end
            // if field format is signed
            if (signed)
            {
                int pos = int.Parse(sb[sb.Length - 1].ToString());
                var lastChar = internalValue >= 0 ? EbcdicConstants.ZonedPositives[pos] : EbcdicConstants.ZonedNegatives[pos];
                sb[sb.Length - 1] = lastChar;
            }

            // Padding left with 0
            int padding = size - sb.Length;
            for (int i = 0; i < padding; i++)
            {
                sb.Insert(0, '0');
            }

            return sb.ToString(Math.Max(0, sb.Length - size), size);
        }
        #endregion

        #region private methods
        /// <summary>
        /// Encode object as zoned number.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="fieldFormat"></param>
        /// <returns></returns>
        /// <exception cref="ValueTypeMismatchException"></exception>
        private byte[] EncodeZoned(object field, FieldFormat fieldFormat)
        {
            if (field is decimal)
            {
                return _encoding.GetBytes(EncodeZoned((decimal)field, fieldFormat.ByteSize, fieldFormat.Decimal, fieldFormat.Signed,
                        fieldFormat.ImpliedDecimal));
            }
            else
            {
                throw GetValueTypeMismatchException(field, fieldFormat, "decimal");
            }
        }

        /// <summary>
        /// Encode objet as packed number
        /// </summary>
        /// <param name="field"></param>
        /// <param name="fieldFormat"></param>
        /// <returns></returns>
        /// <exception cref="ValueTypeMismatchException"></exception>
        private byte[] EncodePacked(object field, FieldFormat fieldFormat)
        {
            if (field is decimal)
            {
                return EncodePacked((decimal)field, fieldFormat.ByteSize, fieldFormat.Decimal, fieldFormat.Signed);
            }
            else
            {
                throw GetValueTypeMismatchException(field, fieldFormat, "decimal");
            }
        }




        /// <summary>
        /// Encode decimal to binary. Only accept decimal or throw ValueTypeMismatchException.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="fieldFormat"></param>
        /// <returns></returns>
        /// <exception cref="ValueTypeMismatchException"></exception>
        private byte[] EncodeBinary(object field, FieldFormat fieldFormat)
        {
            if (!(field is decimal))
            {
                throw GetValueTypeMismatchException(field, fieldFormat, "decimal");
            }
            decimal value = (decimal)field;
            if (fieldFormat.Decimal != GetScale(value))
            {
                value = Math.Round(value, fieldFormat.Decimal);
            }

            byte[] bytes = GetUnscaledValueAsByteArray(value);

            if (bytes.Length == fieldFormat.ByteSize)
            {
                return bytes;
            }
            byte[] result = new byte[fieldFormat.ByteSize];
            if (value < 0)
            {
                for (var i = 0; i < result.Length - bytes.Length; i++)
                {
                    result[i] = 255;
                }
            }

            Buffer.BlockCopy(bytes, 0, result, result.Length - bytes.Length, bytes.Length);
            return result;
        }


        /// <summary>
        /// Encode given object in a transparent way. Only applies to existing byte array having correct length.
        /// All other situations will lead to throwing a ValueTypeMismatchException.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="fieldFormat"></param>
        /// <returns></returns>
        /// <exception cref="ValueTypeMismatchException"></exception>
        private byte[] EncodeTransparent(object field, FieldFormat fieldFormat)
        {
            var bytes = field as byte[];
            if (bytes != null)
            {
                if (bytes.Length == fieldFormat.ByteSize)
                {
                    return bytes;
                }
                else
                {
                    throw new ValueTypeMismatchException("Value size must be equal to field length.");
                }
            }
            else
            {
                throw GetValueTypeMismatchException(field, fieldFormat, "byte[]");
            }
        }

        /// <summary>
        /// Encode object as text, using current encoding
        /// </summary>
        /// <param name="field"></param>
        /// <param name="fieldFormat"></param>
        /// <returns></returns>
        /// <exception cref="ValueTypeMismatchException"></exception>
        private byte[] EncodeText(object field, FieldFormat fieldFormat)
        {
            var s = field as string;
            if (s != null)
            {
                string value = s;
                int size = fieldFormat.ByteSize;
                if (value.Length > size)
                {
                    return _encoding.GetBytes(value.Substring(0, size));
                }
                else if (value.Length < size)
                {
                    StringBuilder sb = new StringBuilder(size);
                    sb.Append(value);
                    for (int i = 0; i < size - value.Length; i++)
                    {
                        sb.Append(' ');
                    }
                    return _encoding.GetBytes(sb.ToString());
                }
                else
                {
                    return _encoding.GetBytes(value);
                }
            }
            else
            {
                throw GetValueTypeMismatchException(field, fieldFormat, "string");
            }
        }

        /// <summary>
        /// Construct a dedicated ValueTypeMismatchException based on given inputs
        /// </summary>
        /// <param name="field"></param>
        /// <param name="fieldFormat"></param>
        /// <param name="expectedType"></param>
        /// <returns>a dedicated ValueTypeMismatchException</returns>
        private ValueTypeMismatchException GetValueTypeMismatchException(object field, FieldFormat fieldFormat, string expectedType)
        {
            return new ValueTypeMismatchException(string.Format(ErrorMessage,
                fieldFormat.Name,
                expectedType,
                field.GetType().Name));
        }

        #endregion
    }
}