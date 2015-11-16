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
using Summer.Batch.Common.Util;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Summer.Batch.CoreTests.Util
{
    [TestClass()]
    public class ObjectUtilsTests
    {
        [TestMethod()]
        public void IdentityToStringTest()
        {
            string o1 = "TEST_STRING";
            string id1 = ObjectUtils.IdentityToString(o1);
            Assert.IsNotNull(id1);
            Assert.IsTrue(id1.StartsWith("String@"));
        }

        [TestMethod()]
        public void GetIdentityHexStringTest()
        {
            string o1 = "TEST_STRING";
            string o1Hc = ObjectUtils.GetIdentityHexString(o1);
            Assert.IsNotNull(o1Hc);
        }
    }
}
