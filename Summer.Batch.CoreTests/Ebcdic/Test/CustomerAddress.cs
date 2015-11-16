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
using System.Diagnostics;
using System.Text;

namespace Summer.Batch.CoreTests.Ebcdic.Test
{
    [DebuggerDisplay("Street={Street},City={City},PhoneItem={PhoneItem}")]
    public class CustomerAddress
    {
        public string Street { get; set; }
        public string City { get; set; }
        public PhoneItem PhoneItem { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("CustomerAddress(");
            sb.Append("Street=").Append(Street).Append(',');
            sb.Append("City=").Append(Street).Append(',');
            sb.Append("PhoneItem=").Append(PhoneItem);
            sb.Append(")");
            return sb.ToString();
        }
    }
}