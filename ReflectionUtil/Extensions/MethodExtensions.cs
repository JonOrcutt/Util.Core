using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;

namespace ReflectionUtil.Extensions
{
    internal static class MethodExtensions
    {

        #region MethodInto

        internal static MethodInfo Method(this Type type, string strMethodName, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            MethodInfo method = type.Methods().Where(methodInfo => methodInfo.Name == strMethodName).FirstOrDefault();

            return method;
        }

        internal static MethodInfo Method(this object obj, string strMethodName, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            MethodInfo method = obj.Method(strMethodName, bindingFlags);

            return method;
        }

        internal static List<MethodInfo> Methods(this Type type, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            List<MethodInfo> listObjectTypeMethods = type.GetMethods(bindingFlags).ToList();

            return listObjectTypeMethods;
        }

        internal static List<MethodInfo> Methods(this object obj, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            List<MethodInfo> listObjectTypeMethods = obj.GetType().Methods();

            return listObjectTypeMethods;
        }

        #endregion

        #region Check / Validation

        internal static bool HasMethod(this object obj, string strMethodName, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            bool boolHasMethod = obj.Methods(bindingFlags)
                .Where(method => method.Name == strMethodName).Any();

            return boolHasMethod;
        }

        #endregion

        #region Constructors

        internal static bool HasConstructor(this Type type, params object[] listParams)
        {
            try
            {
                ConstructorInfo constructorInfo = null;

                if (listParams != null)
                {
                    Type[] listTypes = listParams.Select(param => param.GetType()).ToArray();

                    constructorInfo = type.GetConstructor(listTypes);
                }
                else
                {
                    constructorInfo = type.GetConstructor(System.Type.EmptyTypes);
                }

                bool boolHasConstructor = constructorInfo != null;

                return boolHasConstructor;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                return false;
            }
        }

        internal static bool HasConstructor(this object obj, params object[] listParams)
        {
            bool boolHasConstructor = obj.GetType().HasConstructor(listParams);

            return boolHasConstructor;
        }

        internal static bool HasParameterlessConstructor(this Type type)
        {
            ConstructorInfo constructorInfo = type.GetConstructor(Type.EmptyTypes);
            bool boolHasParameterlessConstructor = constructorInfo != null;

            return boolHasParameterlessConstructor;
        }

        internal static bool HasParameterlessConstructor(this object obj)
        {
            bool boolHasParameterlessConstructor = obj.GetType().HasParameterlessConstructor();

            return boolHasParameterlessConstructor;
        }

        #endregion
    }
}
