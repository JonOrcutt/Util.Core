using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ReflectionUtil
{
    interface IMemberReflectionInformation
    {
        #region Properties

        /// <summary>
        /// MemberInfo
        /// </summary>
        /// <returns></returns>
        MemberInfo MemberInfo { get; }

        /// <summary>
        /// FieldInfo
        /// </summary>
        /// <returns></returns>
        FieldInfo FieldInfo { get; }

        /// <summary>
        /// PropertyInfo
        /// </summary>
        /// <returns></returns>
        PropertyInfo PropertyInfo { get; }

        /// <summary>
        /// Member Attributes
        /// </summary>
        /// <returns></returns>
        List<Attribute> Attributes { get; }

        #endregion
    }
}
