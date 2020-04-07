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
        private IEnumerable<IComponentScannerStrategy> _scannerStrategies;

        public ComponentScanner(IEnumerable<IComponentScannerStrategy> scannerStrategies)
        {
            _scannerStrategies = scannerStrategies;
        }

        public IEnumerable<IBeanDefinition> ScanComponents(Assembly asm)
        {
            return ScanComponents(asm, asm.GetAttribute<ComponentScanAttribute>().BaseNamespaces);
        }

        public IEnumerable<IBeanDefinition> ScanComponents(Assembly asm, params string[] baseNamespaces)
        {
            var regexes = BuildBaseNamespacesRegexes(baseNamespaces);

            return asm.GetTypes()
                .Where(t => t.Namespace != null && regexes.Any(regex => regex.IsMatch(t.Namespace)))
                .Where(t => t.HasAttribute<ComponentAttribute>(true))
                .SkipWhile(t => !_scannerStrategies.Any(stg => stg.IsMatch(t)))
                .Select(t =>
                {
                    var def = _scannerStrategies.First(stg => stg.IsMatch(t)).Create(t);

                    def.FromComponentScanning = true;

                    return def;
                });
        }

        private IEnumerable<Regex> BuildBaseNamespacesRegexes(string[] baseNamespaces)
        {
            return baseNamespaces.Select(ns => new Regex($"^{ns.Replace(".", "\\.")}(\\..*)?$", RegexOptions.Compiled));
        }
    }
}
