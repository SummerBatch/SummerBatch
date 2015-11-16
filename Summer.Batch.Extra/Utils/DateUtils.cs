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
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Globalization;
using NLog;

namespace Summer.Batch.Extra.Utils
{
    /// <summary>
    /// DateUtils utilitary class.
    /// </summary>
    public static class DateUtils
    {
        /// <summary>
        /// Number of millisenconds in a day.
        /// </summary>
        public const long MILLISECOND_PER_DAY = 24 * 60 * 60 * 1000;

        /// <summary>
        /// Number of millisenconds in a year.
        /// </summary>
        public const long MILLISECOND_PER_YEAR = MILLISECOND_PER_DAY * 365; 
    
        /// <summary>
        /// Short date format ("ddMMyy").
        /// </summary>
        public const string SHORT_DATE_FORMAT = "ddMMyy";
        
        /// <summary>
        /// Standard date format ("dd/MM/yyyy")
        /// </summary>
        public const string STANDARD_DATE_FORMAT = "dd/MM/yyyy";

        /// <summary>
        /// Standard date format, without separators ("ddMMyyyy")
        /// </summary>
        public const string STANDARD_DATE_FORMAT_WITHOUT_SEP = "ddMMyyyy";

        /// <summary>
        /// Short date format, with separators ("ddMMyy")
        /// </summary>
        public const string SHORT_DATE_FORMAT_WITH_SEP = "dd/MM/yy";

        /// <summary>
        /// Standard date format with dashes ("dd-MM-yyyy")
        /// </summary>
        public static readonly string STANDARD_DATE_FORMAT_WITH_TIRET = "dd-MM-yyyy";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Converts a date to a string.
        /// </summary>
        /// <param name="date">The date to convert.</param>
        /// <param name="pattern">The date pattern used to parse the date.</param>
        /// <returns>String representation of given date.</returns>
        public static string ConvertDateToString(DateTime? date, string pattern)
        {
            return date != null && pattern != null
                ? ((DateTime) date).ToString(pattern, CultureInfo.InvariantCulture)
                : null;
        }

        /// <summary>
        /// Builds a date using the given day, month, and year.
        /// </summary>
        /// <param name="day">The day of the month.</param>
        /// <param name="month">The month.</param>
        /// <param name="year">The year.</param>
        /// <returns>A new instance of <see cref="DateTime"/>.</returns>
        public static DateTime? BuildDate(int? day, int? month, int? year)
        {
            return day == null || month == null || year == null
                ? (DateTime?) null
                : new DateTime((int) year, (int) month, (int) day, new GregorianCalendar());
        }

        /// <summary>
        /// Builds a date using the given day, month, and year.
        /// </summary>
        /// <param name="day">The day of the month.</param>
        /// <param name="month">The month.</param>
        /// <param name="year">The year.</param>
        /// <returns>A new instance of <see cref="DateTime"/>.</returns>
        public static DateTime? BuildDate(string day, string month, string year)
        {
            return BuildDate(Convert.ToInt32(day), Convert.ToInt32(month), Convert.ToInt32(year));
        }

        /// <summary>
        /// Updates the year of a date.
        /// </summary>
        /// <param name="date">The date to update.</param>  
        /// <param name="year">The new year.</param>  
        /// <returns>A new date, with updated year value.</returns> 
        public static DateTime? UpdateYear(DateTime? date, int? year) 
        {
            if (date == null || year == null)
            {
                return null;
            }
            var dateTime = date.Value;
            return new DateTime((int) year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute,
                dateTime.Second, dateTime.Millisecond, new GregorianCalendar());
        }	
    
        /// <summary>
        /// Updates the year and month of a date.
        /// </summary>
        /// <param name="date">The date to update.</param>  
        /// <param name="year">The new year.</param>  
        /// <param name="month">The new month.</param>
        /// <returns>A new date, with updated year and month values.</returns>
        public static DateTime? UpdateYearAndMonth(DateTime? date, int? year, int? month)
        {
            if (date == null || year == null || month == null)
            {
                return null;
            }
            var dateTime = (DateTime) date;
            return new DateTime((int) year, (int) month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second,
                dateTime.Millisecond, new GregorianCalendar());
        }

