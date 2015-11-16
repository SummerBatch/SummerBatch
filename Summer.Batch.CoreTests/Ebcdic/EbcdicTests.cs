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
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Summer.Batch.Common.Extensions;
using Summer.Batch.Extra.Copybook;
using Summer.Batch.Extra.Ebcdic;
using Summer.Batch.Extra.Ebcdic.Encode;

namespace Summer.Batch.CoreTests.Ebcdic
{
    [TestClass]
    public class EbcdicTests
    {
        private const string Resources = "./Ebcdic/Resources";

        private const string Copybook = Resources + "/copybooks";
        private const string Inputs = Resources + "/inputs";
        private const string Outputs = "C:/temp/outputs";

        private static readonly byte[] Bytes = {200, 133, 147, 147, 150, 64, 
                                       230, 150, 153, 147, 132, 90,
                                       53, 54, 55, 56, 57, 58,
                                       255, 255, 255, 255, 248, 111,
                                       0, 1, 147, 124,0, 1, 
                                       147, 127,240, 241, 242, 243, 
                                       244, 75, 245, 214,241, 242, 
                                       243, 244, 245, 246, 247, 248};

        private static readonly object[] Objects = {"Hello World!",
                                        new byte[] {53, 54, 55, 56, 57, 58},
                                        -1937m,
                                        1937m,
                                        19.37m,
                                        -1234.56m,
                                        123456.78m};

        public static void BeforeClass()
        {
            if (Directory.Exists(Outputs))
            {
                Directory.Delete(Outputs, true);
                Directory.CreateDirectory(Outputs);
            }
            else
            {
                Directory.CreateDirectory(Outputs);
            }
        }

        [TestMethod]
        public void EbcdicTestsTestReader()
        {
            FileFormat fileFormat = CopybookLoader.LoadCopybook(Copybook + "/Test.fileformat");
            using (BufferedStream inputStream = new BufferedStream(new MemoryStream(Bytes)))
            {
                EbcdicReader reader = new EbcdicReader(inputStream, fileFormat, false);
                List<object> record = reader.NextRecord();
                Assert.AreEqual(Objects.Length, record.ToArray().Length);
                Assert.AreEqual(Objects.GetType(), record.ToArray().GetType());

                //NOTE : CollectionAssert does not support nested collection handling ...
                int index = 0;
                foreach (var rec in record.ToArray())
                {
                    var array = rec as Array;
                    if (array != null)
                    {
                        CollectionAssert.AreEqual((Array)Objects[index], array);
                    }
                    else
                    {
                        Assert.AreEqual(Objects[index], rec);
                    }
                    index++;
                }
            }
        }

        [TestMethod]
        public void EbcdicTestsTestWriter()
        {
            MemoryStream byteArrayOutputStream = new MemoryStream();
            FileFormat fileFormat = CopybookLoader.LoadCopybook(Copybook + "/Test.fileformat");
            using (BufferedStream outputStream = new BufferedStream(byteArrayOutputStream))
            {
                EbcdicWriter writer = new EbcdicWriter(outputStream, fileFormat.Charset, false,
                    EbcdicEncoder.DefaultValue.LowValue) {RecordFormatMap = new RecordFormatMap(fileFormat)};
                writer.WriteRecord(new List<object>(Objects));
            }
            CollectionAssert.AreEqual(Bytes, byteArrayOutputStream.ToArray());
        }

        [TestMethod]
        public void EbcdicTestsTest()
        {
            FieldFormat binary = new FieldFormat
            {
                Decimal = 0,
                Size = "6",
                Type = "B"
            };
            EbcdicEncoder encoder = new EbcdicEncoder("ascii");
            EbcdicDecoder decoder = new EbcdicDecoder("ascii");

            decimal value1 = -1937m;
            decimal value2 = 1937m;
            Assert.AreEqual(value1, decoder.Decode(encoder.Encode(value1, binary), binary));
            Assert.AreEqual(value2, decoder.Decode(encoder.Encode(value2, binary), binary));
        }


        [TestMethod]
        public void EbcdicTestsTestAddress()
        {
            Test("Address");
        }

        [TestMethod]
        public void EbcdicTestsTestCustomer()
        {
            Test("customer");
        }

        [TestMethod]
        public void EbcdicTestsTestEmployee()
        {
            Test("Employee");
        }

        [TestMethod]
        public void EbcdicTestsTestEmployee2()
        {
            Test("Employee2");
        }

        [TestMethod]
        public void EbcdicTestsTestMultipleRecords()
        {
            Test("MultipleRecords");
        }

        [TestMethod]
        public void EbcdicTestsTestDependingOn()
        {
            Test("dependingOn",Encoding.ASCII.GetBytes("\r\n"));
        }

        [TestMethod]
        public void EbcdicTestsTestFieldsGroup()
        {
            Test("fieldsGroup", Encoding.ASCII.GetBytes("\r\n"));
        }

        [TestMethod]
        public void EbcdicTestsTestFieldsGroupDependingOn()
        {
            Test("fieldsGroupDependingOn", Encoding.ASCII.GetBytes("\r\n"));
        }

        [TestMethod]
        public void EbcdicTestsTestRdw()
        {
            Test("rdwTest", new byte[0], true);
        }

        private void Test(string name)
        {
            Test(name, new byte[0]);
        }

        private void Test(String name, byte[] newLine, bool useRdw = false)
        {
            //BeforeClass();
            string input = Inputs +"/"+ name + ".txt";
            string output = Outputs + "/" + name + ".txt";
            string copybook = Copybook + "/" + name + ".fileformat";
            FileFormat fileFormat = CopybookLoader.LoadCopybook(copybook);
            using (BufferedStream inputStream = new BufferedStream(GetInputStream(input)))
            using (BufferedStream outputStream = new BufferedStream(new FileStream(output, FileMode.OpenOrCreate)))
            {
                EbcdicReader reader = new EbcdicReader(inputStream, fileFormat, useRdw);
                EbcdicWriter writer = new EbcdicWriter(outputStream, fileFormat.Charset, useRdw, EbcdicEncoder.DefaultValue.LowValue)
                {
                    RecordFormatMap = new RecordFormatMap(fileFormat)
                };


                List<Object> record = reader.NextRecord();
                while (record != null)
                {
                    writer.WriteRecord(record);
                    outputStream.Write(newLine);
                    record = reader.NextRecord();
                }
            }

            Assert.IsTrue(TestHelper.TestHelper.ContentEquals(GetInputStream(output), GetInputStream(input)));
        }

        private BufferedStream GetInputStream(string filePath)
        {
            return new BufferedStream(new FileStream(filePath, FileMode.Open));
        }
    }
}