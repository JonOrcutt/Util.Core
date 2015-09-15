using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExceptionUtil
{
    public class AssemblyInformation
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

        private string m_Namespace = "";
        public string Namespace
        {
            get
            {
                return this.m_Namespace;
            }
        }

        private string m_QualifiedName = "";
        public string QualifiedName
        {
            get
            {
                return this.m_QualifiedName;
            }
        }

        private string m_FilePath = "";
        public string FilePath
        {
            get
            {
                return this.m_FilePath;
            }
        }

        private string m_Version = "";
        public string Version
        {
            get
            {
                return this.m_Version;
            }
        }

        #endregion

        #region Initialization

        public AssemblyInformation(System.Reflection.MethodInfo info)
        {
            // Validation
            if (info == null) { return; }

            // Get Assembly Properties
            this.m_FullName = info.ReflectedType.FullName;
            this.m_Namespace = info.ReflectedType.Namespace;
            this.m_QualifiedName = info.ReflectedType.AssemblyQualifiedName;
            this.m_Version = this.QualifiedName.Replace(this.QualifiedName.Substring(0, this.QualifiedName.IndexOf("=") + 1), "");
            this.m_Version = (this.Version.Contains(",") == true) ? this.Version.Substring(0, this.Version.IndexOf(",")) : "";

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

            // Validation
            if (assembly != null && assembly.CodeBase != "")
            {
                this.m_FilePath = "Assembly File Path: " + assembly.CodeBase + "\n";
            }
        }
        
        #endregion
    }
}