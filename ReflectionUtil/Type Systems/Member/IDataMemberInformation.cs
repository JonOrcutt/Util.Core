using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ReflectionUtil
{
    interface IDataMemberInformation : IMemberReflectionInformation, IMemberInformation, IColumnInformation
    {
        #region Properties

        /// <summary>
        /// Member Sanitized Name (Without Prefix)
        /// </summary>
        new string Name { get; }

        /// <summary>
        /// Member Name
        /// </summary>
        new string FullName { get; }

        /// <summary>
        /// Member Type
        /// </summary>
        new Type DataType { get; }

        /// <summary>
        /// Data Column Name
        /// </summary>
        new string ColumnName { get; }

        /// <summary>
        /// Column DataType
        /// </summary>
        new SqlDbType SqlDataType { get; }

        /// <summary>
        /// Max Length of the column value
        /// </summary>
        new int FieldLength { get; }

        /// <summary>
        /// Max Length of the column value
        /// </summary>
        string FieldLengthErrorMessage { get; }

        /// <summary>
        /// Flag for whether or not this field can have a null value
        /// </summary>
        new bool Nullable { get; }

        /// <summary>
        /// Flag for whether or not this field can have a null value
        /// </summary>
        new bool IsPrimaryKey { get; }

        #endregion
    }
}
