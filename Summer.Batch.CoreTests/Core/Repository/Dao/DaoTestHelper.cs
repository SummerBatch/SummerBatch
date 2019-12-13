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
using NDbUnit.Core;
using NDbUnit.Core.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.IO;
using Summer.Batch.Data;

namespace Summer.Batch.CoreTests.Core.Repository.Dao
{
    public abstract class DaoTestHelper
    {
        private const string ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\TestDB.mdf;Integrated Security=True;Connect Timeout=30";
        private const string ProviderName = "System.Data.SqlClient";
        private const string XmlSchema = "TestDBDataSet.xsd";

        protected static readonly ConnectionStringSettings ConnectionStringSettings = new ConnectionStringSettings
        {
            ConnectionString = ConnectionString,
            ProviderName = ProviderName
        };

        protected readonly DbOperator DbOperator = new DbOperator
        {
            ConnectionProvider = new ConnectionProvider
            {
                ConnectionStringSettings = ConnectionStringSettings
            }
        };

        private NDbUnitTest _unitTest;

        protected void Initialize()
        {
            _unitTest = new SqlDbUnitTest(ConnectionString);
            _unitTest.ReadXmlSchema(XmlSchema);
        }

        protected void Clean()
        {
            _unitTest.PerformDbOperation(DbOperationFlag.DeleteAll);
        }

        protected void Insert(string testData)
        {
            _unitTest.ReadXml(testData);
            _unitTest.PerformDbOperation(DbOperationFlag.CleanInsertIdentity);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        protected void ResetSequence(string sequence)
        {
            DbOperator.Update(string.Format("drop table {0};", sequence));
            DbOperator.Update(string.Format("create table {0} (id bigint identity);", sequence));
        }
    }
}