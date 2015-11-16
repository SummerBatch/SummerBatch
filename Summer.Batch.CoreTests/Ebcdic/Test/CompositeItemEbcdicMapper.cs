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
    public class CompositeItemEbcdicMapper : AbstractEbcdicReaderMapper<object>
    {
        private const int Name = 1;
        private const int Items = 3;

        public override string DistinguishedPattern
        {
            get { return "2"; }
        }

        private List<IEbcdicReaderMapper<object>> _subMappers;
        protected override IList<IEbcdicReaderMapper<object>> SubMappers
        {
            get { return _subMappers ?? (_subMappers = new List<IEbcdicReaderMapper<object>> { _itemsMapper }); }
        }
        private readonly SingleItemEbcdicMapper _itemsMapper = new SingleItemEbcdicMapper();


        public override object Map(IList<object> values, int itemCount)
        {
            CompositeItem record = new CompositeItem
            {
                Name = (string) values[Name],
                Items = SubMap((List<object>) values[Items], itemCount, _itemsMapper)
            };
            return record;
        }
    }
}