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
using Summer.Batch.Common.Util;
using System.Collections.Generic;
using System.Linq;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Summer.Batch.CoreTests.Util
{
    [TestClass]
    public class SerializationUtilsTests
    {
        [TestMethod]
        public void SerializeDeserializeString()
        {
            const string s = "testString";
            var s2 = s.Serialize().Deserialize<string>();
            Assert.AreEqual(s, s2);
            Assert.AreNotSame(s, s2);
        }

        [TestMethod]
        public void SerializeDeserializeList1()
        {
            var l = new List<string>();
            var l2 = l.Serialize().Deserialize<List<string>>();
            Assert.IsTrue(l.SequenceEqual(l2));
            Assert.AreNotSame(l, l2);
        }

        [TestMethod]
        public void SerializeDeserializeList2()
        {
            var l = new List<string> { "s1", "s2" };

            var l2 = l.Serialize().Deserialize<List<string>>();

            Assert.IsTrue(l.SequenceEqual(l2));
            Assert.AreNotSame(l, l2);
        }
    }
}
