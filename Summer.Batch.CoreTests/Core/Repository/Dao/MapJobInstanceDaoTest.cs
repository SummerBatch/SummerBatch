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
using Summer.Batch.Core.Launch;
using Summer.Batch.Core.Repository.Dao;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Summer.Batch.Common.Collections;

namespace Summer.Batch.CoreTests.Core.Repository.Dao
{
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable"), TestClass]
    public class MapJobInstanceDaoTest
    {
        private MapJobInstanceDao _jobInstanceDao;
        JobParameters _parameters;

        [TestInitialize]
        public void Initialize()
        {
            _jobInstanceDao = new MapJobInstanceDao();
            _parameters = new JobParameters();
        }

        [TestMethod]
        public void TestCreateJobInstance1()
        {
            var instance = _jobInstanceDao.CreateJobInstance("testJob", _parameters);

            Assert.IsNotNull(instance);
            Assert.AreEqual(0, instance.Version);
            Assert.AreEqual(1L, instance.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestCreateJobInstance2()
        {
            _jobInstanceDao.CreateJobInstance("testJob", _parameters);
            _jobInstanceDao.CreateJobInstance("testJob", _parameters);
        }

        [TestMethod]
        public void TestGetJobInstanceParameters()
        {
            var instance = _jobInstanceDao.CreateJobInstance("testJob", _parameters);
            var instance2 = _jobInstanceDao.GetJobInstance("testJob", _parameters);

            Assert.AreEqual(instance, instance2);
        }

        [TestMethod]
        public void TestGetJobInstanceId1()
        {
            var instance = _jobInstanceDao.CreateJobInstance("testJob", _parameters);
            var instance2 = _jobInstanceDao.GetJobInstance(1L);

            Assert.AreEqual(instance, instance2);
        }

        [TestMethod]
        public void TestGetJobInstanceId2()
        {
            var instance = _jobInstanceDao.GetJobInstance(1L);

            Assert.IsNull(instance);
        }

        [TestMethod]
        public void TestGetJobInstances()
        {
            IDictionary<string, JobParameter> params2 = new OrderedDictionary<string, JobParameter>(2);
            params2["param"] = new JobParameter("test");
            var parameters2 = new JobParameters(params2);
            IDictionary<string, JobParameter> params3 = new OrderedDictionary<string, JobParameter>(2);
            params3["param"] = new JobParameter(1);
            var parameters3 = new JobParameters(params3);
            IDictionary<string, JobParameter> params4 = new OrderedDictionary<string, JobParameter>(2);
            params4["param"] = new JobParameter(2);
            var parameters4 = new JobParameters(params4);
            _jobInstanceDao.CreateJobInstance("testJob", _parameters);
            var instance2 = _jobInstanceDao.CreateJobInstance("testJob", parameters2);
            var instance3 = _jobInstanceDao.CreateJobInstance("testJob", parameters3);
            _jobInstanceDao.CreateJobInstance("testJob", parameters4);
            _jobInstanceDao.CreateJobInstance("testJob2", _parameters);

            var instances = _jobInstanceDao.GetJobInstances("testJob", 1, 2);

            Assert.AreEqual(2, instances.Count);
            Assert.AreEqual(instance3, instances[0]);
            Assert.AreEqual(instance2, instances[1]);
        }

        [TestMethod]
        public void TestGetJobNames()
        {
            IDictionary<string, JobParameter> params2 = new OrderedDictionary<string, JobParameter>(2);
            params2["param"] = new JobParameter("test");
            var parameters2 = new JobParameters(params2);
            _jobInstanceDao.CreateJobInstance("testJob3", _parameters);
            _jobInstanceDao.CreateJobInstance("testJob4", _parameters);
            _jobInstanceDao.CreateJobInstance("testJob1", _parameters);
            _jobInstanceDao.CreateJobInstance("testJob1", parameters2);
            _jobInstanceDao.CreateJobInstance("testJob2", _parameters);

            var names = _jobInstanceDao.GetJobNames();

            Assert.AreEqual(4, names.Count);
            Assert.AreEqual("testJob1", names[0]);
            Assert.AreEqual("testJob2", names[1]);
            Assert.AreEqual("testJob3", names[2]);
            Assert.AreEqual("testJob4", names[3]);
        }

        [TestMethod]
        public void TestGetJobInstanceCount1()
        {
            IDictionary<string, JobParameter> params2 = new OrderedDictionary<string, JobParameter>(2);
            params2["param"] = new JobParameter("test");
            var parameters2 = new JobParameters(params2);
            _jobInstanceDao.CreateJobInstance("testJob3", _parameters);
            _jobInstanceDao.CreateJobInstance("testJob4", _parameters);
            _jobInstanceDao.CreateJobInstance("testJob1", _parameters);
            _jobInstanceDao.CreateJobInstance("testJob1", parameters2);
            _jobInstanceDao.CreateJobInstance("testJob2", _parameters);

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
