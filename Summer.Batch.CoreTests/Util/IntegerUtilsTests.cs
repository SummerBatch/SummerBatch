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
    public class IntegerUtilsTests
    {
        #region Test Abs.
        [TestMethod]
        public void IntegerUtils_Abs1Test()
        {
            int? int1 = new int?(1);
            int? result = (int?)IntegerUtils.Abs(int1);
            Assert.IsNotNull(result);
            Assert.AreEqual((byte) 1, (byte)result);
            Assert.AreEqual((short) 1, (short)result);
            Assert.AreEqual(1, result);
            Assert.AreEqual(1L, (long)result);
            Assert.AreEqual(1.0f, (float)result, 1.0f);
            Assert.AreEqual(1.0, (double)result, 1.0);
            Assert.AreEqual("1", result.ToString());
        }
        #endregion

        #region Test Add.
        [TestMethod]
        public void IntegerUtils_Add1Test()
        {
            int? int1 = new int?(1);
            int? int2 = new int?(1);
            int? result = IntegerUtils.Add(int1, int2);
            Assert.IsNotNull(result);
            Assert.AreEqual((byte) 2, (byte)result);
            Assert.AreEqual((short) 2, (short)result);
            Assert.AreEqual(2, result);
            Assert.AreEqual(2L, (long)result);
            Assert.AreEqual(2.0f, (float)result, 1.0f);
            Assert.AreEqual(2.0, (double)result, 1.0);
            Assert.AreEqual("2", result.ToString());
        }

        [TestMethod]
        public void IntegerUtils_Add2Test()
        {
            int? int1 = new int?(1);
            int? int2 = null;
            int? result = IntegerUtils.Add(int1, int2);
            Assert.IsNotNull(result);
            Assert.AreEqual((byte) 1, (byte)result);
            Assert.AreEqual((short) 1, (short)result);
            Assert.AreEqual(1, result);
            Assert.AreEqual(1L, (long)result);
            Assert.AreEqual(1.0f, (float)result, 1.0f);
            Assert.AreEqual(1.0, (double)result, 1.0);
            Assert.AreEqual("1", result.ToString());
        }

        [TestMethod]
        public void IntegerUtils_Add3Test()
        {
            int? int1 = null;
            int? int2 = new int?(1);
            int? result = IntegerUtils.Add(int1, int2);
            Assert.IsNotNull(result);
            Assert.AreEqual((byte) 1, (byte)result);
            Assert.AreEqual((short) 1, (short)result);
            Assert.AreEqual(1, result);
            Assert.AreEqual(1L, (long)result);
            Assert.AreEqual(1.0f, (float)result, 1.0f);
            Assert.AreEqual(1.0, (double)result, 1.0);
            Assert.AreEqual("1", result.ToString());
        }

        [TestMethod]
        public void IntegerUtils_Add4Test()
        {
            int? int1 = new int?(1);
            int? int2 = new int?(1);
            int? int3 = new int?(1);
            int? result = IntegerUtils.Add(int1, int2, int3);
            Assert.IsNotNull(result);
            Assert.AreEqual((byte) 3, (byte)result);
            Assert.AreEqual((short) 3, (short)result);
            Assert.AreEqual(3, result);
            Assert.AreEqual(3L, (long)result);
            Assert.AreEqual(3.0f, (float)result, 1.0f);
            Assert.AreEqual(3.0, (double)result, 1.0);
            Assert.AreEqual("3", result.ToString());
        }

        [TestMethod]
        public void IntegerUtils_Add5Test()
        {
            int? int1 = new int?(1);
            int? int2 = new int?(1);
            int? int3 = null;
            int? result = IntegerUtils.Add(int1, int2, int3);
            Assert.IsNotNull(result);
            Assert.AreEqual((byte) 2, (byte)result);
            Assert.AreEqual((short) 2, (short)result);
            Assert.AreEqual(2, result);
            Assert.AreEqual(2L, (long)result);
            Assert.AreEqual(2.0f, (float)result, 1.0f);
            Assert.AreEqual(2.0, (double)result, 1.0);
            Assert.AreEqual("2", result.ToString());
        }

        [TestMethod]
        public void IntegerUtils_Add6Test()
        {
            int? int1 = new int?(1);
            int? int2 = new int?(1);
            int? int3 = new int?(1);
            int? int4 = new int?(1);
            int? result = IntegerUtils.Add(int1, int2, int3, int4);
            Assert.IsNotNull(result);
            Assert.AreEqual((byte) 4, (byte)result);
            Assert.AreEqual((short) 4, (short)result);
            Assert.AreEqual(4, result);
            Assert.AreEqual(4L, (long)result);
            Assert.AreEqual(4.0f, (float)result, 1.0f);
            Assert.AreEqual(4.0, (double)result, 1.0);
            Assert.AreEqual("4", result.ToString());
        }

        [TestMethod]
        public void IntegerUtils_Add7Test()
        {
            int? int1 = new int?(1);
            int? int2 = new int?(1);
            int? int3 = new int?(1);
            int? int4 = null;
            int? result = IntegerUtils.Add(int1, int2, int3, int4);
            Assert.IsNotNull(result);
            Assert.AreEqual((byte) 3, (byte)result);
            Assert.AreEqual((short) 3, (short)result);
            Assert.AreEqual(3, result);
            Assert.AreEqual(3L, (long)result);
            Assert.AreEqual(3.0f, (float)result, 1.0f);
            Assert.AreEqual(3.0, (double)result, 1.0);
            Assert.AreEqual("3", result.ToString());
        }

        [TestMethod]
        public void IntegerUtils_Add8Test()
        {
            int? int1 = new int?(1);
            int? int2 = new int?(1);
            int? int3 = new int?(1);
            int? int4 = new int?(1);
            int? int5 = new int?(1);
            int? result = IntegerUtils.Add(int1, int2, int3, int4, int5);
            Assert.IsNotNull(result);
            Assert.AreEqual((byte) 5, (byte)result);
            Assert.AreEqual((short) 5, (short)result);
            Assert.AreEqual(5, result);
            Assert.AreEqual(5L, (long)result);
            Assert.AreEqual(5.0f, (float)result, 1.0f);
            Assert.AreEqual(5.0, (double)result, 1.0);
            Assert.AreEqual("5", result.ToString());
        }

        [TestMethod]
        public void IntegerUtils_Add9Test()
        {
            int? int1 = new int?(1);
            int? int2 = new int?(1);
            int? int3 = new int?(1);
            int? int4 = new int?(1);
            int? int5 = null;
            int? result = IntegerUtils.Add(int1, int2, int3, int4, int5);
            Assert.IsNotNull(result);
            Assert.AreEqual((byte) 4, (byte)result);
            Assert.AreEqual((short) 4, (short)result);
            Assert.AreEqual(4, result);
            Assert.AreEqual(4L, (long)result);
            Assert.AreEqual(4.0f, (float)result, 1.0f);
            Assert.AreEqual(4.0, (double)result, 1.0);
            Assert.AreEqual("4", result.ToString());
        }
        #endregion

        #region Test CompareTo.
        [TestMethod]
        public void IntegerUtils_CompareTo1Test()
        {
            int? int1 = new int?(1);
            int? int2 = new int?(1);
            int? result = IntegerUtils.CompareTo(int1, int2);
            Assert.IsNotNull(result);
            Assert.AreEqual((byte) 0, (byte)result);
            Assert.AreEqual((short) 0, (short)result);
            Assert.AreEqual(0, result);
            Assert.AreEqual(0L, (long)result);
            Assert.AreEqual(0.0f, (float)result, 1.0f);
            Assert.AreEqual(0.0, (double)result, 1.0);
            Assert.AreEqual("0", result.ToString());
        }

        [TestMethod]
        public void IntegerUtils_CompareTo2Test()
        {
            int? int1 = null;
            int? int2 = new int?(1);
            int? result = IntegerUtils.CompareTo(int1, int2);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void IntegerUtils_CompareTo3Test()
        {
            int? int1 = new int?(1);
            int? int2 = null;
            int? result = IntegerUtils.CompareTo(int1, int2);
            Assert.AreEqual(null, result);
        }
        #endregion

        #region Test Divide.
        [TestMethod]
        public void IntegerUtils_Divide1Test()
        {
            int? int1 = new int?(1);
            int? int2 = new int?(1);
            int? result = IntegerUtils.Divide(int1, int2);
            Assert.IsNotNull(result);
            Assert.AreEqual((byte) 1, (byte)result);
            Assert.AreEqual((short) 1, (short)result);
            Assert.AreEqual(1, result);
            Assert.AreEqual(1L, (long)result);
            Assert.AreEqual(1.0f, (float)result, 1.0f);
            Assert.AreEqual(1.0, (double)result, 1.0);
            Assert.AreEqual("1", result.ToString());
        }

        [TestMethod]
        public void IntegerUtils_Divide2Test()
        {
            int? int1 = null;
            int? int2 = new int?(1);
            int? result = IntegerUtils.Divide(int1, int2);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void IntegerUtils_Divide3Test()
        {
            int? int1 = new int?(1);
            int? int2 = null;
            int? result = IntegerUtils.Divide(int1, int2);
            Assert.AreEqual(null, result);
        }
        #endregion

        #region Test IsGreaterThan, IsLowerThan, IsNullOrZeroValue.
        [TestMethod]
        public void IntegerUtils_IsGreaterThan1Test()
        {
            int? int1 = new int?(1);
            int? int2 = new int?(1);
            bool? result = IntegerUtils.IsGreaterThan(int1, int2);
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result);
            Assert.AreEqual("False", result.ToString());
        }

        [TestMethod]
        public void IntegerUtils_IsGreaterThan2Test()
        {
            int? int1 = new int?(1);
            int? int2 = new int?(1);
            bool? result = IntegerUtils.IsGreaterThan(int1, int2);
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result);
            Assert.AreEqual("False", result.ToString());
        }

        [TestMethod]
        public void IntegerUtils_IsGreaterThan3Test()
        {
            int? int1 = null;
            int? int2 = new int?(1);
            bool? result = IntegerUtils.IsGreaterThan(int1, int2);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void IntegerUtils_IsGreaterThan4Test()
        {
            int? int1 = new int?(1);
            int? int2 = null;
            bool? result = IntegerUtils.IsGreaterThan(int1, int2);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void IntegerUtils_IsLowerThan1Test()
        {
            int? int1 = new int?(1);
            int? int2 = new int?(1);
            bool? result = IntegerUtils.IsLowerThan(int1, int2);
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result);
            Assert.AreEqual("False", result.ToString());
        }

        [TestMethod]
        public void IntegerUtils_IsLowerThan2Test()
        {
            int? int1 = new int?(1);
            int? int2 = new int?(2);
            bool? result = IntegerUtils.IsLowerThan(int1, int2);
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result);
            Assert.AreEqual("True", result.ToString());
        }

        [TestMethod]
        public void IntegerUtils_IsLowerThan3Test()
        {
            int? int1 = null;
            int? int2 = new int?(1);
            bool? result = IntegerUtils.IsLowerThan(int1, int2);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void IntegerUtils_IsLowerThan4Test()
        {
            int? int1 = new int?(1);
            int? int2 = null;
            bool? result = IntegerUtils.IsLowerThan(int1, int2);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void IntegerUtils_IsNullOrZeroValue1Test()
        {
            int? int1 = null;
            bool result = IntegerUtils.IsNullOrZeroValue(int1);
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result);
            Assert.AreEqual("True", result.ToString());
        }

        [TestMethod]
        public void IntegerUtils_IsNullOrZeroValue2Test()
        {
            int? int1 = new int?(1);
            bool result = IntegerUtils.IsNullOrZeroValue(int1);
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result);
            Assert.AreEqual("False", result.ToString());
        }

        [TestMethod]
        public void IntegerUtils_IsNullOrZeroValue3Test()
        {
            int? int1 = new int?(0);
            Boolean result = IntegerUtils.IsNullOrZeroValue(int1);
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result);
            Assert.AreEqual("True", result.ToString());
        }
        #endregion

        #region Test Multiply.
        [TestMethod]
        public void IntegerUtils_Multiply1Test()
        {
            int? int1 = new int?(1);
            int? int2 = new int?(1);
            int? result = IntegerUtils.Multiply(int1, int2);
            Assert.IsNotNull(result);
            Assert.AreEqual((byte) 1, (byte)result);
            Assert.AreEqual((short) 1, (short)result);
            Assert.AreEqual(1, result);
            Assert.AreEqual(1L, (long)result);
            Assert.AreEqual(1.0f, (float)result, 1.0f);
            Assert.AreEqual(1.0, (double)result, 1.0);
            Assert.AreEqual("1", result.ToString());
        }

        [TestMethod]
        public void IntegerUtils_Multiply2Test()
        {
            int? int1 = null;
            int? int2 = new int?(1);
            int? result = IntegerUtils.Multiply(int1, int2);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void IntegerUtils_Multiply3Test()
        {
            int? int1 = new int?(1);
            int? int2 = null;
            int? result = IntegerUtils.Multiply(int1, int2);
            Assert.AreEqual(null, result);
        }
        #endregion

        #region Test Substract.
        [TestMethod]
        public void IntegerUtils_Substract1Test()
        {
            int? int1 = new int?(1);
            int? int2 = new int?(1);
            int? result = IntegerUtils.Substract(int1, int2);
            Assert.IsNotNull(result);
            Assert.AreEqual((byte) 0, (byte)result);
            Assert.AreEqual((short) 0, (short)result);
            Assert.AreEqual(0, result);
            Assert.AreEqual(0L, (long)result);
            Assert.AreEqual(0.0f, (float)result, 1.0f);
            Assert.AreEqual(0.0, (double)result, 1.0);
            Assert.AreEqual("0", result.ToString());
        }

        [TestMethod]
        public void IntegerUtils_Substract2Test()
        {
            int? int1 = null;
            int? int2 = new int?(1);
            int? result = IntegerUtils.Substract(int1, int2);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void IntegerUtils_Substract3Test()
        {
            int? int1 = new int?(1);
            int? int2 = null;
            int? result = IntegerUtils.Substract(int1, int2);
            Assert.AreEqual(null, result);
        }
        #endregion

        #region Test ToString.
        [TestMethod]
        public void IntegerUtils_ToString1Test()
        {
            int? int1 = new int?(1);
            String result = IntegerUtils.ToString(int1);
            Assert.AreEqual("1", result);
        }

        [TestMethod]
        public void IntegerUtils_ToString2Test()
        {
            int? int1 = null;
            String result = IntegerUtils.ToString(int1);
            Assert.AreEqual("", result);
        }
        #endregion
    }
}
