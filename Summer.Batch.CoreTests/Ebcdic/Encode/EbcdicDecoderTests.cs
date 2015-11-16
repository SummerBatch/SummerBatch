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
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Summer.Batch.Extra.Copybook;
using Summer.Batch.Extra.Ebcdic.Encode;

namespace Summer.Batch.CoreTests.Ebcdic.Encode
{
    [TestClass]
    public class EbcdicDecoderTests
    {
        private static readonly FieldFormat Text = new FieldFormat();
        private static readonly FieldFormat Transparent = new FieldFormat();
        private static readonly FieldFormat Binary = new FieldFormat();
        private static readonly FieldFormat Hex = new FieldFormat();
        private static readonly FieldFormat Packed = new FieldFormat();
        private static readonly FieldFormat DecimalPacked = new FieldFormat();
        private static readonly FieldFormat Zoned = new FieldFormat();
        private static readonly FieldFormat UnsignedZoned = new FieldFormat();

        private EbcdicDecoder _decoder;

        public static void BeforeClass()
        {
            Text.Name = "text field";
            Text.Occurs = 1;
            Text.Size = "12";
            Text.Type = "X";

            Transparent.Name = "transparent";
            Transparent.Type = "T";

            Binary.Name = "binary";
            Binary.Type = "B";
            Binary.Decimal = 0;

            Hex.Name = "HEX";
            Hex.Type = "H";

            Packed.Name = "packed";
            Packed.Type = "3";
            Packed.Decimal = 0;
            Packed.ImpliedDecimal = false;

            DecimalPacked.Name = "unsigned packed";
            DecimalPacked.Type = "3";
            DecimalPacked.Decimal = 2;
            DecimalPacked.ImpliedDecimal = true;

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
            _decoder = new EbcdicDecoder(Encoding.ASCII);
        }

        [TestMethod()]
        public void EbcdicDecoderTestText()
        {
            BeforeClass();
            Before();
            byte[] input = { 72, 101, 108, 108, 111, 32, 87, 111, 114, 108, 100, 33 };
            Assert.AreEqual("Hello World!", _decoder.Decode(input, Text));
        }

        [TestMethod()]
        public void EbcdicDecoderTestTransparent()
        {
            BeforeClass();
            Before();
            byte[] input = { 72, 101, 108, 108, 111, 32, 87, 111, 114, 108, 100, 33 };
            Assert.AreEqual(input, _decoder.Decode(input, Transparent));
        }

        [TestMethod()]
        public void EbcdicDecoderTestBinary()
        {
            BeforeClass();
            Before();
            byte[] input = { 0, 0, 0, 0, 7, 145 };
            Assert.AreEqual(1937m, _decoder.Decode(input, Binary));
        }

        [TestMethod()]
        public void EbcdicDecoderTestHex()
        {
            BeforeClass();
            Before();
            byte[] input = { 0, 0, 0, 0, 7, 145 };
            Assert.AreEqual("000000000791", _decoder.Decode(input, Hex));
        }

        [TestMethod()]
        public void EbcdicDecoderTestPacked1()
        {
            BeforeClass();
            Before();
            byte[] input = { 1, 147, 125 };
            Assert.AreEqual(-1937m, _decoder.Decode(input, Packed));
        }

        [TestMethod()]
        public void EbcdicDecoderTestPacked2()
        {
            BeforeClass();
            Before();
            byte[] input = { 1, 147, 124 };
            Assert.AreEqual(1937m, _decoder.Decode(input, Packed));
        }

        [TestMethod()]
        public void EbcdicDecoderTestPacked3()
        {
            BeforeClass();
            Before();
            byte[] input = { 1, 147, 124 };
            Assert.AreEqual(19.37m, _decoder.Decode(input, DecimalPacked));
        }

        [TestMethod()]
        public void EbcdicDecoderTestZoned1()
        {
            BeforeClass();
            Before();
            byte[] input = { 49, 57, 51, 71 };
            Assert.AreEqual(1937m, _decoder.Decode(input, Zoned));
        }

        [TestMethod()]
        public void EbcdicDecoderTestZoned2()
        {
            BeforeClass();
            Before();
            byte[] input = { 49, 57, 51, 80 };
            Assert.AreEqual(-1937m, _decoder.Decode(input, Zoned));
        }

        [TestMethod()]
        public void EbcdicDecoderTestZoned3()
        {
            BeforeClass();
            Before();
            byte[] input = { 49, 57, 51, 119 };
            Assert.AreEqual(-1937m, _decoder.Decode(input, Zoned));
        }

        [TestMethod()]
        public void EbcdicDecoderTestZoned4()
        {
            BeforeClass();
            Before();
            byte[] input = { 49, 57, 51, 39 };
            Assert.AreEqual(-1937m, _decoder.Decode(input, Zoned));
        }

        [TestMethod()]
        public void EbcdicDecoderTestZoned5()
        {
            BeforeClass();
            Before();
            byte[] input = { 49, 57, 51, 55 };
            Assert.AreEqual(1937m, _decoder.Decode(input, Zoned));
        }
    }
}
