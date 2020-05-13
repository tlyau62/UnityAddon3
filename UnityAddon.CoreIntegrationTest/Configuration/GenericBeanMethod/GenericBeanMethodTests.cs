using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.Configuration.GenericBeanMethod
{
    [Configuration]
    public class Config
    {
        [Bean]
        public virtual List<int> IntList()
        {
            return new List<int>();
        }

        [Bean]
        public virtual IEnumerable<string> StrList()
        {
            return new List<string>();
        }
    }

    [ComponentScan]
    public class GenericBeanMethodTests : UnityAddonTest
    {
        [Dependency]
        public List<int> A { get; set; }

        [Dependency]
        public IEnumerable<string> B { get; set; }

        [Fact]
        public void GenericBeanMethod()
        {
            Assert.IsType<List<int>>(A);
            Assert.IsType<List<string>>(B);
        }
    }
}
