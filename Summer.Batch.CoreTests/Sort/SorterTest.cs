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
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Summer.Batch.Core;
using Summer.Batch.Extra.Sort;
using Summer.Batch.Extra.Sort.Comparer;
using Summer.Batch.Extra.Sort.Filter;
using Summer.Batch.Extra.Sort.Legacy;
using Summer.Batch.Extra.Sort.Legacy.Accessor;
using Summer.Batch.Extra.Sort.Legacy.Comparer;
using Summer.Batch.Extra.Sort.Legacy.Filter;
using Summer.Batch.Extra.Sort.Legacy.Format;
using Summer.Batch.Extra.Sort.Legacy.Parser;
using Summer.Batch.Common.IO;

namespace Summer.Batch.CoreTests.Sort
{
    [TestClass]
    public class SorterTest
    {
        private static readonly byte[] CrLf = Encoding.Default.GetBytes("\r\n");
        private static readonly byte[] Lf = Encoding.Default.GetBytes("\n");

        private static readonly Encoding Cp1147 = Encoding.GetEncoding("IBM01147");
        private static readonly Encoding Cp1047 = Encoding.GetEncoding("IBM01047");
        private static readonly Encoding Cp1252 = Encoding.GetEncoding("Windows-1252");

        [TestMethod]
        public void TestSort1()
        {
            var sorter = new Sorter<byte[]>
            {
                RecordAccessorFactory = new SeparatorAccessorFactory { Separator = CrLf },
                HeaderSize = 1,
                Comparer = new ComparerChain<byte[]>
                {
                    Comparers = new List<IComparer<byte[]>>
                    {
                        new StringComparer { Start = 0, Length = 25 },
                        new DefaultComparer<decimal>
                        {
                            Ascending = false,
                            Accessor = new ZonedAccessor { Start = 25, Length = 10 }
                        }
                    }
                },
                Filter = new DecimalFilter
                {
                    Left = new ZonedAccessor { Start = 35, Length = 1 },
                    Right = new ConstantAccessor<decimal> { Constant = 5 },
                    Operator = ComparisonOperator.Ne
                }
            };

            var input = new List<FileInfo>
            {
                new FileInfo(@"TestData\Sort\Input\sort1a.txt"),
                new FileInfo(@"TestData\Sort\Input\sort1b.txt")
            };
            var output = new FileInfo(@"TestData\Sort\Output\sort1.txt");
            var expected = new FileInfo(@"TestData\Sort\Expected\sort1.txt");

            sorter.Sort(input, output);

            Assert.IsTrue(TestHelper.TestHelper.ContentEquals(expected.OpenRead(), output.OpenRead()));
        }

        [TestMethod]
        public void TestSort1Parsed()
        {
            var comparerParser = new ComparerParser();
            var filterParser = new FilterParser();
            var sorter = new Sorter<byte[]>
            {
                RecordAccessorFactory = new SeparatorAccessorFactory { Separator = CrLf },
                HeaderSize = 1,
                Comparer = comparerParser.GetComparer("FORMAT=CH,FIELDS=(1,25,A,26,10,ZD,D)"),
                Filter = filterParser.GetFilter(null, "(36,1,ZD,EQ,5)")
            };

            var input = new List<FileInfo>
            {
                new FileInfo(@"TestData\Sort\Input\sort1a.txt"),
                new FileInfo(@"TestData\Sort\Input\sort1b.txt")
            };
            var output = new FileInfo(@"TestData\Sort\Output\sort1p.txt");
            var expected = new FileInfo(@"TestData\Sort\Expected\sort1.txt");

            sorter.Sort(input, output);

            Assert.IsTrue(TestHelper.TestHelper.ContentEquals(expected.OpenRead(), output.OpenRead()));
        }

        [TestMethod]
        public void TestSort1Tasklet()
        {
            var output = new FileInfo(@"TestData\Sort\Output\sort1t.txt");
            var expected = new FileInfo(@"TestData\Sort\Expected\sort1.txt");
            var sortTasklet = new SortTasklet
            {
                Separator = "\r\n",
                HeaderSize = 1,
                SortCard = "FORMAT=CH,FIELDS=(1,25,A,26,10,ZD,D)",
                Omit = "(36,1,ZD,EQ,5)",
                Input = new List<IResource>
                {
                    new FileSystemResource(new FileInfo(@"TestData\Sort\Input\sort1a.txt")),
                    new FileSystemResource(new FileInfo(@"TestData\Sort\Input\sort1b.txt"))
                },
                Output = new FileSystemResource(output)
            };

            sortTasklet.Execute(new StepContribution(new StepExecution("sort", new JobExecution(1))), null);

            Assert.IsTrue(TestHelper.TestHelper.ContentEquals(expected.OpenRead(), output.OpenRead()));
        }

