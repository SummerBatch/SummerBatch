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
using System.Linq;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Summer.Batch.CoreTests.Util
{
    [TestClass]
    public class StringUtilsTest
    {

        #region Test concat.
        private static readonly string concat1 = StringUtils.Concat("bonjour ", "ça va ?");
        private static readonly string concat2 = StringUtils.Concat("bonjour ", null);
        private static readonly string concat3 = StringUtils.Concat(null, "ça va ?");

        [TestMethod]
        public void StringUtils_ConcatTest()
        {
            Assert.IsTrue(concat1.Equals("bonjour ça va ?"));
            Assert.IsTrue(concat2.Equals("bonjour "));
            Assert.IsTrue(concat3.Equals("ça va ?"));
        }
        #endregion

        #region Test IsValidEmailAddress.

        private static readonly bool email1 = StringUtils.IsValidEmailAddress("test@netfective.com");
        private static readonly bool email2 = StringUtils.IsValidEmailAddress("te.st@netfective.com");
        private static readonly bool email3 = StringUtils.IsValidEmailAddress("te_st@netfective.com");
        private static readonly bool email4 = StringUtils.IsValidEmailAddress("test-netfective.com");
        private static readonly bool email5 = StringUtils.IsValidEmailAddress("test.netfective.com");
        private static readonly bool email6 = StringUtils.IsValidEmailAddress(".com");

        [TestMethod]
        public void StringUtils_IsValidEmailAddressTest()
        {
            Assert.IsTrue(email1);
            Assert.IsTrue(email2);
            Assert.IsTrue(email3);
            Assert.IsFalse(email4);
            Assert.IsFalse(email5);
            Assert.IsFalse(email6);
        }
        #endregion

        #region Test IsDate.

        private static readonly bool date1 = StringUtils.IsDate("010189");
        private static readonly bool date2 = StringUtils.IsDate("010119");
        private static readonly bool date3 = StringUtils.IsDate("01/01/1989");
        private static readonly bool date4 = StringUtils.IsDate("31/02/2012");
        private static readonly bool date5 = StringUtils.IsDate("01011989");
        private static readonly bool date6 = StringUtils.IsDate("01/01/19");
        private static readonly bool date7 = StringUtils.IsDate("01/01/89");

        [TestMethod]
        public void StringUtils_IsDateTest()
        {
            Assert.IsFalse(date1);
            Assert.IsFalse(date2);
            Assert.IsTrue(date3);
            Assert.IsFalse(date4);
            Assert.IsFalse(date5);
            Assert.IsFalse(date6);
            Assert.IsFalse(date7);
        }
        #endregion

        #region Tests isDouble, isInteger.
        private static readonly bool isdouble1 = StringUtils.IsDouble("0");
        private static readonly bool isdouble2 = StringUtils.IsDouble("0,0");
        private static readonly bool isdouble3 = StringUtils.IsDouble("1");
        private static readonly bool isdouble4 = StringUtils.IsDouble("1,0");
        private static readonly bool isdouble5 = StringUtils.IsDouble("-1");
        private static readonly bool isdouble6 = StringUtils.IsDouble("-1,0");
        private static readonly bool isdouble7 = StringUtils.IsDouble("544867,454218");
        private static readonly bool isdouble8 = StringUtils.IsDouble("-604867,54");
        private static readonly bool isdouble9 = StringUtils.IsDouble("-2147483648");
        private static readonly bool isdouble10 = StringUtils.IsDouble("2147483648");
        private static readonly bool isdouble11 = StringUtils.IsDouble("1,79769313486232E+308");
        private static readonly bool isdouble12 = StringUtils.IsDouble("segdrg");
        private static readonly bool isdouble13 = StringUtils.IsDouble("");
        private static readonly bool isdouble14 = StringUtils.IsDouble(" ");
        private static readonly bool isdouble15 = StringUtils.IsDouble(null);

        private static readonly bool isinteger1 = StringUtils.IsInteger("0");
        private static readonly bool isinteger2 = StringUtils.IsInteger("0,0");
        private static readonly bool isinteger3 = StringUtils.IsInteger("1");
        private static readonly bool isinteger4 = StringUtils.IsInteger("1,0");
        private static readonly bool isinteger5 = StringUtils.IsInteger("-1");
        private static readonly bool isinteger6 = StringUtils.IsInteger("-1,0");
        private static readonly bool isinteger7 = StringUtils.IsInteger("544867,454218");
        private static readonly bool isinteger8 = StringUtils.IsInteger("-604867,54");
        private static readonly bool isinteger9 = StringUtils.IsInteger("-2147483648");
        private static readonly bool isinteger10 = StringUtils.IsInteger("2147483648");
        private static readonly bool isinteger11 = StringUtils.IsInteger("1,79769313486232E+308");
        private static readonly bool isinteger12 = StringUtils.IsInteger("segdrg");
        private static readonly bool isinteger13 = StringUtils.IsInteger("");
        private static readonly bool isinteger14 = StringUtils.IsInteger(" ");
        private static readonly bool isinteger15 = StringUtils.IsInteger(null);

        [TestMethod]
        public void StringUtils_IsNumberTest()
        {
            Assert.IsTrue(isdouble1);
            Assert.IsTrue(isdouble2);
            Assert.IsTrue(isdouble3);
            Assert.IsTrue(isdouble4);
            Assert.IsTrue(isdouble5);
            Assert.IsTrue(isdouble6);
            Assert.IsTrue(isdouble7);
            Assert.IsTrue(isdouble8);
            Assert.IsTrue(isdouble9);
            Assert.IsTrue(isdouble10);
            Assert.IsFalse(isdouble11);
            Assert.IsFalse(isdouble12);
            Assert.IsFalse(isdouble13);
            Assert.IsFalse(isdouble14);
            Assert.IsFalse(isdouble15);

            Assert.IsTrue(isinteger1);
            Assert.IsFalse(isinteger2);
            Assert.IsTrue(isinteger3);
            Assert.IsFalse(isinteger4);
            Assert.IsTrue(isinteger5);
            Assert.IsFalse(isinteger6);
            Assert.IsFalse(isinteger7);
            Assert.IsFalse(isinteger8);
            Assert.IsTrue(isinteger9);
            Assert.IsFalse(isinteger10);
            Assert.IsFalse(isinteger11);
            Assert.IsFalse(isinteger12);
            Assert.IsFalse(isinteger13);
            Assert.IsFalse(isinteger14);
            Assert.IsFalse(isinteger15);
        }
        #endregion

        #region Test CharAt.

        private static readonly string charatstring1 = "hello";
        private static readonly char charat0 = StringUtils.CharAt(charatstring1, 0);
        private static readonly char charat1 = StringUtils.CharAt(charatstring1, 1);
        private static readonly char charat2 = StringUtils.CharAt(charatstring1, 2);
        private static readonly char charat3 = StringUtils.CharAt(charatstring1, 3);
        private static readonly char charat4 = StringUtils.CharAt(charatstring1, 4);

        [TestMethod]
        public void StringUtils_CharAtTest()
        {
            Assert.AreEqual('h', charat0);
            Assert.AreEqual('e', charat1);
            Assert.AreEqual('l', charat2);
            Assert.AreEqual('l', charat3);
            Assert.AreEqual('o', charat4);
        }
        #endregion

        #region Test CompareTo, CompareToIgnoreCase.

        private static readonly string toCompare1 = "abcdef";
        private static readonly string toCompare2 = "AbCdEf";
        private static readonly string toCompare3 = "zyxwvu";

        private static readonly int compare1 = StringUtils.CompareTo(toCompare1, toCompare1);
        private static readonly int compare2 = StringUtils.CompareTo(toCompare1, toCompare2);
        private static readonly int compare3 = StringUtils.CompareTo(toCompare1, toCompare3);
        private static readonly int compare4 = StringUtils.CompareTo(toCompare3, toCompare1);

        private static readonly int compareIgnoreCase1 = StringUtils.CompareToIgnoreCase(toCompare1, toCompare1);
        private static readonly int compareIgnoreCase2 = StringUtils.CompareToIgnoreCase(toCompare1, toCompare2);
        private static readonly int compareIgnoreCase3 = StringUtils.CompareToIgnoreCase(toCompare1, toCompare3);
        private static readonly int compareIgnoreCase4 = StringUtils.CompareToIgnoreCase(toCompare3, toCompare1);

        [TestMethod]
        public void StringUtils_CompareToTest()
        {
            Assert.IsTrue(0 == compare1);
            Assert.IsTrue(0 != compare2);
            Assert.IsTrue(0 > compare3);
            Assert.IsTrue(-1 < compare4);

            Assert.IsTrue(0 == compareIgnoreCase1);
            Assert.IsTrue(0 == compareIgnoreCase2);
            Assert.IsTrue(0 > compareIgnoreCase3);
            Assert.IsTrue(-1 < compareIgnoreCase4);

        }
        #endregion

        #region Test GetBytes.

        private static readonly string getbytestest = "bonjour";
        private static readonly byte[] bytes1 = StringUtils.GetBytes(getbytestest);

        [TestMethod]
        public void StringUtils_GetBytesTest()
        {
            byte[] result = { 98, 111, 110, 106, 111, 117, 114 };
            Assert.IsTrue(result.SequenceEqual(bytes1));
        }
        #endregion

        #region Test Lenght.

        private static readonly bool lenght1 = StringUtils.Length("bonjour") == 7;
        private static readonly bool lenght2 = StringUtils.Length("Comment ça va ?") == 15;
        private static readonly bool lenght3 = StringUtils.Length("Nom: \nPrenom: ") == 14;
        private static readonly bool lenght4 = StringUtils.Length("") == 0;

        [TestMethod]
        public void StringUtils_LengthTest()
        {
            Assert.IsTrue(lenght1);
            Assert.IsTrue(lenght2);
            Assert.IsTrue(lenght3);
            Assert.IsTrue(lenght4);
        }
        #endregion

        #region Test Matches.

        private static readonly string matchesregex1 = "Mr\\.?";
        private static readonly bool matches1 = StringUtils.Matches("Mr Raymond Bar", matchesregex1);
        private static readonly bool matches2 = StringUtils.Matches("Mr. Raymond Bar", matchesregex1);
        private static readonly bool matches3 = StringUtils.Matches("Raymond Bar", matchesregex1);

        private static readonly string matchesregex2 = "[0-9]{1,3}";
        private static readonly bool matches4 = StringUtils.Matches("015", matchesregex2);
        private static readonly bool matches5 = StringUtils.Matches("0684", matchesregex2);
        private static readonly bool matches6 = StringUtils.Matches("aaa", matchesregex2);

        [TestMethod]
        public void StringUtils_MatchesTest()
        {
            Assert.IsTrue(matches1);
            Assert.IsTrue(matches2);
            Assert.IsFalse(matches3);
            Assert.IsTrue(matches4);
            Assert.IsTrue(matches5);
            Assert.IsFalse(matches6);
        }
        #endregion

        #region Test Replace(char,string,string), Replace(string,string,string), ReplaceAll, ReplaceFirst, Split.

        private static readonly string replacetest1 = "héhéhé, hehehe, ahahaah !";

        private static readonly bool replacchar1 = StringUtils.Replace(replacetest1, 'e', 'a').Equals("héhéhé, hahaha, ahahaah !");
        private static readonly bool replacchar2 = StringUtils.Replace(replacetest1, ',', '!').Equals("héhéhé! hehehe! ahahaah !");
        private static readonly bool replacchar3 = StringUtils.Replace(replacetest1, 'h', 'l').Equals("lélélé, lelele, alalaal !");

        private static readonly bool replace1 = StringUtils.Replace(replacetest1, "hehehe", "hahaha").Equals("héhéhé, hahaha, ahahaah !");
        private static readonly bool replace2 = StringUtils.Replace(replacetest1, ", ", "!!").Equals("héhéhé!!hehehe!!ahahaah !");

        private static readonly string replacetest2 = "516dqzdgrd876sefefs1546sefsef878";
        private static readonly string replaceregex1 = "[0-9]{3}";

        private static readonly bool replaceall1 = StringUtils.ReplaceAll(replacetest2, replaceregex1, "AAA").Equals("AAAdqzdgrdAAAsefefsAAA6sefsefAAA");
        private static readonly bool replacefirst1 = StringUtils.ReplaceFirst(replacetest2, replaceregex1, "AAA").Equals("AAAdqzdgrd876sefefs1546sefsef878");

        private static readonly string[] split1 = StringUtils.Split(replacetest2, replaceregex1);

        [TestMethod]
        public void StringUtils_ReplaceTest()
        {
            Assert.IsTrue(replacchar1);
            Assert.IsTrue(replacchar2);
            Assert.IsTrue(replacchar3);

            Assert.IsTrue(replace1);
            Assert.IsTrue(replace2);

            Assert.IsTrue(replaceall1);

            Assert.IsTrue(replacefirst1);

            Assert.IsTrue(split1[0].Equals(""));
            Assert.IsTrue(split1[1].Equals("dqzdgrd"));
            Assert.IsTrue(split1[2].Equals("sefefs"));
            Assert.IsTrue(split1[3].Equals("6sefsef"));
            Assert.IsTrue(split1[4].Equals(""));
        }
        #endregion

        #region Test ToCharArray.

        private static readonly char[] result2 = StringUtils.ToCharArray("salut !");

        [TestMethod]
        public void StringUtils_ToCharArrayTest()
        {
            char[] result1 = { 's', 'a', 'l', 'u', 't', ' ', '!' };
            Assert.IsTrue(result1.Length == result2.Length);
            for (int i = 0; i < result1.Length; i++)
            {
                Assert.IsTrue(result1[i] == result2[i]);
            }
        }
        #endregion

        #region Test ConvertIntegerToString(int), ConvertIntegerToString(int,int).

        private static readonly bool convertint1 = StringUtils.ConvertIntegerTostring(204).Equals("204");
        private static readonly bool convertint2 = StringUtils.ConvertIntegerTostring(null).Equals("");

        private static readonly bool convertintpad1 = StringUtils.ConvertIntegerTostring(204, 6).Equals("000204");
        private static readonly bool convertintpad2 = StringUtils.ConvertIntegerTostring(null, 6).Equals("");
        private static readonly bool convertintpad3 = StringUtils.ConvertIntegerTostring(204, null).Equals("204");

        [TestMethod]
        public void StringUtils_ConvertIntegerTostringTest()
        {
            Assert.IsTrue(convertint1);
            Assert.IsTrue(convertint2);

            Assert.IsTrue(convertintpad1);
            Assert.IsTrue(convertintpad2);
            Assert.IsTrue(convertintpad3);
        }
        #endregion

        #region Test IsEmpty, IsNotEmpty, IsBlank, IsNotBlank.

        private static readonly string emptyblank1 = "bonjour";
        private static readonly string emptyblank2 = "       ";
        private static readonly string emptyblank3 = "";
        private static readonly string emptyblank4 = null;

        private static readonly bool isempty1 = StringUtils.IsEmpty(emptyblank1);
        private static readonly bool isempty2 = StringUtils.IsEmpty(emptyblank2);
        private static readonly bool isempty3 = StringUtils.IsEmpty(emptyblank3);
        private static readonly bool isempty4 = StringUtils.IsEmpty(emptyblank4);

        private static readonly bool isnotempty1 = StringUtils.IsNotEmpty(emptyblank1);
        private static readonly bool isnotempty2 = StringUtils.IsNotEmpty(emptyblank2);
        private static readonly bool isnotempty3 = StringUtils.IsNotEmpty(emptyblank3);
        private static readonly bool isnotempty4 = StringUtils.IsNotEmpty(emptyblank4);

        private static readonly bool isblank1 = StringUtils.IsBlank(emptyblank1);
        private static readonly bool isblank2 = StringUtils.IsBlank(emptyblank2);
        private static readonly bool isblank3 = StringUtils.IsBlank(emptyblank3);
        private static readonly bool isblank4 = StringUtils.IsBlank(emptyblank4);

        private static readonly bool isnotblank1 = StringUtils.IsNotBlank(emptyblank1);
        private static readonly bool isnotblank2 = StringUtils.IsNotBlank(emptyblank2);
        private static readonly bool isnotblank3 = StringUtils.IsNotBlank(emptyblank3);
        private static readonly bool isnotblank4 = StringUtils.IsNotBlank(emptyblank4);

        [TestMethod]
        public void StringUtils_EmptyBlankTest()
        {
            Assert.IsFalse(isempty1);
            Assert.IsFalse(isempty2);
            Assert.IsTrue(isempty3);
            Assert.IsTrue(isempty4);

            Assert.IsTrue(isnotempty1);
            Assert.IsTrue(isnotempty2);
            Assert.IsFalse(isnotempty3);
            Assert.IsFalse(isnotempty4);

            Assert.IsFalse(isblank1);
            Assert.IsTrue(isblank2);
            Assert.IsTrue(isblank3);
            Assert.IsTrue(isblank4);

            Assert.IsTrue(isnotblank1);
            Assert.IsFalse(isnotblank2);
            Assert.IsFalse(isnotblank3);
            Assert.IsFalse(isnotblank4);
        }
        #endregion

        #region Test Trim.

        private static readonly bool trim1 = StringUtils.Trim(null) == null;
        private static readonly bool trim2 = StringUtils.Trim("").Equals("");
        private static readonly bool trim3 = StringUtils.Trim("     ").Equals("");
        private static readonly bool trim4 = StringUtils.Trim("abc").Equals("abc");
        private static readonly bool trim5 = StringUtils.Trim("     abc     ").Equals("abc");
        private static readonly bool trim6 = StringUtils.Trim("     abc    abc     ").Equals("abc    abc");

        [TestMethod]
        public void StringUtils_TrimTest()
        {
            Assert.IsTrue(trim1);
            Assert.IsTrue(trim2);
            Assert.IsTrue(trim3);
            Assert.IsTrue(trim4);
            Assert.IsTrue(trim5);
            Assert.IsTrue(trim6);
        }
        #endregion

        #region Test AreEquals, EqualsIgnoreCase.

        private static readonly bool areequals1 = StringUtils.AreEqual(null, null);
        private static readonly bool areequals2 = StringUtils.AreEqual(null, "abc");
        private static readonly bool areequals3 = StringUtils.AreEqual("abc", null);
        private static readonly bool areequals4 = StringUtils.AreEqual("abc", "abc");
        private static readonly bool areequals5 = StringUtils.AreEqual("abc", "ABC");

        private static readonly bool areequalsignorecase1 = StringUtils.EqualsIgnoreCase(null, null);
        private static readonly bool areequalsignorecase2 = StringUtils.EqualsIgnoreCase(null, "abc");
        private static readonly bool areequalsignorecase3 = StringUtils.EqualsIgnoreCase("abc", null);
        private static readonly bool areequalsignorecase4 = StringUtils.EqualsIgnoreCase("abc", "abc");
        private static readonly bool areequalsignorecase5 = StringUtils.EqualsIgnoreCase("abc", "ABC");

        [TestMethod]
        public void StringUtils_AreEqualsTest()
        {
            Assert.IsTrue(areequals1);
            Assert.IsFalse(areequals2);
            Assert.IsFalse(areequals3);
            Assert.IsTrue(areequals4);
            Assert.IsFalse(areequals5);

            Assert.IsTrue(areequalsignorecase1);
            Assert.IsFalse(areequalsignorecase2);
            Assert.IsFalse(areequalsignorecase3);
            Assert.IsTrue(areequalsignorecase4);
            Assert.IsTrue(areequalsignorecase5);
        }
        #endregion

        #region Test IndexOfTest, LastIndexOfTest.

        private static readonly bool indexof1 = StringUtils.IndexOf(null, '*') == -1;
        private static readonly bool indexof2 = StringUtils.IndexOf("", '*') == -1;
        private static readonly bool indexof3 = StringUtils.IndexOf("aabaabaa", 'a') == 0;
        private static readonly bool indexof4 = StringUtils.IndexOf("aabaabaa", 'b') == 2;

        private static readonly bool lastindexof1 = StringUtils.LastIndexOf(null, '*') == -1;
        private static readonly bool lastindexof2 = StringUtils.LastIndexOf("", '*') == -1;
        private static readonly bool lastindexof3 = StringUtils.LastIndexOf("aabaabaa", 'a') == 7;
        private static readonly bool lastindexof4 = StringUtils.LastIndexOf("aabaabaa", 'b') == 5;

        [TestMethod]
        public void StringUtils_IndexOfTest()
        {
            Assert.IsTrue(indexof1);
            Assert.IsTrue(indexof2);
            Assert.IsTrue(indexof3);
            Assert.IsTrue(indexof4);
            Assert.IsTrue(lastindexof1);
            Assert.IsTrue(lastindexof2);
            Assert.IsTrue(lastindexof3);
            Assert.IsTrue(lastindexof4);
        }
        #endregion

        #region Test Contains.

        private static readonly bool contains1 = StringUtils.Contains(null, '*');
        private static readonly bool contains2 = StringUtils.Contains("", '*');
        private static readonly bool contains3 = StringUtils.Contains("abc", 'a');
        private static readonly bool contains4 = StringUtils.Contains("abc", 'z');

        [TestMethod]
        public void StringUtils_ContainsTest()
        {
            Assert.IsFalse(contains1);
            Assert.IsFalse(contains2);
            Assert.IsTrue(contains3);
            Assert.IsFalse(contains4);
        }
        #endregion

        #region Test Substring, SubstringBefore, SubstringAfter.

        private static readonly bool substring1 = StringUtils.Substring(null, '*') == null;
        private static readonly bool substring2 = StringUtils.Substring("", '*').Equals("");
        private static readonly bool substring3 = StringUtils.Substring("abc", 0).Equals("abc");
        private static readonly bool substring4 = StringUtils.Substring("abc", 2).Equals("c");
        private static readonly bool substring5 = StringUtils.Substring("abc", 4).Equals("");
        private static readonly bool substring6 = StringUtils.Substring("abc", -2).Equals("bc");
        private static readonly bool substring7 = StringUtils.Substring("abc", -4).Equals("abc");

        private static readonly bool substringbefore1 = StringUtils.SubstringBefore(null, "*") == null;
        private static readonly bool substringbefore2 = StringUtils.SubstringBefore("", "*").Equals("");
        private static readonly bool substringbefore3 = StringUtils.SubstringBefore("abc", "a").Equals("");
        private static readonly bool substringbefore4 = StringUtils.SubstringBefore("abcba", "b").Equals("a");
        private static readonly bool substringbefore5 = StringUtils.SubstringBefore("abc", "c").Equals("ab");
        private static readonly bool substringbefore6 = StringUtils.SubstringBefore("abc", "d").Equals("abc");
        private static readonly bool substringbefore7 = StringUtils.SubstringBefore("abc", "").Equals("");
        private static readonly bool substringbefore8 = StringUtils.SubstringBefore("abc", null).Equals("abc");

        private static readonly bool substringafter1 = StringUtils.SubstringAfter(null, "*") == null;
        private static readonly bool substringafter2 = StringUtils.SubstringAfter("", "*").Equals("");
        private static readonly bool substringafter3 = StringUtils.SubstringAfter("*", null).Equals("");
        private static readonly bool substringafter4 = StringUtils.SubstringAfter("abc", "a").Equals("bc");
        private static readonly bool substringafter5 = StringUtils.SubstringAfter("abcba", "b").Equals("cba");
        private static readonly bool substringafter6 = StringUtils.SubstringAfter("abc", "c").Equals("");
        private static readonly bool substringafter7 = StringUtils.SubstringAfter("abc", "d").Equals("");
        private static readonly bool substringafter8 = StringUtils.SubstringAfter("abc", "").Equals("abc");

        private static readonly bool substring11 = StringUtils.Substring("abcd", 1, 2).Equals("bc");

        [TestMethod]
        public void StringUtils_SubstringTest()
        {
            Assert.IsTrue(substring1);
            Assert.IsTrue(substring2);
            Assert.IsTrue(substring3);
            Assert.IsTrue(substring4);
            Assert.IsTrue(substring5);
            Assert.IsTrue(substring6);
            Assert.IsTrue(substring7);

            Assert.IsTrue(substringbefore1);
            Assert.IsTrue(substringbefore2);
            Assert.IsTrue(substringbefore3);
            Assert.IsTrue(substringbefore4);
            Assert.IsTrue(substringbefore5);
            Assert.IsTrue(substringbefore6);
            Assert.IsTrue(substringbefore7);
            Assert.IsTrue(substringbefore8);

            Assert.IsTrue(substringafter1);
            Assert.IsTrue(substringafter2);
            Assert.IsTrue(substringafter3);
            Assert.IsTrue(substringafter4);
            Assert.IsTrue(substringafter5);
            Assert.IsTrue(substringafter6);
            Assert.IsTrue(substringafter7);
            Assert.IsTrue(substringafter8);

            Assert.IsTrue(substring11);
        }
        #endregion

        #region Test Chomp, Chop, StripEnd.

        private static readonly bool chomp1 = StringUtils.Chomp(null) == null;
        private static readonly bool chomp2 = StringUtils.Chomp("").Equals("");
        private static readonly bool chomp3 = StringUtils.Chomp("abc \r").Equals("abc ");
        private static readonly bool chomp4 = StringUtils.Chomp("abc\n").Equals("abc");
        private static readonly bool chomp5 = StringUtils.Chomp("abc\r\n").Equals("abc");
        private static readonly bool chomp6 = StringUtils.Chomp("abc\r\n\r\n").Equals("abc\r\n");
        private static readonly bool chomp7 = StringUtils.Chomp("abc\n\r").Equals("abc\n");
        private static readonly bool chomp8 = StringUtils.Chomp("abc\n\rabc").Equals("abc\n\rabc");
        private static readonly bool chomp9 = StringUtils.Chomp("\r").Equals("");
        private static readonly bool chomp10 = StringUtils.Chomp("\n").Equals("");
        private static readonly bool chomp11 = StringUtils.Chomp("\r\n").Equals("");

        private static readonly bool chop1 = StringUtils.Chop(null) == null;
        private static readonly bool chop2 = StringUtils.Chop("").Equals("");
        private static readonly bool chop3 = StringUtils.Chop("abc \r").Equals("abc ");
        private static readonly bool chop4 = StringUtils.Chop("abc\n").Equals("abc");
        private static readonly bool chop5 = StringUtils.Chop("abc\r\n").Equals("abc");
        private static readonly bool chop6 = StringUtils.Chop("abc").Equals("ab");
        private static readonly bool chop7 = StringUtils.Chop("abc\nabc").Equals("abc\nab");
        private static readonly bool chop8 = StringUtils.Chop("a").Equals("");
        private static readonly bool chop9 = StringUtils.Chop("\r").Equals("");
        private static readonly bool chop10 = StringUtils.Chop("\n").Equals("");
        private static readonly bool chop11 = StringUtils.Chop("\r\n").Equals("");

        private static readonly bool stripend1 = StringUtils.StripEnd(null, "*") == null;
        private static readonly bool stripend2 = StringUtils.StripEnd("", "*").Equals("");
        private static readonly bool stripend3 = StringUtils.StripEnd("abc", "").Equals("abc");
        private static readonly bool stripend4 = StringUtils.StripEnd("abc", null).Equals("abc");
        private static readonly bool stripend5 = StringUtils.StripEnd("  abc", null).Equals("  abc");
        private static readonly bool stripend6 = StringUtils.StripEnd("abc  ", null).Equals("abc");
        private static readonly bool stripend7 = StringUtils.StripEnd(" abc ", null).Equals(" abc");
        private static readonly bool stripend8 = StringUtils.StripEnd("  abcyx", "xyz").Equals("  abc");
        private static readonly bool stripend9 = StringUtils.StripEnd("120.00", ".0").Equals("12");

        [TestMethod]
        public void StringUtils_ChompChopStripEndTest()
        {
            Assert.IsTrue(chomp1);
            Assert.IsTrue(chomp2);
            Assert.IsTrue(chomp3);
            Assert.IsTrue(chomp4);
            Assert.IsTrue(chomp5);
            Assert.IsTrue(chomp6);
            Assert.IsTrue(chomp7);
            Assert.IsTrue(chomp8);
            Assert.IsTrue(chomp9);
            Assert.IsTrue(chomp10);
            Assert.IsTrue(chomp11);

            Assert.IsTrue(chop1);
            Assert.IsTrue(chop2);
            Assert.IsTrue(chop3);
            Assert.IsTrue(chop4);
            Assert.IsTrue(chop5);
            Assert.IsTrue(chop6);
            Assert.IsTrue(chop7);
            Assert.IsTrue(chop8);
            Assert.IsTrue(chop9);
            Assert.IsTrue(chop10);
            Assert.IsTrue(chop11);

            Assert.IsTrue(stripend1);
            Assert.IsTrue(stripend2);
            Assert.IsTrue(stripend3);
            Assert.IsTrue(stripend4);
            Assert.IsTrue(stripend5);
            Assert.IsTrue(stripend6);
            Assert.IsTrue(stripend7);
            Assert.IsTrue(stripend8);
            Assert.IsTrue(stripend9);
        }
        #endregion

        #region Test Repeat.

        private static readonly bool repeat1 = StringUtils.Repeat(null, 2) == null;
        private static readonly bool repeat2 = StringUtils.Repeat("", 0).Equals("");
        private static readonly bool repeat3 = StringUtils.Repeat("", 2).Equals("");
        private static readonly bool repeat4 = StringUtils.Repeat("a", 3).Equals("aaa");
        private static readonly bool repeat5 = StringUtils.Repeat("ab", 2).Equals("abab");
        private static readonly bool repeat6 = StringUtils.Repeat("a", -2).Equals("");

        [TestMethod]
        public void StringUtils_RepeatTest()
        {
            Assert.IsTrue(repeat1);
            Assert.IsTrue(repeat2);
            Assert.IsTrue(repeat3);
            Assert.IsTrue(repeat4);
            Assert.IsTrue(repeat5);
            Assert.IsTrue(repeat6);
        }
        #endregion

        #region Test UpperCase, LowerCase.

        private static readonly bool lower1 = StringUtils.LowerCase(null) == null;
        private static readonly bool lower2 = StringUtils.LowerCase("").Equals("");
        private static readonly bool lower3 = StringUtils.LowerCase("aBc").Equals("abc");

        private static readonly bool upper1 = StringUtils.UpperCase(null) == null;
        private static readonly bool upper2 = StringUtils.UpperCase("").Equals("");
        private static readonly bool upper3 = StringUtils.UpperCase("aBc").Equals("ABC");

        [TestMethod]
        public void StringUtils_CaseTest()
        {
            Assert.IsTrue(lower1);
            Assert.IsTrue(lower2);
            Assert.IsTrue(lower3);
            Assert.IsTrue(upper1);
            Assert.IsTrue(upper2);
            Assert.IsTrue(upper3);
        }
        #endregion

        #region Test IsAlpha, IsAlphaSpace, IsNumeric, IsNumericSpace, IsAlphaNumeric, IsAlphaNumericSpace, IsWhiteSpace.

        private static readonly bool alpha1 = StringUtils.IsAlpha(null).Equals(false);
        private static readonly bool alpha2 = StringUtils.IsAlpha("").Equals(false);
        private static readonly bool alpha3 = StringUtils.IsAlpha("  ").Equals(false);
        private static readonly bool alpha4 = StringUtils.IsAlpha("abc").Equals(true);
        private static readonly bool alpha5 = StringUtils.IsAlpha("ab2c").Equals(false);
        private static readonly bool alpha6 = StringUtils.IsAlpha("ab-c").Equals(false);

        private static readonly bool alphaspace1 = StringUtils.IsAlphaSpace(null).Equals(false);
        private static readonly bool alphaspace2 = StringUtils.IsAlphaSpace("").Equals(true);
        private static readonly bool alphaspace3 = StringUtils.IsAlphaSpace("  ").Equals(true);
        private static readonly bool alphaspace4 = StringUtils.IsAlphaSpace("abc").Equals(true);
        private static readonly bool alphaspace5 = StringUtils.IsAlphaSpace("ab c").Equals(true);
        private static readonly bool alphaspace6 = StringUtils.IsAlphaSpace("ab2c").Equals(false);
        private static readonly bool alphaspace7 = StringUtils.IsAlphaSpace("ab-c").Equals(false);

        private static readonly bool alphanumeric1 = StringUtils.IsAlphanumeric(null).Equals(false);
        private static readonly bool alphanumeric2 = StringUtils.IsAlphanumeric("").Equals(false);
        private static readonly bool alphanumeric3 = StringUtils.IsAlphanumeric("  ").Equals(false);
        private static readonly bool alphanumeric4 = StringUtils.IsAlphanumeric("abc").Equals(true);
        private static readonly bool alphanumeric5 = StringUtils.IsAlphanumeric("ab c").Equals(false);
        private static readonly bool alphanumeric6 = StringUtils.IsAlphanumeric("ab2c").Equals(true);
        private static readonly bool alphanumeric7 = StringUtils.IsAlphanumeric("ab-c").Equals(false);

        private static readonly bool alphanumericspace1 = StringUtils.IsAlphanumericSpace(null).Equals(false);
        private static readonly bool alphanumericspace2 = StringUtils.IsAlphanumericSpace("").Equals(true);
        private static readonly bool alphanumericspace3 = StringUtils.IsAlphanumericSpace("  ").Equals(true);
        private static readonly bool alphanumericspace4 = StringUtils.IsAlphanumericSpace("abc").Equals(true);
        private static readonly bool alphanumericspace5 = StringUtils.IsAlphanumericSpace("ab c").Equals(true);
        private static readonly bool alphanumericspace6 = StringUtils.IsAlphanumericSpace("ab2c").Equals(true);
        private static readonly bool alphanumericspace7 = StringUtils.IsAlphanumericSpace("ab-c").Equals(false);

        private static readonly bool numeric1 = StringUtils.IsNumeric(null).Equals(false);
        private static readonly bool numeric2 = StringUtils.IsNumeric("").Equals(false);
        private static readonly bool numeric3 = StringUtils.IsNumeric("  ").Equals(false);
        private static readonly bool numeric4 = StringUtils.IsNumeric("123").Equals(true);
        private static readonly bool numeric5 = StringUtils.IsNumeric("12 3").Equals(false);
        private static readonly bool numeric6 = StringUtils.IsNumeric("ab2c").Equals(false);
        private static readonly bool numeric7 = StringUtils.IsNumeric("12-3").Equals(false);
        private static readonly bool numeric8 = StringUtils.IsNumeric("12.3").Equals(false);
        private static readonly bool numeric9 = StringUtils.IsNumeric("-123").Equals(false);
        private static readonly bool numeric10 = StringUtils.IsNumeric("+123").Equals(false);

        private static readonly bool numericspace1 = StringUtils.IsNumericSpace(null).Equals(false);
        private static readonly bool numericspace2 = StringUtils.IsNumericSpace("").Equals(true);
        private static readonly bool numericspace3 = StringUtils.IsNumericSpace("  ").Equals(true);
        private static readonly bool numericspace4 = StringUtils.IsNumericSpace("123").Equals(true);
        private static readonly bool numericspace5 = StringUtils.IsNumericSpace("12 3").Equals(true);
        private static readonly bool numericspace6 = StringUtils.IsNumericSpace("ab2c").Equals(false);
        private static readonly bool numericspace7 = StringUtils.IsNumericSpace("12-3").Equals(false);
        private static readonly bool numericspace8 = StringUtils.IsNumericSpace("12.3").Equals(false);

        private static readonly bool whitespace1 = StringUtils.IsWhitespace(null).Equals(false);
        private static readonly bool whitespace2 = StringUtils.IsWhitespace("").Equals(true);
        private static readonly bool whitespace3 = StringUtils.IsWhitespace("  ").Equals(true);
        private static readonly bool whitespace4 = StringUtils.IsWhitespace("abc").Equals(false);
        private static readonly bool whitespace5 = StringUtils.IsWhitespace("ab2c").Equals(false);
        private static readonly bool whitespace6 = StringUtils.IsWhitespace("ab-c").Equals(false);

        [TestMethod]
        public void StringUtils_AlphaNumericTest()
        {
            Assert.IsTrue(alpha1);
            Assert.IsTrue(alpha2);
            Assert.IsTrue(alpha3);
            Assert.IsTrue(alpha4);
            Assert.IsTrue(alpha5);
            Assert.IsTrue(alpha6);

            Assert.IsTrue(alphaspace1);
            Assert.IsTrue(alphaspace2);
            Assert.IsTrue(alphaspace3);
            Assert.IsTrue(alphaspace4);
            Assert.IsTrue(alphaspace5);
            Assert.IsTrue(alphaspace6);
            Assert.IsTrue(alphaspace7);

            Assert.IsTrue(alphanumeric1);
            Assert.IsTrue(alphanumeric2);
            Assert.IsTrue(alphanumeric3);
            Assert.IsTrue(alphanumeric4);
            Assert.IsTrue(alphanumeric5);
            Assert.IsTrue(alphanumeric6);
            Assert.IsTrue(alphanumeric7);

            Assert.IsTrue(alphanumericspace1);
            Assert.IsTrue(alphanumericspace2);
            Assert.IsTrue(alphanumericspace3);
            Assert.IsTrue(alphanumericspace4);
            Assert.IsTrue(alphanumericspace5);
            Assert.IsTrue(alphanumericspace6);
            Assert.IsTrue(alphanumericspace7);

            Assert.IsTrue(numeric1);
            Assert.IsTrue(numeric2);
            Assert.IsTrue(numeric3);
            Assert.IsTrue(numeric4);
            Assert.IsTrue(numeric5);
            Assert.IsTrue(numeric6);
            Assert.IsTrue(numeric7);
            Assert.IsTrue(numeric8);
            Assert.IsTrue(numeric9);
            Assert.IsTrue(numeric10);

            Assert.IsTrue(numericspace1);
            Assert.IsTrue(numericspace2);
            Assert.IsTrue(numericspace3);
            Assert.IsTrue(numericspace4);
            Assert.IsTrue(numericspace5);
            Assert.IsTrue(numericspace6);
            Assert.IsTrue(numericspace7);
            Assert.IsTrue(numericspace8);

            Assert.IsTrue(whitespace1);
            Assert.IsTrue(whitespace2);
            Assert.IsTrue(whitespace3);
            Assert.IsTrue(whitespace4);
            Assert.IsTrue(whitespace5);
            Assert.IsTrue(whitespace6);
        }
        #endregion

        #region Test Reverse.

        public static readonly bool reverse1 = StringUtils.Reverse(null) == null;
        public static readonly bool reverse2 = StringUtils.Reverse("").Equals("");
        public static readonly bool reverse3 = StringUtils.Reverse("bat").Equals("tab");

        [TestMethod]
        public void StringUtils_ReverseTest()
        {
            Assert.IsTrue(reverse1);
            Assert.IsTrue(reverse2);
            Assert.IsTrue(reverse3);
        }
        #endregion
    }
}
