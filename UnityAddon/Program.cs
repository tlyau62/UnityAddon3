using System;
using Unity;
using UnityAddon.Attributes;
using Generic;
using UnityAddon.Value;
using Castle.DynamicProxy;
using UnityAddon.Bean;
using System.Collections.Generic;
using System.Linq;
using UnityAddon.Reflection;
using System.Reflection;
using UnityAddon;

namespace Generic
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RepoAttribute : Attribute
    {
    }

    [Component]
    public class RepoInterceptor : IInterceptorFactory<RepoAttribute>, IInterceptor
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

namespace UnityAddon
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

            foreach (var beanDef in defContainer.GetAllBeanDefinitions(typeof(IInterceptorFactory<>)))
            {
                var factory = TypeHierarchyScanner.GetInterfaces(beanDef.GetBeanType()).Single(itf => itf.IsGenericType && itf.GetGenericTypeDefinition() == typeof(IInterceptorFactory<>));
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
            var p = 1;


            //var config = appContext.Resolve<ValueProvider>().Parse(typeof(string), "section0.key0");
        }
    }
}
