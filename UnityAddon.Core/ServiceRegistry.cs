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
    public interface IServiceRegistry : IBeanRegistry
    {
        void Unregister(Type type, string name = null);

        void Unregister<T>(string name = null);

        void ConfigureBeans(Action<IBeanDefinitionCollection> config);

        void ConfigureBeans(IBeanDefinitionCollection defCollection);
    }

    public class ServiceRegistry : BeanRegistry, IServiceRegistry
    {
        [Dependency]
        public IUnityAddonSP Sp { get; set; }

        [Dependency]
        public IBeanDefinitionContainer BeanDefinitionContainer { get; set; }

        [Dependency]
        public BeanDefintionCandidateSelector BeanDefintionCandidateSelector { get; set; }

        public override void Add(IBeanDefinition beanDefinition)
        {
            ConfigureBeans(config => config.Add(beanDefinition));
        }

        public void ConfigureBeans(Action<IBeanDefinitionCollection> config)
        {
            var defCol = new BeanDefinitionCollection();

            config(defCol);

            ConfigureBeans(defCol);
        }

        public void ConfigureBeans(IBeanDefinitionCollection defCollection)
        {
            foreach (var beanDef in defCollection)
            {
                if (!BeanDefintionCandidateSelector.Filter(beanDef))
                {
                    continue;
                }

                BeanDefinitionContainer.RegisterBeanDefinition(beanDef);
                Sp.UnityContainer.RegisterFactory(beanDef.Type, beanDef.Name, (c, t, n) => beanDef.Constructor(new UnityAddonSP(c), t, n), (IFactoryLifetimeManager)beanDef.Scope);
            }
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
