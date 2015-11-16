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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Summer.Batch.CoreTests.Core.Repository.Dao
{
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable"), TestClass]
    public class MapStepExecutionDaoTest
    {
        private MapStepExecutionDao _stepExecutionDao;
        private JobExecution _jobExecution;

        [TestInitialize]
        public void Initialize()
        {
            _stepExecutionDao = new MapStepExecutionDao();
            _jobExecution = new JobExecution(1L);
        }

        [TestMethod]
        public void TestSaveStepExecution()
        {
            var stepExecution = new StepExecution("testStep", _jobExecution);

            _stepExecutionDao.SaveStepExecution(stepExecution);

            Assert.AreEqual(1L, stepExecution.Id);
            Assert.AreEqual(0, stepExecution.Version);
        }

        [TestMethod]
        public void TestSaveStepExecutions()
        {
            var stepExecution1 = new StepExecution("testStep", _jobExecution);
            var stepExecution2 = new StepExecution("testStep", _jobExecution);
            ICollection<StepExecution> executions = new List<StepExecution>();
            executions.Add(stepExecution1);
            executions.Add(stepExecution2);

            _stepExecutionDao.SaveStepExecutions(executions);

            Assert.AreEqual(1L, stepExecution1.Id);
            Assert.AreEqual(0, stepExecution1.Version);
            Assert.AreEqual(2L, stepExecution2.Id);
            Assert.AreEqual(0, stepExecution2.Version);
        }

        [TestMethod]
        public void TestUpdateStepExecution1()
        {
            var stepExecution = new StepExecution("testStep", _jobExecution);
            _stepExecutionDao.SaveStepExecution(stepExecution);
            stepExecution.BatchStatus = BatchStatus.Completed;

            _stepExecutionDao.UpdateStepExecution(stepExecution);
            var persisted = _stepExecutionDao.GetStepExecution(_jobExecution, 1);

            Assert.AreEqual(1, stepExecution.Version);
            Assert.AreEqual(BatchStatus.Completed, persisted.BatchStatus);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestUpdateStepExecution2()
        {
            var stepExecution = new StepExecution("testStep", _jobExecution);
            _stepExecutionDao.SaveStepExecution(stepExecution);
            stepExecution.Version = 1;

            _stepExecutionDao.UpdateStepExecution(stepExecution);
        }

        [TestMethod]
        public void TestAddStepExecutions1()
        {
            var stepExecution = new StepExecution("testStep", _jobExecution);
            _stepExecutionDao.SaveStepExecution(stepExecution);

            _stepExecutionDao.AddStepExecutions(_jobExecution);

            Assert.AreEqual(1, _jobExecution.StepExecutions.Count);
            Assert.IsTrue(_jobExecution.StepExecutions.Contains(stepExecution));
        }

        [TestMethod]
        public void TestAddStepExecutions2()
        {
            _stepExecutionDao.AddStepExecutions(_jobExecution);

            Assert.AreEqual(0, _jobExecution.StepExecutions.Count);
        }
    }
}
