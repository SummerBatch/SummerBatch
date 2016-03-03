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
        public void TestMultipleOutputFilesSortFunction()
        {

            var output = new FileInfo(@"TestData\Sort\Input\customSort\sort_test_out1.txt");


            var sortTasklet = new ExtendedSortTasklet
            {
                Separator = "\n",
                SortCard = "(121,14,CH,A)",
                OutputFiles = new List<OutputFile>(),
                Encoding = Cp1252,
                SortEncoding = Cp1147,
                Input = new List<IResource> { new FileSystemResource(new FileInfo(@"TestData\Sort\Input\customSort\sort_test_in.txt")) },
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
            sortTasklet.OutputFiles.Add(outputFile1);
            sortTasklet.OutputFiles.Add(outputFile2);
            sortTasklet.OutputFiles.Add(outputFile3);
            sortTasklet.OutputFiles.Add(outputFile4);
            sortTasklet.Execute(new StepContribution(new StepExecution("sort", new JobExecution(1))), null);
        }

        [TestMethod]
        public void TestHexadecimalFilterCriteriaSort()
        {

            var output = new FileInfo(@"TestData\Sort\Input\customSort\sort_test_out2.txt");


            var sortTasklet = new SortTasklet
            {
                Separator = "\n",
                SortCard = "(121,14,CH,A)",
                Include = "75,2,BI,NE,X'5244'",
                Encoding = Cp1252,
                SortEncoding = Cp1147,
                Input = new List<IResource> { new FileSystemResource(new FileInfo(@"TestData\Sort\Input\customSort\sort_test_in.txt")) },
                Output = new FileSystemResource(output)
            };

            sortTasklet.Execute(new StepContribution(new StepExecution("sort", new JobExecution(1))), null);

        }


        [TestMethod]
        public void TestHeaderAndTrailerForPageAndReport()
        {

            var output = new FileInfo(@"TestData\Sort\Input\customSort\sort_test_out2.txt");


            var sortTasklet = new ExtendedSortTasklet
            {
                Separator = "\r\n",
                SortCard = "(1,15,CH,A)",
                OutputFiles = new List<OutputFile>(),
                Encoding = Cp1252,
                SortEncoding = Cp1147,
                Input = new List<IResource> { new FileSystemResource(new FileInfo(@"TestData\Sort\Input\customSort\sort_test_in.txt")) },
                Output = new FileSystemResource(output)
            };
            var outputFile1 = new OutputFile
            {
                Outrec = "1,200,77,3,ZD,EDIT=($TT.00)",
                //section = "0,15,SKIP=2L,HEADER3=(1:'DEPARTMENT', 23:'SALES MGR'),TRAILER3=(1:'TOTAL RECORDS FOR DEPARTMENT = ',&COUNT)",
                Header1 = "47:'ALBERTA HEALTH OVERLAY ROWS REPORT',/,&DATE",
                Header2 = "2:'REPORT ID: NCI049R1 ',&PAGE,/,46:'WORKERS'' COMPENSATION BOARD - ALBERTA'",
                Trailer2 = "100:'TOTAL RECORDS FOR DEPARTMENT = ',&COUNT",
                Lines = 6
            };
            sortTasklet.OutputFiles.Add(outputFile1);
            sortTasklet.Execute(new StepContribution(new StepExecution("sort", new JobExecution(1))), null);

        }

        [TestMethod]
        public void TestEBCDICReport()
        {

            var output = new FileInfo(@"TestData\Sort\Input\test_raw_output_EBCDIC.dat");
            var sortTasklet = new ExtendedSortTasklet
            {

                RecordLength = 89,
                SortCard = "(1,88,BI,A)",
                OutputFiles = new List<OutputFile>(),
                Encoding = Cp1047,
                SortEncoding = Cp1047,
                Input = new List<IResource> { new FileSystemResource(new FileInfo(@"TestData\Sort\Input\test_raw_input_EBCDIC.dat")) },
                Output = new FileSystemResource(output)
            };
            var outputFile1 = new OutputFile
            {
                Outrec = "12:1,7,33:8,10,52:18,10,72:28,1,60X",
                //section = "88,1,TRAILER3=(//,15:'TOTAL NUMBER OF RECORDS OVERLAID: ',&COUNT,//,51:'* * * * END OF REPORT * * * *')",
                Header2 = "2:'REPORT ID: NCI046R1',46:'WORKERS'' COMPENSATION BOARD - ALBERTA',118:'PAGE: ',&PAGE,/," +
                "47:'ALBERTA HEALTH OVERLAY ROWS REPORT',103:'RUN DATE (MM/DD/YY): ',&DATE,/," +
                "52:'FOR SERVICE CODE OVERLAYS',///,12:'SERVICE CODE',31:'EFFECTIVE DATE',53:'END DATE',65:'DIAG CD REQD FLG',/,12:'------------',31:'--------------',52:'----------',65:'---------------',/",
                Trailer1 = "//,15:'TOTAL NUMBER OF RECORDS OVERLAID: ',&COUNT,//,51:'* * * * END OF REPORT * * * *'",
                Lines = 60,
                OutputFileRecordLength = 132
            };
            sortTasklet.OutputFiles.Add(outputFile1);
            sortTasklet.Execute(new StepContribution(new StepExecution("sort", new JobExecution(1))), null);

        }
    }
}
