using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreUtil;

namespace DataUtil
{
    /// <summary>
    /// Base class for database Connection Information
    /// </summary>
    public abstract class DatabaseConnectionInformation : IDatabaseConnectionInformation
    {
        #region Description

        // This is the Base Class for holding Database Server Connection Information

        #endregion

        #region Properties

        private string m_ServerName = "";
        /// <summary>
        /// Name of the database server
        /// </summary>
        public string DatabaseServer
        {
            get
            {
                return this.m_ServerName;
            }
        }

        private string m_DatabaseName = "";
        /// <summary>
        /// Name of the database
        /// </summary>
        public string DatabaseName
        {
            get
            {
                return this.m_DatabaseName;
            }
        }

        private string m_UserID = "";
        /// <summary>
        /// User ID
        /// </summary>
        public string UserID
        {
            get
            {
                return this.m_UserID;
            }
        }

        private string m_Password = "";
        /// <summary>
        /// User password
        /// </summary>
        public string Password
        {
            get
            {
                return this.m_Password;
            }
        }

        private string m_ConnectionString = "";
        /// <summary>
        /// Database server connection string
        /// </summary>
        public virtual string ConnectionString
        {
            get
            {
                return this.m_ConnectionString;
            }
            protected set
            {
                this.m_ConnectionString = value;
            }
        }


        #endregion

        #region Initialization

        /// <summary>
        /// Default constructor
        /// </summary>
        public DatabaseConnectionInformation()
        {

        }

        /// <summary>
        /// Default constructor with server and database names
        /// </summary>
        /// <param name="strServerName">Name of the database server</param>
        /// <param name="strDatabaseName">Name of the database</param>
        public DatabaseConnectionInformation(string strServerName, string strDatabaseName)
        {
            this.m_ServerName = strServerName;
            this.m_DatabaseName = strDatabaseName;
        }

        /// <summary>
        /// Constructor including username and password
        /// </summary>
        /// <param name="strServerName">Name of the database server</param>
        /// <param name="strDatabaseName">Name of the database</param>
        /// <param name="strUserId">User ID</param>
        /// <param name="strPassword">User password</param>
        public DatabaseConnectionInformation(string strServerName, string strDatabaseName, string strUserId, string strPassword)
        {
            this.m_ServerName = strServerName;
            this.m_DatabaseName = strDatabaseName;
            this.m_UserID = strUserId;
            this.m_Password = strPassword;
        }

        #endregion

        #region Functions

        /// <summary>
        /// Connection string for the database server connection
        /// </summary>
        /// <returns></returns>
        public virtual string GetConnectionString()
        {
            return default(string);
        }

        #endregion

    }
}