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
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Summer.Batch.Common.Util;
using Summer.Batch.Data.Parameter;

namespace Summer.Batch.Data
{
    /// <summary>
    /// An action executed on each data record in a query.
    /// </summary>
    /// <param name="dataRecord">a data record representing a row</param>
    public delegate void RowHandler(IDataRecord dataRecord);

    /// <summary>
    /// A function for mapping a row to an element.
    /// </summary>
    /// <typeparam name="T">&nbsp;The type of the mapped element</typeparam>
    /// <param name="dataRecord">a data record representing a row</param>
    /// <param name="rowNumber">the current row number</param>
    /// <returns>the mapped element</returns>
    public delegate T RowMapper<out T>(IDataRecord dataRecord, int rowNumber);

    /// <summary>
    /// A function that extracts an element from a data reader.
    /// </summary>
    /// <typeparam name="T">&nbsp;the type of the extracted element</typeparam>
    /// <param name="dataReader">a datareader to extract the element from</param>
    /// <returns>the extracted element</returns>
    public delegate T DataReaderExtractor<out T>(IDataReader dataReader);

    /// <summary>
    /// Dedicated database operations. Most services that need to interact with a database should
    /// be using the methods exposed by this operator. 
    /// </summary>
    public class DbOperator
    {
        /// <summary>
        /// The <see cref="IConnectionProvider"/> to use to get the connections.
        /// </summary>
        public IConnectionProvider ConnectionProvider { get; set; }

        #region Scalar queries

        /// <summary>
        /// Execute a query expected to return a single result.
        /// </summary>
        /// <typeparam name="T">&nbsp;the type of the result</typeparam>
        /// <param name="query">the SQL query</param>
        /// <param name="parameters">the parameters of the query</param>
        /// <returns>the result of the query</returns>
        public T Query<T>(string query, IDictionary<string, object> parameters)
        {
            return Query<T>(new ParsedQuery(query, ConnectionProvider.PlaceholderGetter),
                new DictionaryParameterSource { Parameters = parameters });
        }

        /// <summary>
        /// Execute a query expected to return a single result.
        /// </summary>
        /// <typeparam name="T">&nbsp;the type of the result</typeparam>
        /// <param name="query">the SQL query</param>
        /// <param name="parameterSource">a parameter source that holds the parameter values for the query</param>
        /// <returns>the result of the query</returns>
        public T Query<T>(string query, IQueryParameterSource parameterSource = null)
        {
            return Query<T>(new ParsedQuery(query, ConnectionProvider.PlaceholderGetter), parameterSource);
        }

        /// <summary>
        /// Execute a query exepcted to return a single result.
        /// </summary>
        /// <typeparam name="T">&nbsp;the type of the result</typeparam>
        /// <param name="query">the parsed query</param>
        /// <param name="parameterSource">a parameter source that holds the parameter values for the query</param>
        /// <returns>the result of the query</returns>
        public T Query<T>(ParsedQuery query, IQueryParameterSource parameterSource = null)
        {
            using (var command = GetCommand(query.SubstitutedQuery))
            {
                SetParameters(command, query, parameterSource);
                return Converter.Convert<T>(command.ExecuteScalar());
            }
        }

        #endregion

        #region DataAdapter queries

        /// <summary>
        /// Executes a select query using a DataAdapter. A row handler is run on each returned row.
        /// </summary>
        /// <param name="query">the SQL query</param>
        /// <param name="rowHandler">a mapper that will be executed on each returned row</param>
        /// <param name="parameters">the parameters of the query</param>
        public void Select(string query, RowHandler rowHandler, IDictionary<string, object> parameters)
        {
            Select(query, GetDataReaderExtractor(rowHandler), new DictionaryParameterSource { Parameters = parameters });
        }

        /// <summary>
        /// Executes a select query using a DataAdapter and returns a list of objects using the provided row mapper.
        /// </summary>
        /// <typeparam name="T">&nbsp;the type of the elements to return</typeparam>
        /// <param name="query">the SQL query</param>
        /// <param name="mapper">the row mapper responsible for creating the returned elements</param>
        /// <param name="parameters">the parameters of the query</param>
        /// <returns>a list of T elements obtained by executing the query</returns>
        public IList<T> Select<T>(string query, RowMapper<T> mapper, IDictionary<string, object> parameters)
        {
            return Select(query, GetDataReaderExtractor(mapper), new DictionaryParameterSource { Parameters = parameters });
        }

        /// <summary>
        /// Executes a select query using a DataAdapter and returns data using the provided data reader extractor.
        /// </summary>
        /// <typeparam name="T">&nbsp;the type of the elements to return</typeparam>
        /// <param name="query">the SQL query</param>
        /// <param name="mapper">the row mapper responsible for creating the returned elements</param>
        /// <param name="parameterSource">a parameter source that holds the parameter values for the query</param>
        /// <returns>a list of T elements obtained by executing the query</returns>
        public IList<T> Select<T>(string query, RowMapper<T> mapper, IQueryParameterSource parameterSource)
        {
            return Select(query, GetDataReaderExtractor(mapper), parameterSource);
        }

