//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Reflection;
//using System.Data;

//namespace ReflectionUtil
//{
//    interface IMemberTypeInformation
//    {
//        #region Properties

//        #region Reflection

//        /// <summary>
//        /// Member Name
//        /// </summary>
//        string MemberFullName { get; }

//        /// <summary>
//        /// Member Sanitized Name (Without Prefix)
//        /// </summary>
//        string MemberName { get; }

//        /// <summary>
//        /// Member Type
//        /// </summary>
//        Type MemberDataType { get; }

//        #endregion

//        #region Column

//        /// <summary>
//        /// Data Column Name
//        /// </summary>
//        string ColumnName { get; }

//        /// <summary>
//        /// Column DataType
//        /// </summary>
//        SqlDbType SqlDataType { get; }

//        /// <summary>
//        /// Max Length of the column value
//        /// </summary>
//        int FieldLength { get; }

//        /// <summary>
//        /// Max Length of the column value
//        /// </summary>
//        string FieldLengthErrorMessage { get; }

//        /// <summary>
//        /// Flag for whether or not this field can have a null value
//        /// </summary>
//        bool Nullable { get; }

//        /// <summary>
//        /// Flag for whether or not this field can have a null value
//        /// </summary>
//        bool IsPrimaryKey { get; }

//        #endregion

//        #region Data

//        DataTable Data { get; }

//        #endregion

//        #endregion
//    }
//}
