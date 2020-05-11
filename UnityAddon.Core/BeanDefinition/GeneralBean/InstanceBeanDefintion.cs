using System;
using System.Collections.Generic;
using System.Text;
using Unity.Lifetime;
using UnityAddon.Core.Attributes;

namespace UnityAddon.Core.BeanDefinition.GeneralBean
{
    public class InstanceBeanDefintion : AbstractGeneralBeanDefintion
    {
        private readonly string _uuid = Guid.NewGuid().ToString();

        public InstanceBeanDefintion(Type type, object instance, string name, ScopeType scopeType) : base(type, name, scopeType)
        {
            Instance = instance;
        }

        public override string Name => $"instance-{Type.Name}-{_uuid}";

        public object Instance { get; private set; }

        public override object Constructor(IUnityAddonSP serviceProvider, Type type, string name)
        {
            return Instance;
        }
    }
}
