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

        public UnityAddonDefaultTest(bool preInstantiateSingleton) : this(Assembly.GetCallingAssembly(), null)
        {
        }

        public UnityAddonDefaultTest(Assembly assembly = null, string testNamespace = null)
        {
            _host = Host.CreateDefaultBuilder()
                .RegisterUA()
                .ScanComponentsUA(assembly ?? Assembly.GetCallingAssembly(), testNamespace ?? GetType().Namespace)
                .BuildUA()
                .RunTestUA(this);
        }

        public void Dispose()
        {
            _host.StopAsync().Wait();
        }
    }
}
