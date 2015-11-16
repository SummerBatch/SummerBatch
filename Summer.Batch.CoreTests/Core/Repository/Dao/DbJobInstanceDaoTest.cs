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
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Summer.Batch.Core;
using Summer.Batch.Core.Launch;
using Summer.Batch.Core.Repository.Dao;
using Summer.Batch.Data.Incrementer;
using Summer.Batch.Data.Parameter;

namespace Summer.Batch.CoreTests.Core.Repository.Dao
{
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable"), TestClass]
    
    public class DbJobInstanceDaoTest : DaoTestHelper
    {
        private DbJobInstanceDao _jobInstanceDao;
        private JobParameters _parameters;

        [TestInitialize]
        public new void Initialize()
        {
            base.Initialize();
            _jobInstanceDao = new DbJobInstanceDao
            {
                DbOperator = DbOperator,
                PlaceholderGetter = new PlaceholderGetter(name => "@" + name, true),
                JobIncrementer = new SqlServerIncrementer
                {
                    IncrementerName = "BATCH_JOB_SEQ",
                    ConnectionStringSettings = ConnectionStringSettings,
                    ColumnName = "ID"
                }
            };
            _parameters = new JobParameters();
        }

        [TestCleanup]
        public void CleanUp()
        {
            Clean();
        }

        [TestMethod]
        public void TestCreateJobInstance1()
        {
            ResetSequence("BATCH_JOB_SEQ");
            Clean();

            var instance = _jobInstanceDao.CreateJobInstance("testJob", _parameters);

            Assert.IsNotNull(instance);
            Assert.AreEqual(1, instance.Id);
            Assert.AreEqual(0, instance.Version);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestCreateJobInstance2()
        {
            Insert(@"TestData\DbDao\JobInstanceTestData1.xml");

            _jobInstanceDao.CreateJobInstance("testJob", _parameters);
        }

        [TestMethod]
        public void TestGetJobInstanceParameters()
        {
            Insert(@"TestData\DbDao\JobInstanceTestData1.xml");
            
            var instance = _jobInstanceDao.GetJobInstance("testJob", _parameters);

            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void TestGetJobInstanceId1()
        {
            Insert(@"TestData\DbDao\JobInstanceTestData1.xml");

            var instance = _jobInstanceDao.GetJobInstance(1L);

            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void TestGetJobInstanceId2()
        {
            Clean();

            var instance = _jobInstanceDao.GetJobInstance(1L);

            Assert.IsNull(instance);
        }

        [TestMethod]
        public void TestGetJobInstanceJobExecution()
        {
            Insert(@"TestData\DbDao\JobInstanceTestData1.xml");
            var jobExecution = new JobExecution(1L);

            var instance = _jobInstanceDao.GetJobInstance(jobExecution);

            Assert.IsNotNull(instance);
            Assert.AreEqual("TestJob", instance.JobName);
        }

        [TestMethod]
        public void TestGetJobInstances()
        {
            Insert(@"TestData\DbDao\JobInstanceTestData2.xml");

            var instances = _jobInstanceDao.GetJobInstances("TestJob", 1, 2);

            Assert.AreEqual(2, instances.Count);
        }

        [TestMethod]
        public void TestGetJobNames()
        {
            Insert(@"TestData\DbDao\JobInstanceTestData3.xml");

            var names = _jobInstanceDao.GetJobNames();

            Assert.AreEqual(4, names.Count);
            Assert.AreEqual("TestJob1", names[0]);
            Assert.AreEqual("TestJob2", names[1]);
            Assert.AreEqual("TestJob3", names[2]);
            Assert.AreEqual("TestJob4", names[3]);
        }

        [TestMethod]
        public void TestGetJobInstanceCount1()
        {
            Insert(@"TestData\DbDao\JobInstanceTestData3.xml");

            Assert.AreEqual(2, _jobInstanceDao.GetJobInstanceCount("testJob1"));
            Assert.AreEqual(1, _jobInstanceDao.GetJobInstanceCount("testJob2"));
        }

        [TestMethod]
        [ExpectedException(typeof(NoSuchJobException))]
        public void TestGetJobInstanceCount2()
        {
            _jobInstanceDao.GetJobInstanceCount("testJob");
        }
    }
}
