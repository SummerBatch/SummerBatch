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
using Summer.Batch.Core.Job.Flow;
using System;

namespace Summer.Batch.CoreTests.Core.Job.Flow
{
    [TestClass()]
    public class FlowExecutionStatusTests
    {

        [TestMethod()]
        public void FlowExecutionStatusTest()
        {
            FlowExecutionStatus status1 = FlowExecutionStatus.Completed;
            Assert.IsNotNull(status1);
            FlowExecutionStatus status2 = new FlowExecutionStatus("COMPLETED");
            Assert.IsNotNull(status2);
            Assert.AreEqual(status1,status2);
        }

        [TestMethod()]
        public void IsStopTest()
        {
            FlowExecutionStatus status = FlowExecutionStatus.Stopped;
            Assert.IsTrue(status.IsStop());
            FlowExecutionStatus status5 = new FlowExecutionStatus("STOPPEDBYBUS");
            Assert.IsTrue(status5.IsStop());
            FlowExecutionStatus status2 = FlowExecutionStatus.Completed;
            Assert.IsFalse(status2.IsStop());
            FlowExecutionStatus status3 = FlowExecutionStatus.Failed;
            Assert.IsFalse(status3.IsStop());
            FlowExecutionStatus status4= FlowExecutionStatus.Unkown;
            Assert.IsFalse(status4.IsStop());
        }

        [TestMethod()]
        public void IsFailTest()
        {
            FlowExecutionStatus status = FlowExecutionStatus.Stopped;
            Assert.IsFalse(status.IsFail());
            FlowExecutionStatus status2 = FlowExecutionStatus.Completed;
            Assert.IsFalse(status2.IsFail());
            FlowExecutionStatus status3 = FlowExecutionStatus.Failed;
            Assert.IsTrue(status3.IsFail());
            FlowExecutionStatus status5 = new FlowExecutionStatus("FAILEDBYMISTAKE");
            Assert.IsTrue(status5.IsFail());
            FlowExecutionStatus status4 = FlowExecutionStatus.Unkown;
            Assert.IsFalse(status4.IsFail());
        }

        [TestMethod()]
        public void IsEndTest()
        {
            FlowExecutionStatus status = FlowExecutionStatus.Stopped;
            Assert.IsTrue(status.IsEnd());
            FlowExecutionStatus status6 = new FlowExecutionStatus("STOPPEDBYBUS");
            Assert.IsTrue(status6.IsEnd());
            FlowExecutionStatus status2 = FlowExecutionStatus.Completed;
            Assert.IsTrue(status2.IsEnd());
            FlowExecutionStatus status7 = new FlowExecutionStatus("COMPLETEDONSCHEDULE");
            Assert.IsTrue(status7.IsEnd());
            FlowExecutionStatus status3 = FlowExecutionStatus.Failed;
            Assert.IsTrue(status3.IsEnd());
            FlowExecutionStatus status5 = new FlowExecutionStatus("FAILEDBYMISTAKE");
            Assert.IsTrue(status5.IsEnd());
            FlowExecutionStatus status4 = FlowExecutionStatus.Unkown;
            Assert.IsFalse(status4.IsEnd());
        }

        [TestMethod()]
        public void CompareToTest()
        {
            FlowExecutionStatus status = new FlowExecutionStatus("COMPLETED");
            FlowExecutionStatus status2 = FlowExecutionStatus.Completed;
            FlowExecutionStatus status3 = new FlowExecutionStatus("COMPLETEDBYFORCE");
            Assert.AreEqual(0, status.CompareTo(status2));
            Assert.AreNotEqual(0, status2.CompareTo(status3));
            Assert.AreEqual(string.Compare("COMPLETED", "COMPLETEDBYFORCE", StringComparison.Ordinal), 
                status2.CompareTo(status3));
        }

        [TestMethod()]
        public void ToStringTest()
        {
            FlowExecutionStatus status = new FlowExecutionStatus("Completed");
            Assert.AreEqual("Completed", status.ToString());
            FlowExecutionStatus status2 = FlowExecutionStatus.Completed;
            Assert.AreEqual("COMPLETED", status2.ToString());
        }

        [TestMethod()]
        public void GetHashCodeTest()
        {
            FlowExecutionStatus status = new FlowExecutionStatus("Completed");
            Assert.AreEqual(status.Name.GetHashCode(), status.GetHashCode());
        }

        [TestMethod()]
        public void EqualsTest()
        {
            FlowExecutionStatus status1 = FlowExecutionStatus.Completed;
            Assert.IsNotNull(status1);
            FlowExecutionStatus status2 = new FlowExecutionStatus("COMPLETED");
            Assert.IsNotNull(status2);
            Assert.IsTrue(status1.Equals(status2));
        }
    }
}
