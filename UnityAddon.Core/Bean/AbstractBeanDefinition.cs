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

        public abstract Type GetBeanType();

        public Type GetBeanScope()
        {
            var scopeAttr = _member.GetAttribute<ScopeAttribute>();

            return scopeAttr != null ? scopeAttr.Value : typeof(ContainerControlledLifetimeManager);
        }

        public abstract string GetBeanName();

        public string[] GetBeanQualifiers()
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

        public abstract MethodBase GetConstructor();

        public string[] GetBeanProfiles()
        {
            return _member.GetAttribute<ProfileAttribute>()?.Values ?? new string[0];
        }

        public abstract bool IsPrimary();

        public override string ToString()
        {
            var type = GetBeanType().Name;
            var namepsace = GetBeanType().Namespace;

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

        public override Type GetBeanType()
        {
            return _type;
        }

        public override MethodBase GetConstructor()
        {
            return DefaultConstructor.Select(_type);
        }

        public bool IsConfiguration()
        {
            return _type.HasAttribute<ConfigurationAttribute>();
        }

        public override string GetBeanName()
        {
            return GetBeanType().Name;
        }

        public override bool IsPrimary()
        {
            return _type.HasAttribute<PrimaryAttribute>();
        }
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

        public override Type GetBeanType()
        {
            return _method.ReturnType;
        }

        public Type GetConfigType()
        {
            return _method.DeclaringType;
        }

        public override MethodBase GetConstructor()
        {
            return _method;
        }

        public override string GetBeanName()
        {
            return _method.Name;
        }

        public string GetFactoryName()
        {
            return $"#{GetBeanName()}";
        }

        public override bool IsPrimary()
        {
            return _method.HasAttribute<PrimaryAttribute>();
        }
    }
}
