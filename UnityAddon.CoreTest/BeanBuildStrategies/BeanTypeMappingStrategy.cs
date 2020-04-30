using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
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
}

namespace UnityAddon.CoreTest
{
    public class BeanTypeMappingStrategy
    {
        [Fact]
        public void ConcreteType()
        {
            var serCol = new ServiceCollection();

            serCol.AddSingleton<IConcrete, Concrete>();

            var factory = new UnityAddonServiceProviderFactory();
            var defCol = factory.CreateBuilder(serCol);
            var usp = factory.CreateServiceProvider(defCol);

            Assert.IsType<Concrete>(usp.GetService<IConcrete>());
        }

        [Fact]
        public void ConcreteGenericType()
        {
            var serCol = new ServiceCollection();

            serCol.AddSingleton<IGeneric<int>, ConcreteGeneric>();

            var factory = new UnityAddonServiceProviderFactory();
            var defCol = factory.CreateBuilder(serCol);
            var usp = factory.CreateServiceProvider(defCol);

            Assert.IsType<ConcreteGeneric>(usp.GetService<IGeneric<int>>());
        }

        [Fact]
        public void GenericType()
        {
            var serCol = new ServiceCollection();

            serCol.AddSingleton(typeof(IGeneric<>), typeof(Generic<>));

            var factory = new UnityAddonServiceProviderFactory();
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

            var factory = new UnityAddonServiceProviderFactory();
            var defCol = factory.CreateBuilder(serCol);
            var usp = factory.CreateServiceProvider(defCol);

            Assert.Equal(usp.GetService<IEnumerable<IEnum>>(), new IEnum[] { usp.GetService<IEnum>("E1"), usp.GetService<IEnum>("E2") });
            Assert.IsType<IEnum[]>(usp.GetService<IEnumerable<IEnum>>());
        }

        [Fact]
        public void EnumerableEmpty()
        {
            var serCol = new ServiceCollection();

            var factory = new UnityAddonServiceProviderFactory();
            var defCol = factory.CreateBuilder(serCol);
            var usp = factory.CreateServiceProvider(defCol);

            Assert.Empty(usp.GetService<IEnumerable<IEnum>>());
            Assert.IsType<IEnum[]>(usp.GetService<IEnumerable<IEnum>>());
        }

        [Fact]
        public void Null()
        {
            var serCol = new ServiceCollection();

            var factory = new UnityAddonServiceProviderFactory();
            var defCol = factory.CreateBuilder(serCol);
            var usp = factory.CreateServiceProvider(defCol);

            Assert.Null(usp.GetService<IEnum>());
        }
    }
}
