using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreUtil.Extensions
{
    /// <summary>
    /// This class provides extension methods for datetime
    /// </summary>
    public static class DateExtensions
    {

        #region Validation
        
        /// <summary>
        /// Determine whether or not the date is a weekend day
        /// </summary>
        /// <param name="date">Date to check</param>
        /// <returns></returns>
        public static bool IsWeekend(this DateTime date)
        {
            bool boolIsWeekend = (date.DayOfWeek == DayOfWeek.Sunday || date.DayOfWeek == DayOfWeek.Saturday);
            
            return boolIsWeekend;
        }

        /// <summary>
        /// Whether or not the day is a leap day
        /// </summary>
        /// <param name="date">Date to check</param>
        /// <returns></returns>
        public static bool IsLeapDay(this DateTime date)
        {
            bool boolIsLeapDay = (date.Month == 2 && date.Day == 29);

            return boolIsLeapDay;
        }

        #endregion

        #region Formatting

        /// <summary>
        /// Whether or not the date is the dfirst day of the month
        /// </summary>
        /// <param name="date">Date to check</param>
        /// <returns></returns>
        public static string FirstDayOfMonth(this DateTime date)
        {
            string strDate = new DateTime(date.Year, date.Month, 1).ToString("MM/dd/yyyy");
            
            return strDate; 
        }

        /// <summary>
        /// Check date is the last day of the month
        /// </summary>
        /// <param name="date">Date to check</param>
        /// <returns></returns>
        public static string LastDayOfMonth(this DateTime date)
        {
            string strDate =  new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, 1).AddDays(-1).ToString("MM/dd/yyyy");

            return strDate; 
        }

        /// <summary>
        /// Get reversed datetime
        /// </summary>
        /// <param name="date">Date to check</param>
        /// <returns></returns>
        public static string ToReversedDateTime(this DateTime date)
        {
            string strDate = string.Format("{0:u}", date);
            
            return strDate;
        }

        /// <summary>
        /// Convert date to a sql formatted datetime
        /// </summary>
        /// <param name="date">Date to check</param>
        /// <returns></returns>
        public static string ToSqlDateTime(this DateTime date)
        {
            // Get Date Time Formatted To Sql
            string strDate = date.ToString("dd/MM/yy hh:mm:ss");

            return strDate;
        }

        #endregion
        
    }
}
