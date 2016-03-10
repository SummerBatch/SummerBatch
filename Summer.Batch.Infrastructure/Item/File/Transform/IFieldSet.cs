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

namespace Summer.Batch.Infrastructure.Item.File.Transform
{
    /// <summary>
    /// Interface used by flat file input sources to read data from a string array.
    /// </summary>
    public interface IFieldSet
    {
        /// <summary>
        /// The number of fields in this field set.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Whether the field set has named fields.
        /// </summary>
        bool HasNames { get; }

        /// <summary>
        /// The names of the fields.
        /// </summary>
        string[] Names { get; }

        /// <summary>
        /// The values of the fields.
        /// </summary>
        string[] Values { get; }

        /// <summary>
        /// Reads a field as a string.
        /// </summary>
        /// <param name="index">the index of the field to read</param>
        /// <returns>the trimmed content of the field</returns>
        /// <exception cref="ArgumentOutOfRangeException">&nbsp;if the index does not correspond to a field</exception>
        string ReadString(int index);

        /// <summary>
        /// Reads a field as a string.
        /// </summary>
        /// <param name="name">the name of the field to read</param>
        /// <returns>the trimmed content of the field</returns>
        /// <exception cref="ArgumentException">&nbsp;if no field has the given name</exception>
        string ReadString(string name);

        /// <summary>
        /// Reads a field as a string.
        /// </summary>
        /// <param name="index">the index of the field to read</param>
        /// <returns>the raw content of the field</returns>
        /// <exception cref="ArgumentOutOfRangeException">&nbsp;if the index does not correspond to a field</exception>
        string ReadRawString(int index);

        /// <summary>
        /// Reads a field as a string.
        /// </summary>
        /// <param name="name">the name of the field to read</param>
        /// <returns>the raw content of the field</returns>
        /// <exception cref="ArgumentException">&nbsp;if no field has the given name</exception>
        string ReadRawString(string name);

        /// <summary>
        /// Reads a field as a boolean.
        /// </summary>
        /// <param name="index">the index of the field to read</param>
        /// <param name="trueValue">the literal corresponding to true (default is <c>"true"</c>)</param>
        /// <returns>true if the field contains trueValue, false otherwise</returns>
        /// <exception cref="ArgumentOutOfRangeException">&nbsp;if the index does not correspond to a field</exception>
        bool ReadBoolean(int index, string trueValue = "true");

        /// <summary>
        /// Reads a field as a boolean.
        /// </summary>
        /// <param name="name">the name of the field to read</param>
        /// <param name="trueValue">the literal corresponding to true (default is <c>"true"</c>)</param>
        /// <returns>true if the field contains trueValue, false otherwise</returns>
        /// <exception cref="ArgumentException">&nbsp;if no field has the given name</exception>
        bool ReadBoolean(string name, string trueValue = "true");

        /// <summary>
        /// Reads a field as a character.
        /// </summary>
        /// <param name="index">the index of the field to read</param>
        /// <returns>the read character</returns>
        /// <exception cref="ArgumentOutOfRangeException">&nbsp;if the index does not correspond to a field</exception>
        char ReadChar(int index);

        /// <summary>
        /// Reads a field as a character.
        /// </summary>
        /// <param name="name">the name of the field to read</param>
        /// <returns>the read character</returns>
        /// <exception cref="ArgumentException">&nbsp;if no field has the given name</exception>
        char ReadChar(string name);

        /// <summary>
        /// Reads a field as a byte.
        /// </summary>
        /// <param name="index">the index of the field to read</param>
        /// <returns>the read byte</returns>
        /// <exception cref="ArgumentOutOfRangeException">&nbsp;if the index does not correspond to a field</exception>
        byte ReadByte(int index);

        /// <summary>
        /// Reads a field as a byte.
        /// </summary>
        /// <param name="name">the name of the field to read</param>
        /// <returns>the read byte</returns>
        /// <exception cref="ArgumentException">&nbsp;if no field has the given name</exception>
        byte ReadByte(string name);

        /// <summary>
        /// Reads a field as a short.
        /// </summary>
        /// <param name="index">the index of the field to read</param>
        /// <returns>the read short</returns>
        /// <exception cref="ArgumentOutOfRangeException">&nbsp;if the index does not correspond to a field</exception>
        short ReadShort(int index);

