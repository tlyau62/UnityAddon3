using Castle.DynamicProxy;
using UnityAddon.Attributes;
using UnityAddon.Bean;
using Unity;
using Unity.Lifetime;
using Unity.Injection;
using System.Collections.Generic;
using System;
using UnityAddon.BeanBuildStrategies;
using System.Reflection;
using System.Linq;
using UnityAddon.Reflection;

namespace UnityAddon
{
    public interface IApplicationContext
    {
        void RegisterType<TRegType, TMapType>(string name);
        void RegisterType<TRegType, TMapType>(ITypeLifetimeManager lifetimeManager);
        void RegisterType<TRegType, TMapType>(string name, ITypeLifetimeManager lifetimeManager);
        void RegisterType(Type regType, Type mapType, string name, ITypeLifetimeManager lifetimeManager);
        T Resolve<T>(string name = null);
        object Resolve(Type type, string name = null);
        T[] ResolveAll<T>();
        object[] ResolveAll(Type type);
    }

    [Component]
    public class ApplicationContext : IApplicationContext
    {
        [Dependency]
        public IUnityContainer Container { get; set; }

        [Dependency]
        public ComponentScanner ComponentScanner { get; set; }

        [Dependency("baseNamespaces")]
        public string[] BaseNamespaces { get; set; }

        [Dependency]
        public ConfigurationParser ConfigurationParser { get; set; }

        [Dependency("entryAssembly")]
        public Assembly EntryAssembly { get; set; }

        [Dependency]
        public IBeanDefinitionContainer BeanDefinitionContainer { get; set; }

        public ApplicationContext(IUnityContainer container, params string[] baseNamespaces)
        {
            Container = container;
            BaseNamespaces = baseNamespaces;
            EntryAssembly = Assembly.GetCallingAssembly();
            Config();
            Init();
        }

        protected void Config()
        {
            // basic
            Container.RegisterInstance("baseNamespaces", BaseNamespaces, new ContainerControlledLifetimeManager());
            Container.RegisterInstance("entryAssembly", EntryAssembly, new ContainerControlledLifetimeManager());
            Container.RegisterType<ApplicationContext>(new ContainerControlledLifetimeManager());

            // for component scan
            Container.RegisterType<ComponentScanner>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ProxyGenerator>(new ContainerControlledLifetimeManager());
            Container.RegisterType<BeanFactory>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IBeanDefinitionContainer, BeanDefinitionContainer>(new ContainerControlledLifetimeManager());

            // singleton dependencies needed not included in component scan
            Container.RegisterType<IAsyncLocalFactory<Stack<IInvocation>>, AsyncLocalFactory<Stack<IInvocation>>>(new ContainerControlledLifetimeManager(), new InjectionConstructor(new Func<Stack<IInvocation>>(() => new Stack<IInvocation>())));
            Container.RegisterType<IAsyncLocalFactory<Stack<ResolveStackEntry>>, AsyncLocalFactory<Stack<ResolveStackEntry>>>(new ContainerControlledLifetimeManager(), new InjectionConstructor(new Func<Stack<ResolveStackEntry>>(() => new Stack<ResolveStackEntry>())));

            // config internal
            ComponentScanner = Container.Resolve<ComponentScanner>();
            ComponentScanner.ScanComponents(GetType().Namespace);

            Container.AddNewExtension<BeanBuildStrategyExtension>();

            Container.BuildUp(this);
        }

        protected void Init()
        {
            ComponentScanner.ScanComponents(BaseNamespaces);
            ConfigurationParser.ParseScannedConfigurations();
        }

        public void RegisterType<TRegType, TMapType>(string name)
        {
            RegisterType(typeof(TRegType), typeof(TMapType), name, null);
        }

        public void RegisterType<TRegType, TMapType>(ITypeLifetimeManager lifetimeManager)
        {
            RegisterType(typeof(TRegType), typeof(TMapType), null, lifetimeManager);
        }

        public void RegisterType<TRegType, TMapType>(string name, ITypeLifetimeManager lifetimeManager)
        {
            RegisterType(typeof(TRegType), typeof(TMapType), name, lifetimeManager);
        }

        public void RegisterType(Type regType, Type mapType, string name, ITypeLifetimeManager lifetimeManager)
        {
            Container.RegisterType(regType, mapType, name, lifetimeManager);
        }

        public T Resolve<T>(string name = null)
        {
            return (T)Resolve(typeof(T), name);
        }

        public object Resolve(Type type, string name = null)
        {
            return Container.Resolve(type, name);
        }

        public T[] ResolveAll<T>()
        {
            return ResolveAll(typeof(T)).Cast<T>().ToArray();
        }

        public object[] ResolveAll(Type type)
        {
            List<object> beans = new List<object>();

            if (BeanDefinitionContainer.HasBeanDefinition(type))
            {
                foreach (var beanDef in BeanDefinitionContainer.GetAllBeanDefinitions(type))
                {
                    if (beanDef.GetBeanQualifiers().Length > 0)
                    {
                        beans.Add(Container.Resolve(beanDef.GetBeanType(), beanDef.GetBeanName()));
                    }
                }
            }

            foreach (var reg in Container.Registrations)
            {
                if (reg.RegisteredType == type && reg.Name != null)
                {
                    if (!BeanDefinitionContainer.HasBeanDefinition(type, reg.Name))
                    {
                        beans.Add(Container.Resolve(type, reg.Name));
                    }
                }
            }

            return beans.ToArray();
        }

    }
}
