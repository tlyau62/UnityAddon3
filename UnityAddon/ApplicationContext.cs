using Castle.DynamicProxy;
using UnityAddon.Attributes;
using UnityAddon.Bean;
using Unity;
using Unity.Lifetime;
using Unity.Injection;
using System.Collections.Generic;
using System;
using UnityAddon.BeanBuildStrategies;

namespace UnityAddon
{
    [Component]
    public class ApplicationContext
    {
        private IUnityContainer _container;
        private ComponentScanner _componentScanner;
        private string[] _baseNamespaces;

        [InjectionConstructor]
        public ApplicationContext(ComponentScanner componentScanner, [Dependency("baseNamespaces")]params string[] baseNamespaces)
        {
            _baseNamespaces = baseNamespaces;
            _componentScanner = componentScanner;

            Init();
        }

        public ApplicationContext(IUnityContainer container, params string[] baseNamespaces)
        {
            _container = container;
            _baseNamespaces = baseNamespaces;
            Config();
        }

        protected void Config()
        {
            // registries for both before and after component scan
            _container.RegisterInstance("baseNamespaces", _baseNamespaces, new SingletonLifetimeManager());
            _container.RegisterType<IAsyncLocalFactory<Stack<IInvocation>>, AsyncLocalFactory<Stack<IInvocation>>>(new SingletonLifetimeManager(), new InjectionConstructor(new Func<Stack<IInvocation>>(() => new Stack<IInvocation>())));
            _container.RegisterType<IAsyncLocalFactory<Stack<ResolveStackEntry>>, AsyncLocalFactory<Stack<ResolveStackEntry>>>(new SingletonLifetimeManager(), new InjectionConstructor(new Func<Stack<ResolveStackEntry>>(() => new Stack<ResolveStackEntry>())));

            _componentScanner = new ComponentScanner(_container);

            // config internal
            _componentScanner.ScanComponents(GetType().Namespace);

            _container.AddNewExtension<BeanBuildStrategyExtension>();

            _container.BuildUp(this);
        }

        protected void Init()
        {
            _componentScanner.ScanComponents(_baseNamespaces);
        }
    }
}
