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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Summer.Batch.Core;
using Summer.Batch.Core.Repository.Dao;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Summer.Batch.CoreTests.Core.Repository.Dao
{
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable"), TestClass]
    public class MapJobExecutionDaoTest
    {
        private MapJobExecutionDao _jobExecutionDao;
        private JobInstance _instance;
        private JobParameters _parameters;
        private JobExecution _execution;

        [TestInitialize]
        public void Initialize()
        {
            _jobExecutionDao = new MapJobExecutionDao();
            _instance = new JobInstance(1, "testJob");
            _parameters = new JobParameters();
            _execution = new JobExecution(_instance, _parameters);
        }

        [TestMethod]
        public void TestSaveJobExecution()
        {
            _jobExecutionDao.SaveJobExecution(_execution);

            Assert.AreEqual(1L, _execution.Id);
            Assert.AreEqual(_execution, _jobExecutionDao.GetJobExecution(1L));
        }

        [TestMethod]
        public void TestUpdateJobExecution()
        {
            _jobExecutionDao.SaveJobExecution(_execution);

            _jobExecutionDao.UpdateJobExecution(_execution);

            Assert.AreEqual(1L, _execution.Id);
            Assert.AreEqual(_execution, _jobExecutionDao.GetJobExecution(1L));
            Assert.AreEqual(1, _execution.Version);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestUpdateJobExecutionException1()
        {
            _jobExecutionDao.SaveJobExecution(_execution);

            _execution.Version = 2;
            _jobExecutionDao.UpdateJobExecution(_execution);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestUpdateJobExecutionException2()
        {
            _jobExecutionDao.UpdateJobExecution(_execution);
        }

        [TestMethod]
        public void TestFindJobExecutions()
        {
            var execution2 = new JobExecution(_instance, _parameters);
            var instance2 = new JobInstance(2, "testJob2");
            var execution3 = new JobExecution(instance2, _parameters);
            _jobExecutionDao.SaveJobExecution(_execution);
            _jobExecutionDao.SaveJobExecution(execution2);
            _jobExecutionDao.SaveJobExecution(execution3);

            var executions = _jobExecutionDao.FindJobExecutions(_instance);

            Assert.AreEqual(2, executions.Count);
            Assert.IsTrue(executions.Contains(_execution));
            Assert.IsTrue(executions.Contains(execution2));
        }

        [TestMethod]
        public void TestGetLastJobExecution()
        {
            var execution2 = new JobExecution(_instance, _parameters);
            _execution.CreateTime = DateTime.Now;
            _jobExecutionDao.SaveJobExecution(_execution);
            execution2.CreateTime = DateTime.Now.AddMinutes(1);
            _jobExecutionDao.SaveJobExecution(execution2);

            var lastExecution = _jobExecutionDao.GetLastJobExecution(_instance);

            Assert.AreEqual(execution2, lastExecution);
        }

        [TestMethod]
        public void TestFindRunningJobExecutions()
        {
            var instance2 = new JobInstance(2, "testJob2");
            var execution2 = new JobExecution(instance2, _parameters);
            _jobExecutionDao.SaveJobExecution(_execution);
            _jobExecutionDao.SaveJobExecution(execution2);

            var runningExecutions = _jobExecutionDao.FindRunningJobExecutions("testJob");

            Assert.AreEqual(1, runningExecutions.Count);
            Assert.IsTrue(runningExecutions.Contains(_execution));
        }

        [TestMethod]
        public void TestGetJobExecution1()
        {
            _jobExecutionDao.SaveJobExecution(_execution);
            Assert.AreEqual(_execution, _jobExecutionDao.GetJobExecution(1L));
        }

        [TestMethod]
        public void TestGetJobExecution2()
        {
            Assert.IsNull(_jobExecutionDao.GetJobExecution(1L));
        }

        [TestMethod]
        public void TestSynchronizeStatus()
        {
            _jobExecutionDao.SaveJobExecution(_execution);
            var execution2 = new JobExecution(_execution);
            _execution.Status = BatchStatus.Completed;
            _execution.EndTime = DateTime.Now;
            _jobExecutionDao.UpdateJobExecution(_execution);

            _jobExecutionDao.SynchronizeStatus(execution2);

            Assert.AreEqual(BatchStatus.Completed, execution2.Status);
            Assert.AreEqual(_execution.Version, execution2.Version);
            Assert.AreNotEqual(_execution.EndTime, execution2.EndTime);
        }
    }
}
