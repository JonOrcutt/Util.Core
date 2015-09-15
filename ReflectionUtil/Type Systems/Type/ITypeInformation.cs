using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReflectionUtil
{
    interface ITypeInformation
    {
        #region Properties

        #region Type

        Type Type { get; }

        #endregion

        #region Members / Relations

        List<Type> InstanceTypes { get; }

        List<Type> CollectionTypes { get; }

        List<Type> ChildTypes { get; }

        List<Type> ParentTypes { get; }

        #endregion

        #endregion
    }
}
