using UnityAddon.Core.Attributes;
using UnityAddon.Core.Reflection;
using System;
using Unity;
using System.Linq;
using UnityAddon.Core.Bean;
using Microsoft.Extensions.DependencyInjection;

namespace UnityAddon.Core.BeanDefinition.MemberBean
{
    public class MemberComponentBeanDefinition : MemberBeanDefinition
    {
        private string _uuid = Guid.NewGuid().ToString();

        public MemberComponentBeanDefinition(Type type) : base(type)
        {
        }

        public override Type Type => (Type)Member;

        public override string Name => $"member-type-{Type.Name}-{_uuid}";

        public override string[] Qualifiers => base.Qualifiers.Union(new[] { Type.Name }).ToArray();

        public override object Constructor(IServiceProvider serviceProvider, Type type, string name)
        {
            return serviceProvider.GetRequiredService<BeanFactory>()
                .Construct(type, serviceProvider);
        }

        public override string Namespace => Type.Namespace;

        public override Type[] AutoWiredTypes => TypeResolver.GetAssignableTypes(Type).ToArray();
    }
}