        /// <summary>
        /// Updates the year, month and day of a date.
        /// <param name="date">The date to update.</param>  
        /// <param name="year">The new year.</param>  
        /// <param name="month">The new month.</param>
        /// <param name="day">The new day</param>
        /// <returns>A new date, with updated year, month and days values.</returns>
        /// </summary>
        public static DateTime? UpdateYearMonthAndDay(DateTime? date,int? year,int? month,int? day) 
        {
            if (date == null || year == null || month == null || day == null )
            {
                return null;
            }
            var dateTime = (DateTime) date; 
            return new DateTime((int)year, (int)month, (int)day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, new GregorianCalendar());
        }
    
        /// <summary>
        /// Adds a number of days to the current date.
        /// </summary>
        /// <param name="days">The number of days to add.</param>
        /// <returns>A new date, built by adding <paramref name="days"/> days to the current date.</returns>
        public static DateTime? AddDaysToCurrentDate(int? days)
        {
            return days == null ? (DateTime?) null : DateTime.Now.AddDays(Convert.ToDouble(days));
        }

        /// <summary>
        /// Adds a number of months to the current date.
        /// </summary>
        /// <param name="months">The number of months to add.</param>
        /// <returns>A new date, built by adding <paramparamref name="months"/> months to the current date.</returns>
        public static DateTime? AddMonthsToCurrentDate(int? months)
        {
            return months == null ? (DateTime?) null : DateTime.Now.AddMonths((int)months);
        }

        /// <summary>
        /// Adds a number of years to the current date.
        /// </summary>
        /// <param name="years">The number of years to add.</param>
        /// <returns>A new date, built by adding <paramref name="years"/> years to the current date.</returns>
        public static DateTime? AddYearsToCurrentDate(int? years)
        {
            return years == null ? (DateTime?) null : DateTime.Now.AddYears((int)years);
        }	
    
        /// <summary>
        /// Adds a number of days to a given date.
        /// </summary>
        /// <param name="date">The date to modify.</param>
        /// <param name="days">The number of days to add.</param>
        /// <returns>A new date, built by adding <paramref name="days"/> days to the <paramref name="date"/>.</returns>
        public static DateTime? AddDaysToDate(DateTime? date, int? days)
        {
            return days == null || date == null ? (DateTime?) null : date.Value.AddDays(Convert.ToDouble(days));
        }

        /// <summary>
        /// Adds a number of months to a given date.
        /// </summary>
        /// <param name="date">The date to modify.</param>
        /// <param name="months">The number of months to add.</param>
        /// <returns>A new date, built by adding <paramref name="months"/> months to the <paramref name="date"/>.</returns>
        public static DateTime? AddMonthsToDate(DateTime? date, int? months)
        {
            return months == null || date == null ? (DateTime?) null : date.Value.AddMonths((int)months);
        }

        /// <summary>
        /// Adds a number of years to a given date.
        /// </summary>
        /// <param name="date">The date to modify.</param>
        /// <param name="years">The number of years to add.</param>
        /// <returns>A new date, built by adding <paramref name="years"/> years to the <paramref name="date"/>.</returns>
        public static DateTime? AddYearsToDate(DateTime? date, int? years)
        {
            return years == null || date == null ? (DateTime?) null : date.Value.AddYears((int)years);
        }
    
        /// <summary>
        /// Compares two dates.
        /// </summary>
        /// <param name="date1">The first date to compare.</param>
        /// <param name="date2">The second date to compare.</param>
        /// <returns>1 if the first date is later than the second date. -1 If the first date is anterior to the second date, 0 if the two dates are identical (to the day).</returns>
        public static int? CompareDates(DateTime? date1, DateTime? date2)
        {
            // Compare null dates
            var result = CompareNullDates(date1, date2);
    
            if (result == null)
            {
                var dateTime1 = (DateTime) date1;
                var dateTime2 = (DateTime) date2;
                var calendar = new GregorianCalendar();

                if (calendar.GetYear(dateTime1) < calendar.GetYear(dateTime2))
                {
                    result =-1;
                }
                else if (calendar.GetYear(dateTime1) > calendar.GetYear(dateTime2))
                {
                    result = 1;
                } 
                else
                {		
                    result = CompareDatesIgnoringYear(date1, date2);			
                }
            }
        
            return result;
        }

