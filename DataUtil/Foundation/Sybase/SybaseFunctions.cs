using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;
using CoreUtil;

namespace DataUtil
{
    /// <summary>
    /// Sybase Functions Instance
    /// </summary>
    public sealed class SybaseFunctions : DatabaseFunctions
    {

        #region Properties
        
        /// <summary>
        /// Sybase connection
        /// </summary>
        private OdbcConnection m_Connection { get; set; }

        /// <summary>
        /// Sybase connection information object
        /// </summary>
        private SybaseConnectionInformation m_SybaseInfo = null;

        private bool m_IsConnected = false;
        /// <summary>
        /// Flag for whether or not the connection to the database server is open
        /// </summary>
        public new bool IsConnected
        {
            get
            {
                return this.m_IsConnected;
            }
        }

        #endregion
        
        #region Initialization

        /// <summary>
        /// Default constructor including Sybase connection information object
        /// </summary>
        /// <param name="sybaseConnectionInformation">Sybase connection information object</param>
        public SybaseFunctions(SybaseConnectionInformation sybaseConnectionInformation)
        {
            this.m_SybaseInfo = sybaseConnectionInformation;

            string strConnectionString = this.m_SybaseInfo.ConnectionString;
            this.m_Connection = new OdbcConnection(strConnectionString);
        }

        /// <summary>
        /// Constructor including database server name and database name
        /// </summary>
        /// <param name="strServerName">Database server name</param>
        /// <param name="strDatabaseName">Database name</param>
        public SybaseFunctions(string strServerName, string strDatabaseName)
        {
            this.m_SybaseInfo = new SybaseConnectionInformation(strServerName, strDatabaseName);

            string strConnectionString = this.m_SybaseInfo.ConnectionString;
            this.m_Connection = new OdbcConnection(strConnectionString);
        }

        /// <summary>
        /// Constructor including database server name, database name, username, and user password
        /// </summary>
        /// <param name="strServerName">Database server name</param>
        /// <param name="strDatabaseName">Database name</param>
        /// <param name="strUserId">Username</param>
        /// <param name="strPassword">User password</param>
        public SybaseFunctions(string strServerName, string strDatabaseName, string strUserId, string strPassword)
        {
            this.m_SybaseInfo = new SybaseConnectionInformation(strServerName, strDatabaseName, strUserId, strPassword);

            string strConnectionString = this.m_SybaseInfo.ConnectionString;
            this.m_Connection = new OdbcConnection(strConnectionString);
        }

        #endregion

        #region Connection Functions

        /// <summary>
        /// Retrieve a generic database connection
        /// </summary>
        /// <returns></returns>
        public override IDbConnection GetConnection()
        {
            return (IDbConnection)this.m_Connection;
        }

        /// <summary>
        /// Attempt to connect to the database server if not already connected. Returns success if already connected. Method is useful for cases not knowing if already connected to the database server and reducing validation
        /// </summary>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <returns></returns>
        public override Globals.ResultType TryConnect(ref string strError)
        {
            // Check Is Connected
            if (this.IsConnected == false || (this.m_Connection != null && this.m_Connection.State == ConnectionState.Closed))
            {
                // Connect to Server
                Globals.ResultType connectResult = this.Connect(ref strError);

                // Validation
                if (connectResult == Globals.ResultType.Failure || strError != "")
                {
                    strError += "Error: The connection to server '" + this.m_SybaseInfo.DatabaseServer + "' failed.";

                    return Globals.ResultType.Failure; ;
                }
            }

            return Globals.ResultType.Success;
        }

        /// <summary>
        /// Connect to the database server
        /// </summary>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <returns></returns>
        public override Globals.ResultType Connect(ref string strError)
        {
            try
            {
                // Validation
                if (this.m_Connection == null) { return Globals.ResultType.Failure; }

                // Validation
                if (this.m_Connection.State != ConnectionState.Open)
                {
                    this.m_Connection.Open();
                }

                this.m_IsConnected = true;

                return Globals.ResultType.Success;
            }
            catch (Exception ex)
            {
                strError = ex.ToString();
                Console.Write(ex.ToString());

                return Globals.ResultType.Failure;
            }
        }

