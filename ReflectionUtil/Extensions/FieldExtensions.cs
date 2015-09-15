using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using CoreUtil;

namespace ReflectionUtil.Extensions
{
    internal static class FieldExtensions
    {
        #region FieldInfo

        internal static FieldInfo Field(this object obj, string strFieldName, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            // Load Type Fields Into Cache List If They Are Not Already There
            ClassTypeInformation classInfo = CacheRepository.LoadTypeInformation(obj.GetType(), bindingFlags);

            // Find Field
            FieldInfo objectTypeField = (FieldInfo)classInfo.Members
                    .Where(member => member.FieldInfo.Name == strFieldName)
                    .Select(member => member.FieldInfo)
                    .FirstOrDefault();

            return objectTypeField;
        }

        internal static List<FieldInfo> Fields(this Type type, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            // Load Type Fields Into Cache List If They Are Not Already There
            CacheRepository.LoadTypeInformation(type, bindingFlags);

            List<FieldInfo> listObjectTypeFields = (List<FieldInfo>)CacheRepository.ClassTypeList
                .Where(classInfo => classInfo.Type == type)
                .SelectMany(classInfo =>
                    classInfo.Members.Select(member => member.FieldInfo))
                    .ToList();

            return listObjectTypeFields;
        }

        internal static List<FieldInfo> Fields(this object obj, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            List<FieldInfo> listObjectTypeFields = obj.GetType().Fields(bindingFlags);

            return listObjectTypeFields;
        }

        internal static List<FieldInfo> Fields(this object obj, Type type, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            // Get Fields By Type
            List<FieldInfo> listObjectTypeFields = obj.Fields(bindingFlags)
                .Where(field => field != null && field.FieldType == type).ToList();

            return listObjectTypeFields;
        }

        #endregion

        #region FieldInfo Instance
        
        internal static bool HasInstanceFields(this object obj, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            bool boolHasInstanceFields = obj.GetType().HasInstanceFields(bindingFlags);

            return boolHasInstanceFields;
        }

        internal static bool HasInstanceFields(this Type type, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            bool boolHasInstanceFields = type.Fields(bindingFlags).Where(field => field != null && field.IsInstanceField() == true).Any();

            return boolHasInstanceFields;
        }

        internal static bool IsInstanceField(this FieldInfo field)
        {
            bool boolIsInstance = field != null && field.FieldType.Assembly.FullName == Assembly.GetExecutingAssembly().FullName;

            return boolIsInstance;
        }

        internal static bool IsInstanceField(this object obj, string strName, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            bool boolIsInstance = obj.InstanceFields(bindingFlags).Where(field => field != null && field.Name == strName).Any();

            return boolIsInstance;
        }

        internal static bool IsInstanceField(this Type type, string strName, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            if (type.HasParameterlessConstructor() == false) { return false; }

            object obj = Activator.CreateInstance(type);

            bool boolIsInstance = obj.InstanceFields(bindingFlags).Where(field => field != null && field.Name == strName).Any();

            return boolIsInstance;
        }

        internal static List<FieldInfo> InstanceFields(this object obj, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            List<FieldInfo> listObjectTypeFields = obj.GetType().InstanceFields(bindingFlags);

            return listObjectTypeFields;
        }

        internal static List<FieldInfo> InstanceFields(this Type type, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            List<FieldInfo> listObjectTypeFields = type.Fields(bindingFlags)
                .Where(field => field != null && field.FieldType.IsGenericType == true
                    && field.FieldType.Name.ToLower().Contains("list`") == false
                    && field.FieldType.Name.ToLower().Contains("array`") == false
                    && field.FieldType.Name.ToLower().Contains("ienumerable`") == false
                    ).ToList();


            return listObjectTypeFields;
        }
        
        #endregion

        #region FieldInfo Collection

        internal static bool HasCollectionFields(this object obj, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            bool boolHasCollectionFields = obj.GetType().HasInstanceFields(bindingFlags);

            return boolHasCollectionFields;
        }

        internal static bool HasCollectionFields(this Type type, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            bool boolHasCollectionFields = type.Fields(bindingFlags).Where(field => field != null && 
                    (field.FieldType.Name.ToLower().Contains("list`")
                    || field.FieldType.Name.ToLower().Contains("array`")
                    || field.FieldType.Name.ToLower().Contains("ienumerable`"))).ToList().Any();

            return boolHasCollectionFields;
        }

