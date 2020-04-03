using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;

namespace UnityAddon.Core
{
    public static class UnityAddonHost
    {
        public static IUnityContainer GetUnityContainer(this IHost host)
        {
            return host.Services.GetService(typeof(IUnityContainer)) as IUnityContainer;
        }
    }
}
