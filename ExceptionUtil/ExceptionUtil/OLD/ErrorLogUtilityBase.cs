//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;


//namespace ExceptionUtil
//{
//    public class ErrorLogUtilityBase
//    {

//        // MAIN ERROR CLASSES (BUILDING BLOCKS)

//        #region Internal Classes

//        public class ErrorInfoList : List<ErrorInfo>
//        {
//            #region Initialization

//            public ErrorInfoList()
//            {
//            }

//            #endregion
//        }

//        public class ErrorInfo
//        {
//            #region Properties

//            public string ErrorString = "";
//            public string ErrorXML = "";
//            public System.Xml.XmlElement ErrorXmlElement = null;

//            internal ExceptionInfo ExceptionData = new ExceptionInfo();
//            internal UserInfo User = new UserInfo();
//            internal FileInfo File = new FileInfo();
//            internal AssemblyInfo Assembly = new AssemblyInfo();

//            #endregion

//            #region Initialization

//            public ErrorInfo()
//            {
//            }

//            #endregion
//        }

//        protected internal class ExceptionInfo
//        {
//            #region Properties

//            internal Exception Exception = null;
//            internal Type Type = null;
//            internal string StackTrace = "";
//            internal string InnerException = "";
//            internal string Message = "";

//            #endregion

//            #region Initialization

//            public ExceptionInfo()
//            {
//            }

//            #endregion
//        }

//        protected internal class UserInfo
//        {
//            #region Properties

//            internal string DomainUserName = "";
//            internal DateTime ErrorDateTime = DateTime.MinValue;
//            internal TimeSpan ApplicationElapsedTime = new TimeSpan(0, 0, 0);

//            internal string ApplicationElapsedTimeStringFormatted
//            {
//                get
//                {
//                    string strTimeElapsed = "";
//                    strTimeElapsed += (this.ApplicationElapsedTime.Hours > 0) ? this.ApplicationElapsedTime.TotalHours.ToString() + " Days, " : "";
//                    strTimeElapsed += (this.ApplicationElapsedTime.Minutes > 0) ? this.ApplicationElapsedTime.Minutes.ToString() + " Minutes, " : "";
//                    strTimeElapsed += (this.ApplicationElapsedTime.Seconds > 0) ? this.ApplicationElapsedTime.Seconds.ToString() + " Seconds" : "";

//                    return strTimeElapsed;
//                }
//            }

//            #endregion

//            #region Initialization

//            public UserInfo()
//            {

//            }

//            #endregion
//        }

//        protected internal class AssemblyInfo
//        {
//            #region Properties

//            internal string FullName = "";
//            internal string Namespace = "";
//            internal string QualifiedName = "";
//            internal string FilePath = "";
//            internal string Version = "";

//            #endregion

//            #region Initialization

//            public AssemblyInfo()
//            {

//            }

//            #endregion
//        }

//        protected internal class FileInfo
//        {
//            #region Properties

//            internal string ClassName = "";
//            internal string FullName = "";
//            internal string LineNumber = "";
//            internal string AttemptedMethod = "";
//            internal MethodInfo Method = new MethodInfo();

//            #endregion

//            #region Initialization

//            public FileInfo()
//            {
//            }

//            #endregion

//            #region Internal Classes

//            protected internal class MethodInfo
//            {
//                internal string MethodName = "";
//                internal bool IsVirtual = false;
//                internal bool IsStatic = false;
//                internal bool IsPublic = false;
//                internal bool IsPrivate = false;
//                internal bool IsConstructor = false;

//                internal string ReturnType = "";
//                internal string ReturnParameter = "";

//                internal List<ParameterInfo> Parameters = new List<ParameterInfo>();
//                protected internal class ParameterInfo
//                {
//                    #region Properties

//                    internal string Name = "";
//                    internal string Type = "";

//                    #endregion

//                    #region Initialization

//                    public ParameterInfo()
//                    {
//                    }

//                    #endregion
//                }

//                public MethodInfo()
//                {

//                }
//            }

//            #endregion
//        }

//        #endregion

//    }

//}