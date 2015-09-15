using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ReflectionUtil
{
    interface IColumnInformation
    {
        #region Properties

        /// <summary>
        /// Data Column Name
        /// </summary>
        string ColumnName { get; }

        /// <summary>
        /// Column DataType
        /// </summary>
        SqlDbType SqlDataType { get; }

        /// <summary>
        /// Member Type
        /// </summary>
        Type DataType { get; }

        /// <summary>
        /// Max Length of the column value
        /// </summary>
        int FieldLength { get; }

        /// <summary>
        /// Flag for whether or not this field can have a null value
        /// </summary>
        bool Nullable { get; }

        /// <summary>
        /// Flag for whether or not this field can have a null value
        /// </summary>
        bool IsPrimaryKey { get; }

        #endregion
    }
}
