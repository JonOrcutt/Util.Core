using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using CoreUtil;

namespace DataUtil
{
    internal static class DataTableFunctions
    {
        #region Description

        // This is Class holds DataTable Methods

        #endregion

        #region Type Function

        internal static Type TypeMappings(SqlDbType sqltype)
        {
            Type resulttype = null;
            Dictionary<SqlDbType, Type> Types = new Dictionary<SqlDbType, Type>();
            Types.Add(SqlDbType.BigInt, typeof(Int64));
            Types.Add(SqlDbType.Binary, typeof(Byte[]));
            Types.Add(SqlDbType.Bit, typeof(Boolean));
            Types.Add(SqlDbType.Char, typeof(String));
            Types.Add(SqlDbType.Date, typeof(DateTime));
            Types.Add(SqlDbType.DateTime, typeof(DateTime));
            Types.Add(SqlDbType.DateTime2, typeof(DateTime));
            Types.Add(SqlDbType.DateTimeOffset, typeof(DateTimeOffset));
            Types.Add(SqlDbType.Decimal, typeof(Decimal));
            Types.Add(SqlDbType.Float, typeof(Double));
            Types.Add(SqlDbType.Image, typeof(Byte[]));
            Types.Add(SqlDbType.Int, typeof(Int32));
            Types.Add(SqlDbType.Money, typeof(Decimal));
            Types.Add(SqlDbType.NChar, typeof(String));
            Types.Add(SqlDbType.NText, typeof(String));
            Types.Add(SqlDbType.NVarChar, typeof(String));
            Types.Add(SqlDbType.Real, typeof(Single));
            Types.Add(SqlDbType.SmallDateTime, typeof(DateTime));
            Types.Add(SqlDbType.SmallInt, typeof(Int16));
            Types.Add(SqlDbType.SmallMoney, typeof(Decimal));
            Types.Add(SqlDbType.Text, typeof(String));
            Types.Add(SqlDbType.Time, typeof(TimeSpan));
            Types.Add(SqlDbType.Timestamp, typeof(Byte[]));
            Types.Add(SqlDbType.TinyInt, typeof(Byte));
            Types.Add(SqlDbType.UniqueIdentifier, typeof(Guid));
            Types.Add(SqlDbType.VarBinary, typeof(Byte[]));
            Types.Add(SqlDbType.VarChar, typeof(String));

            Types.TryGetValue(sqltype, out resulttype);

            return resulttype;
        }

        #endregion

        #region Get Data Functions

        public static string GetDataRowAsString(DataRow dr, string strDelimiter, string strDefaultNullValue, ref string strError)
        {
            // Validation - Ensure DataRow Content Exists Or Is Mutable
            Globals.ResultType ensureMutableResult = CheckDataRowIsMutable(dr, ref strError);
            if (ensureMutableResult == Globals.ResultType.Failure || strError != "") { return ""; }

            // Set Default Row Null Values
            Globals.ResultType resultType = SetDefaultNullDataRowValues(dr, strDefaultNullValue, ref strError);
            if (resultType == Globals.ResultType.Failure || strError != "") { return ""; }

            // Get DataRow Content As Delimited String
            string strValue = String.Join(strDelimiter, dr.ItemArray.Select(obj => obj.ToString()).ToArray());

            return strValue;
        }

        public static string GetDataTableAsString(DataTable dt, char strDelimiter, ref string strError)
        {
            if (dt == null || dt.Rows.Count == 0) { return ""; }

            string strData = "";

            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    foreach (object objValue in dr.ItemArray)
                    {
                        string strValue = (objValue != null && objValue != DBNull.Value) ? objValue.ToString() : "";
                        strValue += strDelimiter;

                        strData += strValue;
                    }

                    strData += "\r\n";
                }
            }
            catch (Exception ex)
            {
                strError = ex.ToString();
            }

