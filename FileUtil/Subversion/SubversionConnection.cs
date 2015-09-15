using SharpSvn;

namespace FileUtil.Subversion
{
    /// <summary>
    /// Subversion client
    /// </summary>
    public sealed class SubversionConnection : SvnClient
    {
        #region Description

        // This Class provides Subversion Helper Functions

        #endregion

        #region Properties

        private System.Net.NetworkCredential m_Credential = new System.Net.NetworkCredential();
        /// <summary>
        /// Network credential for connecting to Subversion
        /// </summary>
        public System.Net.NetworkCredential Credential 
        {
            get
            {
                return this.m_Credential;
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="strUsername">Username</param>
        /// <param name="strPassword">User password</param>
        public SubversionConnection(string strUsername, string strPassword)
        {
            this.m_Credential = new System.Net.NetworkCredential(strUsername, strPassword);
            this.Authentication.DefaultCredentials = this.m_Credential;
        }

        #endregion        
    }
}