        [TestMethod]
        public void TestSort2()
        {
            var sorter = new Sorter<byte[]>
            {
                RecordAccessorFactory = new SeparatorAccessorFactory { Separator = Lf },
                Filter = new ConjunctionFilter<byte[]>
                {
                    Filters = new List<IFilter<byte[]>>
                    {
                        new StringFilter
                        {
                            Left = new StringAccessor { Start = 0, Length = 2 },
                            Right = new ConstantAccessor<string> { Constant = "99" },
                            Operator = ComparisonOperator.Eq
                        },
                        new StringFilter
                        {
                            Left = new StringAccessor { Start = 21, Length = 1 },
                            Right = new ConstantAccessor<string> { Constant = "&" },
                            Operator = ComparisonOperator.Eq
                        }
                    }
                }
            };

            var input = new List<FileInfo>
            {
                new FileInfo(@"TestData\Sort\Input\sort2.txt")
            };
            var output = new FileInfo(@"TestData\Sort\Output\sort2.txt");
            var expected = new FileInfo(@"TestData\Sort\Expected\sort2.txt");

            sorter.Sort(input, output);

            Assert.IsTrue(TestHelper.TestHelper.ContentEquals(expected.OpenRead(), output.OpenRead()));
        }

        [TestMethod]
        public void TestSort2Tasklet()
        {
            var output = new FileInfo(@"TestData\Sort\Output\sort2t.txt");
            var expected = new FileInfo(@"TestData\Sort\Expected\sort2.txt");

            var sortTasklet = new SortTasklet
            {
                Separator = "\n",
                Include = "(1,2,CH,EQ,C'99',AND,22,1,CH,EQ,C'&')",
                SortEncoding = Encoding.GetEncoding("IBM01147"),
                Input = new List<IResource> { new FileSystemResource(new FileInfo(@"TestData\Sort\Input\sort2.txt")) },
                Output = new FileSystemResource(output)
            };

            sortTasklet.Execute(new StepContribution(new StepExecution("sort", new JobExecution(1))), null);

            Assert.IsTrue(TestHelper.TestHelper.ContentEquals(expected.OpenRead(), output.OpenRead()));
        }

        [TestMethod]
        public void TestSort3()
        {
            var sorter = new Sorter<byte[]>
            {
                RecordAccessorFactory = new SeparatorAccessorFactory { Separator = Lf },
                Comparer = new ComparerChain<byte[]>
                {
                    Comparers = new List<IComparer<byte[]>>
                    {
                        new StringComparer { Start = 8, Length = 2, SortEncoding = Cp1147 },
                        new StringComparer { Start = 258, Length = 6, Ascending = false },
                        new StringComparer { Start = 16, Length = 3, SortEncoding = Cp1147 },
                        new StringComparer { Start = 295, Length = 1, Ascending = false, SortEncoding = Cp1147 }
                    }
                },
                Sum = new BytesSum
                {
                    Accessors = new List<IAccessor<decimal>>
                    {
                        new ZonedAccessor { Start = 232, Length = 12 }
                    }
                }
            };

            var input = new List<FileInfo>
            {
                new FileInfo(@"TestData\Sort\Input\sort3.txt")
            };
            var output = new FileInfo(@"TestData\Sort\Output\sort3.txt");
            var expected = new FileInfo(@"TestData\Sort\Expected\sort3.txt");

            sorter.Sort(input, output);

            Assert.IsTrue(TestHelper.TestHelper.ContentEquals(expected.OpenRead(), output.OpenRead()));
        }

