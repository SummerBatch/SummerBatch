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
    public class DoubleUtilsTests
    {
        #region Test Abs, Ceil, Floor, Round.
        [TestMethod]
        public void DoubleUtils_Abs1Test() 
        {
            double? double1 = new double? (1.0);
            double? result = DoubleUtils.Abs(double1);
            Assert.IsNotNull(result);
            Assert.AreEqual((byte) 1, (byte)result);
            Assert.AreEqual((short) 1, (short)result);
            Assert.AreEqual(1, (int)result);
            Assert.AreEqual(1L, (long)result);
            Assert.AreEqual(1.0f, (float)result, 1.0f);
            Assert.AreEqual(1.0, (double)result, 1.0);
            Assert.AreEqual("1.0", result.Value.ToString("0.0",CultureInfo.InvariantCulture));
            Assert.AreEqual(false, Double.IsNaN((double)result));
            Assert.AreEqual(false, Double.IsInfinity((double)result));
        }

        [TestMethod]
        public void DoubleUtils_Ceil1Test()
        {
            double? double1 = new double?(1.0);
            double? result = DoubleUtils.Ceil(double1);
            Assert.IsNotNull(result);
            Assert.AreEqual((byte)1, (byte)result);
            Assert.AreEqual((short)1, (short)result);
            Assert.AreEqual(1, (int)result);
            Assert.AreEqual(1L, (long)result);
            Assert.AreEqual(1.0f, (float)result, 1.0f);
            Assert.AreEqual(1.0, (double)result, 1.0);
            Assert.AreEqual("1.0", result.Value.ToString("0.0", CultureInfo.InvariantCulture));
            Assert.AreEqual(false, Double.IsNaN((double)result));
            Assert.AreEqual(false, Double.IsInfinity((double)result));
        }

        [TestMethod]
        public void DoubleUtils_Floor1()
        {
            double? double1 = new double?(1.0);
            double? result = DoubleUtils.Floor(double1);
            Assert.IsNotNull(result);
            Assert.AreEqual((byte)1, (byte)result);
            Assert.AreEqual((short)1, (short)result);
            Assert.AreEqual(1, (int)result);
            Assert.AreEqual(1L, (long)result);
            Assert.AreEqual(1.0f, (float)result, 1.0f);
            Assert.AreEqual(1.0, (double)result, 1.0);
            Assert.AreEqual("1.0", result.Value.ToString("0.0", CultureInfo.InvariantCulture));
            Assert.AreEqual(false, Double.IsNaN((double)result));
            Assert.AreEqual(false, Double.IsInfinity((double)result));
        }

        [TestMethod]
        public void DoubleUtils_Round1()
        {
            double? double1 = new double?(1.0);
            long? result = DoubleUtils.Round(double1);
            Assert.IsNotNull(result);
            Assert.AreEqual((byte)1, (byte)result);
            Assert.AreEqual((short)1, (short)result);
            Assert.AreEqual(1, (int)result);
            Assert.AreEqual(1L, (long)result);
            Assert.AreEqual(1.0f, (float)result, 1.0f);
            Assert.AreEqual(1.0, (long)result, 1.0);
            Assert.AreEqual("1", result.Value.ToString("0", CultureInfo.InvariantCulture));
        }
        #endregion

        #region Test Add.
        [TestMethod]
        public void DoubleUtils_Add1Test() 
        {
            double? double1 = new double? (1.0);
            double? double2 = new double? (1.0);
            double? result = DoubleUtils.Add(double1, double2);
            Assert.IsNotNull(result);
            Assert.AreEqual((byte) 2, (byte)result);
            Assert.AreEqual((short) 2, (short)result);
            Assert.AreEqual(2, (int)result);
            Assert.AreEqual(2L, (long)result);
            Assert.AreEqual(2.0f, (float)result, 1.0f);
            Assert.AreEqual(2.0, (double)result, 1.0);
            Assert.AreEqual("2.0", result.Value.ToString("0.0", CultureInfo.InvariantCulture));
            Assert.AreEqual(false, Double.IsNaN((double)result));
            Assert.AreEqual(false, Double.IsInfinity((double)result));
        }

        [TestMethod]
        public void DoubleUtils_Add2Test() 
        {
            double? double1 = new double? (1.0);
            double? double2 = null;
            double? result = DoubleUtils.Add(double1, double2);
            Assert.IsNotNull(result);
            Assert.AreEqual((byte) 1, (byte)result);
            Assert.AreEqual((short) 1, (short)result);
            Assert.AreEqual(1, (int)result);
            Assert.AreEqual(1L, (long)result);
            Assert.AreEqual(1.0f, (float)result, 1.0f);
            Assert.AreEqual(1.0, (double)result, 1.0);
            Assert.AreEqual("1.0", result.Value.ToString("0.0", CultureInfo.InvariantCulture));
            Assert.AreEqual(false, Double.IsNaN((double)result));
            Assert.AreEqual(false, Double.IsInfinity((double)result));
        }

        [TestMethod]
        public void DoubleUtils_Add3Test() 
        {
            double? double1 = null;
            double? double2 = new double? (1.0);
            double? result = DoubleUtils.Add(double1, double2);
            Assert.IsNotNull(result);
            Assert.AreEqual((byte) 1, (byte)result);
            Assert.AreEqual((short) 1, (short)result);
            Assert.AreEqual(1, (int)result);
            Assert.AreEqual(1L, (long)result);
            Assert.AreEqual(1.0f, (float)result, 1.0f);
            Assert.AreEqual(1.0, (double)result, 1.0);
            Assert.AreEqual("1.0", result.Value.ToString("0.0", CultureInfo.InvariantCulture));
            Assert.AreEqual(false, Double.IsNaN((double)result));
            Assert.AreEqual(false, Double.IsInfinity((double)result));
        }

        [TestMethod]
        public void DoubleUtils_Add4Test() 
        {
            double? double1 = new double? (1.0);
            double? double2 = new double? (1.0);
            double? double3 = new double? (1.0);
            double? result = DoubleUtils.Add(double1, double2, double3);
            Assert.IsNotNull(result);
            Assert.AreEqual((byte) 3, (byte)result);
            Assert.AreEqual((short) 3, (short)result);
            Assert.AreEqual(3, (int)result);
            Assert.AreEqual(3L, (long)result);
            Assert.AreEqual(3.0f, (float)result, 1.0f);
            Assert.AreEqual(3.0, (double)result, 1.0);
            Assert.AreEqual("3.0", result.Value.ToString("0.0", CultureInfo.InvariantCulture));
            Assert.AreEqual(false, Double.IsNaN((double)result));
            Assert.AreEqual(false, Double.IsInfinity((double)result));
        }

        [TestMethod]
        public void DoubleUtils_Add5Test() 
        {
            double? double1 = new double? (1.0);
            double? double2 = new double? (1.0);
            double? double3 = null;
            double? result = DoubleUtils.Add(double1, double2, double3);
            Assert.IsNotNull(result);
            Assert.AreEqual((byte) 2, (byte)result);
            Assert.AreEqual((short) 2, (short)result);
            Assert.AreEqual(2, (int)result);
            Assert.AreEqual(2L, (long)result);
            Assert.AreEqual(2.0f, (float)result, 1.0f);
            Assert.AreEqual(2.0, (double)result, 1.0);
            Assert.AreEqual("2.0", result.Value.ToString("0.0", CultureInfo.InvariantCulture));
            Assert.AreEqual(false, Double.IsNaN((double)result));
            Assert.AreEqual(false, Double.IsInfinity((double)result));
        }

        [TestMethod]
        public void DoubleUtils_Add6Test() 
        {
            double? double1 = new double? (1.0);
            double? double2 = new double? (1.0);
            double? double3 = new double? (1.0);
            double? double4 = new double? (1.0);

            double? result = DoubleUtils.Add(double1, double2, double3, double4);
            Assert.IsNotNull(result);
            Assert.AreEqual((byte) 4, (byte)result);
            Assert.AreEqual((short) 4, (short)result);
            Assert.AreEqual(4, (int)result);
            Assert.AreEqual(4L, (long)result);
            Assert.AreEqual(4.0f, (float)result, 1.0f);
            Assert.AreEqual(4.0, (double)result, 1.0);
            Assert.AreEqual("4.0", result.Value.ToString("0.0", CultureInfo.InvariantCulture));
            Assert.AreEqual(false, Double.IsNaN((double)result));
            Assert.AreEqual(false, Double.IsInfinity((double)result));
        }

        [TestMethod]
        public void DoubleUtils_Add7()
        {
            double? double1 = new double? (1.0);
            double? double2 = new double? (1.0);
            double? double3 = new double? (1.0);
            double? double4 = null;

            double? result = DoubleUtils.Add(double1, double2, double3, double4);

            Assert.IsNotNull(result);
            Assert.AreEqual((byte) 3, (byte)result);
            Assert.AreEqual((short) 3, (short)result);
            Assert.AreEqual(3, (int)result);
            Assert.AreEqual(3L, (long)result);
            Assert.AreEqual(3.0f, (float)result, 1.0f);
            Assert.AreEqual(3.0, (double)result, 1.0);
            Assert.AreEqual("3.0", result.Value.ToString("0.0", CultureInfo.InvariantCulture));
            Assert.AreEqual(false, Double.IsNaN((double)result));
            Assert.AreEqual(false, Double.IsInfinity((double)result));
        }

        [TestMethod]
        public void DoubleUtils_Add8()
        {
            double? double1 = new double? (1.0);
            double? double2 = new double? (1.0);
            double? double3 = new double? (1.0);
            double? double4 = new double? (1.0);
            double? double5 = new double? (1.0);

            double? result = DoubleUtils.Add(double1, double2, double3, double4, double5);

            Assert.IsNotNull(result);
            Assert.AreEqual((byte) 5, (byte)result);
            Assert.AreEqual((short) 5, (short)result);
            Assert.AreEqual(5, (int)result);
            Assert.AreEqual(5L, (long)result);
            Assert.AreEqual(5.0f, (float)result, 1.0f);
            Assert.AreEqual(5.0, (double)result, 1.0);
            Assert.AreEqual("5.0", result.Value.ToString("0.0", CultureInfo.InvariantCulture));
            Assert.AreEqual(false, Double.IsNaN((double)result));
            Assert.AreEqual(false, Double.IsInfinity((double)result));
        }

        [TestMethod]
        public void DoubleUtils_Add9Test()
        {
            double? double1 = new double? (1.0);
            double? double2 = new double? (1.0);
            double? double3 = new double? (1.0);
            double? double4 = new double? (1.0);
            double? double5 = null;
            double? result = DoubleUtils.Add(double1, double2, double3, double4, double5);
            Assert.IsNotNull(result);
            Assert.AreEqual((byte) 4, (byte)result);
            Assert.AreEqual((short) 4, (short)result);
            Assert.AreEqual(4, (int)result);
            Assert.AreEqual(4L, (long)result);
            Assert.AreEqual(4.0f, (float)result, 1.0f);
            Assert.AreEqual(4.0, (double)result, 1.0);
            Assert.AreEqual("4.0", result.Value.ToString("0.0", CultureInfo.InvariantCulture));
            Assert.AreEqual(false, Double.IsNaN((double)result));
            Assert.AreEqual(false, Double.IsInfinity((double)result));
        }
        #endregion

        #region Test CompareTo.
        [TestMethod]
        public void DoubleUtils_CompareTo1()
        {
            double? double1 = new double? (1.0);
            double? double2 = new double? (1.0);
            int? result = DoubleUtils.CompareTo(double1, double2);
            Assert.IsNotNull(result);
            Assert.AreEqual((byte) 0, (byte)result);
            Assert.AreEqual((short) 0, (short)result);
            Assert.AreEqual(0, (int)result);
            Assert.AreEqual(0L, (long)result);
            Assert.AreEqual(0.0f, (float)result, 1.0f);
            Assert.AreEqual(0.0, (int)result, 1.0);
            Assert.AreEqual("0", result.Value.ToString("0", CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public void DoubleUtils_CompareTo2Test()
        {
            double? double1 = null;
            double? double2 = new double? (1.0);
            int? result = DoubleUtils.CompareTo(double1, double2);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void DoubleUtils_CompareTo3()
        {
            double? double1 = new double? (1.0);
            double? double2 = null;
            int? result = DoubleUtils.CompareTo(double1, double2);
            Assert.AreEqual(null, result);
        }
        #endregion

        #region Test Divide.
        [TestMethod]
        public void DoubleUtils_Divide1()
        {
            double? double1 = new double? (1.0);
            double? double2 = new double? (1.0);
            double? result = DoubleUtils.Divide(double1, double2);
            Assert.IsNotNull(result);
            Assert.AreEqual((byte) 1, (byte)result);
            Assert.AreEqual((short) 1, (short)result);
            Assert.AreEqual(1, (int)result);
            Assert.AreEqual(1L, (long)result);
            Assert.AreEqual(1.0f, (float)result, 1.0f);
            Assert.AreEqual(1.0, (double)result, 1.0);
            Assert.AreEqual("1.0", result.Value.ToString("0.0", CultureInfo.InvariantCulture));
            Assert.AreEqual(false, Double.IsNaN((double)result));
            Assert.AreEqual(false, Double.IsInfinity((double)result));
        }

        [TestMethod]
        public void DoubleUtils_Divide2()
        {
            double? double1 = null;
            double? double2 = new double? (1.0);
            double? result = DoubleUtils.Divide(double1, double2);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void DoubleUtils_Divide3()
        {
            double? double1 = new double? (1.0);
            double? double2 = null;
            double? result = DoubleUtils.Divide(double1, double2);
            Assert.AreEqual(null, result);
        }
        #endregion

        #region Test IsGreaterThan, IsLowerThan, IsNullOrZeroValue.
        [TestMethod]
        public void DoubleUtils_IsGreaterThan1Test()
        {
            double? double1 = new double? (1.0);
            double? double2 = new double? (1.0);
            bool? result = DoubleUtils.IsGreaterThan(double1, double2);
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result);
            Assert.AreEqual("False", result.ToString());
        }

        [TestMethod]
        public void DoubleUtils_IsGreaterThan2()
        {
            double? double1 = new double? (1.1);
            double? double2 = new double? (1.0);
            bool? result = DoubleUtils.IsGreaterThan(double1, double2);
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result);
            Assert.AreEqual("True", result.ToString());
        }

        [TestMethod]
        public void DoubleUtils_IsGreaterThan3Test()
        {
            double? double1 = null;
            double? double2 = new double? (1.0);
            bool? result = DoubleUtils.IsGreaterThan(double1, double2);	
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void DoubleUtils_IsGreaterThan4Test() 
        {
            double? double1 = new double? (1.0);
            double? double2 = null;
            bool? result = DoubleUtils.IsGreaterThan(double1, double2);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void DoubleUtils_IsLowerThan1Test() 
        {
            double? double1 = new double? (1.0);
            double? double2 = new double? (1.0);
            bool? result = DoubleUtils.IsLowerThan(double1, double2);
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result);
            Assert.AreEqual("False", result.ToString());
        }

        [TestMethod]
        public void DoubleUtils_IsLowerThan2Test() 
        {
            double? double1 = new double? (1.0);
            double? double2 = new double? (1.1);
            bool? result = DoubleUtils.IsLowerThan(double1, double2);
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result);
            Assert.AreEqual("True", result.ToString());
        }

        [TestMethod]
        public void DoubleUtils_IsLowerThan3Test() 
        {
            double? double1 = null;
            double? double2 = new double? (1.0);
            bool? result = DoubleUtils.IsLowerThan(double1, double2);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void DoubleUtils_IsLowerThan4Test() 
        {
            double? double1 = new double? (1.0);
            double? double2 = null;
            bool? result = DoubleUtils.IsLowerThan(double1, double2);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void DoubleUtils_IsNullOrZeroValue1Test() 
        {
            double? double1 = null;
            bool? result = DoubleUtils.IsNullOrZeroValue(double1);
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result);
            Assert.AreEqual("True", result.ToString());
        }

        [TestMethod]
        public void DoubleUtils_IsNullOrZeroValue2Test() 
        {
            double? double1 = new double? (1.0);
            bool? result = DoubleUtils.IsNullOrZeroValue(double1);
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result);
            Assert.AreEqual("False", result.ToString());
        }

        [TestMethod]
        public void DoubleUtils_IsNullOrZeroValue3Test() 
        {
            double? double1 = new double? (0.0);
            bool? result = DoubleUtils.IsNullOrZeroValue(double1);
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result);
            Assert.AreEqual("True", result.ToString());
        }
        #endregion

        #region Test Multiply.
        [TestMethod]
        public void DoubleUtils_Multiply1()
        {
            double? double1 = new double? (1.0);
            double? double2 = new double? (1.0);
            double? result = DoubleUtils.Multiply(double1, double2);
            Assert.IsNotNull(result);
            Assert.AreEqual((byte) 1, (byte)result);
            Assert.AreEqual((short) 1, (short)result);
            Assert.AreEqual(1, (int)result);
            Assert.AreEqual(1L, (long)result);
            Assert.AreEqual(1.0f, (float)result, 1.0f);
            Assert.AreEqual(1.0, (double)result, 1.0);
            Assert.AreEqual("1.0", result.Value.ToString("0.0", CultureInfo.InvariantCulture));
            Assert.AreEqual(false, Double.IsNaN((double)result));
            Assert.AreEqual(false, Double.IsInfinity((double)result));
        }

        [TestMethod]
        public void DoubleUtils_Multiply2()
        {
            double? double1 = null;
            double? double2 = new double? (1.0);
            double? result = DoubleUtils.Multiply(double1, double2);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void DoubleUtils_Multiply3Test() 
        {
            double? double1 = new double? (1.0);
            double? double2 = null;
            double? result = DoubleUtils.Multiply(double1, double2);
            Assert.AreEqual(null, result);
        }
        #endregion

        #region Test Substract.
        [TestMethod]
        public void DoubleUtils_Substract1Test() 
        {
            double? double1 = new double? (1.0);
            double? double2 = new double? (1.0);
            double? result = DoubleUtils.Substract(double1, double2);
            Assert.IsNotNull(result);
            Assert.AreEqual((byte) 0, (byte)result);
            Assert.AreEqual((short) 0, (short)result);
            Assert.AreEqual(0, (int)result);
            Assert.AreEqual(0L, (long)result);
            Assert.AreEqual(0.0f, (float)result, 1.0f);
            Assert.AreEqual(0.0, (double)result, 1.0);
            Assert.AreEqual("0.0", result.Value.ToString("0.0", CultureInfo.InvariantCulture));
            Assert.AreEqual(false, Double.IsNaN((double)result));
            Assert.AreEqual(false, Double.IsInfinity((double)result));
        }

        [TestMethod]
        public void DoubleUtils_Substract2Test() 
        {
            double? double1 = null;
            double? double2 = new double? (1.0);
            double? result = DoubleUtils.Substract(double1, double2);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void DoubleUtils_Substract3Test() 
        {
            double? double1 = new double? (1.0);
            double? double2 = null;
            double? result = DoubleUtils.Substract(double1, double2);
            Assert.AreEqual(null, result);
        }
        #endregion

        #region Test ToString.
        [TestMethod]
        public void DoubleUtils_ToString1Test() 
        {
            double? double1 = new double? (1.0);
            String result = DoubleUtils.ToString(double1);
            Assert.AreEqual("1", result);
        }

        [TestMethod]
        public void DoubleUtils_ToString2Test() 
        {
            double? double1 = null;
            String result = DoubleUtils.ToString(double1);
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void DoubleUtils_ToString3Test()
        {
            double? double1 = new double?(1.0);
            string s = CultureInfo.InvariantCulture.ToString();
            var result = DoubleUtils.ToString(double1, "0.0", "");
            Assert.AreEqual("1.0", result);
        }
        
        [TestMethod]
        public void DoubleUtils_ToString4Test()
        {
            double? double1 = null;
            var result = DoubleUtils.ToString(double1, "0.0", "InvalideCulture");
            Assert.AreEqual("", result);
        }

        [TestMethod]
        [ExpectedException(typeof(CultureNotFoundException))]
        public void DoubleUtils_ToString5Test()
        {
            double? double1 = new double?(1.0);
            var result = DoubleUtils.ToString(double1, "0.0", "InvalideCulture");
        }
        #endregion
    }
}