        /// <summary>
        /// Disconnect from the database server
        /// </summary>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <returns></returns>
        public override Globals.ResultType Disconnect(ref string strError)
        {
            try
            {
                // Validation
                if (this.m_Connection == null) 
                {
                    this.m_IsConnected = false;

                    return Globals.ResultType.Failure; 
                }
                
                // Validation
                if (this.m_Connection.State == ConnectionState.Open)
                {
                    this.m_Connection.Close();
                }

                this.m_IsConnected = false;

                return Globals.ResultType.Success;
            }
            catch (Exception ex)
            {
                strError = ex.ToString();
                Console.Write(ex.ToString());

                this.m_IsConnected = false;

                return Globals.ResultType.Failure;
            }
        }

        #endregion

        #region Validation
        
        #endregion
        
        #region Get Data

        /// <summary>
        /// Query to be executed
        /// </summary>
        /// <param name="strQuery"></param>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <param name="intDefaultTimeout">Optional: Number of seconds before timing out the Sql query</param>
        /// <returns></returns>
        public override DataTable GetDataTable(string strQuery, ref string strError, int intDefaultTimeout = 300)
        {
            try
            {
                // Validation
                if (strQuery == "") { return null; }

                // Ensure Connected
                Globals.ResultType connectedResult = this.TryConnect(ref strError);

                // Validation
                if (connectedResult == Globals.ResultType.Failure || strError != "") { return null; }

                // Create DataAdapter
                OdbcDataAdapter dataAdapter = new OdbcDataAdapter(strQuery, this.m_Connection);
                dataAdapter.SelectCommand.CommandTimeout = 300;
                DataSet ds = new DataSet();

                // Execute Select Query
                dataAdapter.Fill(ds);

                // Disconnect From Server
                this.m_Connection.Close();

                if (ds == null || ds.Tables.Count == 0) { return null; }

                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                strError = ex.ToString();
                return null;
            }
        }

        /// <summary>
        /// Execute a query and return a single scalar value of a specified type
        /// </summary>
        /// <typeparam name="T">Type of object to be returned</typeparam>
        /// <param name="strQuery">Query to be executed</param>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <returns></returns>
        public override T ExecuteScalar<T>(string strQuery, ref string strError)
        {
            try
            {
                // Validation
                if (strQuery == "") { return default(T); }

                // Ensure Connected
                Globals.ResultType connectedResult = this.TryConnect(ref strError);

                // Validation
                if (connectedResult == Globals.ResultType.Failure || strError != "") { return default(T); }

                // Create SqlCommand
                OdbcCommand cmd = new OdbcCommand(strQuery, this.m_Connection);

                // Execute Scalar
                object obj = cmd.ExecuteScalar();

                // Create Object To Return
                T objReturn = (T)Activator.CreateInstance<T>();

                // Validation
                if (objReturn.GetType().IsAssignableFrom(obj.GetType()) == false)
                {
                    return default(T);
                }

                // Set Return Object Value
                objReturn = (T)obj;

                return objReturn;
            }
            catch (Exception ex)
            {
                strError = ex.ToString();

                return default(T);
            }
        }

        /// <summary>
        /// Create a OdbcDataReader from a Sql query
        /// </summary>
        /// <param name="strQuery">Query to be executed</param>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <param name="behavior">Provides a description of the results of the query and its effect on the database</param>
        /// <returns></returns>
        public OdbcDataReader ExecuteReader(string strQuery, ref string strError, CommandBehavior behavior = CommandBehavior.Default)
        {
            try
            {
                // Validation
                if (strQuery == "") { return null; }

                // Create SqlCommand
                OdbcCommand cmd = new OdbcCommand(strQuery, this.m_Connection);

                // Create Data Reader
                OdbcDataReader reader = this.ExecuteReader(cmd, ref strError, behavior);

                // Validation
                if (reader == null || strError != "") { return null; }

                // Return Number Of Rows Affected
                return reader;
            }
            catch (Exception ex)
            {
                strError = ex.ToString();

                return null;
            }
        }

