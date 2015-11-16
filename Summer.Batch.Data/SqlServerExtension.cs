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

using System.Collections.Generic;
using Summer.Batch.Data.Incrementer;
using Summer.Batch.Data.Parameter;

namespace Summer.Batch.Data
{
    /// <summary>
    /// Extension for SQL Server support (provider name "System.Data.SqlClient").
    /// </summary>
    public class SqlServerExtension : IDatabaseExtension
    {
        /// <summary>
        /// An enumerable containing the only supported provider name ("System.Data.SqlClient").
        /// </summary>
        public IEnumerable<string> ProviderNames { get { return new[] { "System.Data.SqlClient" }; } }

        /// <summary>
        /// The placeholder getter for SQL Server.
        /// </summary>
        public IPlaceholderGetter PlaceholderGetter { get { return new PlaceholderGetter(name => "@" + name, true); } }

        /// <summary>
        /// An instance of <see cref="SqlServerIncrementer"/>.
        /// </summary>
        public IDataFieldMaxValueIncrementer Incrementer { get { return new SqlServerIncrementer { ColumnName = "ID" }; } }
    }
}