//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;


//namespace ExceptionUtil
//{
//    public class ExceptionInformation
//    {
//        #region Properties

//        private Exception m_Exception = null;
//        public Exception Exception
//        {
//            get
//            {
//                return this.m_Exception;
//            }
//        }

//        private Type m_Type = null;
//        public Type Type
//        {
//            get
//            {
//                return this.m_Type;
//            }
//        }

//        private string m_StackTrace = "";
//        public string StackTrace
//        {
//            get
//            {
//                return this.StackTrace;
//            }
//        }

//        private string m_InnerException = "";
//        public string InnerException
//        {
//            get
//            {
//                return this.m_InnerException;
//            }
//        }

//        private string m_Message = "";
//        public string Message
//        {
//            get
//            {
//                return this.m_Message;
//            }
//        }

//        #endregion

//        #region Initialization

//        public ExceptionInformation(Exception exception)
//        {
//            // Get Exception Information
//            this.m_Exception = exception;
//            this.m_Type = exception.GetType();
//            this.m_StackTrace = (exception.StackTrace != null) ? exception.StackTrace : "";
//            this.m_InnerException = (exception.InnerException != null) ? exception.InnerException.ToString() : "";
//            this.m_Message = exception.Message;
//        }        

//        #endregion
//    }
//}