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
using System.Globalization;

namespace Summer.Batch.CoreTests.Util
{
[TestClass]
    public class LongUtilsTest
    {

        #region Test Add.
        [TestMethod]
        public void LongUtils_Add1Test()
        {
            long? long1 = 1L;
            long? long2 = 1L;
            long? result = LongUtils.Add(long1, long2);
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result);
            Assert.AreEqual(2L, result);
        }

        [TestMethod]
        public void LongUtils_Add2Test()
        {
            long? long1 = 1L;
            long? long2 = default(long?);
            long? result = LongUtils.Add(long1, long2);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result);
            Assert.AreEqual(1L, result);
        }

        [TestMethod]
        public void LongUtils_Add3Test()
        {
            long? long1 = default(long?);
            long? long2 = 1L;
            long? result = LongUtils.Add(long1, long2);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result);
            Assert.AreEqual(1L, result);
        }

        [TestMethod]
        public void LongUtils_Add4Test()
        {
            long? long1 = 1L;
            long? long2 = 1L;
            long? long3 = 1L;

            long? result = LongUtils.Add(long1, long2, long3);
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result);
            Assert.AreEqual(3L, result);
        }

        [TestMethod]
        public void LongUtils_Add5Test()
        {
            long? long1 = 1L;
            long? long2 = 1L;
            long? long3 = default(long?);
            long? result = LongUtils.Add(long1, long2, long3);
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result);
            Assert.AreEqual(2L, result);
        }

        [TestMethod]
        public void LongUtils_Add6Test() 
        {
            long? long1 = 1L;
            long? long2 = 1L;
            long? long3 = 1L;
            long? long4 = 1L;
            long? result = LongUtils.Add(long1, long2, long3, long4);
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result);
            Assert.AreEqual(4L, result);
        }

        [TestMethod]
        public void LongUtils_Add7Test() 
        {
            long? long1 = 1L;
            long? long2 = 1L;
            long? long3 = 1L;
            long? long4 = default(long?);
            long? result = LongUtils.Add(long1, long2, long3, long4);
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result);
            Assert.AreEqual(3L, result);
        }

        [TestMethod]
        public void LongUtils_Add8Test()
        {
            long? long1 = 1L;
            long? long2 = 1L;
            long? long3 = 1L;
            long? long4 = 1L;
            long? long5 = 1L;
            long? result = LongUtils.Add(long1, long2, long3, long4, long5);
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result);
            Assert.AreEqual(5L, result);
        }

        [TestMethod]
        public void LongUtils_Add9Test()
        {
            long? long1 = 1L;
            long? long2 = 1L;
            long? long3 = 1L;
            long? long4 = 1L;
            long? long5 = default(long?);
            long? result = LongUtils.Add(long1, long2, long3, long4, long5);
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result);
            Assert.AreEqual(4L, result);
        }
        #endregion

        #region Test Compare.
        [TestMethod]
        public void LongUtils_CompareTo1Test() 
        {
            long? long1 = 1L;
            long? long2 = 1L;
            int? result = LongUtils.CompareTo(long1, long2);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result);		
        }

        [TestMethod]
        public void LongUtils_CompareTo2Test() 
        {
            long? long1 = default(long?);
            long? long2 = 1L;
            int? result = LongUtils.CompareTo(long1, long2);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void LongUtils_CompareTo3Test() 
        {
            long? long1 = 1L;
            long? long2 = default(long?);
            int? result = LongUtils.CompareTo(long1, long2);
            Assert.AreEqual(null, result);
        }
        #endregion

        #region Test Divide.
        [TestMethod]
        public void LongUtils_Divide1Test() 
        {
            long? long1 = 1L;
            long? long2 = 1L;
            long? result = LongUtils.Divide(long1, long2);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result);
            Assert.AreEqual(1L, result);
        }

        [TestMethod]
        public void LongUtils_Divide2Test() 
        {
            long? long1 = default(long?);
            long? long2 = 1L;
            long? result = LongUtils.Divide(long1, long2);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void LongUtils_Divide3Test() 
        {
            long? long1 = 1L;
            long? long2 = default(long?);
            long? result = LongUtils.Divide(long1, long2);
            Assert.AreEqual(null, result);
        }
    
        [TestMethod]
        [ExpectedException(typeof(DivideByZeroException))]
        public void LongUtils_Divide4Test() 
        {
            long? long1 = 1L;
            long? long2 = 0L;
            long? result = LongUtils.Divide(long1, long2);
            Assert.AreEqual(null, result);
        }
        #endregion

        #region Test IsGreaterThan, IsLowerThan, IsNullOrZeroValue.
        [TestMethod]
        public void LongUtils_IsGreaterThan1Test() 
        {
            long? long1 = 1L;
            long? long2 = 1L;
            bool? result = LongUtils.IsGreaterThan(long1, long2);
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result);
            Assert.AreEqual("false", result.ToString(),true);
        }

        [TestMethod]
        public void LongUtils_IsGreaterThan2Test() 
        {
            long? long1 = 3L;
            long? long2 = 1L;
            bool? result = LongUtils.IsGreaterThan(long1, long2);
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result);
            Assert.AreEqual("true", result.ToString(),true);
        }
    
        [TestMethod]
        public void LongUtils_IsGreaterThan3Test() 
        {
            long? long1 = default(long?);
            long? long2 = 1L;
            bool? result = LongUtils.IsGreaterThan(long1, long2);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void LongUtils_IsGreaterThan4Test() 
        {
            long? long1 = 1L;
            long? long2 = default(long?);
            bool? result = LongUtils.IsGreaterThan(long1, long2);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void LongUtils_IsLowerThan1Test() 
        {
            long? long1 = 1L;
            long? long2 = 1L;
            bool? result = LongUtils.IsLowerThan(long1, long2);
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result);
            Assert.AreEqual("false", result.ToString(),true);
        }

        [TestMethod]
        public void LongUtils_IsLowerThan2Test() 
        {
            long? long1 = 1L;
            long? long2 = 3L;
            bool? result = LongUtils.IsLowerThan(long1, long2);
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result);
            Assert.AreEqual("true", result.ToString(),true);
        }

        [TestMethod]
        public void LongUtils_IsLowerThan3Test() 
        {
            long? long1 = default(long?);
            long? long2 = 1L;
            bool? result = LongUtils.IsLowerThan(long1, long2);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void LongUtils_IsLowerThan4Test() 
        {
            long? long1 = 1L;
            long? long2 = default(long?);
            bool? result = LongUtils.IsLowerThan(long1, long2);
            Assert.AreEqual(null, result);
        }
        
        [TestMethod]
        public void LongUtils_IsNullOrZeroValue1Test() 
        {
            long? value = default(long?);
            Boolean result = LongUtils.IsNullOrZeroValue(value);
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result);
            Assert.AreEqual("true", result.ToString(),true);
        }

        [TestMethod]
        public void LongUtils_IsNullOrZeroValue2Test()
        {
            long? value = 1L;
            Boolean result = LongUtils.IsNullOrZeroValue(value);
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result);
            Assert.AreEqual("false", result.ToString(),true);
        }

        [TestMethod]
        public void LongUtils_IsNullOrZeroValue3Test() 
        {
            long? value = 0L;
            Boolean result = LongUtils.IsNullOrZeroValue(value);
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result);
            Assert.AreEqual("true", result.ToString(),true);
        }
        #endregion

        #region Test Multiply.
        [TestMethod]
        public void LongUtils_Multiply1Test()
        {
            long? long1 = 1L;
            long? long2 = 1L;
            long? result = LongUtils.Multiply(long1, long2);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result);
            Assert.AreEqual(1L, result);
        }

        [TestMethod]
        public void LongUtils_Multiply2Test() 
        {
            long? long1 = default(long?);
            long? long2 = 1L;
            long? result = LongUtils.Multiply(long1, long2);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void LongUtils_Multiply3Test() 
        {
            long? long1 = 1L;
            long? long2 = default(long?);
            long? result = LongUtils.Multiply(long1, long2);
            Assert.AreEqual(null, result);
        }
        #endregion

        #region Test Substract
        [TestMethod]
        public void LongUtils_Substract1Test() 
        {
            long? long1 = 1L;
            long? long2 = 1L;
            long? result = LongUtils.Substract(long1, long2);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result);
            Assert.AreEqual(0L, result);
        }

        [TestMethod]
        public void LongUtils_Substract2Test() 
        {
            long? long1 = default(long?);
            long? long2 = 1L;
            long? result = LongUtils.Substract(long1, long2);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void LongUtils_Substract3Test() 
        {
            long? long1 = 1L;
            long? long2 = default(long?);
            long? result = LongUtils.Substract(long1, long2);
            Assert.AreEqual(null, result);
        }
        #endregion

        #region Test ToString.
        [TestMethod]
        public void LongUtils_ToString1Test() 
        {
            long? long1 = 1L;
            String result = LongUtils.ToString(long1);
            Assert.AreEqual("1", result);
        }

        [TestMethod]
        public void LongUtils_ToString2Test() 
        {
            long? long1 = default(long?);
            string result = LongUtils.ToString(long1);
            Assert.AreEqual("", result);
        }
        #endregion
    }
}
