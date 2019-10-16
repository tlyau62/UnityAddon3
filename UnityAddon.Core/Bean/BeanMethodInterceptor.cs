using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Reflection;
using UnityAddon.Core.Thread;

namespace UnityAddon.Core.Bean
{
    /// <summary>
    /// Used in construct bean defined by bean method.
    /// It delegates the bean construction to bean factory via a Stack<IInvocation>.
    /// </summary>
    [Component]
    public class BeanMethodInterceptor : IInterceptor
    {
        [Dependency]
        public IBeanDefinitionContainer DefContainer { get; set; }

        [Dependency]
        public IContainerRegistry ContainerRegistry { get; set; }

        [Dependency]
        public IAsyncLocalFactory<Stack<IInvocation>> InvocationStackFactory { get; set; }

        public void Intercept(IInvocation invocation)
        {
            var method = invocation.Method;
            var tempBeanDef = new MethodBeanDefinition(method);
            var beanName = tempBeanDef.GetBeanName();
            var factoryName = tempBeanDef.GetFactoryName();

            if (!method.HasAttribute<BeanAttribute>())
            {
                invocation.Proceed();
            }
            else
            {
                var beanDef = DefContainer.GetBeanDefinition(tempBeanDef.GetBeanType(), beanName); // valid bean def
                var hasStack = InvocationStackFactory.Exist();
                var stack = hasStack ? InvocationStackFactory.Get() : InvocationStackFactory.Set();

                stack.Push(invocation);

                invocation.ReturnValue = ContainerRegistry.Resolve(beanDef.GetBeanType(), factoryName);

                stack.Pop();

                if (!hasStack)
                {
                    InvocationStackFactory.Delete();
                }
            }

        }
    }
}
