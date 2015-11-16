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
using System.Linq;

namespace Summer.Batch.Data.Incrementer
{
    /// <summary>
    /// Column maximum value incrementer for the sql server database.
    /// </summary>
    public class SqlServerIncrementer : AbstractColumnMaxValueIncrementer
    {
        private long[] _valueCache;
        private int _nextValueIndex = -1;

        /// <summary>
        /// Get next value from column.
        /// </summary>
        /// <returns></returns>
        public override long NextLong()
        {
            if (_nextValueIndex < 0 || _nextValueIndex >= CacheSize)
            {
                _valueCache = new long[CacheSize];
                _nextValueIndex = 0;
                using (var connection = GetConnection())
                {
                    using (var insertCommand = GetCommand(string.Format("insert into {0} default values; select scope_identity();", IncrementerName), connection))
                    {
                        for (var i = 0; i < CacheSize; i++)
                        {
                            var result = insertCommand.ExecuteScalar();
                            if (result is decimal)
                            {
                                _valueCache[i] = Convert.ToInt64((decimal)result);
                            }
                            else
                            {
                                throw new Exception("scope_identity() failed after executing an update");
                            }
                        }
                    }
                    var maxValue = _valueCache.Last();
                    using (var deleteCommand = GetCommand(string.Format("delete from {0} where {1} < {2}", IncrementerName, ColumnName, maxValue), connection))
                    {
                        deleteCommand.ExecuteNonQuery();
                    }
                }
            }
            return _valueCache[_nextValueIndex++];
        }
    }
}