        /// <summary>
        /// Executes a select query using a DataAdapter and returns data using the provided data reader extractor.
        /// </summary>
        /// <typeparam name="T">&nbsp;the type of the data to return</typeparam>
        /// <param name="query">the SQL query</param>
        /// <param name="extractor">the data reader mapper used to extract data</param>
        /// <param name="parameters">the parameters of the query</param>
        /// <returns>the extracted data</returns>
        public T Select<T>(string query, DataReaderExtractor<T> extractor, IDictionary<string, object> parameters)
        {
            return Select(query, extractor, new DictionaryParameterSource { Parameters = parameters });
        }

        /// <summary>
        /// Executes a select query using a DataAdapter and returns data using the provided data reader extractor.
        /// </summary>
        /// <typeparam name="T">&nbsp;the type of the data to return</typeparam>
        /// <param name="query">the SQL query</param>
        /// <param name="extractor">the data reader mapper used to extract data</param>
        /// <param name="parameterSource">a parameter source that holds the parameter values for the query</param>
        /// <returns>the extracted data</returns>
        public T Select<T>(string query, DataReaderExtractor<T> extractor, IQueryParameterSource parameterSource = null)
        {
            var parsedQuery = new ParsedQuery(query, ConnectionProvider.PlaceholderGetter);
            return Select(parsedQuery, extractor, parameterSource);
        }

        /// <summary>
        /// Executes a select query using a DataAdapter and returns data using the provided data reader extractor.
        /// </summary>
        /// <typeparam name="T">&nbsp;the type of the data to return</typeparam>
        /// <param name="query">the parsed query</param>
        /// <param name="extractor">the data reader mapper used to extract data</param>
        /// <param name="parameterSource">a parameter source that holds the parameter values for the query</param>
        /// <returns>the extracted data</returns>
        public T Select<T>(ParsedQuery query, DataReaderExtractor<T> extractor, IQueryParameterSource parameterSource = null)
        {
            var table = new DataTable();
            using (var dataAdapter = ConnectionProvider.ProviderFactory.CreateDataAdapter())
            using (var command = GetCommand(query.SubstitutedQuery))
            {
                dataAdapter.SelectCommand = command;
                SetParameters(command, query, parameterSource);
                dataAdapter.Fill(table);
            }
            return extractor(new DataTableReader(table));
        }

        /// <summary>
        /// Executes a select query using a <see cref="DbDataAdapter"/> and returns a <see cref="DataSet"/>.
        /// </summary>
        /// <param name="query">the query</param>
        /// <param name="parameterSource">a parameter source that holds the parameter values for the query</param>
        /// <returns>a data set filled using the query</returns>
        public DataSet Select(string query, IQueryParameterSource parameterSource = null)
        {
            var parsedQuery = new ParsedQuery(query, ConnectionProvider.PlaceholderGetter);
            return Select(parsedQuery, parameterSource);
        }

        /// <summary>
        /// Executes a select query using a <see cref="DbDataAdapter"/> and returns a <see cref="DataSet"/>.
        /// </summary>
        /// <param name="query">the parsed query</param>
        /// <param name="parameterSource">a parameter source that holds the parameter values for the query</param>
        /// <returns>a data set filled using the query</returns>
        public DataSet Select(ParsedQuery query, IQueryParameterSource parameterSource = null)
        {
            var dataSet = new DataSet();
            using (var dataAdapter = ConnectionProvider.ProviderFactory.CreateDataAdapter())
            using (var command = GetCommand(query.SubstitutedQuery))
            {
                dataAdapter.SelectCommand = command;
                SetParameters(command, query, parameterSource);
                dataAdapter.Fill(dataSet);
            }
            return dataSet;
        }

        #endregion

        #region Update

        /// <summary>
        /// Executes an insert or update query on a database.
        /// </summary>
        /// <param name="query">the query to execute</param>
        /// <param name="parameters">the parameters of the query</param>
        /// <returns>the number of affected rows</returns>
        public int Update(string query, IDictionary<string, object> parameters)
        {
            return Update(query, new DictionaryParameterSource { Parameters = parameters });
        }

        /// <summary>
        /// Executes an insert or update query on a database.
        /// </summary>
        /// <param name="query">the query to execute</param>
        /// <param name="parameterSource">a parameter source that holds the parameter values for the query</param>
        /// <returns>the number of affected rows</returns>
        public int Update(string query, IQueryParameterSource parameterSource = null)
        {
            var parsedQuery = new ParsedQuery(query, ConnectionProvider.PlaceholderGetter);
            return Update(parsedQuery, parameterSource);
        }

        /// <summary>
        /// Executes an insert or update query on a database.
        /// </summary>
        /// <param name="query">the parsed query</param>
        /// <param name="parameterSource">a parameter source that holds the parameter values for the query</param>
        /// <returns>the number of affected rows</returns>
        public int Update(ParsedQuery query, IQueryParameterSource parameterSource = null)
        {
            using (var command = GetCommand(query.SubstitutedQuery))
            {
                SetParameters(command, query, parameterSource);
                return command.ExecuteNonQuery();
            }
        }

