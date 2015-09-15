using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CoreUtil;

namespace DataUtil
{
    /// <summary>
    /// Class for providing Database Interoperability Functionality
    /// </summary>
    public sealed class DataBaseInteropUtilities
    {

        #region Properties
        
        #region DataBase 

        private string m_TableName = "";

        private string m_Query;

        private bool CreateTableIfNotExists = false;

        private SybaseConnectionInformation m_SybaseConnection;
        private SybaseFunctions m_SybaseFunctions;

        private SQLConnectionInformation m_SqlConnection;
        private SQLFunctions m_SqlFunctions;

        private DataTable m_Data;

        #endregion

        #region Enums

        /// <summary>
        /// Database server type
        /// </summary>
        public enum DatabaseServerType : int
        {
            /// <summary>
            /// Database server type is Unknown
            /// </summary>
            Unknown = 0,
            /// <summary>
            /// Sybase server type
            /// </summary>
            Sybase = 1,
            /// <summary>
            /// Sql server type
            /// </summary>
            Sql = 2
        }
        
        /// <summary>
        /// Database interoperability type
        /// </summary>
        public enum InterOpType : int
        {
            /// <summary>
            /// InterOp type is Unknown
            /// </summary>
            Unknown = 0,
            /// <summary>
            /// Load data from a Sybase server to a Sql server
            /// </summary>
            SybaseToSql = 1,
            /// <summary>
            /// Load data from a Sql sserver to a Sybase server
            /// </summary>
            SqlToSybase = 2
        }

        public enum DataTableLoadType : int
        {
            /// <summary>
            /// DataTable load type is Unknown
            /// </summary>
            Unknown = 0,
            /// <summary>
            /// Load Data information from database
            /// </summary>
            FromDatabase = 1,
            /// <summary>
            /// Load data information from DataTable
            /// </summary>
            FromDataTable = 2
        }

        #endregion

        #endregion

        #region Initialization

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="connInfoA">Connection information</param>
        /// <param name="connInfoB">Connection information</param>
        /// <param name="strTableName">Database table name</param>
        /// <param name="boolCreateTableIfNotExists">Flag for whether to create the database table if it does not exist</param>
        public DataBaseInteropUtilities(DatabaseConnectionInformation connInfoA, DatabaseConnectionInformation connInfoB, string strTableName, bool boolCreateTableIfNotExists = false)
        {
            // Check The Connection Information Type For The First Parameter
            if (connInfoA.GetType() == typeof(SQLConnectionInformation) && typeof(SQLConnectionInformation).IsAssignableFrom(connInfoA.GetType()) == true)
            {
                this.m_SqlConnection = (SQLConnectionInformation)connInfoA;

                // Create SQL Functions
                this.m_SqlFunctions = new SQLFunctions(this.m_SqlConnection);
            }
            else if (connInfoA.GetType() == typeof(SybaseConnectionInformation) && typeof(SybaseConnectionInformation).IsAssignableFrom(connInfoA.GetType()) == true)
            {
                this.m_SybaseConnection = (SybaseConnectionInformation)connInfoA;

                // Create Sybase Functions
                this.m_SybaseFunctions = new SybaseFunctions(this.m_SybaseConnection);
            }

            // Check The Connection Information Type For The Second Parameter
            if (connInfoB.GetType() == typeof(SQLConnectionInformation) && typeof(SQLConnectionInformation).IsAssignableFrom(connInfoB.GetType()) == true)
            {
                this.m_SqlConnection = (SQLConnectionInformation)connInfoB;

                // Create SQL Functions
                this.m_SqlFunctions = new SQLFunctions(this.m_SqlConnection);
            }
            else if (connInfoB.GetType() == typeof(SybaseConnectionInformation) && typeof(SybaseConnectionInformation).IsAssignableFrom(connInfoB.GetType()) == true)
            {
                this.m_SybaseConnection = (SybaseConnectionInformation)connInfoB;

                // Create Sybase Functions
                this.m_SybaseFunctions = new SybaseFunctions(this.m_SybaseConnection);
            }

            this.m_TableName = strTableName;
            this.CreateTableIfNotExists = boolCreateTableIfNotExists;
        }

        #endregion

        #region Copy Data Functions

        /// <summary>
        /// Copy data from one database server type to another
        /// </summary>
        /// <param name="strQuery"></param>
        /// <param name="interOpType"></param>
        /// <param name="dataTableLoadType"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        public Globals.ResultType CopyData(string strQuery, InterOpType interOpType, DataTableLoadType dataTableLoadType, ref string strError)
        {
            try
            {
                // Validation
                if (strQuery == "") { return Globals.ResultType.Failure; }

                this.m_Query = strQuery;
                Globals.ResultType checkCreateResult = Globals.ResultType.Unknown;
                Globals.ResultType loadDataResult = Globals.ResultType.Unknown;

                // Validation
                if (this.m_SybaseFunctions == null || this.m_SqlFunctions == null) { return Globals.ResultType.Failure; }

                switch (interOpType)
                {
                    case InterOpType.SybaseToSql:

                        // Check Create Table
                        checkCreateResult = this.CheckCreateTable(this.m_SybaseFunctions, this.m_SqlFunctions, dataTableLoadType, ref strError);

                        // Validation
                        if (checkCreateResult == Globals.ResultType.Failure || strError != "") { return Globals.ResultType.Failure; }

                        // Load Data Into Table
                        loadDataResult = this.LoadDataIntoTable(this.m_SybaseFunctions, this.m_SqlFunctions, DatabaseServerType.Sql, ref strError);
                        break;
                    case InterOpType.SqlToSybase:

                        // Check Create Table
                        checkCreateResult = this.CheckCreateTable(this.m_SqlFunctions, this.m_SybaseFunctions, dataTableLoadType, ref strError);

                        // Validation
                        if (checkCreateResult == Globals.ResultType.Failure || strError != "") { return Globals.ResultType.Failure; }

                        // Load Data Into Table
                        loadDataResult = this.LoadDataIntoTable(this.m_SqlFunctions, this.m_SybaseFunctions, DatabaseServerType.Sybase, ref strError);
                        break;
                }

                // Validation
                if (loadDataResult == Globals.ResultType.Failure || strError != "") { return Globals.ResultType.Failure; }

                return Globals.ResultType.Success;
            }
            catch (Exception ex)
            {
                strError = ex.ToString();

                return Globals.ResultType.Failure;
            }
        }
             
        #endregion

        #region Create Table

