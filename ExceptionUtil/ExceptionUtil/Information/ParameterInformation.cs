using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ExceptionUtil
{
    public class ParameterInformation
    {
        #region Properties

        private string m_Name = "";
        public string Name
        {
            get
            {
                return this.m_Name;
            }
        }

        private string m_Type = "";
        public string Type
        {
            get
            {
                return this.m_Type;
            }
        }

        #endregion

        #region Initialization

        public ParameterInformation(string strName, string strType)
        {
            this.m_Name = strName;
            this.m_Type = strType;
        }

        #endregion
    }
}