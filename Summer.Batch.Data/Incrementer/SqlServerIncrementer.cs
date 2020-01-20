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
                    using (var insertCommand = GetCommand(string.Format("SELECT NEXT VALUE FOR {0};", IncrementerName), connection))
                    {
                        for (var i = 0; i < CacheSize; i++)
                        {
                            var result = insertCommand.ExecuteScalar();
                            _valueCache[i] = (long)result;
                        }
                    }
                    var maxValue = _valueCache.Last();
                }
            }
            return _valueCache[_nextValueIndex++];
        }
    }
}