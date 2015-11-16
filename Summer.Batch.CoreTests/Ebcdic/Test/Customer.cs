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
using System.Diagnostics;
using System.Text;

namespace Summer.Batch.CoreTests.Ebcdic.Test
{
    [DebuggerDisplay("Id={Id},Name={Name},Emails={Emails},Addresses={Addresses}")]
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<string> Emails { get; set; }
        public IList<CustomerAddress> Addresses { get; set; }

        public int NbAddresses
        {
            get
            {
                return Addresses.Count;
            }
        }

        public int NbEmails
        {
            get
            {

                return Emails.Count;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("Customer (");
            sb.Append("Id=").Append(Id).Append(',');
            sb.Append("Name=").Append(Name).Append(',');
            sb.Append("Adresses={");
            sb.Append(string.Join(",", Addresses));
            sb.Append("},");
            sb.Append("Emails={");
            sb.Append(string.Join(",", Emails));
            sb.Append("})");
            return sb.ToString();
        }
    }
}