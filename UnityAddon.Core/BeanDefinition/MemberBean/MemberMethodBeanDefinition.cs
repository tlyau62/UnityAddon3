using UnityAddon.Core.Attributes;
using UnityAddon.Core.Reflection;
using System;
using System.Reflection;
using Unity;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using UnityAddon.Core.Bean.DependencyInjection;
using Castle.DynamicProxy;

namespace UnityAddon.Core.BeanDefinition.MemberBean
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

        public override string[] Profiles => base.Profiles.Union(ConfigType.GetAttribute<ProfileAttribute>()?.Values ?? new string[0]).ToArray();

        public override object Constructor(IServiceProvider serviceProvider, Type type, string name)
        {
            var config = serviceProvider.GetRequiredService(ConfigType);
            var paramFill = serviceProvider.GetRequiredService<ParameterFill>();

            // enter into the interceptor, construct the bean inside the interceptor
            return Method.Invoke(config, paramFill.FillAllParamaters(Method, serviceProvider));
        }

        public override string Namespace => Method.DeclaringType.Namespace;

        public override Type[] AutoWiredTypes => new[] { Type };

        public Type ConfigType => Method.DeclaringType;

        public IInvocation Invocation { get; set; }
    }
}
