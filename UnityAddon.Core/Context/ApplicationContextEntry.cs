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

        public ApplicationContextEntry(ApplicationContextEntryOrder order)
        {
            Order = order;
        }

        public ApplicationContextEntryOrder Order { get; set; } = ApplicationContextEntryOrder.App;

        public Action<IUnityAddonSP> Process { get; set; } = sp => { };
    }
}
