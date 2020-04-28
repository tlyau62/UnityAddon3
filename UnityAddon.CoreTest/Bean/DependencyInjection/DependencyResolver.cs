using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using DepResolver = UnityAddon.Core.Bean.DependencyInjection.DependencyResolver;

namespace UnityAddon.CoreTest.Bean.DependencyInjection
{
    public class TestAttribute : Attribute { }

    public class TestDependencyResolver : DepResolver
    {
    }


    public class DependencyResolver
    {
        [Fact]
        public void Resolve()
        {
            var depResolver = new TestDependencyResolver();
            var fakeObj = new object();
            var objT = typeof(object);
            var testattr = new TestAttribute();
            var mockSp = new Mock<IServiceProvider>().Object;

            depResolver.AddResolveStrategy<TestAttribute>((type, attr, sp) =>
            {
                Assert.Same(objT, type);
                Assert.Same(testattr, attr);
                Assert.Same(mockSp, sp);

                return fakeObj;
            });

            Assert.Same(fakeObj, depResolver.Resolve(objT, new[] { testattr }, mockSp));
        }

        [Fact]
        public void ResolveException()
        {
            var depResolver = new TestDependencyResolver();

            depResolver.AddResolveStrategy<TestAttribute>((type, attr, sp) =>
            {
                throw new InvalidOperationException();
            });

            Assert.Throws<InvalidOperationException>(() => depResolver.Resolve(null, new[] { new TestAttribute() }, new Mock<IServiceProvider>().Object));
        }
    }
}
