using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityAddon.Core.Reflection;
using Xunit;

namespace UnityAddon.CoreTest.Reflection
{
    public interface IComplexService<T, U> { }

    public abstract class AbstractService<T, U> { }

    public class ComplexService<T, U> : AbstractService<T, U>, IComplexService<T, U> { }

    public class MixService<U> : IComplexService<int, U> { }

    public enum TestEnum { }

    public class Nested
    {
        public static class InnerNested
        {
            public interface IInner<T, U> { }

            public class Inner<T, U> : IInner<T, U> { }
        }
    }

    public class TypeResolverTests
    {
        [Fact]
        public void LoadWrongInterface()
        {
            var a = typeof(ComplexService<,>).GetInterfaces().ToArray().First();

            Assert.NotSame(typeof(IComplexService<,>), a);
        }

        [Fact]
        public void LoadInterface()
        {
            var a = typeof(ComplexService<,>).GetInterfaces().ToArray().First();

            Assert.Same(typeof(IComplexService<,>), TypeResolver.LoadType(a));
        }

        [Fact]
        public void LoadMixTypeArgInterface()
        {
            var a = typeof(MixService<>).GetInterfaces().ToArray().First();

            Assert.Same(typeof(IComplexService<,>), TypeResolver.LoadType(a));
        }

        [Fact]
        public void LoadInnerInterface()
        {
            var a = typeof(Nested.InnerNested.Inner<,>).GetInterfaces().ToArray().First();
            Assert.Same(typeof(Nested.InnerNested.IInner<,>), TypeResolver.LoadType(a));
        }

        [Fact]
        public void LoadAbstractClass()
        {
            var a = typeof(ComplexService<,>).BaseType;

            Assert.Same(typeof(AbstractService<,>), TypeResolver.LoadType(a));
        }

        [Fact]
        public void LoadConcreteGeneric()
        {
            Assert.Same(typeof(IComplexService<int, string>), TypeResolver.LoadType(typeof(IComplexService<int, string>)));
        }

        [Fact]
        public void LoadPrimitive()
        {
            Assert.Same(typeof(int), TypeResolver.LoadType(typeof(int)));
        }

        [Fact]
        public void LoadEnum()
        {
            Assert.Same(typeof(TestEnum), TypeResolver.LoadType(typeof(TestEnum)));
        }
    }
}
