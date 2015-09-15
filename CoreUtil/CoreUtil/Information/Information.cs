using System;
using System.Collections.Generic;
using System.Text;

namespace CoreUtil.Information
{
    /// <summary>
    /// 
    /// </summary>
    public static class Information
    {
        #region Properties

        #region Machine

        /// <summary>
        /// The name of the machine executing the current assembly
        /// </summary>
        public static string MachineName
        {
            get
            {
                return Environment.MachineName;
            }
        }

        /// <summary>
        /// The name of th user executing the current assembly
        /// </summary>
        public static string UserName
        {
            get
            {
                return Environment.UserDomainName;
            }
        }

        /// <summary>
        /// The operating system version
        /// </summary>
        public static string OSVersion
        {
            get
            {
                return Environment.OSVersion.ToString();
            }
        }

        /// <summary>
        /// The current .NET framework version on the machine executing the current assembly
        /// </summary>
        public static string NETFrameworkVersion
        {
            get
            {
                return System.Runtime.InteropServices.RuntimeEnvironment.GetSystemVersion().ToString();
            }
        }

        #endregion

        #region Program

        /// <summary>
        /// The name of the program executing the current assembly
        /// </summary>
        public static string ProgramName
        {
            get
            {
                return System.Reflection.Assembly.GetEntryAssembly().FullName.Split(',')[0];
            }
        }

        /// <summary>
        /// The version of the program executing the current assembly
        /// </summary>
        public static string ProgramVersion
        {
            get
            {
                return System.Reflection.Assembly.GetEntryAssembly().FullName.Split(',')[1].ToLower().Replace("version=", "").Trim();
            }
        }

        /// <summary>
        /// The base directory of the currently executing program
        /// </summary>
        public static string ProgramDirectory
        {
            get
            {
                return Environment.CurrentDirectory;
            }
        }

        #endregion

        #endregion
    }
}
