using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using Unity.Lifetime;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.BeanDefinition.GeneralBean;
using UnityAddon.Core.Context;
using UnityAddon.Core.Exceptions;

namespace UnityAddon.Core
{
    public interface IServicePostRegistry : IBeanRegistry
    {
        void Unregister(Type type, string name = null);

        void Unregister<T>(string name = null);
    }

    public class ServicePostRegistry : BeanRegistry, IServicePostRegistry
    {
        [Dependency]
        public ApplicationContext AppContext { get; set; }

        [Dependency]
        public IUnityAddonSP Sp { get; set; }

        public override void Add(IBeanDefinition beanDefinition)
        {
            AppContext.ConfigureBeans(config => config.Add(beanDefinition));
        }

        public void Unregister(Type type, string name)
        {
            var container = Sp.UnityContainer;
            var beanDefContainer = Sp.GetRequiredService<IBeanDefinitionContainer>();
            var bean = Sp.GetRequiredService(type, name);
            var beanDef = beanDefContainer.RemoveBeanDefinition(type, name);
            var matchedList = container.Registrations.Where(p => p.RegisteredType == beanDef.Type && p.Name == beanDef.Name);

            foreach (var registration in matchedList)
            {
                registration.LifetimeManager.RemoveValue();

                container.RegisterFactory(beanDef.Type, beanDef.Name, (c, t, n) =>
                {
                    throw new NoSuchBeanDefinitionException($"Type {beanDef.Type} with name '{beanDef.Name}' is unregistered.");
                }, (IFactoryLifetimeManager)Activator.CreateInstance(registration.LifetimeManager.GetType()));
            }
        }

        public void Unregister<T>(string name)
        {
            Unregister(typeof(T), name);
        }
    }
}
