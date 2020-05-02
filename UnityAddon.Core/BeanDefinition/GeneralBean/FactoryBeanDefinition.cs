using System;
using System.Collections.Generic;
using System.Text;
using Unity.Lifetime;

namespace UnityAddon.Core.BeanDefinition.GeneralBean
{
    public class FactoryBeanDefinition<T> : IBeanDefinition
    {
        private readonly Func<IServiceProvider, Type, string, T> _factory;

        private readonly string _uuid = Guid.NewGuid().ToString();

        public FactoryBeanDefinition(Func<IServiceProvider, Type, string, T> factory)
        {
            _factory = factory;
        }

        public FactoryBeanDefinition(Func<IServiceProvider, Type, string, T> factory, LifetimeManager scope)
        {
            _factory = factory;
            _scope = scope;
        }

        public Type Type => typeof(T);

        public string Name => $"factory-{_uuid}";

        private LifetimeManager _scope;

        public LifetimeManager Scope => _scope ??= new ContainerControlledLifetimeManager();

        public Type[] AutoWiredTypes => new[] { Type };

        public string[] Qualifiers => new string[0];

        public string[] Profiles => new string[0];

        public bool IsPrimary => false;

        public string Namespace => Type.Namespace;

        public bool FromComponentScanning { get; set; } = false;

        public object Constructor(IServiceProvider serviceProvider, Type type, string name)
        {
            return _factory(serviceProvider, type, name);
        }
    }
}
