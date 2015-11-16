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
using Summer.Batch.Extra.Copybook;
using Summer.Batch.Extra.Ebcdic.Encode;
using Summer.Batch.Extra.Ebcdic.Exception;

namespace Summer.Batch.CoreTests.Ebcdic.Encode
{
    [TestClass]
    public class EbcdicEncoderTests
    {
        private static readonly FieldFormat Text = new FieldFormat();
        private static readonly FieldFormat Transparent = new FieldFormat();
        private static readonly FieldFormat Binary = new FieldFormat();
        private static readonly FieldFormat Packed = new FieldFormat();
        private static readonly FieldFormat UnsignedPacked = new FieldFormat();
        private static readonly FieldFormat Zoned = new FieldFormat();
        private static readonly FieldFormat UnsignedZoned = new FieldFormat();

        private EbcdicEncoder _encoder;

        public static void BeforeClass()
        {
            Text.Name = "text field";
            Text.Occurs = 1;
            Text.Size = "12";
            Text.Type = "X";

            Transparent.Name = "transparent";
            Transparent.Size = "6";
            Transparent.Type = "T";

            Binary.Name = "binary";
            Binary.Size = "6";
            Binary.Type = "B";
            Binary.Decimal = 0;

            Packed.Name = "packed";
            Packed.Size = "6";
            Packed.Type = "3";
            Packed.Signed = true;
            Packed.Decimal = 0;

            UnsignedPacked.Name = "unsigned packed";
            UnsignedPacked.Size = "6";
            UnsignedPacked.Type = "3";
            UnsignedPacked.Signed = false;
            UnsignedPacked.Decimal = 2;

            Zoned.Name = "zoned";
            Zoned.Size = "8";
            Zoned.Type = "9";
            Zoned.Decimal = 2;
            Zoned.Signed = true;
            Zoned.ImpliedDecimal = false;

            UnsignedZoned.Name = "unsigned zoned";
            UnsignedZoned.Size = "8";
            UnsignedZoned.Type = "9";
            UnsignedZoned.Decimal = 2;
            UnsignedZoned.Signed = false;
            UnsignedZoned.ImpliedDecimal = true;
        }

        public void Before()
        {
            _encoder = new EbcdicEncoder("ascii");
        }

