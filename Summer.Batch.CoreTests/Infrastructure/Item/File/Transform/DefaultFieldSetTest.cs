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
using Summer.Batch.Infrastructure.Item.File.Transform;

namespace Summer.Batch.CoreTests.Infrastructure.Item.File.Transform
{
    [TestClass]
    public class DefaultFieldSetTest
    {
        [TestMethod]
        public void TestReadStringIndex()
        {
            var fieldSet = new DefaultFieldSet(new[] { " string " });

            var read = fieldSet.ReadString(0);

            Assert.AreEqual("string", read);
        }

        [TestMethod]
        public void TestReadStringName()
        {
            var fieldSet = new DefaultFieldSet(new[] { "name" }, new[] { " string " });

            var read = fieldSet.ReadString("name");

            Assert.AreEqual("string", read);
        }

        [TestMethod]
        public void TestReadRawStringIndex()
        {
            var fieldSet = new DefaultFieldSet(new[] { " string " });

            var read = fieldSet.ReadRawString(0);

            Assert.AreEqual(" string ", read);
        }

        [TestMethod]
        public void TestReadRawStringName()
        {
            var fieldSet = new DefaultFieldSet(new[] { "name" }, new[] { " string " });

            var read = fieldSet.ReadRawString("name");

            Assert.AreEqual(" string ", read);
        }

        [TestMethod]
        public void TestReadBooleanIndex1()
        {
            var fieldSet = new DefaultFieldSet(new[] { "true" });

            var read = fieldSet.ReadBoolean(0);

            Assert.IsTrue(read);
        }

        [TestMethod]
        public void TestReadBooleanIndex2()
        {
            var fieldSet = new DefaultFieldSet(new[] { "Y" });

            var read = fieldSet.ReadBoolean(0, "Y");

            Assert.IsTrue(read);
        }

        [TestMethod]
        public void TestReadBooleanName1()
        {
            var fieldSet = new DefaultFieldSet(new[] { "name" }, new[] { "true" });

            var read = fieldSet.ReadBoolean("name");

            Assert.IsTrue(read);
        }

        [TestMethod]
        public void TestReadBooleanName2()
        {
            var fieldSet = new DefaultFieldSet(new[] { "name" }, new[] { "Y" });

            var read = fieldSet.ReadBoolean("name", "Y");

            Assert.IsTrue(read);
        }

        [TestMethod]
        public void TestReadBooleanIndex()
        {
            var fieldSet = new DefaultFieldSet(new[] { "c" });

            var read = fieldSet.ReadChar(0);

            Assert.AreEqual('c', read);
        }

        [TestMethod]
        public void TestReadBooleanName()
        {
            var fieldSet = new DefaultFieldSet(new[] { "name" }, new[] { "c" });

            var read = fieldSet.ReadChar("name");

            Assert.AreEqual('c', read);
        }

        [TestMethod]
        public void TestReadByteIndex()
        {
            var fieldSet = new DefaultFieldSet(new[] { "255" });

            var read = fieldSet.ReadByte(0);

            Assert.AreEqual(255, read);
        }

        [TestMethod]
        public void TestReadByteName()
        {
            var fieldSet = new DefaultFieldSet(new[] { "name" }, new[] { "255" });

            var read = fieldSet.ReadByte("name");

            Assert.AreEqual(255, read);
        }

        [TestMethod]
        public void TestReadShortIndex()
        {
            var fieldSet = new DefaultFieldSet(new[] { "32767" });

            var read = fieldSet.ReadShort(0);

            Assert.AreEqual(32767, read);
        }

        [TestMethod]
        public void TestReadShortName()
        {
            var fieldSet = new DefaultFieldSet(new[] { "name" }, new[] { "32767" });

            var read = fieldSet.ReadShort("name");

            Assert.AreEqual(32767, read);
        }

        [TestMethod]
        public void TestReadIntIndex()
        {
            var fieldSet = new DefaultFieldSet(new[] { "2147483647" });

            var read = fieldSet.ReadInt(0);

            Assert.AreEqual(2147483647, read);
        }

        [TestMethod]
        public void TestReadIntName()
        {
            var fieldSet = new DefaultFieldSet(new[] { "name" }, new[] { "2147483647" });

            var read = fieldSet.ReadInt("name");

            Assert.AreEqual(2147483647, read);
        }

        [TestMethod]
        public void TestReadIntIndexDefaultValue()
        {
            var fieldSet = new DefaultFieldSet(new[] { " " });

            var read = fieldSet.ReadInt(0, 3);

            Assert.AreEqual(3, read);
        }

        [TestMethod]
        public void TestReadIntNameDefaultValue()
        {
            var fieldSet = new DefaultFieldSet(new[] { "name" }, new[] { " " });

            var read = fieldSet.ReadInt("name", 3);

            Assert.AreEqual(3, read);
        }

        [TestMethod]
        public void TestReadLongIndex()
        {
            var fieldSet = new DefaultFieldSet(new[] { "9223372036854775807" });

            var read = fieldSet.ReadLong(0);

            Assert.AreEqual(9223372036854775807, read);
        }

        [TestMethod]
        public void TestReadLongName()
        {
            var fieldSet = new DefaultFieldSet(new[] { "name" }, new[] { "9223372036854775807" });

            var read = fieldSet.ReadLong("name");

            Assert.AreEqual(9223372036854775807, read);
        }

