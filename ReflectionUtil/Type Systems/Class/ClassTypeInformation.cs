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
    internal sealed class ClassTypeInformation : TypeInformation, IClassTypeInformation
    {
        #region Properties

        #region Type

        public new Type Type
        {
            get
            {
                return base.Type;
            }
        }

        #endregion

        #region Type Members

        private List<MemberTypeInformation> m_Members;
        public List<MemberTypeInformation> Members
        {
            get
            {
                return this.m_Members;
            }
        }

        private List<MemberTypeInformation> m_PrimaryKeyMembers;
        internal List<MemberTypeInformation> PrimaryKeyMembers
        {
            get
            {
                if (this.m_PrimaryKeyMembers == null && this.Members != null)
                {
                    this.m_PrimaryKeyMembers = this.Members.Where(member => member.IsPrimaryKey == true).ToList();
                }

                return this.m_PrimaryKeyMembers;
            }
        }

        private List<ClassTypeInformation> m_InstanceClasses;
        public List<ClassTypeInformation> InstanceClasses
        {
            get
            {
                if (this.m_InstanceClasses == null)
                {
                    this.m_InstanceClasses = CacheRepository.ClassTypeList.Where(cls => cls.Type !=this.Type 
                        && this.InstanceTypes.Contains(cls.Type)).ToList();
                }

                return this.m_InstanceClasses;
            }
        }

        private List<ClassTypeInformation> m_CollectionClasses;
        public List<ClassTypeInformation> CollectionClasses
        {
            get
            {
                if (this.m_CollectionClasses == null)
                {
                    this.m_CollectionClasses = CacheRepository.ClassTypeList.Where(cls => cls.Type != this.Type
                        && this.CollectionTypes.Contains(cls.Type)).ToList();
                }

                return this.m_CollectionClasses;
            }
        }

        #endregion

        #region Type Relation

        private List<ClassTypeInformation> m_ParentClasses;
        internal List<ClassTypeInformation> ParentClasses
        {
            get
            {
                this.m_ParentClasses = CacheRepository.ClassTypeList.Where(cls => cls.Type != this.Type
                    && (cls.InstanceTypes.Contains(this.Type)
                    || cls.CollectionTypes.Contains(this.Type))).ToList();                

                return this.m_ParentClasses;
            }
        }

        private List<ClassTypeInformation> m_ChildClasses;
        internal List<ClassTypeInformation> ChildClasses
        {
            get
            {
                if (this.m_ChildClasses == null)
                {
                    this.m_ChildClasses = CacheRepository.ClassTypeList.Where(cls => cls.Type != this.Type && 
                        (this.InstanceTypes.Contains(cls.Type)
                        || this.CollectionTypes.Contains(cls.Type))).ToList();
                }

                return this.m_ChildClasses;
            }
        }

        #endregion

        #region Data

        private DataTable m_DataTable;
        internal DataTable DataTable
        {
            get
            {
                return m_DataTable;
            }
        }
                
        internal bool CanBeHydrated
        {
            get
            {
                if (this.Members == null || this.PrimaryKeyMembers == null || CacheRepository.DataTable == null) { return false; }

                return this.PrimaryKeyMembers.All(member => CacheRepository.DataTable.Columns.Contains(member.ColumnName));
            }
        }

        #endregion

        #endregion

        #region Initialization

        internal ClassTypeInformation(Type type, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            this.m_Members = new List<MemberTypeInformation>();
            base.Type = type;

            // Load Class Members
            this.LoadClassTypeMembers(bindingFlags);
        }

        private void LoadClassTypeMembers(BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            // Get All Fields
            List<FieldInfo> listFields = this.Type.GetFields(bindingFlags).Distinct().ToList();

            // Loop Fields
            foreach (FieldInfo field in listFields)
            {
                // Create New Member
                MemberTypeInformation member = new MemberTypeInformation(field, bindingFlags);

                // Add Member To List
                this.Members.Add(member);
            }

            // Get All Properties
            List<PropertyInfo> listProperties = this.Type.GetProperties(bindingFlags).ToList();

            // Loop Properties
            foreach (PropertyInfo property in listProperties)
            {
                // Create New Member
                MemberTypeInformation member = new MemberTypeInformation(property, bindingFlags);

                // Add Member To List
                this.Members.Add(member);
            }
        }

        #endregion

        #region Load DataTable

        internal void LoadDataTable()
        {
            try
            {
                DateTime dtStart = DateTime.Now;

                #region Initial Fragment Table

                // Validation
                if (this.DataTable != null || this.CanBeHydrated == false) { return; }

                // Get All Column Names
                List<string> listColumnNames = this.Members
                    .Where(member => member.ColumnExists == true)
                    .Select(member => member.ColumnName).ToList();

                // Get DataTable Containing Only Columns That This Class Has Members For
                this.m_DataTable = CacheRepository.DataTable.AsDataView()
                    .ToTable(this.Type.Name, false, listColumnNames.ToArray());

                #endregion

                #region Primary Key Fragment Table

                // Get Primary Keys Where Column Names Exist In The DataTable
                string[] listPrimaryKeys = this.PrimaryKeyMembers.Select(member => member.ColumnName).ToArray();
                
                // Validation
                if (listPrimaryKeys.Count() == 0) { return; }

                // Fragment Table By Primary Keys - Non-Distinct
                DataTable dtPrimaryKeyTable = this.m_DataTable.FragmentToTable(listPrimaryKeys.ToList(), true);
                    //.AsDataView().ToTable(true, listPrimaryKeys);

                #endregion

                #region Clone DataTable And Filter By Distinct Primary Key Rows And Construct New Full Distinct Table

                // Clone DataTable
                DataTable dtNew = this.m_DataTable.Clone();

                // Loop DataRows
                foreach (DataRow dr in dtPrimaryKeyTable.Rows)
                {
                    // Sanitize Row Values
                    List<string> listRowValues = dr.ItemArray.Select(objValue => (objValue == null || objValue == DBNull.Value) ? "" : objValue.ToString()).ToList();

                    // Create Filter Based On Primary Key Columns
                    string strFilter = String.Join(" AND  ", this.PrimaryKeyMembers
                         .Where(column => listPrimaryKeys.Contains(column.ColumnName))
                         .Select(member => member.GetQueryFilterString(listRowValues[dr.Table.Columns.IndexOf(member.ColumnName)].ToString())).ToArray());

                    // Filter Table And Grab First Row Satisfying The Expression
                    DataRow drNew = this.m_DataTable.Select(strFilter).First();

                    // Import Row
                    dtNew.ImportRow(drNew);
                }

                // Set This Classes DataTable To The New Filtered DataTable
                this.m_DataTable = dtNew;

                #endregion
                
                #region LOG TIME

                TimeSpan ts = DateTime.Now - dtStart;
                Console.WriteLine("Type: " + this.Type.Name + " - Load Time: " + ts.TotalSeconds + " Seconds | Current Time: " + DateTime.Now.ToLongTimeString());

                #endregion               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        #endregion

        #region Create Instances

        internal object CreateInstance(Type type, DataRow dr, ref string strError, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            try
            {
                // Create New Instance
                object obj = Activator.CreateInstance(type);

                // Validation
                if (obj == null) { return null; }

                // Loop Columns
                foreach (MemberTypeInformation memberInfo in this.Members)
                {
                    // Validation
                    if (memberInfo == null || memberInfo.ColumnExists == false || memberInfo.FieldInfo == null || memberInfo.MemberInfo == null || dr.Table.Columns.Contains(memberInfo.ColumnName) == false || dr[memberInfo.ColumnName] == DBNull.Value) { continue; }

                    // Check MemberInfo Type
                    if (memberInfo.MemberInfo.MemberType == MemberTypes.Field && memberInfo.FieldInfo != null)
                    {
                        // Set Field Value
                        memberInfo.FieldInfo.SetValue(obj, dr[memberInfo.ColumnName]);
                    }
                    else if (memberInfo.MemberInfo.MemberType == MemberTypes.Field && memberInfo.FieldInfo != null)
                    {
                        // Set Property Value
                        memberInfo.PropertyInfo.SetValue(obj, dr[memberInfo.ColumnName], null);
                    }
                }
                
                return obj;
            }
            catch (Exception ex)
            {
                strError = ex.ToString();

                return null;
            }
        }

        internal List<object> CreateInstanceList(ClassTypeInformation classInfo, DataTable dt, ref string strError, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            List<object> listObjects = new List<object>();

            try
            {
                // Validation
                if (classInfo.Type.HasParameterlessConstructor() == false) { return null; }

                // Loop DataRows
                foreach (DataRow dr in dt.Rows)
                {
                    // Check Kill Switch To Break Out Of Recursive Reflection
                    if (CacheRepository.KILL_SWITCH == true) { break; }

                    // Create New Instance
                    object obj = this.CreateInstance(classInfo.Type, dr, ref strError, bindingFlags);

                    // Validation
                    if (obj == null || strError != "") { return null; }

                    // Load Child Member Data
                    this.LoadChildMemberData(obj, dr, bindingFlags);

                    // Add Object To List
                    listObjects.Add(obj);
                }

                return listObjects;
            }
            catch (Exception ex)
            {
                strError = ex.ToString();

                return null;
            }
        }        

        #endregion

        #region Reflect And Load

        internal void LoadChildMemberData(object obj, DataRow dr, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            try
            {
                #region Get Class Information

                // Get Cached Class Information Collection 
                ClassTypeInformation classInfo = CacheRepository.LoadTypeInformation(obj.GetType(), bindingFlags);

                // Validation
                if (classInfo == null || classInfo.DataTable == null) { return; }

                #endregion

                #region Get Class Common Primary Keys

                // Create New List Of Common Primary Keys
                List<MemberTypeInformation> listCommonPrimaryKeys = new List<MemberTypeInformation>();

                // Loop Primary Key Members
                foreach (MemberTypeInformation memberPK in this.PrimaryKeyMembers)
                {
                    MemberTypeInformation commonPrimaryKey = classInfo.PrimaryKeyMembers.Where(pk => pk.ColumnName == memberPK.ColumnName).FirstOrDefault();

                    // Validation
                    if (commonPrimaryKey != null)
                    {
                        // Add Primary Key To List
                        listCommonPrimaryKeys.Add(commonPrimaryKey);
                    }
                }

                #endregion

                #region Load Collection And Instance Members

                // Load Object Instance Properties
                this.LoadInstanceList(obj, listCommonPrimaryKeys, bindingFlags);

                // Load Instance Collection Properties
                this.LoadCollectionList(obj, dr, listCommonPrimaryKeys, bindingFlags);

                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR IN TYPE " + this.Type + " - ROW INDEX: " + this.DataTable.Rows.IndexOf(dr) + " Detail: " + ex.ToString());
            }
        }

        private void LoadInstanceList(object obj, List<MemberTypeInformation> listPrimaryKeyMembers, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            string strError = "";

            // Loop Instance Fields
            foreach (FieldInfo fieldChildInstance in obj.InstanceFields(bindingFlags))
            {
                #region Get Class Information

                // Get Cached Class Information Collection 
                ClassTypeInformation classInfoInstance = CacheRepository.LoadTypeInformation(fieldChildInstance.CollectionFieldType(), bindingFlags);

                // Validation
                if (classInfoInstance == null || classInfoInstance.DataTable == null || classInfoInstance.CanBeHydrated == false) { return; }

                #endregion

                #region Filter Class DataTable

                // Create Data Filter
                string strFilter = String.Join(" AND ", listPrimaryKeyMembers.Select(pk => pk.ColumnName + "=" + classInfoInstance.DataTable.Rows[0][pk.ColumnName]).ToArray());

                // Get DataRow
                DataRow drInstance = classInfoInstance.DataTable.Select(strFilter).First();

                // Validation
                if (drInstance == null) { return; }

                #endregion

                #region Create Instance With Filtered DataTable

                // Create Instance
                object objNew = classInfoInstance.CreateInstance(fieldChildInstance.FieldType, drInstance, ref strError, bindingFlags);

                // Set Value
                fieldChildInstance.SetValue(obj, objNew);

                #endregion
            }
        }

        private void LoadCollectionList(object obj, DataRow dr, List<MemberTypeInformation> listPrimaryKeyMembers, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            string strError = "";

            // Loop Collection Fields
            foreach (FieldInfo fieldChildCollection in obj.CollectionFields(bindingFlags))
            {
                #region Get Class Information

                // Get Cached Class Information Collection 
                ClassTypeInformation classInfoCollection = CacheRepository.LoadTypeInformation(fieldChildCollection.CollectionFieldType(), bindingFlags);

                // Validation
                if (classInfoCollection == null || classInfoCollection.DataTable == null || classInfoCollection.CanBeHydrated == false) { return; }

                #endregion

                #region Filter Class DataTable

                // Create Data Filter
                string strFilter = String.Join(" AND ", listPrimaryKeyMembers.Select(pk => pk.GetQueryFilterString(dr[pk.ColumnName])).ToArray());

                // Get DataRows Based On Filter
                DataRow[] rows = classInfoCollection.DataTable.Select(strFilter);

                // Validation
                if (rows == null || rows.Count() == 0) { return; }

                // Copy Rows To DataTable
                DataTable dtInstance = rows.CopyToDataTable();

                #endregion

                #region Create Instance List With Filtered DataTable

                // Create Instance List
                List<object> listItems = classInfoCollection.CreateInstanceList(classInfoCollection, dtInstance, ref strError, bindingFlags);

                // Validation
                if (listItems == null) { continue; }

                // Create List Instance
                object objList = Activator.CreateInstance(fieldChildCollection.FieldType, null);

                // Loop Instance List
                foreach (object objItem in listItems)
                {
                    // Invoke Add Method And Add Instance To List
                    fieldChildCollection.FieldType.GetMethod("Add")
                        .Invoke(objList, new object[] { objItem });
                }

                // Set List Value
                fieldChildCollection.SetValue(obj, objList);

                #endregion
            }
        }

        #endregion


        #region ORIGINAL

        //private List<object> CreateInstanceList(Type type, DataTable dt, ref string strError, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        //{
        //    List<object> listObjects = new List<object>();

        //    try
        //    {
        //        // Validation
        //        if (type.HasParameterlessConstructor() == false) { return null; }

        //        // Loop DataRows
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            // Check Kill Switch To Break Out Of Recursive Reflection
        //            if (CacheRepository.KILL_SWITCH == true) { break; }

        //            // Create New Instance
        //            object obj = this.CreateInstance(type, dr, ref strError, bindingFlags);

        //            // Validation
        //            if (obj == null || strError != "") { return null; }

        //            // Load Child Member Data
        //            this.LoadChildMemberData(obj, dr, bindingFlags);

        //            // Add Object To List
        //            listObjects.Add(obj);
        //        }

        //        return listObjects;
        //    }
        //    catch (Exception ex)
        //    {
        //        strError = ex.ToString();

        //        return null;
        //    }
        //}
        //private void LoadCollectionList(object obj, DataRow dr, List<MemberTypeInformation> listPrimaryKeyMembers, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        //{
        //    string strError = "";

        //    // Loop Collection Fields
        //    foreach (FieldInfo fieldChildCollection in obj.CollectionFields(bindingFlags))
        //    {
        //        // Load Type Information
        //        CacheRepository.LoadTypeInformation(fieldChildCollection.CollectionFieldType(), bindingFlags);

        //        // Get Cached Class Information Collection 
        //        ClassTypeInformation classInfoCollection = CacheRepository.ClassTypeList.Where(cls => cls.Type == fieldChildCollection.CollectionFieldType()).FirstOrDefault();

        //        // Validation
        //        if (classInfoCollection == null || classInfoCollection.DataTable == null) { return; }

        //        // Create Data Filter
        //        string strFilter = String.Join(" AND ", listPrimaryKeyMembers.Select(pk => pk.ColumnName + "=" + dr[pk.ColumnName]).ToArray());

        //        // Get DataRows Based On Filter
        //        DataRow[] rows = classInfoCollection.DataTable.Select(strFilter);

        //        // Validation
        //        if (rows == null || rows.Count() == 0) { return; }

        //        // Copy Rows To DataTable
        //        DataTable dtInstance = rows.CopyToDataTable();

        //        // Create Instance List
        //        List<object> listItems = classInfoCollection.CreateInstanceList(classInfoCollection.Type, dtInstance, ref strError, bindingFlags);

        //        // Validation
        //        if (listItems == null) { continue; }

        //        // Create List Instance
        //        object objList = Activator.CreateInstance(fieldChildCollection.FieldType, null);

        //        // Loop Instance List
        //        foreach (object objItem in listItems)
        //        {
        //            // Invoke Add Method And Add Instance To List
        //            fieldChildCollection.FieldType.GetMethod("Add")
        //                .Invoke(objList, new object[] { objItem });
        //        }

        //        // Set List Value
        //        fieldChildCollection.SetValue(obj, objList);
        //    }
        //}

        #endregion
    }
}