        [TestMethod]
        public void TestSort3Tasklet()
        {
            var output = new FileInfo(@"TestData\Sort\Output\sort3t.txt");
            var expected = new FileInfo(@"TestData\Sort\Expected\sort3.txt");

            var sortTasklet = new SortTasklet
            {
                Separator = "\n",
                SortCard = "9,2,CH,A,259,6,CH,D,17,3,CH,A,296,1,CH,D",
                Sum = "233,12,ZD",
                Input = new List<IResource> { new FileSystemResource(new FileInfo(@"TestData\Sort\Input\sort3.txt")) },
                Output = new FileSystemResource(output)
            };

            sortTasklet.Execute(new StepContribution(new StepExecution("sort", new JobExecution(1))), null);

            Assert.IsTrue(TestHelper.TestHelper.ContentEquals(expected.OpenRead(), output.OpenRead()));
        }

        [TestMethod]
        public void TestSort4()
        {
            var sorter = new Sorter<byte[]>
            {
                RecordAccessorFactory = new SeparatorAccessorFactory { Separator = Lf },
                Comparer = new StringComparer { Start = 0, Length = 23, SortEncoding = Cp1147 },
                InputFormatter = new LegacyFormatter
                {
                    Encoding = Cp1252,
                    Formatters = new List<ISubFormatter>
                    {
                        new CopyFormatter { InputIndex = 4, Length = 6 },
                        new CopyFormatter { InputIndex = 299, Length = 3, OutputIndex = 6 },
                        new CopyFormatter { InputIndex = 11, Length = 14, OutputIndex = 9 },
                        new CopyFormatter { InputIndex = 59, Length = 40, OutputIndex = 23 },
                        new CopyFormatter { InputIndex = 10, Length = 1, OutputIndex = 63 },
                        new ConstantFormatter { Constant = Cp1252.GetBytes(" "), OutputIndex = 64 },
                        new CopyFormatter { InputIndex = 140, Length = 3, OutputIndex = 65 },
                        new ConstantFormatter { Constant = Cp1252.GetBytes("                      "), OutputIndex = 68 }
                    }
                }
            };

            var input = new List<FileInfo>
            {
                new FileInfo(@"TestData\Sort\Input\sort4.txt")
            };
            var output = new FileInfo(@"TestData\Sort\Output\sort4.txt");
            var expected = new FileInfo(@"TestData\Sort\Expected\sort4.txt");

            sorter.Sort(input, output);

            Assert.IsTrue(TestHelper.TestHelper.ContentEquals(expected.OpenRead(), output.OpenRead()));
        }

        [TestMethod]
        public void TestSort4Tasklet()
        {
            var output = new FileInfo(@"TestData\Sort\Output\sort4t.txt");
            var expected = new FileInfo(@"TestData\Sort\Expected\sort4.txt");

            var sortTasklet = new SortTasklet
            {
                Separator = "\n",
                SortCard = "(1,23,CH,A)",
                Inrec = "5,6,300,3,12,14,60,40,11,1,X,141,3,22X",
                Encoding = Cp1252,
                SortEncoding = Cp1147,
                Input = new List<IResource> { new FileSystemResource(new FileInfo(@"TestData\Sort\Input\sort4.txt")) },
                Output = new FileSystemResource(output)
            };

            sortTasklet.Execute(new StepContribution(new StepExecution("sort", new JobExecution(1))), null);

            Assert.IsTrue(TestHelper.TestHelper.ContentEquals(expected.OpenRead(), output.OpenRead()));
        }

        [TestMethod]
        public void TestSort5()
        {
            var sorter = new Sorter<byte[]>
            {
                RecordAccessorFactory = new SeparatorAccessorFactory { Separator = Lf },
                Comparer = new StringComparer { Start = 0, Length = 6, SortEncoding = Cp1147 },
                Filter = new StringFilter
                {
                    Left = new StringAccessor { Start = 74, Length = 7, Encoding = Cp1252 },
                    Right = new ConstantAccessor<string> { Constant = "       " },
                    Operator = ComparisonOperator.Ne
                },
                OutputFormatter = new LegacyFormatter
                {
                    Formatters = new List<ISubFormatter>
                    {
                        new CopyFormatter { InputIndex = 25, Length = 6 },
                        new CopyFormatter { InputIndex = 74, Length = 7, OutputIndex = 6 },
                        new CopyFormatter { InputIndex = 81, Length = 40, OutputIndex = 13 },
                        new ConstantFormatter { Constant = Encoding.Default.GetBytes("  "), OutputIndex = 53 },
                        new CopyFormatter { InputIndex = 227, Length = 80, OutputIndex = 55 },
                        new CopyFormatter { InputIndex = 680, Length = 4, OutputIndex = 135 },
                        new ConstantFormatter { Constant = Encoding.Default.GetBytes("           "), OutputIndex = 139 }
                    }
                }
            };

            var input = new List<FileInfo>
            {
                new FileInfo(@"TestData\Sort\Input\sort5.txt")
            };
            var output = new FileInfo(@"TestData\Sort\Output\sort5.txt");
            var expected = new FileInfo(@"TestData\Sort\Expected\sort5.txt");

            sorter.Sort(input, output);

            Assert.IsTrue(TestHelper.TestHelper.ContentEquals(expected.OpenRead(), output.OpenRead()));
        }

