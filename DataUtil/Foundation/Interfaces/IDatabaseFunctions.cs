using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.Sql;
using System.Data.SqlClient;
using CoreUtil;

namespace DataUtil
{
    /// <summary>
    /// Core Database Functions Object Interface
    /// </summary>
    public interface IDatabaseFunctions
    {

        #region Connection
        /// <summary>
        /// Retrieve database connection
        /// </summary>
        /// <returns></returns>
        IDbConnection GetConnection();

        /// <summary>
        /// Attempt to connect to the database if not already connected. Returns true if already connected
        /// </summary>
        /// <param name="strError"></param>
        /// <returns></returns>
        Globals.ResultType TryConnect(ref string strError);

        /// <summary>
        /// Attempt to connect to the database
        /// </summary>
        /// <param name="strError"></param>
        /// <returns></returns>
        Globals.ResultType Connect(ref string strError);

        /// <summary>
        /// Attempt to disconnect to the database
        /// </summary>
        /// <param name="strError"></param>
        /// <returns></returns>
        Globals.ResultType Disconnect(ref string strError);

        #endregion

        #region Validation

        #endregion

        #region Get Data

        /// <summary>
        /// Retrieve datatable via a sql query string
        /// </summary>
        /// <param name="strQuery"></param>
        /// <param name="strError"></param>
        /// <param name="intDefaultTimeout"></param>
        /// <returns></returns>
        DataTable GetDataTable(string strQuery, ref string strError, int intDefaultTimeout = 300);

        #endregion

        #region Execute Commands

        /// <summary>
        /// Execute a non-query statement
        /// </summary>
        /// <param name="strQuery"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        int ExecuteNonQuery(string strQuery, ref string strError);

        /// <summary>
        /// Execute a query returning a scalar value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strQuery"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        T ExecuteScalar<T>(string strQuery, ref string strError);

        #endregion

        #region Transaction Functions

        /// <summary>
        /// Begin a sql transaction
        /// </summary>
        /// <param name="strError">Any error encountered</param>
        /// <param name="strTransactionName">Name of the transaction</param>
        /// <returns></returns>
        Globals.ResultType BeginTransaction(ref string strError, string strTransactionName = "");

        /// <summary>
        /// Execute a sql transaction
        /// </summary>
        /// <param name="strError">Any error encountered</param>
        /// <param name="strTransactionName">Name of the transaction</param>
        Globals.ResultType CommitTransaction(ref string strError, string strTransactionName = "");

        /// <summary>
        /// Rollback a sql transaction
        /// </summary>
        /// <param name="strError">Any error encountered</param>
        /// <param name="strTransactionName">Name of the transaction</param>
        Globals.ResultType RollbackTransaction(ref string strError, string strTransactionName = "");

        /// <summary>
        /// Execute a sql transaction
        /// </summary>
        /// <param name="strQuery">Query to execute</param>
        /// <param name="strError">Any error encountered</param>
        Globals.ResultType ExecuteTransaction(string strQuery, ref string strError);

        #endregion

    }
}
