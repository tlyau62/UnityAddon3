using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Lifetime;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;

namespace UnityAddon.Core.BeanDefinition.GeneralBean
{
    public class TypeBeanDefintion : AbstractGeneralBeanDefintion
    {
        private string _uuid = Guid.NewGuid().ToString();

        public TypeBeanDefintion(Type type, Type implType, string name, ScopeType scopeType) : base(type, name, scopeType)
        {
            ImplType = implType;
        }

        public override string Name => $"type-{Type.Name}-{_uuid}";

        public Type ImplType { get; set; } = typeof(object);

        public override object Constructor(IServiceProvider serviceProvider, Type type, string name)
        {
            var impl = ImplType.IsGenericType && ImplType.ContainsGenericParameters ?
                ImplType.MakeGenericType(type.GetGenericArguments()) : ImplType;

            return serviceProvider.GetRequiredService<BeanFactory>().Construct(impl, serviceProvider);
        }
    }
}
