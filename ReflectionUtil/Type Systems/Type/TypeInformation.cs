using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReflectionUtil.Extensions;

namespace ReflectionUtil
{
    internal abstract class TypeInformation : ITypeInformation
    {
        #region Properties

        #region Type

        private Type m_Type;
        public Type Type
        {
            get
            {
                return this.m_Type;
            }
            set
            {
                this.m_Type = value;
            }
        }

        protected bool IsGeneric
        {
            get
            {
                return this.Type.IsGenericType;
            }
        }

        #endregion

        #region Members / Relations

        private List<Type> m_InstanceTypes;
        public List<Type> InstanceTypes
        {
            get
            {
                //if (this.m_InstanceTypes == null)
                //{
                    this.m_InstanceTypes = this.Type.InstanceFields()
                        //.Where(field => this.CollectionTypes.Contains(field.FieldType) == false)
                        .Select(field => field.FieldType)
                        .Distinct().ToList();
                //}

                return this.m_InstanceTypes;
            }
        }

        private List<Type> m_CollectionTypes;
        public List<Type> CollectionTypes
        {
            get
            {
                //if (this.m_CollectionTypes == null)
                //{
                    this.m_CollectionTypes = this.Type.CollectionFields()
                        .Select(field => field.FieldType.GetGenericArguments()[0])
                        .Distinct().ToList();
                //}

                return this.m_CollectionTypes;
            }
        }

        private List<Type> m_ChildTypes;
        public List<Type> ChildTypes
        {
            get
            {
                if (this.m_ChildTypes == null)
                {
                    this.m_ChildTypes = this.InstanceTypes;
                    this.m_ChildTypes.AddRange(this.CollectionTypes.Where(type => this.ChildTypes.Contains(type) == false).ToArray());
                }

                return this.m_ChildTypes;
            }
        }

        private List<Type> m_ParentTypes;
        public List<Type> ParentTypes
        {
            get
            {
                this.m_ParentTypes = CacheRepository.ClassTypeList
                    .Where(cls => cls.Type != this.Type && (cls.InstanceTypes.Contains(this.Type) || cls.CollectionTypes.Contains(this.Type)))
                    .Select(cls => cls.Type).ToList();

                return this.m_ParentTypes;
            }
        }

        #endregion

        #endregion
    }
}
