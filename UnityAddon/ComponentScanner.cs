using Castle.DynamicProxy;
using UnityAddon.Attributes;
using UnityAddon.Bean;
using UnityAddon.Reflection;
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

namespace UnityAddon
{
    [Component]
    public class ComponentScanner
    {
        [Dependency]
        public IUnityContainer Container { get; set; }

        [Dependency]
        public IBeanDefinitionContainer BeanDefinitionContainer { get; set; }

        [Dependency]
        public BeanFactory BeanFactory { get; set; }

        [InjectionConstructor]
        public ComponentScanner()
        {
        }

        public ComponentScanner(IUnityContainer container)
        {
            container.RegisterType<ComponentScanner>();
            container.RegisterType<ProxyGenerator>();
            container.RegisterType<ApplicationContext>();
            container.RegisterType<BeanFactory>();
            container.RegisterType<IBeanDefinitionContainer, BeanDefinitionContainer>();
            
            container.BuildUp(this);
        }

        public void ScanComponents(string[] namesps)
        {
            foreach (var namesp in namesps)
            {
                ScanComponents(namesp);
            }
        }

        public void ScanComponents(string namesp)
        {
            var components = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => new Regex($"^{namesp.Replace(".", "\\.")}(\\..*)?$").IsMatch(t.Namespace))
                .Where(t => t.HasAttribute<ComponentAttribute>(true));

            foreach (var component in components)
            {
                var beanDef = new TypeBeanDefinition(component);

                BeanDefinitionContainer.RegisterBeanDefinition(beanDef);
                BeanFactory.CreateFactory(beanDef);
            }
        }
    }
}
