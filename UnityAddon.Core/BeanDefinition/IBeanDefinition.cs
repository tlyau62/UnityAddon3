using System;
using Unity;
using Unity.Lifetime;

namespace UnityAddon.Core.BeanDefinition
{
    public interface IBeanDefinition
    {
        Type Type { get; }

        string Name { get; }

        LifetimeManager Scope { get; }

        Type[] AutoWiredTypes { get; }

        string[] Qualifiers { get; }

        object Constructor(IUnityAddonSP serviceProvider, Type type, string name);

        string[] Profiles { get; }

        bool IsPrimary { get; }

        string Namespace { get; }
    }
}
