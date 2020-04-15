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
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.DependencyInjection;

namespace UnityAddon.Moq
{
    public static class UnityAddonMoqHostBuilder
    {
        public static IHostBuilder EnableUnityAddonMoq(this IHostBuilder hostBuilder, object testcase)
        {
            return hostBuilder
                .ConfigureUA<IUnityContainer>(container =>
                {
                    testcase.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                        .Select(p => new { p.PropertyType, Attribute = p.GetCustomAttribute<MockAttribute>() })
                        .Where(p => p.Attribute != null)
                        .ToList()
                        .ForEach(p =>
                        {
                            if (!(p.PropertyType.IsConstructedGenericType && p.PropertyType.GetGenericTypeDefinition().Equals(typeof(Mock<>))))
                                throw new ArgumentException("property type must be Mock<>");

                            container.RegisterFactoryUA(p.PropertyType, null, (container, type, name) => Activator.CreateInstance(type),
                                new ContainerControlledLifetimeManager());

                            container.RegisterFactoryUA(p.PropertyType.GetGenericArguments()[0], p.Attribute.Name,
                                (container, type, name) =>
                                {
                                    dynamic mock = container.ResolveUA(p.PropertyType);
                                    return mock.Object;
                                },
                                new ContainerControlledLifetimeManager());
                        });

                    testcase.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                        .Where(p => p.GetCustomAttribute<TestSubjectAttribute>() != null)
                        .ToList()
                        .ForEach(p =>
                        {
                            container.RegisterTypeUA(null, p.PropertyType, p.PropertyType, new ContainerControlledLifetimeManager());
                        });
                })
                .ConfigureUA<DependencyResolver>(c =>
                {
                    c.AddResolveStrategy<MockAttribute>((type, attr, c) =>
                    {
                        return c.ResolveUA(type);
                    });

                    c.AddResolveStrategy<TestSubjectAttribute>((type, attr, c) =>
                    {   
                        return c.ResolveUA(type);
                    });
                });
        }
    }
}
