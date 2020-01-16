using UnityAddon.Core.Attributes;
using UnityAddon.Core.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity.Lifetime;
using System.Runtime.InteropServices;

namespace UnityAddon.Core.Bean
{
    /// <summary>
    /// A receipt for bean construction.
    /// </summary>
    public abstract class AbstractBeanDefinition
    {
        private MemberInfo _member;

        public AbstractBeanDefinition(MemberInfo member)
        {
            _member = member;
        }

        public abstract Type BeanType { get; }

        public Type BeanScope
        {
            get
            {
                var scopeAttr = _member.GetAttribute<ScopeAttribute>();

                return scopeAttr != null ? scopeAttr.Value : typeof(ContainerControlledLifetimeManager);
            }
        }

        public abstract string BeanName { get; }

        public string[] BeanQualifiers
        {
            get
            {
                var qAttr = _member.GetAttribute<QualifierAttribute>();
                var gAttr = _member.GetAttribute<GuidAttribute>();
                var qualifiers = new List<string>();

                if (qAttr != null)
                {
                    qualifiers.AddRange(qAttr.Values);
                }

                if (gAttr != null)
                {
                    qualifiers.Add(gAttr.Value);
                }

                return qualifiers.ToArray();
            }
        }

        public abstract MethodBase Constructor { get; }

        public string[] BeanProfiles => _member.GetAttribute<ProfileAttribute>()?.Values ?? new string[0];

        public abstract bool IsPrimary { get; }

        public override string ToString()
        {
            var type = BeanType.Name;
            var namepsace = BeanType.Namespace;

            return $"{type}: defined in namespace [{namepsace}]";
        }
    }

    public class TypeBeanDefinition : AbstractBeanDefinition
    {
        private Type _type;

        public TypeBeanDefinition(Type type) : base(type)
        {
            if (!type.HasAttribute<ComponentAttribute>(true))
            {
                throw new InvalidOperationException($"Missing component attribute on type {type}.");
            }

            if (type.HasAttribute<ConfigurationAttribute>() && !type.IsPublic)
            {
                throw new InvalidOperationException($"Configuration {type} must be public.");
            }

            _type = type;
        }

        public override Type BeanType => TypeResolver.LoadType(_type);

        public override MethodBase Constructor => DefaultConstructor.Select(BeanType);

        public bool IsConfiguration => _type.HasAttribute<ConfigurationAttribute>();

        public override string BeanName => BeanType.Name;

        public override bool IsPrimary => BeanType.HasAttribute<PrimaryAttribute>();
    }

    public class MethodBeanDefinition : AbstractBeanDefinition
    {
        private MethodInfo _method;

        public MethodBeanDefinition(MethodInfo method) : base(method)
        {
            if (method.HasAttribute<BeanAttribute>() && !method.IsVirtual)
            {
                throw new InvalidOperationException($"Bean method {method} in class {method.DeclaringType} must be virtual.");
            }

            _method = method;
        }

        public override Type BeanType => TypeResolver.LoadType(_method.ReturnType);

        public Type ConfigType => TypeResolver.LoadType(_method.DeclaringType);

        public override MethodBase Constructor => _method;

        public override string BeanName => _method.Name;

        public string FactoryName => $"#{BeanName}";

        public override bool IsPrimary => _method.HasAttribute<PrimaryAttribute>();
    }
}
