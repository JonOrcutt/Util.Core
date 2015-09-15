//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.IO;
//using CoreUtil;

//namespace ExceptionUtil
//{
//    public sealed class ErrorLogUtility : ErrorLogUtilityBase
//    {

//        #region Properties

//        private static ErrorInfo m_Error = null;
//        protected internal static ErrorInfo Error
//        {
//            get
//            {
//                return m_Error;
//            }
//            set
//            {
//                m_Error = value;
//            }
//        }

//        private static ErrorInfoList m_ErrorList = new ErrorInfoList();
//        protected internal static ErrorInfoList ErrorList
//        {
//            get
//            {
//                return m_ErrorList;
//            }
//            set
//            {
//                m_ErrorList = value;
//            }
//        }

//        #endregion

//        #region Public Functions

//        public static ErrorInfo WriteError(Exception ex)
//        {
//            bool boolConsoleWrittenTo = false;

//            string strError = "";
//            Globals.ResultType resultStatus = Globals.ResultType.Unknown;

//            Error = new ErrorInfo();
//            Error.ExceptionData.Exception = ex;            

//            try
//            {
//                // Load Error Message From Exception via Reflection
//                resultStatus = LoadError(Error.ExceptionData.Exception, ref strError);
//            }
//            catch (Exception ExMiscellaneous)
//            {
//                if (boolConsoleWrittenTo == false)
//                {
//                    string strMiscException = ExMiscellaneous.ToString();
//                    boolConsoleWrittenTo = true;
//                }

//                resultStatus = Globals.ResultType.Failure;
//            }

//            return Error;
//        }

//        #endregion


//        #region Log Exception

//        //private static Globals.ResultType WriteErrorToConsole(string strError)
//        //{
//        //    Globals.ResultType resultStatus = Globals.ResultType.Unknown;

//        //    Console.BackgroundColor = ConsoleColor.Black;
//        //    Console.ForegroundColor = ConsoleColor.Green;

//        //    try
//        //    {
//        //        Console.Write(strError);
//        //        resultStatus = Globals.ResultType.Success;
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        // Currently Unused Exception Info 
//        //        string strExceptionWritingToConsole = ex.ToString();

//        //        resultStatus = Globals.ResultType.Failure;
//        //    }

//        //    return resultStatus;
//        //}
        
//        #endregion

//        #region Load Error

//        private static Globals.ResultType LoadError(Exception exception, ref string strError)
//        {
//            try
//            {
//                // CHECK ALL ERROR OBJECTS ARE VALID
//                bool boolAllObjectsValid = CheckNullObjectReferences(ref strError);
//                if (boolAllObjectsValid == false || strError != "") { return Globals.ResultType.Failure; }

//                // CHECK IS SYSTEM EXCEPTION
//                Globals.ResultType checkStatus = CheckSystemException(exception);
//                if (checkStatus != Globals.ResultType.Success) { return checkStatus; }

//                // LOAD EXCEPTION PROPERTIES
//                LoadExceptionProperties(exception);

//                // LOAD USER PROPERTIES
//                LoadUserProperties();
                
//                // LOOP STACK TRACE
//                System.Diagnostics.StackTrace CurrentStackTest = new System.Diagnostics.StackTrace(exception, 0, true);
//                for (int intIndex = 0; intIndex < CurrentStackTest.FrameCount; intIndex++)
//                {
//                    // MAIN REFLECTION VARS
//                    System.Diagnostics.StackFrame frame = (CurrentStackTest != null && intIndex >= 0) ? CurrentStackTest.GetFrame(intIndex) : null;
//                    System.Reflection.MethodBase methodBase = (frame != null) ? frame.GetMethod() : null;
//                    System.Reflection.MethodBody methodBody = (methodBase != null) ? methodBase.GetMethodBody() : null;
//                    System.Reflection.MethodInfo info = (methodBase != null && typeof(System.Reflection.MethodInfo).IsAssignableFrom(methodBase.GetType()) == true) ? info = (System.Reflection.MethodInfo)methodBase : null;
                    
//                    // IF METHOD NAME IS THIS CURRENT METHOD - CONTINUE
//                    if (Error.File.Method.MethodName == System.Reflection.MethodBase.GetCurrentMethod().Name) { continue; }

//                    // LOAD ASSEMBLY PROPERTIES
//                    LoadAssemblyProperties(info);

//                    // LOAD METHOD PROPERTIES
//                    LoadMethodProperties(exception, frame);

