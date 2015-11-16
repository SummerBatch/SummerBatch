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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Summer.Batch.Extra.Utils;
using System;
using System.Globalization;

namespace Summer.Batch.CoreTests.Util
{
    [TestClass]
    public class DateUtilsTest
    {
        private static readonly long TEST_DATE_TIME = 635804130403255231L;
        private static readonly long A_DAY_IN_MS = TimeSpan.TicksPerDay;

        /// <summary>
        /// DateUtils.addDaysToCurrentDateTime? method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_AddDaysToCurrentDate1()
        {
            int? days = 1;
            long start = DateTime.Now.Ticks;
            DateTime? result = DateUtils.AddDaysToCurrentDate(days);		
            Assert.IsNotNull(result);
            long diff = ((DateTime)result).Ticks - start;
            Assert.AreEqual(true,(diff-TimeSpan.TicksPerDay)>=0);
            DateTime test1 = new DateTime(diff-A_DAY_IN_MS);
            DateTime test2 = new DateTime(A_DAY_IN_MS);
            Assert.AreEqual(true,(diff-A_DAY_IN_MS)<A_DAY_IN_MS);
        }

        /// <summary>
        /// DateUtils.addDaysToDateTime? method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_AddDaysToDate1()
        {
            DateTime? date = new DateTime(TEST_DATE_TIME);
            int? days = 1;
            DateTime? result = DateUtils.AddDaysToDate(date, days);
            Assert.IsNotNull(result);
            Assert.AreEqual(true,((((DateTime)result).Ticks- ((DateTime)date).Ticks)==A_DAY_IN_MS));
        }

        /// <summary>
        /// DateUtils.addMonthsToCurrentDateTime? method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_AddMonthsToCurrentDate1()
        {
            int? months = 1;
            DateTime? result = DateUtils.AddMonthsToCurrentDate(months);
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// DateUtils.addMonthsToDateTime? method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_AddMonthsToDate1() 
        {
            DateTime? date = new DateTime(TEST_DATE_TIME);
            int? months = 1;
            DateTime? result = DateUtils.AddMonthsToDate(date, months);
            Assert.IsNotNull(result);
            Assert.AreEqual(11,((DateTime)result).Month);
        }

        /// <summary>
        /// DateUtils.addYearsToCurrentDateTime? method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_AddYearsToCurrentDate1() 
        {
            int? years = 1;
            DateTime? result = DateUtils.AddYearsToCurrentDate(years);
            Assert.IsNotNull(result);
            int resultYear = ((DateTime)result).Year;
            Assert.AreEqual(true,resultYear-DateTime.Now.Year==1);
        }

        /// <summary>
        /// DateUtils.addYearsToDateTime? method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_AddYearsToDate1()
        {
            DateTime? date = new DateTime(TEST_DATE_TIME);
            int? years = 1;
            DateTime? result = DateUtils.AddYearsToDate(date, years);
            Assert.IsNotNull(result);
            Assert.AreEqual(2016,((DateTime)result).Year);
        }

        /// <summary>
        /// DateUtils.buildDateTime? method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_BuildDate1()
        {
            String day = "11";
            String month = "10";
            String year = "2013";
            DateTime? result = DateUtils.BuildDate(day, month, year);
            Assert.IsNotNull(result);
            Assert.AreEqual(((DateTime)result).Day, 11);
            Assert.AreEqual(((DateTime)result).Month, 10);
            Assert.AreEqual(((DateTime)result).Year, 2013);
        }

        /// <summary>
        /// DateUtils.buildDateTime? method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_BuildDate3()
        {
            int? day = 11;
            int? month = 10;
            int? year = 2013;
            DateTime? result = DateUtils.BuildDate(day, month, year);
            Assert.IsNotNull(result);
            Assert.AreEqual(((DateTime)result).Day, 11);
            Assert.AreEqual(((DateTime)result).Month, 10);
            Assert.AreEqual(((DateTime)result).Year, 2013);
        }

        /// <summary>
        /// DateUtils.buildDateTime? method test.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(System.FormatException))]
        public void DateUtils_BuildDate2()
        {
            String day = "";
            String month = "";
            String year = "";
            DateTime? result = DateUtils.BuildDate(day, month, year);
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// DateUtils.compareDatesIgnoringYear method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_CompareDatesIgnoringYear()
        {
            DateTime? date1 = new DateTime(2013, 10, 23);
            DateTime? date2 = new DateTime(2013, 10, 23);
            int? result = DateUtils.CompareDatesIgnoringYear(date1, date2);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result);
        }

        /// <summary>
        /// DateUtils.compareDatesIgnoringYear method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_CompareDatesIgnoringYear2()
        {
            DateTime? date1 = new DateTime(2013, 10, 23);
            DateTime? date2 = new DateTime(2014, 10, 24);
            int? result = DateUtils.CompareDatesIgnoringYear(date1, date2);
            Assert.IsNotNull(result);
            Assert.AreEqual(-1, result);
        }

        /// <summary>
        /// DateUtils.compareDatesIgnoringYear method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_CompareDatesIgnoringYear3()
        {
            DateTime? date1 = new DateTime(2013, 10, 24);
            DateTime? date2 = new DateTime(2014, 10, 23);
            int? result = DateUtils.CompareDatesIgnoringYear(date1, date2);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result);
        }

