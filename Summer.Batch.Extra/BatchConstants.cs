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
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Summer.Batch.Extra
{
    /// <summary>
    /// Constants used in extra batch support classes.
    /// </summary>
    public static class BatchConstants
    {
        /// <summary>
        /// Step Context Manager Singleton Name
        /// </summary>
        public const string StepContextManagerName = "stepContextManager";

        /// <summary>
        /// Job Context Manager Singleton Name
        /// </summary>
        public const string JobContextManagerName = "jobContextManager";

        /// <summary>
        /// Last record key (in context)
        /// </summary>
        public const string LastRecordKey = "batch.lastRecord";

        /// <summary>
        /// CSV Report format. 
        /// </summary>
        public const string CsvReportFormat = "CSV";

        /// <summary>
        /// HTML Report format.  
        /// </summary>
        public const string HtmlReportFormat = "HTML";
        
        /// <summary>
        /// PDF Report format. 
        /// </summary>
        public const string PdfReportFormat = "PDF";
        
        /// <summary>
        /// TXT Report format.  
        /// </summary>
        public const string TxtReportFormat = "TXT";
        
        /// <summary>
        /// XLS Report format.  
        /// </summary>
        public const string XlsReportFormat = "XLS";
        
        /// <summary>
        /// Character width for txt reports. 
        /// </summary> 
        public const float TxtCharacterWidth = 5f;

        /// <summary>
        ///Character height for txt reports. 
        /// </summary>
        public const float TxtCharacterHeight = 15f;
    }
}