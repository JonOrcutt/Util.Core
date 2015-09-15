using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace CoreUtil.Extensions
{
    /// <summary>
    /// This class provides extension methods for data rows
    /// </summary>
    public static class DataRowExtensions
    {
        #region As Enumerable

        /// <summary>
        /// Get data row list
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public static List<DataRow> ToList(this DataRowCollection rows)
        {
            List<DataRow> listRows = new List<DataRow>();

            // Loop Rows
            foreach (DataRow row in rows)
            {
                // Add Row To List
                listRows.Add(row);
            }

            return listRows;
        }

        /// <summary>
        /// Get datarow as a list of strings
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="strNullValueReplacement"></param>
        /// <returns></returns>
        public static List<string> ToStringList(this DataRow dr, string strNullValueReplacement = "")
        {
            // Get Row Value List As String List
            List<string> listValues = dr.ItemArray
                .Select(item => (item != null && item != DBNull.Value) ? item.ToString() : strNullValueReplacement).ToList();

            return listValues;
        }

        #endregion

        #region Mutable Functions

        /// <summary>
        /// Whether or not the datarow is mutable
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static bool IsMutable(this DataRow dr)
        {
            // Check Item Array Is ReadOnly
            bool boolIsMutable = dr.ItemArray.IsReadOnly == true;

            return boolIsMutable;
        }

        #endregion


        #region Functions Causing Ambiguity Issues

        #region Transform Functions

        //public static string ToString(this DataRow dr, string strDelimiter, string strDefaultNullValue)
        //{
        //    //Validation - Ensure DataRow Content Exists Or Is Mutable
        //    if (dr.IsMutable() == false) { return ""; }

        //    //Set Default Row Null Values
        //    Globals.ResultType resultType = dr.SetNullValues(strDefaultNullValue);
        //    if (resultType == Globals.ResultType.Failure) { return ""; }

        //    //Get DataRow Content As Delimited String
        //    string strValue = String.Join(strDelimiter, dr.ItemArray.Select(obj => obj.ToString()).ToArray());

        //    return strValue;
        //}

        #endregion

        #region Column

        //internal static List<DataColumn> Columns(this DataRow dr)
        //{
        //    // Get DataTable Columns
        //    List<DataColumn> listColumns = dr.Table.Columns();

        //    return listColumns;
        //}

        //internal static List<string> ColumnNames(this DataRow dr)
        //{

        //    // Get DataTable Column Names
        //    List<string> listColumnNames = dr.Table.ColumnNames();

        //    return listColumnNames;
        //}

        #endregion

        #region Null Functions

        //public static int NullValueCount(this DataRow dr)
        //{
        //    // Get Row Null Value Count
        //    int intNullCount = dr.ItemArray
        //        .Where(item => item == null && item == DBNull.Value).Count();

        //    return intNullCount;
        //}

        //public static List<string> NullValueColumnNames(this DataRow dr)
        //{
        //    // Get Column Names Containing Null Values
        //    List<string> listColumnNames = dr.ItemArray
        //        .Where(item => item == null && item == DBNull.Value)
        //        .Select((item, i) => dr.Table.Columns[i].ColumnName).ToList();

        //    return listColumnNames;
        //}

        //public static Globals.ResultType SetNullValues(this DataRow dr, object objDefaultValue)
        //{
        //    // Validation - Ensure DataRow Content Exists Or Is Mutable
        //    if (dr.IsMutable() == false) { return Globals.ResultType.Failure; }

        //    // Reset Null Values With Specified Default Value
        //    dr.ItemArray = dr.ItemArray.Select(obj => (obj == null || obj == DBNull.Value) ? objDefaultValue : obj).ToArray();

        //    return Globals.ResultType.Success;
        //}

        #endregion

        #region Primary Keys

        //internal static DataColumn[] PrimaryKeys(this DataRow dr, ref string strError)
        //{
        //    // Get Primary Keys
        //    DataColumn[] listPrimaryKeys = dr.Table.PrimaryKeys();

        //    return listPrimaryKeys;
        //}

        #endregion


        #endregion
    }
}
