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
using Summer.Batch.Extra.Ebcdic;

namespace Summer.Batch.CoreTests.Ebcdic.Test
{
    public class CustomerAddressEbcdicMapper : AbstractEbcdicReaderMapper<CustomerAddress>
    {
        private const int Street = 0;
        private const int City = 1;
        private const int PhoneItem = 2;

        private List<IEbcdicReaderMapper<object>> _subMappers;

        private readonly PhoneItemEbcdicMapper _phoneItemMapper = new PhoneItemEbcdicMapper();

        protected override IList<IEbcdicReaderMapper<object>> SubMappers
        {
            get { return _subMappers ?? (_subMappers = new List<IEbcdicReaderMapper<object>> { _phoneItemMapper }); }
        }

        public override CustomerAddress Map(IList<object> values, int itemCount)
        {
            CustomerAddress record = new CustomerAddress
            {
                Street = (string)values[Street],
                City = (string)values[City],
                PhoneItem = _phoneItemMapper.Map((List<object>)values[PhoneItem], itemCount)
            };

            return record;
        }


        public override string DistinguishedPattern { get { return null; } }
    }
}