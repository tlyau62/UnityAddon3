using Unity;
using UnityAddon.Core.BeanDefinition;

namespace UnityAddon.Core.Context
{
    public interface IAppContainerBuilderEntry
    {
        ApplicationContextEntryOrder Order { get; }

        bool PreInstantiate { get; }

        void PreProcess(IUnityContainer container);

        void PostProcess(IUnityContainer container);

        void ConfigureBeanDefinitions(IBeanDefinitionCollection collection);
    }
}