        /// <summary>
        /// Run the int? compareDatesIgnoringYear(Date,Date) method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_CompareDatesIgnoringYear4()
        {
            DateTime? date1 = null;
            DateTime? date2 = null;
            int? result = DateUtils.CompareDatesIgnoringYear(date1, date2);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result);
        }

        /// <summary>
        ///  DateUtils.convertDateToString method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_ConvertDateToString1()
        {
            DateTime? date = new DateTime();
            string pattern = "";
            string result = DateUtils.ConvertDateToString(date, pattern);
            Assert.AreEqual("01/01/0001 00:00:00", result);
        }

        /// <summary>
        ///  DateUtils.convertDateToString method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_ConvertDateToString2(){
            DateTime? date = null;
            string pattern = "";
            string result = DateUtils.ConvertDateToString(date, pattern);
            Assert.AreEqual(null, result);
        }
    
        /// <summary>
        ///  DateUtils.convertDateToString method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_ConvertDateToString3()
        {
            DateTime? date1 = new DateTime(2013, 10, 24);
            string pattern = "dd/MM/yyyy";
            string result = DateUtils.ConvertDateToString(date1, pattern);
            Assert.AreEqual("24/10/2013", result);
        }

        /// <summary>
        /// DateUtils.convertStringToDateTime? method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_ConvertStringToDate1()
        {
            string sDate = null;
            DateTime? result = DateUtils.ConvertStringToDate(sDate);
            Assert.AreEqual(null, result);
        }

        /// <summary>
        /// DateUtils.convertStringToDateTime? method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_ConvertStringToDate2()
        {
            String sDate = "";
            DateTime? result = DateUtils.ConvertStringToDate(sDate);
            Assert.AreEqual(new DateTime(), result);
        }

        /// <summary>
        /// DateUtils.convertStringToDateTime? method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_ConvertStringToDate3()
        {
            String sDate = "24112013";
            DateTime? result = DateUtils.ConvertStringToDate(sDate);
            Assert.IsNotNull(result);
            Assert.AreEqual(635208480000000000L, ((DateTime)result).Ticks);
        }

        /// <summary>
        /// DateUtils.convertStringToDateTime? method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_ConvertStringToDate4()
        {
            String sDate = "24/11/2013";
            DateTime? result = DateUtils.ConvertStringToDate(sDate);
            Assert.IsNotNull(result);
            Assert.AreEqual(635208480000000000L, ((DateTime)result).Ticks);
        }

        /// <summary>
        /// DateUtils.convertStringToDateTime? method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_ConvertStringToDate5()
        {
            String sDate = "24/11/13";
            DateTime? result = DateUtils.ConvertStringToDate(sDate);
            Assert.IsNotNull(result);
            Assert.AreEqual(635208480000000000L, ((DateTime)result).Ticks);
        }

        /// <summary>
        /// DateUtils.convertStringToDateTime? method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_ConvertStringToDate6()
        {
            String sDate = "241113";
            DateTime? result = DateUtils.ConvertStringToDate(sDate);
            Assert.IsNotNull(result);
            Assert.AreEqual(635208480000000000L, ((DateTime)result).Ticks);
        }

        /// <summary>
        /// DateUtils.convertStringToDateTime? method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_ConvertStringToDate7()
        {
            String sDate = "24-11-2013";
            DateTime? result = DateUtils.ConvertStringToDate(sDate);
            Assert.IsNotNull(result);
            Assert.AreEqual(635208480000000000L, ((DateTime)result).Ticks);
        }

        /// <summary>
        /// DateUtils.getDay method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_GetDay1()
        {
            DateTime? date1 = new DateTime(2013, 10, 24);
            int? result = DateUtils.GetDay(date1);
            Assert.IsNotNull(result);
            Assert.AreEqual(24, result);
        }

        /// <summary>
        /// DateUtils.getDayAsString method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_GetDayAsString1()
        {
            DateTime? date1 = new DateTime(2013, 10, 24);
            String result = DateUtils.GetDayAsString(date1);
            Assert.AreEqual("24", result);
        }

        /// <summary>
        /// DateUtils.getDifferenceInDays method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_GetDifferenceInDays1()
        {		
            DateTime? date1 = new DateTime();
            DateTime? date2 = new DateTime();
            int? result = DateUtils.GetDifferenceInDays(date1, date2);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result);
        }

        /// <summary>
        /// DateUtils.getDifferenceInWorkingDays method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_GetDifferenceInWorkingDays1()
        {
            DateTime? date1 = new DateTime();
            DateTime? date2 = new DateTime();
            int? result = DateUtils.GetDifferenceInWorkingDays(date1, date2);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result);
        }

        /// <summary>
        /// DateUtils.getDifferenceInWorkingDays method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_GetDifferenceInWorkingDays2()
        {
            DateTime? date1 = new DateTime(2013, 10, 24);
            DateTime? date2 = new DateTime(2013, 10, 23);
            int? result = DateUtils.GetDifferenceInWorkingDays(date1, date2);
            Assert.IsNotNull(result);
            Assert.AreEqual(-1, result);
        }


        /// <summary>
        /// DateUtils.getDifferenceInWorkingDays method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_GetDifferenceInWorkingDays3()
        {
            DateTime? date1 = new DateTime(2013, 10, 23);
            DateTime? date2 = new DateTime(2013, 10, 26);
            int? result = DateUtils.GetDifferenceInWorkingDays(date1, date2);
            Assert.IsNotNull(result);
            // 1 because of the week-end effect
            Assert.AreEqual(1, result);
        }

        /// <summary>
        /// DateUtils.getDifferenceInWorkingDays method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_GetDifferenceInWorkingDays4()
        {
            DateTime? date1 = new DateTime(2013, 10, 26);
            DateTime? date2 = new DateTime(2013, 10, 23);
            int? result = DateUtils.GetDifferenceInWorkingDays(date1, date2);
            Assert.IsNotNull(result);
            // 1 because of the week-end effect
            Assert.AreEqual(-1, result);
        }

        /// <summary>
        /// DateUtils.getDifferenceInWorkingDays method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_GetDifferenceInWorkingDays5()
        {
            DateTime? date1 = new DateTime(2013, 9, 24);
            DateTime? date2 = new DateTime(2013, 9, 23);
            int? result = DateUtils.GetDifferenceInWorkingDays(date1, date2);
            Assert.IsNotNull(result);
            Assert.AreEqual(-1, result);
        }

        /// <summary>
        /// DateUtils.getDifferenceInYears method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_GetDifferenceInYears1()
        {
            DateTime? date1 = new DateTime();
            DateTime? date2 = new DateTime();
            int? result = DateUtils.GetDifferenceInYears(date1, date2);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result);
        }
    
        /// <summary>
        /// DateUtils.getDifferenceInYears method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_GetDifferenceInYears2()
        {
            DateTime? date1 = new DateTime(2013, 9, 24);
            DateTime? date2 = new DateTime(2014, 9, 24);
            int? result = DateUtils.GetDifferenceInYears(date1, date2);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result);
        }

        /// <summary>
        /// DateUtils.getMonth method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_GetMonth1()
        {
            DateTime? date1 = new DateTime(2013, 9, 24);
            int? result = DateUtils.GetMonth(date1);
            Assert.IsNotNull(result);
            Assert.AreEqual(10, result);
        
        }

        /// <summary>
        /// DateUtils.getMonthAsString method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_GetMonthAsString1()
        {
            DateTime? date1 = new DateTime(2013, 9, 24);
            String result = DateUtils.GetMonthAsString(date1);
            Assert.AreEqual("10", result);
        }

        /// <summary>
        /// DateUtils.getYear method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_GetYear1()
        {
            DateTime? date1 = new DateTime(2013, 9, 24);
            int? result = DateUtils.GetYear(date1);
            Assert.IsNotNull(result);
            Assert.AreEqual(2013, result);
        }

    
        /// <summary>
        /// DateUtils.getYearAsString method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_GetYearAsString1()
        {
            DateTime? date1 = new DateTime(2013, 9, 24);
            String result = DateUtils.GetYearAsString(date1);
            Assert.IsNotNull(result);
            Assert.AreEqual("2013", result);
        }
    

        /// <summary>
        /// DateUtils.isInInterval method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_IsInInterval1()
        {
            DateTime? dateInf = null;
            DateTime? dateSup = null;
            DateTime? dateToTest = null;
            bool? result = DateUtils.IsInInterval(dateInf, dateSup, dateToTest);
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result);
            Assert.AreEqual("True", result.ToString());
        }

        /// <summary>
        /// DateUtils.isInInterval method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_IsInInterval2() 
        {
            DateTime? dateInf = null;
            DateTime? dateSup = new DateTime();
            DateTime? dateToTest = null;
            bool? result = DateUtils.IsInInterval(dateInf, dateSup, dateToTest);
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result);
            Assert.AreEqual("True", result.ToString());
        }

        /// <summary>
        /// Run the bool? isInInterval(Date,Date,Date) method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_IsInInterval3()
        {
            DateTime? dateInf = new DateTime(2013, 9, 24);
            DateTime? dateSup = new DateTime(2013, 10, 24);
            DateTime? dateToTest = new DateTime(2013, 10, 1);
            bool? result = DateUtils.IsInInterval(dateInf, dateSup, dateToTest);
            // add additional test code here
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result);
            Assert.AreEqual("True", result.ToString());
        }

        /// <summary>
        /// Run the bool? isInInterval(Date,Date,Date) method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_IsInInterval4()
        {
            DateTime? dateInf = new DateTime(2013, 9, 24);
            DateTime? dateSup = new DateTime(2014, 10, 24);
            DateTime? dateToTest = new DateTime(2014, 10, 25);
            bool? result = DateUtils.IsInInterval(dateInf, dateSup, dateToTest);
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result);
            Assert.AreEqual("False", result.ToString());
        }

        /// <summary>
        /// DateUtils.updateYear method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_UpdateYear1()
        {
            DateTime? date = new DateTime(2013, 9, 24);
            int? year = 2014;
            DateTime? result = DateUtils.UpdateYear(date, year);
            Assert.IsNotNull(result);
            Assert.AreEqual(635471136000000000L, ((DateTime)result).Ticks);
        }

        /// <summary>
        /// DateUtils.updateYearAndMonth method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_UpdateYearAndMonth1()
        {
            DateTime? date = new DateTime(2013, 9, 24);
            int? year = 2014;
            int? month = 10;
            DateTime? result = DateUtils.UpdateYearAndMonth(date, year, month);
            Assert.IsNotNull(result);
            Assert.AreEqual(635497056000000000L, ((DateTime)result).Ticks);
        }

        /// <summary>
        /// DateUtils.updateYearMonthAndDay method test.
        /// </summary>
        [TestMethod]
        public void DateUtils_UpdateYearMonthAndDay1()
        {
            DateTime? date = new DateTime(2013, 9, 24);
            int? year = 2014;
            int? month = 10;
            int? day = 25;
            DateTime? result = DateUtils.UpdateYearMonthAndDay(date, year, month, day);
            Assert.IsNotNull(result);
            Assert.AreEqual(635497920000000000L, ((DateTime)result).Ticks);
        }
    }
}