        /// <summary>
        /// Compares two dates ignoring the year.
        /// </summary>
        /// <param name="date1">The first date to compare.</param>
        /// <param name="date2">The second date to compare.</param>
        /// <returns>-1 If the first date is anterior to the second date, 0 if the two dates are identical (to the day).</returns>
        public static int? CompareDatesIgnoringYear(DateTime? date1, DateTime? date2)
        {
            // Compare null dates
            var result = CompareNullDates(date1, date2);

            if (result ==null)
            {
                var calendar = new GregorianCalendar();
                var dateTime1 = (DateTime) date1;
                var dateTime2 = (DateTime) date2;

                if (calendar.GetMonth(dateTime1) < calendar.GetMonth(dateTime2))
                {
                    result = -1;
                }
                else if (calendar.GetMonth(dateTime1) > calendar.GetMonth(dateTime2))
                {
                    result = 1;
                }
                else 
                {
                    // Compare days
                    result = CompareDays(dateTime1, dateTime2);
                }
            }
            return result;
        }

        private static int? CompareDays(DateTime? date1, DateTime? date2) 
        {
            if (date1 == null || date2 == null)
            {
                return default(int?);
            }
            int? result;
            var dateTime1 = (DateTime)date1;
            var dateTime2 = (DateTime)date2;
            var calendar = new GregorianCalendar();
            if (calendar.GetDayOfMonth(dateTime1) < calendar.GetDayOfMonth(dateTime2))
            {
                result = -1;
            }
            else if (calendar.GetDayOfMonth(dateTime1) > calendar.GetDayOfMonth(dateTime2))
            {
                result = 1;
            }
            else
            {
                result = 0;
            }
            return result;
        }

        private static int? CompareNullDates(DateTime? date1, DateTime? date2)
        {
            int? result = null;
            if(date1 == null && date2 == null)
            {
                result = 0;
            } 
            else if (date1 == null && date2 != null)
            {
                result = -1;
            }
            else if (date1 != null && date2 == null)
            {
                result = 1;
            }
            return result;
        }	

