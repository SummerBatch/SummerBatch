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
using Summer.Batch.Infrastructure.Item.File.Mapping;
using Summer.Batch.Infrastructure.Item.File.Transform;

namespace Summer.Batch.CoreTests.Delegating
{
    public class CommandItemMapper : IFieldSetMapper<CommandItem>
    {
        private const int User = 0;
        private const int CommandId = 1;
        private const int Amount = 2;

        public CommandItem MapFieldSet(IFieldSet fieldSet)
        {
            CommandItem result = new CommandItem
            {
                User = fieldSet.ReadRawString(User),
                CommandId = fieldSet.ReadInt(CommandId),
                Amount = fieldSet.ReadInt(Amount)
            };
            return result;
        }
    }
}