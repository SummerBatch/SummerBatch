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
using System.Linq;
using Summer.Batch.Extra.Ebcdic;

namespace Summer.Batch.CoreTests.Ebcdic.Test
{
    public class CustomerEbcdicMapper : AbstractEbcdicReaderMapper<Customer>
    {
        private const int Name = 0;
        private const int Addresses = 2;
        private const int Emails = 4;
        

        public override string DistinguishedPattern { get { return null; } }

        private readonly CustomerAddressEbcdicMapper _addressesMapper = new CustomerAddressEbcdicMapper();

        private List<IEbcdicReaderMapper<object>> _subMappers;

        protected override IList<IEbcdicReaderMapper<object>> SubMappers
        {
            get { return _subMappers ?? (_subMappers = new List<IEbcdicReaderMapper<object>> {_addressesMapper}); }
        }

        public override Customer Map(IList<object> values, int itemCount)
        {
            Customer record = new Customer
            {
                Id = itemCount,
                Name = (string) values[Name],
                Emails = ((List<object>) values[Emails]).Cast<string>().ToList(),
                Addresses = (List<CustomerAddress>) SubMap((List<object>) values[Addresses], itemCount,_addressesMapper)
            };
            return record;
        }
    }
}