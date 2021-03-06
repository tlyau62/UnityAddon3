﻿using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Test;
using UnityAddon.Test.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.Bean.TransientBean
{
    [Component]
    [Scope(ScopeType.Transient)]
    public class Helper
    {
    }

    [Component]
    public class Service
    {
        [Dependency]
        public Helper Helper { get; set; }

        [Dependency]
        public Helper Helper2 { get; set; }
    }

    [ComponentScan]
    public class TransientBeanTests : UnityAddonTest
    {
        public TransientBeanTests(UnityAddonTestFixture testFixture) : base(testFixture)
        {
        }

        [Dependency]
        public Service Service { get; set; }

        [Fact]
        public void TransientBean()
        {
            Assert.NotSame(Service.Helper, Service.Helper2);
        }
    }
}
