using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DataUtil
{
    /// <summary>
    /// Contains DataColumn information
    /// </summary>
    public sealed class DataColumnInformation
    {

        #region Properties

        private string m_ColumnName;
        /// <summary>
        /// Data Column Name
        /// </summary>
        public string ColumnName
        {
            get
            {
                return this.m_ColumnName;
            }
        }

        private SqlDbType m_DataType;
        /// <summary>
        /// Column DataType
        /// </summary>
        public SqlDbType DataType
        {
            get
            {
                return this.m_DataType;
            }
        }

        private int m_FieldLength;
        /// <summary>
        /// Max Length of the column value
        /// </summary>
        public int FieldLength
        {
            get
            {
                return this.m_FieldLength;
            }
        }

        private bool m_Nullable;
        /// <summary>
        /// Flag for whether or not this field can have a null value
        /// </summary>
        public bool Nullable
        {
            get
            {
                return this.m_Nullable;
            }
        }

        /// <summary>
        /// Get this columns create table entry string
        /// </summary>
        public string CreateTableColumnEntry
        {
            get
            {
                string strColumnEntry = "\t" + this.ColumnName + " " + this.DataType.ToString().ToLower();

                if (this.m_DataType == SqlDbType.Char || this.m_DataType == SqlDbType.NChar || this.m_DataType == SqlDbType.NText 
                    || this.m_DataType == SqlDbType.NVarChar || this.m_DataType == SqlDbType.Text || this.m_DataType == SqlDbType.VarChar 
                    || this.m_DataType == SqlDbType.Xml)
                {
                    strColumnEntry += "(" + this.FieldLength + ")";
                }

                strColumnEntry += (this.m_Nullable == true) ? " NULL" : "NOT NULL";

                return strColumnEntry;
            }
        }

        #endregion 

        #region Initialization

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="strColumnName">Name of the column</param>
        /// <param name="dataType">Column data type</param>
        /// <param name="intFieldLength">Maximum length of the field</param>
        /// <param name="boolNullable">Whether column value is nullable</param>
        public DataColumnInformation(string strColumnName, SqlDbType dataType, int intFieldLength, bool boolNullable = true)
        {
            this.m_ColumnName = strColumnName;
            this.m_DataType = dataType;
            this.m_FieldLength = intFieldLength;
            this.m_Nullable = boolNullable;
        }

        #endregion

        #region Functions

        #endregion
    }
}
