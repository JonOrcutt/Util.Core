using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReflectionUtil
{
    internal interface IClassTypeInformation
    {
        #region Properties

        #region Type

        Type Type { get; }

        #endregion

        #region Type Members

        List<MemberTypeInformation> Members { get; }

        List<ClassTypeInformation> InstanceClasses { get; }

        List<ClassTypeInformation> CollectionClasses { get; }

        #endregion

        #endregion
    }
}
