using System;
using Unity;
using UnityAddon.Core.BeanDefinition;

namespace UnityAddon.Core.Bean
{
    public class BeanLoaderEntry
    {
        public BeanLoaderEntry()
        {
        }

        public BeanLoaderEntry(BeanLoaderEntryOrder order, bool preInstantiate)
        {
            Order = order;
            PreInstantiate = preInstantiate;
        }

        public BeanLoaderEntryOrder Order { get; set; } = BeanLoaderEntryOrder.App;

        public bool PreInstantiate { get; set; } = false;

        public IBeanDefinitionCollection BeanDefinitionCollection { get; } = new BeanDefinitionCollection();

        public Action<IUnityContainer> PreProcess { get; set; } = container => { };

        public Action<IUnityContainer> PostProcess { get; set; } = container => { };

        public BeanLoaderEntry ConfigureBeanDefinitions(Action<IBeanDefinitionCollection> config)
        {
            config(BeanDefinitionCollection);

            return this;
        }

        public BeanLoaderEntry ConfigureBeanDefinitions(Action<IServiceProvider, IBeanDefinitionCollection> config)
        {
            //config(container. BeanDefinitionCollection);

            throw new NotImplementedException();

            // return this;
        }
    }
}
