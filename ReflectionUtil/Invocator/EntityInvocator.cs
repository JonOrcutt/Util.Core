using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Reflection;
using Utilities;
using Utilities.Helpers;
using ReflectionUtil.Extensions;

namespace ReflectionUtil
{
    public class EntityInvocator
    {
        #region Create Instance

        public static T CreateInstance<T>(DataRow dr, ref string strError, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            try
            {
                // Get Cached Class Information Instance
                ClassTypeInformation classInfo = CacheRepository.ClassTypeList.Where(cls => cls.Type == typeof(T)).FirstOrDefault();

                // Validation
                if (classInfo == null) { return default(T); }

                object obj = classInfo.CreateInstance(typeof(T), dr, ref strError, bindingFlags);

                return (T)obj;
            }
            catch (Exception ex)
            {
                strError = ex.ToString();

                return default(T);
            }
        }

        #endregion

        #region Create Instance List

        public static List<T> CreateInstanceList<T>(DataTable dt, ref string strError, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            DateTime dtStart = DateTime.Now;

            // Validation
            if (typeof(T).HasParameterlessConstructor() == false) { return null; }

            // Set Cache Repository DataTable
            CacheRepository.DataTable = dt;

            // Spider Crawl Load Reflection And Data
            ClassTypeInformation classInfo = CacheRepository.LoadTypeInformation(typeof(T), bindingFlags);

            // Validation
            if (classInfo == null) { return default(List<T>); }
            
            // Load All Class Type DataTables
            CacheRepository.ClassTypeList.ForEach(cls => cls.LoadDataTable());

            // Create Instance List
            List<object> listTemp = classInfo.CreateInstanceList(classInfo,classInfo.DataTable, ref strError, bindingFlags);

            // Validation
            if (listTemp == null) { return default(List<T>); }

            // Convert Objects To Type <T>
            List<T> listObjects = (List<T>)listTemp.Select(obj => (T)obj).ToList();

            // Remove Null Objects
            listObjects = listObjects.Where(obj => obj != null).ToList();

            return listObjects;
        }
        
        #endregion
        
        #region Create DataTable From Object List

        public static DataTable CreateDataTable<T>(List<T> listObjects, ref string strError)
        {
            // Get Class Type
            ClassTypeInformation classInfo = CacheRepository.ClassTypeList.Where(cls => cls.Type == typeof(T)).FirstOrDefault();

            // Validation
            if (classInfo == null) { return null; }

            // Create New DataTable
            DataTable dt = new DataTable();

            // Loop Class Type Members
            foreach (MemberTypeInformation member in classInfo.Members)
            {
                // Validation
                if (member.ColumnName == null || member.ColumnName == "" || member.DataType  == null || member.FieldInfo == null) { continue; }

                DataColumn column = new DataColumn(member.ColumnName, member.DataType);
                column.AllowDBNull = member.Nullable;
                if (member.DataType == typeof(string))
                {
                    column.MaxLength = (member.FieldLength > 0) ? member.FieldLength : 255;
                }

                dt.Columns.Add(column);
            }

            // Loop Object List
            foreach (T obj in listObjects)
            {
                // Create New Row
                DataRow dr = dt.NewRow();

                // Loop Class Members
                foreach (MemberTypeInformation member in classInfo.Members)
                {
                    // Validation
                    if (member.ColumnName == null || member.ColumnName == "" || member.DataType == null || member.FieldInfo == null) { continue; }

                    // Get Member Value
                    object objValue = member.FieldInfo.GetValue(obj);

                    // Set DataColumn Value
                    dr[member.ColumnName] = objValue;
                }

                // Add Row To Table
                dt.Rows.Add(dr);
            }

            return dt;
        }

        #endregion
    }
}
