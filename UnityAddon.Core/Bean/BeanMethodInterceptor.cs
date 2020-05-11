using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public IUnityAddonSP Sp { get; set; }

        private readonly object _lockObj = new object();

        public void Intercept(IInvocation invocation)
        {
            var method = invocation.Method;

            if (!method.HasAttribute<BeanAttribute>())
            {
                invocation.Proceed();
            }
            else
            {
                var beanDef = (MemberMethodBeanDefinition)DefContainer.GetBeanDefinition(method.ReturnType, method.Name);

                lock (_lockObj)
                {
                    if (beanDef.Invocation != null)
                    {
                        throw new InvalidOperationException("Invocation must be null.");
                    }

                    // ensure no other thread can access to this invocation field until this resolution is finished.
                    // may cause deadlock if circular dep happens, but detected by BeanDependencyValidatorStrategy.
                    beanDef.Invocation = invocation;

                    invocation.ReturnValue = Sp.GetRequiredService<MethodFactoryValue>(beanDef.Name + "-factory").Value;

                    beanDef.Invocation = null;
                }
            }
        }
    }
}
