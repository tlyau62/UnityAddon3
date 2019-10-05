using Castle.DynamicProxy;
using UnityAddon.Attributes;
using UnityAddon.Bean;
using Unity;
using Unity.Lifetime;

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
            _container.RegisterInstance("baseNamespaces", _baseNamespaces, new SingletonLifetimeManager());

            _componentScanner = new ComponentScanner(_container);

            // config internal
            _componentScanner.ScanComponents(GetType().Namespace);

            _container.AddNewExtension<BeanUnityExtension>();

            _container.BuildUp(this);
        }

        protected void Init()
        {
            _componentScanner.ScanComponents(_baseNamespaces);
        }
    }
}
