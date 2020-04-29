using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.BeanDefinition.MemberBean;
using UnityAddon.Core.Reflection;
using UnityAddon.Core.Thread;

namespace UnityAddon.Core.Bean
{
    /// <summary>
    /// Used in construct bean defined by bean method.
    /// It delegates the bean construction to bean factory via a Stack<IInvocation>.
    /// </summary>
    public class BeanMethodInterceptor : IInterceptor
    {
        [Dependency]
        public IBeanDefinitionContainer DefContainer { get; set; }

        [Dependency]
        public IThreadLocalFactory<Stack<IInvocation>> InvocationStackFactory { get; set; }

        public IServiceProvider _serviceProvider { get; set; }

        public BeanMethodInterceptor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Intercept(IInvocation invocation)
        {
            var method = invocation.Method;

            if (!method.HasAttribute<BeanAttribute>())
            {
                invocation.Proceed();
            }
            else
            {
                var tempBeanDef = new MemberMethodFactoryBeanDefinition(method);
                var hasStack = InvocationStackFactory.Exist();
                var stack = hasStack ? InvocationStackFactory.Get() : InvocationStackFactory.Set();

                stack.Push(invocation);

                invocation.ReturnValue = _serviceProvider.GetRequiredService(tempBeanDef.Type, tempBeanDef.Name);

                stack.Pop();

                if (!hasStack)
                {
                    InvocationStackFactory.Delete();
                }
            }
        }
    }
}
