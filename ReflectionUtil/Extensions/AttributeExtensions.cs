using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;

namespace ReflectionUtil.Extensions
{
    internal static class AttributeExtensions
    {
        #region FieldInfo

        internal static T Attribute<T>(this FieldInfo fieldInfo, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            T attribute = fieldInfo.Attributes<T>(bindingFlags).FirstOrDefault();

            return attribute;
        }

        internal static List<T> Attributes<T>(this FieldInfo fieldInfo, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            List<Attribute> listAllAttributes = CacheRepository.ClassTypeList
                .Where(classInfo => classInfo.Type == fieldInfo.DeclaringType)
                .SelectMany(member => member.Members)
                .Where(member => member.FieldInfo.Name == fieldInfo.Name)
                .SelectMany(member => member.Attributes)
                .Where(attribute => attribute.GetType() == typeof(T)).ToList();

            List<T> listAttributes = (List<T>)listAllAttributes.Select(attribute => (T)Convert.ChangeType(attribute, typeof(T))).ToList();

            if (listAttributes != null) { return listAttributes; }

            listAttributes = (List<T>)fieldInfo.GetCustomAttributes(typeof(T), true)
                .Select(attribute => (T)attribute).ToList();

            return listAttributes;
        }

        #endregion

        #region PropertyInfo

        internal static T Attribute<T>(this PropertyInfo propertyInfo, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            T property = propertyInfo.Attributes<T>(bindingFlags).FirstOrDefault();

            return property;
        }

        internal static List<T> Attributes<T>(this PropertyInfo propertyInfo, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            List<T> listAttributes = (List<T>)propertyInfo.GetCustomAttributes(typeof(T), true)
                .Select(attribute => (T)attribute).ToList();

            return listAttributes;
        }

        #endregion
    }
}
