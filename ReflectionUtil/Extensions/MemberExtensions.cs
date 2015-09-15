using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;

namespace ReflectionUtil.Extensions
{
    internal static class MemberExtensions
    {
        #region FieldInfo

        internal static MemberInfo Member(this object obj, string strFieldName, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            // Load Type Fields Into Cache List If They Are Not Already There
            ClassTypeInformation classInfo = CacheRepository.LoadTypeInformation(obj.GetType(), bindingFlags);

            // Find Field
            MemberInfo objectTypeField = (MemberInfo)classInfo.Members
                    .Where(member => member.MemberInfo.Name == strFieldName)
                    .Select(member => member.FieldInfo)
                    .FirstOrDefault();

            // If Field Exists In Cache Return It
            if (objectTypeField != null) { return objectTypeField; }

            //objectTypeField = obj.GetType().GetFields(bindingFlags)
            //    .Where(field => field.Name == strFieldName).FirstOrDefault();

            return objectTypeField;
        }

        internal static List<MemberInfo> Members(this object obj, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            // Load Type Fields Into Cache List If They Are Not Already There
            ClassTypeInformation classInfo = CacheRepository.LoadTypeInformation(obj.GetType(), bindingFlags);

            List<MemberInfo> listObjectTypeFields = (List<MemberInfo>)classInfo.Members
                .Select(member => member.MemberInfo).ToList();

            // If Field Exists In Cache Return It
            if (listObjectTypeFields != null) { return listObjectTypeFields; }

            listObjectTypeFields = obj.GetType().GetMembers(bindingFlags).ToList();

            return listObjectTypeFields;
        }

        internal static List<MemberInfo> Members(this object obj, MemberTypes type, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            // Get Fields By Type
            List<MemberInfo> listObjectTypeFields = obj.Members(bindingFlags)
                .Where(field => field.MemberType == type).ToList();

            return listObjectTypeFields;
        }

        #endregion

        #region FieldInfo Instance

        internal static List<MemberInfo> InstanceMembers(this object obj, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            List<MemberInfo> listObjectTypeFields = obj.Members(bindingFlags)
                .Where(field => field.GetType().Assembly.FullName == Assembly.GetAssembly(obj.GetType()).FullName).ToList();

            return listObjectTypeFields;
        }

        #endregion

        #region FieldInfo Collection

        //internal static List<MemberInfo> CollectionFields(this object obj, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        //{
        //    List<MemberInfo> listObjectTypeListFields = obj.Members(bindingFlags)
        //        .Where(field =>
        //            field.FieldType.Name.ToLower().Contains("list`")
        //            || field.FieldType.Name.ToLower().Contains("array`")
        //            || field.FieldType.Name.ToLower().Contains("ienumerable`")).ToList();

        //    return listObjectTypeListFields;
        //}

        //internal static List<MemberInfo> CollectionFields(this object obj, ReflectionInformation.CollectionType collectionType, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        //{
        //    List<FieldInfo> listObjectTypeListFields = obj.Members(bindingFlags)
        //        .Where(field =>
        //            field.FieldType.Name.ToLower().Contains(collectionType.ToString().ToLower() + "`")).ToList();

        //    return listObjectTypeListFields;
        //}

        //internal static Type CollectionFieldType(this MemberInfo field)
        //{
        //    Type type = field.GetCustomAttributes(false)[0].GetType();

        //    return type;
        //}

        #endregion

        #region Check / Validation
            
        #endregion
        
    }
}
