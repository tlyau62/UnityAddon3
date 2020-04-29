using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityAddon.Core.Thread;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace UnityAddon.Core.BeanDefinition.MemberBean
{
    public class MemberMethodFactoryBeanDefinition : MemberMethodBeanDefinition
    {
        private string _uuid = Guid.NewGuid().ToString();

        public MemberMethodFactoryBeanDefinition(MethodInfo method) : base(method)
        {
        }

        public override string Name => base.Name + "-factory";

        public override object Constructor(IServiceProvider serviceProvider, Type type, string name)
        {
            var invocation = serviceProvider.GetRequiredService<IThreadLocalFactory<Stack<IInvocation>>>().Get().Peek();

            invocation.Proceed();

            return invocation.ReturnValue;
        }
    }
}