        /// <summary>
        /// Checks if a date is within a time interval.
        /// </summary>
        /// <param name="dateInf">The inferior limit of the time interval.</param>
        /// <param name="dateSup">The superior limit of the time interval.</param>
        /// <param name="dateToTest">The date to check.</param>
        /// <returns>
        /// true if <paramref name="dateToTest"/> is included between <paramref name="dateInf"/> and <paramref name="dateSup"/>.
        /// </returns> 
        public static bool IsInInterval(DateTime? dateInf, DateTime? dateSup, DateTime? dateToTest)
        {
            var result = true;
            // Inside interval : dateInf > dateSup
            if ((CompareDates(dateInf,dateSup)==1) && (CompareDates(dateToTest,dateInf)==1 || CompareDates(dateSup,dateToTest)==1))
            {			
                result = false;			
            }
            // Outside interval
            if (CompareDates(dateInf,dateToTest)==1 || CompareDates(dateToTest,dateSup)==1)
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Computes the number of days between two dates.
        /// </summary>
        /// <param name="date1">The first date.</param>
        /// <param name="date2">The second date.</param>
        /// <returns>
        /// Rounded difference number of days between date2 and date1. Positive if date2>date1, otherwise negative.
        /// </returns>
        public static int? GetDifferenceInDays(DateTime? date1, DateTime? date2) 
        {
            return date1 == null && date2 == null ? default(int?) : (((DateTime)date2)-((DateTime)date1)).Days;
        }
    
        /// <summary>
        /// Computes the number of years between two dates.
        /// <param name="date1">The first date.</param>
        /// <param name="date2">The second date.</param>
        /// <returns>
        /// Rounded difference number of years between date2 and date1. Positive if date2>date1, otherwise negative.
        /// </returns> 
        /// </summary>
        public static int? GetDifferenceInYears(DateTime? date1, DateTime? date2) 
        {
            return date1 == null && date2 == null ? default(int?) : ((DateTime)date2).Year - ((DateTime)date1).Year;
        }

        /// <summary>
        /// Computes the number of working days between two dates.
        /// </summary>
        /// <param name="date1">The first date.</param>
        /// <param name="date2">The second date.</param>
        /// <returns>
        /// Rounded difference number of working days between date2 and date1. Positive if date2>date1, otherwise negative.
        /// </returns> 
        public static int? GetDifferenceInWorkingDays(DateTime? date1, DateTime? date2) 
        {
            var numberOfDay = GetDifferenceInDays(date1, date2);
            if(numberOfDay != null)
            {
                var delta = 0;
                DateTime dateToIterate;
                if(numberOfDay > 0)
                {
                    dateToIterate = (DateTime) date1;
                }
                else
                {
                    dateToIterate = (DateTime) date2;
                }
                if(numberOfDay != 0)
                {
                     for(var i=1;i<=Math.Abs((int)numberOfDay);i++)
                     {
                         var dow = dateToIterate.DayOfWeek;
                         if(dow != DayOfWeek.Sunday && dow != DayOfWeek.Saturday)
                         {
                             delta++;
                         }
                         dateToIterate = dateToIterate.AddDays(1.0);
                     }
                }
                return Math.Sign((int)numberOfDay*delta);        
            }
            return null;
        }		

        /// <summary>
        /// Retrieves the year of a date.
        /// </summary>
        /// <param name="date">A date</param>
        /// <returns>The year of given date.</returns>
        public static int? GetYear(DateTime? date)
        {
            return date == null ? default(int?) : ((DateTime)date).Year;
        }

        /// <summary>
        /// Retrieves the year of a date as a string.
        /// </summary>
        /// <param name="date">A date</param>
        /// <returns>The string representation of the year of given date.</returns>
        public static string GetYearAsString(DateTime? date)
        {
            return date == null ? default(string) : Convert.ToString(GetYear(date), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Retrieves the month of a date.
        /// </summary>
        /// <param name="date">A date</param>
        /// <returns>The month of given date.</returns>
        public static int? GetMonth(DateTime? date) 
        {
            return date == null ? default(int?) : ((DateTime)date).Month+1;
        }

        /// <summary>
        /// Retrieves the month of a date as a string.
        /// </summary>
        /// <param name="date">A date</param>
        /// <returns>The string representation of the month of given date.</returns>
        public static string GetMonthAsString(DateTime? date)
        {
            return date == null ? default(string) : Convert.ToString(GetMonth(date),CultureInfo.InvariantCulture); 
        }

        /// <summary>
        /// Retrieves the day of the month of a date.
        /// </summary>
        /// <param name="date">A date</param>
        /// <returns>The day of the month of given date.</returns>
        public static int? GetDay(DateTime? date)
        {
            return date == null ? default(int?) : ((DateTime)date).Day;
        }

        /// <summary>
        /// Retrieves the day of the month of a date as a string.
        /// </summary>
        /// <param name="date">A date</param>
        /// <returns>The string representation of the day of the month of given date.</returns>
        public static string GetDayAsString(DateTime? date)
        {
            return date == null ? default(string) : Convert.ToString(GetDay(date), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts a string to a date.
        /// Supported formats : "ddMMyy", "dd/MM/yyyy", "ddMMyyyy", "dd/MM/yy" and "dd-MM-yyyy".
        /// </summary>
        /// <param name="sDate">A string reprensenting a date.</param>
        /// <returns>The date extracted from <paramref name="sDate"/>.</returns>
        public static DateTime? ConvertStringToDate(string sDate)
        {
            if (sDate == null)
            {
                return null;
            }
            DateTime result;
            DateTime.TryParseExact(sDate,
                new[]
                {
                    SHORT_DATE_FORMAT, STANDARD_DATE_FORMAT, STANDARD_DATE_FORMAT_WITHOUT_SEP, SHORT_DATE_FORMAT_WITH_SEP,
                    STANDARD_DATE_FORMAT_WITH_TIRET
                }, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
            return result;
        }
    
        /// <summary>
        /// Convert a string to a date using specified format.
        /// </summary>
        /// <param name="sDate">A string reprensenting a date.</param>
        /// <param name="format">The format to parse the date.</param>
        /// <returns>The date extracted from <paramref name="sDate"/>.</returns>
        public static DateTime? ConvertStringToDate(string sDate, string format) 
        {
            return sDate == null || format == null ? null : CheckFormatDate(sDate, format);
        }
    
        private static DateTime? CheckFormatDate(string sDate, string format)
        {
            var result = DateTime.ParseExact(sDate, format, CultureInfo.InvariantCulture);
            var formatResult = result.ToString(format, CultureInfo.InvariantCulture);
            if(formatResult.Equals(sDate))
            {
                return result;
            }
            Logger.Debug("DateUtils::checkFormatDate");
            return (null);
        }
    }
}
