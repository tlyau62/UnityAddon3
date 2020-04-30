using System;
using System.Collections.Generic;
using System.Text;
using Unity.Lifetime;

namespace UnityAddon.Core.BeanDefinition.GeneralBean
{
    public class FactoryBeanDefinition<T> : IBeanDefinition
    {
        private readonly Func<IServiceProvider, Type, string, T> _factory;

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

        public string Name => throw new NotImplementedException();

        private LifetimeManager _scope;

        public LifetimeManager Scope => _scope ??= new ContainerControlledLifetimeManager();

        public Type[] AutoWiredTypes => new[] { Type };

        public string[] Qualifiers => throw new NotImplementedException();

        public string[] Profiles => throw new NotImplementedException();

        public bool IsPrimary => throw new NotImplementedException();

        public string Namespace => throw new NotImplementedException();

        public bool FromComponentScanning { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public object Constructor(IServiceProvider serviceProvider, Type type, string name)
        {
            return _factory(serviceProvider, type, name);
        }
    }
}
