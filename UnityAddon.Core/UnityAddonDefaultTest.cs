using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Unity;

namespace UnityAddon.Core
{
    public abstract class UnityAddonDefaultTest
    {
        private IHost _host;

        public UnityAddonDefaultTest(bool preInstantiateSingleton) : this(Assembly.GetCallingAssembly(), null, preInstantiateSingleton)
        {
        }

        public UnityAddonDefaultTest(Assembly assembly = null, string testNamespace = null, bool preInstantiateSingleton = false)
        {
            _host = Host.CreateDefaultBuilder()
                .RegisterUA()
                .ScanComponentsUA(assembly ?? Assembly.GetCallingAssembly(), testNamespace ?? GetType().Namespace)
                .BuildUA()
                .RunTestUA(this);

            if (preInstantiateSingleton)
            {
                var hostContainer = _host.Services.GetService(typeof(IUnityContainer)) as IUnityContainer;

                hostContainer.PreInstantiateSingleton();
            }
        }

        public void Dispose()
        {
            _host.StopAsync().Wait();
        }
    }
}
