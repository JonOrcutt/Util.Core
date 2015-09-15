using System;
using CoreUtil;

namespace DataUtil
{
    /// <summary>
    /// Base class Sybase Server database Connection Information
    /// </summary>
    public sealed class SybaseConnectionInformation : DatabaseConnectionInformation
    {
        #region Properties

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
        public SybaseConnectionInformation(string strConnectionString)
        {
            this.ConnectionString = strConnectionString;
        }

        /// <summary>
        /// Default constructor with server and database names
        /// </summary>
        /// <param name="strServerName">Name of the database server</param>
        /// <param name="strDatabaseName">Name of the database</param>
        public SybaseConnectionInformation(string strServerName, string strDatabaseName)
            : base(strServerName, strDatabaseName)
        {
        }

        /// <summary>
        /// Constructor including username and password
        /// </summary>
        /// <param name="strServerName">Name of the database server</param>
        /// <param name="strDatabaseName">Name of the database</param>
        /// <param name="strUserId">User ID</param>
        /// <param name="strPassword">User password</param>
        public SybaseConnectionInformation(string strServerName, string strDatabaseName, string strUserId, string strPassword)
            : base(strServerName, strDatabaseName, strUserId, strPassword)
        {
        }

        #endregion

        #region Connection String

        /// <summary>
        /// Retrieve the database connection string
        /// </summary>
        /// <returns></returns>
        public override string GetConnectionString()
        {
            string strError = "";

            // Validation
            if (this.DatabaseServer == "" || this.DatabaseName == "") { return ""; }

            string strConnectionString = "";

            string strSybaseOdbcIniFileName = Environment.GetEnvironmentVariable("SYBASE", EnvironmentVariableTarget.Machine);

            strSybaseOdbcIniFileName += "odbcconf.ini";
            strConnectionString = "Driver={" + IniFileFunctions.ReadIniValue("SybaseDriver", "PARM01", "", strSybaseOdbcIniFileName, ref strError) + "};";

            // Validation
            if (strError != "")
            {
                Console.WriteLine(strError);

                return "";
            }

            bool boolLoop = true;
            int intID = 2;

            do
            {
                string t = "PARM" + String.Format("00", intID);
                string strValue = IniFileFunctions.ReadIniValue("SybaseDriver", "PARM" + String.Format("{0:00}", intID), "", strSybaseOdbcIniFileName, ref strError);

                // Validation
                if (strError != "")
                {
                    Console.WriteLine(strError);

                    return "";
                }

                if (strValue.Length == 0)
                {
                    boolLoop = false;
                }
                else
                {
                    strConnectionString += strValue + ";";
                }

                intID++;

            } while (boolLoop);

            strConnectionString += "UID=" + this.UserID + ";PWD=" + this.Password + ";ServerName=" + this.DatabaseServer + ";Database=" + this.DatabaseName + ";";

            return strConnectionString;
        }

        #endregion
    }
}