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

using System.Configuration;
using System.Data.Common;
using Summer.Batch.Common.Transaction;
using Summer.Batch.Data.Parameter;

namespace Summer.Batch.Data
{
    /// <summary>
    /// Implementation of <see cref="IConnectionProvider"/> that relies on <see cref="ConnectionUtil"/>
    /// to manager connections.
    /// </summary>
    public class ConnectionProvider : IConnectionProvider
    {
        private string _connectionString;

        /// <summary>
        /// The connection string settings used to create a connection.
        /// </summary>
        public ConnectionStringSettings ConnectionStringSettings
        {
            set
            {
                ProviderFactory = DbProviderFactories.GetFactory(value.ProviderName);
                PlaceholderGetter = DatabaseExtensionManager.GetPlaceholderGetter(value.ProviderName);
                _connectionString = value.ConnectionString;
            }
        }

        /// <summary>
        /// An open connection ready to use.
        /// </summary>
        public DbConnection Connection
        {
            get { return ConnectionUtil.GetConnection(ProviderFactory, _connectionString); }
        }

        /// <summary>
        /// The provider factory to use with the provided connection.
        /// </summary>
        public DbProviderFactory ProviderFactory { get; private set; }

        /// <summary>
        /// The placeholder getter to use with the provided connection.
        /// </summary>
        public IPlaceholderGetter PlaceholderGetter { get; private set; }
    }
}