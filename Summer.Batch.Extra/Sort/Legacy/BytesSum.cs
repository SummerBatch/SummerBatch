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
using Summer.Batch.Extra.Sort.Legacy.Accessor;
using Summer.Batch.Extra.Sort.Sum;

namespace Summer.Batch.Extra.Sort.Legacy
{
    /// <summary>
    /// Implementation of <see cref="ISum{T}"/> that sums number encoded in byte arrays.
    /// Numbers are read using <see cref="IAccessor{T}"/>s.
    /// </summary>
    public class BytesSum : ISum<byte[]>
    {
        /// <summary>
        /// The accessors that get and set the numbers to sum.
        /// </summary>
        public IList<IAccessor<decimal>> Accessors { get; set; }

        /// <summary>
        /// Sums a list of records.
        /// </summary>
        /// <param name="records">the records to sum</param>
        /// <returns>the record resulting of the sum</returns>
        public byte[] Sum(IList<byte[]> records)
        {
            // Fill the values with decimal.Zero
            var values = new decimal[Accessors.Count];
            for (var i = 0; i < Accessors.Count; i++)
            {
                values[i] = decimal.Zero;
            }

            // Sum each value
            foreach (var record in records)
            {
                for (var i = 0; i < Accessors.Count; i++)
                {
                    values[i] += Accessors[i].Get(record);
                }
            }

            // Inject the sums in the first record
            var firstRecord = records[0];
            var result = firstRecord.SubArray(0, firstRecord.Length);
            for (var i = 0; i < Accessors.Count; i++)
            {
                Accessors[i].Set(result, values[i]);
            }

            return result;
        }
    }
}