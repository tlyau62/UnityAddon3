using Castle.DynamicProxy;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;
using System.Reflection;
using UnityAddon.Core.Thread;
using UnityAddon.Core.DependencyInjection;

namespace UnityAddon.Core.Bean
{
    /// <summary>
    /// Register the constructor of any bean definition
    /// into the unity container.
    /// </summary>
    [Component]
    public class BeanFactory
    {
        [Dependency]
        public IUnityContainer Container { get; set; }

        [Dependency]
        public IAsyncLocalFactory<Stack<IInvocation>> InvocationStackFactory { get; set; }

        [Dependency]
        public IContainerRegistry ContainerRegistry { get; set; }

        [Dependency]
        public ParameterFill ParameterFill { get; set; }

        [Dependency]
        public ConfigurationFactory ConfigurationFactory { get; set; }

        public void CreateFactory(TypeBeanDefinition typeBeanDefinition)
        {
            var type = typeBeanDefinition.GetBeanType();
            var scope = typeBeanDefinition.GetBeanScope();
            var beanName = typeBeanDefinition.GetBeanName();

            if (type.HasAttribute<ConfigurationAttribute>())
            {
                Container.RegisterFactory(type, beanName, (c, t, n) =>
                {
                    var ctor = DefaultConstructor.Select(t); // TODO: usage of typeBeanDefinition.GetConstructor()

                    return ConfigurationFactory.CreateConfiguration(type, ctor);
                }, BuildScope<IFactoryLifetimeManager>(scope));
            }
            else if (type.HasAttribute<ComponentAttribute>(true))
            {
                Container.RegisterFactory(type, beanName, (c, t, n) =>
                {
                    var ctor = DefaultConstructor.Select(t); // TODO: usage of typeBeanDefinition.GetConstructor()?

                    return ctor.Invoke(ParameterFill.FillAllParamaters(ctor)); 
                }, BuildScope<IFactoryLifetimeManager>(scope));
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void CreateFactory(MethodBeanDefinition methodBeanDefinition)
        {
            var type = methodBeanDefinition.GetBeanType();
            var configType = methodBeanDefinition.GetConfigType();
            var ctor = methodBeanDefinition.GetConstructor();
            var beanName = methodBeanDefinition.GetBeanName();
            var factoryName = methodBeanDefinition.GetFactoryName();
            var scope = methodBeanDefinition.GetBeanScope();

            Container.RegisterFactory(type, beanName, (c, t, n) =>
             {
                 // enter into the interceptor, construct the bean inside the interceptor
                 var config = c.Resolve(configType);

                 return ctor.Invoke(config, ParameterFill.FillAllParamaters(ctor));
             }, BuildScope<IFactoryLifetimeManager>(scope));

            Container.RegisterFactory(type, factoryName, (c, t, n) =>
            {
                var invocation = InvocationStackFactory.Get().Peek();

                invocation.Proceed();

                return invocation.ReturnValue;
            }, BuildScope<IFactoryLifetimeManager>(scope));
        }

        private TLifetimeManager BuildScope<TLifetimeManager>(Type scope)
        {
            return (TLifetimeManager)Activator.CreateInstance(scope);
        }
    }
}
