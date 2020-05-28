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

namespace UnityAddon.Core.Util.ComponentScanning
{
    /// <summary>
    /// Scan components from an assembly.
    /// Possible to manually add a component
    /// </summary>
    public class ComponentScanner
    {
        private readonly IEnumerable<IComponentScannerStrategy> _scannerStrategies;

        private readonly IList<Func<Type, bool>> _includeFilters;

        private readonly IList<Func<Type, bool>> _excludeFilters;

        public ComponentScanner(ComponentScannerOption compScannerOption)
        {
            _scannerStrategies = compScannerOption.ScannerStrategies.OrderBy(stg => Ordered.GetOrder(stg.GetType())).ToArray();
            _includeFilters = compScannerOption.IncludeFilters;
            _excludeFilters = compScannerOption.ExcludeFilters;
        }

        public IBeanDefinitionCollection ScanAssembly(Assembly asm, params string[] baseNamespaces)
        {
            var regexes = BuildBaseNamespacesRegexes(baseNamespaces);
            var beanDefCol = new BeanDefinitionCollection();

            beanDefCol.AddRange(asm.GetTypes()
                .Where(t => t.Namespace != null && regexes.Any(regex => regex.IsMatch(t.Namespace)))
                .Where(t => IsCandidate(t))
                .Where(t => t.HasAttribute<ComponentAttribute>(true))
                .SkipWhile(t => !_scannerStrategies.Any(stg => stg.IsMatch(t)))
                .SelectMany(t => _scannerStrategies.First(stg => stg.IsMatch(t)).Create(t))
                .ToArray());

            return beanDefCol;
        }

        public bool IsCandidate(Type type)
        {
            if (_excludeFilters.Any(f => f(type)))
            {
                return false;
            }

            return _includeFilters.Count == 0 || _includeFilters.Any(f => f(type));
        }

        private IEnumerable<Regex> BuildBaseNamespacesRegexes(string[] baseNamespaces)
        {
            return baseNamespaces.Select(ns => new Regex($"^{ns.Replace(".", "\\.")}(\\..*)?$", RegexOptions.Compiled));
        }
    }
}
