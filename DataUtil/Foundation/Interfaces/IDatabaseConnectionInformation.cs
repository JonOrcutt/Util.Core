using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreUtil;

namespace DataUtil
{
    public interface IDatabaseConnectionInformation
    {

        #region Properties

        string GetConnectionString();

        #endregion
    }
}
