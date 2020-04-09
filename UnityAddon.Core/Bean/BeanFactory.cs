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
using UnityAddon.Core.DependencyInjection;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.Thread;
using Castle.DynamicProxy;

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
        public ParameterFill ParameterFill { get; set; }

        [Dependency]
        public ConfigurationFactory ConfigurationFactory { get; set; }

        [Dependency]
        public IThreadLocalFactory<Stack<IInvocation>> InvocationStackFactory { get; set; }

        [Dependency]
        public ProxyGenerator ProxyGenerator { get; set; }

        public void CreateFactory(IEnumerable<IBeanDefinition> beanDefinitions, IUnityContainer container)
        {
            foreach (var def in beanDefinitions.Where(d => d.RequireFactory))
            {
                CreateFactory((dynamic)def, container);
            }
        }

        public void CreateFactory(SimpleFactoryBeanDefinition simpFactoryBeanDef, IUnityContainer container)
        {
            var type = simpFactoryBeanDef.BeanType;
            var scope = simpFactoryBeanDef.BeanScope;
            var beanName = simpFactoryBeanDef.BeanName;
            var ctor = simpFactoryBeanDef.Constructor;

            container.RegisterFactory(type, beanName, ctor, BuildScope<IFactoryLifetimeManager>(scope));
        }

        public void CreateFactory(TypeBeanDefinition typeBeanDefinition, IUnityContainer container)
        {
            var type = typeBeanDefinition.BeanType;
            var scope = typeBeanDefinition.BeanScope;
            var beanName = typeBeanDefinition.BeanName;

            if (type.HasAttribute<ConfigurationAttribute>())
            {
                container.RegisterFactory(type, beanName, configFactory, BuildScope<IFactoryLifetimeManager>(scope));
            }
            else if (type.HasAttribute<ComponentAttribute>(true))
            {
                container.RegisterFactory(type, beanName, componentFactory, BuildScope<IFactoryLifetimeManager>(scope));
            }
            else
            {
                throw new NotImplementedException();
            }

            object configFactory(IUnityContainer c, Type t, string n)
            {
                var ctor = DefaultConstructor.Select(t);

                return ConfigurationFactory.CreateConfiguration(type, ctor, c);
            }

            object componentFactory(IUnityContainer c, Type t, string n)
            {
                if (t.IsInterface)
                {
                    return ProxyGenerator.CreateInterfaceProxyWithoutTarget(t);
                }

                var ctor = DefaultConstructor.Select(t);

                return ctor.Invoke(ParameterFill.FillAllParamaters(ctor, container));
            }
        }

        public void CreateFactory(MethodBeanDefinition methodBeanDefinition, IUnityContainer container)
        {
            var type = methodBeanDefinition.BeanType;
            var configType = methodBeanDefinition.ConfigType;
            var ctor = methodBeanDefinition.Method;
            var beanName = methodBeanDefinition.BeanName;
            var factoryName = methodBeanDefinition.FactoryName;
            var scope = methodBeanDefinition.BeanScope;

            container.RegisterFactory(type, beanName, (c, t, n) =>
            {
                // enter into the interceptor, construct the bean inside the interceptor
                var config = c.Resolve(configType);

                return ctor.Invoke(config, ParameterFill.FillAllParamaters(ctor, container));
            }, BuildScope<IFactoryLifetimeManager>(scope));

            container.RegisterFactory(type, factoryName, (c, t, n) =>
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
