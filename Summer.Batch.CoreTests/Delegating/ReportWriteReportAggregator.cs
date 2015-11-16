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
using Summer.Batch.Extra.Template;

namespace Summer.Batch.CoreTests.Delegating
{
    public class ReportWriteReportAggregator : 
        AbstractTemplateLineAggregator<UserTotal>
    {

        public string HeaderId { private get; set; }
        public string FooterId { private get; set; }

        protected override IEnumerable<object> GetParameters(UserTotal item)
        {
            return UserTotalWriter(item);
        }

        public override void WriteHeader(TextWriter writer)
        {
            writer.Write(GetFormattedLine(HeaderId, new List<object>()));
        }

        public override void WriteFooter(TextWriter writer)
        {
            writer.Write(GetFormattedLine(FooterId, new List<object>()));        
        }        

        private List<object> UserTotalWriter(UserTotal userTotal)
        {
            return new List<object> {userTotal.User, userTotal.NbCommands, userTotal.Total};
        } 

    }
}