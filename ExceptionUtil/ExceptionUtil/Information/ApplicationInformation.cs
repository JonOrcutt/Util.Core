using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ExceptionUtil
{
    public class ApplicationInformation
    {
        #region Properties

        private string m_DomainUserName;
        public string DomainUserName
        {
            get
            {
                return this.m_DomainUserName;
            }
        }

        private DateTime m_ErrorDateTime;
        public DateTime ErrorDateTime
        {
            get
            {
                return this.m_ErrorDateTime;
            }
        }

        private TimeSpan m_ApplicationElapsedTime;
        public TimeSpan ApplicationElapsedTime
        {
            get
            {
                return this.m_ApplicationElapsedTime;
            }
        }

        #endregion

        #region Initialization

        public ApplicationInformation()
        {
            this.m_DomainUserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            this.m_ErrorDateTime = DateTime.Now;
            this.m_ApplicationElapsedTime = this.ErrorDateTime - System.Diagnostics.Process.GetCurrentProcess().StartTime;
        }

        #endregion
    }
}