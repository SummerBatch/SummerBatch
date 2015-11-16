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

namespace Summer.Batch.CoreTests.Core
{
    [TestClass()]
    public class ExitStatusTests
    {
        [TestMethod()]
        public void GetExitCodeTest()
        {
            ExitStatus status = new ExitStatus("myExitCode");
            Assert.AreEqual("myExitCode", status.ExitCode);
        }

        [TestMethod()]
        public void GetExitDescriptionTest()
        {
            ExitStatus status = new ExitStatus("myExitCode", "myExitDescription");
            Assert.AreEqual("myExitDescription", status.ExitDescription);
        }

        [TestMethod()]
        public void AndTest()
        {
            ExitStatus running = ExitStatus.Executing;
            ExitStatus runnningAndNull = running.And(null);
            Assert.AreEqual(running, runnningAndNull);
            ExitStatus completed = ExitStatus.Completed;
            running = running.And(completed);
            Assert.AreEqual( completed,running);
        }

        [TestMethod()]
        public void CompareToTest()
        {
            ExitStatus status = new ExitStatus("myExitCode", "myExitDescription");
            ExitStatus status2 = new ExitStatus("myExitCode", "myExitDescription");
            ExitStatus status3 = new ExitStatus("myExitCode1", "myExitDescription");
            ExitStatus status4 = new ExitStatus("myExitCode1", "myExitDescription1");
            Assert.AreEqual(0,status.CompareTo(status2));
            Assert.AreNotEqual(0,status.CompareTo(status3));
            Assert.AreNotEqual(0,status.CompareTo(status4));
            Assert.AreEqual(0,status3.CompareTo(status4));
        }

        [TestMethod()]
        public void ToStringTest()
        {
            ExitStatus status = new ExitStatus("myExitCode", "myExitDescription");
            Assert.AreEqual("(exitCode=myExitCode;exitDescription=myExitDescription)", status.ToString());
        }

        [TestMethod()]
        public void EqualsTest()
        {
            ExitStatus status = new ExitStatus("myExitCode", "myExitDescription");
            ExitStatus status2 = new ExitStatus("myExitCode", "myExitDescription");
            ExitStatus status3 = new ExitStatus("myExitCode1", "myExitDescription");
            ExitStatus status4 = new ExitStatus("myExitCode1", "myExitDescription1");
            Assert.IsTrue(status.Equals(status2));
            Assert.IsFalse(status.Equals(status3));
            Assert.IsFalse(status.Equals(status4));
            Assert.IsFalse(status3.Equals(status4));
        }

        [TestMethod()]
        public void GetHashCodeTest()
        {
            ExitStatus status = new ExitStatus("myExitCode", "myExitDescription");
            Assert.AreEqual("(exitCode=myExitCode;exitDescription=myExitDescription)".GetHashCode(), status.GetHashCode());
        }

        [TestMethod()]
        public void ReplaceExitCodeTest()
        {
            ExitStatus status = new ExitStatus("myExitCode", "myExitDescription");
            status = status.ReplaceExitCode("myReplacedExitCode");
            Assert.AreEqual("myReplacedExitCode", status.ExitCode);
        }

        [TestMethod()]
        public void IsRunningTest()
        {
            ExitStatus running = ExitStatus.Executing;
            ExitStatus completed = ExitStatus.Completed;
            Assert.IsTrue(running.IsRunning());
            Assert.IsFalse(completed.IsRunning());

        }

        [TestMethod()]
        public void AddExitDescriptionTest()
        {
            ExitStatus stopped = ExitStatus.Stopped;
            stopped = stopped.AddExitDescription("STOPPED BECAUSE ABNORMALLY ENDED");
            Assert.AreEqual ("; STOPPED BECAUSE ABNORMALLY ENDED",stopped.ExitDescription);
            stopped = stopped.AddExitDescription(".NO MORE INFO");
            Assert.AreEqual("; STOPPED BECAUSE ABNORMALLY ENDED; .NO MORE INFO", stopped.ExitDescription);
        }

        [TestMethod()]
        public void IsNonDefaultExitStatusTest()
        {
            ExitStatus stopped = ExitStatus.Stopped;
            Assert.IsTrue(ExitStatus.IsNonDefaultExitStatus(stopped));
            ExitStatus custom = new ExitStatus("myExitCode");
            Assert.IsFalse(ExitStatus.IsNonDefaultExitStatus(custom));
        }
    }
}