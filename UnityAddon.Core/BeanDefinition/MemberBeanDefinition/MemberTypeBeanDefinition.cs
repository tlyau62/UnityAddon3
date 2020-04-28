using UnityAddon.Core.Attributes;
using UnityAddon.Core.Reflection;
using System;
using Unity;
using System.Linq;

namespace UnityAddon.Core.BeanDefinition
{
    public class MemberTypeBeanDefinition : MemberBeanDefinition
    {
        private string _uuid = Guid.NewGuid().ToString();

        public MemberTypeBeanDefinition(Type type) : base(type)
        {
            if (type.HasAttribute<ConfigurationAttribute>() && !type.IsPublic)
            {
                throw new InvalidOperationException($"Configuration {type} must be public.");
            }
        }

        public override Type Type => (Type)Member;

        public override string Name => $"member-type-{Type.Name}-{_uuid}";

        public override string[] Qualifiers => base.Qualifiers.Union(new[] { Type.Name }).ToArray();

        public override object Constructor(IServiceProvider serviceProvider, Type type, string name)
        {
            throw new NotImplementedException();
        }

        public override string Namespace => Type.Namespace;

        public override Type[] AutoWiredTypes => TypeResolver.GetAssignableTypes(Type).ToArray();

        public bool IsConfiguration => Type.HasAttribute<ConfigurationAttribute>();
    }
}
