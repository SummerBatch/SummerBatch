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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Summer.Batch.Core;
using Summer.Batch.Core.Repository.Dao;
using Summer.Batch.Data.Incrementer;
using Summer.Batch.Data.Parameter;

namespace Summer.Batch.CoreTests.Core.Repository.Dao
{
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable"), TestClass]
    
    public class DbJobExecutionDaoTest : DaoTestHelper
    {
        private DbJobExecutionDao _jobExecutionDao;
        private JobInstance _instance;
        private JobParameters _parameters;
        private JobExecution _execution;

        [TestInitialize]
        public new void Initialize()
        {
            base.Initialize();
            _jobExecutionDao = new DbJobExecutionDao
            {
                DbOperator = DbOperator,
                PlaceholderGetter = new PlaceholderGetter(name => "@" + name, true),
                JobIncrementer = new SqlServerIncrementer
                {
                    IncrementerName = "BATCH_JOB_EXECUTION_SEQ",
                    ConnectionStringSettings = ConnectionStringSettings,
                    ColumnName = "ID"
                }
            };
            _instance = new JobInstance(1, "TestJob");
            _parameters = new JobParameters();
            _execution = new JobExecution(_instance, _parameters);
        }

        [TestCleanup]
        public void CleanUp()
        {
            Clean();
        }

        [TestMethod]
        public void TestSaveJobExecution()
        {
            ResetSequence("BATCH_JOB_EXECUTION_SEQ");
            Insert(@"TestData\DbDao\JobExecutionTestData1.xml");
            var dictionary = new Dictionary<string, JobParameter>();
            dictionary["string"] = new JobParameter("string");
            dictionary["long"] = new JobParameter(3);
            dictionary["double"] = new JobParameter(4.3);
            dictionary["date"] = new JobParameter(DateTime.Now);
            _parameters = new JobParameters(dictionary);
            _execution = new JobExecution(_instance, _parameters);

            _jobExecutionDao.SaveJobExecution(_execution);

            Assert.AreEqual(1L, _execution.Id);
            Assert.AreEqual(_execution, _jobExecutionDao.GetJobExecution(1L));
        }

        [TestMethod]
        public void TestUpdateJobExecution()
        {
            Insert(@"TestData\DbDao\JobExecutionTestData2.xml");
            _execution.Id = 1;
            _execution.Version = 0;

            _jobExecutionDao.UpdateJobExecution(_execution);

            Assert.AreEqual(1L, _execution.Id);
            Assert.AreEqual(_execution, _jobExecutionDao.GetJobExecution(1L));
            Assert.AreEqual(1, _execution.Version);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestUpdateJobExecutionException1()
        {
            Insert(@"TestData\DbDao\JobExecutionTestData2.xml");
            _execution.Id = 1;
            _execution.Version = 2;

            _jobExecutionDao.UpdateJobExecution(_execution);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestUpdateJobExecutionException2()
        {
            _jobExecutionDao.UpdateJobExecution(_execution);
        }

        [TestMethod]
        public void TestFindJobExecutions()
        {
            Insert(@"TestData\DbDao\JobExecutionTestData3.xml");

            var executions = _jobExecutionDao.FindJobExecutions(_instance);

            Assert.AreEqual(2, executions.Count);
            var createTimes = (from execution in executions select execution.CreateTime).ToList();
            Assert.IsTrue(createTimes.Contains(new DateTime(2015, 5, 19, 18, 2, 0)));
            Assert.IsTrue(createTimes.Contains(new DateTime(2015, 5, 20, 9, 31, 0)));
        }

        [TestMethod]
        public void TestGetLastJobExecution1()
        {
            Insert(@"TestData\DbDao\JobExecutionTestData3.xml");

            var lastExecution = _jobExecutionDao.GetLastJobExecution(_instance);

            Assert.IsNotNull(lastExecution);
            Assert.AreEqual(new DateTime(2015, 5, 20, 9, 31, 0), lastExecution.CreateTime);
        }



        [TestMethod]
        public void TestGetLastJobExecution2()
        {
            Insert(@"TestData\DbDao\JobExecutionTestData1.xml");

            var lastExecution = _jobExecutionDao.GetLastJobExecution(_instance);

            Assert.IsNull(lastExecution);
        }

        [TestMethod]
        public void TestFindRunningJobExecutions()
        {
            Insert(@"TestData\DbDao\JobExecutionTestData4.xml");

            var runningExecutions = _jobExecutionDao.FindRunningJobExecutions("TestJob");

            Assert.AreEqual(1, runningExecutions.Count);
        }

        [TestMethod]
        public void TestGetJobExecution1()
        {
            Insert(@"TestData\DbDao\JobExecutionTestData5.xml");

            var execution = _jobExecutionDao.GetJobExecution(1);

            Assert.IsNotNull(execution);
            Assert.AreEqual("string", execution.JobParameters.GetString("string"));
            Assert.AreEqual(3, execution.JobParameters.GetLong("long"));
            Assert.AreEqual(4.3, execution.JobParameters.GetDouble("double"));
            Assert.AreEqual(new DateTime(2015, 5, 20, 11, 52, 28), execution.JobParameters.GetDate("date"));
        }

        [TestMethod]
        public void TestGetJobExecution2()
        {
            Assert.IsNull(_jobExecutionDao.GetJobExecution(1L));
        }

        [TestMethod]
        public void TestSynchronizeStatus()
        {
            Insert(@"TestData\DbDao\JobExecutionTestData6.xml");
            _execution.Id = 1;
            _execution.Version = 0;

            _jobExecutionDao.SynchronizeStatus(_execution);

            Assert.AreEqual(BatchStatus.Completed, _execution.Status);
            Assert.AreEqual(1, _execution.Version);
            Assert.IsNull(_execution.EndTime);
        }
    }
}