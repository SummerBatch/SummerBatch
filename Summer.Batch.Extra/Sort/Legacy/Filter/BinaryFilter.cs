using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summer.Batch.Extra.Sort.Legacy.Filter
{
    class BinaryFilter: AbstractLegacyFilter<decimal>
    {
        protected override int DoComparison(decimal leftValue, decimal rightValue)
        {
            return leftValue.CompareTo(rightValue);
        }
    }
}
