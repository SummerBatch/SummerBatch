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
using System.Data;
using System.IO;
using System.Linq;
using Microsoft.Reporting.WinForms;
using NLog;
using Summer.Batch.Common.Factory;
using Summer.Batch.Common.IO;
using Summer.Batch.Common.Util;
using Summer.Batch.Core;
using Summer.Batch.Core.Scope.Context;
using Summer.Batch.Core.Step.Tasklet;
using Summer.Batch.Data;
using Summer.Batch.Data.Parameter;
using Summer.Batch.Infrastructure.Repeat;

namespace Summer.Batch.Extra.Report
{
    /// <summary>
    /// Tasklet to perform report generation.
    /// Based on Microsoft Data Report.
    /// </summary>
    public class ReportStep : ITasklet,IInitializationPostOperations
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Report file resource property.
        /// </summary>
        public IResource ReportFile { private get; set; }
        
        /// <summary>
        /// Out file (produced report) resource property.
        /// </summary>
        public IResource OutFile { private get; set; }

        /// <summary>
        /// The sql query used to populate the report.
        /// </summary>
        public String Query { private get; set; }

        /// <summary>
        /// Name of the dataset consumed by the report.
        /// </summary>
        public String DatasetName { private get; set; }

        /// <summary>
        /// Map of report parameters.
        /// </summary>
        public IDictionary<string, string> Parameters { private get; set; } 

        /// <summary>
        /// Report format property.
        /// </summary>
        public string ReportFormat { private get; set; }

        /// <summary>
        /// DbOperator used.
        /// </summary>
        public DbOperator DbOperator { private get; set; }

        /// <summary>
        /// Query parameter source property.
        /// </summary>
        public IQueryParameterSource QueryParameterSource { private get; set; }

        /// <summary>
        /// Generates the report
        /// @see ITasklet#Execute
        /// </summary>
        /// <param name="contribution"></param>
        /// <param name="chunkContext"></param>
        /// <returns></returns>
        public RepeatStatus Execute(StepContribution contribution, ChunkContext chunkContext)
        {
            LocalReport report = new LocalReport
            {
                ReportPath = ReportFile.GetFileInfo().FullName
            };

            if (Parameters != null && Parameters.Any())
            {
                if (Logger.IsTraceEnabled)
                {
                    Logger.Trace("{0} parameter(s) were given for the report ", Parameters.Count);
                }
                report.SetParameters(Parameters.Select(p=>new ReportParameter(p.Key,p.Value)));
            }
            else
            {
                if (Logger.IsTraceEnabled)
                {
                    Logger.Trace("No parameter was given for the report ");
                }
            }

            //DataSet
            DataSet ds = DbOperator.Select(Query,QueryParameterSource);

            //ReportDataSource
            ReportDataSource rds = new ReportDataSource
            {
                Name = DatasetName,
                Value = ds.Tables[0]
            };

            report.DataSources.Add(rds);

            if (Logger.IsTraceEnabled)
            {
                Logger.Trace("Report init : DONE => Preparing to render");
            }
            
            byte[] output = report.Render(ReportFormat);
            
            if (Logger.IsTraceEnabled)
            {
                Logger.Trace("Report init : rendering DONE => Preparing to serialize");
            }
            //Create target directory if required
            OutFile.GetFileInfo().Directory.Create();

            //dump to target file
            using (FileStream fs = new FileStream(OutFile.GetFileInfo().FullName, FileMode.Create))
            {
                fs.Write(output,0,output.Length);
            }
            if (Logger.IsTraceEnabled)
            {
                Logger.Info("Report init : serialization DONE - end of ReportTasklet execute.");
            }
            return RepeatStatus.Finished;
        }

        /// <summary>
        /// @see IInitializationPostOperations#AfterPropertiesSet
        /// </summary>
        public void AfterPropertiesSet()
        {
            Assert.NotNull(ReportFile,"ReportFile must be set");
            Assert.NotNull(OutFile, "OutFile must be set");
            Assert.NotNull(ReportFormat, "ReportFormat must be set");
            Assert.NotNull(DatasetName, "DatasetName must be set");
            Assert.NotNull(Query,"Query must be set");
            Assert.NotNull(DbOperator,"DbOperator must be set");
        }
    }
}