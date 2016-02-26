using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Summer.Batch.Extra.Sort.Legacy.Accessor
{
    class BigBinaryAccessor : AbstractAccessor<BigInteger>
    {
        /// <summary>
        /// Whether the number is signed.
        /// </summary>
        public bool Signed { get; set; }

        /// <summary>
        /// Gets a value from a record.
        /// </summary>
        /// <param name="record">the record to get the value from</param>
        /// <returns>the read value</returns>
        public override BigInteger Get(byte[] record)
        {
            var bytes = record.SubArray(Start, Length);
            Array.Reverse(bytes);
            if (!Signed)
            {
                // Add an empty byte at the end so that it is interpreted as a positive integer
                Array.Resize(ref bytes, bytes.Length + 1);
            }
            return new BigInteger(bytes);
        }

        /// <summary>
        /// Sets a value on a record.
        /// </summary>
        /// <param name="record">the record to set the value on</param>
        /// <param name="value">the value to set</param>
        public override void Set(byte[] record, BigInteger value)
        {
            var bytes = value.ToByteArray();
            Array.Reverse(bytes);
            SetBytes(record, bytes, 0);
        }
    }
}
