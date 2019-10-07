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

namespace UnityAddon
{
    [Component]
    public class ApplicationContext
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
    }
}
