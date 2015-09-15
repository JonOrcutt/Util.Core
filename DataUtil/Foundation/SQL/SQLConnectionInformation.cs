using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;
using System.Data.Sql;
using System.Data.SqlClient;

namespace DataUtil
{
    /// <summary>
    /// Base class Sql Server database Connection Information
    /// </summary>
    public sealed class SQLConnectionInformation : DatabaseConnectionInformation
    {

        #region Description

        // This is Class holds SQL Database Server Connection Information

        #endregion

        #region Properties

        private AuthenticationType m_Authentication = AuthenticationType.Unknown;
        /// <summary>
        /// Sql server authentication type (Windows/Sql)
        /// </summary>
        private AuthenticationType Authentication
        {
            get
            {
                return this.m_Authentication;
            }
        }

        /// <summary>
        /// Enumeration for Sql authentication type
        /// </summary>
        public enum AuthenticationType : int
        {
            /// <summary>
            /// Type of authentication is Unknown
            /// </summary>
            Unknown = 0,
            /// <summary>
            /// Authenticate as current Windows user
            /// </summary>
            WindowsAuthentication = 1,
            /// <summary>
            /// Authenticate as a Sql server user
            /// </summary>
            SQLAuthentication = 2
        }

        /// <summary>
        /// Database connection string
        /// </summary>
        public override string ConnectionString
        {
            get
            {
                if (this.ConnectionString == "")
                {
                    return this.GetConnectionString();
                }
                else
                {
                    return this.ConnectionString;
                }
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Default constructor including connection string
        /// </summary>
        /// <param name="strConnectionString">Connection string</param>
        public SQLConnectionInformation(string strConnectionString)
        {
            this.ConnectionString = strConnectionString;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="authenticationType">The type of Sql authentication to be used when connecting to the server</param>
        /// <param name="strServerName">The name of the server to connect to</param>
        /// <param name="strDatabaseName">The name of the database</param>
        public SQLConnectionInformation(AuthenticationType authenticationType, string strServerName, string strDatabaseName)
            : base(strServerName, strDatabaseName)
        {
            this.m_Authentication = authenticationType;
        }

        /// <summary>
        /// Default constructor including username and password
        /// </summary>
        /// <param name="authenticationType">The type of Sql authentication to be used when connecting to the server</param>
        /// <param name="strServerName">The name of the server to connect to</param>
        /// <param name="strDatabaseName">The name of the database</param>
        /// <param name="strUserId">User ID</param>
        /// <param name="strPassword">Password</param>
        public SQLConnectionInformation(AuthenticationType authenticationType, string strServerName, string strDatabaseName, string strUserId, string strPassword)
            : base(strServerName, strDatabaseName, strUserId, strPassword)
        {
            this.m_Authentication = authenticationType;
        }

        #endregion

        #region Connection String Functions

        /// <summary>
        /// Retrieve connection string
        /// </summary>
        /// <returns></returns>
        public override string GetConnectionString()
        {
            if (this.DatabaseServer == "" || this.DatabaseName == "") { return ""; }

            string strConnectionString = "";

            switch (this.Authentication)
            {
                case AuthenticationType.WindowsAuthentication:
                    strConnectionString = "Data Source=" + this.DatabaseServer + ";Initial Catalog=" + this.DatabaseName + ";Integrated Security=SSPI;";
                    break;
                case AuthenticationType.SQLAuthentication:
                    strConnectionString = "Server=" + this.DatabaseServer + "; Database=" + this.DatabaseName + "; User Id=" + this.UserID + "; Password=" + this.Password + ";";
                    break;
            }

            return strConnectionString;
        }

        #endregion

    }
}