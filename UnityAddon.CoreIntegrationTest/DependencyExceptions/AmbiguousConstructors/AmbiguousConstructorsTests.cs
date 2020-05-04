using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Exceptions;
using Xunit;

namespace UnityAddon.CoreIntegrationTest.DependencyExceptions.AmbiguousConstructor
{
    public interface IService { }

    [Component]
    public class A : IService { }

    [Component]
    public class B : IService { }

    [Component]
    public class User
    {
        public User(A a)
        {
        }

        public User(B b)
        {
        }
    }

    public class AmbiguousConstructorsTests : DefaultTest
    {
        [Dependency]
        public IServiceProvider Sp { get; set; }

        [Fact]
        public void MultipleSameParametersConstructors()
        {
            var ex = Assert.Throws<BeanCreationException>(() => Sp.GetRequiredService<User>());

            Assert.Equal($"Ambiguous constructors are found{string.Join("", typeof(User).GetConstructors().Select(ctor => "\r\n- " + ctor.ToString()))}", ex.Message);
        }
    }
}