        /// <summary>
        /// Check whether or not a table can be created
        /// </summary>
        /// <param name="functionsFrom"></param>
        /// <param name="functionsTo"></param>
        /// <param name="createAndLoadType"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        private Globals.ResultType CheckCreateTable(IDatabaseFunctions functionsFrom, IDatabaseFunctions functionsTo, DataTableLoadType createAndLoadType, ref string strError)
        {
            // Check Create Table
            if (this.CreateTableIfNotExists == true)
            {
                // Attempt Create Table (Will Return If Already Exists)
                Globals.ResultType createTableResult = CreateDataBaseTable(functionsFrom, functionsTo, createAndLoadType, ref strError);

                // Validation 
                if (createTableResult == Globals.ResultType.Failure || strError != "") { return Globals.ResultType.Failure; }
            }

            return Globals.ResultType.Success;
        }

        private Globals.ResultType CreateDataBaseTable(IDatabaseFunctions functionsFrom, IDatabaseFunctions functionsTo, DataTableLoadType createAndLoadType, ref string strError)
        {
            try
            {
                // Create New Table Info To Object
                DataTableInformation tableInfoTo = new DataTableInformation(this.m_TableName, functionsTo);

                // Validation
                if (tableInfoTo.TableExists == true) { return Globals.ResultType.Success; }
                
                // Create New Table Info From Object
                DataTableInformation tableInfoFrom = new DataTableInformation(this.m_TableName, functionsFrom);

                // Validation
                if (tableInfoFrom.TableExists == false) { return Globals.ResultType.Failure; }
                
                string strCreateTableQuery = "";

                // Check Create Table Type
                switch (createAndLoadType)
                {
                    case DataTableLoadType.FromDatabase:

                        // Get Create Table Script
                        strCreateTableQuery = tableInfoFrom.GetCreateTableQuery(ref strError);
                        
                        break;
                    case DataTableLoadType.FromDataTable:
                        
                        //// Get DataTable
                        //this.m_Data = functionsFrom.GetDataTable(this.m_Query, ref strError);
                        
                        //// Validation
                        //if (this.m_Data == null || strError != "") { return Globals.ResultType.Failure; }

                        // Reset Table Info From With Retrieved Data Table
                        tableInfoFrom = new DataTableInformation(this.m_TableName, functionsFrom);

                        // Get Create Table Script
                        strCreateTableQuery = tableInfoFrom.GetCreateTableQuery(ref strError);

                        break;
                }

                // Validation 
                if (strError != "") { return Globals.ResultType.Failure; }
                                                
                // Execute Create Table Script
                functionsTo.ExecuteNonQuery(strCreateTableQuery, ref strError);

                // Validation
                if (strError != "") { return Globals.ResultType.Failure; }
                                               
                return Globals.ResultType.Success;
            }
            catch (Exception ex)
            {
                strError = ex.ToString();

                return Globals.ResultType.Failure;
            }        
        }
        
        #endregion

        #region Load Data
        
        private Globals.ResultType LoadDataIntoTable(IDatabaseFunctions functionsFrom, IDatabaseFunctions functionsTo, DatabaseServerType serverType, ref string strError)
        {
            try
            {
                Globals.ResultType insertAllRowsResultType = Globals.ResultType.Success;

                // Get DataTable
                this.m_Data = functionsFrom.GetDataTable(this.m_Query, ref strError);

                // Validation
                if (this.m_Data == null || strError != "") { return Globals.ResultType.Failure; }

                // Create New Table Info To Object
                DataTableInformation tableInfoTo = new DataTableInformation(this.m_TableName, functionsTo);

                // Validation
                if (tableInfoTo.TableExists == false) { return Globals.ResultType.Success; }

                // Create New Table Info To Object
                DataTableInformation tableInfoFrom = new DataTableInformation(this.m_TableName, functionsFrom);

                // Validation
                if (tableInfoFrom.TableExists == false) { return Globals.ResultType.Failure; }

                int intAllRowsAffected = 0;
                int intAllRowsFailed = 0;
                int intCurrentRowCount = 0;

                // Clone Data To Temporary Table
                DataTable dtTemp = this.m_Data.Clone();

                List<string> listInsertQueries = new List<string>();
                Globals.ResultType insertDataResultType = Globals.ResultType.Success;

                // Execute DataReader
                IDataReader reader = ((SybaseFunctions)functionsFrom).ExecuteReader("", ref strError);
                
                // If DataRow Count Exceeds 50,000 - Load Data Incrementally
                if (this.m_Data.Rows.Count > 50000)
                {
                    do
                    {
                        // Clear Queries And Temp Table
                        listInsertQueries.Clear();
                        dtTemp.Clear();

                        // Loop Rows And Import Into Temp Table
                        for (int intIndex = intCurrentRowCount; intIndex < intCurrentRowCount + 50000; intIndex++)
                        {
                            // Import Row
                            dtTemp.ImportRow(this.m_Data.Rows[intIndex]);
                        }

                        // Increment Current Row COunt
                        intCurrentRowCount += 50000;

                        // Get Insert Query List
                        listInsertQueries = tableInfoFrom.GetSqlInsertQueryList(dtTemp, ref strError);

                        // Validation
                        if (listInsertQueries == null || strError != "") { return Globals.ResultType.Failure; }

                        // Insert Data Into Table
                        insertDataResultType = this.InsertDataIntoTable(listInsertQueries, functionsTo, ref strError, ref intAllRowsAffected, ref intAllRowsFailed);
                    }
                    while (this.m_Data.Rows.Count - intCurrentRowCount > 50000);

                    dtTemp.Clear();

                    // Loop Rows
                    for (int intIndex = intCurrentRowCount; intIndex < this.m_Data.Rows.Count; intIndex++)
                    {
                        // Import Remaining Rows
                        dtTemp.ImportRow(this.m_Data.Rows[intIndex]);
                    }

                    // Get Insert Query List
                    listInsertQueries = tableInfoFrom.GetSqlInsertQueryList(dtTemp, ref strError);

                    // Validation
                    if (listInsertQueries == null || strError != "") { return Globals.ResultType.Failure; }

                    // Insert Data Into Table
                    insertDataResultType = this.InsertDataIntoTable(listInsertQueries, functionsTo, ref strError, ref intAllRowsAffected, ref intAllRowsFailed);
                }
                else
                {
                    // Get Insert Query List
                    listInsertQueries = tableInfoFrom.GetSqlInsertQueryList(this.m_Data, ref strError);

                    // Validation
                    if (listInsertQueries == null || strError != "") { return Globals.ResultType.Failure; }

                    // Insert Data Into Table
                    insertDataResultType = this.InsertDataIntoTable(listInsertQueries, functionsTo, ref strError, ref intAllRowsAffected, ref intAllRowsFailed);
                }
                
                //// Get Insert Query List
                //List<string> listInsertQueries = tableInfoFrom.GetSqlInsertQueryList(this.m_Data, ref strError);

                //// Validation
                //if (listInsertQueries == null || strError != "") { return Globals.ResultType.Failure; }

                //// Insert Data Into Table
                //Globals.ResultType insertDataResultType = this.InsertDataIntoTable(listInsertQueries, functionsTo, ref strError, ref intAllRowsAffected, ref intAllRowsFalied);

                return insertAllRowsResultType;
            }
            catch (Exception ex)
            {
                strError = ex.ToString();

                return Globals.ResultType.Failure;
            }
        }