        [TestMethod]
        public void TestSort5Tasklet()
        {
            var output = new FileInfo(@"TestData\Sort\Output\sort5t.txt");
            var expected = new FileInfo(@"TestData\Sort\Expected\sort5.txt");

            var sortTasklet = new SortTasklet
            {
                Separator = "\n",
                SortCard = "1,6,CH,A",
                Omit = "75,7,CH,EQ,C'       '",
                Outrec = "26,6,75,7,82,40,2X,228,80,681,4,11X",
                Encoding = Cp1252,
                SortEncoding = Cp1147,
                Input = new List<IResource> { new FileSystemResource(new FileInfo(@"TestData\Sort\Input\sort5.txt")) },
                Output = new FileSystemResource(output)
            };

            sortTasklet.Execute(new StepContribution(new StepExecution("sort", new JobExecution(1))), null);

            Assert.IsTrue(TestHelper.TestHelper.ContentEquals(expected.OpenRead(), output.OpenRead()));
        }

        [TestMethod]
        public void TestSort6Tasklet()
        {
            var output = new FileInfo(@"TestData\Sort\Output\sort6t.txt");
            var expected = new FileInfo(@"TestData\Sort\Expected\sort6.txt");

            var sortTasklet = new SortTasklet
            {
                Separator = "\r\n",
                Input = new List<IResource> { new FileSystemResource(new FileInfo(@"TestData\Sort\Input\sort6.txt")) },
                Output = new FileSystemResource(output)
            };

            sortTasklet.Execute(new StepContribution(new StepExecution("sort", new JobExecution(1))), null);

            Assert.IsTrue(TestHelper.TestHelper.ContentEquals(expected.OpenRead(), output.OpenRead()));
        }

        [TestMethod]
        public void TestSort7Tasklet()
        {
            var output = new FileInfo(@"TestData\Sort\Output\sort7t.txt");
            var expected = new FileInfo(@"TestData\Sort\Expected\sort7.txt");

            var sortTasklet = new SortTasklet
            {
                RecordLength = 2,
                SortCard = "(1,2,ZD,A)",
                Input = new List<IResource> { new FileSystemResource(new FileInfo(@"TestData\Sort\Input\sort7.txt")) },
                Output = new FileSystemResource(output)
            };

            sortTasklet.Execute(new StepContribution(new StepExecution("sort", new JobExecution(1))), null);

            Assert.IsTrue(TestHelper.TestHelper.ContentEquals(expected.OpenRead(), output.OpenRead()));
        }

        [TestMethod]
        public void TestSort8Tasklet()
        {
            var output = new FileInfo(@"TestData\Sort\Output\sort8t.txt");
            var expected = new FileInfo(@"TestData\Sort\Expected\sort8.txt");

            var sortTasklet = new SortTasklet
            {
                Separator = "\r\n",
                SortCard = "(7,10,CH,A)",
                Sum = "FIELDS=(23,11,34,11,45,11,56,11),FORMAT=ZD",
                Omit = "(04,03,CH,EQ,C'555')",
                Input = new List<IResource> { new FileSystemResource(new FileInfo(@"TestData\Sort\Input\sort8.txt")) },
                Output = new FileSystemResource(output)
            };

            sortTasklet.Execute(new StepContribution(new StepExecution("sort", new JobExecution(1))), null);

            Assert.IsTrue(TestHelper.TestHelper.ContentEquals(expected.OpenRead(), output.OpenRead()));
        }

