using UnityAddon.Core.Attributes;
using UnityAddon.Core.Reflection;
using System;
using System.Reflection;
using Unity;
using System.Linq;

namespace UnityAddon.Core.BeanDefinition
{
    public class MemberMethodBeanDefinition : MemberBeanDefinition
    {
        private string _uuid = Guid.NewGuid().ToString();

        public MemberMethodBeanDefinition(MethodInfo method) : base(method)
        {
            if (method.HasAttribute<BeanAttribute>() && !method.IsVirtual)
            {
                throw new InvalidOperationException($"Bean method {method} in class {method.DeclaringType} must be virtual.");
            }
        }

        public MethodInfo Method => (MethodInfo)Member;

        public override Type Type => Method.ReturnType;

        public override string Name => $"member-method-{Method.Name}-{_uuid}";

        public override string[] Qualifiers => base.Qualifiers.Union(new[] { Method.Name }).ToArray();

        public override object Constructor(IServiceProvider serviceProvider, Type type, string name)
        {
            throw new NotImplementedException();
        }

        public override string Namespace => Method.DeclaringType.Namespace;

        public override Type[] AutoWiredTypes => new[] { Type };

        public Type ConfigType => Method.DeclaringType;

        public string FactoryName => $"#{Name}";
    }
}
