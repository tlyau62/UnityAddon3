using System;
using System.Collections.Generic;
using System.Text;
using Unity.Lifetime;
using UnityAddon.Core.Attributes;

namespace UnityAddon.Core.BeanDefinition.GeneralBean
{
    public class FactoryBeanDefinition : AbstractGeneralBeanDefintion
    {
        private readonly string _uuid = Guid.NewGuid().ToString();

        public FactoryBeanDefinition(Type type, Func<IUnityAddonSP, Type, string, object> factory, string name, ScopeType scopeType) : base(type, name, scopeType)
        {
            Factory = factory;
        }

        public override string Name => $"factory-{Type.Name}-{_uuid}";

        public Func<IUnityAddonSP, Type, string, object> Factory { get; private set; }

        public override object Constructor(IUnityAddonSP serviceProvider, Type type, string name)
        {
            return Factory(serviceProvider, type, name);
        }
    }
}
