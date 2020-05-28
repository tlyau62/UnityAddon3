using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using Unity.Lifetime;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean.DependencyInjection;
using UnityAddon.Core.BeanDefinition;

namespace UnityAddon.Core.Value
{
    public class ValueConfig
    {
        public IUnityContainer Build(IUnityAddonSP sp)
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterType<ValueProvider>(new SingletonLifetimeManager());
            container.RegisterFactory<ConfigBracketParser>(c => new ConfigBracketParser(sp.GetService<IConfiguration>()), new SingletonLifetimeManager());
            container.RegisterType<DependencyResolverOption>(new SingletonLifetimeManager())
                .Resolve<DependencyResolverOption>().AddResolveStrategy<ValueAttribute>((type, attr, sp) =>
              sp.GetRequiredService<ValueProvider>().GetValue(type, attr.Value));

            return container;
        }
    }
}
