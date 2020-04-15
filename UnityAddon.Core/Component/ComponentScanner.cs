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
using Microsoft.Extensions.DependencyInjection;

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
                .SelectMany(t =>
                {
                    var defs = _scannerStrategies.First(stg => stg.IsMatch(t)).Create(t);

                    foreach (var def in defs)
                    {
                        def.FromComponentScanning = true;
                    }

                    return defs;
                });
        }

        public IEnumerable<IBeanDefinition> ScanComponents(IServiceCollection serviceCollection)
        {
            return serviceCollection.Select(descriptor => new SimpleBeanDefinition(descriptor.ServiceType));
        }

        private IEnumerable<Regex> BuildBaseNamespacesRegexes(string[] baseNamespaces)
        {
            return baseNamespaces.Select(ns => new Regex($"^{ns.Replace(".", "\\.")}(\\..*)?$", RegexOptions.Compiled));
        }
    }
}
