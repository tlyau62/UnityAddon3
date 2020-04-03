using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;
using UnityAddon.Core.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;
using Unity.Injection;
using System.Runtime.Loader;
using UnityAddon.Core.BeanDefinition;

namespace UnityAddon.Core.Bean
{
    public interface IComponentScanner
    {
        void AddComponent(IBeanDefinition beanDefinition, IUnityContainer container);

        IEnumerable<Type> ScanComponent(Assembly asm, IUnityContainer container);

        IEnumerable<Type> ScanComponent(Assembly asm, IUnityContainer container, params string[] baseNamespaces);
    }

    /// <summary>
    /// Scan components from an assembly.
    /// Possible to manually add a component
    /// </summary>
    //TODO:[Component]
    public class ComponentScanner : IComponentScanner
    {
        [Dependency]
        public BeanFactory BeanFactory { get; set; }

        [Dependency]
        public IBeanDefinitionContainer BeanDefinitionContainer { get; set; }

        public void AddComponent(IBeanDefinition beanDefinition, IUnityContainer container)
        {
            BeanDefinitionContainer.RegisterBeanDefinitionForAllTypes(beanDefinition);
            BeanFactory.CreateFactory((dynamic)beanDefinition, container);
        }

        public IEnumerable<Type> ScanComponent(Assembly asm, IUnityContainer container)
        {
            return ScanComponent(asm, container, asm.GetAttribute<ComponentScanAttribute>().BaseNamespaces);
        }

        public IEnumerable<Type> ScanComponent(Assembly asm, IUnityContainer container, params string[] baseNamespaces)
        {
            var regexes = BuildBaseNamespacesRegexes(baseNamespaces);
            var components = asm.GetTypes()
                .Where(t => t.Namespace != null && regexes.Any(regex => regex.IsMatch(t.Namespace)))
                .Where(t => t.HasAttribute<ComponentAttribute>(true));

            foreach (var component in components)
            {
                AddComponent(new TypeBeanDefinition(component), container);
            }

            return components;
        }

        private IEnumerable<Regex> BuildBaseNamespacesRegexes(string[] baseNamespaces)
        {
            return baseNamespaces.Select(ns => new Regex($"^{ns.Replace(".", "\\.")}(\\..*)?$", RegexOptions.Compiled));
        }
    }
}
