using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReflectionUtil
{
    internal static class ReflectionInformation
    {
        #region Properties

        internal enum ClassMemberType
        {
            Unknown = 0,
            Field = 1,
            Property = 2,
            Method = 3
        }

        internal enum CollectionType : int
        {
            Unknown = 0,
            List = 1,
            Array = 2,
            IEnumerable = 3,
            Dictionary = 4
        }

        #endregion
    }
}
