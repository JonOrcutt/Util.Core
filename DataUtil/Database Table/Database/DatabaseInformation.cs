using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DataUtil
{
    /// <summary>
    /// Static class for retrieving Database related information
    /// </summary>
    public static class DatabaseInformation
    {

        #region Properties
        
        private static Dictionary<SqlDbType, Type> m_DataTypeMappings = null;
        /// <summary>
        /// Dictionary of mappings between SqlDbTypes and .NET Types
        /// </summary>
        public static Dictionary<SqlDbType, Type> DataTypeMappings
        {
            get
            {
                // Validation
                if (m_DataTypeMappings == null)
                {
                    // Initialize Type Mapping Dictionary
                    m_DataTypeMappings = new Dictionary<SqlDbType, Type>();
                    m_DataTypeMappings.Add(SqlDbType.BigInt, typeof(Int64));
                    m_DataTypeMappings.Add(SqlDbType.Binary, typeof(Byte[]));
                    m_DataTypeMappings.Add(SqlDbType.Bit, typeof(Boolean));
                    m_DataTypeMappings.Add(SqlDbType.Char, typeof(String));
                    //m_DataTypeMappings.Add(SqlDbType.Date, typeof(DateTime));
                    m_DataTypeMappings.Add(SqlDbType.DateTime, typeof(DateTime));
                    m_DataTypeMappings.Add(SqlDbType.DateTime2, typeof(DateTime));
                    m_DataTypeMappings.Add(SqlDbType.DateTimeOffset, typeof(DateTimeOffset));
                    m_DataTypeMappings.Add(SqlDbType.Decimal, typeof(Decimal));
                    m_DataTypeMappings.Add(SqlDbType.Float, typeof(Double));
                    m_DataTypeMappings.Add(SqlDbType.Image, typeof(Byte[]));
                    m_DataTypeMappings.Add(SqlDbType.Int, typeof(Int32));
                    m_DataTypeMappings.Add(SqlDbType.Money, typeof(Decimal));
                    m_DataTypeMappings.Add(SqlDbType.NChar, typeof(String));
                    m_DataTypeMappings.Add(SqlDbType.NText, typeof(String));
                    m_DataTypeMappings.Add(SqlDbType.NVarChar, typeof(String));
                    m_DataTypeMappings.Add(SqlDbType.Real, typeof(Single));
                    m_DataTypeMappings.Add(SqlDbType.SmallDateTime, typeof(DateTime));
                    m_DataTypeMappings.Add(SqlDbType.SmallInt, typeof(Int16));
                    m_DataTypeMappings.Add(SqlDbType.SmallMoney, typeof(Decimal));
                    m_DataTypeMappings.Add(SqlDbType.Text, typeof(String));
                    m_DataTypeMappings.Add(SqlDbType.Time, typeof(TimeSpan));
                    m_DataTypeMappings.Add(SqlDbType.Timestamp, typeof(Byte[]));
                    m_DataTypeMappings.Add(SqlDbType.TinyInt, typeof(Byte));
                    m_DataTypeMappings.Add(SqlDbType.UniqueIdentifier, typeof(Guid));
                    m_DataTypeMappings.Add(SqlDbType.VarBinary, typeof(Byte[]));
                    m_DataTypeMappings.Add(SqlDbType.VarChar, typeof(String));
                }

                return m_DataTypeMappings;
            }
        }

        #endregion
        
        #region Type Mappings

        /// <summary>
        /// Retrieve a .NET type from a Sql type
        /// </summary>
        /// <param name="sqltype">Type to convert to Sql type</param>
        /// <returns></returns>
        public static Type GetTypeFromSqlType(SqlDbType sqltype)
        {
            Type resultType = null;

            // Get Type Value
            DataTypeMappings.TryGetValue(sqltype, out resultType);

            return resultType;
        }

        /// <summary>
        ///  Retrieve a Sql type from a .Net type
        /// </summary>
        /// <param name="type">Type to convert to Sql type</param>
        /// <returns></returns>
        public static SqlDbType GetSqlTypeFromType(Type type)
        {
            SqlDbType resultType = SqlDbType.Char;

            // Get Type Value
            resultType = DataTypeMappings
                .Where(pair => pair.Value == type)
                .Select(pair => pair.Key).FirstOrDefault();

            return resultType;
        }

        /// <summary>
        /// Get SqlDbType from string
        /// </summary>
        /// <param name="strType">Type to convert to Sql type</param>
        /// <returns></returns>
        public static SqlDbType GetSqlTypeFromString(string strType)
        {
            SqlDbType resultType = SqlDbType.Char;

            // Get Type Value
            resultType = DataTypeMappings
                .Where(pair => pair.Key.ToString().ToLower() == strType.ToLower())
                .Select(pair => pair.Key).FirstOrDefault();

            return resultType;
        }

        #endregion

        #region Validation

        /// <summary>
        /// Check to ensure a database table exists
        /// </summary>
        /// <param name="strTableName">Name of the table to check for</param>
        /// <param name="functions">IDatabaseFunctions object</param>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <returns></returns>
        public static bool CheckDataBaseTableExists(string strTableName, IDatabaseFunctions functions, ref string strError)
        {
            try
            {
                // Sanitize Table Name
                strTableName = strTableName.Trim();

                // Validation
                if (strTableName == "") { return false; }

                // Create Query
                string strQuery = @"
								SELECT
                                    
                                    COUNT(sc.name)
                                FROM 
                                    sysobjects so
                                INNER JOIN syscolumns sc ON sc.id = so.id
                                INNER JOIN systypes st on st.usertype = sc.usertype 
                                WHERE 
                                    so.name = '" + strTableName + "'";

                // Get Table Column Count
                int intTableColumns = functions.ExecuteScalar<int>(strQuery, ref strError);

                // Validation
                if (intTableColumns == 0 || strError != "") { return false; }

                // Check Table Exists
                bool boolTableExists = intTableColumns > 0;

                return boolTableExists;
            }
            catch (Exception ex)
            {
                strError = ex.ToString();

                return false;
            }
        }

        /// <summary>
        /// Check to ensure a database table has a specific column
        /// </summary>
        /// <param name="strTableName">Name of the table to check</param>
        /// <param name="functions">IDatabaseFunctions object</param>
        /// <param name="strColumnName">Column name</param>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <returns></returns>
        public static bool CheckDataBaseTableHasColumn(string strTableName, IDatabaseFunctions functions, string strColumnName, ref string strError)
        {
            try
            {
                // Sanitize Table Name
                strTableName = strTableName.Trim();

                // Validation
                if (strTableName == "" || strColumnName == "") { return false; }

                // Create Query
                string strQuery = @"
								SELECT                                    
                                    COUNT(sc.name)
                                FROM 
                                    sysobjects so
                                INNER JOIN syscolumns sc ON sc.id = so.id
                                INNER JOIN systypes st on st.usertype = sc.usertype 
                                WHERE 
                                    so.name         = '" + strTableName + @"'
                                    AND sc.name     = '" + strColumnName + "'";

                // Get Table Column Count
                int intTableColumns = functions.ExecuteScalar<int>(strQuery, ref strError);

                // Validation
                if (intTableColumns == 0 || strError != "") { return false; }

                // Check Table Exists
                bool boolTableExists = intTableColumns > 0;

                return boolTableExists;
            }
            catch (Exception ex)
            {
                strError = ex.ToString();

                return false;
            }
        }

        /// <summary>
        /// Check to ensure a database table has a specific list of columns
        /// </summary>
        /// <param name="strTableName">Name of the table to check</param>
        /// <param name="listColumnNames">Column name list</param>
        /// <param name="functions">IDatabaseFunctions object</param>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <returns></returns>
        public static bool CheckDataBaseTableHasColumnList(string strTableName, List<string> listColumnNames, IDatabaseFunctions functions, ref string strError)
        {
            try
            {
                // Validation
                if (listColumnNames == null || listColumnNames.Count == 0) { return false; }

                foreach (string strColumnName in listColumnNames)
                {
                    // Check Table Exists
                    bool boolTableExists = CheckDataBaseTableHasColumn(strTableName, functions, strColumnName, ref strError);

                    // Validation
                    if (boolTableExists == false || strError != "") { return false; }
                }

                return true;
            }
            catch (Exception ex)
            {
                strError = ex.ToString();

                return false;
            }
        }                

        #endregion
    }
}
