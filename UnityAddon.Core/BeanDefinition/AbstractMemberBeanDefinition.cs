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

namespace UnityAddon.Core.BeanDefinition
{
    public interface IBeanDefinition
    {
        Type BeanType { get; }

        string BeanName { get; }

        string[] BeanQualifiers { get; }

        string[] BeanProfiles { get; }

        bool IsPrimary { get; }

        string Namespace { get; }
    }

    public class SimpleBeanDefinition : IBeanDefinition
    {
        public SimpleBeanDefinition(Type beanType, params string[] beanQualifiers)
        {
            BeanType = beanType;
            BeanQualifiers = beanQualifiers.Where(q => q != null).ToArray();
        }

        public Type BeanType { get; set; }

        public string BeanName { get; }

        public string[] BeanQualifiers { get; set; } = new string[0];

        public string[] BeanProfiles { get; set; } = new string[0];

        public bool IsPrimary { get; set; } = false;

        public string Namespace { get => BeanType.Name; }
    }

    /// <summary>
    /// A receipt for bean construction.
    /// </summary>
    public abstract class AbstractMemberBeanDefinition : IBeanDefinition
    {
        private MemberInfo _member;

        public AbstractMemberBeanDefinition(MemberInfo member)
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

        public string[] BeanProfiles => _member.GetAttribute<ProfileAttribute>()?.Values ?? new string[0];

        public abstract bool IsPrimary { get; }

        public override string ToString()
        {
            var type = BeanType.Name;
            var namepsace = BeanType.Namespace;

            return $"{type}: defined in namespace [{namepsace}]";
        }

        public abstract string Namespace { get; }
    }

    public class TypeBeanDefinition : AbstractMemberBeanDefinition
    {
        private Type _type;

        public TypeBeanDefinition(Type type) : base(type)
        {
            if (type.HasAttribute<ConfigurationAttribute>() && !type.IsPublic)
            {
                throw new InvalidOperationException($"Configuration {type} must be public.");
            }

            _type = TypeResolver.LoadType(type);
        }

        public override Type BeanType => _type;

        public bool IsConfiguration => _type.HasAttribute<ConfigurationAttribute>();

        public override string BeanName => BeanType.Name;

        public override bool IsPrimary => BeanType.HasAttribute<PrimaryAttribute>();

        public override string Namespace { get => BeanType.Namespace; }
    }

    public class MethodBeanDefinition : AbstractMemberBeanDefinition
    {
        public MethodInfo Method { get; set; }

        public MethodBeanDefinition(MethodInfo method) : base(method)
        {
            if (method.HasAttribute<BeanAttribute>() && !method.IsVirtual)
            {
                throw new InvalidOperationException($"Bean method {method} in class {method.DeclaringType} must be virtual.");
            }

            Method = method;
        }

        public override Type BeanType => TypeResolver.LoadType(Method.ReturnType);

        public Type ConfigType => TypeResolver.LoadType(Method.DeclaringType);

        public override string BeanName => Method.Name;

        public string FactoryName => $"#{BeanName}";

        public override bool IsPrimary => Method.HasAttribute<PrimaryAttribute>();

        public override string Namespace { get => ConfigType.Namespace; }
    }
}
