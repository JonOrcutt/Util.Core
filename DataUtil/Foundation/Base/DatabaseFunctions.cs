using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using CoreUtil;

namespace DataUtil
{
    /// <summary>
    /// Base class for providing Database communication functions
    /// </summary>
    public abstract class DatabaseFunctions : IDatabaseFunctions
    {
        #region Properties
        
        private bool m_IsConnected = false;
        /// <summary>
        /// Flag for whether or not the connection to the database server is open
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return this.m_IsConnected;
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Default constructor
        /// </summary>
        protected DatabaseFunctions()
        {

        }

        #endregion

        #region Connection Functions

        /// <summary>
        /// Retrieve a generic database connection
        /// </summary>
        /// <returns></returns>
        public virtual IDbConnection GetConnection()
        {
            return default(IDbConnection);
        }        

        /// <summary>
        /// Attempt to connect to the database server if not already connected. Returns success if already connected. Method is useful for cases not knowing if already connected to the database server and reducing validation
        /// </summary>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <returns></returns>
        public abstract Globals.ResultType TryConnect(ref string strError);

        /// <summary>
        /// Connect to the database server
        /// </summary>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <returns></returns>
        public abstract Globals.ResultType Connect(ref string strError);

        /// <summary>
        /// Disconnect from the database server
        /// </summary>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <returns></returns>
        public abstract Globals.ResultType Disconnect(ref string strError);

        #endregion
        
        #region Validation

        #endregion

        #region Get Data

        /// <summary>
        /// Query to be executed
        /// </summary>
        /// <param name="strQuery">Query string to execute</param>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <param name="intDefaultTimeout">Optional: Number of seconds before timing out the Sql query</param>
        /// <returns></returns>
        public abstract DataTable GetDataTable(string strQuery, ref string strError, int intDefaultTimeout = 300);
                
        public virtual DataTableInformation GetDataTableInformation(string strTableName, ref string strError)
        {
            try
            {
                // Sanitize Table Name
                strTableName = strTableName.Trim();

                // Validation
                if (strTableName == "") { return null; }

                // Get DataTable Information
                DataTableInformation tableInfo = new DataTableInformation(strTableName, this);

                return tableInfo;
            }
            catch (Exception ex)
            {
                strError = ex.ToString();

                return null;
            }
        }

        #endregion

        #region Execute Commands

        /// <summary>
        /// Execute a non-query command
        /// </summary>
        /// <param name="strQuery">Query to be executed</param>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <returns>An integer representing the number of rows affected by the query command</returns>
        public abstract int ExecuteNonQuery(string strQuery, ref string strError);

        /// <summary>
        /// Execute a query and return a single scalar value of a specified type
        /// </summary>
        /// <typeparam name="T">Type of object to be returned</typeparam>
        /// <param name="strQuery">Query to be executed</param>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <returns></returns>
        public abstract T ExecuteScalar<T>(string strQuery, ref string strError);

        /// <summary>
        /// Returns an IDataReader
        /// </summary>
        /// <param name="strQuery">Query to be executed</param>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <returns></returns>
        public IDataReader GetDataReader(string strQuery, ref string strError)
        {
            // Attempt To Connect
            Globals.ResultType connectResultType = this.TryConnect(ref strError);

            // Validation
            if (connectResultType == Globals.ResultType.Failure || strError != "") { return null; }

            // Get Current Database Connection
            IDbConnection connection = this.GetConnection();

            // Open Database Connection
            connection.Open();

            // Create DataBase Command
            IDbCommand command = connection.CreateCommand();

            // Set Command Text
            command.CommandText = strQuery;            

            // Execute Reader
            IDataReader reader = command.ExecuteReader();

            return reader;
        }
        
        #endregion

        #region Transaction Functions

        /// <summary>
        /// Begin a transaction
        /// </summary>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <param name="strTransactionName">Transaction Name</param>
        /// <returns></returns>
        public virtual Globals.ResultType BeginTransaction(ref string strError, string strTransactionName = "")
        {
            strTransactionName = (strTransactionName.Trim() == "") ? "GENERIC_TRANSACTION" : strTransactionName;
            String strQuery = "BEGIN TRANSACTION " + strTransactionName;

            // Execute Transaction
            Globals.ResultType resultType = this.ExecuteTransaction(strQuery, ref strError);

            return resultType;
        }

        /// <summary>
        /// commit a transaction
        /// </summary>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <param name="strTransactionName">Transaction Name</param>
        /// <returns></returns>
        public virtual Globals.ResultType CommitTransaction(ref string strError, string strTransactionName = "")
        {
            strTransactionName = (strTransactionName.Trim() == "") ? "GENERIC_TRANSACTION" : strTransactionName;
            String strQuery = "COMMIT TRANSACTION " + strTransactionName;

            // Execute Transaction
            Globals.ResultType resultType = this.ExecuteTransaction(strQuery, ref strError);

            return resultType;
        }
        
        /// <summary>
        /// Rollback a transaction
        /// </summary>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <param name="strTransactionName">Transaction Name</param>
        /// <returns></returns>
        public virtual Globals.ResultType RollbackTransaction(ref string strError, string strTransactionName = "")
        {
            strTransactionName = (strTransactionName.Trim() == "") ? "GENERIC_TRANSACTION" : strTransactionName;
            String strQuery = "ROLLBACK TRANSACTION " + strTransactionName;

            // Execute Transaction
            Globals.ResultType resultType = this.ExecuteTransaction(strQuery, ref strError);

            return resultType;
        }

        /// <summary>
        /// Execute/Commit a transaction
        /// </summary>
        /// <param name="strQuery">Query to be executed</param>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <returns></returns>
        public abstract Globals.ResultType ExecuteTransaction(string strQuery, ref string strError);

        #endregion

    }
}
