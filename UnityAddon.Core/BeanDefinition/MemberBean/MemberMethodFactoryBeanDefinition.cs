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
        private MemberMethodBeanDefinition _parentBeanDef;

        public MemberMethodFactoryBeanDefinition(MemberMethodBeanDefinition methodBeanDef) : base(methodBeanDef.Method)
        {
            _parentBeanDef = methodBeanDef;
        }

        public override string Name => _parentBeanDef.Name + "-factory";

        public override Type Type => typeof(MethodFactoryValue);

        public override object Constructor(IServiceProvider serviceProvider, Type type, string name)
        {
            var invocation = serviceProvider.GetRequiredService<IThreadLocalFactory<Stack<IInvocation>>>().Get().Peek();

            invocation.Proceed();

            return new MethodFactoryValue(invocation.ReturnValue);
        }
    }

    public class MethodFactoryValue
    {
        public object Value { get; }

        public MethodFactoryValue(object val)
        {
            Value = val;
        }
    }
}
