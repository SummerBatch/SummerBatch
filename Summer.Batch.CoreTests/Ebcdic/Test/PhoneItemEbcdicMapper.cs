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
using System.Text.RegularExpressions;
using Summer.Batch.Extra.Ebcdic;

namespace Summer.Batch.CoreTests.Ebcdic.Test
{
    public class PhoneItemEbcdicMapper : AbstractEbcdicReaderMapper<PhoneItem>
    {
        private const int PhoneNumber = 0;
        private const int FaxNumber = 1;

        public override PhoneItem Map(IList<object> values, int itemCount)
        {
            PhoneItem record = new PhoneItem
            {
                PhoneNumber = (string)values[PhoneNumber],
                FaxNumber = (string)values[FaxNumber]
            };
            return record;
        }

        protected bool? ToBoolean(string value)
        {
            bool? result = null;
            if (!string.IsNullOrWhiteSpace(value))
            {
                Regex regex = new Regex("[Y1]");
                result = regex.IsMatch(value);
            }
            return result;
        }
    }
}