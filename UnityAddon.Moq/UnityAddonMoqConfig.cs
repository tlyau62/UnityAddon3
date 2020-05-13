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
using UnityAddon.Core.Bean;

namespace UnityAddon.Moq
{
    public class UnityAddonMoqConfig
    {
        [Dependency]
        public ApplicationContext ApplicationContext { get; set; }

        [Dependency("UAMoqPartial")]
        public bool Partial { get; set; }

        public UnityAddonTest Testcase { get; set; }

        [PostConstruct]
        public void Setup()
        {
            ApplicationContext.ConfigureContext<DependencyResolverOption>(option =>
            {
                option.AddResolveStrategy<MockAttribute>((type, attr, sp) => sp.GetRequiredService(type));
                option.AddResolveStrategy<TestSubjectAttribute>((type, attr, sp) => sp.GetRequiredService(type));

                if (Partial)
                {
                    option.AddResolveStrategy<DependencyAttribute>((type, attr, sp) => sp.GetService(type, attr.Name));
                }
            });
        }

        [Bean]
        public virtual IBeanDefinitionCollection EnableUnityAddonMoq(UnityAddonTest testcase)
        {
            IBeanDefinitionCollection beanDefCol = new BeanDefinitionCollection();

            testcase.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Select(p => new { p.PropertyType, Attribute = p.GetCustomAttribute<MockAttribute>() })
                .Where(p => p.Attribute != null)
                .ToList()
                .ForEach(p =>
                {
                    if (!(p.PropertyType.IsConstructedGenericType && p.PropertyType.GetGenericTypeDefinition().Equals(typeof(Mock<>))))
                        throw new ArgumentException("property type must be Mock<>");

                    beanDefCol.AddSingleton(p.PropertyType, (sp, type, name) => Activator.CreateInstance(type), null);
                    beanDefCol.AddSingleton(p.PropertyType.GetGenericArguments()[0], (sp, type, name) => ((dynamic)sp.GetRequiredService(p.PropertyType)).Object);
                });

            testcase.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(p => p.GetCustomAttribute<TestSubjectAttribute>() != null)
                .ToList()
                .ForEach(p => beanDefCol.AddSingleton(p.PropertyType, p.PropertyType));

            return beanDefCol;
        }
    }

    public static class UnityAddonMoqExt
    {
        public static void AddUnityAddonMoq(this IBeanRegistry beanRegistry)
        {
            beanRegistry.AddConfiguration<UnityAddonMoqConfig>();
        }
    }
}
