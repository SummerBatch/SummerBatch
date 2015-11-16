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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Summer.Batch.Data;
using Summer.Batch.Data.Parameter;

namespace Summer.Batch.CoreTests.Data
{
    [TestClass]
    public class ParsedQueryTest
    {
        [TestMethod]
        public void TestParsedQuery1()
        {
            var parsedQuery = new ParsedQuery("INSERT into BATCH_JOB_INSTANCE(JOB_INSTANCE_ID, JOB_NAME, JOB_KEY, VERSION) values (@id, @jobName, @jobKey, @version)", new PlaceholderGetter(name => ":" + name, true));

            Assert.AreEqual("INSERT into BATCH_JOB_INSTANCE(JOB_INSTANCE_ID, JOB_NAME, JOB_KEY, VERSION) values (:id, :jobName, :jobKey, :version)",
                parsedQuery.SubstitutedQuery);
            Assert.IsTrue(parsedQuery.ParameterNames.SequenceEqual(new List<string> { "id", "jobName", "jobKey", "version" }));
        }


        [TestMethod]
        public void TestParsedQuery2()
        {
            var parsedQuery = new ParsedQuery("SELECT JOB_EXECUTION_ID, START_TIME, END_TIME, STATUS, EXIT_CODE, EXIT_MESSAGE, CREATE_TIME, LAST_UPDATED, VERSION, JOB_CONFIGURATION_LOCATION from BATCH_JOB_EXECUTION E where JOB_INSTANCE_ID = @id and JOB_EXECUTION_ID in (SELECT max(JOB_EXECUTION_ID) from BATCH_JOB_EXECUTION E2 where E2.JOB_INSTANCE_ID = @id)",
                new PlaceholderGetter(name => "?", false));

            Assert.AreEqual("SELECT JOB_EXECUTION_ID, START_TIME, END_TIME, STATUS, EXIT_CODE, EXIT_MESSAGE, CREATE_TIME, LAST_UPDATED, VERSION, JOB_CONFIGURATION_LOCATION from BATCH_JOB_EXECUTION E where JOB_INSTANCE_ID = ? and JOB_EXECUTION_ID in (SELECT max(JOB_EXECUTION_ID) from BATCH_JOB_EXECUTION E2 where E2.JOB_INSTANCE_ID = ?)",
                parsedQuery.SubstitutedQuery);
            Assert.IsTrue(parsedQuery.ParameterNames.SequenceEqual(new List<string> { "id", "id" }));
        }

        [TestMethod]
        public void TestParsedQuery3()
        {
            var parsedQuery = new ParsedQuery("SELECT SHORT_CONTEXT, SERIALIZED_CONTEXT FROM {0}JOB_EXECUTION_CONTEXT WHERE JOB_EXECUTION_ID = :id",
                new PlaceholderGetter(name => "?", false));

            Assert.AreEqual("SELECT SHORT_CONTEXT, SERIALIZED_CONTEXT FROM {0}JOB_EXECUTION_CONTEXT WHERE JOB_EXECUTION_ID = ?",
                parsedQuery.SubstitutedQuery);
            Assert.IsTrue(parsedQuery.ParameterNames.SequenceEqual(new List<string> { "id" }));
        }
    }
}