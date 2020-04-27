using UnityAddon.Core.Attributes;
using UnityAddon.Core.Reflection;
using System;
using System.Reflection;
using Unity;

namespace UnityAddon.Core.BeanDefinition
{
    public class MemberMethodDefinition : MemberBeanDefinition
    {
        private string _uuid = Guid.NewGuid().ToString();

        public MemberMethodDefinition(MethodInfo method) : base(method)
        {
            if (method.HasAttribute<BeanAttribute>() && !method.IsVirtual)
            {
                throw new InvalidOperationException($"Bean method {method} in class {method.DeclaringType} must be virtual.");
            }

            Method = method;
        }

        public MethodInfo Method { get; private set; }

        public override Type Type => Method.ReturnType;

        public Type ConfigType => Method.DeclaringType;

        public override string Name => $"member-method-{Method.Name}-{_uuid}";

        public string FactoryName => $"#{Name}";

        public override string Namespace { get => ConfigType.Namespace; }

        public override bool RequireAssignableTypes => false;

        public override object Constructor(IUnityContainer container, Type type, string name)
        {
            throw new NotImplementedException();
        }
    }
}
