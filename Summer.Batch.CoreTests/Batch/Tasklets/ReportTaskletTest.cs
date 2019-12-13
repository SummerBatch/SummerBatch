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
using System.Configuration;
using System.IO;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Summer.Batch.Common.IO;
using Summer.Batch.Core.Launch;
using Summer.Batch.Core.Step.Tasklet;
using Summer.Batch.Core.Unity;
using Summer.Batch.Core.Unity.Xml;
using Summer.Batch.Data;
using Summer.Batch.Extra.Report;

namespace Summer.Batch.CoreTests.Batch.Tasklets
{
    [TestClass]
    public class ReportTaskletTest
    {
        protected const string TestDataDirectoryIn = @".\TestData\Report\";
        protected const string TestDataDirectoryOut = @"C:\temp\out\";

        private static readonly string ReportPath = Path.Combine(TestDataDirectoryIn, "EmployeesReport.rdlc");
        private static readonly string TestPathOut = Path.Combine(TestDataDirectoryOut, "EmployeesReport.pdf");

        [TestMethod()]
        public void RunJobWithTasklet()
        {
            //Delete any prior existing output file
            FileInfo priorOutputFile = new FileInfo(TestPathOut);
            if (priorOutputFile.Exists) { priorOutputFile.Delete();}

            XmlJob job = XmlJobParser.LoadJob("Job1.xml");
            IJobOperator jobOperator = BatchRuntime.GetJobOperator(new MyUnityLoaderJob1(), job);
            Assert.IsNotNull(jobOperator);
            Assert.AreEqual(1, jobOperator.StartNextInstance(job.Id));

            // Post controls
            FileInfo outputFile = new FileInfo(TestPathOut);
            Assert.IsTrue(outputFile.Exists, "Job output file does not exist, job was not successful");
            Assert.IsTrue(outputFile.Length > 0, "Job output file is empty, job was not successful");
        }

        /// <summary>
        /// Extends UnityLoader and redefines LoadArtifacts() to supply the batch artifacts.
        /// </summary>
        private class MyUnityLoaderJob1 : UnityLoader
        {
            public override void LoadArtifacts(IUnityContainer unityContainer)
            {
                
                string rvConnection =
                    @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\BA_REPORTS.mdf";
                string dataProvider = "System.Data.SqlClient";
                string sqlCommand =
                    "SELECT e.empId, e.empName, c.name, e.empDob, e.empSalary, e.emailId, a.buildingNo, a.streetName, a.city, " +
                    "a.state FROM BA_REPORTS_TABLE_2 e, BA_REPORTS_TABLE_3 a, BA_REPORTS_TABLE_1 c where e.empId=a.empId" +
                    " and e.companyId=c.companyid order by e.empId";
                
                ConnectionStringSettings connectionStringSettings =
                    new ConnectionStringSettings("conn",rvConnection,dataProvider);

                //IList<ReportParameter> rparams = new List<ReportParameter>();
                //rparams.Add(new ReportParameter("Limit","2000"));
                IDictionary<string,string> rparams = new Dictionary<string, string>();
                rparams.Add("Limit","2000");

                //Register Tasklet
                unityContainer.RegisterType<ITasklet, ReportStep>("tasklet1",
                    new InjectionProperty("ReportFile",new FileSystemResource(ReportPath)),
                    new InjectionProperty("ReportFormat","PDF"),
                    new InjectionProperty("OutFile",new FileSystemResource(TestPathOut)),
                    new InjectionProperty("DbOperator", new DbOperator()
                    {
                        ConnectionProvider = new ConnectionProvider{ConnectionStringSettings = connectionStringSettings}
                    }),
                    new InjectionProperty("Parameters", rparams),
                    new InjectionProperty("Query",sqlCommand),
                    new InjectionProperty("DatasetName", "BA_REPORTSDataSet")
                    );
            }
        }
    }
}
