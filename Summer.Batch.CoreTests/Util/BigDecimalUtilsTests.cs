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
    public sealed class BigDecimalUtilsTests
    {
        #region Test Abs.
        [TestMethod]
        public void BigDecimalUtils_Abs1Test()
        {
            decimal? decimal1 = 1.0m;
            decimal? result = BigDecimalUtils.Abs(decimal1);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, (int)result);
            Assert.AreEqual(1L, (long)result);
            Assert.AreEqual(1.0f, (float)result);
            Assert.AreEqual(1.0d, (double)result);
            string s = BigDecimalUtils.ToString(result);
            Assert.AreEqual("1.0", BigDecimalUtils.ToString(result));
            Assert.AreEqual((byte) 1, (byte)result);
            Assert.AreEqual((short) 1, (short)result);
        }

        [TestMethod]
        public void BigDecimalUtils_Abs2Test()
        {
            decimal? decimal1 = null;
            decimal? result = BigDecimalUtils.Abs(decimal1);
            Assert.AreEqual(null, result);
        }
        #endregion

        #region Test Add.
        [TestMethod]
        public void BigDecimalUtils_Add1Test() 
        {
            decimal? decimal1 = 1.0m;
            decimal? decimal2 = 1.0m;
            decimal? result = BigDecimalUtils.Add(decimal1, decimal2);
            Assert.IsNotNull(result);
            Assert.AreEqual(2, (int)result);
            Assert.AreEqual(2L, (long)result);
            Assert.AreEqual(2.0f, (float)result, 1.0f);
            Assert.AreEqual(2.0, (double)result, 1.0);
            Assert.AreEqual("2.0", BigDecimalUtils.ToString(result));
            Assert.AreEqual((byte) 2, (byte)result);
            Assert.AreEqual((short) 2, (short)result);
        }

        [TestMethod]
        public void BigDecimalUtils_Add2Test()
        {
            decimal? decimal1 = 1.0m;
            decimal? decimal2 = null;
            decimal? result = BigDecimalUtils.Add(decimal1, decimal2);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, (int)result);
            Assert.AreEqual(1L, (long)result);
            Assert.AreEqual(1.0f, (float)result, 1.0f);
            Assert.AreEqual(1.0d, (double)result, 1.0d);
            Assert.AreEqual("1.0", BigDecimalUtils.ToString(result));
            Assert.AreEqual((byte) 1, (byte)result);
            Assert.AreEqual((short) 1, (short)result);
        }

        [TestMethod]
        public void BigDecimalUtils_Add3Test() 
        {
            decimal? decimal1 = null;
            decimal? decimal2 = 1.0m;
            decimal? result = BigDecimalUtils.Add(decimal1, decimal2);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, (int)result);
            Assert.AreEqual(1L, (long)result);
            Assert.AreEqual(1.0f, (float)result, 1.0f);
            Assert.AreEqual(1.0d, (double)result, 1.0d);
            Assert.AreEqual("1.0", BigDecimalUtils.ToString(result));

            Assert.AreEqual((byte) 1, (byte)result);
            Assert.AreEqual((short) 1, (short)result);
        }
        #endregion

        #region Test CompareTo.
        [TestMethod]
        public void BigDecimalUtils_CompareTo1Test()
        {
            decimal? decimal1 = 1.0m;
            decimal? decimal2 = 1.0m;
            int? result = BigDecimalUtils.CompareTo(decimal1, decimal2);
            Assert.IsNotNull(result);
            Assert.AreEqual((byte) 0, (byte)result);
            Assert.AreEqual((short) 0, (short)result);
            Assert.AreEqual(0, (int)result);
            Assert.AreEqual(0L, (long)result);
            Assert.AreEqual(0.0f, (float)result);
            Assert.AreEqual(0.0d, (double)result);
            Assert.AreEqual("0", result.ToString());
        }

        [TestMethod]
        public void BigDecimalUtils_CompareTo2Test() 
        {
            decimal? decimal1 = null;
            decimal? decimal2 = 1.0m;
            int? result = BigDecimalUtils.CompareTo(decimal1, decimal2);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void BigDecimalUtils_CompareTo3Test() 
        {
            decimal? decimal1 = 1.0m;
            decimal? decimal2 = null;
            int? result = BigDecimalUtils.CompareTo(decimal1, decimal2);
            Assert.AreEqual(null, result);
        }
        #endregion

        #region Test Divide.
        [TestMethod]
        public void BigDecimalUtils_Divide1Test() {
            decimal? decimal1 = 1.0m;
            decimal? decimal2 = 1.0m;
            int scale = 1;
            decimal? result = BigDecimalUtils.Divide(decimal1,decimal2,scale);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, (int)result);
            Assert.AreEqual(1L, (long)result);
            Assert.AreEqual(1.0f, (float)result);
            Assert.AreEqual(1.0d, (double)result);
            Assert.AreEqual("1", BigDecimalUtils.ToString(result));
            Assert.AreEqual((byte) 1, (byte)result);
            Assert.AreEqual((short) 1, (short)result);
        }

        [TestMethod]
        public void BigDecimalUtils_Divide2Test()
        {
            decimal? decimal1 = 1.0m;
            decimal? decimal2 = 3.0m;
            int scale = 2;
            decimal? result = BigDecimalUtils.Divide(decimal1, decimal2, scale);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, (int)result);
            Assert.AreEqual(0L, (long)result);
            Assert.AreEqual(0.33f, (float)result);
            Assert.AreEqual(0.33d, (double)result);
            Assert.AreEqual("0.33", BigDecimalUtils.ToString(result));
            Assert.AreEqual((byte)0, (byte)result);
            Assert.AreEqual((short)0, (short)result);
        }

        [TestMethod]
        public void BigDecimalUtils_Divide3Test()
        {
            decimal? decimal1 = 2.0m;
            decimal? decimal2 = 3.0m;
            int scale = 26;
            decimal? result = BigDecimalUtils.Divide(decimal1, decimal2, scale);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, (int)result);
            Assert.AreEqual(0L, (long)result);
            Assert.AreEqual(0.66666666666666666666666667f, (float)result);
            Assert.AreEqual(0.66666666666666666666666667d, (double)result);
            Assert.AreEqual("0.66666666666666666666666667", BigDecimalUtils.ToString(result));
            Assert.AreEqual((byte)0, (byte)result);
            Assert.AreEqual((short)0, (short)result);
        }

        [TestMethod]
        public void BigDecimalUtils_Divide4Test() {
            decimal? decimal1 = null;
            decimal? decimal2 = 1.0m;
            int scale = 1;
            decimal? result = BigDecimalUtils.Divide(decimal1, decimal2, scale);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void BigDecimalUtils_Divide5Test() {
            decimal? decimal1 = 1.0m;
            decimal? decimal2 = null;
            int scale = 1;
            decimal? result = BigDecimalUtils.Divide(decimal1, decimal2, scale);
            Assert.AreEqual(null, result);
        }
        #endregion

        #region Test IsGreaterThan1, IsLowerThan, IsNullOrZeroValue.
        [TestMethod]
        public void BigDecimalUtils_IsGreaterThan1Test()
        {
            decimal? decimal1 = 1.0m;
            decimal? decimal2 = 1.0m;
            bool? result = BigDecimalUtils.IsGreaterThan(decimal1, decimal2);
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result);
            Assert.AreEqual("False", result.ToString());
        }

        [TestMethod]
        public void BigDecimalUtils_IsGreaterThan2Test() 
        {
            decimal? decimal1 = 1.0m;
            decimal? decimal2 = 1.0m;
            bool? result = BigDecimalUtils.IsGreaterThan(decimal1, decimal2);
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result);
            Assert.AreEqual("False", result.ToString());
        }

        [TestMethod]
        public void BigDecimalUtils_IsGreaterThan3Test() 
        {
            decimal? decimal1 = null;
            decimal? decimal2 = 1.0m;
            bool? result = BigDecimalUtils.IsGreaterThan(decimal1, decimal2);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void BigDecimalUtils_IsGreaterThan4Test()
        {
            decimal? decimal1 = 1.0m;
            decimal? decimal2 = null;
            bool? result = BigDecimalUtils.IsGreaterThan(decimal1, decimal2);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void BigDecimalUtils_IsLowerThan1Test() 
        {
            decimal? decimal1 = 1.0m;
            decimal? decimal2 = 1.0m;
            bool? result = BigDecimalUtils.IsLowerThan(decimal1, decimal2);
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result);
            Assert.AreEqual("False", result.ToString());
        }

        [TestMethod]
        public void BigDecimalUtils_IsLowerThan2Test()
        {
            decimal? decimal1 = 1.0m;
            decimal? decimal2 = 1.0m;
            bool? result = BigDecimalUtils.IsLowerThan(decimal1, decimal2);		
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result);
            Assert.AreEqual("False", result.ToString());
        }

        [TestMethod]
        public void BigDecimalUtils_IsLowerThan3Test() 
        {
            decimal? decimal1 = null;
            decimal? decimal2 = 1.0m;
            bool? result = BigDecimalUtils.IsLowerThan(decimal1, decimal2);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void BigDecimalUtils_IsLowerThan4Test()
        {
            decimal? decimal1 = 1.0m;
            decimal? decimal2 = null;		
            bool? result = BigDecimalUtils.IsLowerThan(decimal1, decimal2);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void BigDecimalUtils_IsNullOrZeroValue1Test() 
        {
            decimal? decimal1 = null;
            bool? result = BigDecimalUtils.IsNullOrZeroValue(decimal1);
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result);
            Assert.AreEqual("True", result.ToString());
        }

        [TestMethod]
        public void BigDecimalUtils_IsNullOrZeroValue2Test()
        {
            decimal? decimal1 = 1.0m;
            bool? result = BigDecimalUtils.IsNullOrZeroValue(decimal1);
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result);
            Assert.AreEqual("False", result.ToString());
        }

        [TestMethod]
        public void BigDecimalUtils_IsNullOrZeroValue3Test() 
        {
            decimal? decimal1 = 1.0m;
            bool? result = BigDecimalUtils.IsNullOrZeroValue(decimal1);
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result);
            Assert.AreEqual("False", result.ToString());
        }
        #endregion

        #region Test Multiply.
        [TestMethod]
        public void BigDecimalUtils_Multiply1Test()
        {
            decimal? decimal1 = 1.0m;
            decimal? decimal2 = 1.0m;
            decimal? result = BigDecimalUtils.Multiply(decimal1, decimal2);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, (int)result);
            Assert.AreEqual(1L, (long)result);
            Assert.AreEqual(1.0f, (float)result);
            Assert.AreEqual(1.0d, (double)result);
            Assert.AreEqual("1.00", BigDecimalUtils.ToString(result));
            Assert.AreEqual((byte) 1, (byte)result);
            Assert.AreEqual((short) 1, (short)result);
        }

        [TestMethod]
        public void BigDecimalUtils_Multiply2Test() 
        {
            decimal? decimal1 = null;
            decimal? decimal2 = 1.0m;
            decimal? result = BigDecimalUtils.Multiply(decimal1, decimal2);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void BigDecimalUtils_Multiply3Test()
        {
            decimal? decimal1 = 1.0m;
            decimal? decimal2 = null;		
            decimal? result = BigDecimalUtils.Multiply(decimal1, decimal2);
            Assert.AreEqual(null, result);
        }
        #endregion

        #region Test Substract.
        [TestMethod]
        public void BigDecimalUtils_Subtract1Test() 
        {
            decimal? decimal1 = 1.0m;
            decimal? decimal2 = 1.0m;
            decimal? result = BigDecimalUtils.Subtract(decimal1, decimal2);		
            Assert.IsNotNull(result);
            Assert.AreEqual(0, (int)result);
            Assert.AreEqual(0L, (long)result);
            Assert.AreEqual(0.0f, (float)result);
            Assert.AreEqual(0.0d, (double)result);
            Assert.AreEqual("0.0", BigDecimalUtils.ToString(result));
            Assert.AreEqual((byte) 0, (byte)result);
            Assert.AreEqual((short) 0, (short)result);
        }

        [TestMethod]
        public void BigDecimalUtils_Subtract2Test()
        {
            decimal? decimal1 = null;
            decimal? decimal2 = 1.0m;
            decimal? result = BigDecimalUtils.Subtract(decimal1, decimal2);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void BigDecimalUtils_Subtract3Test()
        {
            decimal? decimal1 = 1.0m;
            decimal? decimal2 = null;
            decimal? result = BigDecimalUtils.Subtract(decimal1, decimal2);
            Assert.AreEqual(null, result);
        }
        #endregion

        #region Test ToString.
        [TestMethod]
        public void BigDecimalUtils_ToString1Test()
        {
            decimal? decimal1 = 1.0m;
            String result = BigDecimalUtils.ToString(decimal1);
            Assert.AreEqual("1.0", result);
        }

        [TestMethod]
        public void BigDecimalUtils_ToString2Test()
        {
            decimal? decimal1 = null;
            String result = BigDecimalUtils.ToString(decimal1);
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void BigDecimalUtils_ToString3Test()
        {
            decimal? decimal1 = 10.03513m;
            String result = BigDecimalUtils.ToString(decimal1);
            Assert.AreEqual("10.03513", result);
        }
        #endregion
    }
}
