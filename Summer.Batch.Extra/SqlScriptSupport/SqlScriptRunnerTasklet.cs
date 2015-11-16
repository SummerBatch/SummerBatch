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
using NLog;
using System.Configuration;
using System.Data.Common;
using System.Text;
using Summer.Batch.Common.Factory;
using Summer.Batch.Core;
using Summer.Batch.Core.Scope.Context;
using Summer.Batch.Core.Step.Tasklet;
using Summer.Batch.Common.IO;
using Summer.Batch.Common.Util;
using Summer.Batch.Infrastructure.Repeat;

namespace Summer.Batch.Extra.SqlScriptSupport
{
    /// <summary>
    /// This tasklet launches a sql script.
    /// </summary>
    public class SqlScriptRunnerTasklet : ITasklet, IInitializationPostOperations
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The script resource file
        /// </summary>
        public IResource Resource { private get; set; }

        private DbProviderFactory _providerFactory;

        /// <summary>
        /// The connection string to the database
        /// </summary>
        private string _connectionString;

        /// <summary>
        /// Connection String settings property.
        /// </summary>
        public ConnectionStringSettings ConnectionStringSettings
        {
            set
            {
                _providerFactory = DbProviderFactories.GetFactory(value.ProviderName);
                _connectionString = value.ConnectionString;
            }
        }

        /// <summary>
        /// Checks that:
        /// <list type="bullet">
        ///     <item>
        ///         <description>
        ///             <see cref="Resource"/> is not null and that it exists
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <description>
        ///             a connection string and a provider have been specified via <see cref="ConnectionStringSettings"/>
        ///         </description>
        ///     </item>
        /// </list>
        /// </summary>
        public void AfterPropertiesSet()
        {
            Assert.NotNull(Resource, "Resource must be set.");
            Assert.IsTrue(Resource.Exists(), "Resource must exist.");
            Assert.HasLength(_connectionString, "A connection string must be specified using ConnectionStringSettings.");
            Assert.NotNull(_providerFactory, "A provider must be specified using ConnectionStringSettings.");
        }

        /// <summary>
        /// Execute the sql script in the resource file.
        /// @see ITasklet#Execute
        /// </summary>
        /// <param name="contribution"></param>
        /// <param name="chunkContext"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public RepeatStatus Execute(StepContribution contribution, ChunkContext chunkContext)
        {
            // open connection
            using (DbConnection connection = _providerFactory.CreateConnection())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();
                string preparedCommand = PrepareCommands(Resource);
                DbCommand command = connection.CreateCommand();
                command.CommandText = preparedCommand;
                int sqlDone = command.ExecuteNonQuery();
                if(Logger.IsTraceEnabled)
                {
                    Logger.Trace("SQL script execution end with {0} return code", sqlDone);
                }
            }
            return RepeatStatus.Finished;
        }

        /// <summary>
        /// Extract significant script lines to be executed.
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        private string PrepareCommands(IResource resource)
        {
            StringBuilder query = new StringBuilder();
            string line;

            System.IO.StreamReader file = new System.IO.StreamReader(resource.GetFileInfo().FullName);
            while ((line = file.ReadLine()) != null)
            {
                line = line.Trim();
                if (!IsComment(line))
                {
                    query.Append(line);
                }
            }
            return query.ToString();
        }

        /// <summary>
        /// Detects comments in a line.
        /// </summary>
        /// <param name="line">the line to analyze</param>
        /// <returns>whether it is comment</returns>
        private static bool IsComment(string line)
        {
            if (!string.IsNullOrEmpty(line))
            {
                return (line[0] == '#') || (line.StartsWith("--"));
            }
            return false;
        }
    }
}