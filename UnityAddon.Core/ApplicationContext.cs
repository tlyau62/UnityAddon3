using Castle.DynamicProxy;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;
using Unity;
using Unity.Lifetime;
using Unity.Injection;
using System.Collections.Generic;
using System;
using UnityAddon.Core.BeanBuildStrategies;
using System.Reflection;
using System.Linq;
using UnityAddon.Core.DependencyInjection;
using UnityAddon.Core.Value;
using UnityAddon.Core.Aop;
using UnityAddon.Core.Thread;
using UnityAddon.Core.BeanPostprocessor;

namespace UnityAddon.Core
{
    /// <summary>
    /// Where all the magic happens.
    /// </summary>
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

        [Dependency]
        public AopInterceptorContainer InterceptorContainer { get; set; }

        private bool _preInstantiateSingleton;

        public ApplicationContext(IUnityContainer container, params string[] baseNamespaces) : this(container, true, Assembly.GetCallingAssembly(), baseNamespaces)
        {
        }

        public ApplicationContext(IUnityContainer container, bool preInstantiateSingleton, params string[] baseNamespaces) : this(container, preInstantiateSingleton, Assembly.GetCallingAssembly(), baseNamespaces)
        {
        }

        public ApplicationContext(IUnityContainer container, bool preInstantiateSingleton, Assembly entryAssembly, params string[] baseNamespaces)
        {
            Container = container;
            BaseNamespaces = baseNamespaces;
            EntryAssembly = entryAssembly ?? Assembly.GetCallingAssembly();
            _preInstantiateSingleton = preInstantiateSingleton;

            ConfigureGlobal();
            ConfigBeanBuildingStrategy();
            ConfigComponentScanner();
            Refresh();
            Init();
        }

        // TODO: replace Container to ContainerRegistry
        protected void ConfigureGlobal()
        {
            // app
            Container.RegisterType<ApplicationContext>(new ContainerControlledLifetimeManager());

            // app config
            Container.RegisterInstance("baseNamespaces", BaseNamespaces, new ContainerControlledLifetimeManager());
            Container.RegisterInstance("entryAssembly", EntryAssembly, new ContainerControlledLifetimeManager());

            // global singleton
            Container.RegisterType<IAsyncLocalFactory<Stack<IInvocation>>, AsyncLocalFactory<Stack<IInvocation>>>(new ContainerControlledLifetimeManager(), new InjectionConstructor(new Func<Stack<IInvocation>>(() => new Stack<IInvocation>())));
            Container.RegisterType<IBeanDefinitionContainer, BeanDefinitionContainer>(new ContainerControlledLifetimeManager());
        }

        /// <summary>
        /// Dependencies needed by BeanBuildStrategyExtension.
        /// 
        /// All type/bean registered here are plain objects, in other words,
        /// they will not be affected by BeanBuildStrategyExtension.
        /// All type/bean registered after BeanBuildStrategyExtension will
        /// be affected.
        /// </summary>
        protected void ConfigBeanBuildingStrategy()
        {
            Container.RegisterType<IAsyncLocalFactory<Stack<ResolveStackEntry>>, AsyncLocalFactory<Stack<ResolveStackEntry>>>(new ContainerControlledLifetimeManager(), new InjectionConstructor(new Func<Stack<ResolveStackEntry>>(() => new Stack<ResolveStackEntry>())));
            Container.RegisterType<AopInterceptorContainer>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IContainerRegistry, ContainerRegistry>();

            Container.AddNewExtension<BeanBuildStrategyExtension>(); // fill up dep using unity container
        }

        /// <summary>
        /// Dependencies needed by ComponentScanner.
        /// Must be after BeanBuildStrategyExtension is registered.
        /// </summary>
        protected void ConfigComponentScanner()
        {
            Container.RegisterType<ProxyGenerator>();
            Container.RegisterType<ValueProvider>();
            Container.RegisterType<ParameterFill>();
            Container.RegisterType<BeanMethodInterceptor>();
            Container.RegisterType<ConfigurationFactory>();
            Container.RegisterType<BeanFactory>();
            Container.RegisterType<BeanDefinitionRegistry>();
            Container.RegisterType<ConfigBracketParser>();
            Container.RegisterType<DependencyExceptionFactory>();
            Container.RegisterType<DependencyResolver>();
        }

        /// <summary>
        /// Override all previous registration if they are components on unity container,
        /// also cause them to be reconstructed even if singleton.
        /// </summary>
        protected void Refresh()
        {
            ComponentScanner = Container.Resolve<ComponentScanner>();
            ComponentScanner.ScanComponentsFromAppDomain();

            Container.BuildUp(this);
        }

        /// <summary>
        /// Execute after refresh at "buildUp(this)".
        /// All internal beans are well registered before executing this method.
        /// </summary>
        protected void Init()
        {
            ComponentScanner.ScanComponentsFromAppEntry();
            ConfigurationParser.ParseScannedConfigurations();
            InterceptorContainer.Build();

            if (_preInstantiateSingleton)
            {
                PreInstantiateSingleton();
            }
        }

        /// <summary>
        /// Recursive instantiate singleton bean.
        /// Some bean may do bean registration at postconstruct,
        /// so recursive needed.
        /// 
        /// The final number of un-registrations will be converge to 0,
        /// since each bean is postconstructed once only.
        /// </summary>
        public void PreInstantiateSingleton()
        {
            var currentRegs = Container.Registrations.Count();

            foreach (var reg in Container.Registrations)
            {
                if (!(reg.LifetimeManager is ContainerControlledLifetimeManager))
                {
                    continue;
                }

                if (!reg.RegisteredType.IsGenericType || !reg.RegisteredType.ContainsGenericParameters)
                {
                    Container.Resolve(reg.RegisteredType, reg.Name);
                }
            }

            if (Container.Registrations.Count() != currentRegs)
            {
                PreInstantiateSingleton();
            }
        }

        public void RegisterBeanPostProcessors(IEnumerable<IBeanPostProcessor> beanPostprocessors)
        {
            Container.RegisterInstance(beanPostprocessors);
        }
    }
}
