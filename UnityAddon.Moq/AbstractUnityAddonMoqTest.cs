using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Moq;

namespace UnityAddon.Moq
{
    public abstract class AbstractUnityAddonMoqTest
    {
        public AbstractUnityAddonMoqTest() : this(false)
        {
        }

        public AbstractUnityAddonMoqTest(bool partial)
        {
            var host = new HostBuilder()
                .RegisterUA()
                .EnableUnityAddonMoq(this, partial)
                .Build();

            host.Services.BuildUp(GetType(), this);
        }
    }
}
