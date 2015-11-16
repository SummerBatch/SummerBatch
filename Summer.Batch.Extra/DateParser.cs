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
using System;
using System.Globalization;
using NLog;

namespace Summer.Batch.Extra
{
    /// <summary>
    /// The default date parser
    /// </summary>
    public class DateParser : IDateParser
    {

        /** The logger. **/
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        /** Constant for empty date. **/
        private const string EmptyDate = "        ";
        /** Constant for legacy 0 date. **/
        private const int DateZeroLegacy = 0;

        /** Constant for maximum legacy date. **/
        private const int MaxDateLegacy = 99999999;
        /** December 31st 9999. **/
        private static readonly DateTime? MaxDateModernized = new DateTime(9999,12,31);
        /** Used to extract the year from the big decimal date. **/
        private const int BdYearFactor = 10000;
        /** Used to extract the month from the big decimal date. **/
        private const int BdMonthFactor = 100;

        private const string DateFormat = "yyyyMMdd";
        private const string DateFormatOut = "{0:yyyyMMdd}";

        /// <summary>
        /// @see IDateParser#Decode
        /// </summary>
        /// <param name="bdDate"></param>
        /// <returns></returns>
        public DateTime? Decode(decimal bdDate)
        {
            int bdDateInt = (int) bdDate;
            return Decode(bdDateInt.ToString());
        }

        /// <summary>
        /// @see IDateParser#Decode
        /// </summary>
        /// <param name="sDate"></param>
        /// <returns></returns>
        public DateTime? Decode(string sDate)
        {
            try
            {
                return DateTime.ParseExact(sDate.Trim(), DateFormat, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                Logger.Warn("[WARNING] DateParser : parsing of the date >{0}< failed", sDate);
                return null;
            }
        }

        /// <summary>
        /// @see IDateParser#Encode
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public string EncodeString(DateTime? date)
        {
            string ret;
            if (date != null)
            {
                ret = date.Equals(MaxDateModernized) ? MaxDateLegacy.ToString() : string.Format(DateFormatOut, date);
            }
            else
            {
                ret = EmptyDate;
            }
            return ret;
        }

        /// <summary>
        /// @see IDateParser#Encode
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public decimal EncodeDecimal(DateTime? date)
        {
            decimal ret;
            if (date != null)
            {
                if (date.Equals(MaxDateModernized))
                {
                    ret = new decimal(MaxDateLegacy);
                }
                else
                {                  
                    int year = date.Value.Year;
                    int month = date.Value.Month;
                    int day = date.Value.Day;                    
                    ret = new decimal(year * BdYearFactor + month * BdMonthFactor + day);
                }
            }
            else
            {
                ret = new decimal(DateZeroLegacy);
            }
            return ret;
        }
    }
}