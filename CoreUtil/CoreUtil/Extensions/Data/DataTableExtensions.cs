using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace CoreUtil.Extensions
{
    /// <summary>
    /// This class provides extension methods for data tables
    /// </summary>
    public static class DataTableExtensions
    {

        #region Fragmentation

        /// <summary>
        /// Fragment table to a list of rows based on a specific list of column names
        /// </summary>
        /// <param name="dt">Datatable to fragment</param>
        /// <param name="listColumnNamesConformTo">Column names to fragment with</param>
        /// <param name="boolDistinct">Whether or not the row list returned should be distinct</param>
        /// <returns></returns>
        public static List<DataRow> FragmentToRowList(this DataTable dt, List<string> listColumnNamesConformTo, bool boolDistinct = false)
        {
            // Retrieve Distinct Data Based On Column Names
            List<DataRow> listRows = dt
                .DefaultView
                .ToTable(boolDistinct, listColumnNamesConformTo.ToArray())
                .AsEnumerable()
                .ToList();

            return listRows;
        }

        /// <summary>
        /// Fragment a datatable from a list of column names
        /// </summary>
        /// <param name="dt">Datatable to fragment</param>
        /// <param name="listColumnNamesConformTo">Column names to fragment with</param>
        /// <param name="boolDisctinct">Whether or not the row list returned should be distinct</param>
        /// <returns></returns>
        public static DataTable FragmentToTable(this DataTable dt, List<string> listColumnNamesConformTo, bool boolDisctinct = false)
        {
            // Retrieve Distinct Data Based On Column Names
            DataTable newDataTable = dt
                .DefaultView
                .ToTable(boolDisctinct, listColumnNamesConformTo.ToArray());

            return newDataTable;
        }


        #endregion

        #region Column

        /// <summary>
        /// Get datatable rows as an enumerable list
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<DataRow> Rows(this DataTable dt)
        {
            List<DataRow> listRows = dt.AsEnumerable().ToList();

            return listRows;
        }

        #endregion

        #region Mutable

        /// <summary>
        /// Check whether or not a datatable is mutable
        /// </summary>
        /// <param name="dt">Datatable to check</param>
        /// <param name="boolEnsureNotReadOnly">Ensure table is not readonly</param>
        /// <returns></returns>
        public static bool IsMutable(this DataTable dt, bool boolEnsureNotReadOnly = true)
        {
            if (dt.Rows.Count > 0 && (dt.Rows[0].ItemArray != null || dt.Rows[0].ItemArray.IsReadOnly == true))
            {
                return false;
            }

            return true;
        }

        #endregion


        #region Functions Causing Ambiguity Issues
        
        #region Column

        //internal static List<DataColumn> Columns(this DataTable dt)
        //{
        //    List<DataColumn> listColumnNames = dt.Columns.ToList();

        //    return listColumnNames;
        //}

        //internal static List<string> ColumnNames(this DataTable dt)
        //{
        //    List<string> listColumnNames = dt.Columns()
        //        .Select(column => column.ColumnName).ToList();

        //    return listColumnNames;
        //}

        //internal static List<List<string>> RowsToStringList(this DataTable dt, string strDefaultNullValue = "")
        //{
        //    List<List<string>> listRowLists = dt.Rows.ToList()
        //        .Select(row => row.ToStringList(strDefaultNullValue)).ToList();

        //    return listRowLists;
        //}

        #endregion

        #region Primary Keys

        //internal static DataColumn[] PrimaryKeys(this DataTable dt)
        //{
        //    // Validation - Ensure DataTable Content Exists Or Is Mutable
        //    if (dt.IsMutable(false) == false) { return null; }

        //    DataColumn[] listPrimaryKeys = dt.PrimaryKey;

        //    return listPrimaryKeys;
        //}

        #endregion

        //public static List<string> FragmentToColumnValueList(this DataTable dt, string strColumnName, string strDefaultNullValue = "")
        //{
        //    // Validation
        //    if (dt.Columns.Contains(strColumnName) == false) { return null; }

        //    List<string> listValues = dt.Rows.ToList()
        //        .Select(row => (row[strColumnName] != null && row[strColumnName] != DBNull.Value)
        //            ? row[strColumnName].ToString()
        //            : strDefaultNullValue).ToList();

        //    return listValues;
        //}
        #endregion
    }
}
