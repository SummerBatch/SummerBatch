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
using Summer.Batch.Extra;

namespace Summer.Batch.CoreTests
{
    [TestClass()]
    public class DateParserTests
    {
        [TestMethod()]
        public void DecodeTest()
        {
            string birthday = "19700731";
            IDateParser dateParser = new DateParser();
            DateTime? bDay = dateParser.Decode(birthday);
            Assert.IsNotNull(bDay);
            Assert.AreEqual(31,bDay.Value.Day);
            Assert.AreEqual(1970, bDay.Value.Year);
            Assert.AreEqual(07, bDay.Value.Month);

            string wrong = "1970X031";
            DateTime? wrongDay = dateParser.Decode(wrong);
            Assert.IsNull(wrongDay);
        }

        [TestMethod()]
        public void DecodeTest1()
        {
            decimal birthday = 19700731.0m;
            IDateParser dateParser = new DateParser();
            DateTime? bDay = dateParser.Decode(birthday);
            Assert.IsNotNull(bDay);
            Assert.AreEqual(31, bDay.Value.Day);
            Assert.AreEqual(1970, bDay.Value.Year);
            Assert.AreEqual(07, bDay.Value.Month);

            decimal wrong = 0m;
            DateTime? wrongDay = dateParser.Decode(wrong);
            Assert.IsNull(wrongDay);

            decimal antics = -500731m;
            DateTime? anticsDay = dateParser.Decode(antics);
            Assert.IsNull(anticsDay);            
        }

        [TestMethod()]
        public void EncodeStringTest()
        {
            DateTime birthday = new DateTime(1970,07,31);
            IDateParser dateParser = new DateParser();
            string bday = dateParser.EncodeString(birthday);
            Assert.IsNotNull(bday);
            Assert.AreEqual("19700731",bday);
            DateTime max = new DateTime(9999, 12, 31);
            string maxDay = dateParser.EncodeString(max);
            Assert.IsNotNull(maxDay);
            Assert.AreEqual("99999999", maxDay);
        }

        [TestMethod()]        
        public void EncodeDecimalTest()
        {
            DateTime birthday = new DateTime(1970, 07, 31);
            IDateParser dateParser = new DateParser();
            decimal bday = dateParser.EncodeDecimal(birthday);
            Assert.IsNotNull(bday);
            Assert.AreEqual(19700731, bday);
            DateTime max = new DateTime(9999, 12, 31);
            decimal maxDay = dateParser.EncodeDecimal(max);
            Assert.IsNotNull(maxDay);
            Assert.AreEqual(99999999, maxDay);
        }
    }
}
