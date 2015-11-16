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

using System.Configuration;
using System.Data.Common;

namespace Summer.Batch.Data.Incrementer
{
    /// <summary>
    /// Base class for IDataFieldMaxValueIncrementer that holds general information such as the connection string or the provider factory.
    /// </summary>
    public abstract class AbstractDataFieldMaxValueIncrementer : IDataFieldMaxValueIncrementer
    {
        /// <summary>
        /// Connection String
        /// </summary>
        protected string ConnectionString;

        /// <summary>
        /// Database Provider Factory
        /// </summary>
        protected DbProviderFactory ProviderFactory;

        /// <summary>
        /// The connection string settings to use for connecting the database.
        /// </summary>
        public ConnectionStringSettings ConnectionStringSettings
        {
            set
            {
                ConnectionString = value.ConnectionString;
                ProviderFactory = DbProviderFactories.GetFactory(value.ProviderName);
            }
        }

        /// <summary>
        /// The name of the incrementer in the database.
        /// </summary>
        public string IncrementerName { get; set; }

        /// <summary>
        /// Returns the next value for the current incrementer.
        /// </summary>
        /// <returns></returns>
        public abstract long NextLong();

        /// <summary>
        /// Create DbCommand for given query and using provided Database Connection.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        protected static DbCommand GetCommand(string query, DbConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = query;
            return command;
        }

        /// <summary>
        /// Returns the Connection to the database.
        /// The returned connection is already opened.
        /// </summary>
        /// <returns></returns>
        protected DbConnection GetConnection()
        {
            var connection = ProviderFactory.CreateConnection();
            connection.ConnectionString = ConnectionString;
            connection.Open();
            return connection;
        }
    }
}