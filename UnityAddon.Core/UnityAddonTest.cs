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
        public UnityAddonTest()
        {
            var hostBuilder = Host.CreateDefaultBuilder().RegisterUA();
            var configAttrs = GetType().GetAttributes<ImportAttribute>();
            var configArgAttrs = GetType().GetAttributes<ConfigArgAttribute>();

            hostBuilder
                .ConfigureContainer<ApplicationContext>(ctx =>
                {
                    ctx.ConfigureBeans((config, sp) =>
                    {
                        foreach (var configAttr in configAttrs)
                        {
                            foreach (var configT in configAttr.Configs)
                            {
                                config.AddConfiguration(configT);
                            }
                        }

                        foreach (var argAttr in configArgAttrs)
                        {
                            if (!argAttr.Type.IsAssignableFrom(argAttr.Value.GetType()))
                            {
                                throw new InvalidOperationException("Type mismatch");
                            }

                            config.AddSingleton(argAttr.Type, argAttr.Value, argAttr.Key);
                        }
                    });
                })
                .Build()
                .Services
                .GetRequiredService<IUnityAddonSP>().BuildUp(GetType(), this);
        }
    }
}
