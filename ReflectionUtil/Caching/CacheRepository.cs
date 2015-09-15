using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;

namespace ReflectionUtil
{
    internal static class CacheRepository
    {
        #region Properties

        private static ClassTypeInformationList m_ClassTypeList = new ClassTypeInformationList();
        /// <summary>
        /// All Cached Class Type Data
        /// </summary>
        /// <returns></returns>
        internal static ClassTypeInformationList ClassTypeList
        {
            get
            {
                return m_ClassTypeList;
            }
        }

        private static DataTable m_DataTable;
        /// <summary>
        /// 
        /// </summary>
        internal static DataTable DataTable
        {
            get
            {
                return m_DataTable;
            }
            set
            {
                m_DataTable = value;
            }
        }

        #region DEBUG - Statistic Properties

        /// <summary>
        /// Kill Switch For Stopping Recursive Reflection :)
        /// </summary>
        internal static bool KILL_SWITCH = false;
        
        #endregion

        #endregion

        #region Caching

        internal static ClassTypeInformation LoadTypeInformation(Type type, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            // Validation
            if (ClassTypeList == null) { return null; }

            // Check To See If Type Is Already Cached
            ClassTypeInformation classTypeInfo = ClassTypeList.Where(cls => cls.Type == type).FirstOrDefault();

            // Validation
            if (classTypeInfo != null) { return classTypeInfo; }

            // Create Class Information
            ClassTypeInformation classInfo = new ClassTypeInformation(type);

            // Add Class Information To Cache List
            ClassTypeList.Add(classInfo);

            // For Each Class Child Type
            classInfo.ChildTypes
                .ForEach(childType => 
                    // Load Child Type Information
                    LoadTypeInformation(childType, bindingFlags));

            return classInfo;
        }

        #endregion


        #region Unused Code

        //internal static T CreateObject<T>(params object[] listParams)
        //{
        //    try
        //    {
        //        // Check has Parameterless Constructor if Params is empty
        //        if (listParams == null || listParams.Count() == 0)
        //        {
        //            bool boolHasParameterlessConstructor = typeof(T).HasParameterlessConstructor();
        //            if (boolHasParameterlessConstructor == false)
        //            {
        //                return default(T);
        //            }

        //            // Create new Instance
        //            T obj = (T)Activator.CreateInstance<T>();

        //            return obj;
        //        }
        //        else
        //        {
        //            bool boolHasConstructor = typeof(T).HasConstructor(listParams);
        //            if (boolHasConstructor == false)
        //            {
        //                return default(T);
        //            }

        //            T obj = (T)Activator.CreateInstance(typeof(T), listParams);

        //            return obj;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.ToString());

        //        return default(T);
        //    }
        //}
        
        //internal static void LoadClassType(object obj, BindingFlags bindingFlags)
        //{
        //    // Load Class Type Information
        //    LoadClassType(obj.GetType(), bindingFlags);
        //}

        //internal static void LoadDataTable(DataTable dt)
        //{
        //    m_DataTable = dt;

        //    CachedClassDataList.ForEach(cls => cls.LoadDataTable(dt));
        //}

        #endregion
    }
}
