using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace UnityAddon.Core
{
    public class UnityAddonTest
    {
        public UnityAddonTest(string testNamespace)
        {
            IHost host = Host.CreateDefaultBuilder()
                .RegisterUnityAddon()
                .ScanComponentUnityAddon(Assembly.GetCallingAssembly(), testNamespace)
                .InitUnityAddon()
                .EnableTestMode(this)
                .Build();

            host.WaitForShutdown();
        }
    }
}
