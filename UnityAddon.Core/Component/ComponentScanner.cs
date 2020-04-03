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

namespace UnityAddon.Core.Component
{
    /// <summary>
    /// Scan components from an assembly.
    /// Possible to manually add a component
    /// </summary>
    public class ComponentScanner
    {
        [Dependency]
        public ConfigurationParser ConfigurationParser { get; set; }

        public IEnumerable<IBeanDefinition> ScanComponents(Assembly asm)
        {
            return ScanComponents(asm, asm.GetAttribute<ComponentScanAttribute>().BaseNamespaces);
        }

        public IEnumerable<IBeanDefinition> ScanComponents(Assembly asm, params string[] baseNamespaces)
        {
            var regexes = BuildBaseNamespacesRegexes(baseNamespaces);

            var components = asm.GetTypes()
                .Where(t => t.Namespace != null && regexes.Any(regex => regex.IsMatch(t.Namespace)))
                .Where(t => t.HasAttribute<ComponentAttribute>(true))
                .Select(t => new TypeBeanDefinition(t));

            return components.Union(ConfigurationParser.Parse(components));
        }

        private IEnumerable<Regex> BuildBaseNamespacesRegexes(string[] baseNamespaces)
        {
            return baseNamespaces.Select(ns => new Regex($"^{ns.Replace(".", "\\.")}(\\..*)?$", RegexOptions.Compiled));
        }
    }
}
