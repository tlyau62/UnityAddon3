using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core.Bean;

namespace UnityAddon.Core
{
    public interface IServicePostRegistry
    {
        void AddSingleton(Type type, string name);
        void AddSingleton(object instance, string name);
        void AddSingleton(Func<IServiceProvider, object> factory, string name);
    }

    public class ServicePostRegistry : IServicePostRegistry
    {
        [Dependency]
        public ContainerBuilder ContainerBuilder { get; set; }

        public void AddSingleton(Type type, string name)
        {
            var entry = new ContainerBuilderEntry(ContainerBuilderEntryOrder.App, false);

            entry.ConfigureBeanDefinitions(config =>
            {

            });

            throw new NotImplementedException();
        }

        public void AddSingleton(object instance, string name)
        {
            throw new NotImplementedException();
        }

        public void AddSingleton(Func<IServiceProvider, object> factory, string name)
        {
            throw new NotImplementedException();
        }
    }
}
