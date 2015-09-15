using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ExceptionUtil
{
    public class MethodInformation
    {
        #region Properties

        private string m_Name = "";
        public string Name
        {
            get
            {
                return this.m_Name;
            }
        }

        private string m_ReturnType = "";
        public string ReturnType
        {
            get
            {
                return this.m_ReturnType;
            }
        }

        private string m_ReturnParameter = "";
        public string ReturnParameter
        {
            get
            {
                return this.m_ReturnParameter;
            }
        }

        private int m_LineNumber = 0;
        public int LineNumber
        {
            get
            {
                return this.m_LineNumber;
            }
        }

        private List<ParameterInformation> m_Parameters = new List<ParameterInformation>();
        public List<ParameterInformation> Parameters
        {
            get
            {
                return this.m_Parameters;
            }
        }

        #endregion

        #region Initialization

        public MethodInformation(Exception exception, System.Diagnostics.StackFrame stackFrame)
        {
            this.m_Name = stackFrame.GetMethod().Name;

            this.LoadMethodProperties(exception, stackFrame);
        }

        #endregion

        #region Functions

        internal void LoadMethodProperties(Exception exception, System.Diagnostics.StackFrame stackFrame)
        {
            // Validation
            if (exception == null || exception.TargetSite == null || typeof(System.Reflection.MethodInfo).IsAssignableFrom(exception.TargetSite.GetType()) == false) { return; }

            // Get Reflection Information
            System.Reflection.MethodBase methodBase = stackFrame.GetMethod();
            System.Reflection.MethodBody methodBody = methodBase.GetMethodBody();
            System.Reflection.MethodInfo methodInfo = (methodBase != null && typeof(System.Reflection.MethodInfo).IsAssignableFrom(methodBase.GetType()) == true) ? (System.Reflection.MethodInfo)methodBase : null;
            System.Reflection.MethodInfo targetSiteInfo = ((System.Reflection.MethodInfo)exception.TargetSite);

            // Get Return Information
            this.m_ReturnParameter = (targetSiteInfo != null && targetSiteInfo.ReturnParameter != null) ? targetSiteInfo.Name : "";
            this.m_ReturnType = (targetSiteInfo != null && targetSiteInfo.ReturnType != null) ? targetSiteInfo.ReturnType.FullName : "";

            // Get Line Number
            int intLineNumber = (GetLineNumber(exception) == 0) ? stackFrame.GetFileLineNumber() : GetLineNumber(exception);
            this.m_LineNumber = (this.LineNumber == 0) ? stackFrame.GetFileLineNumber() : intLineNumber;

            if (methodInfo != null && methodInfo.GetParameters().Count() > 0)
            {
                this.m_Parameters = methodInfo.GetParameters().Select(param => new ParameterInformation(param.Name, param.ParameterType.ToString())).ToList();
            }
        }
        
        private static int GetLineNumber(Exception exception)
        {
            // Attempt To Get Line Number From Stack Trace
            int intLineNumber = 0;
            if (exception == null || exception.StackTrace == null || exception.StackTrace.ToLower().Contains(":line") == false) { return intLineNumber; }

            string[] strValues = exception.StackTrace.Split(':');
            if (strValues.Length > 0 && strValues[strValues.Length - 1].Contains("line") == true)
            {
                string strValue = strValues[strValues.Length - 1].Replace("line ", "");
                intLineNumber = (strValue.Contains(" ") == true && int.TryParse(strValue.Substring(0, strValue.IndexOf(" ")).Trim(), out intLineNumber) == true) ? int.Parse(strValue.Substring(0, strValue.IndexOf(" ")).Trim()) : 0;
                return intLineNumber;
            }

            return intLineNumber;
        }

        #endregion

    }
}