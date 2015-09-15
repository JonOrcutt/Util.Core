using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreUtil;

namespace ExceptionUtil.New
{
    public static class Logger
    {
        #region Validation

        private static Globals.ResultType CheckSystemException(Exception exception)
        {
            // Validation
            if (exception == null || exception.Message == null) { return Globals.ResultType.Failure; }

            string strExceptionMessage = exception.Message.ToLower();

            // Validation
            if (strExceptionMessage.Contains("attempted to read or write protected memory. this is often an indication that other memory is corrupt") == true)
               { return Globals.ResultType.Failure; }

            System.Diagnostics.StackTrace CurrentStackTest = new System.Diagnostics.StackTrace(exception, 0, true);

            // Validation
            if (CurrentStackTest == null || CurrentStackTest.FrameCount == 0) { return Globals.ResultType.Failure; }

            return Globals.ResultType.Success;
        }
        

        #endregion
    }
}
