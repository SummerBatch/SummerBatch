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
namespace Summer.Batch.Data.Incrementer
{
    /// <summary>
    /// Base class for IDataFieldMaxValueIncrementer that use a column in a custom sequence table.
    /// </summary>
    public abstract class AbstractColumnMaxValueIncrementer : AbstractDataFieldMaxValueIncrementer
    {
        private int _cacheSize = 1;

        /// <summary>
        /// The number of values that are cached
        /// </summary>
        public int CacheSize { get { return _cacheSize; } set { _cacheSize = value; } }

        /// <summary>
        /// The name of the column that holds the id in the table
        /// </summary>
        public string ColumnName { get; set; }
    }
}