        /// <summary>
        /// Reads a field as a short.
        /// </summary>
        /// <param name="name">the name of the field to read</param>
        /// <returns>the read short</returns>
        /// <exception cref="ArgumentException">&nbsp;if no field has the given name</exception>
        short ReadShort(string name);

        /// <summary>
        /// Reads a field as an int.
        /// </summary>
        /// <param name="index">the index of the field to read</param>
        /// <returns>the read int</returns>
        /// <exception cref="ArgumentOutOfRangeException">&nbsp;if the index does not correspond to a field</exception>
        int ReadInt(int index);

        /// <summary>
        /// Reads a field as an int.
        /// </summary>
        /// <param name="name">the name of the field to read</param>
        /// <returns>the read int</returns>
        /// <exception cref="ArgumentException">&nbsp;if no field has the given name</exception>
        int ReadInt(string name);

        /// <summary>
        /// Reads a field as an int.
        /// </summary>
        /// <param name="index">the index of the field to read</param>
        /// <param name="defaultValue">the default value if the field is blank</param>
        /// <returns>the read int</returns>
        /// <exception cref="ArgumentOutOfRangeException">&nbsp;if the index does not correspond to a field</exception>
        int ReadInt(int index, int defaultValue);

        /// <summary>
        /// Reads a field as an int.
        /// </summary>
        /// <param name="name">the name of the field to read</param>
        /// <param name="defaultValue">the default value if the field is blank</param>
        /// <returns>the read int</returns>
        /// <exception cref="ArgumentException">&nbsp;if no field has the given name</exception>
        int ReadInt(string name, int defaultValue);

        /// <summary>
        /// Reads a field as a long.
        /// </summary>
        /// <param name="index">the index of the field to read</param>
        /// <returns>the read long</returns>
        /// <exception cref="ArgumentOutOfRangeException">&nbsp;if the index does not correspond to a field</exception>
        long ReadLong(int index);

        /// <summary>
        /// Reads a field as a long.
        /// </summary>
        /// <param name="name">the name of the field to read</param>
        /// <returns>the read long</returns>
        /// <exception cref="ArgumentException">&nbsp;if no field has the given name</exception>
        long ReadLong(string name);

        /// <summary>
        /// Reads a field as a long.
        /// </summary>
        /// <param name="index">the index of the field to read</param>
        /// <param name="defaultValue">the default value if the field is blank</param>
        /// <returns>the read long</returns>
        /// <exception cref="ArgumentOutOfRangeException">&nbsp;if the index does not correspond to a field</exception>
        long ReadLong(int index, long defaultValue);

        /// <summary>
        /// Reads a field as a long.
        /// </summary>
        /// <param name="name">the name of the field to read</param>
        /// <param name="defaultValue">the default value if the field is blank</param>
        /// <returns>the read long</returns>
        /// <exception cref="ArgumentException">&nbsp;if no field has the given name</exception>
        long ReadLong(string name, long defaultValue);

        /// <summary>
        /// Reads a field as a float.
        /// </summary>
        /// <param name="index">the index of the field to read</param>
        /// <returns>the read float</returns>
        /// <exception cref="ArgumentOutOfRangeException">&nbsp;if the index does not correspond to a field</exception>
        float ReadFloat(int index);

        /// <summary>
        /// Reads a field as a float.
        /// </summary>
        /// <param name="name">the name of the field to read</param>
        /// <returns>the read float</returns>
        /// <exception cref="ArgumentException">&nbsp;if no field has the given name</exception>
        float ReadFloat(string name);

        /// <summary>
        /// Reads a field as a double.
        /// </summary>
        /// <param name="index">the index of the field to read</param>
        /// <returns>the read double</returns>
        /// <exception cref="ArgumentOutOfRangeException">&nbsp;if the index does not correspond to a field</exception>
        double ReadDouble(int index);

        /// <summary>
        /// Reads a field as a double.
        /// </summary>
        /// <param name="name">the name of the field to read</param>
        /// <returns>the read double</returns>
        /// <exception cref="ArgumentException">&nbsp;if no field has the given name</exception>
        double ReadDouble(string name);

        /// <summary>
        /// Reads a field as a decimal.
        /// </summary>
        /// <param name="index">the index of the field to read</param>
        /// <returns>the read decimal</returns>
        /// <exception cref="ArgumentOutOfRangeException">&nbsp;if the index does not correspond to a field</exception>
        decimal ReadDecimal(int index);

