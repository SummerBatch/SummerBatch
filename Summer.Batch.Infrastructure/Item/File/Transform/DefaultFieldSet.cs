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

// This file has been modified.
// Original copyright notice :

/*
 * Copyright 2006-2013 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Globalization;
using Summer.Batch.Common.Util;

namespace Summer.Batch.Infrastructure.Item.File.Transform
{
    /// <summary>
    /// Default implementation of <see cref="IFieldSet"/>.
    /// </summary>
    public class DefaultFieldSet : IFieldSet
    {
        private const string DefaultDatePattern = "yyyy-MM-dd";

        /// <summary>
        /// The names of the fields
        /// </summary>
        public string[] Names { get; private set; }

        /// <summary>
        /// The values of the fields
        /// </summary>
        public string[] Values { get; private set; }

        /// <summary>
        /// The number of fields in this field set
        /// </summary>
        public int Count { get { return Values.Length; } }

        /// <summary>
        /// Whether the field set has named fields
        /// </summary>
        public bool HasNames { get { return Names != null; } }

        /// <summary>
        /// The culture to use to parse numbers and dates.
        /// </summary>
        public CultureInfo Culture { get; set; }

        /// <summary>
        /// The date pattern for formatting.
        /// </summary>
        public string DatePattern { get; set; }

        /// <summary>
        /// Creates a <see cref="DefaultFieldSet"/> with the specified values.
        /// </summary>
        /// <param name="values">an array containing the values of the fields</param>
        public DefaultFieldSet(string[] values)
        {
            Values = (string[]) (values == null ? null : values.Clone());
            Culture = CultureInfo.GetCultureInfo("en-US");
            DatePattern = DefaultDatePattern;
        }

        /// <summary>
        /// Creates a <see cref="DefaultFieldSet"/> with the specified names and values.
        /// </summary>
        /// <param name="names">an array containing the names of the fields</param>
        /// <param name="values">an array containing the values of the fields</param>
        /// <exception cref="ArgumentException">if names and values do not have the same length</exception>
        public DefaultFieldSet(string[] names, string[] values) : this(values)
        {
            Assert.NotNull(names, "names must not be null");
            Assert.NotNull(values, "values must not be null");
            if (names.Length != values.Length)
            {
                throw new ArgumentException("names and values must have the same length");
            }
            Names = (string[]) names.Clone();
        }

        /// <summary>
        /// Reads a field as a string.
        /// </summary>
        /// <param name="index">the index of the field to read</param>
        /// <returns>the trimmed content of the field</returns>
        /// <exception cref="ArgumentOutOfRangeException">if the index does not correspond to a field</exception>
        public string ReadString(int index)
        {
            return ReadAndTrim(index);
        }

        /// <summary>
        /// Reads a field as a string.
        /// </summary>
        /// <param name="name">the name of the field to read</param>
        /// <returns>the trimmed content of the field</returns>
        /// <exception cref="ArgumentException">if no field has the given name</exception>
        public string ReadString(string name)
        {
            return ReadString(IndexOf(name));
        }

        /// <summary>
        /// Reads a field as a string.
        /// </summary>
        /// <param name="index">the index of the field to read</param>
        /// <returns>the raw content of the field</returns>
        /// <exception cref="ArgumentOutOfRangeException">if the index does not correspond to a field</exception>
        public string ReadRawString(int index)
        {
            if (index < 0 || index >= Values.Length)
            {
                throw new ArgumentOutOfRangeException(string.Format("Index ({0}) is out of range.", index));
            }
            return Values[index];
        }

        /// <summary>
        /// Reads a field as a string.
        /// </summary>
        /// <param name="name">the name of the field to read</param>
        /// <returns>the raw content of the field</returns>
        /// <exception cref="ArgumentException">if no field has the given name</exception>
        public string ReadRawString(string name)
        {
            return ReadRawString(IndexOf(name));
        }

        /// <summary>
        /// Reads a field as a boolean.
        /// </summary>
        /// <param name="index">the index of the field to read</param>
        /// <param name="trueValue">the literal corresponding to true (default is <code>"true"</code>)</param>
        /// <returns>true if the field contains trueValue, false otherwise</returns>
        /// <exception cref="ArgumentOutOfRangeException">if the index does not correspond to a field</exception>
        public bool ReadBoolean(int index, string trueValue = "true")
        {
            Assert.NotNull(trueValue, "trueValue must not be null");
            return ReadAndTrim(index) == trueValue;
        }

        /// <summary>
        /// Reads a field as a boolean.
        /// </summary>
        /// <param name="name">the name of the field to read</param>
        /// <param name="trueValue">the literal corresponding to true (default is <code>"true"</code>)</param>
        /// <returns>true if the field contains trueValue, false otherwise</returns>
        /// <exception cref="ArgumentException">if no field has the given name</exception>
        public bool ReadBoolean(string name, string trueValue = "true")
        {
            return ReadBoolean(IndexOf(name), trueValue);
        }

        /// <summary>
        /// Reads a field as a character.
        /// </summary>
        /// <param name="index">the index of the field to read</param>
        /// <returns>the read character</returns>
        /// <exception cref="ArgumentOutOfRangeException">if the index does not correspond to a field</exception>
        public char ReadChar(int index)
        {
            return char.Parse(ReadAndTrim(index));
        }

        /// <summary>
        /// Reads a field as a character.
        /// </summary>
        /// <param name="name">the name of the field to read</param>
        /// <returns>the read character</returns>
        /// <exception cref="ArgumentException">if no field has the given name</exception>
        public char ReadChar(string name)
        {
            return ReadChar(IndexOf(name));
        }

        /// <summary>
        /// Reads a field as a byte.
        /// </summary>
        /// <param name="index">the index of the field to read</param>
        /// <returns>the read byte</returns>
        /// <exception cref="ArgumentOutOfRangeException">if the index does not correspond to a field</exception>
        public byte ReadByte(int index)
        {
            return byte.Parse(ReadAndTrim(index));
        }

        /// <summary>
        /// Reads a field as a byte.
        /// </summary>
        /// <param name="name">the name of the field to read</param>
        /// <returns>the read byte</returns>
        /// <exception cref="ArgumentException">if no field has the given name</exception>
        public byte ReadByte(string name)
        {
            return ReadByte(IndexOf(name));
        }

        /// <summary>
        /// Reads a field as a short.
        /// </summary>
        /// <param name="index">the index of the field to read</param>
        /// <returns>the read short</returns>
        /// <exception cref="ArgumentOutOfRangeException">if the index does not correspond to a field</exception>
        public short ReadShort(int index)
        {
            return short.Parse(ReadAndTrim(index));
        }

        /// <summary>
        /// Reads a field as a short.
        /// </summary>
        /// <param name="name">the name of the field to read</param>
        /// <returns>the read short</returns>
        /// <exception cref="ArgumentException">if no field has the given name</exception>
        public short ReadShort(string name)
        {
            return ReadShort(IndexOf(name));
        }

        /// <summary>
        /// Reads a field as an int.
        /// </summary>
        /// <param name="index">the index of the field to read</param>
        /// <returns>the read int</returns>
        /// <exception cref="ArgumentOutOfRangeException">if the index does not correspond to a field</exception>
        public int ReadInt(int index)
        {
            return int.Parse(ReadAndTrim(index));
        }

        /// <summary>
        /// Reads a field as an int.
        /// </summary>
        /// <param name="name">the name of the field to read</param>
        /// <returns>the read int</returns>
        /// <exception cref="ArgumentException">if no field has the given name</exception>
        public int ReadInt(string name)
        {
            return ReadInt(IndexOf(name));
        }

        /// <summary>
        /// Reads a field as an int.
        /// </summary>
        /// <param name="index">the index of the field to read</param>
        /// <param name="defaultValue">the default value if the field is blank</param>
        /// <returns>the read int</returns>
        /// <exception cref="ArgumentOutOfRangeException">if the index does not correspond to a field</exception>
        public int ReadInt(int index, int defaultValue)
        {
            var value = ReadAndTrim(index);
            return string.IsNullOrWhiteSpace(value) ? defaultValue : int.Parse(value);
        }

        /// <summary>
        /// Reads a field as an int.
        /// </summary>
        /// <param name="name">the name of the field to read</param>
        /// <param name="defaultValue">the default value if the field is blank</param>
        /// <returns>the read int</returns>
        /// <exception cref="ArgumentException">if no field has the given name</exception>
        public int ReadInt(string name, int defaultValue)
        {
            return ReadInt(IndexOf(name), defaultValue);
        }

        /// <summary>
        /// Reads a field as a long.
        /// </summary>
        /// <param name="index">the index of the field to read</param>
        /// <returns>the read long</returns>
        /// <exception cref="ArgumentOutOfRangeException">if the index does not correspond to a field</exception>
        public long ReadLong(int index)
        {
            return long.Parse(ReadAndTrim(index));
        }

        /// <summary>
        /// Reads a field as a long.
        /// </summary>
        /// <param name="name">the name of the field to read</param>
        /// <returns>the read long</returns>
        /// <exception cref="ArgumentException">if no field has the given name</exception>
        public long ReadLong(string name)
        {
            return ReadLong(IndexOf(name));
        }

        /// <summary>
        /// Reads a field as a long.
        /// </summary>
        /// <param name="index">the index of the field to read</param>
        /// <param name="defaultValue">the default value if the field is blank</param>
        /// <returns>the read long</returns>
        /// <exception cref="ArgumentOutOfRangeException">if the index does not correspond to a field</exception>
        public long ReadLong(int index, long defaultValue)
        {
            var value = ReadAndTrim(index);
            return string.IsNullOrWhiteSpace(value) ? defaultValue : long.Parse(value);
        }

        /// <summary>
        /// Reads a field as a long.
        /// </summary>
        /// <param name="name">the name of the field to read</param>
        /// <param name="defaultValue">the default value if the field is blank</param>
        /// <returns>the read long</returns>
        /// <exception cref="ArgumentException">if no field has the given name</exception>
        public long ReadLong(string name, long defaultValue)
        {
            return ReadLong(IndexOf(name), defaultValue);
        }

        /// <summary>
        /// Reads a field as a float.
        /// </summary>
        /// <param name="index">the index of the field to read</param>
        /// <returns>the read float</returns>
        /// <exception cref="ArgumentOutOfRangeException">if the index does not correspond to a field</exception>
        public float ReadFloat(int index)
        {
            return float.Parse(ReadAndTrim(index), Culture);
        }

        /// <summary>
        /// Reads a field as a float.
        /// </summary>
        /// <param name="name">the name of the field to read</param>
        /// <returns>the read float</returns>
        /// <exception cref="ArgumentException">if no field has the given name</exception>
        public float ReadFloat(string name)
        {
            return ReadFloat(IndexOf(name));
        }

        /// <summary>
        /// Reads a field as a double.
        /// </summary>
        /// <param name="index">the index of the field to read</param>
        /// <returns>the read double</returns>
        /// <exception cref="ArgumentOutOfRangeException">if the index does not correspond to a field</exception>
        public double ReadDouble(int index)
        {
            return double.Parse(ReadAndTrim(index), Culture);
        }

        /// <summary>
        /// Reads a field as a double.
        /// </summary>
        /// <param name="name">the name of the field to read</param>
        /// <returns>the read double</returns>
        /// <exception cref="ArgumentException">if no field has the given name</exception>
        public double ReadDouble(string name)
        {
            return ReadDouble(IndexOf(name));
        }

        /// <summary>
        /// Reads a field as a decimal.
        /// </summary>
        /// <param name="index">the index of the field to read</param>
        /// <returns>the read decimal</returns>
        /// <exception cref="ArgumentOutOfRangeException">if the index does not correspond to a field</exception>
        public decimal ReadDecimal(int index)
        {
            return decimal.Parse(ReadAndTrim(index), Culture);
        }

        /// <summary>
        /// Reads a field as a decimal.
        /// </summary>
        /// <param name="name">the name of the field to read</param>
        /// <returns>the read decimal</returns>
        /// <exception cref="ArgumentException">if no field has the given name</exception>
        public decimal ReadDecimal(string name)
        {
            return ReadDecimal(IndexOf(name));
        }

        /// <summary>
        /// Reads a field as a decimal.
        /// </summary>
        /// <param name="index">the index of the field to read</param>
        /// <param name="defaultValue">the default value if the field is blank</param>
        /// <returns>the read decimal</returns>
        /// <exception cref="ArgumentOutOfRangeException">if the index does not correspond to a field</exception>
        public decimal ReadDecimal(int index, decimal defaultValue)
        {
            var value = ReadAndTrim(index);
            return string.IsNullOrWhiteSpace(value) ? defaultValue : decimal.Parse(value, Culture);
        }

        /// <summary>
        /// Reads a field as a decimal.
        /// </summary>
        /// <param name="name">the name of the field to read</param>
        /// <param name="defaultValue">the default value if the field is blank</param>
        /// <returns>the read decimal</returns>
        /// <exception cref="ArgumentException">if no field has the given name</exception>
        public decimal ReadDecimal(string name, decimal defaultValue)
        {
            return ReadDecimal(IndexOf(name), defaultValue);
        }

        /// <summary>
        /// Reads a field as a date.
        /// </summary>
        /// <param name="index">the index of the field to read</param>
        /// <returns>the read date</returns>
        /// <exception cref="ArgumentOutOfRangeException">if the index does not correspond to a field</exception>
        public DateTime ReadDate(int index)
        {
            return ReadDate(index, DatePattern);
        }

        /// <summary>
        /// Reads a field as a date.
        /// </summary>
        /// <param name="name">the name of the field to read</param>
        /// <returns>the read date</returns>
        /// <exception cref="ArgumentException">if no field has the given name</exception>
        public DateTime ReadDate(string name)
        {
            return ReadDate(IndexOf(name));
        }

        /// <summary>
        /// Reads a field as a date.
        /// </summary>
        /// <param name="index">the index of the field to read</param>
        /// <param name="defaultValue">the default value if the field is blank</param>
        /// <returns>the read date</returns>
        /// <exception cref="ArgumentOutOfRangeException">if the index does not correspond to a field</exception>
        public DateTime ReadDate(int index, DateTime defaultValue)
        {
            return ReadDate(index, DatePattern, defaultValue);
        }

        /// <summary>
        /// Reads a field as a date.
        /// </summary>
        /// <param name="name">the name of the field to read</param>
        /// <param name="defaultValue">the default value if the field is blank</param>
        /// <returns>the read date</returns>
        /// <exception cref="ArgumentException">if no field has the given name</exception>
        public DateTime ReadDate(string name, DateTime defaultValue)
        {
            return ReadDate(IndexOf(name), defaultValue);
        }

        /// <summary>
        /// Reads a field as a date.
        /// </summary>
        /// <param name="index">the index of the field to read</param>
        /// <param name="pattern">the pattern to use to parse the date</param>
        /// <returns>the read date</returns>
        /// <exception cref="ArgumentOutOfRangeException">if the index does not correspond to a field</exception>
        public DateTime ReadDate(int index, string pattern)
        {
            return DateTime.ParseExact(ReadAndTrim(index), pattern, Culture);
        }

        /// <summary>
        /// Reads a field as a date.
        /// </summary>
        /// <param name="name">the name of the field to read</param>
        /// <param name="pattern">the pattern to use to parse the date</param>
        /// <returns>the read date</returns>
        /// <exception cref="ArgumentException">if no field has the given name</exception>
        public DateTime ReadDate(string name, string pattern)
        {
            return ReadDate(IndexOf(name), pattern);
        }

        /// <summary>
        /// Reads a field as a date, using defaultValue if needed.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="pattern"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public DateTime ReadDate(int index, string pattern, DateTime defaultValue)
        {
            var value = ReadAndTrim(index);
            return string.IsNullOrWhiteSpace(value) ? defaultValue : DateTime.ParseExact(value, pattern, Culture);
        }

        /// <summary>
        /// Reads a field as a date.
        /// </summary>
        /// <param name="name">the name of the field to read</param>
        /// <param name="pattern">the pattern to use to parse the date</param>
        /// <param name="defaultValue">the default value if the field is blank</param>
        /// <returns>the read date</returns>
        /// <exception cref="ArgumentException">if no field has the given name</exception>
        public DateTime ReadDate(string name, string pattern, DateTime defaultValue)
        {
            return ReadDate(IndexOf(name), pattern, defaultValue);
        }

        private string ReadAndTrim(int index)
        {
            if (index < 0 || index >= Values.Length)
            {
                throw new ArgumentOutOfRangeException(string.Format("Index ({0}) is out of range.", index));
            }
            return Values[index] == null ? null : Values[index].Trim();
        }

        private int IndexOf(string name)
        {
            if (Names != null)
            {
                var index = Array.IndexOf(Names, name);
                if (index != -1)
                {
                    return index;
                }
            }
            throw new ArgumentException(string.Format("No field with name \"{0}\" could be found.", name));
        }
    }
}