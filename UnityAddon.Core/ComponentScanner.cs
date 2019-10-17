﻿using Castle.DynamicProxy;
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

namespace UnityAddon.Core
{
    /// <summary>
    /// Take all assemblies with "ComponentScan" attribute into account, then
    /// scan all the given types as component candidate.
    /// </summary>
    [Component]
    public class ComponentScanner
    {
        [Dependency]
        public BeanDefinitionRegistry BeanDefinitionRegistry { get; set; }

        [Dependency("entryAssembly")]
        public Assembly EntryAssembly { get; set; }

        [Dependency("baseNamespaces")]
        public string[] BaseNamespaces { get; set; }

        private ISet<Assembly> _assemblies = new HashSet<Assembly>();

        public void ScanComponentsFromAppEntry(Assembly assembly, string[] namesps)
        {
            if (!_assemblies.Contains(assembly))
            {
                ScanComponent(assembly, namesps);
                _assemblies.Add(EntryAssembly);
            }
        }

        public void ScanComponentsFromAppDomain()
        {
            var domainAsms = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(asm => !_assemblies.Contains(asm) && asm.HasAttribute<ComponentScanAttribute>());

            foreach (var asm in domainAsms)
            {
                ScanComponent(asm, asm.GetAttribute<ComponentScanAttribute>().BaseNamespaces);
                _assemblies.Add(asm);
            }
        }

        public void ScanComponent(Assembly asm, string[] baseNamespaces)
        {
            var regexes = BuildBaseNamespacesRegexes(baseNamespaces);
            var components = asm.GetTypes()
                .Where(t => t.Namespace != null && regexes.Any(regex => regex.IsMatch(t.Namespace)))
                .Where(t => t.HasAttribute<ComponentAttribute>(true));

            foreach (var component in components)
            {
                BeanDefinitionRegistry.Register(new TypeBeanDefinition(component));
            }
        }

        private IEnumerable<Regex> BuildBaseNamespacesRegexes(string[] baseNamespaces)
        {
            return baseNamespaces.Select(ns => new Regex($"^{ns.Replace(".", "\\.")}(\\..*)?$", RegexOptions.Compiled));
        }
    }
}
