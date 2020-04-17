using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.DependencyInjection;

namespace UnityAddon.Moq
{
    public abstract class AbstractUnityAddonMoqTest
    {
        public AbstractUnityAddonMoqTest() : this(Assembly.GetCallingAssembly(), false)
        {
        }

        public AbstractUnityAddonMoqTest(bool partial) : this(Assembly.GetCallingAssembly(), partial)
        {
        }

        public AbstractUnityAddonMoqTest(Assembly assembly, bool partial)
        {
            var host = new HostBuilder()
                .RegisterUA()
                .ScanComponentsUA(assembly, this.GetType().Namespace)
                .EnableUnityAddonMoq(this, partial)
                .BuildUA();

            var container = host.Services.GetService(typeof(IUnityContainer)) as IUnityContainer;
            container.BuildUp(this.GetType(), this);
        }
    }
}
