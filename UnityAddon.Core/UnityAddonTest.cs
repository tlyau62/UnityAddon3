using Castle.Core.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;
using UnityAddon.Core.Context;
using UnityAddon.Core.Util.ComponentScanning;

namespace UnityAddon.Core
{
    public abstract class UnityAddonTest
    {
        public const string CONFIG_PREFIX = "configType_";

        protected IHostBuilder HostBuilder { get; private set; }

        public UnityAddonTest(bool isDefered = false)
        {
            HostBuilder = Host.CreateDefaultBuilder()
                .RegisterUA()
                .ConfigureContainer<ApplicationContext>(ctx =>
                {
                    ctx.ConfigureBeans(config =>
                    {
                        var attrs = GetType().GetAttributes<ConfigArgAttribute>();

                        config.AddSingleton(this);

                        foreach (var attr in attrs)
                        {
                            foreach (var arg in attr.Args)
                            {
                                string key = arg[0] as string;
                                object val = arg[1];
                                Type type = arg[2] as Type;

                                if (key.StartsWith(CONFIG_PREFIX))
                                {
                                    config.AddConfiguration((Type)arg[1]);
                                }
                                else
                                {
                                    if (!type.IsAssignableFrom(val.GetType()))
                                    {
                                        throw new InvalidOperationException("Type mismatch");
                                    }

                                    config.AddSingleton(type, val, key);
                                }
                            }
                        }
                    });
                });

            if (!isDefered)
            {
                HostBuilder.Build()
                    .Services
                    .GetRequiredService<IUnityAddonSP>().BuildUp(GetType(), this);
            }
        }
    }
}
