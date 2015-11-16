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
using Summer.Batch.Core.Scope.Context;
using Summer.Batch.Core.Unity.StepScope;

namespace Summer.Batch.CoreTests.StepScope
{
    [TestClass]
    public class StepScopeLifetimeManagerTest
    {
        private JobInstance _jobInstance;
        private JobExecution _jobExecution;
        private StepExecution _stepExecution1;
        private StepExecution _stepExecution2;

        [TestInitialize]
        public void Initialize()
        {
            _jobInstance = new JobInstance(1, "testJob");
            _jobExecution = new JobExecution(_jobInstance, new JobParameters());
            _stepExecution1 = new StepExecution("testStep1", _jobExecution, 1);
            _stepExecution2 = new StepExecution("testStep2", _jobExecution, 2);
        }

        [TestMethod]
        public void TestGetValue()
        {
            var manager = new StepScopeLifetimeManager();
            StepSynchronizationManager.Register(_stepExecution1);

            var result = manager.GetValue();

            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestSetValue1()
        {
            var obj = new object();
            var manager = new StepScopeLifetimeManager();
            StepSynchronizationManager.Register(_stepExecution1);
            manager.SetValue(obj);

            var result = manager.GetValue();

            Assert.AreEqual(obj, result);
        }

        [TestMethod]
        public void TestSetValue2()
        {
            var obj = new object();
            var manager = new StepScopeLifetimeManager();
            StepSynchronizationManager.Register(_stepExecution1);
            manager.SetValue(obj);
            StepSynchronizationManager.Register(_stepExecution2);

            var result = manager.GetValue();

            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestSetValue3()
        {
            var obj1 = new object();
            var obj2 = new object();
            var manager = new StepScopeLifetimeManager();
            StepSynchronizationManager.Register(_stepExecution1);
            manager.SetValue(obj1);
            StepSynchronizationManager.Register(_stepExecution2);
            manager.SetValue(obj2);

            var result = manager.GetValue();

            Assert.AreEqual(obj2, result);
        }
    }
}