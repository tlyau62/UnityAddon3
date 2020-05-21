using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Reflection;
using Xunit;

namespace UnityAddon.CoreTest.Reflection
{
    public abstract class AbstractConfig
    {
        [Bean]
        public abstract int A();

        [Bean]
        public virtual int B() => 1;

        [Bean]
        public abstract int C();

        public abstract int D();

        public virtual int E() => 1;
    }

    public class Config : AbstractConfig
    {
        public override int A() => 1;

        [Bean]
        public override int C() => 1;

        [Bean]
        public override int D() => 1;

        [Bean]
        public override int E() => 1;

        [Bean]
        internal int F() => 1;
    }


    public class MethodSelectorTests
    {
        [Fact]
        public void GetAllMethodsByAttribute_AbstractMethodAnnotated()
        {
            var a = MethodSelector.GetAllMethodsByAttribute<BeanAttribute>(typeof(Config)).Where(m => m.Name == "A");

            Assert.Single(a);
        }

        [Fact]
        public void GetAllMethodsByAttribute_VirtualMethodAnnotated()
        {
            var a = MethodSelector.GetAllMethodsByAttribute<BeanAttribute>(typeof(Config)).Where(m => m.Name == "B");

            Assert.Single(a);
        }

        [Fact]
        public void GetAllMethodsByAttribute_BothMethodAnnotated()
        {
            var a = MethodSelector.GetAllMethodsByAttribute<BeanAttribute>(typeof(Config)).Where(m => m.Name == "C");

            Assert.Single(a);
        }

        [Fact]
        public void GetAllMethodsByAttribute_ImplMethodAnnotated()
        {
            var a = MethodSelector.GetAllMethodsByAttribute<BeanAttribute>(typeof(Config)).Where(m => m.Name == "D");

            Assert.Single(a);
        }

        [Fact]
        public void GetAllMethodsByAttribute_ImplVirtualMethodAnnotated()
        {
            var a = MethodSelector.GetAllMethodsByAttribute<BeanAttribute>(typeof(Config)).Where(m => m.Name == "E");

            Assert.Single(a);
        }

        [Fact]
        public void GetAllMethodsByAttribute_InternalMethod()
        {
            var a = MethodSelector.GetAllMethodsByAttribute<BeanAttribute>(typeof(Config)).Where(m => m.Name == "F");

            Assert.Single(a);
        }
    }
}