        [TestMethod]
        public void TestSort9Tasklet()
        {
            var output = new FileInfo(@"TestData\Sort\Output\sort9t.txt");
            var expected = new FileInfo(@"TestData\Sort\Expected\sort9.txt");

            var sortTasklet = new SortTasklet
            {
                RecordLength = 5,
                SortCard = "3,5,PD,A",
                Sum = "1,2,PD",
                Omit = "(3,5,PD,GE,300000)",
                Inrec = "(X'001C',1,5)",
                Encoding = Cp1047,
                Input = new List<IResource> { new FileSystemResource(new FileInfo(@"TestData\Sort\Input\sort9.txt")) },
                Output = new FileSystemResource(output)
            };

            sortTasklet.Execute(new StepContribution(new StepExecution("sort", new JobExecution(1))), null);

            Assert.IsTrue(TestHelper.TestHelper.ContentEquals(expected.OpenRead(), output.OpenRead()));
        }

        [TestMethod]
        public void TestSort10Tasklet()
        {
            var output = new FileInfo(@"TestData\Sort\Output\sort10t.txt");
            var expected = new FileInfo(@"TestData\Sort\Expected\sort10.txt");

            var sortTasklet = new SortTasklet
            {
                SortCard = "1,2,FI,A,3,2,BI,D",
                Sum = "3,2,BI",
                Omit = "1,2,FI,LT,0",
                Outrec = "X,1,4",
                RecordLength = 4,
                Input = new List<IResource> { new FileSystemResource(new FileInfo(@"TestData\Sort\Input\sort10.txt")) },
                Output = new FileSystemResource(output)
            };

            sortTasklet.Execute(new StepContribution(new StepExecution("sort", new JobExecution(1))), null);

            Assert.IsTrue(TestHelper.TestHelper.ContentEquals(expected.OpenRead(), output.OpenRead()));
        }

        [TestMethod]
        public void TestSort11Tasklet()
        {
            var output = new FileInfo(@"TestData\Sort\Output\sort11t.txt");
            var expected = new FileInfo(@"TestData\Sort\Expected\sort11.txt");

            var sortTasklet = new SortTasklet
            {
                SortCard = "5,20,CH,A,33,4,PD,A",
                Sum = "25,8,ZD",
                Encoding = Cp1047,
                Input = new List<IResource> { new FileSystemResource(new FileInfo(@"TestData\Sort\Input\sort11.txt")) },
                Output = new FileSystemResource(output)
            };

            sortTasklet.Execute(new StepContribution(new StepExecution("sort", new JobExecution(1))), null);

            Assert.IsTrue(TestHelper.TestHelper.ContentEquals(expected.OpenRead(), output.OpenRead()));
        }

        [TestMethod]
        public void TestSort12Tasklet()
        {
            var output = new FileInfo(@"TestData\Sort\Output\sort12t.txt");
            var expected = new FileInfo(@"TestData\Sort\Expected\sort12.txt");

            var sortTasklet = new SortTasklet
            {
                Include = "1,10,CH,EQ,C'INCLUDE '",
                Separator = "\r\n",
                Input = new List<IResource> { new FileSystemResource(new FileInfo(@"TestData\Sort\Input\sort12.txt")) },
                Output = new FileSystemResource(output)
            };

            sortTasklet.Execute(new StepContribution(new StepExecution("sort", new JobExecution(1))), null);

            Assert.IsTrue(TestHelper.TestHelper.ContentEquals(expected.OpenRead(), output.OpenRead()));
        }

        [TestMethod]
        public void TestSort13Tasklet()
        {
            var output = new FileInfo(@"TestData\Sort\Output\sort13t.txt");
            var expected = new FileInfo(@"TestData\Sort\Expected\sort13.txt");

            var sortTasklet = new SortTasklet
            {
                Outrec = "(1,8,5X,11,6,PD,M4,LENGTH=15,5X,31,7,PD,EDIT=(SI,III,III,IIT.TTT),SIGNS=(,-),5X,41,5,PD,EDIT=(TT-TT-TTTT))",
                Encoding = Cp1047,
                RecordLength = 45,
                Input = new List<IResource> { new FileSystemResource(new FileInfo(@"TestData\Sort\Input\sort13.txt")) },
                Output = new FileSystemResource(output)
            };

            sortTasklet.Execute(new StepContribution(new StepExecution("sort", new JobExecution(1))), null);

            Assert.IsTrue(TestHelper.TestHelper.ContentEquals(expected.OpenRead(), output.OpenRead()));
        }