//                    // SET ATTEMPTED METHOD
//                    System.Diagnostics.StackFrame previousFrame = (intIndex - 1 >= 0) ? CurrentStackTest.GetFrame(intIndex - 1) : null;
//                    System.Reflection.MethodBase previousBase = (previousFrame != null) ? ((System.Reflection.MethodBase)previousFrame.GetMethod()) : null;

//                    string strMethod = (previousBase != null && typeof(System.Reflection.MethodInfo).IsAssignableFrom(previousBase.GetType()) == true) ? previousBase.DeclaringType.FullName + " - " + ((System.Reflection.MethodInfo)previousBase).ToString() : info.Name;
//                    Error.File.AttemptedMethod = (intIndex < 2 && strMethod != Error.File.Method.MethodName) ? strMethod : Error.File.AttemptedMethod;
//                }

//                // LOAD ERROR STRING
//                Globals.ResultType populateErrorResult = LoadErrorString(ref strError);
//                if (populateErrorResult == Globals.ResultType.Failure || strError != "") { return Globals.ResultType.Failure; }

//                // ADD ERROR TO LIST
//                ErrorList.Add(Error);

//                // RETURN SUCCESS
//                return Globals.ResultType.Success;
//            }
//            catch (Exception ex1)
//            {
//                if (ex1.InnerException != null)
//                {
//                    Error.ErrorString = ex1.InnerException.ToString();
//                }

//                // RETURN FAILURE
//                return Globals.ResultType.Failure;
//            }
//        }

//        #region Load Properties

//        private static void LoadUserProperties()
//        {
//            Error.User.ErrorDateTime = DateTime.Now;
//            Error.User.ApplicationElapsedTime = DateTime.Now - System.Diagnostics.Process.GetCurrentProcess().StartTime;
//            Error.User.DomainUserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
//        }

//        private static void LoadMethodProperties(Exception exception, System.Diagnostics.StackFrame stackFrame)
//        {
//            if (exception == null || exception.TargetSite == null || typeof(System.Reflection.MethodInfo).IsAssignableFrom(exception.TargetSite.GetType()) == false) { return; }

//            // SET SCOPE PROPERTIES
//            Error.File.Method.IsVirtual = exception.TargetSite.IsVirtual;
//            Error.File.Method.IsStatic = exception.TargetSite.IsStatic;
//            Error.File.Method.IsPublic = exception.TargetSite.IsPublic;
//            Error.File.Method.IsPrivate = exception.TargetSite.IsPrivate;
//            Error.File.Method.IsConstructor = exception.TargetSite.IsConstructor;

//            // LOCAL VARS
//            System.Reflection.MethodBase methodBase = stackFrame.GetMethod();
//            System.Reflection.MethodBody methodBody = methodBase.GetMethodBody();
//            System.Reflection.MethodInfo methodInfo = (methodBase != null && typeof(System.Reflection.MethodInfo).IsAssignableFrom(methodBase.GetType()) == true) ? (System.Reflection.MethodInfo)methodBase : null;
//            System.Reflection.MethodInfo targetSiteInfo = ((System.Reflection.MethodInfo)exception.TargetSite);

//            // SET RETURN INFO
//            Error.File.Method.ReturnParameter = (targetSiteInfo != null && targetSiteInfo.ReturnParameter != null) ? targetSiteInfo.Name : "";
//            Error.File.Method.ReturnType = (targetSiteInfo != null && targetSiteInfo.ReturnType != null) ? targetSiteInfo.ReturnType.FullName : "";

//            // GET LINE NUMBER
//            int intLineNumber = GetLineNumber(exception);
//            intLineNumber = (intLineNumber == 0) ? stackFrame.GetFileLineNumber() : intLineNumber;
//            Error.File.LineNumber = (Error.File.LineNumber == "0") ? stackFrame.GetFileLineNumber().ToString() : intLineNumber.ToString();

//            if (Error.File.Method.Parameters.Count == 0)
//            {
//                Error.File.Method.Parameters.Clear();
//                System.Reflection.ParameterInfo[] paramsInfo = methodInfo.GetParameters();
//                foreach (System.Reflection.ParameterInfo paramInfo in paramsInfo)
//                {
//                    if (paramsInfo == null || paramInfo.Name == null || paramInfo.ParameterType == null) { continue; }