        #endregion

        #region Batch update

        /// <summary>
        /// Executes an insert or update command multiple times.
        /// </summary>
        /// <param name="query">the query to execute</param>
        /// <param name="parameters">a list containing the parameters for each execution of the query</param>
        /// <returns>an array containing the affected rows for each execution</returns>
        public int[] BatchUpdate(string query, IList<IDictionary<string, object>> parameters)
        {
            var parameterSources = from parameter in parameters select new DictionaryParameterSource { Parameters = parameter } as IQueryParameterSource;
            return BatchUpdate(new ParsedQuery(query, ConnectionProvider.PlaceholderGetter), parameterSources.ToList());
        }

        /// <summary>
        /// Executes an insert or update command multiple times.
        /// </summary>
        /// <param name="query">the query to execute</param>
        /// <param name="parameterSources">a list containing a <see cref="IQueryParameterSource"/> for each execution</param>
        /// <returns>an array containing the affected rows for each execution</returns>
        public int[] BatchUpdate(string query, IList<IQueryParameterSource> parameterSources)
        {
            var parsedQuery = new ParsedQuery(query, ConnectionProvider.PlaceholderGetter);
            return BatchUpdate(parsedQuery, parameterSources);
        }

        /// <summary>
        /// Executes an insert or update command multiple times.
        /// </summary>
        /// <param name="query">the parsed query to execute</param>
        /// <param name="parameterSources">a list containing a <see cref="IQueryParameterSource"/> for each execution</param>
        /// <returns>an array containing the affected rows for each execution</returns>
        public int[] BatchUpdate(ParsedQuery query, IList<IQueryParameterSource> parameterSources)
        {
            var affectedRows = new int[parameterSources.Count];

            using (var command = GetCommand(query.SubstitutedQuery))
            {
                // Define parameters
                var names = query.Named
                    ? query.ParameterNames.Distinct()
                    : Enumerable.Range(0, query.ParameterNames.Count).Select(i => i.ToString());
                foreach (var name in names)
                {
                    command.AddParameter(name);
                }
                // For each parameter source, set the parameter values and execute the command
                for (var i = 0; i < parameterSources.Count; i++)
                {
                    var parameterSource = parameterSources[i];
                    if (query.Named)
                    {
                        foreach (var name in query.ParameterNames.Distinct())
                        {
                            command.Parameters[name].Value = parameterSource[name];
                        }
                    }
                    else
                    {
                        for (var j = 0; j < query.ParameterNames.Count; j++)
                        {
                            var name = query.ParameterNames[j];
                            command.Parameters[j].Value = parameterSource[name];
                        }
                    }
                    affectedRows[i] = command.ExecuteNonQuery();
                }
            }

            return affectedRows;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Creates a command using the provider factory.
        /// </summary>
        /// <param name="query">the query of the command</param>
        /// <returns>a command of the appropriate provider type, with the given query</returns>
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        private DbCommand GetCommand(string query)
        {
            var command = ConnectionProvider.Connection.CreateCommand();
            command.CommandText = query;
            return command;
        }

        private static void SetParameters(IDbCommand command, ParsedQuery query, IQueryParameterSource parameterSource)
        {
            Assert.IsTrue(query.ParameterNames.Count == 0 || parameterSource != null, "The query has parameters but no parameter source was provided.");
            if (query.Named)
            {
                foreach (var name in query.ParameterNames.Distinct())
                {
                    command.AddParameter(name, parameterSource[name]);
                }
            }
            else
            {
                for (var i = 0; i < query.ParameterNames.Count; i++)
                {
                    var name = query.ParameterNames[i];
                    command.AddParameter(i.ToString(), parameterSource[name]);
                }
            }
        }

        /// <summary>
        /// Creates a data reader mapper that extracts a list of elements, one for each row, using a row mapper.
        /// </summary>
        /// <typeparam name="T">&nbsp;the type of the elements returned</typeparam>
        /// <param name="rowMapper">the mapper creating the elements</param>
        /// <returns>a data reader extractor</returns>
        private static DataReaderExtractor<IList<T>> GetDataReaderExtractor<T>(RowMapper<T> rowMapper)
        {
            return dataReader =>
            {
                IList<T> result = new List<T>();
                int i = 0;
                while(dataReader.Read())
                {
                    result.Add(rowMapper(dataReader, i));
                    i++;
                }
                return result;
            };
        }

        /// <summary>
        /// Creates a data reader extractor that delegates each row to a row handler
        /// </summary>
        /// <param name="rowHandler">a row handler that will be executed on each row</param>
        /// <returns>a data reader extractor</returns>
        private static DataReaderExtractor<object> GetDataReaderExtractor(RowHandler rowHandler)
        {
            return dataReader =>
            {
                while (dataReader.Read())
                {
                    rowHandler(dataReader);
                }
                return null;
            };
        }

        #endregion
    }

}
