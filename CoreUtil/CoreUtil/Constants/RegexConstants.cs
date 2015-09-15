using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CoreUtil
{
    /// <summary>
    /// This class has defined regular expressions for common searches
    /// </summary>
    public static class RegexConstants
    {

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public static readonly string ALPHA = "[^a-zA-Z]";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string ALPHA_NUMERIC = "[^a-zA-Z0-9]";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string ALPHA_NUMERIC_SPACE = @"[^a-zA-Z0-9\s]";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string EMAIL = @"^([0-9a-zA-Z]+[-._+&])*[0-9a-zA-Z]+@([-0-9a-zA-Z]+[.])+[a-zA-Z]{2,6}$";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string GUID = "[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string UPPER_CASE = @"^[A-Z]+$";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string LOWER_CASE = @"^[a-z]+$";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string NUMERIC = "[^0-9]";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string SOCIAL_SECURITY = @"^\d{3}[-]?\d{2}[-]?\d{4}$";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string URL = @"^^(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_=]*)?$";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string US_CURRENCY = @"^\$(([1-9]\d*|([1-9]\d{0,2}(\,\d{3})*))(\.\d{1,2})?|(\.\d{1,2}))$|^\$[0](.00)?$";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string US_TELEPHONE = @"^[01]?[- .]?(\([2-9]\d{2}\)|[2-9]\d{2})[- .]?\d{3}[- .]?\d{4}$";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string US_ZIPCODE = @"^\d{5}$";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string US_ZIPCODE_PLUS_FOUR = @"^\d{5}((-|\s)?\d{4})$";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string US_ZIPCODE_PLUS_FOUR_OPTIONAL = @"^\d{5}((-|\s)?\d{4})?$";

        #endregion

    }
}
