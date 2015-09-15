using System;
using System.Collections.Generic;
using System.Text;

namespace CoreUtil.Extensions
{
    /// <summary>
    /// This class provides file formatting information
    /// </summary>
    public class FormatInformation
    {
        #region Properties

        /// <summary>
        /// Format type enumeration
        /// </summary>
        public enum FormatType : int
        {
            /// <summary>
            /// The format type is unknown
            /// </summary>
            Unknown = 0,

            /// <summary>
            /// General format type
            /// </summary>            
            General = 1,

            /// <summary>
            /// Fixed point format type
            /// </summary>                      
            FixedPoint = 2,

            /// <summary>
            /// fixed point group format type
            /// </summary>
            FixedPointGroup = 3,

            /// <summary>
            /// Padded leading zeroes format type
            /// </summary>
            PadLeadingZeroes = 4,

            /// <summary>
            /// Currency format type
            /// </summary>
            Currency = 5,

            /// <summary>
            /// Percentage format type
            /// </summary>
            Percent = 6,

            /// <summary>
            /// Hexadecimal format type
            /// </summary>
            Hexadecimal = 7
        }

        #endregion
    }
}
