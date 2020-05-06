using Microsoft.Extensions.Hosting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity;
using Unity.Lifetime;
using UnityAddon.Core;
using UnityAddon.Core.Bean.DependencyInjection;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.Context;
using Microsoft.Extensions.DependencyInjection;
using UnityAddon.Core.BeanDefinition.GeneralBean;
using UnityAddon.Core.Attributes;

namespace UnityAddon.Moq
{
    public static class UnityAddonMoqHostBuilder
    {
        public static IHostBuilder EnableUnityAddonMoq(this IHostBuilder hostBuilder, object testcase, bool partial)
        {
            return hostBuilder
                .ConfigureContainer<ApplicationContext>(ctx =>
                {
                    ctx.ConfigureContext<DependencyResolverOption>(option =>
                    {
                        option.AddResolveStrategy<MockAttribute>((type, attr, sp) => sp.GetRequiredService(type));
                        option.AddResolveStrategy<TestSubjectAttribute>((type, attr, sp) => sp.GetRequiredService(type));

                        if (partial)
                        {
                            option.AddResolveStrategy<DependencyAttribute>((type, attr, sp) => sp.GetService(type, attr.Name));
                        }
                    });

                    ctx.AddContextEntry(entry =>
                    {
                        testcase.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                            .Select(p => new { p.PropertyType, Attribute = p.GetCustomAttribute<MockAttribute>() })
                            .Where(p => p.Attribute != null)
                            .ToList()
                            .ForEach(p =>
                            {
                                if (!(p.PropertyType.IsConstructedGenericType && p.PropertyType.GetGenericTypeDefinition().Equals(typeof(Mock<>))))
                                    throw new ArgumentException("property type must be Mock<>");

                                entry.ConfigureBeanDefinitions(defs =>
                                {
                                    defs.Add(new FactoryBeanDefinition(p.PropertyType, (sp, type, name) => Activator.CreateInstance(type), null, ScopeType.Singleton));
                                    defs.Add(new FactoryBeanDefinition(p.PropertyType.GetGenericArguments()[0], (sp, type, name) => ((dynamic)sp.GetRequiredService(p.PropertyType)).Object, null, ScopeType.Singleton));
                                });
                            });

                        testcase.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                            .Where(p => p.GetCustomAttribute<TestSubjectAttribute>() != null)
                            .ToList()
                            .ForEach(p =>
                            {
                                entry.ConfigureBeanDefinitions(defs => defs.Add(new TypeBeanDefintion(p.PropertyType, p.PropertyType, null, ScopeType.Singleton)));
                            });
                    });
                });
        }
    }
}
