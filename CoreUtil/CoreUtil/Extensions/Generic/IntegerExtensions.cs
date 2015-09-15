using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreUtil.Extensions
{
    /// <summary>
    /// This class provides extension methods for integers
    /// </summary>
    public static class IntegerExtensions
    { 
        #region Validation

        /// <summary>
        /// Determine if an integer is an even number
        /// </summary>
        /// <param name="intValue"></param>
        /// <returns></returns>
        public static bool IsEven(this int intValue)
        {
            return ((intValue & 1) == 0);
        }

        /// <summary>
        /// Determine if an integer is an odd number
        /// </summary>
        /// <param name="intValue"></param>
        /// <returns></returns>
        public static bool IsOdd(this int intValue)
        {
            return ((intValue & 1) == 1);
        }
        
        #endregion

        #region Formatting

        /// <summary>
        /// Format value as string with a provided format type
        /// </summary>
        /// <param name="intValue"></param>
        /// <param name="formatType"></param>
        /// <param name="intDigits"></param>
        /// <returns></returns>
        public static string ToFormatType(this int intValue, FormatInformation.FormatType formatType, int intDigits = 0)
        {
            string strValueFormatted = "";

            switch (formatType)
            {
                case FormatInformation.FormatType.Currency:
                    strValueFormatted = (intDigits > 0) ? string.Format("C" + intDigits.ToString(), intValue) : string.Format("C", intValue);
                    break;
                case FormatInformation.FormatType.FixedPoint:
                    strValueFormatted = (intDigits > 0) ? string.Format("F" + intDigits.ToString(), intValue) : string.Format("F", intValue);
                    break;
                case FormatInformation.FormatType.FixedPointGroup:
                    strValueFormatted = (intDigits > 0) ? string.Format("N" + intDigits.ToString(), intValue) : string.Format("N", intValue);
                    break;
                case FormatInformation.FormatType.General:
                    strValueFormatted = (intDigits > 0) ? string.Format("G" + intDigits.ToString(), intValue) : string.Format("G", intValue);
                    break;
                case FormatInformation.FormatType.Hexadecimal:
                    strValueFormatted = (intDigits > 0) ? string.Format("X" + intDigits.ToString(), intValue) : string.Format("X", intValue);
                    break;
                case FormatInformation.FormatType.PadLeadingZeroes:
                    strValueFormatted = (intDigits > 0) ? string.Format("D" + intDigits.ToString(), intValue) : string.Format("D", intValue);
                    break;
                case FormatInformation.FormatType.Percent:
                    strValueFormatted = (intDigits > 0) ? string.Format("P" + intDigits.ToString(), intValue) : string.Format("P", intValue);
                    break;
            }

            return intValue.ToString();
        }

        #endregion
    }
}
