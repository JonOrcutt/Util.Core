using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreUtil
{
    /// <summary>
    /// Global information class
    /// </summary>
    public static class Globals
    {

        #region Properties

        /// <summary>
        /// Environment to be deployed to
        /// </summary>
        public static EnvironmentTypes EnvironmentType = EnvironmentTypes.Unknown;
                
        /// <summary>
        /// A generic result type on a processing expression, transaction, or method
        /// </summary>
        public enum ResultType : int
        {
            /// <summary>
            /// Result type is unknown
            /// </summary>
            Unknown = 0,

            /// <summary>
            /// Represents a successful expression return flag
            /// </summary>
            Success = 1,

            /// <summary>
            /// Represents a failed expression return flag
            /// </summary>
            Failure = 2
        }

        /// <summary>
        /// The type of environment to be deployed to 
        /// </summary>
        public enum EnvironmentTypes : int
        {
            /// <summary>
            /// The environment type is unknown
            /// </summary>
            Unknown = 0,

            /// <summary>
            /// The development environment
            /// </summary>
            DEV = 1,

            /// <summary>
            /// Represents the development environment
            /// </summary>
            DEV1 = 2,

            /// <summary>
            /// Represents the quality environment
            /// </summary>
            QUAL = 3,

            /// <summary>
            /// Represents the quality 1 environment
            /// </summary>
            QUAL1 = 4,

            /// <summary>
            /// Represents the user acceptance testing environment
            /// </summary>
            UAT = 5,

            /// <summary>
            /// Represents the production environment
            /// </summary>
            PROD = 6
        }

        #endregion        
    }
}