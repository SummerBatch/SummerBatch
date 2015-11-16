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
using System.Data;

namespace Summer.Batch.Data
{
    /// <summary>
    ///  Utility class to handle data conversion from a <see cref="IDataRecord"/>.
    /// </summary>
    public class DataRecordWrapper
    {
        private readonly IDataRecord _dataRecord;

        /// <summary>
        /// Constructs a new <see cref="DataRecordWrapper"/>.
        /// </summary>
        /// <param name="dataRecord">the data record to get the data from</param>
        public DataRecordWrapper(IDataRecord dataRecord)
        {
            _dataRecord = dataRecord;
        }

        /// <summary>
        /// Gets a raw value from the data record.
        /// </summary>
        /// <param name="i">the index of the column to get the data from</param>
        /// <returns>the data in the column, without conversion</returns>
        public object GetValue(int i)
        {
            return _dataRecord.GetValue(i);
        }

        /// <summary>
        /// Gets a converted value from the data record.
        /// </summary>
        /// <typeparam name="T">the type to convert the data to</typeparam>
        /// <param name="i">the index of the column to get the data from</param>
        /// <returns>the converted data in the column</returns>
        public T Get<T>(int i)
        {
            return (T)Get(i, typeof(T), default(T));
        }

        /// <summary>
        /// Gets a converted value from the data record.
        /// </summary>
        /// <typeparam name="T">the type to convert the data to</typeparam>
        /// <param name="i">the index of the column to get the data from</param>
        /// <param name="defaultValue">the default value if the column is null.</param>
        /// <returns>the converted data in the column</returns>
        public T Get<T>(int i, T defaultValue)
        {
            return (T)Get(i, typeof(T), defaultValue);
        }

        /// <summary>
        /// Gets a converted value from the data record.
        /// </summary>
        /// <param name="i">the index of the column to get the data from</param>
        /// <param name="type">the type to convert the data to</param>
        /// <param name="defaultValue">the default value if the column is null.</param>
        /// <returns>the converted data in the column</returns>
        public object Get(int i, Type type, object defaultValue)
        {
            if (_dataRecord.IsDBNull(i))
            {
                return defaultValue;
            }
            var targetType = type;
            if (type.IsConstructedGenericType &&
                type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                targetType = type.GenericTypeArguments[0];
            }
            return Converter.Convert(_dataRecord.GetValue(i), targetType);
        }
    }
}