using System;
using System.Collections.Generic;
using System.Text;
using Unity.Lifetime;
using UnityAddon.Core.Attributes;

namespace UnityAddon.Core.BeanDefinition.GeneralBean
{
    public abstract class AbstractGeneralBeanDefintion : IBeanDefinition
    {
        private readonly ScopeType _scopedType;

        private LifetimeManager _scope;

        public AbstractGeneralBeanDefintion(Type type, string name, ScopeType scopeType)
        {
            _scopedType = scopeType;
            Type = type;

            if (name != null)
            {
                Qualifiers = new[] { name };
            }
        }

        public Type Type { get; set; } = typeof(object);

        public abstract string Name { get; }

        public LifetimeManager Scope
        {
            get
            {
                if (_scope == null)
                {
                    if (_scopedType == ScopeType.Scoped)
                    {
                        _scope = Activator.CreateInstance<HierarchicalLifetimeManager>();
                    }
                    else if (_scopedType == ScopeType.Singleton)
                    {
                        _scope = Activator.CreateInstance<SingletonLifetimeManager>();
                    }
                    else if (_scopedType == ScopeType.Transient)
                    {
                        _scope = Activator.CreateInstance<ContainerControlledTransientManager>();
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }

                return _scope;
            }
        }

        public Type[] AutoWiredTypes => new[] { Type };

        public string[] Qualifiers { get; set; } = new string[0];

        public string[] Profiles { get; set; } = new string[0];

        public bool IsPrimary { get; set; } = false;

        public string Namespace => Type.Namespace;

        public bool FromComponentScanning { get; set; } = false;

        public abstract object Constructor(IServiceProvider serviceProvider, Type type, string name);
    }
}
