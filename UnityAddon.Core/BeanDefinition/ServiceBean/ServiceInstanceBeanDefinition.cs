using Unity;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace UnityAddon.Core.BeanDefinition.ServiceBean
{
    public class ServiceInstanceBeanDefinition : ServiceBeanDefinition
    {
        private string _uuid = Guid.NewGuid().ToString();

        public ServiceInstanceBeanDefinition(ServiceDescriptor serviceDescriptor) : base(serviceDescriptor)
        {
        }

        public override string Name => $"service-instance-{Type.Name}-{_uuid}";

        public override object Constructor(IServiceProvider serviceProvider, Type type, string name)
        {
            return Descriptor.ImplementationInstance;
        }
    }
}
