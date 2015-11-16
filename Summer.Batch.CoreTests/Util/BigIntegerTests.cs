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
using Summer.Batch.Extra.Utils;
using System;
using System.Numerics;
using System.Globalization;

namespace Summer.Batch.CoreTests.Util
{
    [TestClass]
    public sealed class BigIntegerTests
    {
        #region Test ToString.
        [TestMethod]
        public void BigIntegerUtils_ToString1Test()
        {
            BigInteger? bigInteger = new BigInteger(1);
            String result = BigIntegerUtils.ToString(bigInteger);
            Assert.AreEqual("1", result);
        }
        [TestMethod]
        public void BigIntegerUtils_ToString2Test() 
        {
            BigInteger? bigInteger = null;
            String result = BigIntegerUtils.ToString(bigInteger);
            Assert.AreEqual("", result);
        }
        #endregion

        #region Test IsGreaterThan, IsLowerThan.
        [TestMethod]
        public void BigIntegerUtils_IsGreaterThan1Test() 
        {
            BigInteger? bigInteger1 = new BigInteger(1);
            BigInteger? bigInteger2 = new BigInteger(1);
            bool? result = BigIntegerUtils.IsGreaterThan(bigInteger1, bigInteger2);
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result);
            Assert.AreEqual("False", result.ToString());
        }

        [TestMethod]
        public void BigIntegerUtils_IsGreaterThan2Test()
        {
            BigInteger? bigInteger1 = new BigInteger(1);
            BigInteger? bigInteger2 = new BigInteger(1);
            bool? result = BigIntegerUtils.IsGreaterThan(bigInteger1, bigInteger2);
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result);
            Assert.AreEqual("False", result.ToString());
        }

        [TestMethod]
        public void BigIntegerUtils_IsGreaterThan3Test()
        {
            BigInteger? bigInteger1 = null;
            BigInteger? bigInteger2 = new BigInteger(1);
            bool? result = BigIntegerUtils.IsGreaterThan(bigInteger1, bigInteger2);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void BigIntegerUtils_IsGreaterThan4Test()
        {
            BigInteger? bigInteger1 = new BigInteger(1);
            BigInteger? bigInteger2 = null;
            bool? result = BigIntegerUtils.IsGreaterThan(bigInteger1, bigInteger2);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void BigIntegerUtils_IsLowerThan1Test()
        {
            BigInteger? bigInteger1 = new BigInteger(1);
            BigInteger? bigInteger2 = new BigInteger(1);
            bool? result = BigIntegerUtils.IsLowerThan(bigInteger1, bigInteger2);
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result);
            Assert.AreEqual("False", result.ToString());
        }

        [TestMethod]
        public void BigIntegerUtils_IsLowerThan2Test() 
        {
            BigInteger? bigInteger1 = new BigInteger(1);
            BigInteger? bigInteger2 = new BigInteger(1);
            bool? result = BigIntegerUtils.IsLowerThan(bigInteger1, bigInteger2);		
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result);
            Assert.AreEqual("False", result.ToString());
        }

        [TestMethod]
        public void BigIntegerUtils_IsLowerThan3Test() 
        {
            BigInteger? bigInteger1 = null;
            BigInteger? bigInteger2 = new BigInteger(1);
            bool? result = BigIntegerUtils.IsLowerThan(bigInteger1, bigInteger2);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void BigIntegerUtils_IsLowerThan4Test()
        {
            BigInteger? bigInteger1 = new BigInteger(1);
            BigInteger? bigInteger2 = null;		
            bool? result = BigIntegerUtils.IsLowerThan(bigInteger1, bigInteger2);
            Assert.AreEqual(null, result);
        }
        #endregion
    }
}
