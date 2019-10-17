using System;
using Unity;
using UnityAddon.Core.Attributes;
using Generic;
using UnityAddon.Core.Value;
using Castle.DynamicProxy;
using UnityAddon.Core.Bean;
using System.Collections.Generic;
using System.Linq;
using UnityAddon.Core.Reflection;
using System.Reflection;
using UnityAddon.Core;
using UnityAddon.Core.Aop;

namespace Generic
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RepoAttribute : Attribute
    {
    }

    [Component]
    public class RepoInterceptor : IAttributeInterceptor<RepoAttribute>
    {
        [Dependency]
        public ProxyGenerator ProxyGenerator { get; set; }

        public IInterceptor CreateInterceptor()
        {
            return this;
        }

        public void Intercept(IInvocation invocation)
        {
            throw new NotImplementedException();
        }
    }
}

namespace Generic
{
    // bean registry
    class Program
    {
        static void Main(string[] args)
        {
            var prog = new Program();

            IUnityContainer container = new UnityContainer();
            var appContext = new ApplicationContext(container, "Generic");
            var defContainer = appContext.Resolve<IBeanDefinitionContainer>();
            var interceptorFactoryMap = new Dictionary<Type, IList<IInterceptor>>();
            var createInteceptor = typeof(Program).GetMethod("CreateInterceptor", BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var beanDef in defContainer.GetAllBeanDefinitions(typeof(IAttributeInterceptor<>)))
            {
                var factory = TypeHierarchyScanner.GetInterfaces(beanDef.GetBeanType()).Single(itf => itf.IsGenericType && itf.GetGenericTypeDefinition() == typeof(IAttributeInterceptor<>));
                var attribute = factory.GetGenericArguments().Single();
                IInterceptor interceptor = (IInterceptor)createInteceptor.MakeGenericMethod(attribute).Invoke(prog, new[] { container.Resolve(beanDef.GetBeanType()) });

                if (interceptorFactoryMap.ContainsKey(attribute))
                {
                    interceptorFactoryMap[attribute].Add(interceptor);
                }
                else
                {
                    interceptorFactoryMap[attribute] = new List<IInterceptor>() { interceptor };

                }
            }


            //var config = appContext.Resolve<ValueProvider>().Parse(typeof(string), "section0.key0");
        }
    }
}
