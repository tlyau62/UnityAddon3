using UnityAddon.Attributes;
using UnityAddon.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity.Lifetime;

namespace UnityAddon.Bean
{
    public abstract class AbstractBeanDefinition
    {
        private MemberInfo _member;

        public AbstractBeanDefinition(MemberInfo member)
        {
            _member = member;
        }

        public abstract Type GetBeanType();

        public virtual Type GetBeanScope()
        {
            var scopeAttr = _member.GetAttribute<ScopeAttribute>();

            return scopeAttr != null ? scopeAttr.Value : typeof(ContainerControlledLifetimeManager);
        }

        public virtual string GetBeanName()
        {
            return GetBeanType().Name;
        }

        public virtual string[] GetBeanQualifiers()
        {
            var qAttr = _member.GetAttribute<QualifierAttribute>();

            return qAttr != null ? qAttr.Values : new string[0];
        }

        public abstract MethodBase GetConstructor();
    }

    public class TypeBeanDefinition : AbstractBeanDefinition
    {
        private Type _type;

        public TypeBeanDefinition(Type type) : base(type)
        {
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
    }

    public class MethodBeanDefinition : AbstractBeanDefinition
    {
        private MethodInfo _method;

        public MethodBeanDefinition(MethodInfo method) : base(method)
        {
            if (method.HasAttribute<BeanAttribute>() && !method.IsVirtual)
            {
                throw new InvalidOperationException();
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
    }
}
