using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;
using System.Reflection;
using CoreUtil;

namespace ReflectionUtil
{
    public class MemberTypeInformation : IDataMemberInformation, IMemberReflectionInformation, IMemberInformation, IColumnInformation
    {
        #region Properties

        #region Reflection

        private MemberInfo m_MemberInfo;
        /// <summary>
        /// MemberInfo
        /// </summary>
        /// <returns></returns>
        public MemberInfo MemberInfo
        {
            get
            {
                return this.m_MemberInfo;
            }
        }

        private FieldInfo m_FieldInfo;
        /// <summary>
        /// FieldInfo
        /// </summary>
        /// <returns></returns>
        public FieldInfo FieldInfo
        {
            get
            {
                return this.m_FieldInfo;
            }
        }

        private PropertyInfo m_PropertyInfo;
        /// <summary>
        /// PropertyInfo
        /// </summary>
        /// <returns></returns>
        public PropertyInfo PropertyInfo
        {
            get
            {
                return this.m_PropertyInfo;
            }
        }

        private List<Attribute> m_Attributes;
        /// <summary>
        /// Member Attributes
        /// </summary>
        /// <returns></returns>
        public List<Attribute> Attributes
        {
            get
            {
                return this.m_Attributes;
            }
        }

        private string m_FullName;
        /// <summary>
        /// Member Name
        /// </summary>
        public string FullName
        {
            get
            {
                return this.m_FullName;
            }
        }

        private string m_Name = "";
        /// <summary>
        /// Member Sanitized Name (Without Prefix)
        /// </summary>
        public string Name
        {
            get
            {
                if (this.m_Name != "") { return this.m_Name; }

                // Check Name Contains Prefix
                bool boolContainsPrefix = this.m_FullName.Length >= 2 && this.m_FullName.Substring(0, 2).Contains("_") == true;

                // If Has Prefix
                if (boolContainsPrefix == true)
                {
                    this.m_Name = this.m_FullName.Remove(0, this.m_FullName.IndexOf('_') + 1);
                }

                return this.m_Name;
            }
        }

        private Type m_DataType;
        /// <summary>
        /// Member Type
        /// </summary>
        public Type DataType
        {
            get
            {
                return this.m_DataType;
            }
        }

        #endregion

        #region Column

        private bool m_ColumnExists = false;
        /// <summary>
        /// Flag for whether or not this datacolumn exists
        /// </summary>
        public bool ColumnExists
        {
            get
            {
                return this.ColumnName != null && CacheRepository.DataTable.Columns.Contains(this.ColumnName);
            }
            set
            {
                this.m_ColumnExists = value;
            }
        }

        private string m_ColumnName = "";
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

        private SqlDbType m_SqlDataType;
        /// <summary>
        /// Column DataType
        /// </summary>
        public SqlDbType SqlDataType
        {
            get
            {
                return this.m_SqlDataType;
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

        private string m_FieldLengthErrorMessage = "";
        /// <summary>
        /// Max Length of the column value
        /// </summary>
        public string FieldLengthErrorMessage
        {
            get
            {
                return this.m_FieldLengthErrorMessage;
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

        private bool m_IsPrimaryKey;
        /// <summary>
        /// Flag for whether or not this field can have a null value
        /// </summary>
        public bool IsPrimaryKey
        {
            get
            {
                return this.m_IsPrimaryKey;
            }
        }

        #endregion

        #region Data

        private DataTable m_Data;
        public DataTable Data
        {
            get
            {
                if (this.m_Data == null && this.ColumnName != "" && CacheRepository.DataTable != null)
                {
                    this.m_Data = CacheRepository.ClassTypeList
                        .Where(cls => cls.Type == this.DataType.DeclaringType)
                        .FirstOrDefault()
                        .DataTable
                        .AsDataView()
                        .ToTable(true, this.ColumnName);
                }

                return this.m_Data;
            }
        }

        #endregion

        #endregion

        #region Initialization

        /// <summary>
        /// 
        /// </summary>
        internal MemberTypeInformation()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <param name="bindingFlags"></param>
        internal MemberTypeInformation(FieldInfo fieldInfo, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            this.m_FieldInfo = fieldInfo;
            this.m_MemberInfo = (MemberInfo)fieldInfo;

            // Load Field Information
            this.LoadDataMemberInformation(this.m_MemberInfo, fieldInfo.FieldType, bindingFlags);            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="bindingFlags"></param>
        internal MemberTypeInformation(PropertyInfo propertyInfo, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            this.m_PropertyInfo = propertyInfo;
            this.m_MemberInfo = (MemberInfo)propertyInfo;

            // Load Property Information
            this.LoadDataMemberInformation(this.m_MemberInfo, propertyInfo.PropertyType, bindingFlags);
        }

        /// <summary>
        /// Default constructor for basic MemberInfo
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="memberDataType"></param>
        /// <param name="bindingFlags"></param>
        internal MemberTypeInformation(MemberInfo memberInfo, Type memberDataType, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            this.m_MemberInfo = memberInfo;

            // Load Property Information
            this.LoadDataMemberInformation(this.m_MemberInfo, memberDataType, bindingFlags);
        }
        
        #endregion

        #region Functions

        private Globals.ResultType LoadDataMemberInformation(MemberInfo memberInfo, Type dataType, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            try
            {
                // Get Custom Attributes
                this.m_Attributes = memberInfo.GetCustomAttributes(false)
                    .Select(attribute => (Attribute)attribute).ToList();

                // Get First Column Attribute
                ColumnAttribute columnAttribute = (ColumnAttribute)this.Attributes
                    .Where(attribute => attribute.GetType() == typeof(ColumnAttribute)).FirstOrDefault();

                // Validation
                if (columnAttribute == null) { return Globals.ResultType.Failure; }

                // Member Full Name
                this.m_FullName = memberInfo.Name;

                // Column Name
                this.m_ColumnName = columnAttribute.Name;

                // DataType
                this.m_DataType = dataType;

                // Sql DataType
                this.m_SqlDataType = DatabaseInformation.GetSqlTypeFromString(columnAttribute.DbType);

                // Nullable
                this.m_Nullable = columnAttribute.CanBeNull;

                // Is Primary Key
                this.m_IsPrimaryKey = columnAttribute.IsPrimaryKey;

                // Get First String Length Attribute
                StringLengthAttribute stringLengthAttribute = (StringLengthAttribute)this.Attributes
                    .Where(attribute => attribute.GetType() == typeof(StringLengthAttribute)).FirstOrDefault();

                // Validation
                if (stringLengthAttribute == null) { return Globals.ResultType.Success; }

                // Max Length
                this.m_FieldLength = stringLengthAttribute.MaximumLength;

                // Max Length Error Message
                this.m_FieldLengthErrorMessage = stringLengthAttribute.ErrorMessage;

                return Globals.ResultType.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                return Globals.ResultType.Failure;
            }
        }

        #endregion

        #region Other
        
        internal string GetQueryFilterString(object objValue)
        {
            string strValue = (objValue == null || objValue == DBNull.Value) ? "" : objValue.ToString();

            if (this.DataType == typeof(char) || this.DataType == typeof(string))
            {
                return this.ColumnName + "=" + "'" + strValue.Replace("'", "''") + "'";
            }
            else if (this.m_DataType == typeof(DateTime))
            {
                return this.ColumnName + "=" + "#" + strValue.Replace("'", "''") + "#";
            }
            else
            {
                return this.ColumnName + "=" + strValue;
            }            
        }

        #endregion
    }
}