//                    FileInfo.MethodInfo.ParameterInfo parameter = new FileInfo.MethodInfo.ParameterInfo();
//                    parameter.Name = paramInfo.Name;
//                    parameter.Type = paramInfo.ParameterType.Name; //.ToString();
//                    Error.File.Method.Parameters.Add(parameter);
//                }
//            }

//            Error.File.ClassName = (methodInfo != null && methodInfo.ReflectedType != null) ? methodInfo.ReflectedType.Name : "";
//            Error.File.Method.MethodName = stackFrame.GetMethod().Name;
//            Error.File.FullName = methodBase.ReflectedType.FullName + " - " + Error.File.Method.MethodName;

//            System.Reflection.MethodBase baseThis = System.Reflection.MethodBase.GetCurrentMethod();
//            if (Error.File.Method.MethodName == System.Reflection.MethodBase.GetCurrentMethod().Name || Error.File.Method.MethodName == "WriteError")
//            {
//            }
//            else
//            {
//                //Error.File.AttemptedMethod = Error.File.Method.MethodName;
//            }
//        }

//        private static void LoadAssemblyProperties(System.Reflection.MethodInfo info)
//        {
//            if (info == null) { return; }

//            // SET ASSEMBLY PROPERTIES
//            Error.Assembly.FullName = info.ReflectedType.FullName;
//            Error.Assembly.Namespace = info.ReflectedType.Namespace;
//            Error.Assembly.QualifiedName = info.ReflectedType.AssemblyQualifiedName;
//            Error.Assembly.Version = Error.Assembly.QualifiedName.Replace(Error.Assembly.QualifiedName.Substring(0, Error.Assembly.QualifiedName.IndexOf("=") + 1), "");
//            Error.Assembly.Version = (Error.Assembly.Version.Contains(",") == true) ? Error.Assembly.Version.Substring(0, Error.Assembly.Version.IndexOf(",")) : "";

//            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
//            if (assembly != null && assembly.CodeBase != "")
//            {
//                Error.Assembly.FilePath = "Assembly File Path: " + assembly.CodeBase + "\n";
//            }
//        }

//        private static void LoadExceptionProperties(Exception exception)
//        {
//            // SET EXCEPTION PROPERTIES
//            Error.ExceptionData.Exception = exception;
//            Error.ExceptionData.Type = exception.GetType();
//            Error.ExceptionData.StackTrace = (exception.StackTrace != null) ? exception.StackTrace : "";
//            Error.ExceptionData.InnerException = (exception.InnerException != null) ? exception.InnerException.ToString() : "";
//            Error.ExceptionData.Message = exception.Message;
//        }

//        #endregion

//        #region Other Functions

//        private static Globals.ResultType LoadErrorString(ref string strError)
//        {
//            // BUILD ERROR MESSAGE STRING

//            System.Text.StringBuilder errorSB = new System.Text.StringBuilder();

//            errorSB.AppendLine("<***************************************************************************************************************>");
//            errorSB.AppendLine(">    *********** An Exception Was Thrown ***********");
//            errorSB.AppendLine(">");
//            errorSB.AppendLine(">	Instance Exception Count: " + (ErrorList.Count + 1).ToString());
//            errorSB.AppendLine(">");
//            errorSB.AppendLine(">	Domain User: " + Error.User.DomainUserName);
//            errorSB.AppendLine(">");
//            errorSB.AppendLine(">	Date Time: " + Error.User.ErrorDateTime.ToString());
//            errorSB.AppendLine(">");
//            errorSB.AppendLine(">	Application Time Elapsed: " + Error.User.ApplicationElapsedTimeStringFormatted);
//            errorSB.AppendLine(">");
//            //errorSB.AppendLine">   Version: " + Error.Assembly.Version);
//            //errorSB.AppendLine">");
//            errorSB.AppendLine(">	   Class Name: " + Error.File.ClassName);
//            errorSB.AppendLine(">");
//            //errorSB.AppendLine(">	   Full Name: " + Error.File.FullName);
//            //errorSB.AppendLine(">");
//            errorSB.AppendLine(">	   Method Name: " + Error.File.Method.MethodName);
//            errorSB.AppendLine(">");

//            string strLineNumber = (Error.File.LineNumber != "") ? ">	   Line Number: " + Error.File.LineNumber : "";
//            if (strLineNumber != "")
//            {
//                errorSB.AppendLine(strLineNumber);
//                errorSB.AppendLine(">");
//            }

