using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DataUtil
{
    /// <summary>
    /// Contains basic information about a Database table / DataTable
    /// </summary>
    public sealed class DataTableInformation
    {
        #region Properties

        #region Columns

        private DataColumnInformationList m_Columns;
        /// <summary>
        /// List of data columns in a database table or data table
        /// </summary>
        public DataColumnInformationList Columns
        {
            get
            {
                return this.m_Columns;
            }
        }

        #endregion

        #region Sql Queries

        /// <summary>
        /// Constant query for retrieving database table information
        /// </summary>
        private const string SQL_GET_TABLE_INFO_QUERY = @"
                                SELECT DISTINCT                                   
                                    sc.name 					COLUMN_NAME,
                                    st.name 					DATA_TYPE,
                                    sc.length 					FIELD_LENGTH,
                                    CASE WHEN st.allownulls=1 
                                    THEN 						'true'
                                    ELSE 						'false'
                                    END 						IS_NULLABLE
                                FROM 
                                    sysobjects so
                                INNER JOIN syscolumns sc ON sc.id = so.id
                                INNER JOIN systypes st on st.usertype = sc.usertype 
                                WHERE 
                                    so.name = ";


        #endregion

        #region Other

        private string m_TableName;
        /// <summary>
        /// Database table name
        /// </summary>
        public string TableName
        {
            get
            {
                return this.m_TableName;
            }
        }

        private bool m_TableExists;
        /// <summary>
        /// Whether or not the table exists
        /// </summary>
        public bool TableExists
        {
            get
            {
                return this.m_IsValid;
            }
        }

        private bool m_IsValid;
        /// <summary>
        /// Whether or not the table is valid or loaded correctly
        /// </summary>
        public bool IsValid
        {
            get
            {
                return this.m_IsValid;
            }
        }
        
        #endregion
        
        #endregion

        #region Initialization

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="strTableName">Database table name</param>
        /// <param name="functions">Database functions object</param>
        public DataTableInformation(string strTableName, IDatabaseFunctions functions)
        {
            string strError = "";

            // Validation
            if (TableName == "" || functions == null) { return; }

            // Instantiate Column List
            this.m_Columns = new DataColumnInformationList();
            
            this.m_TableName = strTableName;

            // Get Table Exists
            this.m_TableExists = DatabaseInformation.CheckDataBaseTableExists(this.m_TableName, functions, ref strError);
            
            // Validation
            if (this.m_TableExists == false) { return; }

            // Load Table Information
            this.LoadTableInformationFromDatabase(strTableName, functions);
        }

        /// <summary>
        /// Constructor indluding a DataTable and an optional table name
        /// </summary>
        /// <param name="dt">DataTable to load information from</param>
        /// <param name="strTableName">DataTable name</param>
        public DataTableInformation(DataTable dt, string strTableName = "GENERIC_TABLE")
        {
            // Validation
            if (TableName == "") { return; }

            // Instantiate Column List
            this.m_Columns = new DataColumnInformationList();

            this.m_TableName = strTableName;

            // Load Table Information
            this.LoadTableInformationFromDataTable(dt, strTableName);
        }
        
        #endregion

        #region Load Information

        /// <summary>
        /// Load table information from the database
        /// </summary>
        /// <param name="strTableName">Database table name to load information from</param>
        /// <param name="functions">IDatabaseFunctions object</param>
        private void LoadTableInformationFromDatabase(string strTableName, IDatabaseFunctions functions)
        {
            try
            {
                string strError = "";

                // Get Query String
                string strQuery = SQL_GET_TABLE_INFO_QUERY + "'" + strTableName + "'";

                // Get DataTable
                DataTable dt = functions.GetDataTable(strQuery, ref strError);

                // Validation
                if (dt == null || strError != "") 
                {
                    this.m_IsValid = false;

                    return;
                }

                // Sanitize Rows Into String Lists
                List<List<string>> listRowLists = dt.AsEnumerable().Select(row => row.ItemArray.Select(v => (v == null || v == DBNull.Value) ? "" : v.ToString()).ToList()).ToList();
                                
                // Loop DataRows
                foreach (List<string> listRowValues in listRowLists)
                {
                    // Validation
                    if (listRowLists.Count < 4)
                    {
                        this.m_IsValid = false;

                        return;
                    }

                    // Get Column Name
                    string strColumnName = listRowValues[0];

                    // Get Sql Data Type
                    SqlDbType dataType = DatabaseInformation.GetSqlTypeFromString(listRowValues[1]);

                    // Get Field Length
                    int intFieldLength = (int.TryParse(listRowValues[2], out intFieldLength) == true) ? int.Parse(listRowValues[2]) : 255;

                    // Get Nullable
                    bool boolIsNullable = (bool.TryParse(listRowValues[3], out boolIsNullable) == true) ? bool.Parse(listRowValues[3]) : true;

                    // Create New Column
                    DataColumnInformation column = new DataColumnInformation(strColumnName, dataType, intFieldLength, boolIsNullable);

                    // Add Column To List
                    this.m_Columns.Add(column);
                }

                this.m_IsValid = true;
            }
            catch (Exception ex)
            {
                // To Be Implemented: Throw Custom Exception...
                Console.WriteLine(ex.ToString());
                this.m_IsValid = false;
            }
        }

        /// <summary>
        /// Load data table information from a datatable
        /// </summary>
        /// <param name="dt">DataTable to load information from</param>
        /// <param name="strTableName">Database table name</param>
        private void LoadTableInformationFromDataTable(DataTable dt, string strTableName = "")
        {
            try
            {
                string strError = "";

                // Validation
                if (dt == null || strError != "")
                {
                    this.m_IsValid = false;

                    return;
                }

                DataColumn[] cols = new DataColumn[dt.Columns.Count];
                dt.Columns.CopyTo(cols, 0);

                //Create New List Of Columns
                List<DataColumn> listColumns = new List<DataColumn>();
                listColumns.AddRange(cols);

                // Loop Columns
                foreach (DataColumn column in listColumns)
                {
                    // Get Sql Type
                    SqlDbType type = DatabaseInformation.GetSqlTypeFromType(column.DataType);

                    // Get Max-length
                    int intMaxLength = (column.MaxLength > 0) ? column.MaxLength : 255;

                    // Create Column Info
                    DataColumnInformation columnInfo = new DataColumnInformation(column.ColumnName, type, intMaxLength, column.AllowDBNull);

                    this.m_Columns.Add(columnInfo);
                }

                this.m_IsValid = true;
            }
            catch (Exception ex)
            {
                // To Be Implemented: Throw Custom Exception...
                Console.WriteLine(ex.ToString());

                this.m_IsValid = false;
            }
        }

        #endregion

        #region Sql Query Creation Helpers

        #region General
        
        /// <summary>
        /// Retrieve a create table query string
        /// </summary>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <returns></returns>
        public string GetCreateTableQuery(ref string strError)
        {
            string strQuery = "CREATE TABLE " + this.m_TableName + "\r\n " + "(" + "\r\n";

            // Loop Columns
            foreach (DataColumnInformation column in this.Columns)
            {
                string strColumnEntry = column.CreateTableColumnEntry;

                strColumnEntry += (this.Columns.IndexOf(column) < this.Columns.Count - 1) ? "," + "\r\n" : "\r\n";

                strQuery += strColumnEntry;
            }

            strQuery += ")";

            return strQuery;
        }

        #endregion

        #region SQL

        /// <summary>
        /// Populate a Sql insert query
        /// </summary>
        /// <param name="dr">DataRow to get insert query for</param>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <returns></returns>
        private string GetSqlInsertQuery(DataRow dr, ref string strError)
        {
            string strInsertQuery = "INSERT INTO " + this.m_TableName + " (" + "\r\n";

            // Sanitize Row Values Into New List
            List<string> listValues = dr.ItemArray.Select(value => (value != null && value != DBNull.Value) ? value.ToString() : "").ToList();

            // Loop Columns
            foreach (DataColumn column in dr.Table.Columns)
            {
                strInsertQuery += "\t" + column.ColumnName;

                if (dr.Table.Columns.IndexOf(column) < dr.Table.Columns.Count - 1)
                {
                    strInsertQuery += ",";
                }

                strInsertQuery += "\r\n";
            }

            strInsertQuery += ")" + "\r\n" + " VALUES " + "\r\n" + "(" + "\r\n";

            // Loop Columns
            foreach (DataColumn column in dr.Table.Columns)
            {
                string strValue = (column.DataType == typeof(string) || column.DataType == typeof(char) || column.DataType == typeof(DateTime)) 
                    ? "'" + dr[column].ToString().Replace("'", "''") + "'" 
                    : dr[column].ToString();

                // Add Value To Query
                strInsertQuery += strValue;

                // Add Trailing Comma
                if (dr.Table.Columns.IndexOf(column) < dr.Table.Columns.Count - 1)
                {
                    strInsertQuery += ", " + "\r\n";
                }                
            }

            strInsertQuery += "\r\n" + ")";

            return strInsertQuery;
        }

        /// <summary>
        /// Populate a Sql insert query
        /// </summary>
        /// <param name="dt">DataTable to get insert query for</param>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <returns></returns>
        public List<string> GetSqlInsertQueryList(DataTable dt, ref string strError)
        {
            List<string> listQueries = new List<string>();

            // Loop DataRows
            foreach (DataRow dr in dt.Rows)
            {
                // Get Sql Command
                string strInsertQuery = this.GetSqlInsertQuery(dr, ref strError);

                // Validation
                if (strInsertQuery == null || strError != "") { return null; }

                // Add Command
                listQueries.Add(strInsertQuery);
            }

            return listQueries;
        }

        #endregion

        #region SYBASE

        /// <summary>
        /// Populate a Sybase insert query
        /// </summary>
        /// <param name="dr">DataRow to get insert query for</param>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <returns></returns>
        private string GetSybaseInsertQuery(DataRow dr, ref string strError)
        {
            string strInsertQuery = "INSERT INTO " + this.m_TableName + " (" + "\r\n";

            // Sanitize Row Values Into New List
            List<string> listValues = dr.ItemArray.Select(value => (value != null && value != DBNull.Value) ? value.ToString() : "").ToList();

            // Loop Columns
            foreach (DataColumn column in dr.Table.Columns)
            {
                strInsertQuery += "\t" + column.ColumnName;

                // If Column Index In Table Is Not Last
                if (dr.Table.Columns.IndexOf(column) < dr.Table.Columns.Count - 1)
                {
                    strInsertQuery += ",";
                }

                strInsertQuery += "\r\n";
            }

            strInsertQuery += ")" + "\r\n" + " VALUES " + "\r\n" + "(" + "\r\n";

            // Loop Columns
            foreach (DataColumn column in dr.Table.Columns)
            {
                //strInsertQuery += "@" + column.ColumnName;

                string strValue = (column.DataType == typeof(string) || column.DataType == typeof(char) || column.DataType == typeof(DateTime))
                    ? "'" + dr[column].ToString() + "'"
                    : dr[column].ToString();

                // Add Value To Query
                strInsertQuery += strValue;

                // Add Trailing Comma
                if (dr.Table.Columns.IndexOf(column) < dr.Table.Columns.Count - 1)
                {
                    strInsertQuery += ", " + "\r\n";
                }
            }

            strInsertQuery += "\r\n" + ")";

            return strInsertQuery;
        }

        /// <summary>
        /// Populate a Sybase insert query
        /// </summary>
        /// <param name="dt">DataTable to get insert query for</param>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <returns></returns>
        public List<string> GetSybaseInsertQueryList(DataTable dt, ref string strError)
        {
            List<string> listCommands = new List<string>();

            // Loop DataRows
            foreach (DataRow dr in dt.Rows)
            {
                // Get Sql Command
                string strInsertQuery = this.GetSybaseInsertQuery(dr, ref strError);

                // Validation
                if (strInsertQuery == "" || strError != "") { return null; }

                // Add Command
                listCommands.Add(strInsertQuery);
            }

            return listCommands;
        }

        #endregion

        #endregion
    }
}
