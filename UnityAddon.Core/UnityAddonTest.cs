using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Unity;

namespace UnityAddon.Core
{
    public abstract class UnityAddonTest : IDisposable
    {
        private IUnityContainer _container;

        public UnityAddonTest(bool preInstantiateSingleton) : this(Assembly.GetCallingAssembly(), null, preInstantiateSingleton)
        {
        }

        public UnityAddonTest(Assembly assembly = null, string testNamespace = null, bool preInstantiateSingleton = false)
        {
            _container = new UnityContainer();

            IHostBuilder hostBuilder = Host.CreateDefaultBuilder()
                .RegisterUnityAddon(_container)
                .ScanComponentUnityAddon(assembly ?? Assembly.GetCallingAssembly(), testNamespace ?? GetType().Namespace)
                .InitUnityAddon()
                .EnableTestMode(this);

            if (preInstantiateSingleton)
            {
                hostBuilder.PreInstantiateSingletonUnityAddon();
            }

            hostBuilder.Build();
        }

        public void Dispose()
        {
            _container.Dispose();
        }
    }
}
