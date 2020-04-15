using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;

namespace UnityAddon.Moq
{
    public abstract class AbstractUnityAddonMoqTest
    {
        public AbstractUnityAddonMoqTest()
        {
            var host = new HostBuilder()
                .RegisterUA()
                .EnableUnityAddonMoq()
                .BuildUA();

            var container = host.Services.GetService(typeof(IUnityContainer)) as IUnityContainer;
            container.BuildUp(this.GetType(), this);
        }
    }
}
