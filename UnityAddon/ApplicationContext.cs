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
using UnityAddon.Value;

namespace UnityAddon
{
    public class ApplicationContext : ContainerRegistry
    {
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

            ConfigureGlobal();
            ConfigureBeanFactory();
            Refresh();
            Init();
        }

        protected void ConfigureGlobal()
        {
            // app config
            Container.RegisterInstance("baseNamespaces", BaseNamespaces, new ContainerControlledLifetimeManager());
            Container.RegisterInstance("entryAssembly", EntryAssembly, new ContainerControlledLifetimeManager());

            // must singleton, else init twice
            Container.RegisterType<ApplicationContext>(new ContainerControlledLifetimeManager());

            // must singleton, have internal state
            Container.RegisterType<PropertyFill>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ParameterFill>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IBeanDefinitionContainer, BeanDefinitionContainer>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IAsyncLocalFactory<Stack<IInvocation>>, AsyncLocalFactory<Stack<IInvocation>>>(new ContainerControlledLifetimeManager(), new InjectionConstructor(new Func<Stack<IInvocation>>(() => new Stack<IInvocation>())));
            Container.RegisterType<IAsyncLocalFactory<Stack<ResolveStackEntry>>, AsyncLocalFactory<Stack<ResolveStackEntry>>>(new ContainerControlledLifetimeManager(), new InjectionConstructor(new Func<Stack<ResolveStackEntry>>(() => new Stack<ResolveStackEntry>())));
        }

        protected void ConfigureBeanFactory()
        {
            // used by component scan
            Container.RegisterType<IContainerRegistry, ContainerRegistry>();

            // config bean factory (BeanBuildStrategyExtension)
            ComponentScanner = Container.Resolve<ComponentScanner>();
            ComponentScanner.ScanComponents(GetType().Namespace);

            Container.AddNewExtension<BeanBuildStrategyExtension>();

            //Container.Resolve<IBeanDefinitionContainer>().Clear();
        }

        protected void Refresh()
        {
            ComponentScanner = Container.Resolve<ComponentScanner>(); // new scanner initialized by bean factory
            ComponentScanner.ScanComponents(GetType().Namespace); // override all previous registration on unity container, also cause them to be reconstructed even if singleton
        }

        protected void Init()
        {
            Container.BuildUp(this);
            ComponentScanner.ScanComponents(BaseNamespaces);
            ConfigurationParser.ParseScannedConfigurations();
        }

    }
}
