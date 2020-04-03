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

        private IHost _host;

        public UnityAddonTest(bool preInstantiateSingleton) : this(Assembly.GetCallingAssembly(), null, preInstantiateSingleton)
        {
        }

        public UnityAddonTest(Assembly assembly = null, string testNamespace = null, bool preInstantiateSingleton = false)
        {
            _container = new UnityContainer();

            IHostBuilder hostBuilder = Host.CreateDefaultBuilder()
                .RegisterUA(null, _container)
                .ScanComponentUA(assembly ?? Assembly.GetCallingAssembly(), testNamespace ?? GetType().Namespace)
                .InitUA()
                .EnableTestMode(this);

            _host = hostBuilder.Build();

            if (preInstantiateSingleton)
            {
                _host.GetUnityContainer().PreInstantiateSingleton();
            }
        }

        public void Dispose()
        {
            _container.Dispose();
            _host.Dispose();
        }
    }
}