        /// <summary>
        /// Reads a field as a decimal.
        /// </summary>
        /// <param name="name">the name of the field to read</param>
        /// <returns>the read decimal</returns>
        /// <exception cref="ArgumentException">&nbsp;if no field has the given name</exception>
        decimal ReadDecimal(string name);

        /// <summary>
        /// Reads a field as a decimal.
        /// </summary>
        /// <param name="index">the index of the field to read</param>
        /// <param name="defaultValue">the default value if the field is blank</param>
        /// <returns>the read decimal</returns>
        /// <exception cref="ArgumentOutOfRangeException">&nbsp;if the index does not correspond to a field</exception>
        decimal ReadDecimal(int index, decimal defaultValue);

        /// <summary>
        /// Reads a field as a decimal.
        /// </summary>
        /// <param name="name">the name of the field to read</param>
        /// <param name="defaultValue">the default value if the field is blank</param>
        /// <returns>the read decimal</returns>
        /// <exception cref="ArgumentException">&nbsp;if no field has the given name</exception>
        decimal ReadDecimal(string name, decimal defaultValue);

        /// <summary>
        /// Reads a field as a date.
        /// </summary>
        /// <param name="index">the index of the field to read</param>
        /// <returns>the read date</returns>
        /// <exception cref="ArgumentOutOfRangeException">&nbsp;if the index does not correspond to a field</exception>
        DateTime ReadDate(int index);

        /// <summary>
        /// Reads a field as a date.
        /// </summary>
        /// <param name="name">the name of the field to read</param>
        /// <returns>the read date</returns>
        /// <exception cref="ArgumentException">&nbsp;if no field has the given name</exception>
        DateTime ReadDate(string name);

        /// <summary>
        /// Reads a field as a date.
        /// </summary>
        /// <param name="index">the index of the field to read</param>
        /// <param name="defaultValue">the default value if the field is blank</param>
        /// <returns>the read date</returns>
        /// <exception cref="ArgumentOutOfRangeException">&nbsp;if the index does not correspond to a field</exception>
        DateTime ReadDate(int index, DateTime defaultValue);

        /// <summary>
        /// Reads a field as a date.
        /// </summary>
        /// <param name="name">the name of the field to read</param>
        /// <param name="defaultValue">the default value if the field is blank</param>
        /// <returns>the read date</returns>
        /// <exception cref="ArgumentException">&nbsp;if no field has the given name</exception>
        DateTime ReadDate(string name, DateTime defaultValue);

        /// <summary>
        /// Reads a field as a date.
        /// </summary>
        /// <param name="index">the index of the field to read</param>
        /// <param name="pattern">the pattern to use to parse the date</param>
        /// <returns>the read date</returns>
        /// <exception cref="ArgumentOutOfRangeException">&nbsp;if the index does not correspond to a field</exception>
        DateTime ReadDate(int index, string pattern);

        /// <summary>
        /// Reads a field as a date.
        /// </summary>
        /// <param name="name">the name of the field to read</param>
        /// <param name="pattern">the pattern to use to parse the date</param>
        /// <returns>the read date</returns>
        /// <exception cref="ArgumentException">&nbsp;if no field has the given name</exception>
        DateTime ReadDate(string name, string pattern);

        /// <summary>
        /// Reads a field as a date.
        /// </summary>
        /// <param name="index">the index of the field to read</param>
        /// <param name="pattern">the pattern to use to parse the date</param>
        /// <param name="defaultValue">the default value if the field is blank</param>
        /// <returns>the read date</returns>
        /// <exception cref="ArgumentOutOfRangeException">&nbsp;if the index does not correspond to a field</exception>
        DateTime ReadDate(int index, string pattern, DateTime defaultValue);

        /// <summary>
        /// Reads a field as a date.
        /// </summary>
        /// <param name="name">the name of the field to read</param>
        /// <param name="pattern">the pattern to use to parse the date</param>
        /// <param name="defaultValue">the default value if the field is blank</param>
        /// <returns>the read date</returns>
        /// <exception cref="ArgumentException">&nbsp;if no field has the given name</exception>
        DateTime ReadDate(string name, string pattern, DateTime defaultValue);
    }
}