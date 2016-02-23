using Microsoft.VisualStudio.TestTools.UnitTesting;
using Summer.Batch.Common.IO;
using Summer.Batch.Core;
using Summer.Batch.Extra.Sort;
using Summer.Batch.Extra.Sort.Comparer;
using Summer.Batch.Extra.Sort.Legacy;
using Summer.Batch.Extra.Sort.Legacy.Accessor;
using Summer.Batch.Extra.Sort.Legacy.Comparer;
using Summer.Batch.Extra.Sort.Legacy.Filter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summer.Batch.CoreTests.Sort
{
    [TestClass]
    public class SplitSorterTest
    {
        private static readonly byte[] CrLf = Encoding.Default.GetBytes("\r\n");
        private static readonly byte[] Lf = Encoding.Default.GetBytes("\n");

        private static readonly Encoding Cp1147 = Encoding.GetEncoding("IBM01147");
        private static readonly Encoding Cp1047 = Encoding.GetEncoding("IBM01047");
        private static readonly Encoding Cp1252 = Encoding.GetEncoding("Windows-1252");

        [TestMethod]
        public void TestSort1()
        {
            SplitSorter<byte[]> sorter = new SplitSorter<byte[]>
            {
                RecordAccessorFactory = new SeparatorAccessorFactory { Separator = CrLf },
                HeaderSize = 1,
                Comparer = new ComparerChain<byte[]>
                {
                    Comparers = new List<IComparer<byte[]>>
                    {
                        new Summer.Batch.Extra.Sort.Legacy.Comparer.StringComparer { Start = 0, Length = 25 },
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
            var output = new FileInfo(@"TestData\Sort\splitSort");
            var expected = new FileInfo(@"TestData\Sort\splitSort\sort101.txt");

            sorter.Sort(input, output);

            //Assert.IsTrue(TestHelper.TestHelper.ContentEquals(expected.OpenRead(), output.OpenRead()));
        }

       [TestMethod]
        public void TestSort2()
        {

            var output = new FileInfo(@"TestData\Sort\Input\sort_17_o.txt");
            

            var sortTasklet = new CustomSortTasklet
            {
                Separator = "\n",
                SortCard = "(121,14,CH,A)",
                outputFile = new List<OutputFile>(),
                Encoding = Cp1252,
                SortEncoding = Cp1147,
                Input = new List<IResource> { new FileSystemResource(new FileInfo(@"TestData\Sort\Input\sort17.txt")) },
                Output = new FileSystemResource(output)
            };
            var outputFile1 = new OutputFile
            {
                Include = "75,2,CH,EQ,C'RD'",
                Outrec = "25,200"
            };
            var outputFile2 = new OutputFile
            {
                Include = "75,2,CH,EQ,C'XD'"
            };
            var outputFile3 = new OutputFile
            {
                Include = "75,2,CH,EQ,C'CD'"
            };
            var outputFile4 = new OutputFile
            {
                Include = "75,2,CH,EQ,C'YD'"
            };
            sortTasklet.outputFile.Add(outputFile1);
            sortTasklet.outputFile.Add(outputFile2);
            sortTasklet.outputFile.Add(outputFile3);
            sortTasklet.outputFile.Add(outputFile4);
            sortTasklet.Execute(new StepContribution(new StepExecution("sort", new JobExecution(1))), null);

           // Assert.IsTrue(TestHelper.TestHelper.ContentEquals(expected.OpenRead(), output.OpenRead()));
        }

        [TestMethod]
        public void TestSort3()
        {

            var output = new FileInfo(@"TestData\Sort\Input\sort17_o_1.txt");


            var sortTasklet = new SortTasklet
            {
                Separator = "\n",
                SortCard = "(121,14,CH,A)",
                Include = "75,2,BI,NE,X'5244'",
                Encoding = Cp1252,
                SortEncoding = Cp1147,
                Input = new List<IResource> { new FileSystemResource(new FileInfo(@"TestData\Sort\Input\sort17.txt")) },
                Output = new FileSystemResource(output)
            };
           
            sortTasklet.Execute(new StepContribution(new StepExecution("sort", new JobExecution(1))), null);

            //Assert.IsTrue(TestHelper.TestHelper.ContentEquals(expected.OpenRead(), output.OpenRead()));
        }


        [TestMethod]
        public void TestSort4()
        {

            var output = new FileInfo(@"TestData\Sort\Input\sort17_o_1.txt");


            var sortTasklet = new CustomSortTasklet
            {
                Separator = "\r\n",
                SortCard = "(1,15,CH,A)",
                outputFile = new List<OutputFile>(),
                Encoding = Cp1252,
                SortEncoding = Cp1147,
                Input = new List<IResource> { new FileSystemResource(new FileInfo(@"TestData\Sort\Input\sort17.txt")) },
                Output = new FileSystemResource(output)
            };
            var outputFile1 = new OutputFile
            {
                Outrec = "1,200,77,3,ZD,EDIT=($TT.00)",
                //section = "0,15,SKIP=2L,HEADER3=(1:'DEPARTMENT', 23:'SALES MGR'),TRAILER3=(1:'TOTAL RECORDS FOR DEPARTMENT = ',&COUNT)",
                header1 = "47:'ALBERTA HEALTH OVERLAY ROWS REPORT',/,&DATE",
                header2 = "2:'REPORT ID: NCI049R1 ',&PAGE,/,46:'WORKERS'' COMPENSATION BOARD - ALBERTA'",
                trailer2 = "100:'TOTAL RECORDS FOR DEPARTMENT = ',&COUNT",
                lines = 6
            };
            sortTasklet.outputFile.Add(outputFile1);
            sortTasklet.Execute(new StepContribution(new StepExecution("sort", new JobExecution(1))), null);

            //Assert.IsTrue(TestHelper.TestHelper.ContentEquals(expected.OpenRead(), output.OpenRead()));
        }

        [TestMethod]
        public void TestSort5()
        {

            var output = new FileInfo(@"TestData\Sort\Input\jpnci046.step080.sortout.dat.txt");
            var sortTasklet = new CustomSortTasklet
            {
                
                RecordLength = 89,
                SortCard = "(1,27,A) FORMAT='BI'",
                outputFile = new List<OutputFile>(),
                Encoding = Cp1047,
                SortEncoding = Cp1047,
                Input = new List<IResource> { new FileSystemResource(new FileInfo("P:/PRJ - Projets/Accenture/WCB/000 - Project/02 - inputs/SummerBatchPrototypeFiles/jpnci046.step080.sortin.dat")) },
                Output = new FileSystemResource(output)
            };
            var outputFile1 = new OutputFile
            {
                Outrec = "12:1,7,33:8,10,52:18,10,72:28,1,60X",
                section = "89,0,TRAILER3=(//,15:'TOTAL NUMBER OF RECORDS OVERLAID: ',&COUNT,//,51:'* * * * END OF REPORT * * * *')",
                header2 = "2:'REPORT ID: NCI046R1',46:'WORKERS'' COMPENSATION BOARD - ALBERTA',118:'PAGE: ',&PAGE,/,   47:'ALBERTA HEALTH OVERLAY ROWS REPORT',103:'RUN DATE (MM/DD/YY): ',&DATE,/,52:'FOR SERVICE CODE OVERLAYS',///,12:'SERVICE CODE',31:'EFFECTIVE DATE',53:'END DATE',65:'DIAG CD REQD FLG',/,12:'------------',31:'--------------',52:'----------',65:'---------------',/",
                //trailer2 = "100:'TOTAL RECORDS FOR DEPARTMENT = ',&COUNT",
                lines = 60
            };
            sortTasklet.outputFile.Add(outputFile1);
            sortTasklet.Execute(new StepContribution(new StepExecution("sort", new JobExecution(1))), null);

            //Assert.IsTrue(TestHelper.TestHelper.ContentEquals(expected.OpenRead(), output.OpenRead()));
        }
    }
}