        [TestMethod]
        public void TestSort14Tasklet()
        {
            var output = new FileInfo(@"TestData\Sort\Output\sort14t.txt");
            var expected = new FileInfo(@"TestData\Sort\Expected\sort14.txt");

            var sortTasklet = new SortTasklet
            {
                Outrec = "(1,8,ZD,M3,LENGTH=12,9,8,ZD,M5,LENGTH=12,17,8,ZD,M20,LENGTH=12)",
                Encoding = Cp1047,
                RecordLength = 24,
                Input = new List<IResource> { new FileSystemResource(new FileInfo(@"TestData\Sort\Input\sort14.txt")) },
                Output = new FileSystemResource(output)
            };

            sortTasklet.Execute(new StepContribution(new StepExecution("sort", new JobExecution(1))), null);

            Assert.IsTrue(TestHelper.TestHelper.ContentEquals(expected.OpenRead(), output.OpenRead()));
        }

        [TestMethod]
        public void TestSort15Tasklet()
        {
            var output = new FileInfo(@"TestData\Sort\Output\sort15t.txt");
            var expected = new FileInfo(@"TestData\Sort\Expected\sort15.txt");

            var sortTasklet = new SortTasklet
            {
                SortCard = "(250,3,CH,A,628,09,CH,A,1,10,CH,A)",
                Include = "(609,4,CH,GT,C'0100',AND,920,4,PD,NE,+0)",
                Encoding = Cp1047,
                RecordLength = 2500,
                Input = new List<IResource> { new FileSystemResource(new FileInfo(@"TestData\Sort\Input\sort15.txt")) },
                Output = new FileSystemResource(output)
            };

            sortTasklet.Execute(new StepContribution(new StepExecution("sort", new JobExecution(1))), null);

            Assert.IsTrue(TestHelper.TestHelper.ContentEquals(expected.OpenRead(), output.OpenRead()));
        }

        [TestMethod]
        public void TestSort16Tasklet()
        {
            var output = new FileInfo(@"TestData\Sort\Output\sort16t.txt");
            var expected = new FileInfo(@"TestData\Sort\Expected\sort16.txt");

            var sortTasklet = new SortTasklet
            {
                SortCard = "1,5,CH,A",
                Sum = "6,3,ZD",
                Outrec = "1,5,6,3,ZD,EDIT=(TTT)",
                Separator = "\r\n",
                Input = new List<IResource> { new FileSystemResource(new FileInfo(@"TestData\Sort\Input\sort16.txt")) },
                Output = new FileSystemResource(output)
            };

            sortTasklet.Execute(new StepContribution(new StepExecution("sort", new JobExecution(1))), null);

            Assert.IsTrue(TestHelper.TestHelper.ContentEquals(expected.OpenRead(), output.OpenRead()));
        }

        [TestMethod]
        public void TestSort17Tasklet()
        {
            var output = new FileInfo(@"TestData\Sort\Output\sort17t.txt");
            var expected = new FileInfo(@"TestData\Sort\Expected\sort17.txt");

            var sortTasklet = new SortTasklet
            {
                SortCard = "8,2,ZD,A",
                Outrec = "1:1,7,11:8,2",
                Separator = "\r\n",
                Input = new List<IResource> { new FileSystemResource(new FileInfo(@"TestData\Sort\Input\sort17.txt")) },
                Output = new FileSystemResource(output)
            };

            sortTasklet.Execute(new StepContribution(new StepExecution("sort", new JobExecution(1))), null);

            Assert.IsTrue(TestHelper.TestHelper.ContentEquals(expected.OpenRead(), output.OpenRead()));
        }

        [TestMethod]
        [Ignore]
        // TIMING : 10:38:13.2660 Info 14 (SortTasklet.cs:128) Total sort time: 142,82s (CPU = Intel I7 - 3770 @3.40Ghz)
        // for the given 1.17 Go file
        public void BenchTest()
        {
            var output = new FileInfo(@"Q:\temp\bench\output.csv");
            var input = new FileInfo(@"Q:\temp\bench\input.csv");

            var sortTasklet = new SortTasklet
            {
                SortCard = "1,5,CH,A,7,4,CH,A",
                Separator = "\r\n",
                HeaderSize = 1,
                Input = new List<IResource> { new FileSystemResource(input) },
                Output = new FileSystemResource(output)
            };

            sortTasklet.Execute(new StepContribution(new StepExecution("sort", new JobExecution(1))), null);

            output.Refresh();
            Assert.AreEqual(input.Length, output.Length);
        }
    }
}