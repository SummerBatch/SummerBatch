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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Summer.Batch.Common.Util;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Summer.Batch.CoreTests.Util
{
    [TestClass]
    public class StringConvertTest
    {
        [TestMethod]
        public void TestConvertStringBoolean()
        {
            Assert.IsTrue(StringConverter.Convert<bool>("True"));
            Assert.IsFalse(StringConverter.Convert<bool>("false"));
        }

        [TestMethod]
        public void TestConvertStringByte()
        {
            Assert.AreEqual((byte)123, StringConverter.Convert<byte>("123"));
        }

        [TestMethod]
        public void TestConvertStringChar()
        {
            Assert.AreEqual('c', StringConverter.Convert<char>("c"));
        }

        [TestMethod]
        public void TestConvertStringDateTime()
        {
            Assert.AreEqual(new DateTime(2015, 9, 17), StringConverter.Convert<DateTime>("2015-09-17"));
        }

        [TestMethod]
        public void TestConvertStringDecimal()
        {
            Assert.AreEqual(decimal.MaxValue, StringConverter.Convert<decimal>("79228162514264337593543950335"));
        }

        [TestMethod]
        public void TestConvertStringDouble()
        {
            Assert.AreEqual(3.6, StringConverter.Convert<double>("3.6"));
        }

        [TestMethod]
        public void TestConvertStringShort()
        {
            Assert.AreEqual((short)1024, StringConverter.Convert<short>("1024"));
        }

        [TestMethod]
        public void TestConvertStringInt()
        {
            Assert.AreEqual(262144, StringConverter.Convert<int>("262144"));
        }

        [TestMethod]
        public void TestConvertStringLong()
        {
            Assert.AreEqual(1099511627776L, StringConverter.Convert<long>("1099511627776"));
        }

        [TestMethod]
        public void TestConvertStringSByte()
        {
            Assert.AreEqual((sbyte)-128, StringConverter.Convert<sbyte>("-128"));
        }

        [TestMethod]
        public void TestConvertStringFloat()
        {
            Assert.AreEqual(5.3f, StringConverter.Convert<float>("5.3"));
        }

        [TestMethod]
        public void TestConvertStringUShort()
        {
            Assert.AreEqual((ushort)65535, StringConverter.Convert<ushort>("65535"));
        }

        [TestMethod]
        public void TestConvertStringUInt()
        {
            Assert.AreEqual(4294967295, StringConverter.Convert<uint>("4294967295"));
        }

        [TestMethod]
        public void TestConvertStringULong()
        {
            Assert.AreEqual(18446744073709551615, StringConverter.Convert<ulong>("18446744073709551615"));
        }

        [TestMethod]
        public void TestArray1()
        {
            var expected = new[] { "a", "b", " c" };
            Assert.IsTrue(expected.SequenceEqual(StringConverter.Convert<string[]>("a,b, c")));
        }

        [TestMethod]
        public void TestArray2()
        {
            var expected = new[] { 1, 2, 3 };
            Assert.IsTrue(expected.SequenceEqual(StringConverter.Convert<int[]>("1,2, 3")));
        }
    }
}