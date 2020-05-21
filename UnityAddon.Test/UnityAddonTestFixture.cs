using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityAddon.Core;
using UnityAddon.Core.Context;
using UnityAddon.Test.Attributes;

namespace UnityAddon.Test
{
    public class UnityAddonTestFixture
    {
        public const string CONFIG_PREFIX = "configType_";

        public IHostBuilder TestHostBuilder { get; private set; }

        public IHost TestHost { get; set; }

        public UnityAddonTest UnityAddonTest { get; set; }

        public void Init()
        {
            TestHostBuilder = Host.CreateDefaultBuilder()
                .RegisterUA()
                .ConfigureContainer<ApplicationContext>(ctx =>
                {
                    ctx.ServiceRegistry.ConfigureBeans(config =>
                    {
                        var attrs = UnityAddonTest.GetType().GetCustomAttributes<ConfigArgAttribute>();

                        config.AddSingleton(UnityAddonTest);

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
        }

        public void Build()
        {
            TestHost = TestHostBuilder.Build();
        }

        public void BuildUp()
        {
            TestHost
                .Services
                .GetRequiredService<IUnityAddonSP>()
                .BuildUp(UnityAddonTest.GetType(), UnityAddonTest);
        }
    }
}
