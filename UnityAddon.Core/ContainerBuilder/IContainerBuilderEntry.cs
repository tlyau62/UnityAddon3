using Unity;
using UnityAddon.Core.BeanDefinition;

namespace UnityAddon.Core.Bean
{
    public interface IContainerBuilderEntry
    {
        ContainerBuilderEntryOrder Order { get; }

        bool PreInstantiate { get; }

        void PreProcess(IUnityContainer container);

        void PostProcess(IUnityContainer container);

        void ConfigureBeanDefinitions(IBeanDefinitionCollection collection);
    }
}
