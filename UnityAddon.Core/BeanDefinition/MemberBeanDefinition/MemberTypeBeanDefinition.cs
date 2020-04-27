using UnityAddon.Core.Attributes;
using UnityAddon.Core.Reflection;
using System;

namespace UnityAddon.Core.BeanDefinition
{
    public class MemberTypeBeanDefinition : MemberBeanDefinition
    {
        private Type _type;

        public MemberTypeBeanDefinition(Type type) : base(type)
        {
            if (type.HasAttribute<ConfigurationAttribute>() && !type.IsPublic)
            {
                throw new InvalidOperationException($"Configuration {type} must be public.");
            }

            _type = TypeResolver.LoadType(type);
        }

        public override Type Type => _type;

        public bool IsConfiguration => _type.HasAttribute<ConfigurationAttribute>();

        public override string Name => Type.Name;

        public override string Namespace { get => Type.Namespace; }

        public override bool RequireAssignableTypes => true;
    }
}
