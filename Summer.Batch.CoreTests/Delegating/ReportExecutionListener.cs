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
using Summer.Batch.Extra;
using Summer.Batch.Infrastructure.Item;

namespace Summer.Batch.CoreTests.Delegating
{
    public class ReportExecutionListener : AbstractExecutionListener, 
        IItemProcessor<List<CommandItem>,UserTotal>
    {
        public UserTotal Process(List<CommandItem> group)
        {
            UserTotal result = new UserTotal();
            string user = group[0].User;
            int lastCommand = -1;
            int nbCommands = 0;
            int total = 0;
            foreach (CommandItem item in group)
            {
                if (item.CommandId != lastCommand)
                {
                    nbCommands++;
                    lastCommand = item.CommandId;
                }
                total += item.Amount;
            }
            result.User = user;
            result.NbCommands = nbCommands;
            result.Total = total;
            return result;
        }
    }
}