        /// <summary>
        /// Create a OdbcDataReader from a Odbc command
        /// </summary>
        /// <param name="cmd">Odbc command to be executed</param>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <param name="behavior">Provides a description of the results of the query and its effect on the database</param>
        /// <returns></returns>
        public OdbcDataReader ExecuteReader(OdbcCommand cmd, ref string strError, CommandBehavior behavior = CommandBehavior.Default)
        {
            try
            {
                // Ensure Connected
                Globals.ResultType connectedResult = this.TryConnect(ref strError);

                // Validation
                if (connectedResult == Globals.ResultType.Failure || strError != "") { return null; }

                // Create Data Reader
                OdbcDataReader reader = cmd.ExecuteReader(behavior);

                // Return Number Of Rows Affected
                return reader;
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
        public override int ExecuteNonQuery(string strQuery, ref string strError)
        {
            try
            {
                // Validation
                if (strQuery == "") { return -1; }

                // Ensure Connected
                Globals.ResultType connectedResult = this.TryConnect(ref strError);

                // Validation
                if (connectedResult == Globals.ResultType.Failure || strError != "") { return -1; }

                // Create SqlCommand
                OdbcCommand cmd = new OdbcCommand(strQuery, this.m_Connection);

                // Execute Non Query
                int intRowsAffected = cmd.ExecuteNonQuery();

                // Return Number Of Rows Affected
                return intRowsAffected;
            }
            catch (Exception ex)
            {
                strError = ex.ToString();

                return -1;
            }
        }

        public int ExecuteNonQuery(OdbcCommand cmd, ref string strError)
        {
            try
            {
                // Ensure Connected
                Globals.ResultType connectedResult = this.TryConnect(ref strError);

                // Validation
                if (connectedResult == Globals.ResultType.Failure || strError != "") { return -1; }

                // Execute Non Query
                int intRowsAffected = cmd.ExecuteNonQuery();

                // Return Number Of Rows Affected
                return intRowsAffected;
            }
            catch (Exception ex)
            {
                strError = ex.ToString();

                return -1;
            }
        }
        
        #endregion
        
        #region Transaction Functions

        /// <summary>
        /// Execute/Commit a transaction
        /// </summary>
        /// <param name="strQuery">Query to be executed</param>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <returns></returns>
        public override Globals.ResultType ExecuteTransaction(string strQuery, ref string strError)
        {
            try
            {
                // Create New ODBC Command
                OdbcCommand cmd = new OdbcCommand(strQuery, this.m_Connection);

                //Validation
                if (this.m_Connection == null) { return Globals.ResultType.Failure; }

                //Validation
                if (this.m_Connection.State != ConnectionState.Open)
                {
                    this.m_Connection.Open();
                }

                // Execute Query
                cmd.ExecuteNonQuery();

                return Globals.ResultType.Success;
            }
            catch (Exception ex)
            {
                strError = ex.ToString();

                return Globals.ResultType.Failure;
            }
        }

        #endregion
        

        #region Commented Out Code
        
        //        public bool CheckDataBaseTableExists(string strTableName, ref string strError)
//        {
//            try
//            {
//                // Sanitize Table Name
//                strTableName = strTableName.Trim();

//                // Validation
//                if (strTableName == "") { return false; }

//                // Create Query
//                string strQuery = @"
//								SELECT
//                                    
//                                    COUNT(sc.name)
//                                FROM 
//                                    sysobjects so
//                                INNER JOIN syscolumns sc ON sc.id = so.id
//                                INNER JOIN systypes st on st.usertype = sc.usertype 
//                                WHERE 
//                                    so.name = '" + strTableName + "'";

//                // Get Table Column Count
//                int intTableColumns = this.ExecuteScalar<int>(strQuery, ref strError);

//                // Validation
//                if (intTableColumns == 0 || strError != "") { return false; }

//                // Check Table Exists
//                bool boolTableExists = intTableColumns > 0;

//                return boolTableExists;
//            }
//            catch (Exception ex)
//            {
//                strError = ex.ToString();

//                return false;
//            }
//        }

//        public bool CheckDataBaseTableHasColumn(string strTableName, string strColumnName, ref string strError)
//        {
//            try
//            {
//                // Sanitize Table Name
//                strTableName = strTableName.Trim();

//                // Validation
//                if (strTableName == "" || strColumnName == "") { return false; }

//                // Create Query
//                string strQuery = @"
//								SELECT                                    
//                                    COUNT(sc.name)
//                                FROM 
//                                    sysobjects so
//                                INNER JOIN syscolumns sc ON sc.id = so.id
//                                INNER JOIN systypes st on st.usertype = sc.usertype 
//                                WHERE 
//                                    so.name         = '" + strTableName + @"'
//                                    AND sc.name     = '" + strColumnName + "'";

//                // Get Table Column Count
//                int intTableColumns = this.ExecuteScalar<int>(strQuery, ref strError);

//                // Validation
//                if (intTableColumns == 0 || strError != "") { return false; }

//                // Check Table Exists
//                bool boolTableExists = intTableColumns > 0;

//                return boolTableExists;
//            }
//            catch (Exception ex)
//            {
//                strError = ex.ToString();

//                return false;
//            }
//        }

//        public bool CheckDataBaseTableHasColumnList(string strTableName, List<string> listColumnNames, ref string strError)
//        {
//            try
//            {
//                // Validation
//                if (listColumnNames == null || listColumnNames.Count == 0) { return false; }

//                foreach (string strColumnName in listColumnNames)
//                {
//                    // Check Table Exists
//                    bool boolTableExists = this.CheckDataBaseTableHasColumn(strTableName, strColumnName, ref strError);

//                    // Validation
//                    if (boolTableExists == false || strError != "") { return false; }
//                }

//                return true;
//            }
//            catch (Exception ex)
//            {
//                strError = ex.ToString();

//                return false;
//            }
//        }
        //public Type GetTypeFromSqlType(SqlDbType sqltype)
        //{
        //    Type resulttype = null;

        //    // Create Type Dictionary
        //    Dictionary<SqlDbType, Type> Types = new Dictionary<SqlDbType, Type>();
        //    Types.Add(SqlDbType.BigInt, typeof(Int64));
        //    Types.Add(SqlDbType.Binary, typeof(Byte[]));
        //    Types.Add(SqlDbType.Bit, typeof(Boolean));
        //    Types.Add(SqlDbType.Char, typeof(String));
        //    Types.Add(SqlDbType.Date, typeof(DateTime));
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
        //    Types.TryGetValue(sqltype, out resulttype);

        //    return resulttype;
        //}

        //public SqlDbType GetSqlTypeFromType(Type type)
        //{
        //    SqlDbType resulttype = SqlDbType.Char;

        //    // Create Type Dictionary
        //    Dictionary<Type, SqlDbType> Types = new Dictionary<Type, SqlDbType>();
        //    Types.Add(typeof(Int64), SqlDbType.BigInt);
        //    Types.Add(typeof(Byte[]), SqlDbType.Binary);
        //    Types.Add(typeof(Boolean), SqlDbType.Bit);
        //    Types.Add(typeof(String), SqlDbType.Char);
        //    Types.Add(typeof(DateTime), SqlDbType.Date);
        //    Types.Add(typeof(DateTime), SqlDbType.DateTime);
        //    Types.Add(typeof(DateTime), SqlDbType.DateTime2);
        //    Types.Add(typeof(DateTimeOffset), SqlDbType.DateTimeOffset);
        //    Types.Add(typeof(Decimal), SqlDbType.Decimal);
        //    Types.Add(typeof(Double), SqlDbType.Float);
        //    Types.Add(typeof(Byte[]), SqlDbType.Image);
        //    Types.Add(typeof(Int32), SqlDbType.Int);
        //    Types.Add(typeof(Decimal), SqlDbType.Money);
        //    Types.Add(typeof(String), SqlDbType.NChar);
        //    Types.Add(typeof(String), SqlDbType.NText);
        //    Types.Add(typeof(String), SqlDbType.NVarChar);
        //    Types.Add(typeof(Single), SqlDbType.Real);
        //    Types.Add(typeof(DateTime), SqlDbType.SmallDateTime);
        //    Types.Add(typeof(Int16), SqlDbType.SmallInt);
        //    Types.Add(typeof(Decimal), SqlDbType.SmallMoney);
        //    Types.Add(typeof(String), SqlDbType.Text);
        //    Types.Add(typeof(TimeSpan), SqlDbType.Time);
        //    Types.Add(typeof(Byte[]), SqlDbType.Timestamp);
        //    Types.Add(typeof(Byte), SqlDbType.TinyInt);
        //    Types.Add(typeof(Guid), SqlDbType.UniqueIdentifier);
        //    Types.Add(typeof(Byte[]), SqlDbType.VarBinary);
        //    Types.Add(typeof(String), SqlDbType.VarChar);

        //    // Get Type Value
        //    Types.Where(pair => pair.Key == type).Select(pair => pair.Key).FirstOrDefault();

        //    return resulttype;
        //}


        //public Globals.ResultType BeginTransaction(ref string strError, string strTransactionName = "")
        //{
        //    strTransactionName = (strTransactionName.Trim() == "") ? "GENERIC_TRANSACTION" : strTransactionName;
        //    String strQuery = "BEGIN TRANSACTION " + strTransactionName;

        //    Globals.ResultType resultType = this.ExecuteTransaction(strQuery, ref strError);

        //    return resultType;
        //}

        //public Globals.ResultType CommitTransaction(ref string strError, string strTransactionName = "")
        //{
        //    strTransactionName = (strTransactionName.Trim() == "") ? "GENERIC_TRANSACTION" : strTransactionName;
        //    String strQuery = "COMMIT TRANSACTION " + strTransactionName;

        //    Globals.ResultType resultType = this.ExecuteTransaction(strQuery, ref strError);

        //    return resultType;
        //}

        //public Globals.ResultType RollbackTransaction(ref string strError, string strTransactionName = "")
        //{
        //    strTransactionName = (strTransactionName.Trim() == "") ? "GENERIC_TRANSACTION" : strTransactionName;
        //    String strQuery = "ROLLBACK TRANSACTION " + strTransactionName;

        //    Globals.ResultType resultType = this.ExecuteTransaction(strQuery, ref strError);

        //    return resultType;
        //}
        
//        public List<string> GetDataTableColumnList(string strTableName, ref string strError)
//        {
//            try
//            {
//                // Sanitize Table Name
//                strTableName = strTableName.Trim();

//                // Validation
//                if (strTableName == "") { return null; }

//                // Create Query
//                string strQuery = @"
//								SELECT
//                                    
//                                    sc.name AS COLUMN_NAME
//                                FROM 
//                                    sysobjects so
//                                INNER JOIN syscolumns sc ON sc.id = so.id
//                                INNER JOIN systypes st on st.usertype = sc.usertype 
//                                WHERE 
//                                    so.name = '" + strTableName + "'";

//                // Get Table Column Count
//                DataTable dt = this.GetDataTable(strQuery, ref strError);

//                // Validation
//                if (dt == null || strError != "") { return null; }

//                // Get Column  As String List
//                List<string> listColumnNames = DataTableFunctions.GetDataTableColumnAsList(dt, "COLUMN_NAME");

//                return listColumnNames;
//            }
//            catch (Exception ex)
//            {
//                strError = ex.ToString();

//                return null;
//            }
//        }
        

        #endregion
    }
}