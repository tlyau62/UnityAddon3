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
        public BeanMethodInterceptor BeanMethodInterceptor { get; set; }

        public void CreateFactory(TypeBeanDefinition typeBeanDefinition)
        {
            var type = typeBeanDefinition.GetBeanType();
            var scope = BuildScope<IFactoryLifetimeManager>(typeBeanDefinition.GetBeanScope());
            var ctor = typeBeanDefinition.GetConstructor();

            if (type.HasAttribute<ComponentAttribute>())
            {
                Container.RegisterFactory(type, (c, t, n) =>
                {
                    var obj = ((ConstructorInfo)ctor).Invoke(ParameterFill.FillAllParamaters(ctor, c));

                    return PropertyFill.FillAllProperties(obj, c);
                }, scope);
            }
            else if (type.HasAttribute<ConfigurationAttribute>())
            {
                Container.RegisterFactory(type, (c, t, n) =>
                {
                    var obj = ProxyGenerator.CreateClassProxy(type, ParameterFill.FillAllParamaters(ctor, c), BeanMethodInterceptor);

                    return PropertyFill.FillAllProperties(obj, c);
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

            Container.RegisterFactory(type, (c, t, n) =>
            {
                // enter into the interceptor, constructor the bean inside the interceptor
                var config = c.Resolve(configType);

                return ctor.Invoke(config, ParameterFill.FillAllParamaters(ctor, c));
            }, BuildScope<IFactoryLifetimeManager>(methodBeanDefinition.GetBeanScope()));

            Container.RegisterFactory(type, "#factory", (c, t, n) =>
            {
                var invocation = InvocationStackFactory.Get().Peek();

                invocation.Proceed();

                return PropertyFill.FillAllProperties(invocation.ReturnValue, c); // resolve type is interface, so need to build up
            }, BuildScope<IFactoryLifetimeManager>(methodBeanDefinition.GetBeanScope()));
        }

        private TLifetimeManager BuildScope<TLifetimeManager>(Type scope)
        {
            return (TLifetimeManager)Activator.CreateInstance(scope);
        }
    }
}
