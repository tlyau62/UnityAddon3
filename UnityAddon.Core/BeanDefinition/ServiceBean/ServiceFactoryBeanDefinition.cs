using System;
using Unity;
using Microsoft.Extensions.DependencyInjection;

namespace UnityAddon.Core.BeanDefinition.ServiceBean
{
    public class ServiceFactoryBeanDefinition : ServiceBeanDefinition
    {
        private string _uuid = Guid.NewGuid().ToString();

        public ServiceFactoryBeanDefinition(ServiceDescriptor serviceDescriptor) : base(serviceDescriptor)
        {
        }

        public override string Name => $"service-factory-{Type.Name}-{_uuid}";

        public override object Constructor(IUnityAddonSP serviceProvider, Type type, string name)
        {
            return Descriptor.ImplementationFactory(serviceProvider);
        }
    }
}