        private Globals.ResultType InsertDataIntoTable(List<string> listInsertQueries, IDatabaseFunctions functions, ref string strError, ref int intAllRowsAffected, ref int intAllRowsFalied)
        {
            Globals.ResultType insertAllRowsResultType = Globals.ResultType.Success;

            // Loop Sql Commands
            foreach (string strInsertQuery in listInsertQueries)
            {
                try
                {
                    // Execute Non Query
                    int intRowsAffected = functions.ExecuteNonQuery(strInsertQuery, ref strError);

                    // Validation
                    if (intRowsAffected < 1)
                    {
                        intAllRowsFalied += 1;

                        strError = "Error: Row Not Inserted.";

                        insertAllRowsResultType = Globals.ResultType.Failure;
                    }
                    else
                    {
                        intAllRowsAffected += intRowsAffected;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            return insertAllRowsResultType;
        }
        
        #endregion

        #region Other Functions

        #endregion


        #region Commented Out Code


        ///// <summary>
        ///// Default constructor
        ///// </summary>
        ///// <param name="sybaseInfo">Sybase connection information</param>
        ///// <param name="sqlInfo">Sql connection information</param>
        ///// <param name="strTableName">Database table name</param>
        ///// <param name="boolCreateTableIfNotExists">Flag for whether to create the database table if it does not exist</param>
        //public DataBaseInteropUtilities(SybaseConnectionInformation sybaseInfo, SQLConnectionInformation sqlInfo, string strTableName, bool boolCreateTableIfNotExists = false)
        //{
        //    this.m_SybaseConnection = sybaseInfo;
        //    this.m_SqlConnection = sqlInfo;
        //    this.m_TableName = strTableName;
        //    this.CreateTableIfNotExists = boolCreateTableIfNotExists;

        //    // Create Sybase Functions
        //    this.m_SybaseFunctions = new SybaseFunctions(this.m_SybaseConnection);

        //    // Create SQL Functions
        //    this.m_SqlFunctions = new SQLFunctions(this.m_SqlConnection);
        //}


        //switch (serverType)
        //{
        //    #region SQL

        //    case DatabaseServerType.Sql:

        //// Get Sql Command List
        //List<string> listSqlCommands = tableInfoFrom.GetSqlInsertQueryList(this.m_Data, ref strError);

        //// Validation
        //if (listSqlCommands == null || strError != "") { return Globals.ResultType.Failure; }

        //this.InsertDataIntoTable(listSqlCommands, functionsTo, ref strError, ref intAllRowsAffected, ref intAllRowsFalied);

        //// Create Sql Command
        //System.Data.SqlClient.SqlCommand cmdSql = new System.Data.SqlClient.SqlCommand();
        //cmdSql.Connection = this.m_SqlFunctions.Connection;

        //// Loop Sql Commands
        //foreach (string strInsertQuery in listSqlCommands)
        //{
        //    int intRowsAffected = 0;

        //    // Set Command Connection Property
        //    cmdSql.CommandText = strInsertQuery;

        //    try
        //    {
        //        // Execute Non Query
        //        intRowsAffected = ((SQLFunctions)functionsTo).ExecuteNonQuery(cmdSql, ref strError);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.ToString());
        //    }

        //    // Validation
        //    if (intRowsAffected < 1)
        //    {
        //        intFailureCount += 1;

        //        strError = "Error: Row Not Inserted.";

        //        insertAllRowsResultType = Globals.ResultType.Failure;
        //    }
        //}

        //    break;

        //#endregion

        //#region SYBASE

        //case DatabaseServerType.Sybase:

        //// Get Sql Command List
        //List<string> listSybaseCommands = tableInfoFrom.GetSqlInsertQueryList(this.m_Data, ref strError);

        //// Validation
        //if (listSybaseCommands == null || strError != "") { return Globals.ResultType.Failure; }

        //System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand();

        //// Set Command Connection Property
        //cmd.Connection = this.m_SybaseFunctions.Connection;

        //// Loop Sql Commands
        //foreach (string strInsertQuery in listSybaseCommands)
        //{  
        //    int intRowsAffected = 0;

        //    try
        //    {
        //        // Execute Non Query
        //        intRowsAffected = ((SybaseFunctions)functionsTo).ExecuteNonQuery(cmd, ref strError);

        //        // Validation
        //        if (intRowsAffected < 1)
        //        {
        //            intFailureCount += 1;

        //            strError = "Error: Row Not Inserted.";

        //            insertAllRowsResultType = Globals.ResultType.Failure;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.ToString());
        //    }
        //}

        //    break;

        //#endregion
        //}

        //private Globals.ResultType LoadDataIntoTableOLD(IDatabaseFunctions functionsFrom, IDatabaseFunctions functionsTo, DatabaseServerType serverType, ref string strError)
        //{
        //    try
        //    {
        //        Globals.ResultType insertAllRowsResultType = Globals.ResultType.Success;

        //        // Get DataTable
        //        DataTable dt = functionsFrom.GetDataTable(this.m_Query, ref strError);

        //        // Validation
        //        if (dt == null || strError != "") { return Globals.ResultType.Failure; }

        //        // Create New Table Info To Object
        //        DataBaseTableInformation tableInfoTo = new DataBaseTableInformation(this.m_TableName, functionsTo);

        //        // Validation
        //        if (tableInfoTo.TableExists == false) { return Globals.ResultType.Success; }

        //        // Create New Table Info To Object
        //        DataBaseTableInformation tableInfoFrom = new DataBaseTableInformation(this.m_TableName, functionsFrom);

        //        // Validation
        //        if (tableInfoFrom.TableExists == false) { return Globals.ResultType.Failure; }

        //        switch (serverType)
        //        {
        //            case DatabaseServerType.Sql:

        //                // Get Sql Command List
        //                List<System.Data.SqlClient.SqlCommand> listSqlCommands = tableInfoFrom.GetSqlInsertQueryCommandList(dt, ref strError);

        //                // Validation
        //                if (listSqlCommands == null || strError != "") { return Globals.ResultType.Failure; }

        //                // Loop Sql Commands
        //                foreach (System.Data.SqlClient.SqlCommand cmd in listSqlCommands)
        //                {
        //                    // Set Command Connection Property
        //                    cmd.Connection = this.m_SqlFunctions.Connection;

        //                    // Execute Non Query
        //                    int intRowsAffected = ((SQLFunctions)functionsTo).ExecuteNonQuery(cmd, ref strError);

        //                    // Validation
        //                    if (intRowsAffected < 1)
        //                    {
        //                        strError = "Error: Row Not Inserted.";

        //                        insertAllRowsResultType = Globals.ResultType.Failure;
        //                    }
        //                }

        //                break;
        //            case DatabaseServerType.Sybase:

        //                // Get Sql Command List
        //                List<System.Data.Odbc.OdbcCommand> listSybaseCommands = tableInfoFrom.GetSybaseInsertQueryCommandList(dt, ref strError);

        //                // Validation
        //                if (listSybaseCommands == null || strError != "") { return Globals.ResultType.Failure; }

        //                int intFailureCount = 0;

        //                // Loop Sql Commands
        //                foreach (System.Data.Odbc.OdbcCommand cmd in listSybaseCommands)
        //                {
        //                    // Set Command Connection Property
        //                    cmd.Connection = this.m_SybaseFunctions.Connection;

        //                    int intRowsAffected = 0;

        //                    try
        //                    {
        //                        // Execute Non Query
        //                        intRowsAffected = ((SybaseFunctions)functionsTo).ExecuteNonQuery(cmd, ref strError);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        intFailureCount += 1;
        //                    }

        //                    // Validation
        //                    if (intRowsAffected < 1)
        //                    {
        //                        strError = "Error: Row Not Inserted.";

        //                        insertAllRowsResultType = Globals.ResultType.Failure;
        //                    }
        //                }

        //                break;
        //        }

        //        return insertAllRowsResultType;
        //    }
        //    catch (Exception ex)
        //    {
        //        strError = ex.ToString();

        //        return Globals.ResultType.Failure;
        //    }
        //}


        //public static Type GetTypeFromSqlType(SqlDbType sqltype)
        //{
        //    Type resultType = null;

        //    // Create Type Dictionary
        //    Dictionary<SqlDbType, Type> Types = new Dictionary<SqlDbType, Type>();
        //    Types.Add(SqlDbType.BigInt, typeof(Int64));
        //    Types.Add(SqlDbType.Binary, typeof(Byte[]));
        //    Types.Add(SqlDbType.Bit, typeof(Boolean));
        //    Types.Add(SqlDbType.Char, typeof(String));
        //    //Types.Add(SqlDbType.Date, typeof(DateTime));
        //    Types.Add(SqlDbType.DateTime, typeof(DateTime));
        //    Types.Add(SqlDbType.DateTime2, typeof(DateTime));
        //    Types.Add(SqlDbType.DateTimeOffset, typeof(DateTimeOffset));
        //    Types.Add(SqlDbType.Decimal, typeof(Decimal));
        //    Types.Add(SqlDbType.Float, typeof(Double));
        //    Types.Add(SqlDbType.Image, typeof(Byte[]));
        //    Types.Add(SqlDbType.Int, typeof(Int32));
        //    Types.Add(SqlDbType.Money, typeof(Decimal));
        //    Types.Add(SqlDbType.NChar, typeof(String));
        //    Types.Add(SqlDbType.NText, typeof(String));
        //    Types.Add(SqlDbType.NVarChar, typeof(String));
        //    Types.Add(SqlDbType.Real, typeof(Single));
        //    Types.Add(SqlDbType.SmallDateTime, typeof(DateTime));
        //    Types.Add(SqlDbType.SmallInt, typeof(Int16));
        //    Types.Add(SqlDbType.SmallMoney, typeof(Decimal));
        //    Types.Add(SqlDbType.Text, typeof(String));
        //    Types.Add(SqlDbType.Time, typeof(TimeSpan));
        //    Types.Add(SqlDbType.Timestamp, typeof(Byte[]));
        //    Types.Add(SqlDbType.TinyInt, typeof(Byte));
        //    Types.Add(SqlDbType.UniqueIdentifier, typeof(Guid));
        //    Types.Add(SqlDbType.VarBinary, typeof(Byte[]));
        //    Types.Add(SqlDbType.VarChar, typeof(String));

        //    // Get Type Value
        //    Types.TryGetValue(sqltype, out resultType);

        //    return resultType;
        //}

        //public static SqlDbType GetSqlTypeFromType(Type type)
        //{
        //    SqlDbType resultType = SqlDbType.Char;
            
        //    // Create Type Dictionary
        //    Dictionary<SqlDbType, Type> Types = new Dictionary<SqlDbType, Type>();
        //    Types.Add(SqlDbType.BigInt, typeof(Int64));
        //    Types.Add(SqlDbType.Binary, typeof(Byte[]));
        //    Types.Add(SqlDbType.Bit, typeof(Boolean));
        //    Types.Add(SqlDbType.Char, typeof(String));
        //    //Types.Add(SqlDbType.Date, typeof(DateTime));
        //    Types.Add(SqlDbType.DateTime, typeof(DateTime));
        //    Types.Add(SqlDbType.DateTime2, typeof(DateTime));
        //    Types.Add(SqlDbType.DateTimeOffset, typeof(DateTimeOffset));
        //    Types.Add(SqlDbType.Decimal, typeof(Decimal));
        //    Types.Add(SqlDbType.Float, typeof(Double));
        //    Types.Add(SqlDbType.Image, typeof(Byte[]));
        //    Types.Add(SqlDbType.Int, typeof(Int32));
        //    Types.Add(SqlDbType.Money, typeof(Decimal));
        //    Types.Add(SqlDbType.NChar, typeof(String));
        //    Types.Add(SqlDbType.NText, typeof(String));
        //    Types.Add(SqlDbType.NVarChar, typeof(String));
        //    Types.Add(SqlDbType.Real, typeof(Single));
        //    Types.Add(SqlDbType.SmallDateTime, typeof(DateTime));
        //    Types.Add(SqlDbType.SmallInt, typeof(Int16));
        //    Types.Add(SqlDbType.SmallMoney, typeof(Decimal));
        //    Types.Add(SqlDbType.Text, typeof(String));
        //    Types.Add(SqlDbType.Time, typeof(TimeSpan));
        //    Types.Add(SqlDbType.Timestamp, typeof(Byte[]));
        //    Types.Add(SqlDbType.TinyInt, typeof(Byte));
        //    Types.Add(SqlDbType.UniqueIdentifier, typeof(Guid));
        //    Types.Add(SqlDbType.VarBinary, typeof(Byte[]));
        //    Types.Add(SqlDbType.VarChar, typeof(String));
            
        //    // Get Type Value
        //    resultType = Types.Where(pair => pair.Value == type).Select(pair => pair.Key).FirstOrDefault();

        //    return resultType;
        //}

        //public static SqlDbType GetSqlTypeFromString(string strType)
        //{
        //    SqlDbType resultType = SqlDbType.Char;

        //    // Create Type Dictionary
        //    Dictionary<SqlDbType, Type> Types = new Dictionary<SqlDbType, Type>();
        //    Types.Add(SqlDbType.BigInt, typeof(Int64));
        //    Types.Add(SqlDbType.Binary, typeof(Byte[]));
        //    Types.Add(SqlDbType.Bit, typeof(Boolean));
        //    Types.Add(SqlDbType.Char, typeof(String));
        //    //Types.Add(SqlDbType.Date, typeof(DateTime));
        //    Types.Add(SqlDbType.DateTime, typeof(DateTime));
        //    Types.Add(SqlDbType.DateTime2, typeof(DateTime));
        //    Types.Add(SqlDbType.DateTimeOffset, typeof(DateTimeOffset));
        //    Types.Add(SqlDbType.Decimal, typeof(Decimal));
        //    Types.Add(SqlDbType.Float, typeof(Double));
        //    Types.Add(SqlDbType.Image, typeof(Byte[]));
        //    Types.Add(SqlDbType.Int, typeof(Int32));
        //    Types.Add(SqlDbType.Money, typeof(Decimal));
        //    Types.Add(SqlDbType.NChar, typeof(String));
        //    Types.Add(SqlDbType.NText, typeof(String));
        //    Types.Add(SqlDbType.NVarChar, typeof(String));
        //    Types.Add(SqlDbType.Real, typeof(Single));
        //    Types.Add(SqlDbType.SmallDateTime, typeof(DateTime));
        //    Types.Add(SqlDbType.SmallInt, typeof(Int16));
        //    Types.Add(SqlDbType.SmallMoney, typeof(Decimal));
        //    Types.Add(SqlDbType.Text, typeof(String));
        //    Types.Add(SqlDbType.Time, typeof(TimeSpan));
        //    Types.Add(SqlDbType.Timestamp, typeof(Byte[]));
        //    Types.Add(SqlDbType.TinyInt, typeof(Byte));
        //    Types.Add(SqlDbType.UniqueIdentifier, typeof(Guid));
        //    Types.Add(SqlDbType.VarBinary, typeof(Byte[]));
        //    Types.Add(SqlDbType.VarChar, typeof(String));

        //    // Get Type Value
        //    resultType = Types.Where(pair => pair.Key.ToString().ToLower() == strType.ToLower()).Select(pair => pair.Key).FirstOrDefault();

        //    return resultType;
        //}
        
//public Globals.ResultType CopyDataFromSybaseToSql(string strQuery, DataTableCreateAndLoadType createAndLoadType, ref string strError)
        //{
        //    try
        //    {
        //        // Validation
        //        if (strQuery == "") { return Globals.ResultType.Failure; }

        //        this.m_Query = strQuery;

        //        // Check Create Table
        //        Globals.ResultType checkCreateResult = this.CheckCreateTable(this.m_SybaseFunctions, this.m_SqlFunctions, createAndLoadType, ref strError);

        //        // Validation
        //        if (checkCreateResult == Globals.ResultType.Failure || strError != "") { return Globals.ResultType.Failure; }

        //        // Load Data Into Table
        //        Globals.ResultType loadDataResult = this.LoadDataIntoTable(this.m_SybaseFunctions, this.m_SqlFunctions, DatabaseServerType.Sql, ref strError);

        //        // Validation
        //        if (loadDataResult == Globals.ResultType.Failure || strError != "") { return Globals.ResultType.Failure; }

        //        return Globals.ResultType.Success;
        //    }
        //    catch (Exception ex)
        //    {
        //        strError = ex.ToString();

        //        return Globals.ResultType.Failure;
        //    }
        //}

        //public Globals.ResultType CopyDataFromSqlToSybase(string strQuery, DataTableCreateAndLoadType createAndLoadType, ref string strError)
        //{
        //    try
        //    {
        //        // Validation
        //        if (strQuery == "") { return Globals.ResultType.Failure; }

        //        this.m_Query = strQuery;

        //        // Check Create Table
        //        Globals.ResultType checkCreateResult = this.CheckCreateTable(this.m_SqlFunctions, this.m_SybaseFunctions, createAndLoadType, ref strError);

        //        // Validation
        //        if (checkCreateResult == Globals.ResultType.Failure || strError != "") { return Globals.ResultType.Failure; }

        //        // Load Data Into Table
        //        Globals.ResultType loadDataResult = this.LoadDataIntoTable(this.m_SybaseFunctions, this.m_SqlFunctions, DatabaseServerType.Sybase, ref strError);

        //        // Validation
        //        if (loadDataResult == Globals.ResultType.Failure || strError != "") { return Globals.ResultType.Failure; }

        //        return Globals.ResultType.Success;
        //    }
        //    catch (Exception ex)
        //    {
        //        strError = ex.ToString();

        //        return Globals.ResultType.Failure;
        //    }
        //}

        //private System.Data.SqlClient.SqlCommand GetSqlInsertQueryCommand(DataRow dr, ref string strError)
        //{
        //    string strInsertQuery = "INSERT INTO " + this.m_TableName + " (" + "\r\n";

        //    // Sanitize Row Values Into New List
        //    List<string> listValues = dr.ItemArray.Select(value => (value != null && value != DBNull.Value) ? value.ToString() : "").ToList();

        //    //Create Sql Command
        //    System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
            
        //    // Loop Columns
        //    foreach (DataColumn column in dr.Table.Columns)
        //    {
        //        strInsertQuery += "\t" + column.ColumnName;

        //        if (dr.Table.Columns.IndexOf(column) < dr.Table.Columns.Count - 1)
        //        {
        //            strInsertQuery += ",";
        //        }

        //        strInsertQuery += "\r\n";
        //    }

        //    strInsertQuery += ")" + "\r\n" + " VALUES " + "\r\n" + "(" + "\r\n";

        //    // Loop Columns
        //    foreach (DataColumn column in dr.Table.Columns)
        //    {
        //        strInsertQuery += "@" + column.ColumnName;

        //        // Add Trailing Comma
        //        if (dr.Table.Columns.IndexOf(column) < dr.Table.Columns.Count - 1)
        //        {
        //            strInsertQuery += ", " + "\r\n";
        //        }

        //        string strValue = dr[column].ToString();

        //        // Add Parameters And Values
        //        cmd.Parameters.AddWithValue("@" + column.ColumnName, strValue);
        //    }

        //    strInsertQuery += "\r\n" + ")";

        //    cmd.CommandText = strInsertQuery;
        //    cmd.Connection = this.m_SqlFunctions.Connection;

        //    return cmd;
        //}

        //private List<System.Data.SqlClient.SqlCommand> GetSqlInsertQueryCommandList(DataTable dt, ref string strError)
        //{
        //    List<System.Data.SqlClient.SqlCommand> listCommands = new List<System.Data.SqlClient.SqlCommand>();

        //    // Loop DataRows
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        // Get Sql Command
        //        System.Data.SqlClient.SqlCommand cmd = this.GetSqlInsertQueryCommand(dr, ref strError);

        //        // Validation
        //        if (cmd == null || strError != "") { return null; }

        //        listCommands.Add(cmd);
        //    }

        //    return listCommands;
        //}

        //private System.Data.Odbc.OdbcCommand GetSybaseInsertQueryCommand(DataRow dr, ref string strError)
        //{
        //    string strInsertQuery = "INSERT INTO " + this.m_TableName + " (" + "\r\n";

        //    // Sanitize Row Values Into New List
        //    List<string> listValues = dr.ItemArray.Select(value => (value != null && value != DBNull.Value) ? value.ToString() : "").ToList();

        //    //Create Sql Command
        //    System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand();
            
        //    // Loop Columns
        //    foreach (DataColumn column in dr.Table.Columns)
        //    {
        //        strInsertQuery += "\t" + column.ColumnName;

        //        if (dr.Table.Columns.IndexOf(column) < dr.Table.Columns.Count - 1)
        //        {
        //            strInsertQuery += ",";
        //        }

        //        strInsertQuery += "\r\n";
        //    }

        //    strInsertQuery += ")" + "\r\n" + " VALUES " + "\r\n" + "(" + "\r\n";

        //    // Loop Columns
        //    foreach (DataColumn column in dr.Table.Columns)
        //    {
        //        strInsertQuery += "@" + column.ColumnName;

        //        // Add Trailing Comma
        //        if (dr.Table.Columns.IndexOf(column) < dr.Table.Columns.Count - 1)
        //        {
        //            strInsertQuery += ", " + "\r\n";
        //        }

        //        string strValue = dr[column].ToString();

        //        // Add Parameters And Values
        //        cmd.Parameters.AddWithValue("@" + column.ColumnName, strValue);
        //    }

        //    strInsertQuery += "\r\n" + ")";

        //    cmd.CommandText = strInsertQuery;
        //    cmd.Connection = this.m_SybaseFunctions.Connection;

        //    return cmd;
        //}

        //private List<System.Data.Odbc.OdbcCommand> GetSybaseInsertQueryCommandList(DataTable dt, ref string strError)
        //{
        //    List<System.Data.Odbc.OdbcCommand> listCommands = new List<System.Data.Odbc.OdbcCommand>();

        //    // Loop DataRows
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        // Get Sql Command
        //        System.Data.Odbc.OdbcCommand cmd = this.GetSybaseInsertQueryCommand(dr, ref strError);

        //        // Validation
        //        if (cmd == null || strError != "") { return null; }

        //        listCommands.Add(cmd);
        //    }

        //    return listCommands;
        //}

        //private Globals.ResultType LoadDataIntoSybaseTable(ref string strError)
        //{
        //    try
        //    {
        //        // Get DataTable
        //        DataTable dt = this.m_SqlFunctions.GetDataTable(this.m_Query, ref strError);

        //        // Validation
        //        if (dt == null || strError != "") { return Globals.ResultType.Failure; }

        //        Globals.ResultType insertAllRowsResultType = Globals.ResultType.Success;

        //        // Get Sql Command List
        //        List<System.Data.Odbc.OdbcCommand> listCommands = this.GetSybaseInsertQueryCommandList(dt, ref strError);

        //        // Validation
        //        if (listCommands == null || strError != "") { return Globals.ResultType.Failure; }

        //        // Loop Sql Commands
        //        foreach (System.Data.Odbc.OdbcCommand cmd in listCommands)
        //        {
        //            // Execute Non Query
        //            int intRowsAffected = this.m_SybaseFunctions.ExecuteNonQuery(cmd, ref strError);

        //            // Validation
        //            if (intRowsAffected < 1)
        //            {
        //                strError = "Error: Row Not Inserted.";

        //                insertAllRowsResultType = Globals.ResultType.Failure;
        //            }
        //        }

        //        return insertAllRowsResultType;

        //        #region Commented Out Code

        //        //// Loop DataRows
        //        //foreach (DataRow dr in dt.Rows)
        //        //{
        //        //    string strInsertQuery = "INSERT INTO " + this.m_TableName + " (" + "\r\n";

        //        //    // Sanitize Row Values Into New List
        //        //    List<string> listValues = dr.ItemArray.Select(value => (value != null && value != DBNull.Value) ? value.ToString() : "").ToList();

        //        //    //Create Sql COmmand
        //        //    System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();

        //        //    // Loop Columns
        //        //    foreach (DataColumn column in dt.Columns)
        //        //    {
        //        //        strInsertQuery += "\t" + column.ColumnName;

        //        //        if (dt.Columns.IndexOf(column) < dt.Columns.Count - 1)
        //        //        {
        //        //            strInsertQuery += ",";
        //        //        }

        //        //        strInsertQuery += "\r\n";
        //        //    }

        //        //    strInsertQuery += ")" + "\r\n" + " VALUES " + "\r\n" + "(" + "\r\n";

        //        //    // Loop Columns
        //        //    foreach (DataColumn column in dt.Columns)
        //        //    {
        //        //        strInsertQuery += "@" + column.ColumnName;

        //        //        // Add Trailing Comma
        //        //        if (dt.Columns.IndexOf(column) < dt.Columns.Count - 1)
        //        //        {
        //        //            strInsertQuery += ", " + "\r\n";
        //        //        }

        //        //        string strValue = dr[column].ToString();

        //        //        // Add Parameters And Values
        //        //        cmd.Parameters.AddWithValue("@" + column.ColumnName, strValue);
        //        //    }

        //        //    strInsertQuery += "\r\n" + ")";

        //        //    cmd.CommandText = strInsertQuery;
        //        //    cmd.Connection = this.m_SqlFunctions.Connection;

        //        //    // Execute Non Query
        //        //    int intRowsAffected = this.m_SqlFunctions.ExecuteNonQuery(cmd, ref strError);

        //        //    // Validation
        //        //    if (intRowsAffected < 1)
        //        //    {
        //        //        strError = "Error: Row Not Inserted.";

        //        //        insertAllRowsResultType = Globals.ResultType.Failure;
        //        //    }
        //        //}

        //        //return insertAllRowsResultType;

        //        #endregion

        //    }
        //    catch (Exception ex)
        //    {
        //        strError = ex.ToString();

        //        return Globals.ResultType.Failure;
        //    }
        //}

        //private Globals.ResultType LoadDataIntoTable(IDatabaseFunctions functionsFrom, IDatabaseFunctions functionsTo, ref string strError)
        //{
        //    try
        //    {
        //        // Get DataTable
        //        DataTable dt = functionsFrom.GetDataTable(this.m_Query, ref strError);

        //        // Validation
        //        if (dt == null || strError != "") { return Globals.ResultType.Failure; }

        //        Globals.ResultType insertAllRowsResultType = Globals.ResultType.Success;

        //        // Get Sql Command List
        //        List<System.Data.SqlClient.SqlCommand> listCommands = this.GetSqlInsertQueryCommandList(dt, ref strError);

        //        // Validation
        //        if (listCommands == null || strError != "") { return Globals.ResultType.Failure; }

        //        // Loop Sql Commands
        //        foreach (System.Data.SqlClient.SqlCommand cmd in listCommands)
        //        {
        //            // Execute Non Query
        //            int intRowsAffected = functionsTo.ExecuteNonQuery(cmd.CommandText, ref strError);

        //            // Validation
        //            if (intRowsAffected < 1)
        //            {
        //                strError = "Error: Row Not Inserted.";

        //                insertAllRowsResultType = Globals.ResultType.Failure;
        //            }
        //        }

        //        return insertAllRowsResultType;

        //        #region Commented Out Code

        //        //// Loop DataRows
        //        //foreach (DataRow dr in dt.Rows)
        //        //{
        //        //    string strInsertQuery = "INSERT INTO " + this.m_TableName + " (" + "\r\n";

        //        //    // Sanitize Row Values Into New List
        //        //    List<string> listValues = dr.ItemArray.Select(value => (value != null && value != DBNull.Value) ? value.ToString() : "").ToList();

        //        //    //Create Sql COmmand
        //        //    System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();

        //        //    // Loop Columns
        //        //    foreach (DataColumn column in dt.Columns)
        //        //    {
        //        //        strInsertQuery += "\t" + column.ColumnName;

        //        //        if (dt.Columns.IndexOf(column) < dt.Columns.Count - 1)
        //        //        {
        //        //            strInsertQuery += ",";
        //        //        }

        //        //        strInsertQuery += "\r\n";
        //        //    }

        //        //    strInsertQuery += ")" + "\r\n" + " VALUES " + "\r\n" + "(" + "\r\n";

        //        //    // Loop Columns
        //        //    foreach (DataColumn column in dt.Columns)
        //        //    {
        //        //        strInsertQuery += "@" + column.ColumnName;

        //        //        // Add Trailing Comma
        //        //        if (dt.Columns.IndexOf(column) < dt.Columns.Count - 1)
        //        //        {
        //        //            strInsertQuery += ", " + "\r\n";
        //        //        }

        //        //        string strValue = dr[column].ToString();

        //        //        // Add Parameters And Values
        //        //        cmd.Parameters.AddWithValue("@" + column.ColumnName, strValue);
        //        //    }

        //        //    strInsertQuery += "\r\n" + ")";

        //        //    cmd.CommandText = strInsertQuery;
        //        //    cmd.Connection = this.m_SqlFunctions.Connection;

        //        //    // Execute Non Query
        //        //    int intRowsAffected = this.m_SqlFunctions.ExecuteNonQuery(cmd, ref strError);

        //        //    // Validation
        //        //    if (intRowsAffected < 1)
        //        //    {
        //        //        strError = "Error: Row Not Inserted.";

        //        //        insertAllRowsResultType = Globals.ResultType.Failure;
        //        //    }
        //        //}

        //        //return insertAllRowsResultType;

        //        #endregion

        //    }
        //    catch (Exception ex)
        //    {
        //        strError = ex.ToString();

        //        return Globals.ResultType.Failure;
        //    }
        //}

        //private string GetCreateTableQueryViaOtherDbTable(DataTable dt, ref string strError)
        //{
        //    string strQuery = "CREATE TABLE " + this.m_TableName + "(" + "\r\n";

        //    // Loop DataTable Columns
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        // Sanitize Row Values Into New List
        //        List<string> listValues = dr.ItemArray.Select(value => (value != null && value != DBNull.Value) ? value.ToString() : "").ToList();

        //        string strColumnEntry = "\t" + listValues[0] + " " + listValues[1];

        //        // Get Sql Type
        //        SqlDbType type = GetSqlTypeFromString(listValues[1]);

        //        // TEMPORARY - Char DataType Exceeding Size Issue Causing Error, Switch To Varchar For Now - REMOVE
        //        type = (type == SqlDbType.Char) ? SqlDbType.VarChar : type;

        //        // Check Is Text
        //        if (type == SqlDbType.Char || type == SqlDbType.NChar || type == SqlDbType.NText || type == SqlDbType.NVarChar || type == SqlDbType.Text || type == SqlDbType.VarChar || type == SqlDbType.Xml)
        //        {
        //            strColumnEntry += "(" + listValues[2] + ")";
        //        }

        //        // Set All Columns Nullable For Now
        //        strColumnEntry += " NULL";

        //        // Validate That The Column Doesnt Already Exist
        //        if (strQuery.Contains(strColumnEntry)) { continue; }

        //        // Check Index To Add Trailing Comma
        //        if (dt.Rows.IndexOf(dr) < dt.Rows.Count - 1)
        //        {
        //            strColumnEntry += "," + "\r\n";
        //        }

        //        // Validation
        //        if (strQuery.Contains(strColumnEntry) == true) { continue; }

        //        strQuery += strColumnEntry;
        //    }

        //    strQuery += "\r\n" + ")";

        //    return strQuery;
        //}

        //private string GetCreateTableQueryViaQueryDataTable(DataTable dt, ref string strError)
        //{
        //    string strQuery = "CREATE TABLE " + this.m_TableName + "(" + "\r\n";

        //    // Loop DataTable Columns
        //    foreach (DataColumn column in dt.Columns)
        //    {
        //        //SqlDbType Type
        //        SqlDbType type = GetSqlTypeFromType(column.DataType);

        //        // TEMPORARY - Char DataType Exceeding Size Issue Causing Error, Switch To Varchar For Now - REMOVE
        //        type = (type == SqlDbType.Char) ? SqlDbType.VarChar : type;

        //        string strColumnEntry = "\t" + column.ColumnName + " " + type.ToString().ToLower();

        //        // Check Is Text
        //        if (type == SqlDbType.Char || type == SqlDbType.NChar || type == SqlDbType.NText || type == SqlDbType.NVarChar || type == SqlDbType.Text || type == SqlDbType.VarChar || type == SqlDbType.Xml)
        //        {

        //            strColumnEntry += "(255)";

        //            // Cant Retrieve Max Length For Now

        //            // Get Column Max Length
        //            int intMaxLength = column.MaxLength;
        //            //if (intMaxLength > 0)
        //            //{
        //            //    strColumnEntry += "(" + intMaxLength + ")";
        //            //}
        //        }

        //        // Set All Columns Nullable For Now
        //        strColumnEntry += " NULL";

        //        // Validate That The Column Doesnt Already Exist
        //        if (strQuery.Contains(strColumnEntry)) { continue; }

        //        // Check Index To Add Trailing Comma
        //        if (dt.Columns.IndexOf(column) < dt.Columns.Count - 1)
        //        {
        //            strColumnEntry += "," + "\r\n";
        //        }

        //        // Validation
        //        if (strQuery.Contains(strColumnEntry) == true) { continue; }

        //        strQuery += strColumnEntry;
        //    }

        //    strQuery += "\r\n" + ")";

        //    return strQuery;
        //}



        //// 3rd Party Create Function
        
        ///// <summary>
        ///// Inspects a DataTable and return a SQL string that can be used to CREATE a TABLE in SQL Server.
        ///// </summary>
        ///// <param name="table">System.Data.DataTable object to be inspected for building the SQL CREATE TABLE statement.</param>
        ///// <returns>String of SQL</returns>
        //public static string GetCreateTableSql(DataTable table)
        //{
        //    StringBuilder sql = new StringBuilder();
        //    StringBuilder alterSql = new StringBuilder();

        //    sql.AppendFormat("CREATE TABLE [{0}] (", table.TableName);

        //    for (int i = 0; i < table.Columns.Count; i++)
        //    {
        //        bool isNumeric = false;
        //        bool usesColumnDefault = true;

        //        sql.AppendFormat("\n\t[{0}]", table.Columns[i].ColumnName);

        //        switch (table.Columns[i].DataType.ToString().ToUpper())
        //        {
        //            case "SYSTEM.INT16":
        //                sql.Append(" smallint");
        //                isNumeric = true;
        //                break;
        //            case "SYSTEM.INT32":
        //                sql.Append(" int");
        //                isNumeric = true;
        //                break;
        //            case "SYSTEM.INT64":
        //                sql.Append(" bigint");
        //                isNumeric = true;
        //                break;
        //            case "SYSTEM.DATETIME":
        //                sql.Append(" datetime");
        //                usesColumnDefault = false;
        //                break;
        //            case "SYSTEM.STRING":
        //                sql.AppendFormat(" nvarchar({0})", table.Columns[i].MaxLength);
        //                break;
        //            case "SYSTEM.SINGLE":
        //                sql.Append(" single");
        //                isNumeric = true;
        //                break;
        //            case "SYSTEM.DOUBLE":
        //                sql.Append(" double");
        //                isNumeric = true;
        //                break;
        //            case "SYSTEM.DECIMAL":
        //                sql.AppendFormat(" decimal(18, 6)");
        //                isNumeric = true;
        //                break;
        //            default:
        //                sql.AppendFormat(" nvarchar({0})", table.Columns[i].MaxLength);
        //                break;
        //        }

        //        if (table.Columns[i].AutoIncrement)
        //        {
        //            sql.AppendFormat(" IDENTITY({0},{1})",
        //                table.Columns[i].AutoIncrementSeed,
        //                table.Columns[i].AutoIncrementStep);
        //        }
        //        else
        //        {
        //            // DataColumns will add a blank DefaultValue for any AutoIncrement column. 
        //            // We only want to create an ALTER statement for those columns that are not set to AutoIncrement. 
        //            if (table.Columns[i].DefaultValue != null)
        //            {
        //                if (usesColumnDefault)
        //                {
        //                    if (isNumeric)
        //                    {
        //                        alterSql.AppendFormat("\nALTER TABLE {0} ADD CONSTRAINT [DF_{0}_{1}]  DEFAULT ({2}) FOR [{1}];",
        //                            table.TableName,
        //                            table.Columns[i].ColumnName,
        //                            table.Columns[i].DefaultValue);
        //                    }
        //                    else
        //                    {
        //                        alterSql.AppendFormat("\nALTER TABLE {0} ADD CONSTRAINT [DF_{0}_{1}]  DEFAULT ('{2}') FOR [{1}];",
        //                            table.TableName,
        //                            table.Columns[i].ColumnName,
        //                            table.Columns[i].DefaultValue);
        //                    }
        //                }
        //                else
        //                {
        //                    // Default values on Date columns, e.g., "DateTime.Now" will not translate to SQL.
        //                    // This inspects the caption for a simple XML string to see if there is a SQL compliant default value, e.g., "GETDATE()".
        //                    try
        //                    {
        //                        System.Xml.XmlDocument xml = new System.Xml.XmlDocument();

        //                        xml.LoadXml(table.Columns[i].Caption);

        //                        alterSql.AppendFormat("\nALTER TABLE {0} ADD CONSTRAINT [DF_{0}_{1}]  DEFAULT ({2}) FOR [{1}];",
        //                            table.TableName,
        //                            table.Columns[i].ColumnName,
        //                            xml.GetElementsByTagName("defaultValue")[0].InnerText);
        //                    }
        //                    catch
        //                    {
        //                        // Handle
        //                    }
        //                }
        //            }
        //        }

        //        if (!table.Columns[i].AllowDBNull)
        //        {
        //            sql.Append(" NOT NULL");
        //        }

        //        sql.Append(",");
        //    }

        //    if (table.PrimaryKey.Length > 0)
        //    {
        //        StringBuilder primaryKeySql = new StringBuilder();

        //        primaryKeySql.AppendFormat("\n\tCONSTRAINT PK_{0} PRIMARY KEY (", table.TableName);

        //        for (int i = 0; i < table.PrimaryKey.Length; i++)
        //        {
        //            primaryKeySql.AppendFormat("{0},", table.PrimaryKey[i].ColumnName);
        //        }

        //        primaryKeySql.Remove(primaryKeySql.Length - 1, 1);
        //        primaryKeySql.Append(")");

        //        sql.Append(primaryKeySql);
        //    }
        //    else
        //    {
        //        sql.Remove(sql.Length - 1, 1);
        //    }

        //    sql.AppendFormat("\n);\n{0}", alterSql.ToString());

        //    return sql.ToString();
        //}
        
        #endregion
    }
}
