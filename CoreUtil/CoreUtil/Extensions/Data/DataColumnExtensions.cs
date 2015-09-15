using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace CoreUtil.Extensions
{
    /// <summary>
    /// This class provides extension methods for data columns
    /// </summary>
    public static class DataColumnExtensions
    {

        #region As Enumerable

        /// <summary>
        /// Retrieve datatable columns as an enumerable list
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static List<DataColumn> ToList(this DataColumnCollection collection)
        {
            List<DataColumn> listColumns = new List<DataColumn>();

            foreach (DataColumn column in collection)
            {
                listColumns.Add(column);
            }

            return listColumns;
        }

        #endregion

        #region Functions Causing Ambiguity Issues

        #region Retrieval Functions

        //internal static List<DataColumn> ColumnsOfDataType(this DataColumnCollection collection, Type type)
        //{
        //    List<DataColumn> listColumns = collection.ToList()
        //        .Where(column => column.DataType == type).ToList();

        //    return listColumns;
        //}

        //internal static List<DataColumn> NullableColumns(this DataColumnCollection collection)
        //{
        //    List<DataColumn> listColumns = collection.ToList()
        //        .Where(column => column.AllowDBNull == true).ToList();

        //    return listColumns;
        //}

        #endregion

        //internal static List<string> ColumnNames(this DataColumnCollection collection)
        //{
        //    List<string> listColumnNames = collection.ToList()
        //        .Select(column => column.ColumnName).ToList();

        //    return listColumnNames;
        //}
        
        #endregion
    }
}
