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

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Summer.Batch.Core;

namespace Summer.Batch.CoreTests.Core
{
    [TestClass()]
    public class BatchStatusTests
    {
        [TestMethod()]
        public void MaxTest()
        {
            BatchStatus low = BatchStatus.Completed;
            BatchStatus hi = BatchStatus.Failed;
            BatchStatus max = BatchStatus.Max(low, hi);
            Assert.AreEqual(hi, max);
        }

        [TestMethod()]
        public void IsRunningTest()
        {
            BatchStatus shouldberunning = BatchStatus.Started;
            BatchStatus shouldnotberunning = BatchStatus.Stopped;
            Assert.IsTrue(shouldberunning.IsRunning());
            Assert.IsFalse(shouldnotberunning.IsRunning());
        }

        [TestMethod()]
        public void IsUnsuccessfulTest()
        {
            BatchStatus shouldbesuccessful = BatchStatus.Starting;
            BatchStatus shouldbeunsuccessful = BatchStatus.Failed;
            Assert.IsFalse(shouldbesuccessful.IsUnsuccessful());
            Assert.IsTrue(shouldbeunsuccessful.IsUnsuccessful());
        }

        [TestMethod()]
        public void UpgradeToTest()
        {
            BatchStatus started = BatchStatus.Started;
            // ! reentrant
            started = started.UpgradeTo(BatchStatus.Stopped);
            //should not be running, as upgraded to stopped
            Assert.IsFalse(started.IsRunning());
        }

        [TestMethod()]
        public void IsGreaterThanTest()
        {
            BatchStatus stopped = BatchStatus.Stopped;
            Assert.IsTrue(stopped.IsGreaterThan(BatchStatus.Starting));
            Assert.IsFalse(stopped.IsGreaterThan(BatchStatus.Abandoned));
        }

        [TestMethod()]
        public void IsLessThanTest()
        {
            BatchStatus stopped = BatchStatus.Starting;
            Assert.IsTrue(stopped.IsLessThan(BatchStatus.Stopped));
            Assert.IsFalse(stopped.IsLessThan(BatchStatus.Completed));
        }

        [TestMethod()]
        public void IsLessThanOrEqualToTest()
        {
            BatchStatus stopped = BatchStatus.Starting;
            Assert.IsTrue(stopped.IsLessThanOrEqualTo(BatchStatus.Stopped));
            Assert.IsTrue(stopped.IsLessThanOrEqualTo(BatchStatus.Starting));
            Assert.IsFalse(stopped.IsLessThanOrEqualTo(BatchStatus.Completed));
        }

        [TestMethod()]
        public void MatchTest()
        {
            string started = "STARTED";
            BatchStatus startedStatus = BatchStatus.Match(started);
            Assert.AreEqual(BatchStatus.Started, startedStatus);
        }

        [TestMethod()]
        public void ValueOfTest()
        {
            BatchStatus st1 = BatchStatus.ValueOf("COMPLETED");
            Assert.AreEqual(BatchStatus.Completed,st1);
            BatchStatus st2 = BatchStatus.ValueOf("STARTING");
            Assert.AreEqual(BatchStatus.Starting, st2);
            BatchStatus st3 = BatchStatus.ValueOf("STARTED");
            Assert.AreEqual(BatchStatus.Started, st3);
            BatchStatus st4 = BatchStatus.ValueOf("STOPPING");
            Assert.AreEqual(BatchStatus.Stopping, st4);
            BatchStatus st5 = BatchStatus.ValueOf("STOPPED");
            Assert.AreEqual(BatchStatus.Stopped, st5);
            BatchStatus st6 = BatchStatus.ValueOf("FAILED");
            Assert.AreEqual(BatchStatus.Failed, st6);
            BatchStatus st7 = BatchStatus.ValueOf("ABANDONED");
            Assert.AreEqual(BatchStatus.Abandoned, st7);
            BatchStatus st8 = BatchStatus.ValueOf("UNKNOWN");
            Assert.AreEqual(BatchStatus.Unknown, st8);
        }

        [TestMethod()]
        [ExpectedException(exceptionType:typeof(ArgumentException))]
        public void ValueOf2Test()
        {
            BatchStatus st1 = BatchStatus.ValueOf("COMPLETED1");
        }
    }
}
