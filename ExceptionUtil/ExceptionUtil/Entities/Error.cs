using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExceptionUtil
{
    public class Error
    {
        #region Properties

        #region Exception

        private Exception m_Exception = null;
        public Exception Exception
        {
            get
            {
                return this.m_Exception;
            }
        }
        
        #endregion

        #region Information

        private ApplicationInformation m_Application = null;
        public ApplicationInformation Application
        {
            get
            {
                return this.m_Application;
            }
        }

        private ClassFileInformation m_Class = null;
        public ClassFileInformation Class
        {
            get
            {
                return this.m_Class;
            }
        }

        private AssemblyInformation m_Assembly = null;
        public AssemblyInformation Assembly
        {
            get
            {
                return this.m_Assembly;
            }
        }

        #endregion

        #endregion

        #region Initialization

        public Error(Exception exception)
        {
            this.m_Exception = exception;

            try
            {
                // Load Error
                this.LoadError();
            }
            catch (Exception ex)
            {
                this.m_Exception = ex;

                // Load Error
                this.LoadError();
            }
        }

        #endregion

        #region Functions

        private void LoadError()
        {
            // Get Application Information
            this.m_Application = new ApplicationInformation();

            // Loop Stack Trace Frames
            System.Diagnostics.StackTrace CurrentStackTest = new System.Diagnostics.StackTrace(this.Exception, 0, true);
            for (int intIndex = 0; intIndex < CurrentStackTest.FrameCount; intIndex++)
            {
                // Get Reflection Information
                System.Diagnostics.StackFrame frame = (CurrentStackTest != null && intIndex >= 0) ? CurrentStackTest.GetFrame(intIndex) : null;
                System.Reflection.MethodBase methodBase = (frame != null) ? frame.GetMethod() : null;
                System.Reflection.MethodBody methodBody = (methodBase != null) ? methodBase.GetMethodBody() : null;
                System.Reflection.MethodInfo info = (methodBase != null && typeof(System.Reflection.MethodInfo).IsAssignableFrom(methodBase.GetType()) == true) ? info = (System.Reflection.MethodInfo)methodBase : null;

                //// Validation
                if (this.m_Class != null && this.m_Class.Method.Name != System.Reflection.MethodBase.GetCurrentMethod().Name)
                {
                    // SET ATTEMPTED METHOD
                    System.Diagnostics.StackFrame previousFrame = (intIndex - 1 >= 0) ? CurrentStackTest.GetFrame(intIndex - 1) : null;
                    System.Reflection.MethodBase previousBase = (previousFrame != null) ? ((System.Reflection.MethodBase)previousFrame.GetMethod()) : null;

                    string strMethod = (previousBase != null && typeof(System.Reflection.MethodInfo).IsAssignableFrom(previousBase.GetType()) == true) ? previousBase.DeclaringType.FullName + " - " + ((System.Reflection.MethodInfo)previousBase).ToString() : info.Name;
                    this.m_Class.AttemptedMethod = (intIndex < 2 && strMethod != this.m_Class.Method.Name) ? strMethod : this.m_Class.AttemptedMethod;
                }

                // Validation
                if (intIndex > 2) { continue; }

                // LOAD ASSEMBLY PROPERTIES
                this.m_Assembly = new AssemblyInformation(info);

                // LOAD METHOD PROPERTIES
                this.m_Class = new ClassFileInformation(this.Exception, frame);
            }
        }

        public string ErrorString()
        {
            // Create Error Message

            System.Text.StringBuilder errorSB = new System.Text.StringBuilder();

            errorSB.AppendLine("<***************************************************************************************************************>");
            errorSB.AppendLine(">    *********** An Exception Was Thrown ***********");
            errorSB.AppendLine(">");
            errorSB.AppendLine(">	Domain User: " + this.Application.DomainUserName);
            errorSB.AppendLine(">");
            errorSB.AppendLine(">	Date Time: " + this.Application.ErrorDateTime.ToString());
            errorSB.AppendLine(">");
            //errorSB.AppendLine(">	Application Time Elapsed: " + this.Application.ApplicationElapsedTimeStringFormatted);
            errorSB.AppendLine(">");
            //errorSB.AppendLine">   Version: " + Error.Assembly.Version);
            //errorSB.AppendLine">");
            errorSB.AppendLine(">	   Class Name: " + this.Class.Name);
            errorSB.AppendLine(">");
            //errorSB.AppendLine(">	   Full Name: " + Error.File.FullName);
            //errorSB.AppendLine(">");
            errorSB.AppendLine(">	   Method Name: " + this.Class.Method.Name);            errorSB.AppendLine(">");

            string strLineNumber = ">	   Line Number: " + this.Class.Method.LineNumber;
            if (strLineNumber != "")
            {
                errorSB.AppendLine(strLineNumber);
                errorSB.AppendLine(">");
            }

            string strAttemptedMethod = (this.Class.AttemptedMethod != this.Class.Method.Name && this.Class.AttemptedMethod != "") ? ">	   Attempted Method: " + this.Class.AttemptedMethod : "";
            if (strAttemptedMethod != "")
            {
                errorSB.AppendLine(strAttemptedMethod);
                errorSB.AppendLine(">");
            }

            errorSB.AppendLine(">	      Exception Type: " + this.Exception.GetType().FullName);
            errorSB.AppendLine(">");
            errorSB.AppendLine(">	      Error Message: " + this.Exception.Message);
            errorSB.AppendLine(">");
            errorSB.AppendLine(">	      Local Variable Count: " + this.Class.Method.Parameters.Count.ToString());
            errorSB.AppendLine(">");

            if (this.Class.Method.Parameters.Count > 0)
            {
                int intLength = this.Class.Method.Parameters.Select(p => p.Name.Length).Max();
                foreach (ParameterInformation parameter in this.Class.Method.Parameters)
                {
                    string strLine = ">	         " + parameter.Name.PadRight(intLength + 1, ' ') + " ~ " + parameter.Type;
                    strLine += (this.Class.Method.Parameters.LastIndexOf(parameter) != this.Class.Method.Parameters.Count - 1) ? ", " : "";
                    errorSB.AppendLine(strLine);
                }
            }

            errorSB.AppendLine(">");
            errorSB.AppendLine(">	Namespace: " + this.Assembly.Namespace);
            errorSB.AppendLine(">");
            errorSB.AppendLine(">	Assembly Qualified Name: " + this.Assembly.QualifiedName);
            errorSB.AppendLine("<***************************************************************************************************************>");
            errorSB.AppendLine("_________________________________________________________________________________________________________________");
            errorSB.AppendLine("");

            string strMessage = errorSB.ToString();

            return strMessage;    
        }

        #endregion
    }
}
