using System;
using Unity;
using UnityAddon.Core.BeanDefinition;

namespace UnityAddon.Core.Context
{
    public class ApplicationContextEntry
    {
        public ApplicationContextEntry()
        {
        }

        public ApplicationContextEntry(ApplicationContextEntryOrder order, bool preInstantiate)
        {
            Order = order;
            PreInstantiate = preInstantiate;
        }

        public ApplicationContextEntryOrder Order { get; set; } = ApplicationContextEntryOrder.App;

        public bool PreInstantiate { get; set; } = false;

        public IBeanDefinitionCollection BeanDefinitionCollection { get; } = new BeanDefinitionCollection();

        public Action<IUnityContainer> PreProcess { get; set; } = (container) => { };

        public Action<IUnityContainer> PostProcess { get; set; } = (container) => { };

        public ApplicationContextEntry ConfigureBeanDefinitions(Action<IBeanDefinitionCollection> config)
        {
            PreProcess += (container) => config(BeanDefinitionCollection);

            return this;
        }

        public ApplicationContextEntry ConfigureBeanDefinitions(Action<IServiceProvider, IBeanDefinitionCollection> config)
        {
            //config(container. BeanDefinitionCollection);

            throw new NotImplementedException();

            // return this;
        }
    }
}
