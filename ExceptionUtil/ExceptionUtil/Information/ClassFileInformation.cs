using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ExceptionUtil
{
    public class ClassFileInformation
    {
        #region Properties

        private string m_FullName = "";
        public string FullName
        {
            get
            {
                return this.m_FullName;
            }
        }

        private string m_Name = "";
        public string Name
        {
            get
            {
                return this.m_Name;
            }
        }

        private string m_AttemptedMethod = "";
        public string AttemptedMethod
        {
            get
            {
                return this.m_AttemptedMethod;
            }
            internal set
            {
                this.m_AttemptedMethod = value;
            }
        }

        private MethodInformation m_Method = null;
        public MethodInformation Method
        {
            get
            {
                return this.m_Method;
            }
        }
        
        #endregion

        #region Initialization

        public ClassFileInformation(Exception exception, System.Diagnostics.StackFrame stackFrame)
        {
            this.m_Method = new MethodInformation(exception, stackFrame);
            this.LoadClassFileInformation(exception, stackFrame);
        }

        #endregion

        #region Functions

        internal void LoadClassFileInformation(Exception exception, System.Diagnostics.StackFrame stackFrame)
        {
            // Get Reflection Information
            System.Reflection.MethodBase methodBase = stackFrame.GetMethod();
            System.Reflection.MethodBody methodBody = methodBase.GetMethodBody();
            System.Reflection.MethodInfo methodInfo = (methodBase != null && typeof(System.Reflection.MethodInfo).IsAssignableFrom(methodBase.GetType()) == true) ? (System.Reflection.MethodInfo)methodBase : null;
            System.Reflection.MethodInfo targetSiteInfo = ((System.Reflection.MethodInfo)exception.TargetSite);

            // Load Method Properties
            this.Method.LoadMethodProperties(exception, stackFrame);

            this.m_Name = (methodInfo != null && methodInfo.ReflectedType != null) ? methodInfo.ReflectedType.Name : "";
            this.m_FullName = methodBase.ReflectedType.FullName + " - " + this.Method.Name; 
        }

        #endregion
    }
}