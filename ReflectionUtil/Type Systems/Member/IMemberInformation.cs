using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReflectionUtil
{
    interface IMemberInformation
    {
        #region Propeties

        /// <summary>
        /// Member Sanitized Name (Without Prefix)
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Member Name
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Member Type
        /// </summary>
        Type DataType { get; }

        #endregion
    }
}