            return strData;
        }

        public static List<string> GetDataTableAsStringList(DataTable dt, char strDelimiter, ref string strError)
        {
            if (dt == null || dt.Rows.Count == 0) { return null; }

            List<string> listDataRows = new List<string>();

            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string strRow = "";
                    foreach (object objValue in dr.ItemArray)
                    {
                        string strValue = (objValue != null && objValue != DBNull.Value) ? objValue.ToString() : "";
                        strValue += strDelimiter;

                        strRow += strValue;
                    }

                    strRow += "\r\n";
                    listDataRows.Add(strRow);
                }
            }
            catch (Exception ex)
            {
                strError = ex.ToString();
            }

            return listDataRows;
        }

        public static List<string> GetDataTableColumnAsList(DataTable dt, string strColumnName)
        {
            if (dt == null || dt.Rows.Count == 0 || strColumnName == "") { return null; }

            int intColumnIndex = dt.Columns.IndexOf(strColumnName);
            if (intColumnIndex < 0) { return null; }

            List<string> listValues = new List<string>();

            foreach (DataRow dr in dt.Rows)
            {
                object objValue = dr.ItemArray[intColumnIndex];
                string strValue = (objValue != null && objValue != DBNull.Value) ? objValue.ToString() : "";

                listValues.Add(strValue);
            }

            return listValues;
        }

        #endregion

        #region DataTable Functions

        #region Validation

        public static bool CheckDataTableHasNullValues(DataTable dt, ref string strError)
        {
            // Validation - Ensure DataTable Content Exists Or Is Mutable
            Globals.ResultType ensureMutableResult = CheckDataTableIsMutable(dt, ref strError, false);
            if (ensureMutableResult == Globals.ResultType.Failure || strError != "") { return false; }

            // Get Null DataTable Value Count
            int intNullValueCount = GetDataTableNullValueCount(dt, ref strError);

            bool boolDataTableHasNullValue = intNullValueCount > 0;

            return boolDataTableHasNullValue;
        }
        
        #endregion

        #region Set Default Null Values

        public static Globals.ResultType SetDefaultNullDataTableValues(DataTable dt, object objDefaultValue, ref string strError)
        {
            // Validation - Ensure DataTable Is Not Null And Is Mutable
            Globals.ResultType ensureMutableResult = CheckDataTableIsMutable(dt, ref strError);
            if (ensureMutableResult == Globals.ResultType.Failure || strError != "") { return Globals.ResultType.Failure; }

            // Validation - No Rows - Nothing To Do == Success
            if (dt.Rows.Count == 0) { return Globals.ResultType.Success; }

            try
            {
                // Loop Table Rows
                foreach (DataRow dr in dt.Rows)
                {
                    // Attempt Set Default Values
                    Globals.ResultType resultType = SetDefaultNullDataRowValues(dr, objDefaultValue, ref strError);

                    // Validation
                    if (resultType == Globals.ResultType.Failure || strError != "") { return Globals.ResultType.Failure; }
                }
            }
            catch (Exception ex)
            {
                strError = ex.ToString();

                return Globals.ResultType.Failure;
            }

            return Globals.ResultType.Success;
        }

        public static Globals.ResultType SetDefaultNullDataTableColumnValues(DataTable dt, string strColumnName, object objDefaultValue, ref string strError)
        {
            // Validation - Ensure DataTable Is Not Null And Is Mutable
            Globals.ResultType ensureMutableResult = CheckDataTableIsMutable(dt, ref strError);
            if (ensureMutableResult == Globals.ResultType.Failure || strError != "") { return Globals.ResultType.Failure; }

            // Validation - No Rows - Nothing To Do == Success
            if (dt.Rows.Count == 0) { return Globals.ResultType.Success; }

            // Validation - Check DataTable Has Column
            bool boolTableHasColumn = CheckDataTableHasColumn(dt, strColumnName, ref strError);
            if (boolTableHasColumn == false) { return Globals.ResultType.Failure; }

            try
            {
                // Loop Table Rows
                foreach (DataRow dr in dt.Rows)
                {
                    // Attempt Set Default Values
                    Globals.ResultType resultType = SetDefaultNullDataRowColumnValues(dr, strColumnName, objDefaultValue, ref strError);

                    // Validation
                    if (resultType == Globals.ResultType.Failure || strError != "") { return Globals.ResultType.Failure; }
                }
            }
            catch (Exception ex)
            {
                strError = ex.ToString();

                return Globals.ResultType.Failure;
            }

            return Globals.ResultType.Success;
        }

        public static Globals.ResultType SetDefaultNullDataTableValuesOfType<T>(DataTable dt, object objDefaultValue, ref string strError)
        {
            // Validation - Ensure DataTable Is Not Null And Is Mutable
            Globals.ResultType ensureMutableResult = CheckDataTableIsMutable(dt, ref strError);
            if (ensureMutableResult == Globals.ResultType.Failure || strError != "") { return Globals.ResultType.Failure; }

            try
            {
                // Loop Table Rows
                foreach (DataRow dr in dt.Rows)
                {
                    // Attempt Set Default Values
                    Globals.ResultType resultType = SetDefaultNullDataRowValuesOfType<T>(dr, objDefaultValue, ref strError);

                    // Validation
                    if (resultType == Globals.ResultType.Failure || strError != "") { return Globals.ResultType.Failure; }
                }
            }
            catch (Exception ex)
            {
                strError = ex.ToString();

                return Globals.ResultType.Failure;
            }

            return Globals.ResultType.Success;
        }

        #endregion

        #endregion

        #region DataRow Functions

        #region Validation
        
        public static bool CheckDataRowHasNullValues(DataRow dr, ref string strError)
        {
            // Validation - Ensure DataRow Content Exists Or Is Mutable
            Globals.ResultType ensureMutableResult = CheckDataRowIsMutable(dr, ref strError);
            if (ensureMutableResult == Globals.ResultType.Failure || strError != "") { return false; }

            // Get Null DataRow Value Count
            int intNullValueCount = GetDataRowNullValueCount(dr, ref strError);

            bool boolDataRowHasNullValue = intNullValueCount > 0;

            return boolDataRowHasNullValue;
        }

        #endregion

        #region Set Default Null Values

        public static Globals.ResultType SetDefaultNullDataRowValues(DataRow dr, object objDefaultValue, ref string strError)
        {
            // Validation - Ensure DataRow Content Exists Or Is Mutable
            Globals.ResultType ensureMutableResult = CheckDataRowIsMutable(dr, ref strError);
            if (ensureMutableResult == Globals.ResultType.Failure || strError != "") { return Globals.ResultType.Failure; }

            try
            {
                // Reset Null Values With Specified Default Value
                dr.ItemArray = dr.ItemArray.Select(obj => (obj == null || obj == DBNull.Value) ? objDefaultValue : obj).ToArray();
            }
            catch (Exception ex)
            {
                strError = ex.ToString();

                return Globals.ResultType.Failure;
            }

            return Globals.ResultType.Success;
        }

        public static Globals.ResultType SetDefaultNullDataRowColumnValues(DataRow dr, string strColumnName, object objDefaultValue, ref string strError)
        {
            // Validation - Ensure DataRow Content Exists Or Is Mutable
            Globals.ResultType ensureMutableResult = CheckDataRowIsMutable(dr, ref strError);
            if (ensureMutableResult == Globals.ResultType.Failure || strError != "") { return Globals.ResultType.Failure; }

            // Validation - Check DataRow Has Column
            bool boolTableHasColumn = CheckDataTableHasColumn(dr.Table, strColumnName, ref strError);
            if (boolTableHasColumn == false) { return Globals.ResultType.Failure; }

            try
            {
                // Reset Null Value With Specified Default Value
                dr.ItemArray[dr.Table.Columns.IndexOf(strColumnName)] = objDefaultValue;
            }
            catch (Exception ex)
            {
                strError = ex.ToString();

                return Globals.ResultType.Failure;
            }

            return Globals.ResultType.Success;
        }

        public static Globals.ResultType SetDefaultNullDataRowValuesOfType<T>(DataRow dr, object objDefaultValue, ref string strError)
        {
            // Validation - Ensure DataRow Content Exists Or Is Mutable
            Globals.ResultType ensureMutableResult = CheckDataRowIsMutable(dr, ref strError);
            if (ensureMutableResult == Globals.ResultType.Failure || strError != "") { return Globals.ResultType.Failure; }

            try
            {
                // Reset Null Values Of Type <T> With Specified Default Value
                dr.ItemArray = dr.ItemArray.Select((obj, intIndex) => (obj == null || obj == DBNull.Value &&
                    dr.ItemArray[intIndex].GetType().IsAssignableFrom(typeof(T)) == true)
                    ? objDefaultValue
                    : obj)
                    .ToArray();
            }
            catch (Exception ex)
            {
                strError = ex.ToString();

                return Globals.ResultType.Failure;
            }

            return Globals.ResultType.Success;
        }

        #endregion

        #endregion

        #region Check Mutable Functions

        private static Globals.ResultType CheckDataTableIsMutable(DataTable dt, ref string strError, bool boolEnsureNotReadOnly = true)
        {
            // Validation
            if (dt == null)
            {
                strError = "Error: DataTable is NULL.";

                return Globals.ResultType.Failure;
            }

            // Validation
            if (dt.Rows.Count > 0 && dt.Rows[0] == null)
            {
                strError = "Error: Cannot Set Default Values to DataRow. It Is NULL";

                return Globals.ResultType.Failure;
            }

            // Validation - Only if boolEnsureNotReadOnly is Default ( True )
            if (boolEnsureNotReadOnly == false && dt.Rows.Count > 0 && dt.Rows[0].ItemArray == null || dt.Rows[0].ItemArray.IsReadOnly == true)
            {
                strError = "Error: Cannot Set Default Values to DataRow. It Is Read Only";

                return Globals.ResultType.Failure;
            }

            return Globals.ResultType.Success;
        }

        private static Globals.ResultType CheckDataRowIsMutable(DataRow dr, ref string strError)
        {
            // Validation
            if (dr == null)
            {
                strError = "Error: Cannot Set Default Values to DataRow. It Is NULL";

                return Globals.ResultType.Failure;
            }

            // Validation
            if (dr.ItemArray == null || dr.ItemArray.IsReadOnly == true)
            {
                strError = "Error: Cannot Set Default Values to DataRow. It Is Read Only";

                return Globals.ResultType.Failure;
            }

            return Globals.ResultType.Success;
        }

        #endregion

        #region Check DataTable Has X Functions

        public static bool CheckDataTableHasColumn(DataTable dt, string strColumnName, ref string strError)
        {
            // Get DataTable Column Index
            int intIndex = dt.Columns.IndexOf(strColumnName);

            // Check Value            
            bool boolTableHasColumn = intIndex > -1;

            // Validation
            if (boolTableHasColumn == false)
            {
                strError = "Error: Column '" + strColumnName + "' Does Not Exist.";

                return false;
            }

            return true;
        }

        #endregion
        
        #region Get Data Count Functions

        public static int GetDataRowNullValueCount(DataRow dr, ref string strError)
        {
            // Validation - Ensure DataRow Content Exists Or Is Mutable
            Globals.ResultType ensureMutableResult = CheckDataRowIsMutable(dr, ref strError);
            if (ensureMutableResult == Globals.ResultType.Failure || strError != "") { return -1; }

            // Get Row Null Value Count
            int intRowNullCount = dr.ItemArray.Where(obj => obj == null || obj == DBNull.Value).Count();

            return intRowNullCount;
        }

        public static int GetDataTableNullValueCount(DataTable dt, ref string strError)
        {
            // Validation - Ensure DataTable Content Is Mutable
            Globals.ResultType ensureMutableResult = CheckDataTableIsMutable(dt, ref strError, false);
            if (ensureMutableResult == Globals.ResultType.Failure || strError != "") { return -1; }

            int intTotalCount = 0;

            // Loop All Rows
            foreach (DataRow dr in dt.Rows)
            {
                // Get Row Null Value Count
                int intRowNullCount = GetDataRowNullValueCount(dr, ref strError);

                // Validation
                if (intRowNullCount == -1 || strError != "") { return -1; }

                intTotalCount += intRowNullCount;
            }

            return intTotalCount;
        }

        public static int GetDataTableColumnNullValueCount(DataTable dt, string strColumnName, ref string strError)
        {
            // Validation - Ensure DataTable Content Is Mutable
            Globals.ResultType ensureMutableResult = CheckDataTableIsMutable(dt, ref strError, false);
            if (ensureMutableResult == Globals.ResultType.Failure || strError != "") { return -1; }

            // Validation - Check DataRow Has Column
            bool boolTableHasColumn = CheckDataTableHasColumn(dt, strColumnName, ref strError);
            if (boolTableHasColumn == false) { return -1; }

            int intTotalNullValueCount = 0;

            try
            {
                // Loop All Rows
                foreach (DataRow dr in dt.Rows)
                {
                    // Get Column Field Object
                    object obj = dr.ItemArray[dr.Table.Columns.IndexOf(strColumnName)];

                    // Validation
                    bool boolObjectIsNull = (obj == null || obj == DBNull.Value);

                    intTotalNullValueCount += (boolObjectIsNull == true) ? 1 : 0;
                }

                return intTotalNullValueCount;
            }
            catch (Exception ex)
            {
                strError = ex.ToString();

                return -1;
            }
        }

        #endregion

        #region Primary Keys

        public static DataColumn[] GetDataRowPrimaryKeys(DataRow dr, ref string strError)
        {
            // Get Primary Keys
            DataColumn[] listPrimaryKeys = GetDataTablePrimaryKeys(dr.Table, ref strError);

            return listPrimaryKeys;
        }

        public static DataColumn[] GetDataTablePrimaryKeys(DataTable dt, ref string strError)
        {
            // Validation - Ensure DataTable Content Exists Or Is Mutable
            Globals.ResultType ensureMutableResult = CheckDataTableIsMutable(dt, ref strError, false);
            if (ensureMutableResult == Globals.ResultType.Failure || strError != "") { return null; }

            DataColumn[] listPrimaryKeys = dt.PrimaryKey;

            return listPrimaryKeys;
        }

        #endregion

    }
}
