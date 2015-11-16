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
using Summer.Batch.Infrastructure.Item;
using System.Collections.Generic;

namespace Summer.Batch.CoreTests.Core.Repository.Dao
{
    [TestClass]
    public class MapExecutionContextDaoTest
    {
        private MapExecutionContextDao _executionContextDao;

        [TestInitialize]
        public void Initialize()
        {
            _executionContextDao = new MapExecutionContextDao();
        }

        [TestMethod]
        public void TestSaveExecutionContextJob1()
        {
            var execution = new JobExecution(1);
            var originalContext = new ExecutionContext();
            execution.ExecutionContext = originalContext;

            _executionContextDao.SaveExecutionContext(execution);
            var context = _executionContextDao.GetExecutionContext(execution);

            Assert.AreEqual(originalContext, context);
        }

        [TestMethod]
        public void TestSaveExecutionContextJob2()
        {
            var execution = new JobExecution(1);
            var originalContext = new ExecutionContext();
            execution.ExecutionContext = originalContext;
            originalContext.PutString("test1", "test1");
            originalContext.PutLong("test2", 2L);

            _executionContextDao.SaveExecutionContext(execution);
            var context = _executionContextDao.GetExecutionContext(execution);

            Assert.AreEqual(originalContext, context);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void TestClear()
        {
            var execution = new JobExecution(1);
            var originalContext = new ExecutionContext();
            execution.ExecutionContext = originalContext;

            _executionContextDao.SaveExecutionContext(execution);
            _executionContextDao.Clear();
            _executionContextDao.GetExecutionContext(execution);
        }

        [TestMethod]
        public void TestSaveExecutionContextStep1()
        {
            var jobExecution = new JobExecution(1);
            var execution = new StepExecution("testStep", jobExecution, 2);
            var originalContext = new ExecutionContext();
            execution.ExecutionContext = originalContext;

            _executionContextDao.SaveExecutionContext(execution);
            var context = _executionContextDao.GetExecutionContext(execution);

            Assert.AreEqual(originalContext, context);
        }

        [TestMethod]
        public void TestSaveExecutionContextStep2()
        {
            var jobExecution = new JobExecution(1);
            var execution = new StepExecution("testStep", jobExecution, 2);
            var originalContext = new ExecutionContext();
            execution.ExecutionContext = originalContext;
            originalContext.PutString("test1", "test1");
            originalContext.PutLong("test2", 2L);

            _executionContextDao.SaveExecutionContext(execution);
            var context = _executionContextDao.GetExecutionContext(execution);

            Assert.AreEqual(originalContext, context);
        }

        [TestMethod]
        public void TestSaveExecutionContexts()
        {
            var jobExecution = new JobExecution(1);
            var execution1 = new StepExecution("testStep", jobExecution, 1);
            var execution2 = new StepExecution("testStep", jobExecution, 2);
            execution1.ExecutionContext = new ExecutionContext();
            execution2.ExecutionContext = new ExecutionContext();
            execution2.ExecutionContext.PutString("test1", "test1");
            execution2.ExecutionContext.PutLong("test2", 2L);

            IList<StepExecution> executions = new List<StepExecution>();
            executions.Add(execution1);
            executions.Add(execution2);
            _executionContextDao.SaveExecutionContexts(executions);
            var context1 = _executionContextDao.GetExecutionContext(execution1);
            var context2 = _executionContextDao.GetExecutionContext(execution2);

            Assert.AreEqual(execution1.ExecutionContext, context1);
            Assert.AreEqual(execution2.ExecutionContext, context2);
        }
    }
}
