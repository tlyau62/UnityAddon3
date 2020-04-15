using Microsoft.Extensions.Hosting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Lifetime;
using UnityAddon.Core;
using UnityAddon.Core.DependencyInjection;

namespace UnityAddon.Moq
{
    public static class UnityAddonMoqHostBuilder
    {
        public static IHostBuilder EnableUnityAddonMoq(this IHostBuilder hostBuilder)
        {
            return hostBuilder
                .ConfigureUA<DependencyResolverBuilder>(c =>
                {
                    c.AddResolveStrategy<MockAttribute>((type, attr, c) =>
                    {
                        // type is Mock<T>
                        if (!(type.IsConstructedGenericType && type.GetGenericTypeDefinition().Equals(typeof(Mock<>))))
                            throw new ArgumentException("property type must be Mock<>");

                        dynamic mock = Activator.CreateInstance(type);
                        c.RegisterInstanceUA(type, (object) mock, null);
                        c.RegisterInstanceUA(type.GetGenericArguments()[0], (object) mock.Object, attr.Name);

                        return c.ResolveUA(type);
                    });

                    c.AddResolveStrategy<TestSubjectAttribute>((type, attr, c) =>
                    {
                        c.RegisterTypeUA(null, type, type, new ContainerControlledLifetimeManager());
                        return c.ResolveUA(type);
                    });
                });
        }
    }
}
