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
using System.Configuration;
using System.Data.Common;
using System.Linq;
using Summer.Batch.Data;
using Summer.Batch.Data.Parameter;
using Summer.Batch.Common.Factory;
using Summer.Batch.Infrastructure.Item.Support;
using Summer.Batch.Common.Util;

namespace Summer.Batch.Infrastructure.Item.Database
{
    /// <summary>
    /// Implementation of <see cref="T:IItemReader"/> that reads items from a database using a <see cref="DbDataReader"/>.
    /// It creates its own connection as it will be busy until the reader is closed.
    /// </summary>
    /// <typeparam name="T">&nbsp;the type of the read elements</typeparam>
    public class DataReaderItemReader<T> : AbstractItemCountingItemStreamItemReader<T>, IInitializationPostOperations where T : class
    {
        private bool _initialized;
        private DbProviderFactory _factory;
        private DbConnection _connection;
        private DbCommand _command;
        private DbDataReader _dataReader;

        /// <summary>
        /// A <see cref="ConnectionString"/> that holds the information on the connection to use
        /// </summary>
        public ConnectionStringSettings ConnectionString { get; set; }

        /// <summary>
        /// The sql query to execute for creating the data reader.
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Source for the parameter values.
        /// </summary>
        public IQueryParameterSource ParameterSource { get; set; }

        /// <summary>
        /// Mapper that creates the items from the data reader.
        /// </summary>
        public RowMapper<T> RowMapper { get; set; }


        private int? _commandTimeout;

        /// <summary>
        /// Default timeout is 30 seconds based on MSDN
        /// If a 0 timeout is provided, this means there is no timeout limit
        /// Allows the user to specify a longer timeout to account for longer running scripts than 30 seconds
        /// Since V1.1.11
        /// </summary>
        public int CommandTimeout
        {
            get
            {
                if (_commandTimeout == null)
                {
                    //Set the timeout to -1 to indicate to use the default timeout
                    _commandTimeout = -1;
                }

                return _commandTimeout.Value;
            }
            set
            {
                _commandTimeout = value;
            }
        }


        /// <summary>
        /// Default constructor.
        /// </summary>
        public DataReaderItemReader()
        {
            Name = typeof(DataReaderItemReader<T>).Name;
        }

        /// <summary>
        /// See <see cref="IInitializationPostOperations.AfterPropertiesSet"/>.
        /// </summary>
        public void AfterPropertiesSet()
        {
            Assert.NotNull(ConnectionString, "ConnectionString is required");
            Assert.NotNull(Query, "The sql query must be provided");
            Assert.NotNull(RowMapper, "RowMapper must be provided");
        }

        /// <summary>
        /// Opens the stream.
        /// </summary>
        protected override void DoOpen()
        {
            Assert.State(!_initialized, "Reader is already initialized.");
            Assert.IsNull(_dataReader, "Data reader is still opened.");

            try
            {
                _factory = DbProviderFactories.GetFactory(ConnectionString.ProviderName);
                InitializeConnection();
                InitializeDataReader();
            }
            catch (Exception)
            {
                Close();
                throw;
            }
        }

        /// <summary>
        /// Closes the stream.
        /// </summary>
        protected override void DoClose()
        {
            _initialized = false;
            if (_dataReader!= null)
            {
                _dataReader.Close();
            }
            if (_command != null)
            {
                _command.Dispose();
            }
            if (_connection != null)
            {
                _connection.Close();
            }
        }

        /// <summary>
        /// Reads the next item from the data reader.
        /// </summary>
        /// <returns>the read item or null if there are no more results</returns>
        protected override T DoRead()
        {
            if (_dataReader == null)
            {
                throw new InvalidOperationException("The reader must be open before it can read.");
            }

            return !_dataReader.Read() ? null : RowMapper(_dataReader, CurrentItemCount);
        }

        /// <summary>
        /// Initizalizes the connection to the database.
        /// </summary>
        private void InitializeConnection()
        {
            _connection = _factory.CreateConnection();
            _connection.ConnectionString = ConnectionString.ConnectionString;
            _connection.Open();
        }

        /// <summary>
        /// Initializes the data reader.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        private void InitializeDataReader()
        {
            var parsedQuery = new ParsedQuery(Query, DatabaseExtensionManager.GetPlaceholderGetter(ConnectionString.ProviderName));
            _command = _connection.CreateCommand();
            _command.CommandText = parsedQuery.SubstitutedQuery;

            // Added in V1.1.11 to allow definition of a custom timeout
            if (CommandTimeout != -1)
            {
                _command.CommandTimeout = CommandTimeout;
            }

            SetParameters(parsedQuery);
            _dataReader = _command.ExecuteReader();
        }

        private void SetParameters(ParsedQuery query)
        {
            if (query.Named)
            {
                foreach (var name in query.ParameterNames.Distinct())
                {
                    _command.AddParameter(name, ParameterSource[name]);
                }
            }
            else
            {
                for (var i = 0; i < query.ParameterNames.Count; i++)
                {
                    var name = query.ParameterNames[i];
                    _command.AddParameter(i.ToString(), ParameterSource[name]);
                }
            }
        }
    }
}