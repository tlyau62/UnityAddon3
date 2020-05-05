using System;
using Unity;
using UnityAddon.Core.BeanDefinition;

namespace UnityAddon.Core.Bootstrap
{
    public class ContainerBuilderEntry
    {
        public ContainerBuilderEntry()
        {
        }

        public ContainerBuilderEntry(ContainerBuilderEntryOrder order, bool preInstantiate)
        {
            Order = order;
            PreInstantiate = preInstantiate;
        }

        public ContainerBuilderEntryOrder Order { get; set; } = ContainerBuilderEntryOrder.App;

        public bool PreInstantiate { get; set; } = false;

        public IBeanDefinitionCollection BeanDefinitionCollection { get; } = new BeanDefinitionCollection();

        public Action<IUnityContainer> PreProcess { get; set; } = (container) => { };

        public Action<IUnityContainer> PostProcess { get; set; } = (container) => { };

        public ContainerBuilderEntry ConfigureBeanDefinitions(Action<IBeanDefinitionCollection> config)
        {
            PreProcess += (container) => config(BeanDefinitionCollection);

            return this;
        }

        public ContainerBuilderEntry ConfigureBeanDefinitions(Action<IServiceProvider, IBeanDefinitionCollection> config)
        {
            //config(container. BeanDefinitionCollection);

            throw new NotImplementedException();

            // return this;
        }
    }
}
