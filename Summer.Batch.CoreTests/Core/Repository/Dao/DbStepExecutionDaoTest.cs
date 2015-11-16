using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Summer.Batch.Core;
using Summer.Batch.Core.Repository.Dao;
using Summer.Batch.Data.Incrementer;
using Summer.Batch.Data.Parameter;

namespace Summer.Batch.CoreTests.Core.Repository.Dao
{
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable"), TestClass]
    
    public class DbStepExecutionDaoTest : DaoTestHelper
    {

        private DbStepExecutionDao _stepExecutionDao;
        private JobExecution _jobExecution;

        [TestInitialize]
        public new void Initialize()
        {
            base.Initialize();
            _stepExecutionDao = new DbStepExecutionDao
            {
                DbOperator = DbOperator,
                PlaceholderGetter = new PlaceholderGetter(name => "@" + name, true),
                StepIncrementer = new SqlServerIncrementer
                {
                    ConnectionStringSettings = ConnectionStringSettings,
                    IncrementerName = "BATCH_STEP_EXECUTION_SEQ",
                    ColumnName = "ID"
                }
            };
            _jobExecution = new JobExecution(1);
        }

        [TestCleanup]
        public void CleanUp()
        {
            Clean();
        }

        [TestMethod]
        public void TestGetStepExecution()
        {
            Insert(@"TestData\DbDao\StepExecutionTestData1.xml");

            var execution = _stepExecutionDao.GetStepExecution(_jobExecution, 1);

            Assert.IsNull(execution);
        }

        [TestMethod]
        public void TestSaveStepExecution()
        {
            ResetSequence("BATCH_STEP_EXECUTION_SEQ");
            Insert(@"TestData\DbDao\StepExecutionTestData1.xml");
            var stepExecution = new StepExecution("TestStep", _jobExecution);

            _stepExecutionDao.SaveStepExecution(stepExecution);

            Assert.AreEqual(1, stepExecution.Id);
            Assert.AreEqual(0, stepExecution.Version);
        }

        [TestMethod]
        public void TestSaveStepExecutions()
        {
            ResetSequence("BATCH_STEP_EXECUTION_SEQ");
            Insert(@"TestData\DbDao\StepExecutionTestData1.xml");
            var stepExecution1 = new StepExecution("TestStep", _jobExecution);
            var stepExecution2 = new StepExecution("TestStep", _jobExecution);
            ICollection<StepExecution> executions = new List<StepExecution>();
            executions.Add(stepExecution1);
            executions.Add(stepExecution2);

            _stepExecutionDao.SaveStepExecutions(executions);

            Assert.AreEqual(1, stepExecution1.Id);
            Assert.AreEqual(0, stepExecution1.Version);
            Assert.AreEqual(2, stepExecution2.Id);
            Assert.AreEqual(0, stepExecution2.Version);
        }

        [TestMethod]
        public void TestUpdateStepExecution1()
        {
            Insert(@"TestData\DbDao\StepExecutionTestData2.xml");
            var stepExecution = new StepExecution("TestStep", _jobExecution, 1)
            {
                BatchStatus = BatchStatus.Completed,
                Version = 0
            };

            _stepExecutionDao.UpdateStepExecution(stepExecution);
            var persisted = _stepExecutionDao.GetStepExecution(_jobExecution, 1);

            Assert.AreEqual(1, stepExecution.Version);
            Assert.AreEqual(BatchStatus.Completed, persisted.BatchStatus);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestUpdateStepExecution2()
        {
            Insert(@"TestData\DbDao\StepExecutionTestData2.xml");
            var stepExecution = new StepExecution("TestStep", _jobExecution) { Version = 1 };

            _stepExecutionDao.UpdateStepExecution(stepExecution);
        }

        [TestMethod]
        public void TestAddStepExecutions1()
        {
            Insert(@"TestData\DbDao\StepExecutionTestData2.xml");
            var stepExecution = new StepExecution("TestStep", _jobExecution, 1);

            _stepExecutionDao.AddStepExecutions(_jobExecution);

            Assert.AreEqual(1, _jobExecution.StepExecutions.Count);
            Assert.IsTrue(_jobExecution.StepExecutions.Contains(stepExecution));
        }

        [TestMethod]
        public void TestAddStepExecutions2()
        {
            Insert(@"TestData\DbDao\StepExecutionTestData1.xml");

            _stepExecutionDao.AddStepExecutions(_jobExecution);

            Assert.AreEqual(0, _jobExecution.StepExecutions.Count);
        }
    }
}