using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.BeanDefinition;
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

        public IUnityContainer _container { get; set; }

        public BeanMethodInterceptor(IUnityContainer container)
        {
            _container = container;
        }

        public void Intercept(IInvocation invocation)
        {
            var method = invocation.Method;
            var tempBeanDef = new MethodBeanDefinition(method);
            var beanName = tempBeanDef.BeanName;
            var factoryName = tempBeanDef.FactoryName;
            var beanType = tempBeanDef.BeanType;

            if (!method.HasAttribute<BeanAttribute>())
            {
                invocation.Proceed();
            }
            else
            {
                var beanDef = DefContainer.GetBeanDefinition(beanType, beanName);

                var hasStack = InvocationStackFactory.Exist();
                var stack = hasStack ? InvocationStackFactory.Get() : InvocationStackFactory.Set();

                stack.Push(invocation);

                invocation.ReturnValue = _container.ResolveUA(beanDef.BeanType, factoryName);

                stack.Pop();

                if (!hasStack)
                {
                    InvocationStackFactory.Delete();
                }
            }

        }
    }
}
