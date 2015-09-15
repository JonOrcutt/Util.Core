using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreUtil
{
    /// <summary>
    /// Global Logging Class
    /// </summary>
    public class Log
    {
        #region Properties

        //internal static log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Level of logging to be logged
        /// </summary>
        internal static LogLevelType Level = LogLevelType.Unknown;

        /// <summary>
        /// The log level type to be used in determining how much detailed information is logged
        /// </summary>
        public enum LogLevelType : int
        {
            /// <summary>
            /// The log level is unknown
            /// </summary>
            Unknown = 0,

            /// <summary>
            /// There is a low level of logging
            /// </summary>
            Low = 1,

            /// <summary>
            /// There is a medium level of logging
            /// </summary>
            Medium = 2,

            /// <summary>
            /// There is a high level of logging
            /// </summary>
            High = 3
        }

        /// <summary>
        /// Message logging type
        /// </summary>
        public enum LogType : int
        {
            /// <summary>
            /// The logging type is unknown
            /// </summary>
            Unknown = 0,

            /// <summary>
            /// Messages are logged to the output console
            /// </summary>
            Console = 1,

            /// <summary>
            /// Messages are logged to a log file
            /// </summary>
            LogFile = 2,

            /// <summary>
            /// Messages are logged to a database table
            /// </summary>
            Database = 3,

            /// <summary>
            /// Messages are logged to the output console, a log file, and a database table
            /// </summary>
            All = 4
        }


        #endregion

        #region Get Logger FileName

        /// <summary>
        /// Retrieve log4net filename
        /// </summary>
        /// <returns></returns>
        internal static string GetLoggerFileName()
        {
            String filename = null;
            //log4net.Appender.IAppender[] appenders = Logger.Logger.Repository.GetAppenders();

            // Check each appender this logger has
            //foreach (log4net.Appender.IAppender appender in appenders)
            //{
            //    Type t = appender.GetType();
            //    if (t.Equals(typeof(log4net.Appender.FileAppender)) || t.Equals(typeof(log4net.Appender.RollingFileAppender)))
            //    {
            //        filename = ((log4net.Appender.FileAppender)appender).File;
            //        break;
            //    }
            //}

            return filename;
        }

        #endregion

        #region Log Message

        /// <summary>
        /// Log a debug message
        /// </summary>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <param name="ex">Optional: Exception thrown to log</param>
        internal static void Debug(string strError, Exception ex = null)
        {
            if (ex != null)
            {
                //Logger.Debug(strError, ex);
            }
            else
            {
                //Logger.Debug(strError);
            }

            string strConsoleMessage = "DEBUG: " + strError + ((ex != null) ? "\r\n" + ex.ToString() : "");
            Console.WriteLine(strConsoleMessage);
        }

        /// <summary>
        /// Log an information message
        /// </summary>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <param name="ex">Optional: Exception thrown to log</param>
        internal static void Info(string strError, Exception ex = null)
        {
            if (ex != null)
            {
                //Logger.Info(strError, ex);
            }
            else
            {
                //Logger.Info(strError);
            }

            string strConsoleMessage = "INFO: " + strError + ((ex != null) ? "\r\n" + ex.ToString() : "");
            Console.WriteLine(strConsoleMessage);
        }

        /// <summary>
        /// Log a warning message
        /// </summary>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <param name="ex">Optional: Exception thrown to log</param>
        internal static void Warning(string strError, Exception ex = null)
        {
            if (ex != null)
            {
                //Logger.Warn(strError, ex);
            }
            else
            {
                //Logger.Warn(strError);
            }

            string strConsoleMessage = "WARNING: " + strError + ((ex != null) ? "\r\n" + ex.ToString() : "");
            Console.WriteLine(strConsoleMessage);
        }

        /// <summary>
        /// Log an error message
        /// </summary>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <param name="ex">Optional: Exception thrown to log</param>
        internal static void Error(string strError, Exception ex = null)
        {
            if (ex != null)
            {
                //Logger.Error(strError, ex);
            }
            else
            {
                //Logger.Error(strError);
            }

            string strConsoleMessage = "ERROR: " + strError + ((ex != null) ? "\r\n" + ex.ToString() : "");
            Console.WriteLine(strConsoleMessage);
        }

        #endregion
    }
}
