using Unity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Unity.Lifetime;
using Microsoft.Extensions.Logging;
using System.Reflection;
using UnityAddon.Core.Bean;

namespace UnityAddon.Core.BeanDefinition.ServiceBeanDefinition
{
    public class ServiceTypeBeanDefinition : ServiceBeanDefinition
    {
        private string _uuid = Guid.NewGuid().ToString();

        public ServiceTypeBeanDefinition(ServiceDescriptor serviceDescriptor) : base(serviceDescriptor)
        {
        }

        public override string Name => $"service-type-{Type.Name}-{_uuid}";

        public override string[] Qualifiers => new[] { Descriptor.ImplementationType.Name };

        public override object Constructor(IUnityContainer container, Type type, string name)
        {
            var impl = Descriptor.ImplementationType.IsGenericType && Descriptor.ImplementationType.ContainsGenericParameters ?
                Descriptor.ImplementationType.MakeGenericType(type.GetGenericArguments()) :
                Descriptor.ImplementationType;
            var ctorResolver = container.Resolve<ConstructorResolver>();
            var ctor = ctorResolver.ChooseConstuctor(impl);
            var service = ctor.Invoke(ctor.GetParameters().Select(p => container.Resolve(p.ParameterType)).ToArray());

            foreach (var prop in impl.GetProperties().Where(p => p.GetCustomAttribute<DependencyAttribute>(false) != null))
            {
                if (prop.SetMethod != null)
                {
                    prop.SetMethod.Invoke(service, new[] { container.Resolve(prop.PropertyType) });
                }
            }

            return service;
        }
    }
}