//            string strAttemptedMethod = (Error.File.AttemptedMethod != Error.File.Method.MethodName && Error.File.AttemptedMethod != "") ? ">	   Attempted Method: " + Error.File.AttemptedMethod : "";
//            if (strAttemptedMethod != "")
//            {
//                errorSB.AppendLine(strAttemptedMethod);
//                errorSB.AppendLine(">");
//            }

//            errorSB.AppendLine(">	      Exception Type: " + Error.ExceptionData.Type.FullName);
//            errorSB.AppendLine(">");
//            errorSB.AppendLine(">	      Error Message: " + Error.ExceptionData.Message);
//            errorSB.AppendLine(">");

//            errorSB.AppendLine(">	      Local Variable Count: " + Error.File.Method.Parameters.Count.ToString());
//            errorSB.AppendLine(">");

//            int intLength = Error.File.Method.Parameters.Select(p => p.Name.Length).Max();
//            foreach (FileInfo.MethodInfo.ParameterInfo parameter in Error.File.Method.Parameters)
//            {
//                string strLine = ">	         " + parameter.Name.PadRight(intLength + 1, ' ') + " ~ " + parameter.Type;
//                strLine += (Error.File.Method.Parameters.LastIndexOf(parameter) != Error.File.Method.Parameters.Count - 1) ? ", " : "";
//                errorSB.AppendLine(strLine);
//            }

//            errorSB.AppendLine(">");

//            errorSB.AppendLine(">	Namespace: " + Error.Assembly.Namespace);
//            errorSB.AppendLine(">");
//            errorSB.AppendLine(">	Assembly Qualified Name: " + Error.Assembly.QualifiedName);

//            errorSB.AppendLine("<***************************************************************************************************************>");
//            errorSB.AppendLine("_________________________________________________________________________________________________________________");
//            errorSB.AppendLine("");

//            Error.ErrorString = errorSB.ToString();

//            return Globals.ResultType.Success;
//        }

//        private static int GetLineNumber(Exception exception)
//        {
//            // ATTEMPT GET EXCEPTION LINE NUMBER FROM STACKTRACE
//            int intLineNumber = 0;
//            if (exception == null || exception.StackTrace == null || exception.StackTrace.ToLower().Contains(":line") == false) { return intLineNumber; }

//            string[] strValues = exception.StackTrace.Split(':');
//            if (strValues.Length > 0 && strValues[strValues.Length - 1].Contains("line") == true)
//            {
//                string strValue = strValues[strValues.Length - 1].Replace("line ", "");
//                intLineNumber = (strValue.Contains(" ") == true && int.TryParse(strValue.Substring(0, strValue.IndexOf(" ")).Trim(), out intLineNumber) == true) ? int.Parse(strValue.Substring(0, strValue.IndexOf(" ")).Trim()) : 0;
//                return intLineNumber;
//            }

//            return intLineNumber;
//        }

//        #endregion

//        #endregion
        
//        #region Validation

//        private static Globals.ResultType CheckSystemException(Exception exception)
//        {
//            if (exception == null || exception.Message == null) { return Globals.ResultType.Failure; }

//            string strExceptionMessage = exception.Message.ToLower();
//            if (strExceptionMessage.Contains("attempted to read or write protected memory. this is often an indication that other memory is corrupt") == true)
//            {
//                return Globals.ResultType.Failure;
//            }

//            System.Diagnostics.StackTrace CurrentStackTest = new System.Diagnostics.StackTrace(exception, 0, true);
//            if (CurrentStackTest == null || CurrentStackTest.FrameCount == 0)
//            {
//                return Globals.ResultType.Failure;
//            }

//            return Globals.ResultType.Success;
//        }

//        private static bool CheckNullObjectReferences(ref string strError)
//        {
//            bool boolValid = true;

//            if (Error == null)
//            {
//                boolValid = false;
//                strError = "Error: Object 'Error' is NULL";
//            }
//            else if (Error.Assembly == null)
//            {
//                boolValid = false;
//                strError = "Error: Object 'Error.Assembly' is NULL";
//            }
//            else if (Error.ExceptionData == null)
//            {
//                boolValid = false;
//                strError = "Error: Object 'Error.ExceptionData' is NULL";
//            }
//            else if (Error.File == null)
//            {
//                boolValid = false;
//                strError = "Error: Object 'Error.File' is NULL";
//            }
//            else if (Error.File.Method == null)
//            {
//                boolValid = false;
//                strError = "Error: Object 'Error.File.Method' is NULL";
//            }

//            return boolValid;
//        }

//        #endregion
//    }
//}