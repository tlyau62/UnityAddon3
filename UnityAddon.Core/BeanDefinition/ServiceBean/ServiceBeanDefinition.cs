using System;
using Unity.Lifetime;
using Unity;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace UnityAddon.Core.BeanDefinition.ServiceBean
{
    public abstract class ServiceBeanDefinition : IBeanDefinition
    {
        public ServiceDescriptor Descriptor { get; }

        public ServiceBeanDefinition(ServiceDescriptor serviceDescriptor)
        {
            Descriptor = serviceDescriptor;
        }

        private LifetimeManager _scope;

        public LifetimeManager Scope
        {
            get
            {
                if (_scope == null)
                {
                    if (Descriptor.Lifetime == ServiceLifetime.Scoped)
                    {
                        _scope = Activator.CreateInstance<HierarchicalLifetimeManager>();
                    }
                    else if (Descriptor.Lifetime == ServiceLifetime.Singleton)
                    {
                        _scope = Activator.CreateInstance<ContainerControlledLifetimeManager>();
                    }
                    else if (Descriptor.Lifetime == ServiceLifetime.Transient)
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

        public Type Type => Descriptor.ServiceType;

        public virtual Type[] AutoWiredTypes => new[] { Descriptor.ServiceType };

        public abstract string Name { get; }

        public virtual string[] Qualifiers => new string[0];

        public string[] Profiles => new string[0];

        public bool IsPrimary => false;

        public string Namespace => Type.Namespace;

        public bool FromComponentScanning { get; set; } = false;

        public abstract object Constructor(IServiceProvider serviceProvider, Type type, string name);
    }
}
