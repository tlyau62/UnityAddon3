using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.CoreTest.Mocks.BeanTypeMappingStrategy;
using Xunit;

namespace UnityAddon.CoreTest.Mocks.BeanTypeMappingStrategy
{
    public interface IConcrete { }

    public class Concrete : IConcrete { }

    public interface IGeneric<T> { }

    public class ConcreteGeneric : IGeneric<int> { }

    public class Generic<T> : IGeneric<T> { }

    public interface IEnum { }

    public class E1 : IEnum { }

    public class E2 : IEnum { }

    public class E3 : IEnum { }
}

namespace UnityAddon.CoreTest
{
    public class BeanTypeMappingStrategyTests
    {
        [Fact]
        public void ConcreteType()
        {
            var serCol = new ServiceCollection();

            serCol.AddSingleton<IConcrete, Concrete>();

            var factory = new ServiceProviderFactory();
            var defCol = factory.CreateBuilder(serCol);
            var usp = factory.CreateServiceProvider(defCol);

            Assert.IsType<Concrete>(usp.GetService<IConcrete>());
        }

        [Fact]
        public void ConcreteGenericType()
        {
            var serCol = new ServiceCollection();

            serCol.AddSingleton<IGeneric<int>, ConcreteGeneric>();

            var factory = new ServiceProviderFactory();
            var defCol = factory.CreateBuilder(serCol);
            var usp = factory.CreateServiceProvider(defCol);

            Assert.IsType<ConcreteGeneric>(usp.GetService<IGeneric<int>>());
        }

        [Fact]
        public void GenericType()
        {
            var serCol = new ServiceCollection();

            serCol.AddSingleton(typeof(IGeneric<>), typeof(Generic<>));

            var factory = new ServiceProviderFactory();
            var defCol = factory.CreateBuilder(serCol);
            var usp = factory.CreateServiceProvider(defCol);

            Assert.IsType<Generic<string>>(usp.GetService<IGeneric<string>>());
            Assert.IsType<Generic<int>>(usp.GetService<IGeneric<int>>());
        }

        [Fact]
        public void Enumerable()
        {
            var serCol = new ServiceCollection();

            serCol.AddSingleton<IEnum, E1>();
            serCol.AddSingleton<IEnum, E2>();

            var factory = new ServiceProviderFactory();
            var defCol = factory.CreateBuilder(serCol);
            var usp = (IUnityAddonSP)factory.CreateServiceProvider(defCol);

            Assert.Equal(usp.GetService<IEnumerable<IEnum>>(), new IEnum[] { usp.GetService<IEnum>("E1"), usp.GetService<IEnum>("E2") });
            Assert.IsType<IEnum[]>(usp.GetService<IEnumerable<IEnum>>());
        }

        [Fact]
        public void EnumerableWithQualifiers()
        {
            var factory = new ServiceProviderFactory();
            var appCtx = factory.CreateBuilder();

            appCtx.ServiceRegistry.AddSingleton<IEnum, E1>("A");
            appCtx.ServiceRegistry.AddSingleton<IEnum, E2>("B");
            appCtx.ServiceRegistry.AddSingleton<IEnum, E3>("A");

            var usp = (IUnityAddonSP)factory.CreateServiceProvider(appCtx);

            var beannames = usp.GetRequiredService<IBeanDefinitionContainer>().GetAllBeanDefinitions(typeof(IEnum), "A").Select(d => d.Name);
            var beans = beannames.Select(n => usp.GetService<IEnum>(n)).ToArray();

            Assert.IsType<E1>(beans[0]);
            Assert.IsType<E3>(beans[1]);
            Assert.Equal(2, beans.Length);
            Assert.Equal(usp.GetService<IEnumerable<IEnum>>("A"), beans);

            Assert.IsType<IEnum[]>(usp.GetService<IEnumerable<IEnum>>());
        }

        [Fact]
        public void EnumerableEmpty()
        {
            var serCol = new ServiceCollection();

            var factory = new ServiceProviderFactory();
            var defCol = factory.CreateBuilder(serCol);
            var usp = factory.CreateServiceProvider(defCol);

            Assert.Empty(usp.GetService<IEnumerable<IEnum>>());
            Assert.IsType<IEnum[]>(usp.GetService<IEnumerable<IEnum>>());
        }

        [Fact]
        public void Null()
        {
            var serCol = new ServiceCollection();

            var factory = new ServiceProviderFactory();
            var defCol = factory.CreateBuilder(serCol);
            var usp = factory.CreateServiceProvider(defCol);

            Assert.Null(usp.GetService<IEnum>());
        }

        [Fact]
        public void ResolveNull()
        {
            var serCol = new ServiceCollection();

            serCol.AddSingleton<string>(sp => null);

            var factory = new ServiceProviderFactory();
            var defCol = factory.CreateBuilder(serCol);
            var usp = (IUnityAddonSP)factory.CreateServiceProvider(defCol);

            Assert.Null(usp.GetService<string>());
            Assert.True(usp.IsRegistered<string>());
        }
    }
}
