using System;
using System.Text;

namespace CoreUtil
{
    /// <summary>
    /// This class provides ini file helper methods
    /// </summary>
    public static class IniFileFunctions
    {
        #region Read Ini File

        /// <summary>
        /// Read an INI file value
        /// </summary>
        /// <param name="section"></param>
        /// <param name="keyName"></param>
        /// <param name="defaultValue"></param>
        /// <param name="fileName"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        public static string ReadIniValue(string section, string keyName, string defaultValue, string fileName, ref string strError)
        {
            char[] strNullChar = { '\0' };                 //Null char to trim from the returned string
            string strIniString = new String('\0', 80);    //Buffer to use for the API call

            try
            {
                // Read the key
                PrivateProfileFunctions.GetPrivateProfileString(section, keyName, defaultValue, strIniString, strIniString.Length, fileName);

                // Trim any trailing NULLs that may lurking around
                string strValue = strIniString.TrimEnd(strNullChar);

                return strValue;
            }
            catch (Exception ex)
            {
                strError = ex.ToString();

                return "";
            }
        }

        /// <summary>
        /// Read an INI file value
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string ReadIniValue(string filename, String section, String key)
        {
            StringBuilder buffer = new StringBuilder(256);
            string sDefault = "";

            try
            {
                if (PrivateProfileFunctions.GetPrivateProfileString(section, key, sDefault, buffer, buffer.Capacity, filename) != 0)
                {
                    return buffer.ToString();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                return "";
            }
        }
        
        /// <summary>
        /// Get an INI file parameter
        /// </summary>
        /// <param name="pstrSection"></param>
        /// <param name="pstrEntry"></param>
        /// <param name="pstrIniFile"></param>
        /// <returns></returns>
        public static string GetIniParam(string pstrSection, string pstrEntry, string pstrIniFile)
        {
            string strResult = ReadIniValue(pstrIniFile, pstrSection, pstrEntry);

            return strResult;
        }

        #endregion     
    }
}