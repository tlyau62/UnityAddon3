using Castle.DynamicProxy;
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
    [Component]
    public class ComponentScanner
    {
        [Dependency]
        public BeanDefinitionRegistry BeanDefinitionRegistry { get; set; }

        [Dependency("entryAssembly")]
        public Assembly EntryAssembly { get; set; }

        public void ScanComponents(string[] namesps)
        {
            foreach (var namesp in namesps)
            {
                ScanComponents(namesp);
            }
        }

        public void ScanComponents(string namesp)
        {
            var components = AppDomain.CurrentDomain.GetAssemblies()
                .Where(asm => asm.HasAttribute<ComponentScanAttribute>() || asm == EntryAssembly)
                //.Select(asm => Assembly.Load(asm.FullName))
                .SelectMany(asm => asm.GetTypes())
                .Where(t => t.Namespace != null)
                .Where(t => new Regex($"^{namesp.Replace(".", "\\.")}(\\..*)?$").IsMatch(t.Namespace))
                .Where(t => t.HasAttribute<ComponentAttribute>(true));

            foreach (var component in components)
            {
                BeanDefinitionRegistry.Register(new TypeBeanDefinition(component));
            }
        }
    }
}
