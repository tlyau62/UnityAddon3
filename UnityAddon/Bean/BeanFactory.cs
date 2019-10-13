using Castle.DynamicProxy;
using UnityAddon.Attributes;
using UnityAddon.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;
using System.Reflection;

namespace UnityAddon.Bean
{
    [Component]
    public class BeanFactory
    {
        [Dependency]
        public ProxyGenerator ProxyGenerator { get; set; }

        [Dependency]
        public IUnityContainer Container { get; set; }

        [Dependency]
        public IAsyncLocalFactory<Stack<IInvocation>> InvocationStackFactory { get; set; }

        [Dependency]
        public IContainerRegistry ContainerRegistry { get; set; }

        [Dependency]
        public BeanMethodInterceptor BeanMethodInterceptor { get; set; }

        [Dependency]
        public ParameterFill ParameterFill { get; set; }

        [Dependency]
        public PropertyFill PropertyFill { get; set; }

        public void CreateFactory(TypeBeanDefinition typeBeanDefinition)
        {
            var type = typeBeanDefinition.GetBeanType();
            var scope = BuildScope<IFactoryLifetimeManager>(typeBeanDefinition.GetBeanScope());
            var ctor = typeBeanDefinition.GetConstructor();
            var beanName = typeBeanDefinition.GetBeanName();

            if (type.HasAttribute<ComponentAttribute>())
            {
                Container.RegisterFactory(type, beanName, (c, t, n) =>
                {
                    var ctorInfo = ((ConstructorInfo)ctor);
                    var obj = Activator.CreateInstance(t, ParameterFill.FillAllParamaters(ctor));

                    return PropertyFill.FillAllProperties(obj);
                }, scope);
            }
            else if (type.HasAttribute<ConfigurationAttribute>())
            {
                Container.RegisterFactory(type, beanName, (c, t, n) =>
                 {
                     var obj = ProxyGenerator.CreateClassProxy(type, ParameterFill.FillAllParamaters(ctor), BeanMethodInterceptor);

                     return PropertyFill.FillAllProperties(obj);
                 }, scope);
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

            Container.RegisterFactory(type, beanName, (c, t, n) =>
             {
                 // enter into the interceptor, construct the bean inside the interceptor
                 var config = c.Resolve(configType);

                 return ctor.Invoke(config, ParameterFill.FillAllParamaters(ctor));
             }, BuildScope<IFactoryLifetimeManager>(methodBeanDefinition.GetBeanScope()));

            Container.RegisterFactory(type, factoryName, (c, t, n) =>
            {
                var invocation = InvocationStackFactory.Get().Peek();

                invocation.Proceed();

                return PropertyFill.FillAllProperties(invocation.ReturnValue);
            }, BuildScope<IFactoryLifetimeManager>(methodBeanDefinition.GetBeanScope()));
        }

        private TLifetimeManager BuildScope<TLifetimeManager>(Type scope)
        {
            return (TLifetimeManager)Activator.CreateInstance(scope);
        }
    }
}
