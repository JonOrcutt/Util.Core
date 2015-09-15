using System.Collections.Generic;
using System.Security;
using System.Diagnostics;

namespace FileUtil.Processes
{
    /// <summary>
    /// This class contains information for executing a file process
    /// </summary>
    public sealed class ProcessInformation
    {
        #region Properties

        /// <summary>
        /// Process filename
        /// </summary>
        public string FileName = "";

        /// <summary>
        /// Arguments to pass to the application
        /// </summary>
        public string Arguments = "";

        /// <summary>
        /// Username to run the process under
        /// </summary>
        public string Username = "";

        /// <summary>
        /// User password
        /// </summary>
        public SecureString Password = null;

        /// <summary>
        /// List of environment variables
        /// </summary>
        public List<KeyValuePair<string, string>> EnvironmentVariables = new List<KeyValuePair<string, string>>();

        /// <summary>
        /// The style of the console window when executing
        /// </summary>
        public System.Diagnostics.ProcessWindowStyle WindowStyle = ProcessWindowStyle.Normal;

        private bool m_UseShellExecute = false;
        /// <summary>
        /// Flag to use shell execute
        /// </summary>
        public bool UseShellExecute
        {
            get
            {
                if (this.EnvironmentVariables != null && this.EnvironmentVariables.Count > 0)
                {
                    return false;
                }
                else
                {
                    return this.m_UseShellExecute;
                }                
            }
            set
            {
                this.m_UseShellExecute = value;
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Default constructor
        /// </summary>
        public ProcessInformation()
        {

        }

        /// <summary>
        /// Default constructor 
        /// </summary>
        /// <param name="strFileName"></param>
        public ProcessInformation(string strFileName)
        {
            this.FileName = strFileName;
        }
        
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="strFileName"></param>
        /// <param name="listEnvironmentVariables"></param>
        public ProcessInformation(string strFileName, System.Collections.Specialized.StringDictionary listEnvironmentVariables)
        {
            this.FileName = strFileName;
            //this.EnvironmentVariables = listEnvironmentVariables;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="strFileName"></param>
        /// <param name="strArguments"></param>
        public ProcessInformation(string strFileName, string strArguments)
        {
            this.FileName = strFileName;
            this.Arguments = strArguments;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="strFileName"></param>
        /// <param name="strArguments"></param>
        /// <param name="listEnvironmentVariables"></param>
        public ProcessInformation(string strFileName, string strArguments, System.Collections.Specialized.StringDictionary listEnvironmentVariables)
        {
            this.FileName = strFileName;
            this.Arguments = strArguments;
            //this.EnvironmentVariables = listEnvironmentVariables;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="strFileName"></param>
        /// <param name="strUsername"></param>
        /// <param name="strPassword"></param>
        public ProcessInformation(string strFileName, string strUsername, SecureString strPassword)
        {
            this.FileName = strFileName;
            this.Username = strUsername;
            this.Password = strPassword;
        }

        #endregion
    }
}