        [TestMethod]
        public void TestReadLongIndexDefaultValue()
        {
            var fieldSet = new DefaultFieldSet(new[] { " " });

            var read = fieldSet.ReadLong(0, 2);

            Assert.AreEqual(2, read);
        }

        [TestMethod]
        public void TestReadLongNameDefaultValue()
        {
            var fieldSet = new DefaultFieldSet(new[] { "name" }, new[] { " " });

            var read = fieldSet.ReadLong("name", 2);

            Assert.AreEqual(2, read);
        }

        [TestMethod]
        public void TestReadFloatIndex()
        {
            var fieldSet = new DefaultFieldSet(new[] { "2.37" });

            var read = fieldSet.ReadFloat(0);

            Assert.AreEqual(2.37F, read);
        }

        [TestMethod]
        public void TestReadFloatName()
        {
            var fieldSet = new DefaultFieldSet(new[] { "name" }, new[] { "2.37" });

            var read = fieldSet.ReadFloat("name");

            Assert.AreEqual(2.37F, read);
        }

        [TestMethod]
        public void TestReadDoubleIndex()
        {
            var fieldSet = new DefaultFieldSet(new[] { "2.37" });

            var read = fieldSet.ReadDouble(0);

            Assert.AreEqual(2.37D, read);
        }

        [TestMethod]
        public void TestReadDoubleName()
        {
            var fieldSet = new DefaultFieldSet(new[] { "name" }, new[] { "2.37" });

            var read = fieldSet.ReadDouble("name");

            Assert.AreEqual(2.37D, read);
        }

        [TestMethod]
        public void TestReadDecimalIndex()
        {
            var fieldSet = new DefaultFieldSet(new[] { "1999.37" });

            var read = fieldSet.ReadDecimal(0);

            Assert.AreEqual(1999.37m, read);
        }

        [TestMethod]
        public void TestReadDecimalName()
        {
            var fieldSet = new DefaultFieldSet(new[] { "name" }, new[] { "1999.37" });

            var read = fieldSet.ReadDecimal("name");

            Assert.AreEqual(1999.37m, read);
        }

        [TestMethod]
        public void TestReadDecimalIndexDefaultValue()
        {
            var fieldSet = new DefaultFieldSet(new[] { " " });

            var read = fieldSet.ReadDecimal(0, 33m);

            Assert.AreEqual(33m, read);
        }

        [TestMethod]
        public void TestReadDecimalNameDefaultValye()
        {
            var fieldSet = new DefaultFieldSet(new[] { "name" }, new[] { " " });

            var read = fieldSet.ReadDecimal("name", 33m);

            Assert.AreEqual(33m, read);
        }

        [TestMethod]
        public void TestReadDateIndex()
        {
            var fieldSet = new DefaultFieldSet(new[] { "2015-06-03" });

            var read = fieldSet.ReadDate(0);

            Assert.AreEqual(new DateTime(2015, 6, 3), read);
        }

        [TestMethod]
        public void TestReadDateName()
        {
            var fieldSet = new DefaultFieldSet(new[] { "name" }, new[] { "2015-06-03" });

            var read = fieldSet.ReadDate("name");

            Assert.AreEqual(new DateTime(2015, 6, 3), read);
        }

        [TestMethod]
        public void TestReadDateIndexDefaultValue()
        {
            var fieldSet = new DefaultFieldSet(new[] { " " });

            var read = fieldSet.ReadDate(0, new DateTime(1970, 1, 1));

            Assert.AreEqual(new DateTime(1970, 1, 1), read);
        }

        [TestMethod]
        public void TestReadDateNameDefaultValue()
        {
            var fieldSet = new DefaultFieldSet(new[] { "name" }, new[] { " " });

            var read = fieldSet.ReadDate("name", new DateTime(1970, 1, 1));

            Assert.AreEqual(new DateTime(1970, 1, 1), read);
        }

        [TestMethod]
        public void TestReadDateIndexPattern()
        {
            var fieldSet = new DefaultFieldSet(new[] { "04/06/2015" });

            var read = fieldSet.ReadDate(0, "dd/MM/yyyy");

            Assert.AreEqual(new DateTime(2015, 6, 4), read);
        }

        [TestMethod]
        public void TestReadDateNamePattern()
        {
            var fieldSet = new DefaultFieldSet(new[] { "name" }, new[] { "04/06/2015" });

            var read = fieldSet.ReadDate("name", "dd/MM/yyyy");

            Assert.AreEqual(new DateTime(2015, 6, 4), read);
        }

        [TestMethod]
        public void TestReadDateIndexPatternDefaultValue()
        {
            var fieldSet = new DefaultFieldSet(new[] { " " });

            var read = fieldSet.ReadDate(0, "dd/MM/yyyy", new DateTime(1970, 1, 1));

            Assert.AreEqual(new DateTime(1970, 1, 1), read);
        }

        [TestMethod]
        public void TestReadDateNamePatternDefaultValue()
        {
            var fieldSet = new DefaultFieldSet(new[] { "name" }, new[] { " " });

            var read = fieldSet.ReadDate("name", "dd/MM/yyyy", new DateTime(1970, 1, 1));

            Assert.AreEqual(new DateTime(1970, 1, 1), read);
        }
    }
}