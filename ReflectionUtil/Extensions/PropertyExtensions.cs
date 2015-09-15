using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;

namespace ReflectionUtil.Extensions
{
    internal static class PropertyExtensions
    {
        #region PropertyInfo

        internal static PropertyInfo Property(this object obj, string strPropertyName, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            // Load Type Fields Into Cache List If They Are Not Already There
            ClassTypeInformation classInfo = CacheRepository.LoadTypeInformation(obj.GetType(), bindingFlags);

            // Check Property Cache List For Property By Name
            PropertyInfo objectTypeProperty = (PropertyInfo)classInfo.Members
                .Where(member => member.PropertyInfo.Name == strPropertyName)
                    .Select(member => member.PropertyInfo)
                    .FirstOrDefault();

            // If Property Exists In Cache Return It
            if (objectTypeProperty != null) { return objectTypeProperty; }

            objectTypeProperty = obj.Properties(bindingFlags)
                .Where(property => property.Name == strPropertyName).FirstOrDefault();

            return objectTypeProperty;
        }

        internal static List<PropertyInfo> Properties(this Type type, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            // Load Type Fields Into Cache List If They Are Not Already There
            ClassTypeInformation classInfo = CacheRepository.LoadTypeInformation(type, bindingFlags);

            // Check Property Cache List For Field By Name
            List<PropertyInfo> listObjectTypeProperties = (List<PropertyInfo>)classInfo.Members
                .Select(member => member.PropertyInfo).ToList();
            
            return listObjectTypeProperties;
        }

        internal static List<PropertyInfo> Properties(this object obj, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            List<PropertyInfo> listObjectTypeProperties = obj.GetType().Properties(bindingFlags);

            return listObjectTypeProperties;
        }

        internal static List<PropertyInfo> Properties(this object obj, Type type, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            // Get Properties By Type
            List<PropertyInfo> listObjectTypeProperties = obj.Properties(bindingFlags)
                .Where(property => property.PropertyType == type).ToList();

            return listObjectTypeProperties;
        }

        #endregion

        #region PropertyInfo Instance

        internal static bool HasInstanceProperties(this object obj, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            bool boolHasInstanceProperties = obj.GetType().HasInstanceProperties(bindingFlags);

            return boolHasInstanceProperties;
        }

        internal static bool HasInstanceProperties(this Type type, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            bool boolHasInstanceProperties = type.Properties(bindingFlags).Where(property => property.IsInstanceProperty() == true).Any();

            return boolHasInstanceProperties;
        }
        
        internal static bool IsInstanceProperty(this PropertyInfo property)
        {
            bool boolIsInstance = property.PropertyType.Assembly.FullName == Assembly.GetExecutingAssembly().FullName;

            return boolIsInstance;
        }

        //internal static bool IsInstanceProperty(this Type type)
        //{
        //    bool boolIsInstance = type.Assembly.FullName == Assembly.GetExecutingAssembly().FullName;

        //    return boolIsInstance;
        //}

        internal static bool IsInstanceProperty(this object obj, string strName, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            bool boolIsInstance = obj.GetType().IsInstanceProperty(strName, bindingFlags);

            return boolIsInstance;
        }

        internal static bool IsInstanceProperty(this Type type, string strName, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            if (type.HasParameterlessConstructor() == false) { return false; }

            object obj = Activator.CreateInstance(type);

            bool boolIsInstance = obj.InstanceProperties(bindingFlags).Where(property => property.Name == strName).Any();

            return boolIsInstance;
        }

        internal static List<PropertyInfo> InstanceProperties(this object obj, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            List<PropertyInfo> listObjectTypeProperties = obj.GetType().InstanceProperties(bindingFlags);

            return listObjectTypeProperties;
        }

        internal static List<PropertyInfo> InstanceProperties(this Type type, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            List<PropertyInfo> listObjectTypeProperties = type.Properties(bindingFlags)
                .Where(property => property != null && property.PropertyType.IsGenericType == true
                    && property.PropertyType.Name.ToLower().Contains("list`") == false
                    && property.PropertyType.Name.ToLower().Contains("array`") == false
                    && property.PropertyType.Name.ToLower().Contains("ienumerable`") == false
                    ).ToList();

            return listObjectTypeProperties;
        }

        #endregion

        #region PropertyInfo Collection

        internal static bool HasCollectionProperties(this object obj, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            bool boolHasCollectionProperties = obj.GetType().HasInstanceProperties(bindingFlags);

            return boolHasCollectionProperties;
        }

        internal static bool HasCollectionProperties(this Type type, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            bool boolHasCollectionProperties = type.CollectionProperties(bindingFlags).Any();

            return boolHasCollectionProperties;
        }

        internal static List<PropertyInfo> CollectionProperties(this object obj, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            List<PropertyInfo> listObjectTypeProperties = obj.GetType().CollectionProperties(bindingFlags);

            return listObjectTypeProperties;
        }

        internal static List<PropertyInfo> CollectionProperties(this Type type, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            List<PropertyInfo> listObjectTypeProperties = type.Properties(bindingFlags)
                .Where(property =>
                    property.PropertyType.Name.ToLower().Contains("list`")
                    || property.PropertyType.Name.ToLower().Contains("array`")
                    || property.PropertyType.Name.ToLower().Contains("ienumerable`")).ToList();

            return listObjectTypeProperties;
        }

        internal static List<PropertyInfo> CollectionProperties(this object obj, ReflectionInformation.CollectionType collectionType, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            List<PropertyInfo> listObjectTypeProperties = obj.GetType().CollectionProperties(collectionType, bindingFlags);

            return listObjectTypeProperties;
        }

        internal static List<PropertyInfo> CollectionProperties(this Type type, ReflectionInformation.CollectionType collectionType, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            List<PropertyInfo> listObjectTypeProperties = type.Properties(bindingFlags)
                .Where(property => property.PropertyType.Name.Contains(collectionType.ToString().ToLower() + "`")).ToList();

            return listObjectTypeProperties;
        }

        internal static Type CollectionPropertyType(this PropertyInfo property)
        {
            Type type = (property.PropertyType.GetGenericArguments().Count() > 0) ? property.PropertyType.GetGenericArguments()[0] : typeof(object);

            return type;
        }

        #endregion

        #region Check / Validation

        internal static bool HasProperty(this object obj, string strPropertyName, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            bool boolHasField = obj.Property(strPropertyName, bindingFlags) != null;

            return boolHasField;
        }

        #endregion
    }
}
