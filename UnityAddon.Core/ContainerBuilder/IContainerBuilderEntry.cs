using Unity;
using UnityAddon.Core.BeanDefinition;

namespace UnityAddon.Core.Bean
{
    public interface IContainerBuilderEntry
    {
        ContainerBuilderEntryOrder Order { get; }

        bool PreInstantiate { get; }

        void PreProcess(IUnityContainer container, IUnityContainer configContainer);

        void PostProcess(IUnityContainer container, IUnityContainer configContainer);

        void ConfigureBeanDefinitions(IBeanDefinitionCollection collection);
    }
}
