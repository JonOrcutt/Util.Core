using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ReflectionUtil
{
    /// <summary>
    /// Static class for retrieving Database related information
    /// </summary>
    internal static class DatabaseInformation
    {
        #region Properties
        
        private static Dictionary<SqlDbType, Type> m_DataTypeMappings = null;
        /// <summary>
        /// Dictionary of mappings between SqlDbTypes and .NET Types
        /// </summary>
        internal static Dictionary<SqlDbType, Type> DataTypeMappings
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
        internal static Type GetTypeFromSqlType(SqlDbType sqltype)
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
        internal static SqlDbType GetSqlTypeFromType(Type type)
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
        internal static SqlDbType GetSqlTypeFromString(string strType)
        {
            SqlDbType resultType = SqlDbType.Char;

            // Get Type Value
            resultType = DataTypeMappings
                .Where(pair => pair.Key.ToString().ToLower() == strType.ToLower())
                .Select(pair => pair.Key).FirstOrDefault();

            return resultType;
        }

        #endregion
    }
}
