using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Attributes;
using UnityAddon.Reflection;

namespace UnityAddon.Bean
{
    [Component]
    public class BeanMethodInterceptor : IInterceptor
    {
        [Dependency]
        public IBeanDefinitionContainer DefContainer { get; set; } // should read only

        [Dependency]
        public IUnityContainer Container { get; set; } // should read only

        [Dependency]
        private IAsyncLocalFactory<Stack<IInvocation>> InvocationStackFactory { get; set; }

        public void Intercept(IInvocation invocation)
        {
            var method = invocation.Method;
            var tempBeanDef = new MethodBeanDefinition(method);
            var resolveName = tempBeanDef.GetBeanQualifiers().Length > 0 ?
                tempBeanDef.GetBeanQualifiers()[0] : tempBeanDef.GetBeanName();

            if (!method.HasAttribute<BeanAttribute>())
            {
                invocation.Proceed();
            }
            else
            {
                var beanDef = DefContainer.GetBeanDefinition(tempBeanDef.GetBeanType(), resolveName); // valid bean def
                var hasStack = InvocationStackFactory.Exist();
                var stack = hasStack ? InvocationStackFactory.Get() : InvocationStackFactory.Set();

                stack.Push(invocation);

                invocation.ReturnValue = Container.Resolve(beanDef.GetBeanType(), "#factory");

                stack.Pop();

                if (!hasStack)
                {
                    InvocationStackFactory.Delete();
                }
            }

        }
    }
}
