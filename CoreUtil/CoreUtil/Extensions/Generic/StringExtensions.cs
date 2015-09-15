using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CoreUtil;

namespace CoreUtil.Extensions
{
    /// <summary>
    /// This class provides extension methods for string values
    /// </summary>
    internal static class StringExtensions
    {
        #region Check String

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public static bool IsBoolean(this string strValue)
        {
            var val = strValue.ToLower().Trim();
            List<string> listBooleanValues = new List<string>()
            {
                "false", "f", "n", "no",
                "true", "t", "y", "yes"
            };

            bool boolIsBoolean = listBooleanValues.Contains(val);

            return boolIsBoolean;
        }

        /// <summary>
        /// Check to see if a string can be cast to a date
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public static bool IsDate(this string strValue)
        {
            if (strValue != "")
            {
                DateTime dt;
                return (DateTime.TryParse(strValue, out dt));
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check string is numeric
        /// </summary>
        /// <param name="strValue"></param>
        /// <param name="boolTrimString"></param>
        /// <returns></returns>
        public static bool IsNumeric(this string strValue, bool boolTrimString = true)
        {
            strValue = (boolTrimString == true) ? strValue.Trim() : strValue;
            if (strValue == "") { return false; }

            for (int intIndex = 0; intIndex < strValue.Length; intIndex++)
            {
                bool boolIsDigit = char.IsDigit(strValue[intIndex]);
                if (boolIsDigit == false) { return false; }
            }

            return true;
        }

        /// <summary>
        /// Check string is a letter or digit
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public static bool IsLetterOrDigit(this string strValue)
        {
            if (strValue == "") { return false; }

            for (int intIndex = 0; intIndex < strValue.Length; intIndex++)
            {
                bool boolIsLetter = char.IsLetter(strValue[intIndex]);
                bool boolIsDigit = char.IsDigit(strValue[intIndex]);
                if (boolIsLetter == false || boolIsDigit == false) { return false; }
            }

            return true;
        }

        /// <summary>
        /// Check string is a valid email address
        /// </summary>
        /// <param name="strEmailAddress"></param>
        /// <returns></returns>
        public static bool IsEmail(this string strEmailAddress)
        {
            bool boolIsMatch = Regex.IsMatch(strEmailAddress, RegexConstants.EMAIL);

            return boolIsMatch;
        }

        /// <summary>
        /// Check to see if a string is a GUID formatted string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsGuid(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            const string pattern = @"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$";

            return Regex.IsMatch(value, pattern);
        }

        /// <summary>
        /// Check string is a social security number
        /// </summary>
        /// <param name="strSocialSecurityNumber"></param>
        /// <returns></returns>
        public static bool IsSocialSecurityNumber(this string strSocialSecurityNumber)
        {
            bool boolIsMatch = Regex.IsMatch(strSocialSecurityNumber, RegexConstants.SOCIAL_SECURITY);

            return boolIsMatch;
        }

        /// <summary>
        /// Check string is a valid US zipcode
        /// </summary>
        /// <param name="strZipCode"></param>
        /// <returns></returns>
        public static bool IsUSZipCode(this string strZipCode)
        {
            bool boolIsMatch = Regex.IsMatch(strZipCode, RegexConstants.US_ZIPCODE);

            return boolIsMatch;
        }

        /// <summary>
        /// Check string is a valid US zipcode + 4 numbers
        /// </summary>
        /// <param name="strZipCode"></param>
        /// <returns></returns>
        public static bool IsUSZipCodePlusFour(this string strZipCode)
        {
            bool boolIsMatch = Regex.IsMatch(strZipCode, RegexConstants.US_ZIPCODE_PLUS_FOUR);

            return boolIsMatch;
        }

        #endregion

        #region Find String

        /// <summary>
        /// Check string contains substring
        /// </summary>
        /// <param name="strContent"></param>
        /// <param name="strFind"></param>
        /// <param name="boolMatchCase"></param>
        /// <returns></returns>
        public static bool Contains(this string strContent, string strFind, bool boolMatchCase)
        {
            strContent = (boolMatchCase == true) ? strContent.ToLower() : strContent;
            strFind = (boolMatchCase == true) ? strFind.ToLower() : strFind;

            bool boolContainsValue = strContent.Contains(strFind);

            return boolContainsValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool ContainsAny(this string str, params string[] values)
        {
            if (!string.IsNullOrEmpty(str) || values.Length == 0)
            {
                foreach (string value in values)
                {
                    if (str.Contains(value))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool ContainsAll(this string value, params string[] values)
        {
            foreach (string one in values)
            {
                if (!value.Contains(one))
                {
                    return false;
                }
            }
            return true;
        }
        
        /// <summary>
        /// Check string contains numbers
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public static bool ContainsNumbers(this string strValue)
        {
            if (strValue == "") { return false; }

            for (int intIndex = 0; intIndex < strValue.Length; intIndex++)
            {
                bool boolIsDigit = char.IsDigit(strValue[intIndex]);
                if (boolIsDigit == true) { return true; }
            }

            return false;
        }

        #endregion

        #region Transform

        /// <summary>
        /// Split string
        /// </summary>
        /// <param name="strValue"></param>
        /// <param name="charDelimiter"></param>
        /// <param name="boolTrimValues"></param>
        /// <returns></returns>
        public static string[] Split(this string strValue, char charDelimiter, bool boolTrimValues = false)
        {
            string[] listValues = strValue.Split(charDelimiter);

            listValues = (boolTrimValues == true) ? listValues.Select(value => value.Trim()).ToArray() : listValues;

            return listValues;
        }

        #endregion

        #region Convert

        public static T ToEnum<T>(this string strContent) where T : struct
        {
            System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(strContent));
            return (T)Enum.Parse(typeof(T), strContent, true);
        }

        public static int ToInt(this string strContent, int intDefaultIfNotValid = 0)
        {
            int number;

            int intValue = (Int32.TryParse(strContent, out number) == true) ? int.Parse(strContent) : intDefaultIfNotValid;

            return number;
        }

        public static decimal ToDecimal(this string strContent, int decDefaultIfNotValid = 0)
        {
            decimal number;

            decimal decValue = (Decimal.TryParse(strContent, out number) == true) ? Decimal.Parse(strContent) : decDefaultIfNotValid;

            return number;
        }

        public static byte[] ToBytes(this string strContent)
        {
            byte[] bytes = new byte[strContent.Length * sizeof(char)];
            System.Buffer.BlockCopy(strContent.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static System.IO.MemoryStream ToStream(this string strContent)
        {
            byte[] Bytes = System.Text.Encoding.ASCII.GetBytes(strContent);
            return new System.IO.MemoryStream(Bytes);
        }

        #endregion

        #region Formating

        /// <summary>
        /// Format string to type
        /// </summary>
        /// <param name="strValue"></param>
        /// <param name="formatType"></param>
        /// <param name="intDigits"></param>
        /// <returns></returns>
        public static string ToFormatType(this string strValue, FormatInformation.FormatType formatType, int intDigits = 0)
        {
            string strValueFormatted = "";

            switch (formatType)
            {
                case FormatInformation.FormatType.Currency:
                    strValueFormatted = (intDigits > 0) ? string.Format("C" + intDigits.ToString(), strValue) : string.Format("C", strValue);
                    break;
                case FormatInformation.FormatType.FixedPoint:
                    strValueFormatted = (intDigits > 0) ? string.Format("F" + intDigits.ToString(), strValue) : string.Format("F", strValue);
                    break;
                case FormatInformation.FormatType.FixedPointGroup:
                    strValueFormatted = (intDigits > 0) ? string.Format("N" + intDigits.ToString(), strValue) : string.Format("N", strValue);
                    break;
                case FormatInformation.FormatType.General:
                    strValueFormatted = (intDigits > 0) ? string.Format("G" + intDigits.ToString(), strValue) : string.Format("G", strValue);
                    break;
                case FormatInformation.FormatType.Hexadecimal:
                    strValueFormatted = (intDigits > 0) ? string.Format("X" + intDigits.ToString(), strValue) : string.Format("X", strValue);
                    break;
                case FormatInformation.FormatType.PadLeadingZeroes:
                    strValueFormatted = (intDigits > 0) ? string.Format("D" + intDigits.ToString(), strValue) : string.Format("D", strValue);
                    break;
                case FormatInformation.FormatType.Percent:
                    strValueFormatted = (intDigits > 0) ? string.Format("P" + intDigits.ToString(), strValue) : string.Format("P", strValue);
                    break;
            }

            return strValueFormatted;
        }

        #endregion


        #region Internal Extra Extensions

        /// <summary>
        /// Truncates the string to a specified length and replace the truncated to a ...
        /// </summary>
        /// <param name="text">string that will be truncated</param>
        /// <param name="maxLength">total length of characters to maintain before the truncate happens</param>
        /// <returns>truncated string</returns>
        internal static string Truncate(this string text, int maxLength)
        {
            // replaces the truncated string to a ...
            const string suffix = "...";
            string truncatedString = text;

            if (maxLength <= 0) return truncatedString;
            int strLength = maxLength - suffix.Length;

            if (strLength <= 0) return truncatedString;

            if (text == null || text.Length <= maxLength) return truncatedString;

            truncatedString = text.Substring(0, strLength);
            truncatedString = truncatedString.TrimEnd();
            truncatedString += suffix;
            return truncatedString;
        }

        internal static bool IsNullOrEmptyOrWhiteSpace(this string input)
        {
            return string.IsNullOrEmpty(input) || input.Trim() == string.Empty;
        }

        internal static bool DoesNotStartWith(this string input, string pattern)
        {
            return string.IsNullOrEmpty(pattern) ||
                   string.IsNullOrEmpty(input) ||
                   !input.StartsWith(pattern, StringComparison.CurrentCulture);
        }

        internal static bool DoesNotEndWith(this string input, string pattern)
        {
            return string.IsNullOrEmpty(pattern) ||
                     string.IsNullOrEmpty(input) ||
                     !input.EndsWith(pattern, StringComparison.CurrentCulture);
        }

        internal static List<string> SplitIntoParts(this string input, int partLength)
        {
            var result = new List<string>();
            int partIndex = 0;
            int length = input.Length;
            while (length > 0)
            {
                var tempPartLength = length >= partLength ? partLength : length;
                var part = input.Substring(partIndex * partLength, tempPartLength);
                result.Add(part);
                partIndex++;
                length -= partLength;
            }
            return result;
        }

        /// <summary>
        /// Repeat a string N times
        /// </summary>
        /// <param name="input"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        internal static string Repeat(this string input, int count)
        {
            if (input == null)
            {
                return null;
            }

            var sb = new StringBuilder();

            for (var repeat = 0; repeat < count; repeat++)
            {
                sb.Append(input);
            }

            return sb.ToString();
        }

        internal static string Reverse(this string s)
        {
            char[] c = s.ToCharArray();
            Array.Reverse(c);
            return new string(c);
        }
        
        #endregion
    }
}
