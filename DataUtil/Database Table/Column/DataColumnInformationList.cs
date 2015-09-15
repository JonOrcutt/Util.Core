using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DataUtil
{
    /// <summary>
    /// Contains a list of DataColumnInformation objects
    /// </summary>
    public sealed class DataColumnInformationList : List<DataColumnInformation>
    {

        #region Properties

        #endregion

        #region Initialization

        /// <summary>
        /// Default constructor
        /// </summary>
        public DataColumnInformationList()
        { 
        }

        /// <summary>
        /// DataRow constructor
        /// </summary>
        /// <param name="dr">DataRow to build column list from</param>
        public DataColumnInformationList(DataRow dr)
        {
            // Create New DataTableInformation
            DataTableInformation dataTableInformation = new DataTableInformation(dr.Table);

            // Add Columns To List
            this.AddRange(dataTableInformation.Columns.ToArray());
        }


        /// <summary>
        /// DataTable constructor
        /// </summary>
        /// <param name="dt">DataTable to build column list from</param>
        public DataColumnInformationList(DataTable dt)
        {
            // Create New DataTableInformation
            DataTableInformation dataTableInformation = new DataTableInformation(dt);

            // Add Columns To List
            this.AddRange(dataTableInformation.Columns.ToArray());
        }

        /// <summary>
        /// Database Constructor
        /// </summary>
        /// <param name="strTableName">Database table name</param>
        /// <param name="functions">IDatabaseFunctions object</param>
        public DataColumnInformationList(string strTableName, IDatabaseFunctions functions)
        {
            // Create New DataTableInformation
            DataTableInformation dataTableInformation = new DataTableInformation(strTableName, functions);

            // Add Columns To List
            this.AddRange(dataTableInformation.Columns.ToArray());
        }

        #endregion

    }
}