        [TestMethod]
        public void EbcdicEncoderTestEncodeText1()
        {
            BeforeClass();
            Before();
            byte[] expected = { 104, 101, 108, 108, 111, 32, 32, 32, 32, 32, 32, 32 };
            byte[] actual = _encoder.Encode("hello", Text);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EbcdicEncoderTestEncodeText2()
        {
            BeforeClass();
            Before();
            byte[] expected = { 72, 101, 108, 108, 111, 32, 87, 111, 114, 108, 100, 33 };
            byte[] actual = _encoder.Encode("Hello World!", Text);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EbcdicEncoderTestEncodeText3()
        {
            BeforeClass();
            Before();
            byte[] expected = { 84, 104, 105, 115, 32, 115, 116, 114, 105, 110, 103, 32 };
            byte[] actual = _encoder.Encode("This string is too long", Text);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EbcdicEncoderTestEncodeText4()
        {
            BeforeClass();
            Before();
            try
            {
                _encoder.Encode(3, Text);
                Assert.Fail();
            }
            catch (ValueTypeMismatchException e)
            {
                Assert.AreEqual("Mismatch type for field text field - Expecting: string, Actual: Int32", e.Message);
            }
        }

        [TestMethod]
        public void EbcdicEncoderTestEncodeTransparent1()
        {
            BeforeClass();
            Before();
            byte[] expected = { 1, 2, 3, 4, 5, 6 };
            byte[] input = { 1, 2, 3, 4, 5, 6 };
            byte[] actual = _encoder.Encode(input, Transparent);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EbcdicEncoderTestEncodeTransparent2()
        {
            BeforeClass();
            Before();
            try
            {
                byte[] input = { 1, 2, 3, 4, 5 };
                _encoder.Encode(input, Transparent);
                Assert.Fail();
            }
            catch (ValueTypeMismatchException e)
            {
                Assert.AreEqual("Value size must be equal to field length.", e.Message);
            }
        }

        [TestMethod]
        public void EbcdicEncoderTestEncodeTransparent3()
        {
            BeforeClass();
            Before();
            try
            {
                _encoder.Encode(3, Transparent);
                Assert.Fail();
            }
            catch (ValueTypeMismatchException e)
            {
                Assert.AreEqual("Mismatch type for field transparent - Expecting: byte[], Actual: Int32", e.Message);
            }
        }

        [TestMethod]
        public void EbcdicEncoderTestEncodeBinary1()
        {
            BeforeClass();
            Before();
            byte[] expected = { 0, 0, 0, 0, 7, 145 };
            byte[] actual = _encoder.Encode(1937m, Binary);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EbcdicEncoderTestEncodeBinary2()
        {
            BeforeClass();
            Before();
            byte[] expected = { 9, 24, 78, 114, 159, 255 };
            byte[] actual = _encoder.Encode(9999999999999m, Binary);
            CollectionAssert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void EbcdicEncoderTestEncodeBinary3()
        {
            BeforeClass();
            Before();
            byte[] expected = { 0, 0, 0, 2, 244, 164 };
            byte[] actual = _encoder.Encode(193700m, Binary);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EbcdicEncoderTestEncodeBinary4()
        {
            BeforeClass();
            Before();
            try
            {
                _encoder.Encode("test", Binary);
                Assert.Fail();
            }
            catch (ValueTypeMismatchException e)
            {
                Assert.AreEqual("Mismatch type for field binary - Expecting: decimal, Actual: String",e.Message);
            }
        }


        [TestMethod]
        public void EbcdicEncoderTestPacked1()
        {
            BeforeClass();
            Before();
            byte[] expected = { 0, 1, 147,124 };
            byte[] actual = _encoder.Encode(1937m, Packed);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EbcdicEncoderTestPacked2()
        {
            BeforeClass();
            Before();
            byte[] expected = { 0, 2, 0, 141 };
            byte[] actual = _encoder.Encode(-2008m, Packed);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EbcdicEncoderTestPacked3()
        {
            BeforeClass();
            Before();
            byte[] expected = { 0, 1, 147, 127 };
            byte[] actual = _encoder.Encode(19.37m, UnsignedPacked);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EbcdicEncoderTestPacked4()
        {
            BeforeClass();
            Before();
            byte[] expected = { 1, 147, 112, 12 };
            byte[] actual = _encoder.Encode(193700m, Packed);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EbcdicEncoderTestPacked5()
        {
            BeforeClass();
            Before();
            try
            {
                _encoder.Encode(3, Packed);
                Assert.Fail();
            }
            catch (ValueTypeMismatchException e)
            {
                Assert.AreEqual("Mismatch type for field packed - Expecting: decimal, Actual: Int32", e.Message);
            }
        }

        [TestMethod]
        public void EbcdicEncoderTestZoned1()
        {
            BeforeClass();
            Before();
            byte[] expected = { 48, 49, 57, 51, 55, 46, 48, 123 };
            byte[] actual = _encoder.Encode(1937m, Zoned);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EbcdicEncoderTestZoned2()
        {
            BeforeClass();
            Before();
            byte[] expected = { 48, 50, 48, 48, 56, 46, 51, 77 };
            byte[] actual = _encoder.Encode(-2008.34m, Zoned);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EbcdicEncoderTestZoned3()
        {
            BeforeClass();
            Before();
            byte[] expected = { 48, 48, 49, 57, 51, 55, 48, 48 };
            byte[] actual = _encoder.Encode(1937m, UnsignedZoned);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EbcdicEncoderTestZoned4()
        {
            BeforeClass();
            Before();
            try
            {
                _encoder.Encode(3, Zoned);
                Assert.Fail();
            }
            catch (ValueTypeMismatchException e)
            {
                Assert.AreEqual("Mismatch type for field zoned - Expecting: decimal, Actual: Int32",e.Message);
            }
        }
    }
}