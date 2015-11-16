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
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;

namespace Summer.Batch.Common.Transaction
{
    /// <summary>
    /// Utility class for connections. It holds a connection for a given connection string on a thread.
    /// </summary>
    public static class ConnectionUtil
    {
        private static readonly ThreadLocal<IDictionary<string, DbConnection>> Connections =
           new ThreadLocal<IDictionary<string, DbConnection>>(() => new Dictionary<string, DbConnection>());

        /// <summary>
        /// Gets a connection for the given connectionString. If one already exists for the current
        /// thread it is returned, otherwise a new one is created using the given provider factory.
        /// </summary>
        /// <param name="providerFactory">the provider factory to use when creating a new connection</param>
        /// <param name="connectionString">the connection string of the connection to get</param>
        /// <returns>a <see cref="DbConnection"/> for the given connection string</returns>
        public static DbConnection GetConnection(DbProviderFactory providerFactory, string connectionString)
        {
            var currentConnections = Connections.Value;
            DbConnection connection;
            if (!currentConnections.TryGetValue(connectionString, out connection))
            {
                connection = providerFactory.CreateConnection();
                connection.ConnectionString = connectionString;
                connection.Open();
                currentConnections[connectionString] = connection;
            }
            return connection;
        }

        /// <summary>
        /// Closes the connections and clears the connection dictionary for the current thread.
        /// </summary>
        public static void ReleaseConnections()
        {
            foreach (var connection in Connections.Value.Values.Where(c => c != null && c.State == ConnectionState.Open))
            {
                connection.Close();
            }
            Connections.Value.Clear();
        }
    }
}