using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summer.Batch.Extra.Sort.Legacy.Accessor
{
    class AsciiAcessor : AbstractAccessor<decimal>
    {

        /// <summary>
        /// The precision of the decimal.
        /// </summary>
        public int Precision { get; set; }

        /// </summary>
        /// <param name="record">the record to get the value from</param>
        /// <returns>the read value</returns>
        public override decimal Get(byte[] record)
        {
            string value = Encoding.GetString(record, Start, Length);
            return Decimal.Parse(value);
        }

        private string format = null;

        /// <summary>
        /// Sets a value on a record.
        /// </summary>
        /// <param name="record">the record to set the value on</param>
        /// <param name="value">the value to set</param>
        public override void Set(byte[] record, decimal value)
        {
            if (format == null)
            {
                format = "";
                if (Precision == 0)
                {
                    for (int i = 0; i < Length - 1; i++)
                    {
                        format += "0";
                    }
                }
                else 
                {
                    for (int i = 0; i < Length - Precision - 2; i++)
                    {
                        format += "0";
                    }
                    format += ".";
                    for (int i = 0; i < Precision; i++)
                    {
                        format += "0";
                    }
                }
            }
            string sValue = value.ToString("+"+format+";-" + format);
            SetBytes(record, Encoding.GetBytes(sValue), Encoding.GetBytes(" ")[0]);
        }
    }
}
