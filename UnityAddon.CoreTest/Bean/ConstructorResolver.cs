using Microsoft.Extensions.DependencyInjection;
using System;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Exceptions;
using UnityAddon.CoreTest.Bean.Mocks.ConstructorResolver;
using Xunit;

namespace UnityAddon.CoreTest.Bean.Mocks.ConstructorResolver
{
    public interface IA { }

    public interface IB { }

    public interface IC { }

    public interface ID { }

    public class A : IA { }

    public class B : IB { }

    public class C : IC { }

    public class Service
    {
        public int Choice { get; set; }

        public Service()
        {
            Choice = 0;
        }

        public Service(IA a, IB b)
        {
            Choice = 1;
        }

        public Service(IA a, IC c)
        {
            Choice = 2;
        }
    }

    public class FailService
    {
        public FailService(ID d)
        {
        }
    }
}

namespace UnityAddon.CoreTest.Bean
{
    public class ConstructorResolver
    {
        [Fact]
        public void NoArgCtor()
        {
            var choice = 0;
            var serCol = new ServiceCollection();

            serCol.AddSingleton<Service>();

            var sp = serCol.BuildServiceProvider();
            var factory = new ServiceProviderFactory();
            var defCol = factory.CreateBuilder(serCol);
            var usp = factory.CreateServiceProvider(defCol);

            Assert.Equal(choice, sp.GetRequiredService<Service>().Choice);
            Assert.Equal(choice, usp.GetRequiredService<Service>().Choice);
        }

        [Fact]
        public void NoArgCtor2()
        {
            var choice = 0;
            var serCol = new ServiceCollection();

            serCol.AddSingleton<Service>();
            serCol.AddSingleton<IA, A>();

            var sp = serCol.BuildServiceProvider();
            var factory = new ServiceProviderFactory();
            var defCol = factory.CreateBuilder(serCol);
            var usp = factory.CreateServiceProvider(defCol);

            Assert.Equal(choice, sp.GetRequiredService<Service>().Choice);
            Assert.Equal(choice, usp.GetRequiredService<Service>().Choice);
        }

        [Fact]
        public void ArgCtor()
        {
            var choice = 1;
            var serCol = new ServiceCollection();

            serCol.AddSingleton<Service>();
            serCol.AddSingleton<IA, A>();
            serCol.AddSingleton<IB, B>();

            var sp = serCol.BuildServiceProvider();
            var factory = new ServiceProviderFactory();
            var defCol = factory.CreateBuilder(serCol);
            var usp = factory.CreateServiceProvider(defCol);

            Assert.Equal(choice, sp.GetRequiredService<Service>().Choice);
            Assert.Equal(choice, usp.GetRequiredService<Service>().Choice);
        }

        [Fact]
        public void ArgCtor2()
        {
            var choice = 2;
            var serCol = new ServiceCollection();

            serCol.AddSingleton<Service>();
            serCol.AddSingleton<IA, A>();
            serCol.AddSingleton<IC, C>();

            var sp = serCol.BuildServiceProvider();
            var factory = new ServiceProviderFactory();
            var defCol = factory.CreateBuilder(serCol);
            var usp = factory.CreateServiceProvider(defCol);

            Assert.Equal(choice, sp.GetRequiredService<Service>().Choice);
            Assert.Equal(choice, usp.GetRequiredService<Service>().Choice);
        }

        [Fact]
        public void AmbiguousCtor()
        {
            var serCol = new ServiceCollection();

            serCol.AddSingleton<Service>();
            serCol.AddSingleton<IA, A>();
            serCol.AddSingleton<IB, B>();
            serCol.AddSingleton<IC, C>();

            var factory = new ServiceProviderFactory();
            var defCol = factory.CreateBuilder(serCol);
            var ex = Assert.Throws<BeanCreationException>(() => factory.CreateServiceProvider(defCol));

            Assert.Equal("Ambiguous constructors are found\r\n- Void .ctor(UnityAddon.CoreTest.Bean.Mocks.ConstructorResolver.IA, UnityAddon.CoreTest.Bean.Mocks.ConstructorResolver.IB)\r\n- Void .ctor(UnityAddon.CoreTest.Bean.Mocks.ConstructorResolver.IA, UnityAddon.CoreTest.Bean.Mocks.ConstructorResolver.IC)", ex.Message);
        }

        [Fact]
        public void NoSatisfiedCtor()
        {
            var serCol = new ServiceCollection();

            serCol.AddSingleton<FailService>();

            var factory = new ServiceProviderFactory();
            var defCol = factory.CreateBuilder(serCol);
            var ex = Assert.Throws<BeanCreationException>(() => factory.CreateServiceProvider(defCol));

            Assert.Equal("Fail to satisfy any of these constructors\r\n- Void .ctor(UnityAddon.CoreTest.Bean.Mocks.ConstructorResolver.ID)", ex.Message);
        }
    }
}


