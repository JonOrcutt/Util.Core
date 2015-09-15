using System.Text;
using System.Runtime.InteropServices;

namespace CoreUtil
{
    internal static class PrivateProfileFunctions
    {

        #region Description

        // This Class acts as a library of Private Profile Helper Functions.

        #endregion


        #region Properties

        #endregion

        #region Functions

        [DllImport("KERNEL32.DLL", EntryPoint = "GetPrivateProfileStringW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, string lpReturnString, int nSize, string lpFilename);

        [DllImport("KERNEL32.DLL", EntryPoint = "GetPrivateProfileString")]
        internal static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);

        [DllImport("KERNEL32.DLL", EntryPoint = "GetPrivateProfileSection")]
        internal static extern int GetPrivateProfileSection(string lpAppName, byte[] lpReturnedString, int nSize, string lpFileName);

        [DllImport("KERNEL32.DLL", EntryPoint = "GetPrivateProfileSectionNames")]
        internal static extern int GetPrivateProfileSectionNames(byte[] lpReturnedString, int nSize, string lpFileName);

        #endregion
    }
}