        internal static List<FieldInfo> CollectionFields(this object obj, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            List<FieldInfo> listObjectTypeListFields = obj.GetType().CollectionFields(bindingFlags);

            return listObjectTypeListFields;
        }

        internal static List<FieldInfo> CollectionFields(this Type type, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            List<FieldInfo> listObjectTypeListFields = type.Fields(bindingFlags)
                .Where(field => field != null &&
                    (field.FieldType.Name.ToLower().Contains("list`")
                    || field.FieldType.Name.ToLower().Contains("array`")
                    || field.FieldType.Name.ToLower().Contains("ienumerable`"))).ToList();

            return listObjectTypeListFields;
        }

        internal static List<FieldInfo> CollectionFields(this object obj, ReflectionInformation.CollectionType collectionType, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            List<FieldInfo> listObjectTypeListFields = obj.GetType().CollectionFields(collectionType, bindingFlags);

            return listObjectTypeListFields;
        }

        internal static List<FieldInfo> CollectionFields(this Type type, ReflectionInformation.CollectionType collectionType, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            List<FieldInfo> listObjectTypeListFields = type.Fields(bindingFlags)
                .Where(field => field != null &&
                    field.FieldType.Name.ToLower().Contains(collectionType.ToString().ToLower() + "`")).ToList();

            return listObjectTypeListFields;
        }

        internal static Type CollectionFieldType(this FieldInfo field)
        {
            Type type = (field.FieldType.GetGenericArguments().Count() > 0) ? field.FieldType.GetGenericArguments()[0] : typeof(object);

            return type;
        }

        #endregion

        #region Check / Validation

        internal static bool HasField(this object obj, string strFieldName, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            bool boolHasField = obj.GetType().Field(strFieldName, bindingFlags) != null;

            return boolHasField;
        }

        internal static bool HasField(this Type type, string strFieldName, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            bool boolHasField = type.Field(strFieldName, bindingFlags) != null;

            return boolHasField;
        }

        #endregion

        #region Currently Unused

        #region Get Values

        internal static T GetFieldValue<T>(this object obj, string strFieldName, T objValue, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            try
            {
                FieldInfo field = obj.Field(strFieldName, bindingFlags);
                if (field == null || field.FieldType.IsAssignableFrom(typeof(T))) { return default(T); }

                T value = (T)field.GetValue(obj);

                return value;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                return default(T);
            }
        }

        #endregion

        #region Set Values

        internal static Globals.ResultType SetFieldValue(this object obj, string strFieldName, object objValue, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            try
            {
                FieldInfo field = obj.Field(strFieldName, bindingFlags);
                if (field == null || field.FieldType.IsAssignableFrom(objValue.GetType()) == false) { return Globals.ResultType.Failure; }

                field.SetValue(obj, objValue);

                return Globals.ResultType.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                return Globals.ResultType.Failure;
            }
        }

        internal static Globals.ResultType SetFieldValues(this object obj, DataRow dr, string strFieldNamePrefix = "", BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            try
            {
                strFieldNamePrefix = (strFieldNamePrefix == "") ? "m_" : strFieldNamePrefix;

                List<FieldInfo> listCommonFields = obj.Fields(bindingFlags)
                    .Where(field => field != null && dr.Table.Columns.Contains(field.Name.Replace(strFieldNamePrefix, "").Trim())
                        && dr.Table.Columns[field.Name.Replace(strFieldNamePrefix, "").Trim()] != null
                        && field.FieldType.IsAssignableFrom(dr.Table.Columns[field.Name.Replace(strFieldNamePrefix, "").Trim()].DataType) == true).ToList();

                foreach (FieldInfo field in listCommonFields)
                {
                    if (dr[field.Name.Replace(strFieldNamePrefix, "")] == null || dr[field.Name.Replace(strFieldNamePrefix, "")] == DBNull.Value) { continue; }
                    field.SetValue(obj, dr[field.Name.Replace(strFieldNamePrefix, "")]);
                }

                return Globals.ResultType.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                return Globals.ResultType.Failure;
            }
        }

        #endregion

        #endregion
    }
}
